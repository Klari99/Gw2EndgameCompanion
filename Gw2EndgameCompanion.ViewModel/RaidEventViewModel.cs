using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2EndgameCompanion.ViewModel
{
    public class RaidEventViewModel : BindingSource
    {
        #region Fields

        private string _wing; //wing id
        private string _id; //event name
        private string _type; //boss or encounter event
        private string _icon;
        private bool _done;

        #endregion

        #region Properties

        public string Wing
        {
            get => _wing;
            private set
            {
                if (value != _wing)
                {
                    _wing = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Id
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

        public bool Done
        {
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

        #endregion

        #region Constructor

        public RaidEventViewModel(Model.DTO.RaidEvent item)
        {
            string wing = "";

            string[] words = item.Wing.Split('_');
            foreach(string word in words)
            {
                string firsLetter = word.Substring(0, 1);
                string otherLetters = word.Substring(1, word.Length-1);

                wing += firsLetter.ToUpper() + otherLetters + " ";
            }

            wing = wing.Substring(0, wing.Length - 1);


            string id = "";

            words = item.Id.Split('_');
            foreach (string word in words)
            {
                string firsLetter = word.Substring(0, 1);
                string otherLetters = word.Substring(1, word.Length - 1);

                id += firsLetter.ToUpper() + otherLetters + " ";
            }

            id = id.Substring(0, id.Length - 1);

            Wing = wing;
            Id = id;
            Type = item.Type;
            Icon = item.Icon;
            Done = item.Done;
        }

        #endregion
    }
}
