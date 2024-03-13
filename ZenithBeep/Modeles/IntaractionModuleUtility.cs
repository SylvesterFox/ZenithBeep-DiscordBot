﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Serilog;
using ZenithBeepData;
using RuntimeResult = Discord.Interactions.RuntimeResult;


namespace ZenithBeep.Modeles
{
    public class IntaractionModuleUtility : ZenithBase
    {
        private IConfigurationRoot _config;
        
        public IntaractionModuleUtility(DataAccessLayer accessLayer, IConfigurationRoot config) : base(accessLayer) 
        {
            _config = config;
        }
        

        [SlashCommand("beep", "Ping command")]
        public async Task<RuntimeResult> PingCommand() {
           await RespondAsync("boop!! :ping_pong:");
           var msg = await GetOriginalResponseAsync();
           await msg.ModifyAsync(msg => msg.Content = $"pong.. :ping_pong: \n ping: {Context.Client.Latency}ms");
           Log.Debug($"test {_config["TEST"]}");
           return ZenithResult.FromSuccess();
        }

        [SlashCommand("avatar", "Get user avatar")]
        public async Task<RuntimeResult> UserAvatar(SocketUser? user = null)
        {
            await DeferAsync();
            var _user = user ?? Context.User;

            var author_embed = new EmbedAuthorBuilder();
            author_embed.WithName($"Photo profile: {_user.Username}");
            author_embed.WithIconUrl(Context.User.GetAvatarUrl());
            await SendEmbedAsync(description: $"Photo profile [link]({_user.GetAvatarUrl(size: 1024)})", imageUrl: _user.GetAvatarUrl(size: 1024), author: author_embed);
            return ZenithResult.FromSuccess();
        }
    }
}
