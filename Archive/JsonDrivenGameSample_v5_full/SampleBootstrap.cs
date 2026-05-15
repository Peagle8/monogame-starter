using System;
using JsonDrivenGameSample.Core;
using JsonDrivenGameSample.Core.Ids;
using JsonDrivenGameSample.Core.Localization;
using JsonDrivenGameSample.Gameplay.Dialogue;
using JsonDrivenGameSample.Gameplay.Hints;
using JsonDrivenGameSample.Gameplay.Journal;
using JsonDrivenGameSample.Infrastructure.Data;
using JsonDrivenGameSample.Infrastructure.Debugging;
using JsonDrivenGameSample.Infrastructure.History;
using JsonDrivenGameSample.Infrastructure.Validation;

namespace JsonDrivenGameSample
{
    public static class SampleBootstrap
    {
        public static void Run()
        {
            LocaleSettings locale = new()
            {
                LocaleCode = "en-US"
            };

            JsonDataLoader loader = new();
            LocalizedGameDataRepository repository = new(loader, locale);

            KnownReferences references = new();
            GameDataValidator validator = new(references);
            validator.ValidateAll(repository.Dialogue, repository.Hints, repository.Journal);

            DebugInspectionStore debugStore = new();
            RecentSelectionHistory history = new(maxItemsPerSystem: 2);
            WeightedRandomSelector selector = new(new Random(12345));

            DialogueService dialogueService = new(repository.Dialogue, selector, debugStore, history);
            HintService hintService = new(repository.Hints, debugStore, history);
            JournalService journalService = new(repository.Journal, debugStore, history);

            GameState gameState = new()
            {
                CurrentZoneId = ZoneIds.FloodedRuins,
                ActiveQuestId = QuestIds.ForestShrine,
                TownAlertLevel = 3,
                PlayerReputation = 1
            };

            gameState.Flags.Add(FlagIds.HasLightningCharm);
            gameState.Flags.Add(FlagIds.SparedBanditChief);
            gameState.Flags.Add(FlagIds.FoundCursedRing);

            Console.WriteLine("Dialogue Runs");
            Console.WriteLine(dialogueService.GetLine("Villager", "guarded", QuestIds.ForestShrine, "short"));
            Console.WriteLine(dialogueService.GetLine("Villager", "guarded", QuestIds.ForestShrine, "short"));
            Console.WriteLine(dialogueService.GetLine("Villager", "guarded", QuestIds.ForestShrine, "short"));
            Console.WriteLine();

            Console.WriteLine("Hint Runs");
            Console.WriteLine(hintService.GetHint(gameState, ObjectiveIds.OpenNorthGate));
            Console.WriteLine(hintService.GetHint(gameState, ObjectiveIds.OpenNorthGate));
            Console.WriteLine();

            Console.WriteLine("Journal Runs");
            Console.WriteLine(journalService.GetEntry(gameState).Title);
            Console.WriteLine(journalService.GetEntry(gameState).Title);
            Console.WriteLine();

            Console.WriteLine("Debug Inspection");
            foreach (MatchDebugInfo entry in debugStore.Entries)
            {
                Console.WriteLine($"System: {entry.SystemName}");
                Console.WriteLine($"Matched IDs: {string.Join(", ", entry.MatchedEntryIds)}");
                Console.WriteLine($"Suppressed IDs: {string.Join(", ", entry.SuppressedEntryIds)}");
                Console.WriteLine($"Selected ID: {entry.SelectedEntryId}");
                Console.WriteLine($"Fallback Reason: {entry.FallbackReason}");
                Console.WriteLine($"Result: {entry.ResultText}");
                Console.WriteLine();
            }
        }
    }
}
