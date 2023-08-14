using Discord;
using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;

namespace GrechkaBOT.Modeles
{
    public class InteractionModuleMusic : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IAudioService _audioService;

        public InteractionModuleMusic(IAudioService audioService) 
            => _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));

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
                var player = await GetPlayerAsync();
                await ReplyAsync($"Joined {voiceChannel.VoiceChannel.Name}!");

            } catch (Exception ex)
            {
                await RespondAsync(ex.Message);
            }

        }

        [SlashCommand("play", "Playing music", runMode: RunMode.Async)]
        public async Task PlayAsync(string query)
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            var track = await _audioService.GetTrackAsync(query, SearchMode.YouTube);

            if (track == null)
            {
                await RespondAsync("😖 No results");
                return;
            }

            var position = await player.PlayAsync(track, enqueue: true);

            if (position == 0)
            {
                
                await RespondAsync("Loading..");
                await DeleteOriginalResponseAsync();
                await ReplyAsync($"🔈 Playing: {track.Title}");
                
            } 
            else
            {
                await RespondAsync("Loading..");
                await DeleteOriginalResponseAsync();
                await ReplyAsync($"🔈 Added to queue: {track.Title}");
                return;
            }
        }

        [SlashCommand("skip", "Skip track", runMode: RunMode.Async)]
        public async Task AsyncSkip()
        {
            var player = _audioService.GetPlayer<VoteLavalinkPlayer>(Context.Guild.Id);

            if (player == null)
            {
                return;
            }

            if (player.CurrentTrack == null)
            {
                await RespondAsync("Nothing playing!");
                return;
            }

            await RespondAsync($"{player.CurrentTrack.Title} skip!");
            await player.SkipAsync();
            
        }

        [SlashCommand("disconnect", "Disconnects from the current voice channel connect to", runMode: RunMode.Async)]
        public async Task Disconnect()
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

       
            await player.StopAsync(true);
            await RespondAsync("Disconnected.");
        }

        /// <summary>
        ///     Stop the current track
        /// </summary>
        /// <returns>a task that represents the stop track</returns>

        [SlashCommand("stop", "Stops the current track", runMode: RunMode.Async)]
        public async Task StopAsync()
        {
            var player = await GetPlayerAsync(connectToVoiceChannel: false);

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

            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;

            }

            await player.SetVolumeAsync(volume / 100f);
            await RespondAsync($"Volume update: {volume}");
        }

        private async ValueTask<VoteLavalinkPlayer> GetPlayerAsync(bool connectToVoiceChannel = true)
        {
            var player = _audioService.GetPlayer<VoteLavalinkPlayer>(Context.Guild.Id);

            if (player != null && player.State != PlayerState.NotConnected && player.State != PlayerState.Destroyed)
            {
                return player;
            }

            var user = Context.Guild.GetUser(Context.User.Id);

            if (!user.VoiceState.HasValue)
            {
                await ReplyAsync("You must be in a voice channel!");
                return null;
            }

            if (!connectToVoiceChannel)
            {
                await ReplyAsync("The bot is not a voice channel!");
                return null;
            }

            return await _audioService.JoinAsync<VoteLavalinkPlayer>(user.Guild.Id, user.VoiceChannel.Id);
        }

      
     
    }
      
}