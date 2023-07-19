# auth-backend

Authentication backend service with MySql Database.

This is api authorization and authentication service for managin other self hosted services.

## docker compose

```
version: '3.4'
name: auth_backend

services:
  mysqldb:
    image: mysql:latest
    container_name: auth_db
    ports:
      - ${MYSQL_DB_PORT}:3306
    environment:
      - MYSQL_ROOT_PASSWORD=${MYSQL_ROOT_PASSWORD}
      - MYSQL_DATABASE=${MYSQL_DATABASE_NAME}
    restart: unless-stopped

  authbackend:
    image: vitotivadar/auth-backend
    container_name: auth_api
    build:
      context: .
      dockerfile: ./Dockerfile
    ports:
      - ${AUTH_API_PORT}:5000
    environment:
      - AUTH_API_JWT_SECRET=${AUTH_API_JWT_SECRET}
      - MYSQL_DB_PORT=${MYSQL_DB_PORT}
      - MYSQL_ROOT_PASSWORD=${MYSQL_ROOT_PASSWORD}
      - MYSQL_DATABASE_NAME=${MYSQL_DATABASE_NAME}
    restart: unless-stopped
    depends_on:
      - mysqldb
```
