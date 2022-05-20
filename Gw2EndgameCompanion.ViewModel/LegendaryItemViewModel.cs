using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2EndgameCompanion.ViewModel
{
    public class LegendaryItemViewModel : BindingSource
    {
        #region Fields

        private int _id;
        private string _name;
        private string _icon;
        private string _type;
        private bool _owned;

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

        public string Type
        {
            get => _type;
            private set
            {
                if (value != _type)
                {
                    _type = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Owned
        {
            get => _owned;
            private set
            {
                if (value != _owned)
                {
                    _owned = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Constructor

        public LegendaryItemViewModel(Model.DTO.LegendaryItem item)
        {
            Id = item.Id;
            Name = item.Name;
            Type = item.Type;
            Icon = item.Icon;
            Owned = item.Owned;
        }

        #endregion
    }
}
