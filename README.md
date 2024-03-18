[![Postgres](https://img.shields.io/badge/postgres-%23316192.svg?style=for-the-badge&logo=postgresql&logoColor=white)](#postgres)
[![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)](#.Net)
[![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)](c#)
[![Discord.net](https://img.shields.io/nuget/vpre/Discord.Net.svg?maxAge=2592000?style=plasti)](#Discord.Net)
[![Static Badge](https://img.shields.io/badge/lavalink-net)](https://github.com/angelobreuer/Lavalink4NET)

# <center>ZenitBeep</center>
![Logo](https://i.imgur.com/ovLUlWm.png)
img[alt=logo] { width: 150px; }

# About
My personal Discord bot, which was rewritten from my old work under .NET-8
The project started on October 27, 2022. Under the old name LunaBot, especially for the IB workshop server

Briefly about the bot, at the moment it can create private voice channels, has a system for issuing roles and, of course, the bot has musical capabilities and can play music from YouTube in a voice channel.
Moderation will be available soon

# How deploying ZenitBeep

## Through the docker container

In the repository folder create the `.env` file

```bash
TOKEN=<TOKEN>
LOGS=info
AUDIOSERVICE=true


LAVALINK_ADDRESS=http://localhost:2333
LAVALINK_WEBSOCKET=ws://localhost:2333/v4/websocket
LAVALINK_PASSWORD=youshallnotpass


POSTGRES_HOST=db
POSTGRES_DB=zenith
POSTGRES_USER=docker
POSTGRES_PORT=5432
POSTGRES_PASSWORD=PASSWORDTEST


ConnectionStrings__db=Host=${POSTGRES_HOST};Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}


PG_DATA=/var/lib/pgsql/pgdata
```

