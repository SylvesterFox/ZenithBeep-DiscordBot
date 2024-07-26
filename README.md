![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)  ![Discord.net](https://img.shields.io/nuget/vpre/Discord.Net.svg?maxAge=2592000?style=plasti) ![Static Badge](https://img.shields.io/badge/lavalink-net) 

# ZenitBeep

<div align="center" width="100%">
<img src="https://i.imgur.com/ovLUlWm.png" alt="logo" width="150" height="150" align="center">
</div>

# About

My personal Discord bot, which was rewritten from my old work under .NET-8
The project started on October 27, 2022. Under the old name LunaBot, especially for the IB workshop server

Briefly about the bot, at the moment it can create private voice channels, has a system for issuing roles and, of course, the bot has musical capabilities and can play music from YouTube in a voice channel.
Moderation will be available soon

# How deploying ZenitBeep

## Through the docker container
1. Clone this repository
2. Change your bot token in the configuration file, along the path `Config/appsetings.yml`

```js
Bot:
    Token: <Your token bot>
    Logs: debug
    LavaHost: lavalink-v3
    LavaPassword: youshallnotpass
    AudioService: true
```

3. Next we write to the terminal at the root of the repository itself
```bash
docker-compose up -d 
```
	And we wait for the docker to finish deploying the bot.

4. The bot is now working.
