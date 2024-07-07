namespace ZenithBeep;

public class BotOptions
{
    public string Bot = "Bot";
    public string Token { get; set; } = string.Empty;
    public string Logs { get; set; } = "Info";
    public string LavaHost { get; set; } = "localhost";
    public string LavaPassword { get; set; } = "youshallnotpass";
    public bool AudioService { get; set; } = true;

    public bool DBSetup { get; set; } = true;
}
