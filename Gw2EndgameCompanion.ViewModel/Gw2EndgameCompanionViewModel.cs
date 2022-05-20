using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Gw2EndgameCompanion.ViewModel
{
    public class Gw2EndgameCompanionViewModel : BindingSource
    {
        #region Fields

        private Model.Gw2EndgameCompanionModel _model;
        private string _API_Key;
        private string _accountName;
        private bool _apiSubmitted;
        private Model.DTO.Currencies _currencies;
        private string _permissionsString;

        #endregion

        #region Properties

        public string API_Key
        {
            get => _API_Key;
            set
            {
                if (value != _API_Key)
                {
                    _API_Key = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AccountName
        {
            get => _accountName;
            private set
            {
                if (value != _accountName)
                {
                    _accountName = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool APISubmitted
        {
            get => _apiSubmitted;
            private set
            {
                if (value != _apiSubmitted)
                {
                    _apiSubmitted = value;
                    OnPropertyChanged();
                }
            }
        }

        public Model.DTO.Currencies Currencies
        {
            get => _currencies;
            private set
            {
                if (value != _currencies)
                {
                    _currencies = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PermissionsString
        {
            get => _permissionsString;
            private set
            {
                if (value != _permissionsString)
                {
                    _permissionsString = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<AcievementItemViewModel> WatchList { get; private set; } = new ObservableCollection<AcievementItemViewModel>();
        public ObservableCollection<AcievementItemViewModel> Dailies { get; private set; } = new ObservableCollection<AcievementItemViewModel>();
        public ObservableCollection<AcievementItemViewModel> DailyFractals { get; private set; } = new ObservableCollection<AcievementItemViewModel>();
        public ObservableCollection<AcievementItemViewModel> DailyStrike { get; private set; } = new ObservableCollection<AcievementItemViewModel>();
        public ObservableCollection<AcievementItemViewModel> DailyDRMs { get; private set; } = new ObservableCollection<AcievementItemViewModel>();
        public ObservableCollection<RaidEventViewModel> RaidEvents { get; private set; } = new ObservableCollection<RaidEventViewModel>();
        public ObservableCollection<LegendaryItemViewModel> LegendaryItems { get; private set; } = new ObservableCollection<LegendaryItemViewModel>();

        
        public DelegateCommand SubmitAPIKeyCommand { get; private set; }
        public DelegateCommand RemoveAPIKeyCommand { get; private set; }
        public DelegateCommand LoadAccountCommand { get; private set; }
        public DelegateCommand SaveAccountCommand { get; private set; }
        public DelegateCommand InitializeModelAsyncCommand { get; private set; }
        

        #endregion

        #region Constructors

        public Gw2EndgameCompanionViewModel(Model.Gw2EndgameCompanionModel model)
        {
            _model = model ?? throw new ArgumentNullException("model");
            APISubmitted = false;

            _model.AccountLoaded += _model_AccountLoaded;
            _model.APIVerified += _model_APIVerified;
            _model.NoAccountDataFound += _model_NoAccountDataFound;
            _model.WatchListChanged += _model_WatchListChanged;
            _model.DailyLoaded += _model_DailyLoaded;
            _model.FractalsLoaded += _model_FractalsLoaded;
            _model.StrikeLoaded += _model_StrikeLoaded;
            _model.DRMsLoaded += _model_DRMsLoaded;
            _model.RaidEventsLoaded += _model_RaidEventsLoaded;
            _model.LegendaryItemsLoaded += _model_LegendaryItemsLoaded;
            _model.CurrenciesLoaded += _model_CurrenciesLoaded;
            _model.PermissionsLoaded += _model_PermissionsLoaded;

            InitializeModelAsyncCommand = new DelegateCommand(Command_InitializeModelAsync);
            SubmitAPIKeyCommand = new DelegateCommand(Command_Submit_API);
            RemoveAPIKeyCommand = new DelegateCommand(Command_RemoveAPIKey);
            LoadAccountCommand = new DelegateCommand(Command_LoadAccountAsync);
            SaveAccountCommand = new DelegateCommand(Command_SaveAccountAsync);
        }

        #endregion

        #region Event Handlers

        private void _model_PermissionsLoaded(object sender, EventArgs e)
        {
            _permissionsString = "";
            foreach (string permission in _model.Permissions)
                _permissionsString += permission + ", ";
            PermissionsString = PermissionsString.Substring(0, _permissionsString.Length - 2)+ ".";
        }

        private void _model_CurrenciesLoaded(object sender, EventArgs e)
        {
            Currencies = _model.Currencies;
        }

        private void _model_NoAccountDataFound(object sender, EventArgs e)
        {
            APISubmitted = false;
            API_Key = null;
            AccountName = null;
            Currencies = null;
            PermissionsString = "";
            Command_LoadRaidEvents(null);
            Command_LoadLegendaryItems(null);
        }

        private void _model_LegendaryItemsLoaded(object sender, EventArgs e)
        {
            LegendaryItems.Clear();
            foreach (Model.DTO.LegendaryItem loadedItem in _model.LegendaryItems)
                LegendaryItems.Add(new LegendaryItemViewModel(loadedItem));
        }

        private void _model_AccountLoaded(object sender, EventArgs e)
        {
            API_Key = _model.API_Key;
            AccountName = _model.AccountName;
            if(API_Key != null && API_Key != "")
                APISubmitted = true;
            else
                APISubmitted = false;

            Command_LoadRaidEvents(null);
            Command_LoadLegendaryItems(null);
            Command_LoadCurrencies(null);
            Command_LoadPermissions(null);

        }

        private void _model_DailyLoaded(object sender, EventArgs e)
        {
            foreach (Model.DTO.AchievementItem loadedItem in _model.DailyAchievements)
                Dailies.Add(new AcievementItemViewModel(loadedItem, _model.WatchList.Any(item => item.Id == loadedItem.Id), new DelegateCommand(Command_ChangeAchievementInWatchList)));
        }

        private void _model_DRMsLoaded(object sender, EventArgs e)
        {
            foreach (Model.DTO.AchievementItem loadedItem in _model.DailyDRMAchievements)
                DailyDRMs.Add(new AcievementItemViewModel(loadedItem, _model.WatchList.Any(item => item.Id == loadedItem.Id), new DelegateCommand(Command_ChangeAchievementInWatchList)));
        }

        private void _model_StrikeLoaded(object sender, EventArgs e)
        {
            foreach (Model.DTO.AchievementItem loadedItem in _model.DailyStrikeAchievements)
                DailyStrike.Add(new AcievementItemViewModel(loadedItem, _model.WatchList.Any(item => item.Id == loadedItem.Id), new DelegateCommand(Command_ChangeAchievementInWatchList)));
        }

        private void _model_RaidEventsLoaded(object sender, EventArgs e)
        {
            RaidEvents.Clear();
            foreach (Model.DTO.RaidEvent loadedItem in _model.RaidEvents)
            {
                RaidEvents.Add(new RaidEventViewModel(loadedItem));
            }
        }

        private void _model_WatchListChanged(object sender, EventArgs e)
        {
            WatchList.Clear();
            foreach (Persistence.AchievementItemToWatch achievement in _model.WatchList)
                WatchList.Add(new AcievementItemViewModel(achievement, new DelegateCommand(Command_ChangeAchievementInWatchList)));

            foreach (AcievementItemViewModel achievement in Dailies)
                achievement.OnWatchList = _model.WatchList.Any(item => item.Id == achievement.Id);
            foreach (AcievementItemViewModel achievement in DailyFractals)
                achievement.OnWatchList = _model.WatchList.Any(item => item.Id == achievement.Id);
            foreach (AcievementItemViewModel achievement in DailyStrike)
                achievement.OnWatchList = _model.WatchList.Any(item => item.Id == achievement.Id);
            foreach (AcievementItemViewModel achievement in DailyDRMs)
                achievement.OnWatchList = _model.WatchList.Any(item => item.Id == achievement.Id);
        }

        private void _model_FractalsLoaded(object sender, EventArgs e)
        {
            foreach (Model.DTO.AchievementItem loadedDailyFractal in _model.DailyFractalAchievements)
                DailyFractals.Add(new AcievementItemViewModel(loadedDailyFractal, _model.WatchList.Any(item => item.Id == loadedDailyFractal.Id), new DelegateCommand(Command_ChangeAchievementInWatchList)));
        }

        private void _model_APIVerified(object sender, EventArgs e)
        {
            API_Key = _model.API_Key;
            AccountName = _model.AccountName;

            if (API_Key != null && API_Key != "")
                APISubmitted = true;
            else
                APISubmitted = false;

            Command_LoadRaidEvents(null);
            Command_LoadLegendaryItems(null);
            Command_LoadCurrencies(null);
            Command_LoadPermissions(null);
        }

        #endregion

        #region Command Methods

        private void Command_InitializeModelAsync(object param) => Task.Run(async () => await _model.InitializeAsync());
        private void Command_Submit_API(object param) => _model.SubmitAPI(API_Key);
        private void Command_RemoveAPIKey(object param) => _model.RemoveAPI();
        private void Command_LoadAccountAsync(object param) => Task.Run(async () => await _model.LoadAccountAsync());
        private void Command_SaveAccountAsync(object param) => Task.Run(async () => await _model.SaveAccountAsync());
        private async void Command_ChangeAchievementInWatchList(object param)
        {
            if (param != null && param is AcievementItemViewModel achievement)
            {
                if (achievement.OnWatchList)
                    await _model.RemoveAchievementFromWatchLisAsync(achievement.Id);
                else await _model.AddAchievementToWatchListAsync(achievement.Id, achievement.Name, achievement.Requirement, achievement.Icon);
            }
        }

        #endregion

        #region Private Methods

        private void Command_LoadRaidEvents(object param) => Task.Run(async () => await _model.LoadRaidEventsAsync());
        private void Command_LoadLegendaryItems(object param) => Task.Run(async () => await _model.LoadLegendaryItemsAsync());
        private void Command_LoadCurrencies(object param) => Task.Run(async () => await _model.LoadCurrenciesAsync());
        private void Command_LoadPermissions(object param) => Task.Run(async () => await _model.LoadPermissionsAsync());

        #endregion
    }
}
