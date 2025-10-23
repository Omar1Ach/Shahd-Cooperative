# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["ShahdCooperative.API/ShahdCooperative.API.csproj", "ShahdCooperative.API/"]
COPY ["ShahdCooperative.Application/ShahdCooperative.Application.csproj", "ShahdCooperative.Application/"]
COPY ["ShahdCooperative.Domain/ShahdCooperative.Domain.csproj", "ShahdCooperative.Domain/"]
COPY ["ShahdCooperative.Infrastructure/ShahdCooperative.Infrastructure.csproj", "ShahdCooperative.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "ShahdCooperative.API/ShahdCooperative.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/ShahdCooperative.API"
RUN dotnet build "ShahdCooperative.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ShahdCooperative.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ShahdCooperative.API.dll"]
