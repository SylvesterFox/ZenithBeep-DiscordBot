using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GrechkaBOT.Custom;
using GrechkaBOT.Services;
using Lavalink4NET;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using static GrechkaBOT.Custom.DragonPlayer;

namespace GrechkaBOT.Modeles
{
    public class InteractionModuleMusic : ZenithBase
    {
        public IAudioService LavaNode { private get; set; }
        public PaginationService Pagination { private get; set; }



        [SlashCommand("play", "Playing music", runMode: RunMode.Async)]
        public async Task<RuntimeResult> PlayAsync(string query)
        {
            (DragonPlayer player, ZenithResult result) = await GetPlayer(true);

            if (result is not null)
            {
                return result;
            }

            await DeferAsync();

            LavalinkTrack track = null;

            if (query.Contains("youtube.com") || query.Contains("youtu.be"))
            {
                track = await LavaNode.GetTrackAsync(query);
            }

           

            if (track is null)
            {
                track = await LavaNode.GetTrackAsync(query, SearchMode.YouTube);

                if (track is null)
                {
                    return ZenithResult.FromError("NoResult", "No media was found with that search query.");
                }
                
            }

            QueueInfo info = new QueueInfo(Context.User, Context.Channel);
            track.Context = info;

            

            if (await player.PlayAsync(track, enqueue: true) > 0)
            {

                info.QueueMessage = await FollowupAsync(embed: await track.GetEmbedAsync("Queued"));
                
            } 
            else
            {
                info.Interaction = Context.Interaction;
            }

            return ZenithResult.FromSuccess();
        }

        [SlashCommand("playlist", "Queue an entire playlist")]
        public async Task<RuntimeResult> Playlist(string playlist)
        {
            if (Context.User is not IGuildUser user || user.VoiceChannel is null)
            {
                return ZenithResult.FromError("NoVoiceChannel", "User is not ina voice channel");
            }

            List<LavalinkTrack> tracks = (await LavaNode.GetTracksAsync(playlist)).ToList();

            if (!tracks.Any())
            {
                return ZenithResult.FromError("EmptyPlaylist", "Playlist is empty, private, or non existent");
            }

            (DragonPlayer player, ZenithResult result) = await GetPlayer(true);

            if (result is not null)
            {
                return result;
            }

            await DeferAsync();

            foreach (LavalinkTrack track in tracks)
            {
                track.Context = new QueueInfo(Context.User, Context.Channel);
                player.Queue.Add(track);

            }

            await FollowupAsync("Added all tracks in playlist to queue");

            if (player.State != PlayerState.Playing)
            {
                LavalinkTrack track = player.Queue.Dequeue();
                await player.PlayTopAsync(track);
            }

            return ZenithResult.FromSuccess();
        }

        [SlashCommand("queue", "List track queue")]
        public async Task<RuntimeResult> Queue()
        {
            (DragonPlayer player, ZenithResult result) = await GetPlayer();

            if (result is not null)
            {
                return result;

            }

            if (player.Queue.IsEmpty)
            {
                await RespondAsync(embed: new EmbedBuilder()
                {
                    Title = "Queue",
                    Description = "_... empty ..._",
                    Color = Color.DarkPurple
                }.Build());
            }
            else
            {
                List<EmbedBuilder> builders = player.GetQueuePaged(10).Select(str => new EmbedBuilder().WithDescription(Format.Code(str, "cs"))).ToList();
                await Pagination.SendMeesgeAsync(Context, new PaginationMessage(builders, "Media queue", Color.DarkPurple, Context.User, new AppearanceOptions()
                {
                    Timeout = TimeSpan.FromMinutes(5),
                    Style = DisplayStyle.Full
                }), folloup: false);
            }

            return ZenithResult.FromSuccess();
        }

