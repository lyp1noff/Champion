using System;
using System.IO;

namespace Champion;

public class AppConfig
{
    public int MaxCompetitorsPerGroup { get; set; }
    public int MaxCompetitorsPerRoundGroup { get; set; }
    public string AppFolder { get; private set; }
    public string TemplatesFolder { get; private set; }
    public string ExportFolder { get; private set; }
    public string CategoriesFile { get; private set; }

    public void Initialize()
    {
        string appFolderRaw;
#if WINDOWS
            appFolderRaw = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
#elif MACOS
        appFolderRaw = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Documents");
#else
        appFolderRaw = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
#endif

        AppFolder = Path.Combine(appFolderRaw, "Champion");
        TemplatesFolder = Path.Combine(AppFolder, "Templates");
        ExportFolder = Path.Combine(AppFolder, "Export");
        CategoriesFile = Path.Combine(AppFolder, "categories.txt");
    }
}