# MISA Asset Management - Docker Quick Start

## Yêu Cầu
- Docker Desktop 20.10+
- Docker Compose 2.0+
- 4GB RAM, 10GB disk space

## Chạy Nhanh

### 1. Clone & Setup
git clone <repository-url>
cd MISA.AssetManagement_FinalProject

text

### 2. Tạo .dockerignore
Tạo file `.dockerignore` trong `MISA.AssetManagement.Api/MISA.AssetManagement.Fresher/`:
**/bin/
**/obj/
**/.vs/
**/.vscode/
*.user

text

### 3. Build & Chạy
Build và start tất cả services
 - docker-compose build --no-cache
 - docker-compose up -d

Kiểm tra status
 - docker-compose ps

Xem logs
 - docker-compose logs -f

### 4. Truy Cập
- Frontend: http://localhost:3000
- Backend API: http://localhost:8080/api
- Swagger: http://localhost:8080/swagger

## Các Lệnh Thường Dùng

Dừng tất cả
 - docker-compose down

Dừng + xóa volumes
 - docker-compose down -v

Xem logs real-time
 - docker-compose logs -f backend

Restart service cụ thể
 - docker-compose restart backend

Truy cập container
 - docker exec -it misa_backend bash
