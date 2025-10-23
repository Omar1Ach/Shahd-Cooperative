# Docker Setup Guide

This guide explains how to run the ShahdCooperative API using Docker.

## Prerequisites

- Docker Desktop installed
- Docker Compose installed (comes with Docker Desktop)

## Services Included

The docker-compose setup includes:

1. **SQL Server 2022** - Database server
   - Port: 1433
   - SA Password: `YourStrong@Passw0rd`

2. **Elasticsearch 8.11** - Log storage
   - Port: 9200 (HTTP API)
   - Port: 9300 (Transport)

3. **Kibana 8.11** - Log visualization dashboard
   - Port: 5601
   - Access: http://localhost:5601

4. **ShahdCooperative API** - .NET 8 Web API
   - Port: 5000
   - Swagger: http://localhost:5000/swagger
   - Health Check: http://localhost:5000/health

## Quick Start

### 1. Start All Services

```bash
cd C:\Users\Predator\ShahdCooperative
docker-compose up -d
```

This will:
- Build the API image
- Pull SQL Server, Elasticsearch, and Kibana images
- Start all services in detached mode

### 2. Check Service Status

```bash
docker-compose ps
```

All services should show as "Up" and "healthy"

### 3. View Logs

```bash
# View all logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f api
docker-compose logs -f sqlserver
docker-compose logs -f elasticsearch
```

### 4. Initialize Database

On first run, you need to create the database and run migrations:

```bash
# Access SQL Server container
docker exec -it shahdcooperative-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C

# Create database
CREATE DATABASE ShahdCooperative;
GO

# Exit sqlcmd
quit
```

Or run your existing SQL scripts to set up tables.

## Accessing Services

### API Endpoints

- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health
- **API Base URL**: http://localhost:5000/api

### Kibana Dashboard

1. Open http://localhost:5601
2. Wait for Kibana to initialize (first time takes 1-2 minutes)
3. Go to "Discover" to view logs
4. Create an index pattern: `shahdcooperative-logs-*`

### SQL Server

- **Host**: localhost
- **Port**: 1433
- **User**: sa
- **Password**: YourStrong@Passw0rd
- **Database**: ShahdCooperative

Connect using SQL Server Management Studio or Azure Data Studio

## Stopping Services

```bash
# Stop all services
docker-compose down

# Stop and remove volumes (WARNING: This deletes all data!)
docker-compose down -v
```

## Rebuilding the API

If you make code changes:

```bash
# Rebuild and restart the API service
docker-compose up -d --build api
```

## Troubleshooting

### API won't start

Check if SQL Server is ready:
```bash
docker-compose logs sqlserver
```

### Elasticsearch issues

Increase Docker memory to at least 4GB in Docker Desktop settings

### Connection issues

Make sure no other services are using these ports:
- 1433 (SQL Server)
- 5000 (API)
- 5601 (Kibana)
- 9200 (Elasticsearch)

### View API errors

```bash
docker-compose logs api --tail=100
```

## Production Deployment

For production, update `docker-compose.yml`:

1. Change SQL Server password
2. Update JWT secret key
3. Enable Elasticsearch security (xpack.security.enabled=true)
4. Use environment-specific appsettings
5. Set up proper SSL certificates
6. Configure reverse proxy (nginx/traefik)

## Environment Variables

Key environment variables you can override:

```bash
# SQL Server
MSSQL_SA_PASSWORD=YourStrong@Passw0rd

# API
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=...
JwtSettings__SecretKey=...
ElasticsearchSettings__Uri=http://elasticsearch:9200
```

## Backup and Restore

### SQL Server Backup

```bash
docker exec shahdcooperative-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -C \
  -Q "BACKUP DATABASE ShahdCooperative TO DISK='/var/opt/mssql/backup/ShahdCooperative.bak'"
```

### Copy backup from container

```bash
docker cp shahdcooperative-sqlserver:/var/opt/mssql/backup/ShahdCooperative.bak ./backup/
```

## Clean Up

Remove everything (containers, volumes, networks):

```bash
docker-compose down -v
docker system prune -a
```
