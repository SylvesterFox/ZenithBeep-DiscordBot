using Discord.WebSocket;

namespace ZenithBeep;

public class GetExtension {
    private readonly DiscordSocketClient _discordClient;
    
    public GetExtension(DiscordSocketClient client)
    {
        _discordClient = client;
    }

    private SocketGuild _guild;
    public SocketGuild GetGuild(ulong GuildId)
    {
        return _guild ??= _discordClient.GetGuild(GuildId);
    }

}

public static class ClientExtension
{
    private static SocketGuildUser _member;

    public static SocketGuildUser GetMember(this DiscordSocketClient client, ulong userId, ulong guildId)
    {
        var get = new GetExtension(client);
        SocketGuild _guild = get.GetGuild(guildId);
        _member ??= _guild.GetUser(userId);
        return _member;
    }

}
