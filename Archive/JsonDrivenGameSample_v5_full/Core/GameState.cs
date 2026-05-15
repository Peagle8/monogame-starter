using System.Collections.Generic;

namespace JsonDrivenGameSample.Core
{
    public sealed class GameState
    {
        public string CurrentZoneId { get; set; } = "";
        public string ActiveQuestId { get; set; } = "";
        public int TownAlertLevel { get; set; }
        public int PlayerReputation { get; set; }
        public HashSet<string> Flags { get; } = new();
    }
}
