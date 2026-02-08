[![build and test](https://github.com/AuztinC/Csharp-TaskBoard-API/actions/workflows/build-and-test.yaml/badge.svg)](https://github.com/AuztinC/Csharp-TaskBoard-API/actions/workflows/build-and-test.yaml)

# Csharp-TaskBoard-API

Minimal API for a task board with a SQLite backend.

# Run without Docker

From the repo root:

```bash
# Restore NuGet packages for the solution.
dotnet restore

# Run the API project using the default Development settings.
dotnet run --project TaskBoard.Api
```

# If you want to auto-apply EF Core migrations on startup:
Set an environment variable for this run to apply migrations automatically.

```bash
MigrateOnStartup=true dotnet run --project TaskBoard.Api
```

# Run with Docker

From the repo root:

```bash
# Build the API image and start the container with the compose file.
docker compose -f TaskBoard.Api/docker-compose.yaml up --build
```

The API is exposed at `http://localhost:8080`.


```bash
# Stop containers and remove the compose resources.
docker compose -f TaskBoard.Api/docker-compose.yaml down
```

# Working routes you can hit for data:

- `GET /ping`
- `GET /tasks`