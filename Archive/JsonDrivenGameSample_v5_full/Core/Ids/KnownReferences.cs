using System;
using System.Collections.Generic;

namespace JsonDrivenGameSample.Core.Ids
{
    public sealed class KnownReferences
    {
        public HashSet<string> QuestIds { get; } = new(StringComparer.OrdinalIgnoreCase)
        {
            QuestIds.ForestShrine
        };

        public HashSet<string> ZoneIds { get; } = new(StringComparer.OrdinalIgnoreCase)
        {
            ZoneIds.FloodedRuins
        };

        public HashSet<string> ObjectiveIds { get; } = new(StringComparer.OrdinalIgnoreCase)
        {
            ObjectiveIds.OpenNorthGate
        };

        public HashSet<string> FlagIds { get; } = new(StringComparer.OrdinalIgnoreCase)
        {
            FlagIds.HasLightningCharm,
            FlagIds.WaterChannelActive,
            FlagIds.SparedBanditChief,
            FlagIds.FoundCursedRing
        };
    }
}
