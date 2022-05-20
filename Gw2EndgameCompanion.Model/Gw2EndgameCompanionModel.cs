using Newtonsoft.Json;
using Gw2EndgameCompanion.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gw2EndgameCompanion.Model
{
    public class Gw2EndgameCompanionModel
    {

        #region Fields

        private DTO.AccountInfos _account;
        private IGw2EndgameCompanionPersistence _persistence;
        private IEnumerable<DTO.DailyAchievement> _dailyIDs;
        private IEnumerable<DTO.DailyAchievement> _dailyFractalIDs;
        private IEnumerable<int> _dailyStrikeID;
        private IEnumerable<int> _dailyDRMIDs;
        private IEnumerable<DTO.RaidWing> _raidWings;
        private IEnumerable<DTO.RaidEvent> _raidEvents = new List<DTO.RaidEvent>();
        private List<string> _raidProgressions;
        private IEnumerable<DTO.LegendaryItem> _ownedLegendaries;
        private List<AchievementItemToWatch> _watchList = new List<AchievementItemToWatch>();
        private DTO.Currencies _currencies;

        #endregion

        #region Properties

        public string API_Key { get; private set; }
        public string AccountName { get; private set; }

        public IEnumerable<AchievementItemToWatch> WatchList { get => _watchList; }
        public IEnumerable<DTO.AchievementItem> DailyAchievements { get; private set; }
        public IEnumerable<DTO.AchievementItem> DailyFractalAchievements { get; private set; }
        public IEnumerable<DTO.AchievementItem> DailyStrikeAchievements { get; private set; }
        public IEnumerable<DTO.AchievementItem> DailyDRMAchievements { get; private set; }
        public IEnumerable<DTO.RaidEvent> RaidEvents { get; private set; }
        public IEnumerable<DTO.LegendaryItem> LegendaryItems { get; private set; }
        public DTO.Currencies Currencies { get => _currencies; }
        public List<string> Permissions { get; private set; }

        #endregion

        #region Events

        public event EventHandler APIVerified;
        public event EventHandler AccountLoaded;
        public event EventHandler NoAccountDataFound;
        public event EventHandler WatchListChanged;
        public event EventHandler DailyLoaded;
        public event EventHandler FractalsLoaded;
        public event EventHandler StrikeLoaded;
        public event EventHandler DRMsLoaded;
        public event EventHandler RaidEventsLoaded;
        public event EventHandler LegendaryItemsLoaded;
        public event EventHandler CurrenciesLoaded;
        public event EventHandler PermissionsLoaded;

        #endregion

        #region Constructors

        public Gw2EndgameCompanionModel(IGw2EndgameCompanionPersistence persistence)
        {
            _persistence = persistence;
        }

        #endregion

        #region Private Methods
        private void OnAPIVerified()
        => APIVerified?.Invoke(this, EventArgs.Empty);

        private void OnAccountNotFound()
        => NoAccountDataFound?.Invoke(this, EventArgs.Empty);

        private void OnAccountLoaded()
        => AccountLoaded?.Invoke(this, EventArgs.Empty);

        private void OnWatchListChanged()
        => WatchListChanged?.Invoke(this, EventArgs.Empty);

        private void OnDailyLoaded()
            => DailyLoaded?.Invoke(this, EventArgs.Empty);

        private void OnFractalsLoaded()
            => FractalsLoaded?.Invoke(this, EventArgs.Empty);

        private void OnStrikeLoaded()
            => StrikeLoaded?.Invoke(this, EventArgs.Empty);

        private void OnDRMsLoaded()
            => DRMsLoaded?.Invoke(this, EventArgs.Empty);

        private void OnRaidEventsLoaded()
            => RaidEventsLoaded?.Invoke(this, EventArgs.Empty);

        private void OnLegendaryItemsLoaded()
            => LegendaryItemsLoaded?.Invoke(this, EventArgs.Empty);

        private void OnCurrenciesLoaded()
            => CurrenciesLoaded?.Invoke(this, EventArgs.Empty);
        private void OnPermissionsLoaded()
            => PermissionsLoaded?.Invoke(this, EventArgs.Empty);

        private async Task GetDataAsync(string API_Key)
        {
            Uri uri = new Uri("https://api.guildwars2.com/v2/account?access_token=" + API_Key);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    DTO.AccountInfos apiResponse = JsonConvert.DeserializeObject<DTO.AccountInfos>(await response.Content.ReadAsStringAsync());
                    _account = apiResponse;
                }
            }
        }

        private async Task GetDailyListAsync()
        {
            string icon = "";

            using (HttpClient client = new HttpClient())
            {
                //get pve daily icon
                Uri uri0 = new Uri("https://api.guildwars2.com/v2/achievements/categories/97");
                HttpResponseMessage response = await client.GetAsync(uri0);
                if (response.IsSuccessStatusCode)
                {
                    DTO.DailyAchievement apiResponse = JsonConvert.DeserializeObject<DTO.DailyAchievement>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null)
                        icon = apiResponse.icon;
                }

                //get daily ids
                Uri uri = new Uri("https://api.guildwars2.com/v2/achievements/daily");

                response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    DTO.DailyAchievementsSearch apiResponse = JsonConvert.DeserializeObject<DTO.DailyAchievementsSearch>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null && apiResponse.Pve != null && apiResponse.Pvp != null && apiResponse.Wvw != null)
                    {
                        foreach (Model.DTO.DailyAchievement achievement in apiResponse.Pve)
                            achievement.icon = icon;
                        _dailyIDs = apiResponse.Pve.Concat(apiResponse.Pvp).Concat(apiResponse.Wvw);
                    }

                }

                //get achievements informations
                string ids = "";
                foreach (DTO.DailyAchievement achievement in _dailyIDs) ids += achievement.id + ",";

                Uri uri2 = new Uri("https://api.guildwars2.com/v2/achievements?ids=" + ids);
                response = await client.GetAsync(uri2);
                if (response.IsSuccessStatusCode)
                {
                    List<DTO.AchievementItem> apiResponse = JsonConvert.DeserializeObject<List<DTO.AchievementItem>>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null)
                    {
                        DailyAchievements = apiResponse;
                        foreach (Model.DTO.AchievementItem achievement in DailyAchievements)
                        {
                            if (achievement.Icon == null)
                                achievement.Icon = _dailyIDs.Where(item => item.id == achievement.Id).FirstOrDefault().icon;
                        }
                    }
                }
            }
        }

        private async Task GetDailyFractalListAsync()
        {
            Uri uri = new Uri("https://api.guildwars2.com/v2/achievements/daily");

            using (HttpClient client = new HttpClient())
            {
                //get fractal ids
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    DTO.DailyAchievementsSearch apiResponse = JsonConvert.DeserializeObject<DTO.DailyAchievementsSearch>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null && apiResponse.Fractals != null)
                        _dailyFractalIDs = apiResponse.Fractals;
                }

                //get achievements informations
                string ids = "";
                foreach (DTO.DailyAchievement fractal in _dailyFractalIDs) ids += fractal.id + ",";

                Uri uri2 = new Uri("https://api.guildwars2.com/v2/achievements?ids=" + ids);
                response = await client.GetAsync(uri2);
                if (response.IsSuccessStatusCode)
                {
                    List<DTO.AchievementItem> apiResponse = JsonConvert.DeserializeObject<List<DTO.AchievementItem>>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null)
                        DailyFractalAchievements = apiResponse;
                }
            }
        }

        private async Task GetDailyStrikeListAsync()
        {
            Uri uri = new Uri("https://api.guildwars2.com/v2/achievements/categories/250");

            using (HttpClient client = new HttpClient())
            {
                string icon = "";

                //get strike id
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    DTO.AchievementCategoriesSearch apiResponse = JsonConvert.DeserializeObject<DTO.AchievementCategoriesSearch>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null && apiResponse.Achievements != null)
                    {
                        _dailyStrikeID = apiResponse.Achievements;
                        icon = apiResponse.Icon;
                    }
                }

                //get achievements informations
                string ids = "";
                foreach (int item in _dailyStrikeID) ids += item + ",";

                Uri uri2 = new Uri("https://api.guildwars2.com/v2/achievements?ids=" + ids);
                response = await client.GetAsync(uri2);
                if (response.IsSuccessStatusCode)
                {
                    List<DTO.AchievementItem> apiResponse = JsonConvert.DeserializeObject<List<DTO.AchievementItem>>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null)
                    {
                        //set icon to each achievement element
                        foreach (DTO.AchievementItem item in apiResponse)
                        {
                            item.Icon = icon;
                            DailyStrikeAchievements = apiResponse;
                        }
                    }
                }
            }
        }

        private async Task GetDailyDRMListAsync()
        {
            Uri uri = new Uri("https://api.guildwars2.com/v2/achievements/categories/264");

            using (HttpClient client = new HttpClient())
            {
                string icon = "";

                //get drm ids
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    DTO.AchievementCategoriesSearch apiResponse = JsonConvert.DeserializeObject<DTO.AchievementCategoriesSearch>(await response.Content.ReadAsStringAsync());

                    if (apiResponse != null && apiResponse.Achievements != null)
                    {
                        _dailyDRMIDs = apiResponse.Achievements;
                        icon = apiResponse.Icon;
                    }
                }

                //get achievements informations
                string ids = "";
                foreach (int item in _dailyDRMIDs) ids += item + ",";

                Uri uri2 = new Uri("https://api.guildwars2.com/v2/achievements?ids=" + ids);
                response = await client.GetAsync(uri2);
                if (response.IsSuccessStatusCode)
                {
                    List<DTO.AchievementItem> apiResponse = JsonConvert.DeserializeObject<List<DTO.AchievementItem>>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null)
                    {
                        //set icon to each achievement element
                        foreach (DTO.AchievementItem item in apiResponse) {
                            item.Icon = icon;
                            DailyDRMAchievements = apiResponse;
                        }
                    }
                }
            }
        }

        private async Task GetRaidEventListAsync()
        {
            _raidEvents = Enumerable.Empty<Model.DTO.RaidEvent>();
            using (HttpClient client = new HttpClient())
            {
                //get raid progression of the week
                if (API_Key != null && API_Key != "")
                {
                    Uri uri6 = new Uri("https://api.guildwars2.com/v2/account/raids?access_token=" + API_Key);
                    HttpResponseMessage r = await client.GetAsync(uri6);
                    if (r.IsSuccessStatusCode)
                    {
                        List<string> apiResponse = JsonConvert.DeserializeObject<List<string>>(await r.Content.ReadAsStringAsync());
                        if (apiResponse != null)
                        {
                            _raidProgressions = apiResponse;
                        }
                    }
                }

                //forsaken_thicket
                Uri uri = new Uri("https://api.guildwars2.com/v2/raids/forsaken_thicket");
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    Model.DTO.RaidRegion apiResponse = JsonConvert.DeserializeObject<Model.DTO.RaidRegion>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null)
                    {
                        foreach (Model.DTO.RaidWing wing in apiResponse.Wings)
                        {
                            foreach(Model.DTO.RaidEvent raidEvent in wing.Events)
                            {
                                raidEvent.Wing = wing.Id;
                                if(_raidProgressions != null && _raidProgressions.Count() > 0)
                                    raidEvent.Done = _raidProgressions.Contains(raidEvent.Id);
                                else
                                    raidEvent.Done = false;
                            }
                        }
                        _raidWings = apiResponse.Wings;
                    }
                }

                //bastion_of_the_penitent
                Uri uri2 = new Uri("https://api.guildwars2.com/v2/raids/bastion_of_the_penitent");
                response = await client.GetAsync(uri2);
                if (response.IsSuccessStatusCode)
                {
                    Model.DTO.RaidRegion apiResponse = JsonConvert.DeserializeObject<Model.DTO.RaidRegion>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null)
                    {
                        foreach (Model.DTO.RaidWing wing in apiResponse.Wings)
                        {
                            foreach (Model.DTO.RaidEvent raidEvent in wing.Events)
                            {
                                raidEvent.Wing = wing.Id;
                                if (_raidProgressions != null && _raidProgressions.Count() > 0)
                                    raidEvent.Done = _raidProgressions.Contains(raidEvent.Id);
                                else
                                    raidEvent.Done = false;
                            }
                        }
                        _raidWings = _raidWings.Concat(apiResponse.Wings);
                    }
                }

                //hall_of_chains
                Uri uri3 = new Uri("https://api.guildwars2.com/v2/raids/hall_of_chains");
                response = await client.GetAsync(uri3);
                if (response.IsSuccessStatusCode)
                {
                    Model.DTO.RaidRegion apiResponse = JsonConvert.DeserializeObject<Model.DTO.RaidRegion>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null)
                    {
                        foreach (Model.DTO.RaidWing wing in apiResponse.Wings)
                        {
                            foreach (Model.DTO.RaidEvent raidEvent in wing.Events)
                            {
                                raidEvent.Wing = wing.Id;
                                if (_raidProgressions != null && _raidProgressions.Count() > 0)
                                    raidEvent.Done = _raidProgressions.Contains(raidEvent.Id);
                                else
                                    raidEvent.Done = false;
                            }
                        }
                        _raidWings = _raidWings.Concat(apiResponse.Wings);
                    }
                }

                //mythwright_gambit
                Uri uri4 = new Uri("https://api.guildwars2.com/v2/raids/mythwright_gambit");
                response = await client.GetAsync(uri4);
                if (response.IsSuccessStatusCode)
                {
                    Model.DTO.RaidRegion apiResponse = JsonConvert.DeserializeObject<Model.DTO.RaidRegion>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null)
                    {
                        foreach (Model.DTO.RaidWing wing in apiResponse.Wings)
                        {
                            foreach (Model.DTO.RaidEvent raidEvent in wing.Events)
                            {
                                raidEvent.Wing = wing.Id;
                                if (_raidProgressions != null && _raidProgressions.Count() > 0)
                                    raidEvent.Done = _raidProgressions.Contains(raidEvent.Id);
                                else
                                    raidEvent.Done = false;
                            }
                        }
                        _raidWings = _raidWings.Concat(apiResponse.Wings);
                    }
                }

                //the_key_of_ahdashim
                Uri uri5 = new Uri("https://api.guildwars2.com/v2/raids/the_key_of_ahdashim");
                response = await client.GetAsync(uri5);
                if (response.IsSuccessStatusCode)
                {
                    Model.DTO.RaidRegion apiResponse = JsonConvert.DeserializeObject<Model.DTO.RaidRegion>(await response.Content.ReadAsStringAsync());
                    if (apiResponse != null)
                    {
                        foreach (Model.DTO.RaidWing wing in apiResponse.Wings)
                        {
                            foreach (Model.DTO.RaidEvent raidEvent in wing.Events)
                            {
                                raidEvent.Wing = wing.Id;
                                if (_raidProgressions != null && _raidProgressions.Count() > 0)
                                    raidEvent.Done = _raidProgressions.Contains(raidEvent.Id);
                                else
                                    raidEvent.Done = false;
                            }
                        }
                        _raidWings = _raidWings.Concat(apiResponse.Wings);
                    }
                }
            }

            //get raid events
            foreach (DTO.RaidWing wing in _raidWings)
                _raidEvents = _raidEvents.Concat(wing.Events);

            foreach(DTO.RaidEvent raidEvent in _raidEvents)
            {
                switch (raidEvent.Id)
                {
                    case "vale_guardian":
                        raidEvent.Icon = "https://render.guildwars2.com/file/1CCB9B3880B062465F9F5834F04739289AF27BBA/1301792.png";
                        break;

                    case "spirit_woods":
                        raidEvent.Icon = "https://render.guildwars2.com/file/1CA7D86C2AFC5975410AEE0B01CADF45A255004E/1301728.png";
                        break;

                    case "gorseval":
                        raidEvent.Icon = "https://render.guildwars2.com/file/E5074A6E5CE552A9231195202B45D5DA2C1A585E/1301787.png";
                        break;

                    case "sabetha":
                        raidEvent.Icon = "https://render.guildwars2.com/file/1C96C03C385529A818E9E14ED7BB7E47F10154F5/1301795.png";
                        break;

                    case "slothasor":
                        raidEvent.Icon = "https://render.guildwars2.com/file/2370FE955AC49D38B99F2592F629D2B544BABBD2/1377392.png";
                        break;

                    case "bandit_trio":
                        raidEvent.Icon = "https://render.guildwars2.com/file/A95E0E20EB6FDE9CBCEE667FF13BEAC2E80D594D/1377389.png";
                        break;

                    case "matthias":
                        raidEvent.Icon = "https://render.guildwars2.com/file/EBAFC4EA9C80F6D66AD9A427E15918E0172A476F/1377391.png";
                        break;

                    case "escort":
                        raidEvent.Icon = "https://render.guildwars2.com/file/433A4E66794240AD463941F868754BB46D245020/1451172.png";
                        break;

                    case "keep_construct":
                        raidEvent.Icon = "https://render.guildwars2.com/file/E87C6B0BB4C1D93231A4DB7CBE91157516580A3E/1451173.png";
                        break;

                    case "twisted_castle":
                        raidEvent.Icon = "https://render.guildwars2.com/file/62C547BC593942603192793F44031F22F531624A/1451294.png";
                        break;

                    case "xera":
                        raidEvent.Icon = "https://render.guildwars2.com/file/551509B3B91270BFF016C020D2F9D99B550F93AA/1451174.png";
                        break;

                    case "cairn":
                        raidEvent.Icon = "https://render.guildwars2.com/file/0863ADC609086A5E5F6C031DDC1766033432250C/1633961.png";
                        break;

                    case "mursaat_overseer":
                        raidEvent.Icon = "https://render.guildwars2.com/file/05FCD9D1B4959A051EEC3B160F0802FAB40C2619/1633963.png";
                        break;

                    case "samarog":
                        raidEvent.Icon = "https://render.guildwars2.com/file/9645BE1925D1AC3E7177C99B740078F09EA5A739/1633967.png";
                        break;

                    case "deimos":
                        raidEvent.Icon = "https://render.guildwars2.com/file/03E57D1019181CE015C09BDA0601B7D34ECD3841/1633966.png";
                        break;

                    case "soulless_horror":
                        raidEvent.Icon = "https://render.guildwars2.com/file/4FE00D4F52D0D8340CCCBAE824B7FD0109BE7E33/1894936.png";
                        break;

                    case "river_of_souls":
                        raidEvent.Icon = "https://render.guildwars2.com/file/5276C15002466D2B7D48094396D3117762E12E6C/1894803.png";
                        break;

                    case "statues_of_grenth":
                        raidEvent.Icon = "https://render.guildwars2.com/file/712E5629410C91E6C0F95602F6621FDD000E7A34/1894935.png";
                        break;

                    case "voice_in_the_void":
                        raidEvent.Icon = "https://render.guildwars2.com/file/5C4FEBC890B4D9D2D6A96E4837CD4C0CFA605AB7/1894937.png";
                        break;

                    case "conjured_amalgamate":
                        raidEvent.Icon = "https://render.guildwars2.com/file/B316A9FAA3275D0EF6D84A9179E062BF10C4545A/2038619.png";
                        break;

                    case "twin_largos":
                        raidEvent.Icon = "https://render.guildwars2.com/file/453C959040B6AF7F639FDD78367AF39FD7C73246/2038614.png";
                        break;

                    case "qadim":
                        raidEvent.Icon = "https://render.guildwars2.com/file/64ED38CE4F549E573DA3CE641A15BCB3C4FB48B3/2038618.png";
                        break;

                    case "gate":
                        raidEvent.Icon = "https://render.guildwars2.com/file/A9AEBDAAA1D94F9DA41A4714C7464758D9EE3ED7/2155913.png";
                        break;

                    case "adina":
                        raidEvent.Icon = "https://render.guildwars2.com/file/EAF501CC3C05CAD8C1BB474133B9F89925400EF3/1766806.png";
                        break;

                    case "sabir":
                        raidEvent.Icon = "https://render.guildwars2.com/file/9B52A257E0ED4B3F1992ADE60D6A071ED4D5BBAB/1766790.png";
                        break;

                    case "qadim_the_peerless":
                        raidEvent.Icon = "https://render.guildwars2.com/file/640241E807BF6D14447406C934F5D6FE144A0A4E/2155914.png";
                        break;

                    default:
                        raidEvent.Icon = "https://render.guildwars2.com/file/9F5C23543CB8C715B7022635C10AA6D5011E74B3/1302679.png";
                        break;
                }
            }
        
            RaidEvents = _raidEvents;
        }

        private async Task GetLegendaryItemListAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                //get own legendary items' ids
                if (API_Key != null && API_Key != "")
                {
                    Uri uri0 = new Uri("https://api.guildwars2.com/v2/account/legendaryarmory?access_token=" + API_Key);
                    HttpResponseMessage r = await client.GetAsync(uri0);
                    if (r.IsSuccessStatusCode)
                    {
                        List<DTO.LegendaryItem> apiResponse = JsonConvert.DeserializeObject<List<DTO.LegendaryItem>>(await r.Content.ReadAsStringAsync());
                        if (apiResponse != null)
                        {
                            _ownedLegendaries = apiResponse;
                        }
                    }
                }

                //get all legendary item
                Uri uri = new Uri("https://api.guildwars2.com/v2/legendaryarmory");
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string ids = "";
                    
                    List<int> apiResponse = JsonConvert.DeserializeObject<List<int>>(await response.Content.ReadAsStringAsync());


                    if (apiResponse != null)
                    {
                        foreach (int i in apiResponse) ids += i.ToString() + ",";

                        Uri uri2 = new Uri("https://api.guildwars2.com/v2/items?ids=" + ids);
                        response = await client.GetAsync(uri2);
                        if (response.IsSuccessStatusCode)
                        {
                            List<DTO.LegendaryItem> apiResponse2 = JsonConvert.DeserializeObject<List<DTO.LegendaryItem>>(await response.Content.ReadAsStringAsync());
                            if (apiResponse2 != null)
                            {
                                foreach(Model.DTO.LegendaryItem item in apiResponse2)
                                {
                                    item.Type = item.Details.Type;
                                    if (_ownedLegendaries != null && _ownedLegendaries.Count() > 0)
                                        item.Owned = _ownedLegendaries.Any(i => i.Id == item.Id);
                                    else
                                        item.Owned = false;
                                }
                                LegendaryItems = apiResponse2;
                            }
                        }
                    }
                }
            }
        }

        private async Task GetCurrenciesAsync()
        {
            if (API_Key != null && API_Key != "")
            {
                _currencies = new DTO.Currencies();

                using (HttpClient client = new HttpClient())
                {
                    //get currencies
                    Uri uri = new Uri("https://api.guildwars2.com/v2/currencies?ids=all");
                    HttpResponseMessage response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        List<DTO.Currency> apiResponse = JsonConvert.DeserializeObject<List<DTO.Currency>>(await response.Content.ReadAsStringAsync());


                        //get own values
                        Uri uri2 = new Uri("https://api.guildwars2.com/v2/account/wallet?access_token=" + API_Key);
                        HttpResponseMessage response2 = await client.GetAsync(uri2);

                        if (response2.IsSuccessStatusCode)
                        {
                            List<DTO.AccountCurrency> apiResponse2 = JsonConvert.DeserializeObject<List<DTO.AccountCurrency>>(await response2.Content.ReadAsStringAsync());

                            DTO.AccountCurrency def = new DTO.AccountCurrency()
                            {
                                Id = 0,
                                Value = 0
                            };

                            _currencies.Coin = apiResponse.Where(currency => currency.Name == "Coin").FirstOrDefault();
                            _currencies.Coin.Value = apiResponse2.Where(currency => currency.Id == _currencies.Coin.Id).DefaultIfEmpty(def).FirstOrDefault().Value / 10000;

                            _currencies.Gem = apiResponse.Where(currency => currency.Name == "Gem").FirstOrDefault();
                            _currencies.Gem.Value = apiResponse2.Where(currency => currency.Id == _currencies.Gem.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.Karma = apiResponse.Where(currency => currency.Name == "Karma").FirstOrDefault();
                            _currencies.Karma.Value = apiResponse2.Where(currency => currency.Id == _currencies.Karma.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.Laurel = apiResponse.Where(currency => currency.Name == "Laurel").FirstOrDefault();
                            _currencies.Laurel.Value = apiResponse2.Where(currency => currency.Id == _currencies.Laurel.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.FractalRelic = apiResponse.Where(currency => currency.Name == "Fractal Relic").FirstOrDefault();
                            _currencies.FractalRelic.Value = apiResponse2.Where(currency => currency.Id == _currencies.FractalRelic.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.PristineFractalRelic = apiResponse.Where(currency => currency.Name == "Pristine Fractal Relic").FirstOrDefault();
                            _currencies.PristineFractalRelic.Value = apiResponse2.Where(currency => currency.Id == _currencies.PristineFractalRelic.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.UnstableFractalEssence = apiResponse.Where(currency => currency.Name == "Unstable Fractal Essence").FirstOrDefault();
                            _currencies.UnstableFractalEssence.Value = apiResponse2.Where(currency => currency.Id == _currencies.UnstableFractalEssence.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.RedProphetCrystal = apiResponse.Where(currency => currency.Name == "Red Prophet Crystal").FirstOrDefault();
                            _currencies.RedProphetCrystal.Value = apiResponse2.Where(currency => currency.Id == _currencies.RedProphetCrystal.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.GreenProphetCrystal = apiResponse.Where(currency => currency.Name == "Green Prophet Crystal").FirstOrDefault();
                            _currencies.GreenProphetCrystal.Value = apiResponse2.Where(currency => currency.Id == _currencies.GreenProphetCrystal.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.BlueProphetCrystal = apiResponse.Where(currency => currency.Name == "Blue Prophet Crystal").FirstOrDefault();
                            _currencies.BlueProphetCrystal.Value = apiResponse2.Where(currency => currency.Id == _currencies.BlueProphetCrystal.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.TyrianDefenseSeal = apiResponse.Where(currency => currency.Name == "Tyrian Defense Seal").FirstOrDefault();
                            _currencies.TyrianDefenseSeal.Value = apiResponse2.Where(currency => currency.Id == _currencies.TyrianDefenseSeal.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.MagnetiteShard = apiResponse.Where(currency => currency.Name == "Magnetite Shard").FirstOrDefault();
                            _currencies.MagnetiteShard.Value = apiResponse2.Where(currency => currency.Id == _currencies.MagnetiteShard.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.GaetingCrystal = apiResponse.Where(currency => currency.Name == "Gaeting Crystal").FirstOrDefault();
                            _currencies.GaetingCrystal.Value = apiResponse2.Where(currency => currency.Id == _currencies.GaetingCrystal.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                            _currencies.ProvisionerToken = apiResponse.Where(currency => currency.Name == "Provisioner Token").FirstOrDefault();
                            _currencies.ProvisionerToken.Value = apiResponse2.Where(currency => currency.Id == _currencies.ProvisionerToken.Id).DefaultIfEmpty(def).FirstOrDefault().Value;

                        }
                    }

                    //get materials
                    Uri uri3 = new Uri("https://api.guildwars2.com/v2/items?ids=79230,77302,88485,19675,19976,19721");
                    HttpResponseMessage response3 = await client.GetAsync(uri3);

                    if (response3.IsSuccessStatusCode)
                    {
                        List<DTO.Material> apiResponse = JsonConvert.DeserializeObject<List<DTO.Material>>(await response3.Content.ReadAsStringAsync());


                        //get own values
                        Uri uri4 = new Uri("https://api.guildwars2.com/v2/account/materials?access_token=" + API_Key);
                        HttpResponseMessage response4 = await client.GetAsync(uri4);

                        if (response4.IsSuccessStatusCode)
                        {
                            List<DTO.AccountMaterial> apiResponse2 = JsonConvert.DeserializeObject<List<DTO.AccountMaterial>>(await response4.Content.ReadAsStringAsync());

                            _currencies.IntegratedFractalMatrix = apiResponse.Where(currency => currency.Name == "Integrated Fractal Matrix").FirstOrDefault();
                            _currencies.IntegratedFractalMatrix.Value = apiResponse2.Where(currency => currency.Id == _currencies.IntegratedFractalMatrix.Id).FirstOrDefault().Count;

                            _currencies.LegendaryInsight = apiResponse.Where(currency => currency.Name == "Legendary Insight").FirstOrDefault();
                            _currencies.LegendaryInsight.Value = apiResponse2.Where(currency => currency.Id == _currencies.LegendaryInsight.Id).FirstOrDefault().Count;

                            _currencies.LegendaryDivination = apiResponse.Where(currency => currency.Name == "Legendary Divination").FirstOrDefault();
                            _currencies.LegendaryDivination.Value = apiResponse2.Where(currency => currency.Id == _currencies.LegendaryDivination.Id).FirstOrDefault().Count;

                            _currencies.MysticClover = apiResponse.Where(currency => currency.Name == "Mystic Clover").FirstOrDefault();
                            _currencies.MysticClover.Value = apiResponse2.Where(currency => currency.Id == _currencies.MysticClover.Id).FirstOrDefault().Count;

                            _currencies.MysticCoin = apiResponse.Where(currency => currency.Name == "Mystic Coin").FirstOrDefault();
                            _currencies.MysticCoin.Value = apiResponse2.Where(currency => currency.Id == _currencies.MysticCoin.Id).FirstOrDefault().Count;

                            _currencies.Ectoplasm = apiResponse.Where(currency => currency.Name == "Glob of Ectoplasm").FirstOrDefault();
                            _currencies.Ectoplasm.Value = apiResponse2.Where(currency => currency.Id == _currencies.Ectoplasm.Id).FirstOrDefault().Count;
                        }
                    }
                }
            }
        }

        private async Task GetPermissionsAsync()
        {
            if (API_Key != null && API_Key != "")
            {
                using (HttpClient client = new HttpClient())
                {
                    //get currencies
                    Uri uri = new Uri("https://api.guildwars2.com/v2/tokeninfo?access_token=" + API_Key);
                    HttpResponseMessage response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        DTO.TokenInfo apiResponse = JsonConvert.DeserializeObject<DTO.TokenInfo>(await response.Content.ReadAsStringAsync());
                        if (apiResponse != null)
                        {
                            Permissions = apiResponse.Permissions;
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public async Task InitializeAsync()
        {
            Task t1 = Task.Run(async () => await LoadDailyAsync());
            Task t2 = Task.Run(async () => await LoadDailyFractalsAsync());
            Task t3 = Task.Run(async () => await LoadDailyStrikeAsync());
            Task t4 = Task.Run(async () => await LoadDailyDRMsAsync());

            if (_persistence != null)
            {
                AccountInfos state = await _persistence.LoadAccountInfosAsync();
                if (state != null)
                {
                    AccountName = state.Name;
                    API_Key = state.API_Key;
                    OnAccountLoaded();
                }
                else
                {
                    AccountName = null;
                    API_Key = null;
                    OnAccountNotFound();
                }

                _watchList = (await _persistence.GetWatchListAsync()).ToList();
                t1.Wait();
                t2.Wait();
                t3.Wait();
                t4.Wait();

                List<int> idsToRemove = new List<int>();

                foreach (AchievementItemToWatch achievement in _watchList)
                {
                    if (DailyFractalAchievements.All(item => item.Id != achievement.Id) &&
                        DailyStrikeAchievements.All(item => item.Id != achievement.Id) &&
                        DailyDRMAchievements.All(item => item.Id != achievement.Id) &&
                        DailyAchievements.All(item => item.Id != achievement.Id))
                    {
                        idsToRemove.Add(achievement.Id);
                    }
                }

                if (idsToRemove.Count() > 0)
                {
                    foreach (int i in idsToRemove)
                        await RemoveAchievementFromWatchLisAsync(i);
                }
                OnWatchListChanged();
            }

        }

        public async void SubmitAPI(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                await GetDataAsync(key);
                API_Key = key;
                AccountName = _account.name;
            }

            OnAPIVerified();
        }

        public void RemoveAPI()
        {
            API_Key = null;
            AccountName = null;
            _ownedLegendaries = null;
            _raidProgressions = null;
            _currencies = null;
            Permissions = null;

            OnAccountNotFound();
        }

        public async Task SaveAccountAsync()
            => await _persistence.SaveAccountInfosAsync(new AccountInfos()
            {
                Name = AccountName,
                API_Key = API_Key
            });

        public async Task LoadAccountAsync()
        {
            AccountInfos state = await _persistence.LoadAccountInfosAsync();
            if (state != null)
            {
                AccountName = state.Name;
                API_Key = state.API_Key;
                OnAccountLoaded();
            }
        }

        public async Task LoadDailyAsync()
        {
            await GetDailyListAsync();
            OnDailyLoaded();
        }

        public async Task LoadDailyFractalsAsync()
        {
            await GetDailyFractalListAsync();
            OnFractalsLoaded();
        }

        public async Task LoadDailyStrikeAsync()
        {
            await GetDailyStrikeListAsync();
            OnStrikeLoaded();
        }

        public async Task LoadDailyDRMsAsync()
        {
            await GetDailyDRMListAsync();
            OnDRMsLoaded();
        }

        public async Task LoadRaidEventsAsync()
        {
            await GetRaidEventListAsync();
            OnRaidEventsLoaded();
        }

        public async Task LoadLegendaryItemsAsync()
        {
            await GetLegendaryItemListAsync();
            OnLegendaryItemsLoaded();
        }

        public async Task LoadCurrenciesAsync()
        {
            await GetCurrenciesAsync();
            OnCurrenciesLoaded();
        }

        public async Task LoadPermissionsAsync()
        {
            await GetPermissionsAsync();
            OnPermissionsLoaded();
        }

        public async Task AddAchievementToWatchListAsync(int id, string name, string requirement, string icon)
        {
            if (_watchList.All(item => item.Id != id))
            {
                Persistence.AchievementItemToWatch achievement = new Persistence.AchievementItemToWatch() { Id = id, Name = name, Requirement = requirement, Icon = icon };
                _watchList.Add(achievement);
                OnWatchListChanged();

                if (_persistence != null)
                    await _persistence.AddToWatchListAsync(achievement);
            }
        }

        public async Task RemoveAchievementFromWatchLisAsync(int id)
        {
            AchievementItemToWatch achievement = _watchList.FirstOrDefault(item => item.Id == id);
            if (achievement != null)
            {
                _watchList.Remove(achievement);
                OnWatchListChanged();

                if (_persistence != null)
                    await _persistence.RemoveFromWatchListAsync(id);
            }
        }

        #endregion

    }
}
