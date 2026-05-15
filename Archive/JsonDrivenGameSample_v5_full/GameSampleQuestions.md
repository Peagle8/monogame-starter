# Game Sample Integration Plan

## Current Read On The Sample

The `JsonDrivenGameSample_v5_full` folder is a strong reference spike for three related systems:

- JSON-driven NPC dialogue selection
- JSON-driven hints
- JSON-driven journal entries

The most useful pieces to carry over are:

- pure C# content-selection services
- recent-history suppression so repeated lines do not show up too often
- data validation against known IDs and flags
- debug inspection data for understanding why a line or hint was selected
- locale-folder organization for future localization

The sample does not yet give us everything we need for full in-game conversations. It is strongest at weighted content selection, not branching dialogue trees with player choices.

## Recommendation

I do not recommend copying the sample folder into `MyGame` wholesale.

I recommend treating it as a design reference, then building a native `MyGame` narrative layer that matches the current project shape:

- keep gameplay rules in plain C# classes
- keep MonoGame rendering and input at the edges
- fit into the existing `GameplayScene`, `World`, save/load, and DI seams
- start with one narrow playable slice before adding hints and journal UI

Why I would not merge it directly:

- the sample uses its own `GameState` model that does not line up with current save data or world state yet
- the current game only has a shop-counter interaction shell, not a reusable NPC conversation layer
- `SaveGameData` does not yet persist quest state, flags, locale, or recent dialogue history
- the sample loader/bootstrapping path does not match the repo's current `JsonFileLoader<T>` plus `CopyToOutputDirectory` content pattern
- the sample dialogue model supports weighted line selection, but not conversation choices or branching

## Best-Fit Reuse

These sample concepts look worth porting almost as-is:

- `DialogueService`
- `HintService`
- `JournalService`
- `RecentSelectionHistory`
- `DebugInspectionStore`
- `GameDataValidator`
- weighted random selection

These should be adapted rather than copied directly:

- `GameState`
- locale bootstrapping
- file loading
- known ID constants

## Current Game Touch Points

These are the main seams I would build through first:

- `MyGame/Scenes/Gameplay/GameplayScene.cs`
  It currently owns the shop-only interaction/modal flow.
- `MyGame/Gameplay/Shops/ShopDialogueController.cs`
  It is a good starting point, but it is too narrow for general NPC dialogue.
- `MyGame/Gameplay/World/World.cs`
  It already owns core gameplay state and save/apply seams.
- `MyGame/Infrastructure/Save/SaveGameData.cs`
  It will need narrative state fields once dialogue starts depending on flags or quest progress.
- `MyGame/Infrastructure/DependencyInjection/ServiceRegistration.cs`
  This is where the new services will be wired in, likely with a dedicated narrative registration extension.
- `MyGame/Rendering/Menus/WrappedTextLayout.cs`
  This can help with a first-pass dialogue box without inventing a new text wrapping helper.

## Recommended First Slice

I recommend the first real implementation slice be:

1. a reusable narrative state object
2. JSON-backed NPC dialogue loading and validation
3. recent-history suppression
4. one shopkeeper greeting flow driven by JSON
5. one or two simple town NPCs driven by the same system
6. save/load support for the minimum narrative state needed by that slice
7. unit tests around the new pure logic and controller state transitions

That gives us a real reusable system quickly without committing yet to:

- branching dialogue choices
- full hint UI
- full journal UI
- multi-locale production content

## Proposed Implementation Plan

### Phase 1. Add A Native Narrative State

Create a plain C# state object owned by the game, not by MonoGame rendering code.

Suggested responsibilities:

- current zone or scene ID
- active quest ID
- active objective ID
- unlocked or completed narrative flags
- optional future fields like reputation or relationship state

Suggested outcome:

- dialogue, hints, and journal selection can all read from the same state model
- tests can exercise content rules without spinning up rendering or scene code

### Phase 2. Port The Sample Selection Logic Into `MyGame`

Add a new gameplay area such as `MyGame/Gameplay/Dialogue` or `MyGame/Gameplay/Narrative`.

Port or adapt:

- dialogue models
- hint models
- journal models
- recent-history tracking
- debug inspection store
- validator
- weighted random selector

Testing targets:

- weighted selection behavior
- recent-history suppression behavior
- fallback behavior when nothing matches
- validation failures for bad IDs, duplicate IDs, and bad flags

### Phase 3. Add Data Loading And Content Layout

Create a content folder under `MyGame/Content` for narrative JSON.

Suggested first structure:

- `MyGame/Content/Data/en-US/npc_dialogue.json`
- `MyGame/Content/Data/en-US/hints.json`
- `MyGame/Content/Data/en-US/journal_templates.json`

Implementation notes:

- reuse the repo's current JSON loading pattern where practical
- add the JSON files to `MyGame/MyGame.csproj` with `CopyToOutputDirectory`
- decide whether bad data should fail fast at startup or log warnings and continue in development builds

### Phase 4. Replace The Shop-Specific Interaction Assumption

The current interaction path is counter-specific. That is too tight for general dialogue.

I would introduce either:

- a reusable interaction controller that can target counters, shopkeepers, and future NPC props

or:

- a generalized dialogue controller that the shop system can also call into

Key changes:

- give interactable NPC props stable IDs or roles
- let `GameplayScene` ask "what is the active interaction target?" instead of always assuming a counter
- keep world simulation pause behavior explicit when dialogue is open

### Phase 5. Build The First Dialogue Presentation Layer

Use the existing overlay pattern to render a lightweight dialogue panel.

Suggested first-pass behavior:

- show an interact prompt when near an NPC
- open a text panel on interact
- render one selected line plus speaker name
- close on confirm or cancel
- optionally hand off from shopkeeper greeting into the existing buy/sell shell

