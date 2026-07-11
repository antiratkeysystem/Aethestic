import os
import sys
import subprocess

def install_dependencies():
    print("[*] Checking dependencies...")
    try:
        import flask
        import requests
        print("[+] Dependencies already installed.")
    except ImportError:
        print("[*] Dependencies missing. Installing from requirements.txt...")
        try:
            subprocess.check_call([sys.executable, "-m", "pip", "install", "-r", "requirements.txt"])
            print("[+] Installation successful!")
        except Exception as e:
            print(f"[-] Failed to install dependencies: {e}")
            sys.exit(1)

def run_server():
    print("[*] Starting local server...")
    try:
        # Run Flask server
        subprocess.call([sys.executable, "app.py"])
    except KeyboardInterrupt:
        print("\n[*] Server stopped by user.")
    except Exception as e:
        print(f"[-] Server error: {e}")

if __name__ == "__main__":
    # Change working directory to Panel folder
    os.chdir(os.path.dirname(os.path.abspath(__file__)))
    install_dependencies()
    run_server()
