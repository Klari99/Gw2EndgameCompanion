using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Gw2EndgameCompanion.Persistence;
using Newtonsoft.Json;

namespace Gw2EndgameCompanion.Persistence.TextJSON
{
    public class Gw2EndgameCompanionJSONFilePersistence : IGw2EndgameCompanionPersistence
    {
        #region Properties

        private string _WatchListFilePath { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "watchlist.json"); }

        #endregion

        #region Private Methods

        private async Task SaveFavouritesAsync(List<AchievementItemToWatch> items)
        {
            try
            {
                string json = JsonConvert.SerializeObject(items);
                await Task.Run(() => File.WriteAllText(_WatchListFilePath, json));
            }
            catch { }
        }
        private async Task<List<AchievementItemToWatch>> LoadFavouritesAsync()
        {
            try
            {
                string filePath = _WatchListFilePath;
                List<AchievementItemToWatch> favourites = null;
                await Task.Run(() =>
                {
                    if (File.Exists(filePath))
                    {
                        string json = File.ReadAllText(filePath);
                        favourites = JsonConvert.DeserializeObject<List<AchievementItemToWatch>>(json);
                    }
                });
                return favourites ?? new List<AchievementItemToWatch>();
            }
            catch
            {
                return new List<AchievementItemToWatch>();
            }

        }

        #endregion

        #region Public Methods
        public async Task SaveAccountInfosAsync(AccountInfos account)
        {
            try
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gw2.json");
                string json = JsonConvert.SerializeObject(account);
                await Task.Run(() => File.WriteAllText(fileName, json));
            }
            catch { }
        }
        public async Task<AccountInfos> LoadAccountInfosAsync()
        {
            try
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gw2.json");
                AccountInfos account = null;
                await Task.Run(() =>
                {
                    if (File.Exists(fileName))
                    {
                        string json = File.ReadAllText(fileName);
                        account = JsonConvert.DeserializeObject<AccountInfos>(json);
                    }
                });
                return account;
            }
            catch
            {
                return null;
            }
        }
        public async Task AddToWatchListAsync(AchievementItemToWatch item)
        {
            List<AchievementItemToWatch> watchList = await LoadFavouritesAsync();
            watchList.Add(item);
            await SaveFavouritesAsync(watchList);
        }
        public async Task RemoveFromWatchListAsync(int id)
        {
            List<AchievementItemToWatch> watchList = await LoadFavouritesAsync();
            watchList.RemoveAll(item => item.Id == id);
            await SaveFavouritesAsync(watchList);
        }
        public async Task<IEnumerable<AchievementItemToWatch>> GetWatchListAsync() => await LoadFavouritesAsync();

        #endregion
    }
}
