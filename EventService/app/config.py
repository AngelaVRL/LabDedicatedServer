import os

HOST = os.environ.get("EVENT_SERVICE_HOST", "0.0.0.0")
PORT = int(os.environ.get("EVENT_SERVICE_PORT", "5000"))