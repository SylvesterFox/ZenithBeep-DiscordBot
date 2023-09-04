using Discord;
using Discord.WebSocket;
using Lavalink4NET.Artwork;
using Lavalink4NET.Events;
using Lavalink4NET.Player;


namespace GrechkaBOT.Custom
{
    internal sealed class DragonPlayer : VoteLavalinkPlayer
    {
        private string? _length;

        public class QueueInfo
        {
            public IUser User { get; }
            public ISocketMessageChannel Channel { get; }
            public IDiscordInteraction Interaction { get; set; }
            public IMessage? QueueMessage { get; set; }
            public IMessage? Message { get; set; }

            public QueueInfo(IUser user, ISocketMessageChannel channel)
            {
                User = user;
                Channel = channel;
            }
        }


        public override async Task OnTrackStartedAsync(TrackStartedEventArgs e)
        {
            if (State == PlayerState.Playing && CurrentTrack?.Context is QueueInfo info)
            {


                if (info.Interaction != null)
                {
                    info.Message = await info.Interaction.FollowupAsync(embed: await CurrentTrack.GetEmbedAsync("Playuing"));
                }
                else
                {
                    info.Message = await info.Channel.SendMessageAsync(embed: await CurrentTrack.GetEmbedAsync("Playing")).ConfigureAwait(false);
                }

                if (info.QueueMessage is not null)
                {
                    await info.QueueMessage.DeleteAsync();
                }
            }

            await base.OnTrackStartedAsync(e);
        }




        public override async Task OnTrackEndAsync(TrackEndEventArgs eventArgs)
        {
            if (eventArgs.Reason == TrackEndReason.Replaced)
            {
               

                if (CurrentTrack?.Context is QueueInfo { Message: { } } info)
                {
                    await info.Message.DeleteAsync();
                }


            }

            await base.OnTrackEndAsync(eventArgs);
        }





    }

}
