FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY ./ ./
RUN dotnet restore 
RUN dotnet publish ./ZenithBeep/ZenithBeep.csproj -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app
COPY --from=build-env /app/out .
COPY ./init.sh .
RUN chmod +x ./init.sh 
ENTRYPOINT [ "./init.sh" ]

