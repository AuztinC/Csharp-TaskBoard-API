[![build and test](https://github.com/AuztinC/Csharp-TaskBoard-API/actions/workflows/build-and-test.yaml/badge.svg)](https://github.com/AuztinC/Csharp-TaskBoard-API/actions/workflows/build-and-test.yaml)

# Csharp-TaskBoard-API

A **full-stack task board application** built with:

- **ASP.NET Core Minimal API**
- **Entity Framework Core + SQLite**
- **Vite + modern frontend tooling**
- **Docker & Docker Compose**
- **GitHub Actions CI**
- **Fly.io deployment**

The project focuses on:

- Clean project structure

- Automated builds and tests

- Database migrations

- Local and containerized development

Live deployment: 

Fly.io trial ended :(

## Running Locally (No Docker)

When running locally **without Docker**, the API and frontend are run as
**separate processes**.

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

## Frontend (Vite)

In a separate terminal:
```bash
cd taskboard-ui
npm install
npm run dev
```
The frontend will be avilable at:

`http://localhost:5173`

During local development, the frontend communicates with the API directly.

## Running with Docker (Recommended)

Docker runs **both the API and frontend together** and exposes the app
through a **single localhost port**.

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

The full application is exposed at `http://localhost:8080`.

This setup:

- runs the API and UI together

- persists SQLite data via a volume

- mirrors production behavior more closely than local dev

Stop containers
```bash
docker-compose down
```

# API Endpoints
| Method        | Route             | Description    |
|--------------:|-------------------|----------------|
|GET            | `/api/ping`       | Health Check   |
|GET            | `/api/tasks`      |List all tasks  |
|GET            | `/api/tasks/{id}` |Get task by ID  |
|POST           | `/api/tasks`      |Create a task   |
|PUT            | `/api/tasks/{id}` |Update a task   |
|DELETE         | `/api/tasks/{id}` |Delete a task   |

## ***Notes***

SQLite data is persisted when running via Docker.

EF Core migrations are applied automatically when enabled.

CI runs build and tests on every push and pull request.

The project is designed as a clean reference implementation for a small
but real full-stack system.