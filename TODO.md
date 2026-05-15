# TODO

This file tracks the next implementation chunks in a form that is easy to hand off, build, test, and review.

## Current Build Queue

1. `Inventory model V1`
   Add a real player inventory data model behind the current pause-menu tabs so weapons, armor, items, and ability unlock state live in persistent gameplay data instead of UI-only shells.
2. `Shop system V1`
   Connect the current shopkeeper greeting and buy/sell shell to wares, prices, currency, inventory mutation, and save-friendly progression state.
3. `Quest giver prototype`
   Add one NPC that uses the narrative flag/objective foundation to hand out and complete a simple test quest.
4. `Journal UI V1`
   Add a pause-menu journal tab that reads the backend journal state, lists discovered entries, and marks entries read.
5. `Dungeon entrance and floor plumbing V1`
   Turn the current town dungeon entrance into a real gameplay path with floor transitions, floor-state data, and the first reusable dungeon-scene patterns.
6. `Arena reward loop and selection V1`
   Decide and implement the first pass of arena rewards beyond inter-wave recovery, then add encounter selection that can later key off dungeon slice unlocks.
7. `Ability/loadout progression V1`
   Move the current ability/loadout shell from a tuning scaffold into a real progression system with clearer unlock rules, implemented ability variants, and save-friendly state transitions.

## Recently Completed

1. `Overworld V1 scene network`
   Town hub, four wilderness scenes, cardinal gate transitions, central district layout, placeholder houses, town walls, and mountain-bounded wilderness are now in.
2. `Arena combat prototype room`
   The arena is now a dedicated interior scene with locked/unlocked exits, screen-sized bounds, and a five-wave encounter sequence.
3. `Pause-menu map and loadout shell`
   The pause flow now includes an overworld map, replay controls, and a tabbed inventory shell with an ability/loadout view.
4. `Ability save/equip plumbing`
   Save data now persists unlocked abilities plus equipped dash, defense, ranged, and melee loadout choices.
5. `Enemy roster expansion pass`
   Added bat miniboss, horned rabbit elite, and a 3-stage horned rabbit boss with arena-tested behavior.
6. `Enemy readability and pacing pass`
   Enemy health bars are now always visible, bosses use larger bars, and current enemy HP totals were doubled to improve encounter readability and duration.
7. `Shop shell V1`
   The town now has three shop exteriors and one functional interior with a proximity prompt plus buy/sell dialogue shell.
8. `Narrative foundation V1`
   JSON-backed NPC dialogue, reusable NPC prompts/panels, shopkeeper greeting handoff, hints via NPC dialogue/toasts, backend journal state, recent-history persistence, debug inspection data, and town alert/reputation foundations are now in.

## World Structure Chunks

1. `Town district layout pass`
   Keep refining the town into a more readable street grid with stronger landmarks, cleaner approach paths, and clearer separation between homes, hub services, and the central district.
2. `Wilderness layout pass`
   Continue giving each wilderness scene a stronger identity through landmark props, encounter composition, and traversal readability while keeping the footprint intentionally compact.
3. `Dungeon exterior to dungeon entry pass`
   Replace the current dungeon placeholder/exterior treatment with a more intentional entrance and transition into real dungeon content.
4. `Interior and sub-area registry cleanup`
   Break scene definitions out of `GameplayLevelBuilder` once the number of areas grows enough to justify dedicated builders or scene-definition types.

## Arena And Encounter Chunks

1. `Arena reward loop V1`
   Decide and implement the first pass of arena rewards, likely XP, gold, or crafting materials.
2. `Arena encounter selection V1`
   Let the arena offer multiple encounter sets instead of only the current fixed sequence, with room to later gate options by dungeon progress.
3. `Encounter composition pass`
   Keep using the arena as the controlled test bed for enemy mixes, wave pacing, readability, and combat pressure.
4. `Enemy roster expansion`
   Add 1-2 more enemies with clearly different pressure patterns before expanding the dungeon ecosystem too broadly.
5. `Boss progression template`
   Establish a repeatable pattern for minion, mini-boss, and boss relationships within one themed enemy ecosystem.

## Town, NPC, And Dialogue Chunks

1. `NPC interaction framework V1`
   Done. Reusable conversation props, NPC prompt detection, dialogue state, and dialogue panel are now in.
2. `Dialogue presentation V1`
   Done. JSON-backed dialogue drives townsfolk and shopkeeper greeting text, with recent selection suppression.
3. `Hint delivery V1`
   Done. Hints load from JSON and surface through NPC-delivered text plus world toasts.
4. `Journal backend V1`
   Done. Journal entries load from JSON, discover from narrative flags, and persist discovered/read state.
5. `Town reputation and alert foundation`
   Done. Town alert level and player reputation live in narrative state, save/load, and debug output.
6. `Journal UI V1`
   Add a pause-menu tab for discovered journal entries, unread/read state, and entry detail text.
7. `Quest giver prototype`
   Add one NPC that can hand out and complete a simple test quest.
8. `Lore NPC content pass`
   Replace placeholder townsfolk lines with authored town flavor, hint-bearing lines, and journal-linked lore.
9. `Narrative debug overlay polish`
   Decide how much dialogue/hint/journal debug inspection should be visible in the overlay versus recorder state only.
10. `Dialogue authoring model V2`
   Extend the current weighted-line model toward short exchanges, topics, or branching choices.
11. `Narrative content validation tests`
   Add tests that load the real JSON files and validate all shipped dialogue, hints, and journal templates.
12. `Dynamic dialogue experiment`
   Explore whether LLM-assisted phrasing makes sense for low-stakes flavor dialogue while keeping plot facts and quest rules authored.

## Inventory, Economy, And Progression Chunks

1. `Inventory model V1`
   Add item storage, categories, quantities, and save-friendly inventory state behind the current UI shells.
2. `Currency and rewards V1`
   Add a simple currency or resource model so combat and shops have meaningful outputs.
3. `Shop system V1`
   Add wares, prices, purchase rules, sell rules, and inventory mutation tests.
4. `Equipment and item use V1`
   Decide what counts as equipable versus consumable and add the first pass of actual item behavior.
5. `Ability/loadout progression V1`
   Connect the current slot-based loadout shell to real unlock sources, implemented variants, and clearer equip restrictions.
6. `Save whistle rules`
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
   Keep tuning hit pause, knockback, telegraphs, dash feel, shield / ranged timing, boss transition readability, and the new enemy HP/bar presentation against mixed encounters.
2. `Controller-first verification`
   Continue making combat feel decisions with controller readability as the baseline.
3. `Enemy readability pass`
   Improve telegraphs, damage communication, and state clarity while placeholder art is still acceptable.
4. `Ability gating cleanup`
   Reconcile the current tuning-default unlock setup with the intended progression path so unlock behavior is deliberate instead of half-scaffold and half-final.
5. `Implemented ability variants`
   Turn more of the current loadout catalog from placeholder entries into real combat behaviors.
