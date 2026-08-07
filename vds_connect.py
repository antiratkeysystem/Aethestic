import paramiko
import sys

host = "94.26.106.244"
username = "root"
password = "VbDkQ8!KUEbmAAYz"

try:
    client = paramiko.SSHClient()
    client.set_missing_host_key_policy(paramiko.AutoAddPolicy())

    print(f"[*] connecting to {host}...")
    client.connect(host, username=username, password=password, timeout=10)
    print(f"[+] connected successfully\n")

    if len(sys.argv) > 1:
        command = " ".join(sys.argv[1:])
    else:
        command = "whoami && pwd && ls -la"

    print(f"[*] executing: {command}")
    stdin, stdout, stderr = client.exec_command(command)

    output = stdout.read().decode()
    error = stderr.read().decode()

    if output:
        print(f"\n[output]:\n{output}")
    if error:
        print(f"\n[error]:\n{error}")

    client.close()

except Exception as e:
    print(f"[-] error: {e}")
    sys.exit(1)
