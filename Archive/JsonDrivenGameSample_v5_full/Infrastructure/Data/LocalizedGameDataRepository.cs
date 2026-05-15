using JsonDrivenGameSample.Core.Localization;
using JsonDrivenGameSample.Gameplay.Dialogue;
using JsonDrivenGameSample.Gameplay.Hints;
using JsonDrivenGameSample.Gameplay.Journal;

namespace JsonDrivenGameSample.Infrastructure.Data
{
    public sealed class LocalizedGameDataRepository
    {
        public DialogueDataFile Dialogue { get; }
        public HintDataFile Hints { get; }
        public JournalDataFile Journal { get; }

        public LocalizedGameDataRepository(JsonDataLoader loader, LocaleSettings locale)
        {
            string root = $"Content/Data/{locale.LocaleCode}";

            Dialogue = loader.Load<DialogueDataFile>($"{root}/npc_dialogue.json");
            Hints = loader.Load<HintDataFile>($"{root}/hints.json");
            Journal = loader.Load<JournalDataFile>($"{root}/journal_templates.json");
        }
    }
}
