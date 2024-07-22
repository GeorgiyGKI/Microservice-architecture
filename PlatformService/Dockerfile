# Используем официальный образ .NET 8 SDK для этапа сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Копируем csproj файл и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем все файлы и публикуем приложение
COPY . ./
RUN dotnet publish -c Release -o out

# Используем официальный образ ASP.NET Core 8 Runtime для этапа запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Команда для запуска приложения
ENTRYPOINT ["dotnet", "PlatformService.dll"]