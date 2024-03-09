# Указываем базовый образ для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем файлы проекта и восстанавливаем зависимости
COPY . .

# Собираем приложение
RUN dotnet publish ./ZenithBeep/ZenithBeep.csproj -c Release -o out

# Указываем образ для запуска приложения
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем скомпилированные файлы приложения из предыдущего образа
COPY --from=build /app/out .

COPY .env ./

# Указываем точку входа для запуска приложения
ENTRYPOINT ["dotnet", "ZenithBeep.dll"]