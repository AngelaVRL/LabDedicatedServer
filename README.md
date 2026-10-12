# Dedicated Server Lab

Unity 6 client for two network synchronization approaches over HTTP polling:

- **Part 1:** state replication. Each client publishes its position and polls the remote player's position from a reference service.
- **Part 2:** event propagation. Clients send game events (lever room scenario) to a custom event service and fetch only the events they have not received yet.

## Repository structure

```
Assets/Scripts/
  Config/       ServerSettings
  Movement/     LocalPlayerController
  Networking/   Position and event API clients and models
  Events/       Event publisher, poller and reactions
  Experiments/  Experiments A, B and C
  Visuals/      PlayerColorAssigner
EventService/   Event service (Flask, Docker)
Results/        CSV results of experiments A, B and C
```

## Requirements

- Unity 6 6000.3.8f1
- Docker Desktop with Docker Compose v2
- Git

The project uses the `com.unity.nuget.newtonsoft-json` package. Unity restores it automatically when the project is opened.

## Unity project setup

1. Open the repository root with Unity Hub.
2. Open the scene `TO_COMPLETE`.
3. Verify that Allow downloads over HTTP is set to Always allowed in Edit > Project Settings > Player > Other Settings.
4. Verify that Run In Background is enabled in Player > Resolution and Presentation.

## Services

### Position service (Part 1)

The reference service is not included in this repository.

```
git clone https://github.com/memin2522/DedicatedServer-Api
cd DedicatedServer-Api
docker compose up --build
```

Available at `http://localhost:5005`. Swagger UI is at `http://localhost:5005/swagger-ui`.

### Event service (Part 2)

```
cd EventService
docker compose up --build
```

Available at `http://localhost:5006`. See `EventService/README.md` for the endpoints.

## Running two instances

Use one build and the editor. Each instance needs crossed player ids in the `ServerSettings` component of the `NetworkManager` object:

| Instance | Local Player Id | Remote Player Id |
|---|---|---|
| A (build) | `playerA` | `playerB` |
| B (editor) | `playerB` | `playerA` |

Both instances must use the same `Game Id`. The polling interval is set in `Poll Interval Ms`.

Player A is always blue and player B is always red, configured in `PlayerColorAssigner`.

## Controls

| Key | Action |
|---|---|
| WASD or arrows | Move the local player |
| L | Toggle the lever (`LeverToggled`) |
| O | Open the door (`DoorOpened`) |
| K | Collect the key (`KeyCollected`) |

Part 1 uses the objects `PositionPublisher` and `PositionPoller`. Part 2 uses the `EventManager` object.

## Experiments

Each experiment runs on its own components. Disable `PositionPublisher`, `PositionPoller` and `EventManager` while running them, and use a fresh Play session for each experiment.

| Experiment | Component | Procedure | Output |
|---|---|---|---|
| A: latency and load | `LatencyExperiment` | Runs on its own at Play. Wait for the `Results saved` message. | `experimentA.csv` |
| B: sampling | `SamplingBurstPublisher` (A), `SamplingObserverExperiment` (B) | Press N on B, then press P on A within 2 seconds. Repeat for each interval. | `experimentB.csv` |
| C: event propagation | `EventBurstPublisher` (A), `EventObserverExperiment` (B) | Press M on B, then press B on A within 2 seconds. Repeat for each interval. | `experimentC.csv` |

The CSV files are saved in the `Application.persistentDataPath` folder printed in the console. The results used in the report are in `Results/`.

## Videos
Demonstration video (Part 1 and Part 2): https://youtu.be/1cmneG0IpJY
