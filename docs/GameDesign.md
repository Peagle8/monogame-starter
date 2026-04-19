# Game Design Direction

This document captures the current high-level direction for the project so we can keep making technical decisions that support the game we actually want to build.

## Current Project State

The project currently plays like a compact top-down action RPG prototype with these pieces already in place:

1. a town hub connected to north, south, east, and west wilderness scenes
2. a dedicated arena interior with a four-wave encounter sequence
3. a dedicated shop interior plus three shop exteriors in town
4. a pause menu with map, inventory tabs, replay controls, and an ability/loadout shell
5. save/load support for scene state, enemy state, unlocked abilities, and equipped loadout choices
6. a seven-enemy roster built around crabs, horned rabbits, bats, elites, and bosses
7. always-visible enemy health bars and a recent across-the-board enemy HP increase for clearer encounter pacing

This matters because the next design decisions should build on these playable systems instead of describing a completely different game from scratch.

## Core Priorities

The current order of priorities is:

1. make the game fun to play first
2. tune combat feel before investing heavily in presentation
3. preserve testable architecture so new combat and progression systems do not turn into cleanup work later
4. turn the current UI and progression shells into real game loops only after the underlying feel is solid

Placeholder visuals are acceptable while combat, control feel, progression, and world structure are still taking shape.

## Combat Direction

The game is trending toward an action-focused combat loop where feel matters more than raw feature count.

Current combat goals:

1. strong hit readability
2. satisfying impact on attack connect
3. movement that feels good on controller
4. combat tuning that is easy to iterate on through config and tests

Current feel decisions already pointing in the right direction:

1. controller support comes before deeper combat expansion
2. attacks should read as intentional weapon swings and deliberate abilities instead of abstract boxes
3. enemies and player should react to hits through knockback and hit pause
4. placeholder art should still communicate action clearly
5. enemy health should be readable in moment-to-moment combat without needing the player to infer everything from flashes alone

Current combat state:

1. melee, fireball, shield, fire shield, dash, and bomb-dash trail behavior are all present in the prototype
2. the current arena acts as the best place to tune mixed-enemy pressure and boss readability
3. enemy health bars are now always visible, with larger boss bars, because that reads better in live play than hit-only fades
4. enemy health totals were recently doubled so combat has more room for telegraphs, reactions, and ability use

## Player Abilities

Player actions like dash should not be permanently hardcoded as simple one-off buttons forever.

The current implemented direction is:

1. the pause menu now exposes four loadout slots: dash, defense, ranged, and melee
2. equip state is already saved and restored
3. a catalog of future ability variants exists even though many entries are still placeholders
4. unlock checks already exist in code for the abilities that have started moving beyond pure tuning scaffolding

Important current nuance:

1. some unlock/plumbing work is ahead of the final progression design
2. bomb-dash unlock behavior exists in code, but the broader ability-progression loop is not fully settled yet
3. this is acceptable for now as long as we keep documenting which parts are real progression and which parts are tuning conveniences
4. the current rule moving forward is: new abilities can stay temporarily unlocked while we do manual tuning, then they should be wired into the same real unlock/gating path that bomb dash is proving out

Long-term direction:

1. active abilities can be unlocked and gated intentionally
2. some abilities may come from a permanent progression path such as a skill tree
3. some abilities may come from run-based rewards such as arena rewards, dungeon finds, or similar pickups
4. the same gameplay ability system should support both sources

Architecture direction:

1. input requests an action
2. gameplay checks whether the player currently has the related ability
3. the action executes only if unlocked and otherwise legal

This keeps tuning easy while protecting us from future refactors when more abilities are added.

## Progression Model

The likely progression model is hybrid:

1. permanent progression through a skill tree or similar meta-progression
2. run-specific progression through dungeon, arena, or loot rewards
3. loadout choices that let the player express a build without requiring a giant system too early

That gives room for both:

1. long-term ownership and build identity
2. run variety and experimentation

Current missing pieces:

1. there is no real inventory model yet
2. there is no economy or currency loop yet
3. the shop and loadout screens are ahead of the reward loop that should feed them

## World Structure

The world is now trending toward a compact Zelda-like hub-and-dungeon structure rather than one endlessly extended map.

Current implemented structure:

1. the overworld acts as the main town hub
2. the town hub is fenced in and functions as the preparation space
3. the town connects to a small wilderness ring in all four directions
4. the central district currently contains the arena entrance, shop exteriors, and a dungeon exterior/entrance prop
5. one shop interior and the arena interior are playable; the dungeon path is still scaffolding

Planned structure:

1. the town remains the main social / economy / preparation space
2. the small wilderness ring stays readable and intentionally sized
3. the dungeon becomes the main long-form progression spine
4. the arena stays near the center as a combat challenge / practice space
5. shops and NPC interiors gradually make the town feel inhabited instead of schematic

This gives the world a clear gameplay loop:

1. prepare in town
2. explore nearby wilderness and structures
3. progress the dungeon
4. return to town and the arena for upgrades, quests, practice, and story beats

## Dungeon Structure

The dungeon is currently envisioned as the main long-form progression feature.

