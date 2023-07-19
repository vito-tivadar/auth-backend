#Vito Tivadar - 2023

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

ENV AUTH_API_JWT_SECRET="always_change_this_env_var"
ENV MYSQL_ROOT_PASSWORD="root"
ENV MYSQL_DATABASE_NAME="authdb"
ENV MYSQL_DB_PORT=3306


# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
#install EF Tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /src
COPY ["auth-backend.csproj", "./"]
RUN dotnet restore "auth-backend.csproj"
COPY . .
WORKDIR "/src/."
#RUN dotnet build "auth-backend.csproj" -c Release -o /app/build

#Build project and apply migraitons 
RUN dotnet ef migrations add auth_migration --configuration Release -o /app/build

FROM build AS publish
RUN dotnet publish "auth-backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "auth-backend.dll"]
