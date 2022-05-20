using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2EndgameCompanion.Model.DTO
{
    class RaidRegion
    {
        public string Id { get; set; }
        public List<RaidWing> Wings { get; set; }
    }
}
