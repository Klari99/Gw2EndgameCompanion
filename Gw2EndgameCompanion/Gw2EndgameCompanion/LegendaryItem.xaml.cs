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
    public partial class LegendaryItem : ViewCell
    {
        #region Fields

        public static readonly BindableProperty LegendaryProperty =
            BindableProperty.Create("Legendary", typeof(ViewModel.LegendaryItemViewModel), typeof(LegendaryItem), null, BindingMode.OneWay);

        #endregion

        #region Properties

        public ViewModel.LegendaryItemViewModel Legendary
        {
            get => GetValue(LegendaryProperty) as ViewModel.LegendaryItemViewModel;
            set => SetValue(LegendaryProperty, value);
        }

        #endregion

        #region Constructor

        public LegendaryItem()
        {
            InitializeComponent();
        }
        #endregion
    }
}