import paramiko

host = "94.26.106.244"
username = "root"
password = "VbDkQ8!KUEbmAAYz"

remote_file = "/opt/aesthetic/Panel/public/index.html"
local_file = r"Z:\Aetseticasss\Aethestic\index_downloaded.html"

try:
    client = paramiko.SSHClient()
    client.set_missing_host_key_policy(paramiko.AutoAddPolicy())

    print(f"[*] connecting to {host}...")
    client.connect(host, username=username, password=password, timeout=10)
    print(f"[+] connected\n")

    sftp = client.open_sftp()
    print(f"[*] downloading {remote_file} -> {local_file}")
    sftp.get(remote_file, local_file)
    sftp.close()
    print("[+] downloaded successfully")

    client.close()

except Exception as e:
    print(f"[-] error: {e}")
