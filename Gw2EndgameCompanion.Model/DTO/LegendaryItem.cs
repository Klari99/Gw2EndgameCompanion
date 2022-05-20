using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2EndgameCompanion.Model.DTO
{
    public class LegendaryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public bool Owned { get; set; }
        public LegendaryItemDetails Details { get; set; }
    }

    public class LegendaryItemDetails
    {
        public string Type { get; set; }
    }
}
