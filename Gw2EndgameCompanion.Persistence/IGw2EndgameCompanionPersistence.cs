using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gw2EndgameCompanion.Persistence
{
    public interface IGw2EndgameCompanionPersistence
    {
        Task SaveAccountInfosAsync(AccountInfos account);
        Task<AccountInfos> LoadAccountInfosAsync();
        Task AddToWatchListAsync(AchievementItemToWatch item);
        Task RemoveFromWatchListAsync(int id);
        Task<IEnumerable<AchievementItemToWatch>> GetWatchListAsync();
    }
}
