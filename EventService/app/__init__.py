from flask import Flask

from app.routes.events import create_events_blueprint
from app.storage.event_store import EventStore


def create_app():
    app = Flask(__name__)
    store = EventStore()
    app.register_blueprint(create_events_blueprint(store))
    return app