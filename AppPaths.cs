using System;
using System.IO;
using System.Reflection;

namespace BoxAverageTracker
{
    /// <summary>
    /// Tous les fichiers de l'appli vivent dans %AppData%\BoxAverageTracker :
    ///   sounds\                  dossiers de sons (armes + trades)
    ///   games\                   parties sauvegardées (Save game) et stats exportées (Export stats)
    ///   ApplicationConfig.txt    réglages
    /// L'exe peut donc être posé n'importe où (bureau...) sans rien créer à côté.
    /// </summary>
    public static class AppPaths
    {
        public static string DataDirectory { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BoxAverageTracker");

        public static string SoundsDirectory => Path.Combine(DataDirectory, "sounds");
        public static string GamesDirectory => Path.Combine(DataDirectory, "games");
        public static string ConfigPath => Path.Combine(DataDirectory, "ApplicationConfig.txt");

        private static string ExeDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Crée les dossiers et, la première fois, récupère les anciens fichiers qui étaient à côté de l'exe
        /// (config + sons). Rien n'est supprimé : on copie seulement ce qui n'existe pas encore dans AppData.
        /// </summary>
        public static void Initialize()
        {
            try
            {
                Directory.CreateDirectory(DataDirectory);
                Directory.CreateDirectory(SoundsDirectory);
                Directory.CreateDirectory(GamesDirectory);

                foreach (var oldDir in new[] { ExeDirectory, Directory.GetCurrentDirectory() })
                {
                    string oldConfig = Path.Combine(oldDir, "ApplicationConfig.txt");
                    if (!File.Exists(ConfigPath) && File.Exists(oldConfig))
                        File.Copy(oldConfig, ConfigPath);

                    string oldSounds = Path.Combine(oldDir, "sounds");
                    if (Directory.Exists(oldSounds) && !string.Equals(Path.GetFullPath(oldSounds), Path.GetFullPath(SoundsDirectory), StringComparison.OrdinalIgnoreCase))
                        CopyMissingFiles(oldSounds, SoundsDirectory);
                }
            }
            catch { }
        }

        private static void CopyMissingFiles(string source, string destination)
        {
            foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                string relative = file.Substring(source.Length).TrimStart(Path.DirectorySeparatorChar);
                string target = Path.Combine(destination, relative);
                if (File.Exists(target)) continue;
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(file, target);
            }
        }
    }

    /// <summary>Journal d'erreurs : %AppData%\BoxAverageTracker\crash.log</summary>
    public static class CrashLog
    {
        public static string Path => System.IO.Path.Combine(AppPaths.DataDirectory, "crash.log");

        public static void Write(string where, Exception ex)
        {
            try
            {
                Directory.CreateDirectory(AppPaths.DataDirectory);
                File.AppendAllText(Path,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] build {Updater.LocalBuild} - {where}{Environment.NewLine}{ex}{Environment.NewLine}{Environment.NewLine}");
            }
            catch { }
        }
    }
}
