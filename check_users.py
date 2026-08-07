import paramiko

ssh = paramiko.SSHClient()
ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
ssh.connect('94.26.106.244', username='root', password='VbDkQ8!KUEbmAAYz')

stdin, stdout, stderr = ssh.exec_command('python3 -c "import sqlite3; conn = sqlite3.connect(\'/opt/aesthetic/Panel/database.db\'); print(conn.execute(\'SELECT id, username, role FROM users\').fetchall())"')
print('Users on VDS:', stdout.read().decode())

ssh.close()
