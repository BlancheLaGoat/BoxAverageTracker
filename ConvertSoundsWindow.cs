using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BoxAverageTracker
{
    /// <summary>
    /// Fenêtre "Convert sounds..." : on choisit l'arme (ou le dossier de trade), on glisse des fichiers
    /// audio/vidéo dedans, et ils sont convertis en .wav directement dans sounds/&lt;dossier&gt;.
    /// </summary>
    public class ConvertSoundsWindow : Window
    {
        private class Destination
        {
            public string Label { get; set; }
            public string Folder { get; set; }
            public override string ToString() => Label;
        }

        private readonly ComboBox m_Destination;
        private readonly TextBox m_Log;
        private readonly Border m_DropZone;
        private readonly Button m_PickButton;
        private bool m_Busy;

        private static readonly Brush s_Bg = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E));
        private static readonly Brush s_Fg = Brushes.White;
        private static readonly Brush s_Accent = new SolidColorBrush(Color.FromRgb(0xE0, 0x8A, 0x2E));

        public ConvertSoundsWindow()
        {
            Title = "Convert sounds";
            Width = 480;
            Height = 460;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Background = s_Bg;
            Foreground = s_Fg;
            AllowDrop = true;

            var root = new DockPanel { Margin = new Thickness(12) };

            // Destination
            var destPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };
            DockPanel.SetDock(destPanel, Dock.Top);
            destPanel.Children.Add(new TextBlock { Text = "Dossier de destination :", Foreground = s_Fg, Margin = new Thickness(0, 0, 0, 4) });
            m_Destination = new ComboBox { ItemsSource = BuildDestinations(), SelectedIndex = 0, Height = 26 };
            destPanel.Children.Add(m_Destination);
            root.Children.Add(destPanel);

            // Boutons du bas
            var bottom = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 10, 0, 0) };
            DockPanel.SetDock(bottom, Dock.Bottom);
            m_PickButton = new Button { Content = "Choisir des fichiers...", Padding = new Thickness(10, 4, 10, 4), Margin = new Thickness(0, 0, 8, 0) };
            m_PickButton.Click += (s, e) => PickFiles();
            var openButton = new Button { Content = "Ouvrir le dossier", Padding = new Thickness(10, 4, 10, 4) };
            openButton.Click += (s, e) => OpenSelectedFolder();
            bottom.Children.Add(m_PickButton);
            bottom.Children.Add(openButton);
            root.Children.Add(bottom);

            // Zone de dépôt
            m_DropZone = new Border
            {
                BorderBrush = s_Accent,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(6),
                Height = 110,
                Margin = new Thickness(0, 0, 0, 10),
                Background = Brushes.Transparent,
                Child = new TextBlock
                {
                    Text = "Glisse tes fichiers ici\n(mp3, mp4, m4a, wav, wma, aac, mov...)",
                    Foreground = s_Fg,
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            DockPanel.SetDock(m_DropZone, Dock.Top);
            root.Children.Add(m_DropZone);

            // Journal
            m_Log = new TextBox
            {
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Background = new SolidColorBrush(Color.FromRgb(0x12, 0x12, 0x12)),
                Foreground = s_Fg,
                BorderBrush = Brushes.DimGray,
                FontFamily = new FontFamily("Consolas")
            };
            root.Children.Add(m_Log);

            Content = root;

            // Preview* : sinon la zone de texte du journal intercepte le glisser-déposer
            PreviewDragOver += OnDragOver;
            PreviewDrop += OnDrop;

            Log("Choisis l'arme, puis glisse un ou plusieurs fichiers.");
            Log("Ils seront convertis en .wav dans sounds\\<dossier>.");
        }

        private static List<Destination> BuildDestinations()
        {
            var list = MainWindow.WeaponSoundFolders
                .Select(x => new Destination { Label = $"{x.Weapon}   (sounds\\{x.Folder})", Folder = x.Folder })
                .OrderBy(d => d.Label, StringComparer.OrdinalIgnoreCase)
                .ToList();

            list.AddRange(MainWindow.TradeSoundFolders
                .Select(f => new Destination { Label = $"Trade : {f}   (sounds\\{f})", Folder = f }));

            return list;
        }

        private string SelectedFolderPath =>
            Path.Combine(MainWindow.SoundsDirectory, ((Destination)m_Destination.SelectedItem).Folder);

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = !m_Busy && e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private async void OnDrop(object sender, DragEventArgs e)
        {
            if (m_Busy || !e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = ((string[])e.Data.GetData(DataFormats.FileDrop)).Where(File.Exists).ToArray();
            await ConvertFiles(files);
        }

        private async void PickFiles()
        {
            if (m_Busy) return;
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Title = "Fichiers audio ou vidéo à convertir",
                Filter = "Audio / vidéo|*.mp3;*.mp4;*.m4a;*.aac;*.wav;*.wma;*.mov;*.m4v;*.3gp;*.flac|Tous les fichiers|*.*"
            };
            if (dialog.ShowDialog(this) == true)
                await ConvertFiles(dialog.FileNames);
        }

        private async Task ConvertFiles(string[] files)
        {
            if (files.Length == 0) return;

            m_Busy = true;
            m_PickButton.IsEnabled = false;
            m_Destination.IsEnabled = false;
            string folder = SelectedFolderPath;
            int ok = 0;

            foreach (var file in files)
            {
                Log($"→ {Path.GetFileName(file)}...");
                try
                {
                    string output = await Task.Run(() => SoundConverter.ConvertToWav(file, folder));
                    Log($"   OK : {Path.GetFileName(output)}");
                    ok++;
                }
                catch (Exception ex)
                {
                    Log($"   ÉCHEC : {ex.Message}");
                }
            }

            Log($"Terminé : {ok}/{files.Length} fichier(s) dans sounds\\{Path.GetFileName(folder)}");
            m_Busy = false;
            m_PickButton.IsEnabled = true;
            m_Destination.IsEnabled = true;
        }

        private void OpenSelectedFolder()
        {
            try
            {
                Directory.CreateDirectory(SelectedFolderPath);
                Process.Start(new ProcessStartInfo("explorer.exe", "\"" + SelectedFolderPath + "\"") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Log("Impossible d'ouvrir le dossier : " + ex.Message);
            }
        }

        private void Log(string line)
        {
            m_Log.AppendText(line + Environment.NewLine);
            m_Log.ScrollToEnd();
        }
    }
}
