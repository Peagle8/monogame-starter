# Game Design Direction

This document captures the current high-level direction for the project so we can keep making technical decisions that support the game we actually want to build.

## Core Priorities

The current order of priorities is:

1. make the game fun to play first
2. tune combat feel before investing heavily in presentation
3. preserve testable architecture so new combat and progression systems do not turn into cleanup work later

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
2. attacks should read as intentional weapon swings instead of abstract boxes
3. enemies and player should react to hits through knockback and hit pause
4. placeholder art should still communicate action clearly

## Player Abilities

Player actions like dash should not be permanently hardcoded as always available.

The intended long-term direction is:

1. active abilities can be unlocked and gated
2. some abilities may come from a permanent progression path such as a skill tree
3. some abilities may come from run-based rewards such as a roguelike chest reward
4. the same gameplay ability system should support both sources

Dash is the first candidate for this pattern.

Current architecture direction:

1. input requests an action
2. gameplay checks whether the player currently has the related ability
3. the action executes only if unlocked and otherwise legal

This keeps tuning easy while protecting us from future refactors when more abilities are added.

## Progression Model

The likely progression model is hybrid:

1. permanent progression through a skill tree or similar meta-progression
2. run-specific progression through random rewards, likely from treasure chests or similar pickups

That gives room for both:

1. long-term ownership and build identity
2. run variety and experimentation

Near-term rule:

Abilities should be added behind unlock checks early, even if they default to unlocked during tuning.

## World Structure

The world is now trending toward a compact Zelda-like hub-and-dungeon structure rather than one endlessly extended map.

Planned structure:

1. the current overworld becomes the main town hub
2. the town hub is fenced in and acts as the social / economy / preparation space
3. a small wilderness ring surrounds the town in all directions
4. mountains form the outer bounds of the full map so the world stays readable and intentionally sized
5. the dungeon sits near the center of the map as the main progression spine
6. the arena also sits near the center and acts as a combat challenge / practice space

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

## Arena Role

The arena is now useful beyond being a combat prototype room.

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

One current progression idea worth preserving is the save whistle.

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

## Implementation Notes

As systems expand, the codebase should keep the following split:

1. input mapping stays at the edges
2. gameplay rules stay in plain C# services and state objects
3. unlock rules stay separate from raw input requests
4. save/load should eventually include unlocked abilities once real unlock sources exist

## Near-Term Sequence

The most likely next sequence is:

1. keep refining combat feel
2. add the first ability gating skeleton with dash as the initial case
3. continue improving combat interactions and enemy behavior
4. start building the shared ability framework that can later support both skill tree and run rewards
5. move into early area and boss progression once the combat loop is consistently fun
