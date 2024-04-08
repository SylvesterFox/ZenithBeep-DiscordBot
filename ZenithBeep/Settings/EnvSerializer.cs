

namespace ZenithBeep.Settings
{

    public static class EnvSerializer
    {
        public static BotConfig Deserialize(string filePath)
        {
            var envVariables = new BotConfig();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File {filePath} not found.");
            }

            string[] lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                       
                        foreach (var property in typeof(BotConfig).GetProperties())
                        {
                            if (property.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                            {
                                object typeValue = Convert.ChangeType(value, property.PropertyType);
                                property.SetValue(envVariables, typeValue, null);
                                break;
                            }
                        }
                    }
                    else
                    {
                        throw new FormatException($"Invalid format in line: {line}");
                    }
                }
            }

            return envVariables;
        }
    }

    }
