# ===========================
# 🧩 Stage 1: Build app
# ===========================
FROM node:20 AS build
WORKDIR /app

# Copy trực tiếp
COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

# ===========================
# Stage 2: Nginx serve
# ===========================
FROM nginx:1.27-alpine

# Copy build output
COPY --from=build /app/dist /usr/share/nginx/html

# Copy nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
