﻿
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using ZenithBeep.Modeles;
using ZenithBeep.Player;
using ZenithBeepData;

namespace ZenithBeep;

public class IntaractionModuleMusic : MusicCmd
{
    public IntaractionModuleMusic(IAudioService audio, MusicZenithHelper zenithHelper) : base(audio, zenithHelper)
    {
    }

    [SlashCommand("join", "Connecting in the voice chat")]
    public async Task Join() {
        await JoinAsync(Context);
    }

    [SlashCommand("leave", "Leave voice channel")]
    public async Task Leave() {
        await LeaveAsync(Context);
    }
}
