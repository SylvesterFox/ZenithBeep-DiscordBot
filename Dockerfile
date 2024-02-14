FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY ./ ./
RUN dotnet publish ./GrechkaBOT/GrechkaBOT.csproj -o ../app/publish/build
RUN chmod 755 ../app/publish/build
ARG ENV
RUN echo ${ENV}
RUN sed -i 's/^\(token: \).*$/\1'${ENV}'/' ./publish/build/appsettings.yml | cat ./publish/build/appsettings.yml

