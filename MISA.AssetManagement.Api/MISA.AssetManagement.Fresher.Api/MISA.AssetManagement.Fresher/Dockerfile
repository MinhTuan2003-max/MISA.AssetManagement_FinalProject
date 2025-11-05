# ===========================
# Stage 1: Build
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY MISA.AssetManagement.Fresher.Api.sln ./

# Copy all project files
COPY MISA.AssetManagement.Core/MISA.Core.csproj ./MISA.AssetManagement.Core/
COPY MISA.Infrastructure/MISA.Infrastructure.csproj ./MISA.Infrastructure/
COPY MISA.AssetManagement.Fresher/MISA.AssetManagement.Fresher.csproj ./MISA.AssetManagement.Fresher/

# Restore dependencies
RUN dotnet restore MISA.AssetManagement.Fresher/MISA.AssetManagement.Fresher.csproj

# Copy all source code
COPY MISA.AssetManagement.Core/ ./MISA.AssetManagement.Core/
COPY MISA.Infrastructure/ ./MISA.Infrastructure/
COPY MISA.AssetManagement.Fresher/ ./MISA.AssetManagement.Fresher/

# Build and publish (removed --no-restore)
WORKDIR /src/MISA.AssetManagement.Fresher
RUN dotnet publish -c Release -o /app/publish

# ===========================
# Stage 2: Runtime
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variable
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MISA.AssetManagement.Fresher.dll"]
