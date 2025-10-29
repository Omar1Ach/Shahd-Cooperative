# Docker Image Build Guide - Step by Step

## Prerequisites

### 1. Install Docker Desktop
1. Go to https://www.docker.com/products/docker-desktop
2. Download Docker Desktop for Windows
3. Run the installer
4. Restart your computer if prompted
5. Launch Docker Desktop
6. Wait for Docker to start (you'll see the whale icon in system tray)
7. Verify installation:
   ```bash
   docker --version
   docker-compose --version
   ```

---

## Building Docker Images

### Method 1: Build Single Service Image

#### Step 1: Navigate to Project Directory
```bash
cd C:\Users\Predator\ShahdCooperative
```

#### Step 2: Build the Docker Image
```bash
docker build -t shahdcooperative-authservice:latest .
```

**What this does:**
- `docker build` - Command to build an image
- `-t shahdcooperative-authservice:latest` - Tags the image with name and version
- `.` - Tells Docker to use current directory's Dockerfile

**Expected Output:**
```
[+] Building 45.2s (16/16) FINISHED
 => [internal] load build definition from Dockerfile
 => => transferring dockerfile: 1.23kB
 => [internal] load .dockerignore
 => [internal] load metadata for mcr.microsoft.com/dotnet/aspnet:8.0
 => [internal] load metadata for mcr.microsoft.com/dotnet/sdk:8.0
 => [stage-2 1/3] FROM mcr.microsoft.com/dotnet/aspnet:8.0
 => [build 1/7] FROM mcr.microsoft.com/dotnet/sdk:8.0
 => [internal] load build context
 => CACHED [build 2/7] WORKDIR /src
 => [build 3/7] COPY [ShahdCooperative.AuthService.API/...
 => [build 4/7] RUN dotnet restore
 => [build 5/7] COPY . .
 => [build 6/7] WORKDIR /src/ShahdCooperative.AuthService.API
 => [build 7/7] RUN dotnet build
 => [publish 1/1] RUN dotnet publish
 => [stage-2 2/3] WORKDIR /app
 => [stage-2 3/3] COPY --from=publish /app/publish .
 => exporting to image
 => => exporting layers
 => => writing image sha256:abc123...
 => => naming to docker.io/library/shahdcooperative-authservice:latest
```

#### Step 3: Verify Image Was Built
```bash
docker images | grep shahdcooperative
```

**Expected Output:**
```
shahdcooperative-authservice   latest    abc123def456   2 minutes ago   220MB
```

#### Step 4: Test Run the Image
```bash
docker run -d -p 5000:8080 --name test-authservice shahdcooperative-authservice:latest
```

**What this does:**
- `-d` - Run in detached mode (background)
- `-p 5000:8080` - Map port 5000 on host to port 8080 in container
- `--name test-authservice` - Give container a name
- `shahdcooperative-authservice:latest` - Image to run

#### Step 5: Check if Container is Running
```bash
docker ps
```

**Expected Output:**
```
CONTAINER ID   IMAGE                                  COMMAND                  CREATED          STATUS          PORTS                    NAMES
abc123def456   shahdcooperative-authservice:latest   "dotnet ShahdCooper…"   10 seconds ago   Up 9 seconds   0.0.0.0:5000->8080/tcp   test-authservice
```

#### Step 6: Test the API
Open browser and go to: http://localhost:5000/swagger

#### Step 7: View Container Logs
```bash
docker logs test-authservice
```

#### Step 8: Stop and Remove Test Container
```bash
docker stop test-authservice
docker rm test-authservice
```

---

### Method 2: Build All Services with Docker Compose

#### Step 1: Navigate to Project Directory
```bash
cd C:\Users\Predator\ShahdCooperative
```

#### Step 2: Build All Services
```bash
docker-compose build
```

**What this does:**
- Reads `docker-compose.yml`
- Builds all services defined in the file
- Uses cached layers when possible

**Expected Output:**
```
[+] Building 67.3s (25/25) FINISHED
 => [api internal] load build definition from Dockerfile
 => [api internal] load .dockerignore
 => [api internal] load metadata for mcr.microsoft.com/dotnet/sdk:8.0
 => [api build 1/7] FROM mcr.microsoft.com/dotnet/sdk:8.0
 => [api build 2/7] WORKDIR /src
 => [api build 3/7] COPY [ShahdCooperative.AuthService.API/...
 => [api build 4/7] RUN dotnet restore
 => [api build 5/7] COPY . .
 => [api build 6/7] WORKDIR /src/ShahdCooperative.AuthService.API
 => [api build 7/7] RUN dotnet build
 => [api publish 1/1] RUN dotnet publish
 => [api stage-2 1/3] FROM mcr.microsoft.com/dotnet/aspnet:8.0
 => [api stage-2 2/3] WORKDIR /app
 => [api stage-2 3/3] COPY --from=publish /app/publish .
 => [api] exporting to image
 => => exporting layers
 => => writing image
 => => naming to docker.io/library/shahdcooperative_api
```

#### Step 3: Start All Services
```bash
docker-compose up -d
```

**What this does:**
- `-d` - Run in detached mode
- Starts SQL Server, RabbitMQ, Elasticsearch, Kibana, and API
- Creates network between containers
- Creates volumes for data persistence

**Expected Output:**
```
[+] Running 5/5
 ✔ Network shahdcooperative-network           Created
 ✔ Container shahdcooperative-sqlserver       Started
 ✔ Container shahdcooperative-elasticsearch   Started
 ✔ Container shahdcooperative-rabbitmq        Started
 ✔ Container shahdcooperative-kibana          Started
 ✔ Container shahdcooperative-api             Started
```

#### Step 4: Check All Containers Status
```bash
docker-compose ps
```

**Expected Output:**
```
NAME                              IMAGE                                    STATUS              PORTS
shahdcooperative-api              shahdcooperative_api                     Up (healthy)        0.0.0.0:5000->8080/tcp
shahdcooperative-sqlserver        mcr.microsoft.com/mssql/server:2022      Up (healthy)        0.0.0.0:1433->1433/tcp
shahdcooperative-rabbitmq         rabbitmq:3.12-management-alpine          Up (healthy)        0.0.0.0:5672->5672/tcp, 0.0.0.0:15672->15672/tcp
shahdcooperative-elasticsearch    docker.elastic.co/elasticsearch:8.11.0   Up (healthy)        0.0.0.0:9200->9200/tcp
shahdcooperative-kibana           docker.elastic.co/kibana:8.11.0          Up (healthy)        0.0.0.0:5601->5601/tcp
```

#### Step 5: View Logs for All Services
```bash
docker-compose logs -f
```

**To view specific service logs:**
```bash
docker-compose logs -f api
docker-compose logs -f sqlserver
docker-compose logs -f rabbitmq
```

#### Step 6: Test Services

**API:**
- Open: http://localhost:5000/swagger

**RabbitMQ Management:**
- Open: http://localhost:15672
- Login: admin/admin123

**Kibana:**
- Open: http://localhost:5601

**SQL Server:**
- Server: localhost,1433
- Database: ShahdCooperative
- User: sa
- Password: YourStrong!Passw0rd

#### Step 7: Stop All Services
```bash
docker-compose down
```

**To stop and remove volumes (clean all data):**
```bash
docker-compose down -v
```

---

## Troubleshooting

### Issue 1: Docker Desktop Not Starting
**Solution:**
1. Open Task Manager
2. End all Docker processes
3. Restart Docker Desktop as Administrator
4. Wait 2-3 minutes for initialization

### Issue 2: Port Already in Use
**Error:** `bind: address already in use`

**Solution:**
```bash
# Find what's using the port
netstat -ano | findstr :5000

# Kill the process (replace PID with actual process ID)
taskkill /F /PID <PID>
```

### Issue 3: Build Fails - Out of Disk Space
**Solution:**
```bash
# Remove unused images
docker image prune -a

# Remove unused volumes
docker volume prune

# Remove all stopped containers
docker container prune
```

### Issue 4: Container Not Healthy
**Check health:**
```bash
docker inspect shahdcooperative-sqlserver | grep -A 10 Health
```

**Solution:**
```bash
# Restart the container
docker-compose restart sqlserver

# Check logs for errors
docker-compose logs sqlserver
```

### Issue 5: SQL Server Password Error
**Error:** `Password validation failed`

**Solution:**
- Ensure password meets SQL Server requirements (8+ chars, uppercase, lowercase, number, special char)
- Current password in docker-compose.yml: `YourStrong!Passw0rd`

---

## Advanced Commands

### Rebuild Without Cache
```bash
docker-compose build --no-cache
```

### Scale Services
```bash
docker-compose up -d --scale api=3
```

### View Resource Usage
```bash
docker stats
```

### Execute Commands in Running Container
```bash
# Access container shell
docker exec -it shahdcooperative-api bash

# Run SQL Server query
docker exec -it shahdcooperative-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -Q "SELECT name FROM sys.databases"
```

### Export and Import Images

**Export:**
```bash
docker save shahdcooperative-authservice:latest -o authservice.tar
```

**Import:**
```bash
docker load -i authservice.tar
```

### Push to Registry

**Docker Hub:**
```bash
# Login
docker login

# Tag image
docker tag shahdcooperative-authservice:latest yourusername/shahdcooperative-authservice:latest

# Push
docker push yourusername/shahdcooperative-authservice:latest
```

**GitHub Container Registry:**
```bash
# Login
echo YOUR_GITHUB_TOKEN | docker login ghcr.io -u YOUR_USERNAME --password-stdin

# Tag image
docker tag shahdcooperative-authservice:latest ghcr.io/yourusername/shahdcooperative-authservice:latest

# Push
docker push ghcr.io/yourusername/shahdcooperative-authservice:latest
```

---

## Production Best Practices

1. **Use specific version tags, not `latest`**
   ```bash
   docker build -t shahdcooperative-authservice:v1.0.0 .
   ```

2. **Multi-stage builds** (already implemented in Dockerfile)
   - Reduces final image size
   - Separates build and runtime dependencies

3. **Use .dockerignore** (already created)
   - Speeds up builds
   - Reduces image size

4. **Health checks** (already in docker-compose.yml)
   - Ensures containers are actually working
   - Enables automatic restarts

5. **Environment variables for secrets**
   ```bash
   docker run -e JWT_SECRET=$(cat secret.txt) shahdcooperative-authservice
   ```

6. **Regular cleanup**
   ```bash
   # Weekly cleanup script
   docker system prune -a --volumes -f
   ```
