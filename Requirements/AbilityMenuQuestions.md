# Ability Menu Implementation Questions

This file collects the open design questions for the first implementation pass of the `Abilities` inventory tab.

Please answer inline under each question. Short answers are fine.

## Current Project Context

- The pause menu already has an `Inventory` modal with tabs.
- The `Abilities` tab is still placeholder-only.
- Current gameplay code already has:
  - `Dash` as an unlocked ability gate
  - `Shield` as the current defense ability
  - `Fireball` as the current ranged attack
- Melee is currently a basic attack, not yet modeled as an equipable ability type.

## Questions

### 1. Which slots should be treated as real equip slots in this pass?

You mentioned:
- offense
- defense
- ranged attack
- melee

Please confirm:
- Are all 4 slots required in the UI right now?
- If yes, should `offense` and `melee` be separate slots, or are they meant to overlap?

Answer:
Right sorry, melee makes more sense so forget about offense. Let's also add a slot for dash as well. You know the bomb ability the rabbits use when they dash? I think we should have that be a dash ability you can unlock (as an example)

### 2. Which abilities should appear in each slot right now?

Based on the code today, I only see these concrete abilities wired into gameplay:
- `Dash`
- `Shield`
- `Fireball`

Please list what should appear in the menu for each slot for this first pass.

Example format:
- Offense:
- Defense:
- Ranged:
- Melee:

Answer:
We won't implement all of these, for ones that do not have an implementation yet just add the placeholder option but disabled.
- Dash - base dash ability, bomb dash, lightning dash, superspeed
- Defense - base shield, fire shield, ice shield, electricity shield, stealth shield, wind shield, god shield
- Ranged - fireball, wind cutter, missile, bow, compact bow, legendary bow
- Melee - base attack, charged attack, fire sword, ice sword, lightning sword, sword god



### 3. Should equiping be gameplay-real now, or UI-only for some slots?

Right now:
- defense can already be equipped in code
- ranged can already be equipped in code
- dash is an unlock, not an equipped slot
- melee does not yet have an equip model

Please confirm which behavior you want for this pass:
- Equip changes should immediately affect gameplay where supported
- Equip changes can be UI-only placeholders for unsupported slots

Answer:
- Equip changes should immediately affect gameplay where supported
- Also I would like Dash to be equipable, that way we can allow them to swap out the different types.
- All of these except melee should be unlocks. By default now they can all start unlocked so we can test and we can implement that much later

### 4. How should the player navigate inside the Abilities tab?

Proposed default:
- left/right switches inventory tabs
- up/down moves within the abilities screen
- confirm opens a submenu or activates the selected action
- cancel backs out one level

For the abilities tab specifically, do you want:
- a list of slots first, then a submenu with `Equip` and `View Upgrades`
- a list of abilities first, with actions shown on the side
- some other layout

Answer:
Lets try - a list of slots first, then a submenu with `Equip` and `View Upgrades` first

### 5. What should the placeholder upgrade view look like?

For this pass, I can make the upgrade path a non-functional placeholder.

Please choose the direction you want:
- simple text panel saying upgrade data is coming soon
- per-ability placeholder panel with 2-4 named future upgrade nodes
- a visual tree/mock path layout with placeholder nodes

If you want named placeholder upgrades, list them or I can invent temporary names.

Answer:
- a list of slots first, then a submenu with `Equip` and `View Upgrades`
Also: If you want named placeholder upgrades, list them or I can invent temporary names.
Use the ones described in section 2 for now

### 6. What should be shown for locked vs unlocked abilities?

Possible options:
- show only unlocked abilities
- show all planned abilities, with locked ones dimmed
- show known abilities only after discovered/unlocked

Answer:

So I would like to start with:
- show all planned abilities, with locked ones dimmed
BUT with the idea that later we will have it behave like:
- show known abilities only after discovered/unlocked

### 7. Should the menu show currently equipped abilities in one summary area?

I’m assuming yes based on your note.

Please confirm whether you want:
- a persistent summary panel showing each slot and current selection
- only highlight the equipped option inside each slot list
- both

Answer:

- both for now, lets see how it looks and feels.

### 8. Should equip choices persist in save data in this pass?

Right now save data does not store equipped abilities.

Please confirm:
- persist equipped slots now
- keep equip state session-only for the first pass

Answer:

- persist equipped slots now

### 9. What should happen if a slot only has one ability right now?

Example:
- defense may only have `Shield`
- ranged may only have `Fireball`

Should the menu:
- still allow opening `Equip`, but show only one option
- skip `Equip` and just show it as already equipped
- show the slot anyway because it establishes the future structure

Answer:

- show the slot anyway because it establishes the future structure

### 10. Naming question: do you want `Abilities` to stay the top-level tab name?

Possible alternatives if you want clearer wording later:
- `Abilities`
- `Skills`
- `Loadout`
- `Abilities / Loadout`

Answer:

- `Abilities / Loadout`

## Suggested Default If You Want Me To Move Fast

If you want, I can implement this first pass with these assumptions:

- Keep the top-level inventory tab name as `Abilities`
- Show 4 slot sections: `Offense`, `Defense`, `Ranged`, `Melee`
- Show a current equipment summary at the top
- In the abilities tab, selecting a slot opens a small submenu:
  - `Equip`
  - `View Upgrades`
- `Defense` and `Ranged` use real equip state where supported
- `Offense` and `Melee` can be placeholder/UI-only if no gameplay model exists yet
- Upgrade view is a placeholder panel with simple mock nodes
- The screen shows locked and unlocked abilities, with locked ones dimmed
- Save persistence can wait until the leveling/loadout pass unless you want it now

Your approval / edits to this default:
Not needed, I added the needed context for each section, thanks!
