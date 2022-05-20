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
    public partial class AchievementItem : ViewCell
    {
        #region Fields

        public static readonly BindableProperty AchievementProperty =
            BindableProperty.Create("Achievement", typeof(ViewModel.AcievementItemViewModel), typeof(AchievementItem), null, BindingMode.OneWay);

        #endregion

        #region Properties

        public ViewModel.AcievementItemViewModel Achievement
        {
            get => GetValue(AchievementProperty) as ViewModel.AcievementItemViewModel;
            set => SetValue(AchievementProperty, value);
        }

        #endregion

        #region Constructors

        public AchievementItem()
        {
            InitializeComponent();
        }

        #endregion
    }
}
