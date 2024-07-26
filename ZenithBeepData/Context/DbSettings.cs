namespace ZenithBeepData;

public static class DbSettings
{
    public static string LocalPathDB() {
        string folder = "ZenithBeepDB";
        string file = "BotDB.db";
        string folderComine = Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData), 
                        folder);
        bool folderExists = Directory.Exists(folderComine);
        if (!folderExists) {
            Directory.CreateDirectory(folderComine);
        }
        var pathDb = Path.Join(folderComine, file);
        return pathDb;

    }
}
