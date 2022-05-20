using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gw2EndgameCompanion
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RaidEvent : ViewCell
    {
        #region Fields

        public static readonly BindableProperty EventProperty =
            BindableProperty.Create("Event", typeof(ViewModel.RaidEventViewModel), typeof(RaidEvent), null, BindingMode.OneWay);

        #endregion

        #region Properties

        public ViewModel.RaidEventViewModel Event
        {
            get => GetValue(EventProperty) as ViewModel.RaidEventViewModel;
            set => SetValue(EventProperty, value);
        }

        #endregion

        #region Constructor
        public RaidEvent()
        {
            InitializeComponent();
        }
        #endregion
    }
}