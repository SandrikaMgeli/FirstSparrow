# Use the official .NET 9 SDK image as build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY FirstSparrow.Api/FirstSparrow.Api.csproj FirstSparrow.Api/
COPY FirstSparrow.Application/FirstSparrow.Application.csproj FirstSparrow.Application/
COPY FirstSparrow.Infrastructure/FirstSparrow.Infrastructure.csproj FirstSparrow.Infrastructure/
COPY FirstSparrow.Persistence/FirstSparrow.Persistence.csproj FirstSparrow.Persistence/

# Restore NuGet packages
RUN dotnet restore FirstSparrow.Api/FirstSparrow.Api.csproj

# Copy the entire source code
COPY . .

# Build the application
WORKDIR /src/FirstSparrow.Api
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use the official .NET 9 runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=build /app/publish .

# Create a non-root user for security
RUN addgroup --system --gid 1001 appuser && \
    adduser --system --uid 1001 --gid 1001 appuser

# Change ownership of the app directory
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "FirstSparrow.Api.dll"]
