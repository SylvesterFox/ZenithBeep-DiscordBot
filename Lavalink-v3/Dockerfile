FROM openjdk:17-oracle
WORKDIR /lavalink-v3
COPY ./Lavalink-v3.jar ./
COPY ./init.sh ./
COPY ./application.yml ./
RUN chmod +x ./init.sh
CMD [ "./init.sh" ]