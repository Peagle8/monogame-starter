using System;
using System.Collections.Generic;
using System.Linq;
using JsonDrivenGameSample.Core.Ids;
using JsonDrivenGameSample.Gameplay.Dialogue;
using JsonDrivenGameSample.Gameplay.Hints;
using JsonDrivenGameSample.Gameplay.Journal;

namespace JsonDrivenGameSample.Infrastructure.Validation
{
    public sealed class GameDataValidator
    {
        private readonly KnownReferences _references;

        public GameDataValidator(KnownReferences references)
        {
            _references = references;
        }

        public void ValidateAll(
            DialogueDataFile dialogue,
            HintDataFile hints,
            JournalDataFile journal)
        {
            ValidateDialogue(dialogue);
            ValidateHints(hints);
            ValidateJournal(journal);
        }

        private void ValidateDialogue(DialogueDataFile file)
        {
            ValidateUniqueIds(file.Entries.Select(x => x.Id), "dialogue");

            foreach (DialogueEntry entry in file.Entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Id) ||
                    string.IsNullOrWhiteSpace(entry.SpeakerRole) ||
                    string.IsNullOrWhiteSpace(entry.Mood) ||
                    string.IsNullOrWhiteSpace(entry.QuestId) ||
                    string.IsNullOrWhiteSpace(entry.LineStyle) ||
                    string.IsNullOrWhiteSpace(entry.Text))
                {
                    throw new InvalidOperationException($"Dialogue entry '{entry.Id}' has one or more required empty fields.");
                }

                if (!_references.QuestIds.Contains(entry.QuestId))
                {
                    throw new InvalidOperationException($"Dialogue entry '{entry.Id}' references unknown quest id '{entry.QuestId}'.");
                }

                if (entry.Weight <= 0)
                {
                    throw new InvalidOperationException($"Dialogue entry '{entry.Id}' must have a weight greater than zero.");
                }
            }
        }

        private void ValidateHints(HintDataFile file)
        {
            ValidateUniqueIds(file.Entries.Select(x => x.Id), "hints");

            foreach (HintEntry entry in file.Entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Id) ||
                    string.IsNullOrWhiteSpace(entry.ZoneId) ||
                    string.IsNullOrWhiteSpace(entry.ObjectiveId) ||
                    string.IsNullOrWhiteSpace(entry.Text))
                {
                    throw new InvalidOperationException($"Hint entry '{entry.Id}' has one or more required empty fields.");
                }

                if (!_references.ZoneIds.Contains(entry.ZoneId))
                {
                    throw new InvalidOperationException($"Hint entry '{entry.Id}' references unknown zone id '{entry.ZoneId}'.");
                }

                if (!_references.ObjectiveIds.Contains(entry.ObjectiveId))
                {
                    throw new InvalidOperationException($"Hint entry '{entry.Id}' references unknown objective id '{entry.ObjectiveId}'.");
                }

                ValidateKnownFlags(entry.RequiredFlags, entry.Id, "required");
                ValidateKnownFlags(entry.ExcludedFlags, entry.Id, "excluded");
            }
        }

        private void ValidateJournal(JournalDataFile file)
        {
            ValidateUniqueIds(file.Entries.Select(x => x.Id), "journal");

            foreach (JournalTemplateEntry entry in file.Entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Id) ||
                    string.IsNullOrWhiteSpace(entry.Title) ||
                    string.IsNullOrWhiteSpace(entry.Summary))
                {
                    throw new InvalidOperationException($"Journal entry '{entry.Id}' has one or more required empty fields.");
                }

                ValidateKnownFlags(entry.RequiredFlags, entry.Id, "required");
            }
        }

        private void ValidateKnownFlags(IEnumerable<string> flags, string entryId, string area)
        {
            foreach (string flag in flags)
            {
                if (!_references.FlagIds.Contains(flag))
                {
                    throw new InvalidOperationException($"Entry '{entryId}' references unknown {area} flag '{flag}'.");
                }
            }
        }

        private static void ValidateUniqueIds(IEnumerable<string> ids, string areaName)
        {
            List<string> duplicates = ids
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
            {
                throw new InvalidOperationException($"Duplicate ids found in {areaName}: {string.Join(", ", duplicates)}");
            }
        }
    }
}