        [SlashCommand("skip", "Skip track", runMode: RunMode.Async)]
        public async Task<RuntimeResult> Skip()
        {
            (DragonPlayer player, ZenithResult result) = await GetPlayer();

            if (result is not null)
            {
                return result;
            }


            if (IsAdmin() || IsRequester(player.CurrentTrack))
            {
                 
                await player.SkipAsync();

                await SendEmbedAsync(
                        "Skip Track",
                        $"{Context.User.Mention} has skipped the song!",
                        color: Color.Purple
                    );

            } else
            {
                UserVoteSkipInfo info = await player.VoteAsync(Context.User.Id);

                if (info.WasAdded)
                {
                    await SendEmbedAsync(
                            "Vote Skip Track",
                            $"{Context.User.Mention} has voted to skip the current track. ({info.Percentage:P})",
                            color: Color.DarkBlue
                        );
                }
            }

            return ZenithResult.FromSuccess();
            
            
        }

        [SlashCommand("seek", "Seek in track position.", runMode: RunMode.Async)]
        public async Task<RuntimeResult> Seek(TimeSpan time)
        {
            (DragonPlayer player, ZenithResult result) = await GetPlayer();

            if (result is not null)
            {
                return result;
            }

            if (!IsAdmin() && !IsRequester(player.CurrentTrack))
            {
                return ZenithResult.FromError("NotPermissible", "You can't seek this track");
            }

            if (player.State == PlayerState.Playing && player.CurrentTrack is { IsSeekable: true})
            {
                if (time > player.CurrentTrack.Duration)
                {
                    await player.SkipAsync();
                    await SendEmbedAsync("Seeking beyond track length. Skipping.", color: Color.Purple);
                }
                else
                {
                    await player.SeekPositionAsync(time);
                    await SendEmbedAsync("Seek", $"Seeking to: `{time}`", color: Color.Green);
                }

                return ZenithResult.FromSuccess();
            }

            return ZenithResult.FromError("NotSeekable", "This track can't be seeked.");
        }



        [SlashCommand("stop", "Stops the current track", runMode: RunMode.Async)]
        public async Task<RuntimeResult> StopAsync()
        {
            (DragonPlayer player, ZenithResult result) = await GetPlayer();

            if (result is not null)
            {
                return result;
            }


            if (IsAdmin() || IsRequester(player.CurrentTrack) && player.Queue.IsEmpty)
            {
                await player.StopAsync(true);
                await RespondAsync("Cya.");
                return ZenithResult.FromSuccess();
            }

            return ZenithResult.FromError("NotPermissible", "You can't stop this player");
        }

        [SlashCommand("pause", "Pause/Unpause the track")]
        public async Task<RuntimeResult> Pause()
        {
            (DragonPlayer player, ZenithResult result) = await GetPlayer();

            if (result is not null) return result;

            if (!IsAdmin() && !IsRequester(player.CurrentTrack))
            {
                return ZenithResult.FromError("NotPermissible", "You can't pause this track");
            }

            if (player.State == PlayerState.Paused)
            {
                await player.ResumeAsync();
                await SendEmbedAsync("Resumed track", color: Color.Green);
            } else
            {
                await player.PauseAsync();
                await SendEmbedAsync("Pause track.", color: Color.Blue);
            }

            return ZenithResult.FromSuccess();
        }

        [SlashCommand("volume", "Sets the player volume", runMode: RunMode.Async)]
        public async Task Volume(int volume)
        {
            if (volume is > 100 or < 0)
            {
                await RespondAsync("Volume out of range: 0% - 100%");
                return;
            }

            (DragonPlayer player, ZenithResult result) = await GetPlayer();

   
            await player.SetVolumeAsync(volume / 100f);
            await RespondAsync($"Volume update: {volume}");
        }

        [SlashCommand("clear", "Clear track queue.")]
        public async Task<RuntimeResult> ClearQueue()
        {
            (DragonPlayer player, ZenithResult result) = await GetPlayer();


            if (result is not null) return result;

            player.Queue.Clear();
            await RespondAsync("Cleared the queue");

            return ZenithResult.FromSuccess();
        }

