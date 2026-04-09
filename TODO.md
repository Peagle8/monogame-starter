# TODO

This file tracks the current near-term implementation priorities and the working gameplay ideas we want to preserve while the project is still moving quickly.

## Next Up

1. add 1-2 more enemy types with clearly different combat pressure patterns
2. tune combat feel against mixed encounters instead of only single-crab fights
3. use those encounters to judge whether dash and future abilities are actually fun
4. add the next player combat ability only after the sandbox is broad enough to evaluate it properly
5. start early area progression once combat variety is in a better place

## Scene And Structure Notes

1. keep experimenting with scene transitions a little longer before abstracting too early
2. revisit the current `GameRoot` gameplay scene setup once we have many more transitions or sub-scenes
3. break individual level and interior layouts out of `GameplayLevelBuilder` into focused builders or scene-definition types before that class becomes crowded
4. design a cleaner transition registry or scene-link model before a single area grows to something like 20 transitions
5. preserve the current experiment as proof that small interior spaces can reuse the gameplay stack cleanly

## Shop System Spine

1. add shopkeeper dialogue as the next layer on top of the current room and transition flow
2. add wares and a simple shop inventory model after dialogue exists
3. add buy and sell interactions one step at a time so economy rules stay testable
4. use the shop as the first pass for building reusable NPC interaction patterns
5. keep building these systems incrementally so they become the backbone for later world interactions

## Inventory Follow-Up

1. add a player inventory system after the first dialogue and shop interaction pass
2. decide how items, quantities, and currencies should be represented in save-friendly data
3. connect inventory to shop buy and sell flows once the basic storage model exists
4. keep the inventory rules simple at first so the shop UI can iterate without a lot of rewrites

## Interior Presentation Direction

1. eventually make the shop interior feel full screen so entering it clearly feels like entering a building
2. reuse that same presentation pattern for other sub-structures like caves, castles, dungeons, and similar interiors
3. support transitions that can swap not just layout but also camera framing and room presentation rules

## Enemy Priorities

1. add a fast, low-health chaser that pressures spacing and movement
2. add a ranged or lunging enemy that creates directional pressure
3. later add a sturdier enemy that forces commitment and positioning
4. build encounter mixes that make the player use movement, timing, and ability choices
5. add enemy collision or spacing resolution so groups do not pile into unreadable stacks

## Combat Tuning Priorities

1. keep tuning hit readability, knockback, and hit pause as enemy variety expands
2. test combat on controller first when making feel decisions
3. keep placeholder visuals readable enough to judge whether combat feedback is working
4. avoid overinvesting in final graphics before the combat loop feels consistently fun

## Ability System Direction

We want several kinds of abilities:

1. core actions that stay permanently mapped to a button
2. offensive abilities where only one can be equipped at a time
3. defensive abilities where only one can be equipped at a time
4. passive abilities where the player can enable a limited number at once

Progression direction:

1. unlock new abilities over time
2. support both permanent progression and run-based rewards
3. allow progression to increase passive capacity over time
4. later decide whether capacity growth comes from XP, skill points, or another progression currency

## Working Control Schema

Current proposed controller layout:

1. `X` = attack
2. `A` = parry
3. `Right Trigger` = dash
4. `Left Trigger` = ranged attack
5. `Y` = equipped attack ability
6. `B` = equipped defensive ability

Design notes:

1. some abilities should be permanently mapped
2. some abilities should be swappable loadout slots
3. offensive and defensive abilities should likely be separated into different equip categories
4. passive abilities should probably use a capacity system instead of a direct button mapping
5. separate move direction from attack direction long term so combat aim does not depend on movement facing tie-breaks

## Future Systems To Support This

1. player loadout model for equipped offensive, defensive, and passive abilities
2. unlock model that supports both skill tree progression and roguelike rewards
3. save data for persistent progression when that system becomes real
4. debug shortcuts for unlocking abilities during tuning
5. more enemy behaviors so new abilities can be judged in real encounters
