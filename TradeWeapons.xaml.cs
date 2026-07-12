using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BoxAverageTracker
{
    /// <summary>
    /// Interaction logic for TradeWeapons.xaml
    /// </summary>
    public partial class TradeWeapons : Window
    {
        private ICommand selectAllTrading;
        private ICommand selectNoneTrading;

        public ICommand SelectAllToTrade
        {
            get
            {
                if (selectAllTrading == null)
                {
                    selectAllTrading = new CommandHandler(() => SelectAllToTradeMethod(), null);
                }

                return selectAllTrading;
            }

            set
            {
                selectAllTrading = value;
            }
        }

        public ICommand SelectNoneToTrade
        {
            get
            {
                if (selectNoneTrading == null)
                {
                    selectNoneTrading = new CommandHandler(() => SelectNoneToTradeMethod(), null);
                }

                return selectNoneTrading;
            }

            set
            {
                selectNoneTrading = value;
            }
        }

        public class WeaponToTrade
        {
            public WeaponToTrade()
            {
                IsBeingTraded = false;
                WeaponName = string.Empty;
            }

            public WeaponToTrade(bool beingTraded, string weaponName)
            {
                IsBeingTraded = beingTraded;
                WeaponName = weaponName;
            }

            public bool IsBeingTraded { get; set; }
            public string WeaponName { get; set; }
        }

        public static DependencyProperty WeaponsProperty = DependencyProperty.Register("Weapons", typeof(ObservableCollection<WeaponToTrade>), typeof(TradeWeapons), new PropertyMetadata(new ObservableCollection<WeaponToTrade>()));
        public static DependencyProperty WeaponsBeingTradedProperty = DependencyProperty.Register("WeaponsBeingTraded", typeof(ObservableCollection<string>), typeof(TradeWeapons), new PropertyMetadata(new ObservableCollection<string>()));

        public ObservableCollection<WeaponToTrade> Weapons
        {
            get => (ObservableCollection<WeaponToTrade>)GetValue(WeaponsProperty);
            set => SetValue(WeaponsProperty, value);
        }

        public ObservableCollection<string> WeaponsBeingTraded
        {
            get => (ObservableCollection<string>)GetValue(WeaponsBeingTradedProperty);
            set => SetValue(WeaponsBeingTradedProperty, value);
        }

        public TradeWeapons(List<string> weapons, List<string> weaponsTrading)
        {
            InitializeComponent();

            DataContext = this;

            var weaponsToTrade = new List<WeaponToTrade>();
            weapons = weapons.Distinct().ToList();
            foreach (var weapon in weapons)
            {
                weaponsToTrade.Add(new WeaponToTrade(weaponsTrading.Contains(weapon), weapon));
            }

            Weapons = new ObservableCollection<WeaponToTrade>(weaponsToTrade);
            WeaponsBeingTraded = new ObservableCollection<string>(weaponsTrading);
        }

        public void SelectAllToTradeMethod()
        {
            var weapons = new ObservableCollection<WeaponToTrade>(Weapons);
            var weaponsToDisplay = new ObservableCollection<string>();

            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].IsBeingTraded = true;
                weaponsToDisplay.Add(weapons[i].WeaponName);
            }

            Weapons = weapons;
            WeaponsBeingTraded = weaponsToDisplay;
        }

        public void SelectNoneToTradeMethod()
        {
            var weapons = new ObservableCollection<WeaponToTrade>(Weapons);

            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].IsBeingTraded = false;
            }

            Weapons = weapons;
            WeaponsBeingTraded = new ObservableCollection<string>();
        }

        private void IsTrading_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).DataContext is WeaponToTrade weaponToTrade && weaponToTrade.IsBeingTraded == false && !WeaponsBeingTraded.Any(x => x == weaponToTrade.WeaponName))
            {
                var weaponsToDisplay = new ObservableCollection<string>(WeaponsBeingTraded);
                var weapons = new ObservableCollection<WeaponToTrade>(Weapons);

                weapons.FirstOrDefault(x => x.WeaponName == weaponToTrade.WeaponName).IsBeingTraded = true;
                weaponsToDisplay.Add(weaponToTrade.WeaponName);

                Weapons = weapons;
                WeaponsBeingTraded = weaponsToDisplay;
            }
        }

        private void IsTrading_UnChecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).DataContext is WeaponToTrade weaponToTrade && weaponToTrade.IsBeingTraded == true && WeaponsBeingTraded.Any(x => x == weaponToTrade.WeaponName))
            {
                var weaponsToDisplay = new ObservableCollection<string>(WeaponsBeingTraded);
                var weapons = new ObservableCollection<WeaponToTrade>(Weapons);

                weapons.FirstOrDefault(x => x.WeaponName == weaponToTrade.WeaponName).IsBeingTraded = false;
                weaponsToDisplay.Remove(weaponToTrade.WeaponName);

                Weapons = weapons;
                WeaponsBeingTraded = weaponsToDisplay;
            }
        }
    }
}
