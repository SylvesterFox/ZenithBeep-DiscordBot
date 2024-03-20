# Указываем базовый образ для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем файлы проекта и восстанавливаем зависимости
COPY . .

# Устанавливаем переменную среды для использования EF Core
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true

# Устанавливаем EF Core CLI глобально
RUN dotnet tool install --global dotnet-ef

RUN apt-get update && apt-get install -y postgresql-client

ENV PATH="$PATH:/root/.dotnet/tools"

# Выполняем миграции базы данных
RUN dotnet ef migrations add InitialCreate --context BeepDbContext --output-dir Migrations

RUN chmod +x ./migrate.sh

RUN sed -i 's/\r//g' .env

# # Собираем приложение
# RUN dotnet build -c Release -o out

# # Указываем образ для запуска приложения
# FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime

# Устанавливаем рабочую директорию
# WORKDIR /app

# # Копируем скомпилированные файлы приложения из предыдущего образа
# COPY --from=build /app/out .

# # Указываем точку входа для запуска приложения
# ENTRYPOINT ["dotnet", "YourDatabaseManagementApp.dll"]
