using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Gw2EndgameCompanion
{
    public partial class MainPage : Shell
    {
        public MainPage()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(DailyPage), typeof(DailyPage));
            Routing.RegisterRoute(nameof(FractalPage), typeof(FractalPage));
            Routing.RegisterRoute(nameof(StrikePage), typeof(StrikePage));
            Routing.RegisterRoute(nameof(RaidPage), typeof(RaidPage));
            Routing.RegisterRoute(nameof(LegendaryPage), typeof(LegendaryPage));
        }
    }
}