Planned structure:

1. 100 total floors
2. every 10 floors forms a themed slice
3. each 10-floor slice has its own ecosystem, gimmicks, and enemy families
4. floor 10 of each slice ends in a major boss encounter
5. enemy families should support minion, mini-boss, and boss variants within each slice

The current mental model is not "infinite procedural descent." It is more like a structured action RPG dungeon climb with strong authored themes.

Current status:

1. the dungeon entrance exists in the town layout
2. actual dungeon floor plumbing and content are still next-step work
3. arena and overworld structure are currently standing in as the proof of the scene and progression framework

## Arena Role

The arena is now useful beyond being a combat prototype room.

Current implemented uses:

1. it is the most controlled place to test enemy readability, health pacing, and mixed encounter pressure
2. it already supports waves, banners, inter-wave recovery, and staged boss/miniboss escalation
3. it is currently a fixed sequence rather than a player-selected activity

Planned uses:

1. use it as the first place to test enemy encounter design and wave pacing
2. use it to prototype level-lock rules, combat readability, and arena-specific presentation
3. unlock arena encounter sets based on dungeon progress
4. start with encounters themed around the first unlocked 10-floor dungeon slice
5. later allow the arena to serve as both practice space and a controlled grind space for XP, gold, or materials

Arena unlock direction:

1. each 10-floor dungeon slice unlocks a corresponding arena option
2. the arena lets the player practice enemies they have already met in the dungeon
3. this keeps the arena tied to main progression instead of feeling like a disconnected side mode

## Visual And Presentation Philosophy

Presentation should support gameplay clarity first.

Current philosophy:

1. do not overinvest in graphics before the combat loop is fun
2. keep improving readability and feedback even with placeholder visuals
3. reserve larger art and animation passes for after the game loop and progression feel solid

Art direction note:

1. placeholder art is still the right call for now
2. the long-term plan is for your son to create the pixel art
3. Pixaki on iPad is the expected art pipeline for those assets right now

## NPCs, Town Life, And Dialogue

The town hub should eventually feel inhabited, not just functional.

Current implemented state:

1. the town has structure, entrances, and service markers
2. the shopkeeper interaction shell exists in the functional shop interior
3. there are not yet broader town NPCs, quests, or real dialogue content outside the shop shell

Planned town uses:

1. shops
2. quest-giving NPCs
3. optional lore conversations
4. small character moments that make the hub feel like home base

Dialogue direction:

1. NPC gameplay roles and plot function should be authored and intentional
2. dialogue does not necessarily need to be fully hand-authored line by line at first
3. a practical route may be authored conversation branches, prompts, moods, or topic pools
4. language generation is a promising fit for flavor dialogue, lore variation, or low-stakes conversational texture
5. if LLM dialogue is used, it should likely stay constrained by authored character rules, quest state, and lore facts rather than being fully open-ended

This suggests a useful split:

1. narrative intent, quest logic, and canon facts stay designed by us
2. the exact phrasing can potentially become more dynamic later

## Save Rules

Current save/load already supports:

1. active scene restoration
2. player position, health, and ability points
3. unlocked abilities
4. equipped dash, defense, ranged, and melee abilities
5. enemy health and defeat state

One progression idea worth preserving beyond the current save flow is the save whistle.

Planned rule:

1. the player can record a save point on most floors and areas
2. the final boss chamber of each 10-floor dungeon slice is excluded
3. this keeps tension around boss checkpoints without making the rest of the dungeon too punishing

## Genre Identity

The project is increasingly reading less like "a roguelike with Zelda flavor" and more like:

1. a Zelda-like action RPG
2. with heavier progression and system layering
3. with some structure and tension ideas borrowed from roguelikes
4. and possibly a bit of Hollow Knight energy in tone, challenge, or progression feel

That is a useful clarification, because it suggests the game should prioritize:

1. a strong hub
2. meaningful NPCs
3. authored dungeon slices
4. persistent progression
5. combat mastery through both exploration and practice spaces

## Diagnostics And Tuning

Diagnostics are part of development, not an afterthought.

Current and planned uses:

1. input recording and replay for reproducible debugging
2. debug overlays for scene and combat state
3. configurable combat values through JSON-backed settings
4. future regression coverage for combat and progression rules
5. rapid balancing of enemy pacing, health, and encounter readability through tests plus live play

## Implementation Notes

As systems expand, the codebase should keep the following split:

1. input mapping stays at the edges
2. gameplay rules stay in plain C# services and state objects
3. unlock rules stay separate from raw input requests
4. save/load should continue to persist unlocked and equipped ability state as progression systems become more real
5. UI shells should not be mistaken for complete systems; inventory, economy, and loadout should each gain backing gameplay data in deliberate stages

## Near-Term Sequence

The most likely next sequence is:

1. turn the current inventory, shop, and loadout shells into backed gameplay systems
2. add the first reusable town NPC interaction and dialogue content
3. build the dungeon entrance and floor plumbing off the current overworld structure
4. give the arena rewards and encounter selection that tie it back to progression
5. keep refining combat interactions, enemy readability, and boss pacing as new content is added
