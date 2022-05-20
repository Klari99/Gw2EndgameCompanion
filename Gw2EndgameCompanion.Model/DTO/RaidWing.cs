using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2EndgameCompanion.Model.DTO
{
    public class RaidWing
    {
        public string Id { get; set; }
        public IEnumerable<RaidEvent> Events { get; set; }
    }
}
