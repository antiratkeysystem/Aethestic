import sqlite3
from werkzeug.security import generate_password_hash
import secrets

db_path = '/opt/aesthetic/Panel/database.db'
conn = sqlite3.connect(db_path)
c = conn.cursor()

# Ensure table exists
c.execute('''
    CREATE TABLE IF NOT EXISTS users (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        username TEXT UNIQUE NOT NULL,
        password_hash TEXT NOT NULL,
        role TEXT DEFAULT 'user',
        api_key TEXT UNIQUE NOT NULL,
        is_banned INTEGER DEFAULT 0,
        display_name TEXT,
        avatar TEXT,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    )
''')

# Insert admin account
pass_hash = generate_password_hash('admin123')
api_k = secrets.token_hex(16)

c.execute("INSERT OR REPLACE INTO users (id, username, password_hash, role, api_key) VALUES (1, 'admin', ?, 'admin', ?)", (pass_hash, api_k))
conn.commit()
conn.close()
print("Admin account created successfully: admin / admin123")
