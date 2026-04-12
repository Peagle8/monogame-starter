# TODO

This file tracks the next implementation chunks in a form that is easy to hand off, build, test, and review.

## Current Build Queue

1. `Town interaction shell`
   Add the first reusable NPC interaction flow in the town hub, including prompt detection, dialogue shell plumbing, and at least one quest-style NPC plus one lore-style NPC.
2. `Inventory model V1`
   Add a real player inventory data model behind the current pause-menu inventory shell so weapons, armor, items, and abilities can hold actual entries.
3. `Shop system V1`
   Connect shops to inventory and currency with simple wares, buy flow, sell flow, and save-friendly inventory changes.
4. `Dungeon entrance and floor plumbing V1`
   Add the first dungeon entrance from town, floor transition plumbing, and a simple floor-state model that can later grow into the 100-floor structure.
5. `Arena reward loop V1`
   Decide and implement the first pass of arena rewards beyond inter-wave recovery, likely XP, gold, or crafting materials.

## Recently Completed

1. `Overworld V1 scene network`
   Town hub, four wilderness scenes, cardinal gate transitions, central district layout, placeholder houses, town walls, and mountain-bounded wilderness are now in.
2. `Arena combat prototype room`
   The arena is now a dedicated interior scene with locked/unlocked exits, screen-sized bounds, and multi-wave encounter support.
3. `Pause-menu inventory shell`
   The pause menu now includes an inventory entry with a tabbed modal for weapons, armor, items, and abilities.
4. `Enemy roster expansion pass`
   Added bat miniboss, horned rabbit elite, and a 3-stage horned rabbit boss with arena-tested behavior.
5. `Arena wave framework`
   Arena waves now support banners, inter-wave delays, configurable full-heal between waves, and staged boss progression.

## World Structure Chunks

1. `Town district layout pass`
   Shape the town into a readable street grid with placeholder houses, roads, wall gates, and a central district reserved for dungeon, arena, and three shops.
2. `Wilderness layout pass`
   Give each wilderness scene a distinct first-pass layout, mountain bounds, and placeholder encounter space while keeping the footprint intentionally compact.
3. `Dungeon and arena exterior pass`
   Replace placeholder central-district markers with more intentional exterior entrances and cleaner approach paths.
4. `Interior and sub-area registry cleanup`
   Break scene definitions out of `GameplayLevelBuilder` once the number of areas grows enough to justify dedicated builders or scene-definition types.

## Arena And Encounter Chunks

1. `Arena enemy slice selector V1`
   Let the arena offer encounters based on unlocked dungeon slices, starting with the first 10-floor slice.
2. `Arena reward loop V1`
   Decide and implement the first pass of arena rewards, likely XP, gold, or crafting materials.
3. `Encounter composition pass`
   Keep using the arena as the controlled test bed for enemy mixes, wave pacing, and combat readability.
4. `Enemy roster expansion`
   Add 1-2 more enemies with clearly different pressure patterns before expanding the dungeon ecosystem too broadly.
5. `Boss progression template`
   Establish a repeatable pattern for minion, mini-boss, and boss relationships within one themed enemy ecosystem.

## Town, NPC, And Dialogue Chunks

1. `NPC interaction framework V1`
   Add reusable NPC prompt, facing, interaction, and state plumbing that works for shops, lore, and quests.
2. `Quest giver prototype`
   Add one NPC that can hand out and complete a simple test quest.
3. `Lore NPC prototype`
   Add one NPC that exists only for world flavor and conversation.
4. `Dialogue authoring model`
   Decide whether dialogue is stored as authored lines, branching nodes, topic pools, or another constrained format.
5. `Dynamic dialogue experiment`
   Explore whether LLM-assisted phrasing makes sense for low-stakes flavor dialogue while keeping plot facts and quest rules authored.

## Inventory, Economy, And Progression Chunks

1. `Inventory model V1`
   Add item storage, categories, quantities, and save-friendly inventory state.
2. `Currency and rewards V1`
   Add a simple currency or resource model so combat and shops have meaningful outputs.
3. `Shop system V1`
   Add wares, prices, purchase rules, sell rules, and inventory mutation tests.
4. `Equipment and item use V1`
   Decide what counts as equipable versus consumable and add the first pass of actual item behavior.
5. `Save whistle rules`
   Add the first save-whistle implementation and the rule that it cannot be used in a slice's final boss chamber.

## Dungeon Progression Chunks

1. `Dungeon floor plumbing V1`
   Support floor transitions and a floor-state model that can later scale to 100 floors.
2. `First 10-floor slice prototype`
   Build one themed floor block with placeholder gimmicks, enemy ecosystem, and a boss chamber.
3. `Slice unlock integration`
   Make dungeon progress unlock matching arena encounter sets.
4. `Boss chamber rules`
   Define special save, retry, and transition rules for slice-ending bosses.
5. `Theme and gimmick template`
   Create the repeatable pattern each 10-floor slice will use for ecosystem, boss, and environmental gimmicks.

## Combat And Feel Chunks

1. `Combat tuning pass`
   Keep tuning hit pause, knockback, telegraphs, dash feel, shield / ranged timing, and boss transition readability against mixed encounters.
2. `Controller-first verification`
   Continue making combat feel decisions with controller readability as the baseline.
3. `Enemy readability pass`
   Improve telegraphs, damage communication, and state clarity while placeholder art is still acceptable.
4. `Ability gating skeleton`
   Start moving player abilities behind unlock checks even if they remain unlocked by default during tuning.
5. `Loadout system V1`
   Add the first model for equipped offensive, defensive, and passive ability slots.
