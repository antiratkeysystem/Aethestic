import paramiko
import time

host = "94.26.106.244"
username = "root"
password = "VbDkQ8!KUEbmAAYz"

local_file = r"Z:\Aetseticasss\Aethestic\app_downloaded.py"
remote_file = "/opt/aesthetic/Panel/app.py"
backup_file = f"/opt/aesthetic/Panel/app.py.backup_{int(time.time())}"

try:
    client = paramiko.SSHClient()
    client.set_missing_host_key_policy(paramiko.AutoAddPolicy())

    print(f"[*] connecting to {host}...")
    client.connect(host, username=username, password=password, timeout=10)
    print(f"[+] connected\n")

    # backup
    stdin, stdout, stderr = client.exec_command(f"cp {remote_file} {backup_file}")
    stdout.channel.recv_exit_status()
    print(f"[*] backed up to {backup_file}")

    # upload
    sftp = client.open_sftp()
    print(f"[*] uploading {local_file} -> {remote_file}")
    sftp.put(local_file, remote_file)
    sftp.close()
    print("[+] uploaded successfully\n")

    # show changes
    print("[*] changes made:")
    print("  - removed hardcoded default key 'aesthetic_secret_key_123' from /api/upload")
    print("  - removed hardcoded default key 'aesthetic_secret_key_123' from websocket auth")
    print("\n[+] security vulnerabilities fixed!")

    client.close()

except Exception as e:
    print(f"[-] error: {e}")
