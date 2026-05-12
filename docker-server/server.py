from http.server import BaseHTTPRequestHandler, HTTPServer
import json
import os
from datetime import datetime

LOG_DIR = "/logs"
os.makedirs(LOG_DIR, exist_ok=True)

class LogHandler(BaseHTTPRequestHandler):
    def do_POST(self):
        length = int(self.headers["Content-Length"])
        body = self.rfile.read(length)
        entry = json.loads(body)

        date = datetime.now().strftime("%Y-%m-%d")
        path = os.path.join(LOG_DIR, f"{date}.json")

        logs = []
        if os.path.exists(path):
            with open(path, "r") as f:
                try:
                    logs = json.load(f)
                except:
                    logs = []

        logs.append(entry)

        with open(path, "w") as f:
            json.dump(logs, f, indent=2)

        self.send_response(200)
        self.end_headers()

    def log_message(self, format, *args):
        pass

HTTPServer(("0.0.0.0", 5050), LogHandler).serve_forever()