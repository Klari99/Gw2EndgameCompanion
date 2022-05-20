using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2EndgameCompanion.Model.DTO
{
    public class Currencies
    {
        //https://api.guildwars2.com/v2/currencies?ids=all
        //https://api.guildwars2.com/v2/account/wallet?access_token=646B0AAC-BBEA-2447-89CF-BB7F8ADD8A116B6344F2-41A5-4F68-B38E-793F9CF59551

        //dailypage
        public Currency Coin { get; set; }
        public Currency Gem { get; set; }
        public Currency Karma { get; set; }
        public Currency Laurel { get; set; }

        //fractalpage
        public Currency FractalRelic { get; set; }
        public Currency PristineFractalRelic { get; set; }
        public Currency UnstableFractalEssence { get; set; }

        //https://api.guildwars2.com/v2/items?ids=79230
        //https://api.guildwars2.com/v2/account/materials?access_token=646B0AAC-BBEA-2447-89CF-BB7F8ADD8A116B6344F2-41A5-4F68-B38E-793F9CF59551
        public Material IntegratedFractalMatrix { get; set; }

        //strike and drm
        public Currency RedProphetCrystal { get; set; }
        public Currency GreenProphetCrystal { get; set; }
        public Currency BlueProphetCrystal { get; set; }
        public Currency TyrianDefenseSeal { get; set; }

        //raidpage

        //https://api.guildwars2.com/v2/items?ids=77302
        public Material LegendaryInsight { get; set; }

        //https://api.guildwars2.com/v2/items?ids=88485
        public Material LegendaryDivination { get; set; }
        public Currency MagnetiteShard { get; set; }
        public Currency GaetingCrystal { get; set; }

        //legendarypage

        //https://api.guildwars2.com/v2/items?ids=19675
        public Material MysticClover { get; set; }

        //https://api.guildwars2.com/v2/items?ids=19976
        public Material MysticCoin { get; set; }

        //https://api.guildwars2.com/v2/items?ids=19721
        public Material Ectoplasm { get; set; }
        public Currency ProvisionerToken { get; set; }
    }
}
