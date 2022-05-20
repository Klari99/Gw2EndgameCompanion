using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2EndgameCompanion.Model.DTO
{
    class DailyAchievementsSearch
    {
        public List<DailyAchievement> Pve { get; set; }
        public List<DailyAchievement> Pvp { get; set; }
        public List<DailyAchievement> Wvw { get; set; }
        public List<DailyAchievement> Fractals { get; set; }
        public List<DailyAchievement> Special { get; set; }
    }
}
