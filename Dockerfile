FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY ./ ./
RUN dotnet publish ./ZenithBeep/ZenithBeep.csproj -o ../app/publish/build
RUN chmod 755 ../app/publish/build




