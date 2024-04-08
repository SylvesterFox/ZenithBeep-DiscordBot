
namespace ZenithBeep.Settings
{
    public class SettingsManager
    {

        public BotConfig LoadedConfig = new BotConfig();

        public const string BOT_NAME = "ZenithBeep";
        public const string ENV_FILE = ".env";

        public readonly string BotDataDirectory;

        private SettingsManager() { 
            
            BotDataDirectory = Directory.GetCurrentDirectory();
        }


        public bool LoadConfiguration()
        {
            string configFilePath = Path.Combine(BotDataDirectory, ENV_FILE);
            try
            {
                Directory.CreateDirectory(BotDataDirectory);
                if (!File.Exists(configFilePath)) return false;
            } catch (Exception ex)
            {
                return false;
            }

            LoadedConfig = EnvSerializer.Deserialize(configFilePath);
            return LoadedConfig != null;
        }

        private static SettingsManager? _PrivateInstance;
        public static SettingsManager Instance
        {
            get
            {
                return _PrivateInstance ??= new SettingsManager();
            }
        }
    
    }
}
