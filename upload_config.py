import paramiko

host = "94.26.106.244"
username = "root"
password = "VbDkQ8!KUEbmAAYz"

local_file = r"Z:\Aetseticasss\Aethestic\Panel\stub\config.json"
remote_file = "/opt/aesthetic/Panel/stub/config.json"

try:
    client = paramiko.SSHClient()
    client.set_missing_host_key_policy(paramiko.AutoAddPolicy())

    print(f"[*] connecting to {host}...")
    client.connect(host, username=username, password=password, timeout=10)
    print(f"[+] connected\n")

    # backup old config
    stdin, stdout, stderr = client.exec_command(f"cp {remote_file} {remote_file}.backup")
    print("[*] backed up old config")

    # upload new config
    sftp = client.open_sftp()
    print(f"[*] uploading {local_file} -> {remote_file}")
    sftp.put(local_file, remote_file)
    sftp.close()
    print("[+] uploaded successfully\n")

    # verify
    stdin, stdout, stderr = client.exec_command(f"cat {remote_file}")
    print("[*] new config content:")
    print(stdout.read().decode())

    client.close()

except Exception as e:
    print(f"[-] error: {e}")
