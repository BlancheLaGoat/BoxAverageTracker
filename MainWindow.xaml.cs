using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace BoxAverageTracker
{
    public partial class MainWindow : Window
    {
        public static DependencyProperty BoxHitsCountProperty = DependencyProperty.Register("BoxHitsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        public static DependencyProperty CurrentBoxHitsCountProperty = DependencyProperty.Register("CurrentBoxHitsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        public static DependencyProperty LastBoxHitsCountProperty = DependencyProperty.Register("LastBoxHitsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        public static DependencyProperty ShowCurrentBoxHitsProperty = DependencyProperty.Register("ShowCurrentBoxHits", typeof(bool), typeof(MainWindow), new PropertyMetadata(false, SaveApplicationConfig));
        public static DependencyProperty CurrentTradeHitsCountProperty = DependencyProperty.Register("CurrentTradeHitsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        public static DependencyProperty LastTradeHitsCountProperty = DependencyProperty.Register("LastTradeHitsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        public static DependencyProperty ShowCurrentTradeHitsProperty = DependencyProperty.Register("ShowCurrentTradeHits", typeof(bool), typeof(MainWindow), new PropertyMetadata(true, SaveApplicationConfig));
        public static DependencyProperty TotalTradeCountProperty = DependencyProperty.Register("TotalTradeCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        public static DependencyProperty ShowTotalTradeCountProperty = DependencyProperty.Register("ShowTotalTradeCount", typeof(bool), typeof(MainWindow), new PropertyMetadata(true, SaveApplicationConfig));

        public static DependencyProperty WeaponAveragesProperty = DependencyProperty.Register("WeaponAverages", typeof(ObservableCollection<KeyValuePair<string, float>>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<KeyValuePair<string, float>>()));
        public static DependencyProperty SelectedMapProperty = DependencyProperty.Register("SelectedMap", typeof(string), typeof(MainWindow), new PropertyMetadata(""));
        public static DependencyProperty TotalHeightProperty = DependencyProperty.Register("TotalHeight", typeof(int), typeof(MainWindow), new PropertyMetadata(100, SaveApplicationConfig));
        public static DependencyProperty TotalWidthProperty = DependencyProperty.Register("TotalWidth", typeof(int), typeof(MainWindow), new PropertyMetadata(275, SaveApplicationConfig));
        public static DependencyProperty AutoDisplayProperty = DependencyProperty.Register("AutoDisplay", typeof(bool), typeof(MainWindow), new PropertyMetadata(true, SaveApplicationConfig));
        public static DependencyProperty GameConnectedStringProperty = DependencyProperty.Register("GameConnectedString", typeof(string), typeof(MainWindow), new PropertyMetadata("Game not connected"));

        // Box Hits line font
        public static DependencyProperty BoxHitsFontFamilyProperty = DependencyProperty.Register("BoxHitsFontFamily", typeof(System.Windows.Media.FontFamily), typeof(MainWindow), new PropertyMetadata(new System.Windows.Media.FontFamily(), SaveApplicationConfig));
        public static DependencyProperty BoxHitsForegroundProperty = DependencyProperty.Register("BoxHitsForeground", typeof(System.Windows.Media.Brush), typeof(MainWindow), new PropertyMetadata(System.Windows.Media.Brushes.White, SaveApplicationConfig));
        public static DependencyProperty BoxHitsSizeProperty = DependencyProperty.Register("BoxHitsSize", typeof(float), typeof(MainWindow), new PropertyMetadata(30f, SaveApplicationConfig));
        public static DependencyProperty BoxHitsFontWeightProperty = DependencyProperty.Register("BoxHitsFontWeight", typeof(FontWeight), typeof(MainWindow), new PropertyMetadata(FontWeights.Normal, SaveApplicationConfig));
        public static DependencyProperty BoxHitsFontStyleProperty = DependencyProperty.Register("BoxHitsFontStyle", typeof(FontStyle), typeof(MainWindow), new PropertyMetadata(FontStyles.Normal, SaveApplicationConfig));

        // Current Box line font
        public static DependencyProperty CurrentBoxFontFamilyProperty = DependencyProperty.Register("CurrentBoxFontFamily", typeof(System.Windows.Media.FontFamily), typeof(MainWindow), new PropertyMetadata(new System.Windows.Media.FontFamily(), SaveApplicationConfig));
        public static DependencyProperty CurrentBoxForegroundProperty = DependencyProperty.Register("CurrentBoxForeground", typeof(System.Windows.Media.Brush), typeof(MainWindow), new PropertyMetadata(System.Windows.Media.Brushes.White, SaveApplicationConfig));
        public static DependencyProperty CurrentBoxSizeProperty = DependencyProperty.Register("CurrentBoxSize", typeof(float), typeof(MainWindow), new PropertyMetadata(30f, SaveApplicationConfig));
        public static DependencyProperty CurrentBoxFontWeightProperty = DependencyProperty.Register("CurrentBoxFontWeight", typeof(FontWeight), typeof(MainWindow), new PropertyMetadata(FontWeights.Normal, SaveApplicationConfig));
        public static DependencyProperty CurrentBoxFontStyleProperty = DependencyProperty.Register("CurrentBoxFontStyle", typeof(FontStyle), typeof(MainWindow), new PropertyMetadata(FontStyles.Normal, SaveApplicationConfig));

        // Current Trade line font
        public static DependencyProperty CurrentTradeFontFamilyProperty = DependencyProperty.Register("CurrentTradeFontFamily", typeof(System.Windows.Media.FontFamily), typeof(MainWindow), new PropertyMetadata(new System.Windows.Media.FontFamily(), SaveApplicationConfig));
        public static DependencyProperty CurrentTradeForegroundProperty = DependencyProperty.Register("CurrentTradeForeground", typeof(System.Windows.Media.Brush), typeof(MainWindow), new PropertyMetadata(System.Windows.Media.Brushes.White, SaveApplicationConfig));
        public static DependencyProperty CurrentTradeSizeProperty = DependencyProperty.Register("CurrentTradeSize", typeof(float), typeof(MainWindow), new PropertyMetadata(30f, SaveApplicationConfig));
        public static DependencyProperty CurrentTradeFontWeightProperty = DependencyProperty.Register("CurrentTradeFontWeight", typeof(FontWeight), typeof(MainWindow), new PropertyMetadata(FontWeights.Normal, SaveApplicationConfig));
        public static DependencyProperty CurrentTradeFontStyleProperty = DependencyProperty.Register("CurrentTradeFontStyle", typeof(FontStyle), typeof(MainWindow), new PropertyMetadata(FontStyles.Normal, SaveApplicationConfig));

        // Total Trades line font
        public static DependencyProperty TotalTradeFontFamilyProperty = DependencyProperty.Register("TotalTradeFontFamily", typeof(System.Windows.Media.FontFamily), typeof(MainWindow), new PropertyMetadata(new System.Windows.Media.FontFamily(), SaveApplicationConfig));
        public static DependencyProperty TotalTradeForegroundProperty = DependencyProperty.Register("TotalTradeForeground", typeof(System.Windows.Media.Brush), typeof(MainWindow), new PropertyMetadata(System.Windows.Media.Brushes.White, SaveApplicationConfig));
        public static DependencyProperty TotalTradeSizeProperty = DependencyProperty.Register("TotalTradeSize", typeof(float), typeof(MainWindow), new PropertyMetadata(30f, SaveApplicationConfig));
        public static DependencyProperty TotalTradeFontWeightProperty = DependencyProperty.Register("TotalTradeFontWeight", typeof(FontWeight), typeof(MainWindow), new PropertyMetadata(FontWeights.Normal, SaveApplicationConfig));
        public static DependencyProperty TotalTradeFontStyleProperty = DependencyProperty.Register("TotalTradeFontStyle", typeof(FontStyle), typeof(MainWindow), new PropertyMetadata(FontStyles.Normal, SaveApplicationConfig));

        // Averages font
        public static DependencyProperty AverageTextFontFamilyProperty = DependencyProperty.Register("AverageTextFontFamily", typeof(System.Windows.Media.FontFamily), typeof(MainWindow), new PropertyMetadata(new System.Windows.Media.FontFamily(), SaveApplicationConfig));
        public static DependencyProperty AverageTextForegroundProperty = DependencyProperty.Register("AverageTextForeground", typeof(System.Windows.Media.Brush), typeof(MainWindow), new PropertyMetadata(System.Windows.Media.Brushes.White, SaveApplicationConfig));
        public static DependencyProperty AverageTextSizeProperty = DependencyProperty.Register("AverageTextSize", typeof(float), typeof(MainWindow), new PropertyMetadata(12.5f, SaveApplicationConfig));
        public static DependencyProperty AverageTextFontWeightProperty = DependencyProperty.Register("AverageTextFontWeight", typeof(FontWeight), typeof(MainWindow), new PropertyMetadata(FontWeights.Normal, SaveApplicationConfig));
        public static DependencyProperty AverageTextFontStyleProperty = DependencyProperty.Register("AverageTextFontStyle", typeof(FontStyle), typeof(MainWindow), new PropertyMetadata(FontStyles.Normal, SaveApplicationConfig));

        public static DependencyProperty SetupFinishedProperty = DependencyProperty.Register("SetupFinished", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
        public static DependencyProperty WorstTradesStringProperty = DependencyProperty.Register("WorstTradesString", typeof(string), typeof(MainWindow), new PropertyMetadata(""));
        public static DependencyProperty ShowWorstTradesProperty = DependencyProperty.Register("ShowWorstTrades", typeof(bool), typeof(MainWindow), new PropertyMetadata(false, SaveApplicationConfig));
        public static DependencyProperty TradesDisplayCountProperty = DependencyProperty.Register("TradesDisplayCount", typeof(int), typeof(MainWindow), new PropertyMetadata(3, null));
        public static DependencyProperty BestTradesStringProperty = DependencyProperty.Register("BestTradesString", typeof(string), typeof(MainWindow), new PropertyMetadata(""));
        public static DependencyProperty ShowBestTradesProperty = DependencyProperty.Register("ShowBestTrades", typeof(bool), typeof(MainWindow), new PropertyMetadata(false, SaveApplicationConfig));
        public static DependencyProperty SoundsEnabledProperty = DependencyProperty.Register("SoundsEnabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(true, SaveApplicationConfig));
        public static DependencyProperty ShowWeaponPopupProperty = DependencyProperty.Register("ShowWeaponPopup", typeof(bool), typeof(MainWindow), new PropertyMetadata(true, SaveApplicationConfig));
        public static DependencyProperty WeaponPopupTextProperty = DependencyProperty.Register("WeaponPopupText", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        private CountWeapons countWeaponsWindow = null;
        private TradeWeapons tradeWeaponsWindow = null;

        public int BoxHitsCount
        {
            get => (int)GetValue(BoxHitsCountProperty);
            set => SetValue(BoxHitsCountProperty, value);
        }

        public int CurrentBoxHitsCount
        {
            get => (int)GetValue(CurrentBoxHitsCountProperty);
            set => SetValue(CurrentBoxHitsCountProperty, value);
        }

        public int LastBoxHitsCount
        {
            get => (int)GetValue(LastBoxHitsCountProperty);
            set => SetValue(LastBoxHitsCountProperty, value);
        }

        public int CurrentTradeHitsCount
        {
            get => (int)GetValue(CurrentTradeHitsCountProperty);
            set => SetValue(CurrentTradeHitsCountProperty, value);
        }

        public int LastTradeHitsCount
        {
            get => (int)GetValue(LastTradeHitsCountProperty);
            set => SetValue(LastTradeHitsCountProperty, value);
        }

        public int TotalTradeCount
        {
            get => (int)GetValue(TotalTradeCountProperty);
            set => SetValue(TotalTradeCountProperty, value);
        }

        public bool ShowTotalTradeCount
        {
            get => Dispatcher.Invoke(() => (bool)GetValue(ShowTotalTradeCountProperty));
            set => Dispatcher.Invoke(() => SetValue(ShowTotalTradeCountProperty, value));
        }

        public bool ShowCurrentBoxHits
        {
            get => Dispatcher.Invoke(() => (bool)GetValue(ShowCurrentBoxHitsProperty));
            set => Dispatcher.Invoke(() => SetValue(ShowCurrentBoxHitsProperty, value));
        }

        public bool ShowCurrentTradeHits
        {
            get => Dispatcher.Invoke(() => (bool)GetValue(ShowCurrentTradeHitsProperty));
            set => Dispatcher.Invoke(() => SetValue(ShowCurrentTradeHitsProperty, value));
        }

        public ObservableCollection<KeyValuePair<string, float>> WeaponAverages
        {
            get => Dispatcher.Invoke(() => (ObservableCollection<KeyValuePair<string, float>>)GetValue(WeaponAveragesProperty));
            set => Dispatcher.Invoke(() => SetValue(WeaponAveragesProperty, value));
        }

        public string SelectedMap
        {
            get => Dispatcher.Invoke(() => (string)GetValue(SelectedMapProperty));
            set => Dispatcher.Invoke(() => SetValue(SelectedMapProperty, value));
        }

        // Box Hits line
        public System.Windows.Media.FontFamily BoxHitsFontFamily
        {
            get => (System.Windows.Media.FontFamily)GetValue(BoxHitsFontFamilyProperty);
            set => SetValue(BoxHitsFontFamilyProperty, value);
        }
        public System.Windows.Media.Brush BoxHitsForeground
        {
            get => (System.Windows.Media.Brush)GetValue(BoxHitsForegroundProperty);
            set => SetValue(BoxHitsForegroundProperty, value);
        }
        public FontWeight BoxHitsFontWeight
        {
            get => (FontWeight)GetValue(BoxHitsFontWeightProperty);
            set => SetValue(BoxHitsFontWeightProperty, value);
        }
        public FontStyle BoxHitsFontStyle
        {
            get => (FontStyle)GetValue(BoxHitsFontStyleProperty);
            set => SetValue(BoxHitsFontStyleProperty, value);
        }
        public float BoxHitsSize
        {
            get => (float)GetValue(BoxHitsSizeProperty);
            set => SetValue(BoxHitsSizeProperty, value);
        }

        // Current Box line
        public System.Windows.Media.FontFamily CurrentBoxFontFamily
        {
            get => (System.Windows.Media.FontFamily)GetValue(CurrentBoxFontFamilyProperty);
            set => SetValue(CurrentBoxFontFamilyProperty, value);
        }
        public System.Windows.Media.Brush CurrentBoxForeground
        {
            get => (System.Windows.Media.Brush)GetValue(CurrentBoxForegroundProperty);
            set => SetValue(CurrentBoxForegroundProperty, value);
        }
        public FontWeight CurrentBoxFontWeight
        {
            get => (FontWeight)GetValue(CurrentBoxFontWeightProperty);
            set => SetValue(CurrentBoxFontWeightProperty, value);
        }
        public FontStyle CurrentBoxFontStyle
        {
            get => (FontStyle)GetValue(CurrentBoxFontStyleProperty);
            set => SetValue(CurrentBoxFontStyleProperty, value);
        }
        public float CurrentBoxSize
        {
            get => (float)GetValue(CurrentBoxSizeProperty);
            set => SetValue(CurrentBoxSizeProperty, value);
        }

        // Current Trade line
        public System.Windows.Media.FontFamily CurrentTradeFontFamily
        {
            get => (System.Windows.Media.FontFamily)GetValue(CurrentTradeFontFamilyProperty);
            set => SetValue(CurrentTradeFontFamilyProperty, value);
        }
        public System.Windows.Media.Brush CurrentTradeForeground
        {
            get => (System.Windows.Media.Brush)GetValue(CurrentTradeForegroundProperty);
            set => SetValue(CurrentTradeForegroundProperty, value);
        }
        public FontWeight CurrentTradeFontWeight
        {
            get => (FontWeight)GetValue(CurrentTradeFontWeightProperty);
            set => SetValue(CurrentTradeFontWeightProperty, value);
        }
        public FontStyle CurrentTradeFontStyle
        {
            get => (FontStyle)GetValue(CurrentTradeFontStyleProperty);
            set => SetValue(CurrentTradeFontStyleProperty, value);
        }
        public float CurrentTradeSize
        {
            get => (float)GetValue(CurrentTradeSizeProperty);
            set => SetValue(CurrentTradeSizeProperty, value);
        }

        // Total Trades line
        public System.Windows.Media.FontFamily TotalTradeFontFamily
        {
            get => (System.Windows.Media.FontFamily)GetValue(TotalTradeFontFamilyProperty);
            set => SetValue(TotalTradeFontFamilyProperty, value);
        }
        public System.Windows.Media.Brush TotalTradeForeground
        {
            get => (System.Windows.Media.Brush)GetValue(TotalTradeForegroundProperty);
            set => SetValue(TotalTradeForegroundProperty, value);
        }
        public FontWeight TotalTradeFontWeight
        {
            get => (FontWeight)GetValue(TotalTradeFontWeightProperty);
            set => SetValue(TotalTradeFontWeightProperty, value);
        }
        public FontStyle TotalTradeFontStyle
        {
            get => (FontStyle)GetValue(TotalTradeFontStyleProperty);
            set => SetValue(TotalTradeFontStyleProperty, value);
        }
        public float TotalTradeSize
        {
            get => (float)GetValue(TotalTradeSizeProperty);
            set => SetValue(TotalTradeSizeProperty, value);
        }

        public bool AutoDisplay
        {
            get => Dispatcher.Invoke(() => (bool)GetValue(AutoDisplayProperty));
            set => Dispatcher.Invoke(() => SetValue(AutoDisplayProperty, value));
        }

        public System.Windows.Media.FontFamily AverageTextFontFamily
        {
            get => (System.Windows.Media.FontFamily)GetValue(AverageTextFontFamilyProperty);
            set => SetValue(AverageTextFontFamilyProperty, value);
        }

        public System.Windows.Media.Brush AverageTextForeground
        {
            get => (System.Windows.Media.Brush)GetValue(AverageTextForegroundProperty);
            set => SetValue(AverageTextForegroundProperty, value);
        }

        public float AverageTextSize
        {
            get => (float)GetValue(AverageTextSizeProperty);
            set => SetValue(AverageTextSizeProperty, value);
        }

        public FontWeight AverageTextFontWeight
        {
            get => (FontWeight)GetValue(AverageTextFontWeightProperty);
            set => SetValue(AverageTextFontWeightProperty, value);
        }

        public FontStyle AverageTextFontStyle
        {
            get => (FontStyle)GetValue(AverageTextFontStyleProperty);
            set => SetValue(AverageTextFontStyleProperty, value);
        }

        public int TotalHeight
        {
            get => (int)GetValue(TotalHeightProperty);
            set => SetValue(TotalHeightProperty, value);
        }

        public int TotalWidth
        {
            get => (int)GetValue(TotalWidthProperty);
            set => SetValue(TotalWidthProperty, value);
        }

        public string GameConnectedString
        {
            get => Dispatcher.Invoke(() => (string)GetValue(GameConnectedStringProperty));
            set => Dispatcher.Invoke(() => SetValue(GameConnectedStringProperty, value));
        }

        public bool SetupFinished
        {
            get => Dispatcher.Invoke(() => (bool)GetValue(SetupFinishedProperty));
            set => Dispatcher.Invoke(() => SetValue(SetupFinishedProperty, value));
        }

        public string WorstTradesString
        {
            get => Dispatcher.Invoke(() => (string)GetValue(WorstTradesStringProperty));
            set => Dispatcher.Invoke(() => SetValue(WorstTradesStringProperty, value));
        }

        public bool ShowWorstTrades
        {
            get => Dispatcher.Invoke(() => (bool)GetValue(ShowWorstTradesProperty));
            set => Dispatcher.Invoke(() => SetValue(ShowWorstTradesProperty, value));
        }

        public int TradesDisplayCount
        {
            get => Dispatcher.Invoke(() => (int)GetValue(TradesDisplayCountProperty));
            set => Dispatcher.Invoke(() => SetValue(TradesDisplayCountProperty, value));
        }

        public string BestTradesString
        {
            get => Dispatcher.Invoke(() => (string)GetValue(BestTradesStringProperty));
            set => Dispatcher.Invoke(() => SetValue(BestTradesStringProperty, value));
        }

        public bool ShowBestTrades
        {
            get => Dispatcher.Invoke(() => (bool)GetValue(ShowBestTradesProperty));
            set => Dispatcher.Invoke(() => SetValue(ShowBestTradesProperty, value));
        }


        public bool SoundsEnabled
        {
            get => Dispatcher.Invoke(() => (bool)GetValue(SoundsEnabledProperty));
            set => Dispatcher.Invoke(() => SetValue(SoundsEnabledProperty, value));
        }

        public bool ShowWeaponPopup
        {
            get => Dispatcher.Invoke(() => (bool)GetValue(ShowWeaponPopupProperty));
            set => Dispatcher.Invoke(() => SetValue(ShowWeaponPopupProperty, value));
        }

        public string WeaponPopupText
        {
            get => Dispatcher.Invoke(() => (string)GetValue(WeaponPopupTextProperty));
            set => Dispatcher.Invoke(() => SetValue(WeaponPopupTextProperty, value));
        }

        // Durée d'affichage du popup (en secondes), modifiable dans ApplicationConfig.txt (WeaponPopupSeconds=)
        private double m_WeaponPopupSeconds = 4.0;

        private Dictionary<string, List<int>> m_WeaponsGotten = new Dictionary<string, List<int>>();
        private Dictionary<string, int> m_WeaponsBoxHits = new Dictionary<string, int>();
        private List<string> m_WeaponsToDisplay = new List<string>();
        private List<string> m_WeaponsToNotCount = new List<string>();
        private List<string> m_WeaponsBeingTraded = new List<string>();

        private System.Threading.Timer _autoFinishTimer = null;
        private static readonly Random _random = new Random();

        private static readonly (int Min, int Max, string Folder)[] s_TradeSoundRanges = new[]
        {
            (1,   1,   "trade_1"),
            (2,   10,  "trade_2-10"),
            (11,  20,  "trade_10-20"),
            (21,  30,  "trade_20-30"),
            (31,  50,  "trade_30-50"),
            (51,  100, "trade_50-100"),
            (101, int.MaxValue, "trade_100+"),
        };
        private List<int> _worstTrades = new List<int>();
        private List<int> _bestTrades = new List<int>();

        private List<int> m_WhiteInventory = new List<int>();
        private List<int> m_BlueInventory = new List<int>();
        private List<int> m_YellowInventory = new List<int>();
        private List<int> m_GreenInventory = new List<int>();

        private List<int> m_WhiteAmmos = new List<int>();
        private List<int> m_BlueAmmos = new List<int>();
        private List<int> m_YellowAmmos = new List<int>();
        private List<int> m_GreenAmmos = new List<int>();

        private List<int> m_WhiteTmpInventory = new List<int>();
        private List<int> m_BlueTmpInventory = new List<int>();
        private List<int> m_YellowTmpInventory = new List<int>();
        private List<int> m_GreenTmpInventory = new List<int>();

        private static readonly HashSet<string> s_SpecialWeapons = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Thundergun", "Matrioshka", "Crossbow", "Ballistic Knife", "Scavenger", "VR-11",
            "Wunderwaffe DG-2", "Winter's howl", "Wavegun", "31-79 Jgb 215"
        };

        private static readonly string[] s_AllWeaponNames = new[]
        {
            "Python", "CZ75", "G11", "FAMAS", "Spectre", "CZ75x2", "SPAS-12", "HS-10", "AUG", "Galil", "Commando",
            "FN FAL", "Dragunov", "L96A1", "RPK", "HK21", "M72 LAW", "China Lake", "Gersh", "Matrioshka", "Ray Gun",
            "Thundergun", "Crossbow", "Ballistic Knife", "Scavenger", "VR-11", "Wunderwaffe DG-2", "Winter's howl",
            "QED", "Wavegun", "31-79 Jgb 215", "Teddy bear"
        };

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            m_WeaponsBoxHits = s_AllWeaponNames.ToDictionary(n => n, n => 0);
            m_WeaponsGotten = s_AllWeaponNames.ToDictionary(n => n, n => new List<int>());

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
                hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;

            base.OnSourceInitialized(e);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (BoxHitsCount > 0 && SetupFinished)
            {
                System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                    "You're closing the box hits tracker but it has registered box hits.\nDo you want to save the stats and the game before closing it ?",
                    "Save & Exit", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    SaveGame();
                    ExportGame();
                }
            }

            BlackOpsLibrary.m_ProcessHandle = null;
            BlackOpsLibrary.m_BlackOpsProcess = null;
            GameConnectedString = "Game not connected";

            _autoFinishTimer?.Dispose();
            Application.Current?.Shutdown();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ListMaps.ContextMenu = ContextMenu = (ContextMenu)Resources["MainMenu"];
            LoadApplicationConfig();

            Updater.CleanupPreviousUpdate();
            _ = Updater.CheckForUpdatesAsync(this, silentIfUpToDate: true);

            await UpdateTracking();
        }

        private void CheckTradeSoundOnInventoryChange()
        {
            var dict = GetWeaponsDictionary();
            if (dict == null) return;

            int changedId = GetChangedWeaponId();
            if (changedId == -1) return;

            if (!dict.TryGetValue(changedId, out string weaponName)) return;

            var papSet = GetPapSetForSelectedMap();
            bool isPap = papSet != null && papSet.Contains(changedId);
            if (isPap) return;

            if (!SoundsEnabled) return;

            // Son perso de l'arme : sounds/<nom de l'arme>/ (ex: sounds/thundergun, sounds/raygun).
            // S'il existe et contient des .wav, il passe en priorité sur le son de trade.
            if (PlaySpecialWeaponSound(GetWeaponSoundFolderName(weaponName)))
                return;

            // Sinon, son de trade pour les armes tradées (seulement si SetupFinished)
            if (SetupFinished && m_WeaponsBeingTraded.Any(x => string.Equals(x, weaponName, StringComparison.OrdinalIgnoreCase)))
                PlayTradeSound(GetCurrentTradeHits());
        }

        /// <summary>
        /// Nom du dossier de sons d'une arme : en minuscules, sans espaces ni caractères spéciaux.
        /// "Thundergun" -> thundergun, "Ray Gun" -> raygun, "Wunderwaffe DG-2" -> wunderwaffedg2,
        /// "Winter's howl" -> wintershowl, "31-79 Jgb 215" -> 3179jgb215, "China Lake" -> chinalake
        /// </summary>
        private static string GetWeaponSoundFolderName(string weaponName)
        {
            return new string(weaponName.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        }

        /// <returns>true si un son a été joué</returns>
        private bool PlaySpecialWeaponSound(string folderName)
        {
            try
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string folder = Path.Combine(Path.GetDirectoryName(exePath), "sounds", folderName);
                if (!Directory.Exists(folder)) return false;
                var files = Directory.GetFiles(folder, "*.wav");
                if (files.Length == 0) return false;
                string chosen = files[_random.Next(files.Length)];
                var player = new System.Media.SoundPlayer(chosen);
                player.Load();
                player.Play();
                return true;
            }
            catch { return false; }
        }

        private void PlayTradeSound(int tradeHits)
        {
            try
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string soundsDir = Path.Combine(Path.GetDirectoryName(exePath), "sounds");

                string folder = null;
                foreach (var range in s_TradeSoundRanges)
                {
                    if (tradeHits >= range.Min && tradeHits <= range.Max)
                    {
                        folder = Path.Combine(soundsDir, range.Folder);
                        break;
                    }
                }

                if (folder == null || !Directory.Exists(folder)) return;

                var files = Directory.GetFiles(folder, "*.wav");
                if (files.Length == 0) return;

                string chosen = files[_random.Next(files.Length)];
                var player = new System.Media.SoundPlayer(chosen);
                player.Load();
                player.Play();
            }
            catch { }
        }

        private async Task UpdateTracking()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (!BlackOpsLibrary.IsGameClosed())
                    {
                        bool isBoxRandomizing = false;
                        int timeSinceLastRandom = 0;

                        int previousTime = BlackOpsLibrary.ReadInt(Constants.C_TIMEADDRESS);
                        int previousMoonTeddy = BlackOpsLibrary.ReadInt(Constants.C_MOONTEDDYBEAR);

                        int whitePreviousPoints = BlackOpsLibrary.ReadInt(Constants.C_WHITEPOINTSADDRESS);
                        int whitePreviousKills = BlackOpsLibrary.ReadInt(Constants.C_WHITEKILLSADDRESS);
                        int whitePreviousDowns = BlackOpsLibrary.ReadInt(Constants.C_WHITEDOWNSADDRESS);
                        int whitePreviousRevives = BlackOpsLibrary.ReadInt(Constants.C_WHITEREVIVESADDRESS);

                        int bluePreviousPoints = BlackOpsLibrary.ReadInt(Constants.C_BLUEPOINTSADDRESS);
                        int bluePreviousKills = BlackOpsLibrary.ReadInt(Constants.C_BLUEKILLSADDRESS);
                        int bluePreviousDowns = BlackOpsLibrary.ReadInt(Constants.C_BLUEDOWNSADDRESS);
                        int bluePreviousRevives = BlackOpsLibrary.ReadInt(Constants.C_BLUEREVIVESADDRESS);

                        int yellowPreviousPoints = BlackOpsLibrary.ReadInt(Constants.C_YELLOWPOINTSADDRESS);
                        int yellowPreviousKills = BlackOpsLibrary.ReadInt(Constants.C_YELLOWKILLSADDRESS);
                        int yellowPreviousDowns = BlackOpsLibrary.ReadInt(Constants.C_YELLOWDOWNSADDRESS);
                        int yellowPreviousRevives = BlackOpsLibrary.ReadInt(Constants.C_YELLOWREVIVESADDRESS);

                        int greenPreviousPoints = BlackOpsLibrary.ReadInt(Constants.C_GREENPOINTSADDRESS);
                        int greenPreviousKills = BlackOpsLibrary.ReadInt(Constants.C_GREENKILLSADDRESS);
                        int greenPreviousDowns = BlackOpsLibrary.ReadInt(Constants.C_GREENDOWNSADDRESS);
                        int greenPreviousRevives = BlackOpsLibrary.ReadInt(Constants.C_GREENREVIVESADDRESS);

                        InitializeInventories();

                        while (BlackOpsLibrary.m_BlackOpsProcess != null && !BlackOpsLibrary.m_BlackOpsProcess.HasExited)
                        {
                            try
                            {
                                GameConnectedString = string.Empty;

                                int timeCount = BlackOpsLibrary.ReadInt(Constants.C_TIMEADDRESS);
                                int moonTeddy = BlackOpsLibrary.ReadInt(Constants.C_MOONTEDDYBEAR);
                                int mapId = BlackOpsLibrary.ReadInt(Constants.C_MAPADDRESS);

                                bool mustReset = timeCount < 50 || string.IsNullOrEmpty(SelectedMap);

                                if (mustReset)
                                {
                                    UpdateSelectedMap(mapId);
                                    RefreshWeaponAverages();
                                    ResetTracking();
                                }
                                else if (timeCount != previousTime && BlackOpsLibrary.ReadInt(Constants.C_MAPADDRESS) != 5)
                                {
                                    int whitePointsCount = BlackOpsLibrary.ReadInt(Constants.C_WHITEPOINTSADDRESS);
                                    int whiteKillsCount = BlackOpsLibrary.ReadInt(Constants.C_WHITEKILLSADDRESS);
                                    int whiteDownsCount = BlackOpsLibrary.ReadInt(Constants.C_WHITEDOWNSADDRESS);
                                    int whiteRevivesCount = BlackOpsLibrary.ReadInt(Constants.C_WHITEREVIVESADDRESS);

                                    int bluePointsCount = BlackOpsLibrary.ReadInt(Constants.C_BLUEPOINTSADDRESS);
                                    int blueKillsCount = BlackOpsLibrary.ReadInt(Constants.C_BLUEKILLSADDRESS);
                                    int blueDownsCount = BlackOpsLibrary.ReadInt(Constants.C_BLUEDOWNSADDRESS);
                                    int blueRevivesCount = BlackOpsLibrary.ReadInt(Constants.C_BLUEREVIVESADDRESS);

                                    int yellowPointsCount = BlackOpsLibrary.ReadInt(Constants.C_YELLOWPOINTSADDRESS);
                                    int yellowKillsCount = BlackOpsLibrary.ReadInt(Constants.C_YELLOWKILLSADDRESS);
                                    int yellowDownsCount = BlackOpsLibrary.ReadInt(Constants.C_YELLOWDOWNSADDRESS);
                                    int yellowRevivesCount = BlackOpsLibrary.ReadInt(Constants.C_YELLOWREVIVESADDRESS);

                                    int greenPointsCount = BlackOpsLibrary.ReadInt(Constants.C_GREENPOINTSADDRESS);
                                    int greenKillsCount = BlackOpsLibrary.ReadInt(Constants.C_GREENKILLSADDRESS);
                                    int greenDownsCount = BlackOpsLibrary.ReadInt(Constants.C_GREENDOWNSADDRESS);
                                    int greenRevivesCount = BlackOpsLibrary.ReadInt(Constants.C_GREENREVIVESADDRESS);

                                    bool mustUpdate = false;

                                    bool whiteHasHit = HasHitBox(whitePreviousPoints, whitePointsCount, whitePreviousKills, whiteKillsCount, whitePreviousDowns, whiteDownsCount);
                                    bool blueHasHit = HasHitBox(bluePreviousPoints, bluePointsCount, bluePreviousKills, blueKillsCount, bluePreviousDowns, blueDownsCount);
                                    bool yellowHasHit = HasHitBox(yellowPreviousPoints, yellowPointsCount, yellowPreviousKills, yellowKillsCount, yellowPreviousDowns, yellowDownsCount);
                                    bool greenHasHit = HasHitBox(greenPreviousPoints, greenPointsCount, greenPreviousKills, greenKillsCount, greenPreviousDowns, greenDownsCount);

                                    bool inventoryHasChanged = HasWhiteInventoryChanged() || HasBlueInventoryChanged() || HasYellowInventoryChanged() || HasGreenInventoryChanged();
                                    bool ammoBought = HasWhiteBoughtAmmo() || HasBlueBoughtAmmo() || HasYellowBoughtAmmo() || HasGreenBoughtAmmo();

                                    if (ammoBought) UpdateAmmos();

                                    if (whiteHasHit || blueHasHit || yellowHasHit || greenHasHit)
                                    {
                                        if (!inventoryHasChanged && !ammoBought)
                                        {
                                            SetBoxHits(GetBoxHits() + 1);
                                            SetCurrentBoxHits(GetCurrentBoxHits() + 1);

                                            isBoxRandomizing = true;
                                            timeSinceLastRandom = 0;

                                            m_WhiteTmpInventory = new List<int>(m_WhiteInventory);
                                            m_BlueTmpInventory = new List<int>(m_BlueInventory);
                                            m_YellowTmpInventory = new List<int>(m_YellowInventory);
                                            m_GreenTmpInventory = new List<int>(m_GreenInventory);

                                            if (SetupFinished)
                                            {
                                                AddBoxHitForEachWeapon(whiteHasHit, blueHasHit, yellowHasHit, greenHasHit);

                                                if (!PlayersHaveAllTradingWeapons())
                                                {
                                                    SetCurrentTradeHits(GetCurrentTradeHits() + 1);
                                                }
                                            }
                                        }

                                        mustUpdate = true;
                                    }

                                    bool whiteGotTeddy = HasGottenTeddyBear(whitePreviousPoints, whitePointsCount, whitePreviousKills, whiteKillsCount, whitePreviousRevives, whiteRevivesCount);
                                    bool blueGotTeddy = HasGottenTeddyBear(bluePreviousPoints, bluePointsCount, bluePreviousKills, blueKillsCount, bluePreviousRevives, blueRevivesCount);
                                    bool yellowGotTeddy = HasGottenTeddyBear(yellowPreviousPoints, yellowPointsCount, yellowPreviousKills, yellowKillsCount, yellowPreviousRevives, yellowRevivesCount);
                                    bool greenGotTeddy = HasGottenTeddyBear(greenPreviousPoints, greenPointsCount, greenPreviousKills, greenKillsCount, greenPreviousRevives, greenRevivesCount);

                                    if (whiteGotTeddy || blueGotTeddy || yellowGotTeddy || greenGotTeddy)
                                    {
                                        if (!m_WeaponsToNotCount.Any(x => x == "Teddy bear"))
                                        {
                                            if (SetupFinished)
                                            {
                                                m_WeaponsGotten["Teddy bear"].Add(GetBoxHits());
                                                ShowWeaponPopupFor("Teddy bear");
                                                mustUpdate = true;
                                            }

                                            SetLastBoxHits(GetCurrentBoxHits());
                                            SetCurrentBoxHits(0);
                                        }
                                    }

                                    if (inventoryHasChanged)
                                    {
                                        CheckTradeSoundOnInventoryChange();
                                        if (SetupFinished)
                                        {
                                            UpdateChangedWeapon();
                                            mustUpdate = true;
                                        }
                                        UpdateInventories();
                                    }

                                    if (isBoxRandomizing && timeSinceLastRandom >= 70)
                                    {
                                        isBoxRandomizing = false;
                                        if (SetupFinished)
                                        {
                                            foreach (var diff in m_WhiteTmpInventory.Where(p => !m_WhiteInventory.Contains(p)))
                                            {
                                                AddBoxHitForSpecificWeapon(diff);
                                                mustUpdate = true;
                                            }

                                            foreach (var diff in m_BlueTmpInventory.Where(p => !m_BlueInventory.Contains(p)))
                                            {
                                                AddBoxHitForSpecificWeapon(diff);
                                                mustUpdate = true;
                                            }

                                            foreach (var diff in m_YellowTmpInventory.Where(p => !m_YellowInventory.Contains(p)))
                                            {
                                                AddBoxHitForSpecificWeapon(diff);
                                                mustUpdate = true;
                                            }

                                            foreach (var diff in m_GreenTmpInventory.Where(p => !m_GreenInventory.Contains(p)))
                                            {
                                                AddBoxHitForSpecificWeapon(diff);
                                                mustUpdate = true;
                                            }
                                        }

                                        WeaponAverages = new ObservableCollection<KeyValuePair<string, float>>(WeaponAverages.ToList());
                                    }

                                    if (mustUpdate)
                                    {
                                        RefreshWeaponAverages();
                                    }

                                    previousTime = timeCount;

                                    whitePreviousPoints = whitePointsCount;
                                    whitePreviousKills = whiteKillsCount;
                                    whitePreviousDowns = whiteDownsCount;
                                    whitePreviousRevives = whiteRevivesCount;

                                    bluePreviousPoints = bluePointsCount;
                                    bluePreviousKills = blueKillsCount;
                                    bluePreviousDowns = blueDownsCount;
                                    bluePreviousRevives = blueRevivesCount;

                                    yellowPreviousPoints = yellowPointsCount;
                                    yellowPreviousKills = yellowKillsCount;
                                    yellowPreviousDowns = yellowDownsCount;
                                    yellowPreviousRevives = yellowRevivesCount;

                                    greenPreviousPoints = greenPointsCount;
                                    greenPreviousKills = greenKillsCount;
                                    greenPreviousDowns = greenDownsCount;
                                    greenPreviousRevives = greenRevivesCount;

                                    previousMoonTeddy = moonTeddy;
                                    timeSinceLastRandom++;

                                    UpdateAmmos();
                                }

                                Thread.Sleep(50);
                            }
                            catch (Exception)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        BlackOpsLibrary.m_ProcessHandle = null;
                        BlackOpsLibrary.m_BlackOpsProcess = null;
                        GameConnectedString = "Game not connected";
                        Thread.Sleep(1000);
                    }
                }
            });
        }

        private void RestartAutoFinishTimer()
        {
            _autoFinishTimer?.Dispose();
            _autoFinishTimer = new System.Threading.Timer(_ =>
            {
                if (!SetupFinished)
                    SetupFinished = true;
            }, null, TimeSpan.FromHours(1), System.Threading.Timeout.InfiniteTimeSpan);
        }

        private void ResetTracking()
        {
            SetBoxHits(0);
            SetCurrentBoxHits(0);
            SetCurrentTradeHits(0);
            SetLastTradeHits(0);
            SetLastBoxHits(0);
            SetTotalTradeCount(0);
            SetupFinished = false;

            var newGotten = m_WeaponsGotten.ToDictionary(kv => kv.Key, kv => new List<int>());
            m_WeaponsGotten = newGotten;

            var newHits = m_WeaponsBoxHits.ToDictionary(kv => kv.Key, kv => 0);
            m_WeaponsBoxHits = newHits;

            WeaponAverages = new ObservableCollection<KeyValuePair<string, float>>(WeaponAverages.Select(kv => new KeyValuePair<string, float>(kv.Key, 0f)));

            _worstTrades = new List<int>();
            UpdateWorstTradesString();
            _bestTrades = new List<int>();
            UpdateBestTradesString();
            RestartAutoFinishTimer();
        }

        private void UpdateSelectedMap(int mapId)
        {
            switch (mapId)
            {
                case 3:
                    if (SelectedMap != "Nacht Der Untoten")
                    {
                        m_WeaponsToDisplay = new List<string>();
                        SetSelectedMap("Nacht Der Untoten");
                        if (AutoDisplay) m_WeaponsToDisplay.Add("Thundergun");
                        m_WeaponsBeingTraded.Add("Thundergun");
                    }
                    break;
                case 5:
                    m_WeaponsToDisplay = new List<string>();
                    SetSelectedMap("MainMenu");
                    break;
                case 37:
                    if (SelectedMap != "Kino der Toten")
                    {
                        m_WeaponsToDisplay = new List<string>();
                        SetSelectedMap("Kino der Toten");
                        if (AutoDisplay) m_WeaponsToDisplay.Add("Thundergun");
                        m_WeaponsBeingTraded.Add("Thundergun");
                    }
                    break;
                case 50:
                    if (SelectedMap != "Verruckt")
                    {
                        m_WeaponsToDisplay = new List<string>();
                        SetSelectedMap("Verruckt");
                        if (AutoDisplay) { m_WeaponsToDisplay.AddRange(new[] { "Ray Gun", "Winter's howl", "M72 LAW" }); }
                        m_WeaponsBeingTraded.AddRange(new[] { "Ray Gun", "Winter's howl", "M72 LAW" });
                    }
                    break;
                case 77:
                    if (SelectedMap != "Five")
                    {
                        m_WeaponsToDisplay = new List<string>();
                        SetSelectedMap("Five");
                        if (AutoDisplay) { m_WeaponsToDisplay.AddRange(new[] { "Ray Gun", "Winter's howl", "M72 LAW" }); }
                        m_WeaponsBeingTraded.AddRange(new[] { "Ray Gun", "Winter's howl", "M72 LAW" });
                    }
                    break;
                case 85:
                    if (SelectedMap != "Call of the Dead")
                    {
                        m_WeaponsToDisplay = new List<string>();
                        SetSelectedMap("Call of the Dead");
                        if (AutoDisplay) { m_WeaponsToDisplay.AddRange(new[] { "VR-11", "Scavenger", "Ray Gun", "M72 LAW", "Crossbow" }); }
                        m_WeaponsBeingTraded.AddRange(new[] { "VR-11", "Scavenger", "Ray Gun", "M72 LAW" });
                    }
                    break;
                case 90:
                    if (SelectedMap != "Der Riese")
                    {
                        SetSelectedMap("Der Riese");
                        if (AutoDisplay) m_WeaponsToDisplay.Add("Wunderwaffe DG-2");
                        m_WeaponsBeingTraded.Add("Wunderwaffe DG-2");
                    }
                    break;
                case 99:
                    if (SelectedMap != "Shi No Numa")
                    {
                        m_WeaponsToDisplay = new List<string>();
                        SetSelectedMap("Shi No Numa");
                        if (AutoDisplay) m_WeaponsToDisplay.Add("Wunderwaffe DG-2");
                        m_WeaponsBeingTraded.Add("Wunderwaffe DG-2");
                    }
                    break;
                case 129:
                    if (SelectedMap != "Shangri-la")
                    {
                        m_WeaponsToDisplay = new List<string>();
                        SetSelectedMap("Shangri-la");
                        if (AutoDisplay) m_WeaponsToDisplay.Add("31-79 Jgb 215");
                        m_WeaponsBeingTraded.Add("31-79 Jgb 215");
                    }
                    break;
                case 163:
                    if (SelectedMap != "Moon")
                    {
                        m_WeaponsToDisplay = new List<string>();
                        SetSelectedMap("Moon");
                        if (AutoDisplay) m_WeaponsToDisplay.AddRange(new[] { "Wavegun", "Gersh", "QED" });
                        m_WeaponsBeingTraded.AddRange(new[] { "Gersh", "QED" });
                    }
                    break;
                case 189:
                    if (SelectedMap != "Ascension")
                    {
                        m_WeaponsToDisplay = new List<string>();
                        SetSelectedMap("Ascension");
                        if (AutoDisplay) m_WeaponsToDisplay.AddRange(new[] { "Thundergun", "Gersh" });
                        m_WeaponsBeingTraded.Add("Thundergun");
                    }
                    break;
                default:
                    break;
            }
        }

        private void InitializeInventories()
        {
            m_WhiteInventory = ReadInts(
                Constants.C_WHITEWEAPONSLOT1, Constants.C_WHITEWEAPONSLOT2, Constants.C_WHITEWEAPONSLOT3,
                Constants.C_WHITEWEAPONSLOT4, Constants.C_WHITEWEAPONSLOT5, Constants.C_WHITEWEAPONSLOT6,
                Constants.C_WHITEWEAPONSLOT7, Constants.C_WHITEWEAPONSLOT8, Constants.C_WHITEWEAPONSLOT9);

            m_BlueInventory = ReadInts(
                Constants.C_BLUEWEAPONSLOT1, Constants.C_BLUEWEAPONSLOT2, Constants.C_BLUEWEAPONSLOT3,
                Constants.C_BLUEWEAPONSLOT4, Constants.C_BLUEWEAPONSLOT5, Constants.C_BLUEWEAPONSLOT6,
                Constants.C_BLUEWEAPONSLOT7, Constants.C_BLUEWEAPONSLOT8, Constants.C_BLUEWEAPONSLOT9);

            m_YellowInventory = ReadInts(
                Constants.C_YELLOWWEAPONSLOT1, Constants.C_YELLOWWEAPONSLOT2, Constants.C_YELLOWWEAPONSLOT3,
                Constants.C_YELLOWWEAPONSLOT4, Constants.C_YELLOWWEAPONSLOT5, Constants.C_YELLOWWEAPONSLOT6,
                Constants.C_YELLOWWEAPONSLOT7, Constants.C_YELLOWWEAPONSLOT8, Constants.C_YELLOWWEAPONSLOT9);

            m_GreenInventory = ReadInts(
                Constants.C_GREENWEAPONSLOT1, Constants.C_GREENWEAPONSLOT2, Constants.C_GREENWEAPONSLOT3,
                Constants.C_GREENWEAPONSLOT4, Constants.C_GREENWEAPONSLOT5, Constants.C_GREENWEAPONSLOT6,
                Constants.C_GREENWEAPONSLOT7, Constants.C_GREENWEAPONSLOT8, Constants.C_GREENWEAPONSLOT9);

            m_WhiteAmmos = ReadInts(
                Constants.C_WHITEAMMOSSLOT1, Constants.C_WHITEAMMOSSLOT2, Constants.C_WHITEAMMOSSLOT3,
                Constants.C_WHITEAMMOSSLOT1_2, Constants.C_WHITEAMMOSSLOT2_2, Constants.C_WHITEAMMOSSLOT3_2);

            m_BlueAmmos = ReadInts(
                Constants.C_BLUEAMMOSSLOT1, Constants.C_BLUEAMMOSSLOT2, Constants.C_BLUEAMMOSSLOT3,
                Constants.C_BLUEAMMOSSLOT1_2, Constants.C_BLUEAMMOSSLOT2_2, Constants.C_BLUEAMMOSSLOT3_2);

            m_YellowAmmos = ReadInts(
                Constants.C_YELLOWAMMOSSLOT1, Constants.C_YELLOWAMMOSSLOT2, Constants.C_YELLOWAMMOSSLOT3,
                Constants.C_YELLOWAMMOSSLOT1_2, Constants.C_YELLOWAMMOSSLOT2_2, Constants.C_YELLOWAMMOSSLOT3_2);

            m_GreenAmmos = ReadInts(
                Constants.C_GREENAMMOSSLOT1, Constants.C_GREENAMMOSSLOT2, Constants.C_GREENAMMOSSLOT3,
                Constants.C_GREENAMMOSSLOT1_2, Constants.C_GREENAMMOSSLOT2_2, Constants.C_GREENAMMOSSLOT3_2);
        }

        private List<int> ReadInts(params int[] addresses)
        {
            var list = new List<int>(addresses.Length);
            foreach (var addr in addresses) list.Add(BlackOpsLibrary.ReadInt(addr));
            return list;
        }

        private void AddBoxHitForSpecificWeapon(int weaponKey)
        {
            var dict = GetWeaponsDictionary();
            if (dict == null) return;
            if (dict.TryGetValue(weaponKey, out var weapon) && !m_WeaponsToNotCount.Any(x => x == weapon))
            {
                m_WeaponsBoxHits[weapon]++;
            }
        }

        private static void SaveApplicationConfig(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)d;
            mainWindow.SaveApplicationConfig();
        }

        private bool HasHitBox(int previousPoints, int pointsCount, int previousKills, int killsCount, int previousDowns, int downsCount)
        {
            return previousDowns == downsCount &&
                   ((previousPoints - pointsCount >= 920 && previousPoints - pointsCount <= 950) ||
                    (previousPoints - pointsCount >= 880 && previousPoints - pointsCount <= 900 && killsCount - previousKills == 1) ||
                    (previousPoints - pointsCount == 10) ||
                    (SelectedMap == "Moon" && (previousPoints - pointsCount == 600 || previousPoints - pointsCount == 1200)));
        }

        private bool HasGottenTeddyBear(int previousPoints, int pointsCount, int previousKills, int killsCount, int previousRevives, int revivesCount)
        {
            return previousPoints - pointsCount == -950 && killsCount - previousKills == 0 && SelectedMap != "Moon" && previousRevives == revivesCount;
        }

        private void AddBoxHitForEachWeapon(bool whiteHasHit, bool blueHasHit, bool yellowHasHit, bool greenHasHit)
        {
            var dict = GetWeaponsDictionary();
            if (dict == null) return;
            ManageMapAverages(dict, whiteHasHit, blueHasHit, yellowHasHit, greenHasHit);
        }

        private void ManageMapAverages(Dictionary<int, string> weaponsDictionary, bool whiteHasHit, bool blueHasHit, bool yellowHasHit, bool greenHasHit)
        {
            var alreadyCheckedWeapons = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var kv in weaponsDictionary)
            {
                string weaponName = kv.Value;
                if (alreadyCheckedWeapons.Contains(weaponName)) continue;
                alreadyCheckedWeapons.Add(weaponName);

                bool inAnyInventory = m_WhiteInventory.Any(x => weaponsDictionary.ContainsKey(x) && weaponsDictionary[x] == weaponName)
                                   || m_BlueInventory.Any(x => weaponsDictionary.ContainsKey(x) && weaponsDictionary[x] == weaponName)
                                   || m_YellowInventory.Any(x => weaponsDictionary.ContainsKey(x) && weaponsDictionary[x] == weaponName)
                                   || m_GreenInventory.Any(x => weaponsDictionary.ContainsKey(x) && weaponsDictionary[x] == weaponName);

                bool isSpecial = s_SpecialWeapons.Contains(weaponsDictionary[kv.Key]);

                if (!inAnyInventory && isSpecial)
                {
                    if (!m_WeaponsToNotCount.Any(x => x == weaponName)) m_WeaponsBoxHits[weaponName]++;
                    continue;
                }

                if (!m_WhiteInventory.Any(x => weaponsDictionary.ContainsKey(x) && weaponsDictionary[x] == weaponName) && whiteHasHit && !isSpecial)
                {
                    if (!m_WeaponsToNotCount.Any(x => x == weaponName)) m_WeaponsBoxHits[weaponName]++;
                    continue;
                }

                if (!m_BlueInventory.Any(x => weaponsDictionary.ContainsKey(x) && weaponsDictionary[x] == weaponName) && blueHasHit && !isSpecial)
                {
                    if (!m_WeaponsToNotCount.Any(x => x == weaponName)) m_WeaponsBoxHits[weaponName]++;
                    continue;
                }

                if (!m_YellowInventory.Any(x => weaponsDictionary.ContainsKey(x) && weaponsDictionary[x] == weaponName) && yellowHasHit && !isSpecial)
                {
                    if (!m_WeaponsToNotCount.Any(x => x == weaponName)) m_WeaponsBoxHits[weaponName]++;
                    continue;
                }

                if (!m_GreenInventory.Any(x => weaponsDictionary.ContainsKey(x) && weaponsDictionary[x] == weaponName) && greenHasHit && !isSpecial)
                {
                    if (!m_WeaponsToNotCount.Any(x => x == weaponName)) m_WeaponsBoxHits[weaponName]++;
                }
            }

            if (SelectedMap != "Nacht Der Untoten" && SelectedMap != "Moon" && SelectedMap != "MainMenu")
            {
                if (!m_WeaponsToNotCount.Any(x => x == "Teddy bear"))
                {
                    m_WeaponsBoxHits["Teddy bear"]++;
                }
            }
        }

        private bool HasInventoryChanged(List<int> inventory, params int[] addresses)
        {
            for (int i = 0; i < addresses.Length; i++)
            {
                if (inventory[i] != BlackOpsLibrary.ReadInt(addresses[i])) return true;
            }
            return false;
        }

        private bool HasWhiteInventoryChanged() => HasInventoryChanged(m_WhiteInventory,
            Constants.C_WHITEWEAPONSLOT1, Constants.C_WHITEWEAPONSLOT2, Constants.C_WHITEWEAPONSLOT3,
            Constants.C_WHITEWEAPONSLOT4, Constants.C_WHITEWEAPONSLOT5, Constants.C_WHITEWEAPONSLOT6,
            Constants.C_WHITEWEAPONSLOT7, Constants.C_WHITEWEAPONSLOT8, Constants.C_WHITEWEAPONSLOT9);

        private bool HasBlueInventoryChanged() => HasInventoryChanged(m_BlueInventory,
            Constants.C_BLUEWEAPONSLOT1, Constants.C_BLUEWEAPONSLOT2, Constants.C_BLUEWEAPONSLOT3,
            Constants.C_BLUEWEAPONSLOT4, Constants.C_BLUEWEAPONSLOT5, Constants.C_BLUEWEAPONSLOT6,
            Constants.C_BLUEWEAPONSLOT7, Constants.C_BLUEWEAPONSLOT8, Constants.C_BLUEWEAPONSLOT9);

        private bool HasYellowInventoryChanged() => HasInventoryChanged(m_YellowInventory,
            Constants.C_YELLOWWEAPONSLOT1, Constants.C_YELLOWWEAPONSLOT2, Constants.C_YELLOWWEAPONSLOT3,
            Constants.C_YELLOWWEAPONSLOT4, Constants.C_YELLOWWEAPONSLOT5, Constants.C_YELLOWWEAPONSLOT6,
            Constants.C_YELLOWWEAPONSLOT7, Constants.C_YELLOWWEAPONSLOT8, Constants.C_YELLOWWEAPONSLOT9);

        private bool HasGreenInventoryChanged() => HasInventoryChanged(m_GreenInventory,
            Constants.C_GREENWEAPONSLOT1, Constants.C_GREENWEAPONSLOT2, Constants.C_GREENWEAPONSLOT3,
            Constants.C_GREENWEAPONSLOT4, Constants.C_GREENWEAPONSLOT5, Constants.C_GREENWEAPONSLOT6,
            Constants.C_GREENWEAPONSLOT7, Constants.C_GREENWEAPONSLOT8, Constants.C_GREENWEAPONSLOT9);

        private bool HasAmmoBought(List<int> ammos, params int[] addresses)
        {
            for (int i = 0; i < addresses.Length; i++)
            {
                if (ammos[i] < BlackOpsLibrary.ReadInt(addresses[i])) return true;
            }
            return false;
        }

        private bool HasWhiteBoughtAmmo() => HasAmmoBought(m_WhiteAmmos,
            Constants.C_WHITEAMMOSSLOT1, Constants.C_WHITEAMMOSSLOT2, Constants.C_WHITEAMMOSSLOT3,
            Constants.C_WHITEAMMOSSLOT1_2, Constants.C_WHITEAMMOSSLOT2_2, Constants.C_WHITEAMMOSSLOT3_2);

        private bool HasBlueBoughtAmmo() => HasAmmoBought(m_BlueAmmos,
            Constants.C_BLUEAMMOSSLOT1, Constants.C_BLUEAMMOSSLOT2, Constants.C_BLUEAMMOSSLOT3,
            Constants.C_BLUEAMMOSSLOT1_2, Constants.C_BLUEAMMOSSLOT2_2, Constants.C_BLUEAMMOSSLOT3_2);

        private bool HasYellowBoughtAmmo() => HasAmmoBought(m_YellowAmmos,
            Constants.C_YELLOWAMMOSSLOT1, Constants.C_YELLOWAMMOSSLOT2, Constants.C_YELLOWAMMOSSLOT3,
            Constants.C_YELLOWAMMOSSLOT1_2, Constants.C_YELLOWAMMOSSLOT2_2, Constants.C_YELLOWAMMOSSLOT3_2);

        private bool HasGreenBoughtAmmo() => HasAmmoBought(m_GreenAmmos,
            Constants.C_GREENAMMOSSLOT1, Constants.C_GREENAMMOSSLOT2, Constants.C_GREENAMMOSSLOT3,
            Constants.C_GREENAMMOSSLOT1_2, Constants.C_GREENAMMOSSLOT2_2, Constants.C_GREENAMMOSSLOT3_2);

        private void UpdateAmmos()
        {
            m_WhiteAmmos = ReadInts(
                Constants.C_WHITEAMMOSSLOT1, Constants.C_WHITEAMMOSSLOT2, Constants.C_WHITEAMMOSSLOT3,
                Constants.C_WHITEAMMOSSLOT1_2, Constants.C_WHITEAMMOSSLOT2_2, Constants.C_WHITEAMMOSSLOT3_2);

            m_BlueAmmos = ReadInts(
                Constants.C_BLUEAMMOSSLOT1, Constants.C_BLUEAMMOSSLOT2, Constants.C_BLUEAMMOSSLOT3,
                Constants.C_BLUEAMMOSSLOT1_2, Constants.C_BLUEAMMOSSLOT2_2, Constants.C_BLUEAMMOSSLOT3_2);

            m_YellowAmmos = ReadInts(
                Constants.C_YELLOWAMMOSSLOT1, Constants.C_YELLOWAMMOSSLOT2, Constants.C_YELLOWAMMOSSLOT3,
                Constants.C_YELLOWAMMOSSLOT1_2, Constants.C_YELLOWAMMOSSLOT2_2, Constants.C_YELLOWAMMOSSLOT3_2);

            m_GreenAmmos = ReadInts(
                Constants.C_GREENAMMOSSLOT1, Constants.C_GREENAMMOSSLOT2, Constants.C_GREENAMMOSSLOT3,
                Constants.C_GREENAMMOSSLOT1_2, Constants.C_GREENAMMOSSLOT2_2, Constants.C_GREENAMMOSSLOT3_2);
        }

        private void UpdateChangedWeapon()
        {
            int changedWeaponId = GetChangedWeaponId();
            if (changedWeaponId == -1) return;

            var dict = GetWeaponsDictionary();
            if (dict != null && dict.ContainsKey(changedWeaponId))
            {
                var papSet = GetPapSetForSelectedMap();
                bool isPap = papSet != null && papSet.Contains(changedWeaponId);

                if (!isPap)
                {
                    string gottenWeapon = dict[changedWeaponId];
                    if (!m_WeaponsToNotCount.Any(x => x == gottenWeapon))
                    {
                        m_WeaponsGotten[gottenWeapon].Add(GetBoxHits());
                        ShowWeaponPopupFor(gottenWeapon);
                    }

                    if (m_WeaponsBeingTraded.Any(x => x == gottenWeapon))
                    {
                        int tradeHits = GetCurrentTradeHits();
                        SetLastTradeHits(tradeHits);
                        SetCurrentTradeHits(0);
                        int newTradeCount = GetTotalTradeCount() + 1;
                        SetTotalTradeCount(newTradeCount);

                        _worstTrades.Add(tradeHits);
                        _worstTrades = _worstTrades.OrderByDescending(x => x).ToList();
                        UpdateWorstTradesString();

                        _bestTrades.Add(tradeHits);
                        _bestTrades = _bestTrades.OrderBy(x => x).ToList();
                        UpdateBestTradesString();
                        // Le son (arme perso ou trade) est déjà joué dans CheckTradeSoundOnInventoryChange()
                    }
                }
            }

            UpdateInventories();
        }

        private int GetChangedWeaponId()
        {
            var whiteSlots = new[]
            {
                Constants.C_WHITEWEAPONSLOT1, Constants.C_WHITEWEAPONSLOT2, Constants.C_WHITEWEAPONSLOT3,
                Constants.C_WHITEWEAPONSLOT4, Constants.C_WHITEWEAPONSLOT5, Constants.C_WHITEWEAPONSLOT6,
                Constants.C_WHITEWEAPONSLOT7, Constants.C_WHITEWEAPONSLOT8, Constants.C_WHITEWEAPONSLOT9
            };

            var blueSlots = new[]
            {
                Constants.C_BLUEWEAPONSLOT1, Constants.C_BLUEWEAPONSLOT2, Constants.C_BLUEWEAPONSLOT3,
                Constants.C_BLUEWEAPONSLOT4, Constants.C_BLUEWEAPONSLOT5, Constants.C_BLUEWEAPONSLOT6,
                Constants.C_BLUEWEAPONSLOT7, Constants.C_BLUEWEAPONSLOT8, Constants.C_BLUEWEAPONSLOT9
            };

            var yellowSlots = new[]
            {
                Constants.C_YELLOWWEAPONSLOT1, Constants.C_YELLOWWEAPONSLOT2, Constants.C_YELLOWWEAPONSLOT3,
                Constants.C_YELLOWWEAPONSLOT4, Constants.C_YELLOWWEAPONSLOT5, Constants.C_YELLOWWEAPONSLOT6,
                Constants.C_YELLOWWEAPONSLOT7, Constants.C_YELLOWWEAPONSLOT8, Constants.C_YELLOWWEAPONSLOT9
            };

            var greenSlots = new[]
            {
                Constants.C_GREENWEAPONSLOT1, Constants.C_GREENWEAPONSLOT2, Constants.C_GREENWEAPONSLOT3,
                Constants.C_GREENWEAPONSLOT4, Constants.C_GREENWEAPONSLOT5, Constants.C_GREENWEAPONSLOT6,
                Constants.C_GREENWEAPONSLOT7, Constants.C_GREENWEAPONSLOT8, Constants.C_GREENWEAPONSLOT9
            };

            for (int i = 0; i < whiteSlots.Length; i++)
            {
                int read = BlackOpsLibrary.ReadInt(whiteSlots[i]);
                if (m_WhiteInventory[i] != read) return read;
            }

            for (int i = 0; i < blueSlots.Length; i++)
            {
                int read = BlackOpsLibrary.ReadInt(blueSlots[i]);
                if (m_BlueInventory[i] != read) return read;
            }

            for (int i = 0; i < yellowSlots.Length; i++)
            {
                int read = BlackOpsLibrary.ReadInt(yellowSlots[i]);
                if (m_YellowInventory[i] != read) return read;
            }

            for (int i = 0; i < greenSlots.Length; i++)
            {
                int read = BlackOpsLibrary.ReadInt(greenSlots[i]);
                if (m_GreenInventory[i] != read) return read;
            }

            return -1;
        }

        private void UpdateInventories()
        {
            var white = ReadInts(
                Constants.C_WHITEWEAPONSLOT1, Constants.C_WHITEWEAPONSLOT2, Constants.C_WHITEWEAPONSLOT3,
                Constants.C_WHITEWEAPONSLOT4, Constants.C_WHITEWEAPONSLOT5, Constants.C_WHITEWEAPONSLOT6,
                Constants.C_WHITEWEAPONSLOT7, Constants.C_WHITEWEAPONSLOT8, Constants.C_WHITEWEAPONSLOT9);
            m_WhiteInventory = white;

            var blue = ReadInts(
                Constants.C_BLUEWEAPONSLOT1, Constants.C_BLUEWEAPONSLOT2, Constants.C_BLUEWEAPONSLOT3,
                Constants.C_BLUEWEAPONSLOT4, Constants.C_BLUEWEAPONSLOT5, Constants.C_BLUEWEAPONSLOT6,
                Constants.C_BLUEWEAPONSLOT7, Constants.C_BLUEWEAPONSLOT8, Constants.C_BLUEWEAPONSLOT9);
            m_BlueInventory = blue;

            var yellow = ReadInts(
                Constants.C_YELLOWWEAPONSLOT1, Constants.C_YELLOWWEAPONSLOT2, Constants.C_YELLOWWEAPONSLOT3,
                Constants.C_YELLOWWEAPONSLOT4, Constants.C_YELLOWWEAPONSLOT5, Constants.C_YELLOWWEAPONSLOT6,
                Constants.C_YELLOWWEAPONSLOT7, Constants.C_YELLOWWEAPONSLOT8, Constants.C_YELLOWWEAPONSLOT9);
            m_YellowInventory = yellow;

            var green = ReadInts(
                Constants.C_GREENWEAPONSLOT1, Constants.C_GREENWEAPONSLOT2, Constants.C_GREENWEAPONSLOT3,
                Constants.C_GREENWEAPONSLOT4, Constants.C_GREENWEAPONSLOT5, Constants.C_GREENWEAPONSLOT6,
                Constants.C_GREENWEAPONSLOT7, Constants.C_GREENWEAPONSLOT8, Constants.C_GREENWEAPONSLOT9);
            m_GreenInventory = green;
        }

        private int GetBoxHits()
        {
            return Application.Current.Dispatcher.Invoke(() => BoxHitsCount);
        }

        private void SetBoxHits(int value)
        {
            Application.Current.Dispatcher.Invoke(() => (BoxHitsCount = value));
        }

        private int GetCurrentBoxHits() => Application.Current.Dispatcher.Invoke(() => CurrentBoxHitsCount);
        private int GetCurrentTradeHits() => Application.Current.Dispatcher.Invoke(() => CurrentTradeHitsCount);
        private int GetTotalTradeCount() => Application.Current.Dispatcher.Invoke(() => TotalTradeCount);

        private void SetCurrentBoxHits(int value) => Application.Current.Dispatcher.Invoke(() => (CurrentBoxHitsCount = value));
        private void SetLastBoxHits(int value) => Application.Current.Dispatcher.Invoke(() => (LastBoxHitsCount = value));

        private void SetCurrentTradeHits(int value) => Application.Current.Dispatcher.Invoke(() => (CurrentTradeHitsCount = value));
        private void SetLastTradeHits(int value) => Application.Current.Dispatcher.Invoke(() => (LastTradeHitsCount = value));
        private void SetTotalTradeCount(int value) => Application.Current.Dispatcher.Invoke(() => (TotalTradeCount = value));

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void RightClick_Ascension(object sender, RoutedEventArgs e) => SetSelectedMap("Ascension");
        private void RightClick_CallOfTheDead(object sender, RoutedEventArgs e) => SetSelectedMap("Call of the Dead");
        private void RightClick_DerRiese(object sender, RoutedEventArgs e) => SetSelectedMap("Der Riese");
        private void RightClick_Five(object sender, RoutedEventArgs e) => SetSelectedMap("Five");
        private void RightClick_KinoDerToten(object sender, RoutedEventArgs e) => SetSelectedMap("Kino der Toten");
        private void RightClick_Moon(object sender, RoutedEventArgs e) => SetSelectedMap("Moon");
        private void RightClick_NachtDerUntoten(object sender, RoutedEventArgs e) => SetSelectedMap("Nacht Der Untoten");
        private void RightClick_ShangriLa(object sender, RoutedEventArgs e) => SetSelectedMap("Shangri-la");
        private void RightClick_ShiNoNuma(object sender, RoutedEventArgs e) => SetSelectedMap("Shi No Numa");
        private void RightClick_Verruckt(object sender, RoutedEventArgs e) => SetSelectedMap("Verruckt");
        private void RightClick_Export(object sender, RoutedEventArgs e) => ExportGame();
        private void RightClick_Exit(object sender, RoutedEventArgs e) => BoxAverageTrackerWindow.Close();
        private void RightClick_WorstTrades(object sender, RoutedEventArgs e) => DisplayWorstTrades();

        private void SetSelectedMap(string map)
        {
            Dispatcher.Invoke(() =>
            {
                if (SelectedMap != map) SelectedMap = map;
            });
        }

        private void ExportGame()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string filename = SelectedMap + "_" + (DateTime.Now.ToString().Replace("/", "_").Replace(":", "-")) + ".txt";
            string createdFilepath = Path.Combine(currentDir, filename);

            var toWrite = new StringWriter();

            toWrite.WriteLine("Total hits : " + BoxHitsCount);
            toWrite.WriteLine("Current hits : " + CurrentBoxHitsCount);
            toWrite.WriteLine("Current trade hits : " + CurrentTradeHitsCount);
            toWrite.WriteLine("Total trades : " + TotalTradeCount);

            foreach (var weapon in m_WeaponsGotten)
            {
                if (weapon.Value.Count > 0)
                {
                    toWrite.WriteLine($"{weapon.Key}= {weapon.Value.Count} (Average = {(float)m_WeaponsBoxHits[weapon.Key] / (float)weapon.Value.Count})");
                }
            }

            File.WriteAllText(createdFilepath, toWrite.ToString());
            Process.Start(createdFilepath);
        }

        private void LoadGame(object sender, RoutedEventArgs e) => LoadGame();

        private void LoadGame()
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            string[] fileText = File.ReadAllLines(openFileDialog.FileName);
            string map = fileText.FirstOrDefault(x => x.StartsWith("Map="))?.Substring("Map=".Length) ?? "";
            string setup = fileText.FirstOrDefault(x => x.StartsWith("Setup="))?.Substring("Setup=".Length) ?? "False";
            string totalHits = fileText.FirstOrDefault(x => x.StartsWith("Total hits="))?.Substring("Total hits=".Length) ?? "0";
            string currentHits = fileText.FirstOrDefault(x => x.StartsWith("Current hits="))?.Substring("Current hits=".Length) ?? "0";
            string tradeHits = fileText.FirstOrDefault(x => x.StartsWith("Current trade hits="))?.Substring("Current trade hits=".Length) ?? "0";
            string totalTrades = fileText.FirstOrDefault(x => x.StartsWith("Total trades="))?.Substring("Total trades=".Length) ?? "0";

            var weaponsHits = new Dictionary<string, int>(m_WeaponsBoxHits);
            var weaponsGotten = new Dictionary<string, List<int>>(m_WeaponsGotten);

            foreach (string line in fileText)
            {
                if (line.StartsWith("WG-"))
                {
                    int equalLoc = line.IndexOf("=", StringComparison.Ordinal);
                    string weaponName = line.Substring("WG-".Length, equalLoc - "WG-".Length);
                    List<int> gotten = line.Substring(line.LastIndexOf('=') + 1).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    weaponsGotten[weaponName] = gotten;
                }
                else if (line.StartsWith("WH-"))
                {
                    int equalLoc = line.IndexOf("=", StringComparison.Ordinal);
                    string weaponName = line.Substring("WH-".Length, equalLoc - "WH-".Length);
                    int hits = int.Parse(line.Substring(line.LastIndexOf('=') + 1));
                    weaponsHits[weaponName] = hits;
                }
            }

            SelectedMap = map;
            SetBoxHits(int.Parse(totalHits));
            SetTotalTradeCount(int.Parse(totalTrades));
            m_WeaponsBoxHits = weaponsHits;
            m_WeaponsGotten = weaponsGotten;
            SetupFinished = bool.Parse(setup);

            RefreshWeaponAverages();
        }

        private void SaveGame(object sender, RoutedEventArgs e) => SaveGame();

        private void SaveGame()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string fileName = "Saved_" + SelectedMap + "_" + (DateTime.Now.ToString().Replace("/", "_").Replace(":", "-")) + ".txt";
            string createdFilePath = Path.Combine(currentDir, fileName);

            var toWrite = new StringWriter();
            toWrite.WriteLine("Map=" + SelectedMap);
            toWrite.WriteLine();
            toWrite.WriteLine("Setup=" + SetupFinished);
            toWrite.WriteLine();
            toWrite.WriteLine("Total hits=" + BoxHitsCount);
            toWrite.WriteLine();
            toWrite.WriteLine("Current hits=" + CurrentBoxHitsCount);
            toWrite.WriteLine();
            toWrite.WriteLine("Current trade hits=" + CurrentTradeHitsCount);
            toWrite.WriteLine();
            toWrite.WriteLine("Total trades=" + TotalTradeCount);
            toWrite.WriteLine();

            foreach (var weapon in m_WeaponsGotten)
            {
                if (weapon.Value.Count > 0)
                {
                    toWrite.WriteLine("WG-" + weapon.Key + "=" + string.Join(",", weapon.Value));
                }
            }
            toWrite.WriteLine();

            foreach (var weapon in m_WeaponsBoxHits)
            {
                toWrite.WriteLine("WH-" + weapon.Key + "=" + weapon.Value);
            }

            File.WriteAllText(createdFilePath, toWrite.ToString());
            Process.Start(createdFilePath);
        }

        private void SetBoxHitsFont(object sender, RoutedEventArgs e)
        {
            var color = (BoxHitsForeground as System.Windows.Media.SolidColorBrush).Color;
            System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
            if (BoxHitsFontStyle == FontStyles.Italic) fontStyle = System.Drawing.FontStyle.Italic;
            if (BoxHitsFontWeight == FontWeights.Bold) fontStyle |= System.Drawing.FontStyle.Bold;
            var fontDialog = new System.Windows.Forms.FontDialog()
            {
                AllowVerticalFonts = false, MinSize = 0, MaxSize = 0,
                Font = new System.Drawing.Font(BoxHitsFontFamily.Source, BoxHitsSize, fontStyle),
                Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B),
                ShowColor = true,
            };
            if (fontDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                BoxHitsFontFamily = new System.Windows.Media.FontFamily(fontDialog.Font.FontFamily.Name);
                BoxHitsForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(fontDialog.Color.R, fontDialog.Color.G, fontDialog.Color.B));
                BoxHitsSize = fontDialog.Font.Size;
                BoxHitsFontStyle = fontDialog.Font.Style.HasFlag(System.Drawing.FontStyle.Italic) ? FontStyles.Italic : FontStyles.Normal;
                BoxHitsFontWeight = fontDialog.Font.Style.HasFlag(System.Drawing.FontStyle.Bold) ? FontWeights.Bold : FontWeights.Regular;
            }
        }

        private void SetCurrentBoxFont(object sender, RoutedEventArgs e)
        {
            var color = (CurrentBoxForeground as System.Windows.Media.SolidColorBrush).Color;
            System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
            if (CurrentBoxFontStyle == FontStyles.Italic) fontStyle = System.Drawing.FontStyle.Italic;
            if (CurrentBoxFontWeight == FontWeights.Bold) fontStyle |= System.Drawing.FontStyle.Bold;
            var fontDialog = new System.Windows.Forms.FontDialog()
            {
                AllowVerticalFonts = false, MinSize = 0, MaxSize = 0,
                Font = new System.Drawing.Font(CurrentBoxFontFamily.Source, CurrentBoxSize, fontStyle),
                Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B),
                ShowColor = true,
            };
            if (fontDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                CurrentBoxFontFamily = new System.Windows.Media.FontFamily(fontDialog.Font.FontFamily.Name);
                CurrentBoxForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(fontDialog.Color.R, fontDialog.Color.G, fontDialog.Color.B));
                CurrentBoxSize = fontDialog.Font.Size;
                CurrentBoxFontStyle = fontDialog.Font.Style.HasFlag(System.Drawing.FontStyle.Italic) ? FontStyles.Italic : FontStyles.Normal;
                CurrentBoxFontWeight = fontDialog.Font.Style.HasFlag(System.Drawing.FontStyle.Bold) ? FontWeights.Bold : FontWeights.Regular;
            }
        }

        private void SetCurrentTradeFont(object sender, RoutedEventArgs e)
        {
            var color = (CurrentTradeForeground as System.Windows.Media.SolidColorBrush).Color;
            System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
            if (CurrentTradeFontStyle == FontStyles.Italic) fontStyle = System.Drawing.FontStyle.Italic;
            if (CurrentTradeFontWeight == FontWeights.Bold) fontStyle |= System.Drawing.FontStyle.Bold;
            var fontDialog = new System.Windows.Forms.FontDialog()
            {
                AllowVerticalFonts = false, MinSize = 0, MaxSize = 0,
                Font = new System.Drawing.Font(CurrentTradeFontFamily.Source, CurrentTradeSize, fontStyle),
                Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B),
                ShowColor = true,
            };
            if (fontDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                CurrentTradeFontFamily = new System.Windows.Media.FontFamily(fontDialog.Font.FontFamily.Name);
                CurrentTradeForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(fontDialog.Color.R, fontDialog.Color.G, fontDialog.Color.B));
                CurrentTradeSize = fontDialog.Font.Size;
                CurrentTradeFontStyle = fontDialog.Font.Style.HasFlag(System.Drawing.FontStyle.Italic) ? FontStyles.Italic : FontStyles.Normal;
                CurrentTradeFontWeight = fontDialog.Font.Style.HasFlag(System.Drawing.FontStyle.Bold) ? FontWeights.Bold : FontWeights.Regular;
            }
        }

        private void SetTotalTradeFont(object sender, RoutedEventArgs e)
        {
            var color = (TotalTradeForeground as System.Windows.Media.SolidColorBrush).Color;
            System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
            if (TotalTradeFontStyle == FontStyles.Italic) fontStyle = System.Drawing.FontStyle.Italic;
            if (TotalTradeFontWeight == FontWeights.Bold) fontStyle |= System.Drawing.FontStyle.Bold;
            var fontDialog = new System.Windows.Forms.FontDialog()
            {
                AllowVerticalFonts = false, MinSize = 0, MaxSize = 0,
                Font = new System.Drawing.Font(TotalTradeFontFamily.Source, TotalTradeSize, fontStyle),
                Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B),
                ShowColor = true,
            };
            if (fontDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                TotalTradeFontFamily = new System.Windows.Media.FontFamily(fontDialog.Font.FontFamily.Name);
                TotalTradeForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(fontDialog.Color.R, fontDialog.Color.G, fontDialog.Color.B));
                TotalTradeSize = fontDialog.Font.Size;
                TotalTradeFontStyle = fontDialog.Font.Style.HasFlag(System.Drawing.FontStyle.Italic) ? FontStyles.Italic : FontStyles.Normal;
                TotalTradeFontWeight = fontDialog.Font.Style.HasFlag(System.Drawing.FontStyle.Bold) ? FontWeights.Bold : FontWeights.Regular;
            }
        }

        private void ToggleSetupFinished(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem) SetupFinished = menuItem.IsChecked;
        }

        private void AutoDisplayClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem) AutoDisplay = menuItem.IsChecked;
        }

        private void ToggleShowCurrentBoxHits(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem) ShowCurrentBoxHits = menuItem.IsChecked;
        }

        private void ToggleShowCurrentTradeHits(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem) ShowCurrentTradeHits = menuItem.IsChecked;
        }

        private void ToggleShowBestTrades(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem) ShowBestTrades = menuItem.IsChecked;
        }

        private void UpdateBestTradesString()
        {
            if (_bestTrades.Count == 0)
            {
                BestTradesString = "";
                return;
            }
            var top = _bestTrades.OrderBy(x => x).Take(TradesDisplayCount).ToList();
            var parts = top.Select((v, i) => $"#{i + 1}: {v}");
            BestTradesString = " Best trades : " + string.Join(" | ", parts);
        }

        private void TradesDisplaySlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TradesDisplayCount = (int)e.NewValue;
            UpdateWorstTradesString();
            UpdateBestTradesString();
            SaveApplicationConfig();
        }

        private void ToggleSoundsEnabled(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem) SoundsEnabled = menuItem.IsChecked;
        }

        private void ToggleShowWeaponPopup(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem) ShowWeaponPopup = menuItem.IsChecked;
        }

        /// <summary>
        /// Affiche brièvement sous les averages : "China Lake  avg 12.50  (x3)".
        /// Appelé depuis le thread de tracking, donc tout passe par le Dispatcher.
        /// </summary>
        private void ShowWeaponPopupFor(string weaponName)
        {
            if (!m_WeaponsGotten.TryGetValue(weaponName, out var gotten) || gotten.Count == 0) return;

            int timesGotten = gotten.Count;
            int hits = m_WeaponsBoxHits.TryGetValue(weaponName, out var h) ? h : 0;
            float average = (float)hits / timesGotten;
            string text = $"{weaponName}  avg {average:N2}  (x{timesGotten})";

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!(bool)GetValue(ShowWeaponPopupProperty)) return;

                SetValue(WeaponPopupTextProperty, text);

                double total = Math.Max(1.0, m_WeaponPopupSeconds);
                var anim = new System.Windows.Media.Animation.DoubleAnimationUsingKeyFrames();
                anim.KeyFrames.Add(new System.Windows.Media.Animation.LinearDoubleKeyFrame(1, System.Windows.Media.Animation.KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15))));
                anim.KeyFrames.Add(new System.Windows.Media.Animation.LinearDoubleKeyFrame(1, System.Windows.Media.Animation.KeyTime.FromTimeSpan(TimeSpan.FromSeconds(total - 0.5))));
                anim.KeyFrames.Add(new System.Windows.Media.Animation.LinearDoubleKeyFrame(0, System.Windows.Media.Animation.KeyTime.FromTimeSpan(TimeSpan.FromSeconds(total))));

                // BeginAnimation remplace l'animation en cours : si on chope 2 armes d'affilée, le popup repart de zéro
                WeaponPopup.BeginAnimation(OpacityProperty, anim);
            }));
        }

        private void ToggleShowWorstTrades(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem) ShowWorstTrades = menuItem.IsChecked;
        }

        private void ToggleShowTotalTradeCount(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem) ShowTotalTradeCount = menuItem.IsChecked;
        }

        private void AboutClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Version 2.0 (build " + Updater.LocalBuild + ")", "About", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        private async void CheckForUpdatesClick(object sender, RoutedEventArgs e)
        {
            await Updater.CheckForUpdatesAsync(this, silentIfUpToDate: false);
        }

        private void SetAverageTextFont(object sender, RoutedEventArgs e)
        {
            var color = (AverageTextForeground as System.Windows.Media.SolidColorBrush).Color;
            System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;

            if (AverageTextFontStyle == FontStyles.Italic) fontStyle = System.Drawing.FontStyle.Italic;
            if (AverageTextFontWeight == FontWeights.Bold) fontStyle = System.Drawing.FontStyle.Bold;

            var fontDialog = new System.Windows.Forms.FontDialog()
            {
                AllowVerticalFonts = false,
                MinSize = 0,
                MaxSize = 0,
                Font = new System.Drawing.Font(AverageTextFontFamily.Source, AverageTextSize, fontStyle),
                Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B),
                ShowColor = true,
            };

            if (fontDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                AverageTextFontFamily = new System.Windows.Media.FontFamily(fontDialog.Font.FontFamily.Name);
                AverageTextForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(fontDialog.Color.R, fontDialog.Color.G, fontDialog.Color.B));
                AverageTextSize = fontDialog.Font.Size;
                AverageTextFontStyle = fontDialog.Font.Style.HasFlag(System.Drawing.FontStyle.Italic) ? FontStyles.Italic : FontStyles.Normal;
                AverageTextFontWeight = fontDialog.Font.Style.HasFlag(System.Drawing.FontStyle.Bold) ? FontWeights.Bold : FontWeights.Regular;
            }
        }

        string GetConfigValue(string[] configText, string key, string defaultValue)
        {
            var line = configText.FirstOrDefault(x => x.StartsWith(key + "="));
            return line == null ? defaultValue : line.Substring(key.Length + 1);
        }

        private void LoadApplicationConfig()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "ApplicationConfig.txt");
            if (!File.Exists(filePath)) return;

            string[] fileText = File.ReadAllLines(filePath);

            string boxHitsFont = GetConfigValue(fileText, "BoxHitsFont", "");
            string boxHitsColor = GetConfigValue(fileText, "BoxHitsColor", "#FFFFFFFF");
            string boxHitsSize = GetConfigValue(fileText, "BoxHitsSize", "30");
            string boxHitsStyle = GetConfigValue(fileText, "BoxHitsStyle", "Normal");
            string boxHitsWeight = GetConfigValue(fileText, "BoxHitsWeight", "Normal");

            string currentBoxFont = GetConfigValue(fileText, "CurrentBoxFont", boxHitsFont);
            string currentBoxColor = GetConfigValue(fileText, "CurrentBoxColor", boxHitsColor);
            string currentBoxSize = GetConfigValue(fileText, "CurrentBoxSize", boxHitsSize);
            string currentBoxStyle = GetConfigValue(fileText, "CurrentBoxStyle", boxHitsStyle);
            string currentBoxWeight = GetConfigValue(fileText, "CurrentBoxWeight", boxHitsWeight);

            string currentTradeFont = GetConfigValue(fileText, "CurrentTradeFont", boxHitsFont);
            string currentTradeColor = GetConfigValue(fileText, "CurrentTradeColor", boxHitsColor);
            string currentTradeSize = GetConfigValue(fileText, "CurrentTradeSize", boxHitsSize);
            string currentTradeStyle = GetConfigValue(fileText, "CurrentTradeStyle", boxHitsStyle);
            string currentTradeWeight = GetConfigValue(fileText, "CurrentTradeWeight", boxHitsWeight);

            string totalTradeFont = GetConfigValue(fileText, "TotalTradeFont", boxHitsFont);
            string totalTradeColor = GetConfigValue(fileText, "TotalTradeColor", boxHitsColor);
            string totalTradeSize = GetConfigValue(fileText, "TotalTradeSize", boxHitsSize);
            string totalTradeStyle = GetConfigValue(fileText, "TotalTradeStyle", boxHitsStyle);
            string totalTradeWeight = GetConfigValue(fileText, "TotalTradeWeight", boxHitsWeight);

            string averagesFont = GetConfigValue(fileText, "AveragesFont", "");
            string averagesColor = GetConfigValue(fileText, "AveragesColor", "#FFFFFFFF");
            string averagesSize = GetConfigValue(fileText, "AveragesSize", "12.5");
            string averagesStyle = GetConfigValue(fileText, "AveragesStyle", "Normal");
            string averagesWeight = GetConfigValue(fileText, "AveragesWeight", "Normal");
            string totalHeight = GetConfigValue(fileText, "TotalHeight", "100");
            string totalWidth = GetConfigValue(fileText, "TotalWidth", "275");
            string autoDisplay = GetConfigValue(fileText, "AutoDisplay", "True");
            string showCurrentBoxHits = GetConfigValue(fileText, "ShowCurrentBoxHits", "False");
            string showCurrentTradeHits = GetConfigValue(fileText, "ShowCurrentTradeHits", "True");
            string showTotalTradeCount = GetConfigValue(fileText, "ShowTotalTradeCount", "True");
            string showWorstTrades = GetConfigValue(fileText, "ShowWorstTrades", "False");
            string showBestTrades = GetConfigValue(fileText, "ShowBestTrades", "False");
            string tradesDisplayCount = GetConfigValue(fileText, "TradesDisplayCount", "3");
            string soundsEnabled = GetConfigValue(fileText, "SoundsEnabled", "True");
            string showWeaponPopup = GetConfigValue(fileText, "ShowWeaponPopup", "True");
            string weaponPopupSeconds = GetConfigValue(fileText, "WeaponPopupSeconds", "4");

            BoxHitsFontFamily = new System.Windows.Media.FontFamily(boxHitsFont);
            BoxHitsForeground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(boxHitsColor));
            BoxHitsSize = float.Parse(boxHitsSize);
            BoxHitsFontStyle = (FontStyle)(new FontStyleConverter()).ConvertFromString(boxHitsStyle);
            BoxHitsFontWeight = (FontWeight)(new FontWeightConverter()).ConvertFromString(boxHitsWeight);

            CurrentBoxFontFamily = new System.Windows.Media.FontFamily(currentBoxFont);
            CurrentBoxForeground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(currentBoxColor));
            CurrentBoxSize = float.Parse(currentBoxSize);
            CurrentBoxFontStyle = (FontStyle)(new FontStyleConverter()).ConvertFromString(currentBoxStyle);
            CurrentBoxFontWeight = (FontWeight)(new FontWeightConverter()).ConvertFromString(currentBoxWeight);

            CurrentTradeFontFamily = new System.Windows.Media.FontFamily(currentTradeFont);
            CurrentTradeForeground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(currentTradeColor));
            CurrentTradeSize = float.Parse(currentTradeSize);
            CurrentTradeFontStyle = (FontStyle)(new FontStyleConverter()).ConvertFromString(currentTradeStyle);
            CurrentTradeFontWeight = (FontWeight)(new FontWeightConverter()).ConvertFromString(currentTradeWeight);

            TotalTradeFontFamily = new System.Windows.Media.FontFamily(totalTradeFont);
            TotalTradeForeground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(totalTradeColor));
            TotalTradeSize = float.Parse(totalTradeSize);
            TotalTradeFontStyle = (FontStyle)(new FontStyleConverter()).ConvertFromString(totalTradeStyle);
            TotalTradeFontWeight = (FontWeight)(new FontWeightConverter()).ConvertFromString(totalTradeWeight);

            AverageTextFontFamily = new System.Windows.Media.FontFamily(averagesFont);
            AverageTextForeground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(averagesColor));
            AverageTextSize = float.Parse(averagesSize);
            AverageTextFontStyle = (FontStyle)(new FontStyleConverter()).ConvertFromString(averagesStyle);
            AverageTextFontWeight = (FontWeight)(new FontWeightConverter()).ConvertFromString(averagesWeight);

            TotalHeight = int.Parse(totalHeight);
            TotalWidth = int.Parse(totalWidth);
            AutoDisplay = bool.Parse(autoDisplay);
            ShowCurrentBoxHits = bool.Parse(showCurrentBoxHits);
            ShowCurrentTradeHits = bool.Parse(showCurrentTradeHits);
            ShowTotalTradeCount = bool.Parse(showTotalTradeCount);
            ShowWorstTrades = bool.Parse(showWorstTrades);
            ShowBestTrades = bool.Parse(showBestTrades);
            TradesDisplayCount = int.Parse(tradesDisplayCount);
            SoundsEnabled = bool.Parse(soundsEnabled);
            if (double.TryParse(weaponPopupSeconds.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double secs))
                m_WeaponPopupSeconds = secs;
            ShowWeaponPopup = !bool.TryParse(showWeaponPopup, out bool swp) || swp;
        }

        private void SaveApplicationConfig()
        {
            string createdFilepath = Path.Combine(Directory.GetCurrentDirectory(), "ApplicationConfig.txt");
            var toWrite = new StringWriter();

            toWrite.WriteLine("BoxHitsFont=" + BoxHitsFontFamily.Source);
            toWrite.WriteLine("BoxHitsColor=" + (BoxHitsForeground as System.Windows.Media.SolidColorBrush).Color.ToString());
            toWrite.WriteLine("BoxHitsSize=" + BoxHitsSize);
            toWrite.WriteLine("BoxHitsStyle=" + BoxHitsFontStyle);
            toWrite.WriteLine("BoxHitsWeight=" + BoxHitsFontWeight);

            toWrite.WriteLine("CurrentBoxFont=" + CurrentBoxFontFamily.Source);
            toWrite.WriteLine("CurrentBoxColor=" + (CurrentBoxForeground as System.Windows.Media.SolidColorBrush).Color.ToString());
            toWrite.WriteLine("CurrentBoxSize=" + CurrentBoxSize);
            toWrite.WriteLine("CurrentBoxStyle=" + CurrentBoxFontStyle);
            toWrite.WriteLine("CurrentBoxWeight=" + CurrentBoxFontWeight);

            toWrite.WriteLine("CurrentTradeFont=" + CurrentTradeFontFamily.Source);
            toWrite.WriteLine("CurrentTradeColor=" + (CurrentTradeForeground as System.Windows.Media.SolidColorBrush).Color.ToString());
            toWrite.WriteLine("CurrentTradeSize=" + CurrentTradeSize);
            toWrite.WriteLine("CurrentTradeStyle=" + CurrentTradeFontStyle);
            toWrite.WriteLine("CurrentTradeWeight=" + CurrentTradeFontWeight);

            toWrite.WriteLine("TotalTradeFont=" + TotalTradeFontFamily.Source);
            toWrite.WriteLine("TotalTradeColor=" + (TotalTradeForeground as System.Windows.Media.SolidColorBrush).Color.ToString());
            toWrite.WriteLine("TotalTradeSize=" + TotalTradeSize);
            toWrite.WriteLine("TotalTradeStyle=" + TotalTradeFontStyle);
            toWrite.WriteLine("TotalTradeWeight=" + TotalTradeFontWeight);

            toWrite.WriteLine("AveragesFont=" + AverageTextFontFamily.Source);
            toWrite.WriteLine("AveragesColor=" + (AverageTextForeground as System.Windows.Media.SolidColorBrush).Color.ToString());
            toWrite.WriteLine("AveragesSize=" + AverageTextSize);
            toWrite.WriteLine("AveragesStyle=" + AverageTextFontStyle);
            toWrite.WriteLine("AveragesWeight=" + AverageTextFontWeight);

            toWrite.WriteLine("TotalHeight=" + TotalHeight);
            toWrite.WriteLine("TotalWidth=" + TotalWidth);
            toWrite.WriteLine("AutoDisplay=" + AutoDisplay);
            toWrite.WriteLine("ShowCurrentBoxHits=" + ShowCurrentBoxHits);
            toWrite.WriteLine("ShowCurrentTradeHits=" + ShowCurrentTradeHits);
            toWrite.WriteLine("ShowTotalTradeCount=" + ShowTotalTradeCount);
            toWrite.WriteLine("ShowWorstTrades=" + ShowWorstTrades);
            toWrite.WriteLine("TradesDisplayCount=" + TradesDisplayCount);
            toWrite.WriteLine("ShowBestTrades=" + ShowBestTrades);
            toWrite.WriteLine("SoundsEnabled=" + SoundsEnabled);
            toWrite.WriteLine("ShowWeaponPopup=" + ShowWeaponPopup);
            toWrite.WriteLine("WeaponPopupSeconds=" + m_WeaponPopupSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture));

            File.WriteAllText(createdFilepath, toWrite.ToString());
        }

        private void ShowCounting(object sender, RoutedEventArgs e)
        {
            if (countWeaponsWindow != null) return;

            var mapWeapons = GetWeaponsDictionary() ?? new Dictionary<int, string>();
            List<string> weapons = mapWeapons.Values.ToList();
            if (SelectedMap != "Nacht Der Untoten" && SelectedMap != "Moon" && SelectedMap != "MainMenu")
            {
                weapons.Add("Teddy bear");
            }

            countWeaponsWindow = new CountWeapons(weapons, m_WeaponsToNotCount, m_WeaponsToDisplay);
            countWeaponsWindow.ShowDialog();
            m_WeaponsToNotCount = countWeaponsWindow.WeaponsToNotCount.ToList();
            m_WeaponsToDisplay = countWeaponsWindow.WeaponsToDisplay.ToList();
            RefreshWeaponAverages();
            countWeaponsWindow = null;
        }

        private void ShowTrading(object sender, RoutedEventArgs e)
        {
            if (countWeaponsWindow != null) return;

            var mapWeapons = GetWeaponsDictionary() ?? new Dictionary<int, string>();
            List<string> weapons = mapWeapons.Values.ToList();

            tradeWeaponsWindow = new TradeWeapons(weapons, m_WeaponsBeingTraded);
            tradeWeaponsWindow.ShowDialog();
            m_WeaponsBeingTraded = tradeWeaponsWindow.WeaponsBeingTraded.ToList();
            tradeWeaponsWindow = null;
        }

        private Dictionary<int, string> GetWeaponsDictionary()
        {
            switch (SelectedMap)
            {
                case "Ascension": return BlackOpsLibrary.AscensionWeapons;
                case "Call of the Dead": return BlackOpsLibrary.CallOfTheDeadWeapons;
                case "Der Riese": return BlackOpsLibrary.DerRieseWeapons;
                case "Five": return BlackOpsLibrary.FiveWeapons;
                case "Kino der Toten": return BlackOpsLibrary.KinoDerTotenWeapons;
                case "Moon": return BlackOpsLibrary.MoonWeapons;
                case "Nacht Der Untoten": return BlackOpsLibrary.NachtDerUntotenWeapons;
                case "Shangri-la": return BlackOpsLibrary.ShangriLaWeapons;
                case "Shi No Numa": return BlackOpsLibrary.ShiNoNumaWeapons;
                case "Verruckt": return BlackOpsLibrary.VerrucktWeapons;
                default: return null;
            }
        }

        private bool PlayersHaveAllTradingWeapons()
        {
            if (m_WeaponsBeingTraded == null || m_WeaponsBeingTraded.Count == 0) return true;

            var dict = GetWeaponsDictionary();
            if (dict == null) return false;

            foreach (var weaponName in m_WeaponsBeingTraded)
            {
                var idsForWeapon = dict.Where(kv => string.Equals(kv.Value, weaponName, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Key).ToList();
                if (!idsForWeapon.Any())
                {
                    return false;
                }

                bool present = idsForWeapon.Any(id =>
                    m_WhiteInventory.Contains(id) ||
                    m_BlueInventory.Contains(id) ||
                    m_YellowInventory.Contains(id) ||
                    m_GreenInventory.Contains(id));

                if (!present) return false;
            }

            return true;
        }

        private HashSet<int> GetPapSetForSelectedMap()
        {
            switch (SelectedMap)
            {
                case "Ascension":
                    return new HashSet<int> { 8, 10, 17, 19, 30, 34, 40, 43, 45, 48, 50, 52, 54, 56, 58, 60, 62, 64, 68, 70, 72, 74, 76 };
                case "Call of the Dead":
                    return new HashSet<int> { 9, 11, 18, 20, 31, 36, 43, 46, 48, 51, 53, 55, 57, 59, 61, 63, 65, 67, 69, 71, 73, 75, 78, 80 };
                case "Der Riese":
                    return new HashSet<int> { 31, 32, 33, 34, 35, 37, 38, 40, 41, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 77, 79 };
                case "Five":
                    return new HashSet<int> { 9, 11, 18, 20, 31, 35, 41, 44, 46, 49, 51, 53, 55, 57, 59, 61, 63, 65, 68, 70, 73, 75, 77 };
                case "Kino der Toten":
                    return new HashSet<int> { 10, 12, 19, 21, 34, 38, 44, 47, 49, 52, 54, 56, 58, 60, 62, 64, 66, 68, 71, 73, 75, 77, 79 };
                case "Moon":
                    return new HashSet<int> { 10, 12, 19, 21, 34, 38, 44, 47, 49, 52, 54, 56, 58, 60, 62, 64, 66, 68, 70, 72, 75, 81 };
                case "Shangri-la":
                    return new HashSet<int> { 9, 11, 18, 20, 31, 35, 41, 44, 46, 49, 51, 53, 55, 57, 59, 61, 63, 65, 68, 70, 72, 74, 76 };
                default:
                    return null;
            }
        }

        private void UpdateWorstTradesString()
        {
            if (_worstTrades.Count == 0)
            {
                WorstTradesString = "";
                return;
            }
            var top = _worstTrades.OrderByDescending(x => x).Take(TradesDisplayCount).ToList();
            var parts = top.Select((v, i) => $"#{i + 1}: {v}");
            WorstTradesString = " Pires trades : " + string.Join(" | ", parts);
        }

        private void DisplayWorstTrades()
        {
            if (_worstTrades.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(
                    "Aucun trade enregistré pour cette game.",
                    "3 pires trades", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);
                return;
            }

            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < _worstTrades.Count; i++)
                sb.AppendLine($"#{i + 1} - {_worstTrades[i]} trade hits");

            System.Windows.Forms.MessageBox.Show(
                sb.ToString(),
                "3 pires trades", System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
        }

        private void RefreshWeaponAverages()
        {
            var weaponAverages = new ObservableCollection<KeyValuePair<string, float>>();

            foreach (var wtd in m_WeaponsToDisplay)
            {
                float divider = m_WeaponsGotten.ContainsKey(wtd) && m_WeaponsGotten[wtd].Count > 0 ? m_WeaponsGotten[wtd].Count : 1;
                float avg = m_WeaponsBoxHits.ContainsKey(wtd) ? ((float)m_WeaponsBoxHits[wtd]) / divider : 0f;
                weaponAverages.Add(new KeyValuePair<string, float>(wtd, avg));
            }

            WeaponAverages = weaponAverages;
        }
    }
}