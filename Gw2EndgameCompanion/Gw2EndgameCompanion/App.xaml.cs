using Gw2EndgameCompanion.Model;
using Gw2EndgameCompanion.Persistence;
using Gw2EndgameCompanion.ViewModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gw2EndgameCompanion
{
    public partial class App : Application
    {
        private IGw2EndgameCompanionPersistence _persistence;
        private Gw2EndgameCompanionModel _model;
        private Gw2EndgameCompanionViewModel _viewModel;

        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();

            MainPage = new MainPage();

            _persistence = DependencyService.Get<IGw2EndgameCompanionPersistence>();
            _model = new Gw2EndgameCompanionModel(_persistence);

            _viewModel = new Gw2EndgameCompanionViewModel(_model);
            this.BindingContext = _viewModel;
        }

        protected override void OnStart() => _viewModel.InitializeModelAsyncCommand.Execute(null);

        protected override void OnSleep() => _viewModel.SaveAccountCommand.Execute(null);

        protected override void OnResume() => _viewModel.LoadAccountCommand.Execute(null);
    }
}
