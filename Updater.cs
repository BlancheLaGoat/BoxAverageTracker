using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace BoxAverageTracker
{
    /// <summary>
    /// Mise à jour automatique depuis la Release GitHub "latest".
    ///
    /// Le workflow GitHub Actions publie dans la Release :
    ///   - BoxAverageTracker.exe  (compilé avec AssemblyVersion 2.0.N.0, N = numéro du build)
    ///   - version.txt            (contient juste N)
    ///
    /// Au lancement, l'appli lit version.txt, et si N est plus grand que son propre numéro de build,
    /// elle propose de télécharger le nouvel exe, remplace l'ancien et se relance.
    /// Un exe compilé en local dans Visual Studio a le build 0 : il ne cherche jamais de mise à jour.
    /// </summary>
    public static class Updater
    {
        private const string BaseUrl = "https://github.com/BlancheLaGoat/BoxAverageTracker/releases/download/latest/";
        private const string VersionUrl = BaseUrl + "version.txt";
        private const string ExeUrl = BaseUrl + "BoxAverageTracker.exe";

        public static int LocalBuild => Assembly.GetExecutingAssembly().GetName().Version.Build;

        private static string ExePath => Assembly.GetExecutingAssembly().Location;
        private static string OldExePath => ExePath + ".old";
        private static string NewExePath => ExePath + ".new";

        /// <summary>Supprime l'ancien exe laissé par la mise à jour précédente.</summary>
        public static void CleanupPreviousUpdate()
        {
            try { if (File.Exists(OldExePath)) File.Delete(OldExePath); } catch { }
            try { if (File.Exists(NewExePath)) File.Delete(NewExePath); } catch { }
        }

        /// <param name="silentIfUpToDate">true au démarrage (pas de message si tout est à jour), false quand on clique sur "Check for updates".</param>
        public static async Task CheckForUpdatesAsync(Window owner, bool silentIfUpToDate)
        {
            if (LocalBuild == 0)
            {
                if (!silentIfUpToDate)
                    MessageBox.Show(owner, "Local build (build 0): automatic updates are disabled.", "Updates", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int remoteBuild;
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.UserAgent] = "BoxAverageTracker";
                    client.Headers[HttpRequestHeader.CacheControl] = "no-cache";
                    string text = await client.DownloadStringTaskAsync(VersionUrl + "?t=" + DateTime.UtcNow.Ticks);
                    if (!int.TryParse(text.Trim(), out remoteBuild)) return;
                }
            }
            catch
            {
                // Pas d'internet, GitHub indisponible... on ne gêne pas l'utilisateur au démarrage
                if (!silentIfUpToDate)
                    MessageBox.Show(owner, "Could not check for updates (no connection to GitHub?).", "Updates", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (remoteBuild <= LocalBuild)
            {
                if (!silentIfUpToDate)
                    MessageBox.Show(owner, $"You already have the latest version (build {LocalBuild}).", "Updates", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var answer = MessageBox.Show(owner,
                $"A new version is available (build {remoteBuild}, you have build {LocalBuild}).\n\nUpdate now? The app will restart.",
                "Update available", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (answer != MessageBoxResult.Yes) return;

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.UserAgent] = "BoxAverageTracker";
                    await client.DownloadFileTaskAsync(ExeUrl + "?t=" + DateTime.UtcNow.Ticks, NewExePath);
                }

                if (new FileInfo(NewExePath).Length < 50 * 1024)
                    throw new InvalidDataException("Downloaded file is too small.");

                // Windows autorise à renommer un exe en cours d'exécution, mais pas à l'écraser
                if (File.Exists(OldExePath)) File.Delete(OldExePath);
                File.Move(ExePath, OldExePath);
                File.Move(NewExePath, ExePath);
            }
            catch (Exception ex)
            {
                // Si quelque chose a raté, on remet l'ancien exe en place
                try
                {
                    if (!File.Exists(ExePath) && File.Exists(OldExePath)) File.Move(OldExePath, ExePath);
                    if (File.Exists(NewExePath)) File.Delete(NewExePath);
                }
                catch { }

                MessageBox.Show(owner, "The update failed:\n" + ex.Message, "Updates", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Process.Start(new ProcessStartInfo(ExePath)
            {
                WorkingDirectory = Path.GetDirectoryName(ExePath),
                UseShellExecute = true
            });

            Application.Current.Shutdown();
        }
    }
}
