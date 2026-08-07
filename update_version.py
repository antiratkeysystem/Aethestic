import paramiko

host = "94.26.106.244"
username = "root"
password = "VbDkQ8!KUEbmAAYz"

try:
    client = paramiko.SSHClient()
    client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
    client.connect(host, username=username, password=password, timeout=10)

    # read index.html
    stdin, stdout, stderr = client.exec_command("cat /opt/aesthetic/Panel/public/index.html")
    content = stdout.read().decode('utf-8')

    # replace app.js with versioned
    import time
    version = int(time.time())
    content = content.replace('src="app.js"', f'src="app.js?v={version}"')

    # write back
    sftp = client.open_sftp()
    with sftp.file('/opt/aesthetic/Panel/public/index.html', 'w') as f:
        f.write(content)
    sftp.close()

    print(f"[+] updated app.js version to {version}")

    client.close()

except Exception as e:
    print(f"[-] error: {e}")
