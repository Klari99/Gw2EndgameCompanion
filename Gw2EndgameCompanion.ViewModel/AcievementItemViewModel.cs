using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2EndgameCompanion.ViewModel
{
    public class AcievementItemViewModel : BindingSource
    {
        #region Fields

        private int _id;
        private string _name;
        private string _requirement;
        private string _icon;
        private bool _onWatchList;
        private bool _done;

        #endregion

        #region Properties

        public int Id
        {
            get => _id;
            private set
            {
                if (value != _id)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            private set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Requirement
        {
            get => _requirement;
            private set
            {
                if (value != _requirement)
                {
                    _requirement = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Icon
        {
            get => _icon;
            private set
            {
                if (value != _icon)
                {
                    _icon = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool OnWatchList {
            get => _onWatchList;
            set
            {
                if (value != _onWatchList)
                {
                    _onWatchList = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Done {
            get => _done;
            private set
            {
                if (value != _done)
                {
                    _done = value;
                    OnPropertyChanged();
                }
            }
        }

        public DelegateCommand WatchCommand { get; private set; }

        #endregion

        #region Constructor

        public AcievementItemViewModel(Model.DTO.AchievementItem item, bool onWatchList, DelegateCommand watchCommand)
        {
            Id = item.Id;
            Name = item.Name;
            Requirement = item.Requirement;
            Icon = item.Icon;
            OnWatchList = onWatchList;
            //Done = item.Done;

            WatchCommand = watchCommand;
        }

        public AcievementItemViewModel(Persistence.AchievementItemToWatch item, DelegateCommand watchCommand)
        {
            Id = item.Id;
            Name = item.Name;
            Requirement = item.Requirement;
            Icon = item.Icon;
            OnWatchList = true;
            //Done = item.Done;

            WatchCommand = watchCommand;
        }

        #endregion
    }
}
