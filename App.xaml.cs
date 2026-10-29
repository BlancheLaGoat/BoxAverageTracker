using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BoxAverageTracker
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        // NAudio.dll est embarqué dans l'exe (EmbeddedResource) pour que l'appli reste un seul fichier
        // et que la mise à jour automatique (qui ne remplace que l'exe) continue de marcher.
        static App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                if (new System.Reflection.AssemblyName(args.Name).Name != "NAudio") return null;
                using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NAudio.dll"))
                {
                    if (stream == null) return null;
                    var bytes = new byte[stream.Length];
                    int read = 0;
                    while (read < bytes.Length) read += stream.Read(bytes, read, bytes.Length - read);
                    return System.Reflection.Assembly.Load(bytes);
                }
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainWindow();
        }
    }
}
