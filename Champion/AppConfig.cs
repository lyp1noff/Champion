
using System.IO;
using System;

namespace Champion

{
    public static class AppConfig
    {
        public static string AppFolder { get; private set; }
        public static string TemplatesFolder { get; private set; }
        public static string ExportFolder { get; private set; }

        public static void Initialize()
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
        }
    }
}
