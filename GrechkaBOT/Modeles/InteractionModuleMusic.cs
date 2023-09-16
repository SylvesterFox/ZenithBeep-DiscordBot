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
    public class InteractionModuleMusic : InteractionModuleBase<SocketInteractionContext>
    {
        public IAudioService LavaNode { private get; set; }
        public PaginationService Pagination { private get; set; }



        [SlashCommand("join", "Play music", runMode: RunMode.Async)]
        public async Task JoinAsync()
        {
            var guildId = Context.Guild.Id;
            var voiceChannel = Context.User as IVoiceState;

            if (voiceChannel?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel");
                return;
            }
            
            try
            {
                var player = await GetPlayer();
                await ReplyAsync($"Joined {voiceChannel.VoiceChannel.Name}!");

            } catch (Exception ex)
            {
                await RespondAsync(ex.Message);
            }

        }

        [SlashCommand("play", "Playing music", runMode: RunMode.Async)]
        public async Task<RuntimeResult> PlayAsync(string query)
        {
            (DragonPlayer player, GrechkaResult result) = await GetPlayer(true);

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
                    return GrechkaResult.FromError("NoResult", "No media was found with that search query.");
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

            return GrechkaResult.FromSuccess();
        }

        [SlashCommand("playlist", "Queue an entire playlist")]
        public async Task<RuntimeResult> Playlist(string playlist)
        {
            if (Context.User is not IGuildUser user || user.VoiceChannel is null)
            {
                return GrechkaResult.FromError("NoVoiceChannel", "User is not ina voice channel");
            }

            List<LavalinkTrack> tracks = (await LavaNode.GetTracksAsync(playlist)).ToList();

            if (!tracks.Any())
            {
                return GrechkaResult.FromError("EmptyPlaylist", "Playlist is empty, private, or non existent");
            }

            (DragonPlayer player, GrechkaResult result) = await GetPlayer(true);

            if (result is not null)
            {
                return result;
            }

            await DeferAsync();

            foreach (LavalinkTrack track in tracks)
            {
                track.Context = new QueueInfo(Context.User, Context.Channel);
                player.Queue.Add(track);

                // Test log console
                Console.WriteLine(track.Title);
            }

            await FollowupAsync("Added all tracks in playlist to queue");

            if (player.State != PlayerState.Playing)
            {
                LavalinkTrack track = player.Queue.Dequeue();
                await player.PlayTopAsync(track);
            }

            return GrechkaResult.FromSuccess();
        }

        [SlashCommand("queue", "List track queue")]
        public async Task<RuntimeResult> Queue()
        {
            (DragonPlayer player, GrechkaResult result) = await GetPlayer();

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

            return GrechkaResult.FromSuccess();
        }

        [SlashCommand("skip", "Skip track", runMode: RunMode.Async)]
        public async Task<RuntimeResult> Skip()
        {
            (DragonPlayer player, GrechkaResult result) = await GetPlayer();

            if (result is not null)
            {
                return result;
            }

            var embedSkip = new EmbedBuilder();

            if (IsAdmin() || IsRequester(player.CurrentTrack))
            {
                
                await RespondAsync($"{player.CurrentTrack.Title} skip!");
                await player.SkipAsync();

            } else
            {
                UserVoteSkipInfo info = await player.VoteAsync(Context.User.Id);

                if (info.WasAdded)
                {
                    await RespondAsync($"{Context.User.Mention} has voted to skip the current track. ({info.Percentage:P})");
                }
            }

            return GrechkaResult.FromSuccess();
            
            
        }

        [SlashCommand("disconnect", "Disconnects from the current voice channel connect to", runMode: RunMode.Async)]
        public async Task<RuntimeResult> Leave()
        {
            (DragonPlayer player, GrechkaResult result) = await GetPlayer();


            if (result is not null)
            {
                return result;
            }

            await player.StopAsync(true);
            await RespondAsync("Disconnected.");
            return GrechkaResult.FromSuccess();
        }

        /// <summary>
        ///     Stop the current track
        /// </summary>
        /// <returns>a task that represents the stop track</returns>

        [SlashCommand("stop", "Stops the current track", runMode: RunMode.Async)]
        public async Task StopAsync()
        {
            (DragonPlayer player, GrechkaResult result) = await GetPlayer();

            if (player == null)
            {
                return;
            }

            if (player.CurrentTrack == null)
            {
                await RespondAsync("Nothing playing!");
                return;
            }

            await player.StopAsync();
            await RespondAsync("Stopped playing.");
        }

        [SlashCommand("volume", "Sets the player volume", runMode: RunMode.Async)]
        public async Task Volume(int volume)
        {
            if (volume is > 100 or < 0)
            {
                await RespondAsync("Volume out of range: 0% - 100%");
                return;
            }

            (DragonPlayer player, GrechkaResult result) = await GetPlayer();

   
            await player.SetVolumeAsync(volume / 100f);
            await RespondAsync($"Volume update: {volume}");
        }

        private async ValueTask<(DragonPlayer, GrechkaResult)> GetPlayer(bool autoConnect = true)
        {
            DragonPlayer player = LavaNode.GetPlayer<DragonPlayer>(Context.Guild.Id);

            if (Context.User is not IGuildUser user || user.VoiceChannel is null)
            {
                return (null, GrechkaResult.FromError("NoVoiceChannel", "User is not in a voice channel"));
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
                    return (null, GrechkaResult.FromError("NotConnected", "No active bot session"));
                }
            }

            if (user.VoiceChannel.Id != player.VoiceChannelId)
            {
                return (null, GrechkaResult.FromError("ChannelMismatch", "You are not in the same voice channel"));
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