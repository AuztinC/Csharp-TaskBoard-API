[![build and test](https://github.com/AuztinC/Csharp-TaskBoard-API/actions/workflows/build-and-test.yaml/badge.svg)](https://github.com/AuztinC/Csharp-TaskBoard-API/actions/workflows/build-and-test.yaml)

# Csharp-TaskBoard-API

A Minimal API built with ASP.NET Core that exposes a simple task board backed by SQLite and Entity Framework Core.

The project focuses on:

- Clean project structure

- Automated builds and tests

- Database migrations

- Local and containerized development

Live deployment: 

https://csharp-taskboard-api.fly.dev/ping

# Run without Docker

From the repo root:

Restore NuGet packages for the solution.
```bash
dotnet restore
```

Run the API project using the default Development settings.
```bash
dotnet run --project TaskBoard.Api
```

## Auto-Apply EF Core Migrations
To automatically apply database migrations on startup, set the following environment variable:

```bash
MigrateOnStartup=true dotnet run --project TaskBoard.Api
```

# Run with Docker

From the repo root:

- Build the API image and start the container with the compose file.
```bash
cd TaskBoard.Api
docker-compose up --build
```
 Once a Docker container has been built with the last command.

```
docker-compose up
```

The API is exposed at `http://localhost:8080`.


- Stop containers
```bash
docker-compose down
```
-  and remove the compose resources.
```bash
docker compose -f TaskBoard.Api/docker-compose.yaml down
```

# Working routes you can hit for data:

- `GET /ping`
- `GET /tasks`
- `GET /tasks/{id}`
- `POST /tasks`
- `PUT /tasks/{id}`
- `DELETE /tasks/{id}`