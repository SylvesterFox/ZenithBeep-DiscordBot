using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Tracks;
using ZenithBeep.Custom;

namespace ZenithBeep.Player
{
    public class MusicEvents
    {
        public readonly IAudioService _audioService;

        public MusicEvents(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public async Task ButtonSearchClicked(SocketInteraction component)
        {
            var arg = (SocketMessageComponent)component;
            if(!arg.Data.CustomId.StartsWith("btn_srch"))
            {
                return;
            }

            if (int.TryParse(arg.Data.CustomId[^1].ToString(), out int index) ) {
                var playlist = await _audioService.Players.GetPlayerAsync<ZenithPlayer>(arg.GuildId.Value);
                if (playlist != null )
                {
                    LavalinkTrack? track = playlist.GetSearchResults()[index-1];
                    if (track != null)
                    {
                        await playlist.PlayAsync(track).ConfigureAwait(false);
                        playlist.SearchResults = new LavalinkTrack?[5];

                        await arg.Message.DeleteAsync().ConfigureAwait(false);
                        var msg = await arg.Channel.SendMessageAsync(embed: CustomEmbeds.UniEmbed($"Select track: {track.Title}"));
                        await Task.Delay(15000);
                        await msg.DeleteAsync().ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