This keeps rendering simple while the underlying selection rules settle down.

### Phase 6. Hook In Hints And Journal

Once narrative state exists, the hint and journal systems become much cheaper to add.

Possible hint surfaces:

- pause menu
- objective panel
- world toast
- NPC-delivered hint dialogue

Possible journal surfaces:

- a new pause-menu tab
- a dedicated journal screen
- a simple current-entry readout first, then expansion later

I would defer this until after the first NPC dialogue slice is playable.

### Phase 7. Save/Load And Debugging

Extend save/load once the narrative state shape is agreed on.

Likely save additions:

- active quest ID
- active objective ID
- completed or unlocked flags
- recent dialogue history if we want repetition suppression to survive saves
- selected locale if localization is enabled early

Likely debug additions:

- last matched dialogue IDs
- suppressed IDs
- selected ID
- fallback reason

This would pair well with the existing debug overlay and replay-oriented workflow.

### Phase 8. Content Authoring Workflow

After the first slice works, document the authoring rules for future content.

I would add:

- JSON templates for dialogue, hints, and journal entries
- a short content-authoring README
- validator-backed tests that read real sample content files

That will make future NPC and quest additions safer and faster.

## Questions

### 1. What should the first implementation slice include?

My recommendation:

- reusable NPC dialogue first
- hints and journal second

Answer:
Yes lets go with the recommendation

### 2. Do you want the first version of dialogue to stay simple, or do you already want branching choices?

Options I see:

- weighted authored lines only
- short multi-line exchanges
- branching player choices

My recommendation:

- weighted lines first, with room to grow into short multi-line exchanges

Answer:

Yes lets keep it simple to start with an eye towards branching dialogue

### 3. How should the shopkeeper flow work?

Options I see:

- talking to the shopkeeper shows a greeting first, then opens the buy/sell UI
- talking to the counter still opens buy/sell directly, and the shopkeeper is flavor dialogue only
- the shopkeeper and shop service should become one unified interaction

My recommendation:

- greeting first, then hand off into the existing buy/sell shell

Answer:

Sounds good

### 4. Are you ready for a real quest/objective/flag system now?

The sample assumes dialogue can depend on structured quest and flag state.

My recommendation:

- add a small real flag/objective foundation now, even if we only use a few IDs at first

Answer:

Sure lets start small with this

### 5. Which NPCs or areas should be the first content targets after the shopkeeper?

Suggested candidates:

- arena host
- dungeon entrance caretaker
- one or two townsfolk in the overworld

Answer:

Lets go with two townsfolk in the overworld. Just give them placeholder dialogue for now

### 6. Where should hints appear when we wire them in?

Possible homes:

- pause menu
- dedicated objective panel
- on-screen toast
- NPC dialogue only

Answer:

Lets go with both of the last two options.

### 7. Where should the journal live when we wire it in?

Possible homes:

- a new pause-menu tab
- a dedicated journal screen
- defer UI and keep journal logic backend-only for a while

My recommendation:

- backend first, pause-menu tab later

Answer:

Yes lets keep it in the backend and add to our TODO list

### 8. Should recent dialogue suppression survive save/load?

My recommendation:

- yes, if dialogue variety matters across play sessions
- no, if you want a simpler first implementation

My current recommendation:

- save it once dialogue becomes meaningfully stateful

Answer:

Lets go with the recommendation

### 9. Do you want to preserve the locale-folder structure right away, even if we only author English for now?

My recommendation:

- yes, keep the folder structure now
- only ship `en-US` content at first

Answer:

Yes to both

### 10. The sample includes `TownAlertLevel` and `PlayerReputation`. Do you want systems like that in scope now, or later?

My recommendation:

- later
- start with scene, quest, objective, and flag-based dialogue conditions only

Answer:

Yes lets set the foundation for this now and add to the TODO

### 11. Should bad dialogue data fail fast at startup, or log warnings and keep running in development?

My recommendation:

- fail fast for duplicate IDs and broken references
- consider softer handling only for missing optional content

Answer:

Fail fast

### 12. Do you want the sample's debug-inspection output exposed in the in-game debug overlay?

My recommendation:

- yes, but only after the first dialogue slice is working

Answer:

Yes to your recommendation

## My Current Recommendation In One Sentence

Build a native `MyGame` dialogue foundation inspired by the sample, start with shopkeeper plus a few town NPCs, keep the first content model simple and testable, and defer hints, journal UI, and branching conversation until that first slice feels solid.

## Implementation Chunks

### Chunk 1. Playable Dialogue Chunk

Status: Done

Scope:

- reusable NPC interaction detection
- NPC talk prompt
- simple dialogue panel
- two overworld townsfolk connected to JSON-backed dialogue
- recent selected dialogue IDs recorded through the world-owned history

### Chunk 2. Shop Handoff Chunk

Status: Done

Scope:

- shopkeeper or counter greeting first
- hand off from the greeting into the existing buy/sell shell
- keep the shop flow on the same dialogue foundation where practical

### Chunk 3. Debug Chunk

Status: Done

Scope:

- expose matched dialogue IDs
- expose suppressed recent IDs
- expose selected ID
- expose fallback reason

### Chunk 4. Hints Chunk

Status: Done

Scope:

- hint selection backend
- hint delivery through on-screen toasts
- hint delivery through NPC dialogue

### Chunk 5. Journal Chunk

Status: Done

Scope:

- backend journal entry selection
- persistence for journal state
- defer pause-menu journal UI until the backend is useful

### Chunk 6. Future State Chunk

Status: Done

Scope:

- town alert foundation
- player reputation foundation
- connect those fields to dialogue, hints, or journal only after the first systems are playable