        [SlashCommand("remove", "Remove track from queue.")]
        public async Task<RuntimeResult> Remove(int index)
        {
            (DragonPlayer player, ZenithResult result) = await GetPlayer();


            if (result is not null) return result;

            if (--index > player.Queue.Count - 1 || 0 > index)
            {
                return ZenithResult.FromUserError("InvaildIndex", "Index is out of  queue range");
            }

            LavalinkTrack track = player.Queue[index];

            if (!IsAdmin() && !IsRequester(track))
            {
                return ZenithResult.FromError("NotPermissible", "You can't renove that track from the queue.");
            }

            await RespondAsync(embed: await track.GetEmbedAsync("Removed"));
            player.Queue.RemoveAt(index);

            return ZenithResult.FromSuccess();
        }

        [SlashCommand("track", "Currently playing track")]
        public async Task<RuntimeResult> Track()
        {
            (DragonPlayer player, ZenithResult result) = await GetPlayer();

            if (result is not null)
            {
                return result;
            }

            if (player.State == PlayerState.NotPlaying)
            {
                return ZenithResult.FromError("NoTrack", "Noting is currenttly playing");
            }

            await RespondAsync(embed: await player.CurrentTrack.GetEmbedAsync());
            return ZenithResult.FromSuccess();
        }

        [SlashCommand("looptrack", "Loop current track")]
        public async Task<RuntimeResult> LoopTrack()
        {
            (DragonPlayer player, ZenithResult result) = await GetPlayer();

            if (result is not null)
            {
                return result;
            }

            if (player.State == PlayerState.NotPlaying)
            {
                return ZenithResult.FromError("NoTrack", "Noting is currenttly playing");
            }

            var isLooping = player.LoopMode is not PlayerLoopMode.None;
            player.LoopMode = isLooping ? PlayerLoopMode.None : PlayerLoopMode.Track;
            isLooping = !isLooping;

            if (isLooping)
            {
                await SendEmbedAsync("Track is looping.", color: Color.Green);
            } 
            else
            {
                await SendEmbedAsync("Track not is looping.", color: Color.Blue);
            }
            return ZenithResult.FromSuccess();
        }

        [SlashCommand("shuffle", "Shuffle track queue.")]
        public async Task<RuntimeResult> Shuffle()
        {
            (DragonPlayer player, ZenithResult result) = await GetPlayer();

            if (result is not null)
            {
                return result;
            }

            player.Queue.Shuffle();
            await SendEmbedAsync("Shuffled.", color: Color.Green);

            return ZenithResult.FromSuccess();
        }



        private async ValueTask<(DragonPlayer, ZenithResult)> GetPlayer(bool autoConnect = true)
        {
            DragonPlayer player = LavaNode.GetPlayer<DragonPlayer>(Context.Guild.Id);

            if (Context.User is not IGuildUser user || user.VoiceChannel is null)
            {
                return (null, ZenithResult.FromError("NoVoiceChannel", "User is not in a voice channel"));
            }

            if (player?.VoiceChannelId.HasValue == false)
            {
                await player.DisposeAsync();
                player = null;
            }

            if (player == null)
            {
                if (autoConnect)
                {
                    player = await LavaNode.JoinAsync<DragonPlayer>(Context.Guild.Id, user.VoiceChannel.Id, true);
                } else
                {
                    return (null, ZenithResult.FromError("NotConnected", "No active bot session"));
                }
            }

            if (user.VoiceChannel.Id != player.VoiceChannelId)
            {
                return (null, ZenithResult.FromError("ChannelMismatch", "You are not in the same voice channel"));
            }

            return (player, null);
        }

        private bool IsRequester(LavalinkTrack track)
        {
            return track?.Context is QueueInfo info && info.User.Id == Context.User.Id;
        }

        private bool IsAdmin()
        {
            return Context.User is SocketGuildUser { GuildPermissions: { Administrator: true } or { ManageGuild: true } };
        }



    }
      
}