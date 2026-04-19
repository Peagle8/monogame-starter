# Fire Shield Questions

## Current First-Pass Assumptions

- Fire Shield is immediately equippable from the ability/loadout menu.
- Fire Shield is an offensive aura and does not absorb incoming hits like Base Shield.
- Activating Fire Shield costs 3 AP.
- Fire Shield lasts 9 seconds per activation.
- Enemies take 1 damage every 3 seconds while continuously inside the aura.
- If an enemy leaves the aura, its damage-over-time timer resets.
- The current visual is a procedural placeholder fire ring that we can replace with pixel art later.

## Questions

### 1. Should Fire Shield also block incoming contact damage or special attacks, or should it stay offense-only?

Answer:
Yes let's give all shield abilities the same block as the base shield.

### 2. Is the 9-second duration feeling right, or would you prefer a different duration, a toggle, or an AP-drain model?

Answer:
No lets have the duration stay up as long as the shield is active. Same behavior as the base shield where when you take x number of hits the shield breaks.


### 3. Should Fire Shield remain immediately equippable, or should it become a later unlock?

Answer:
Treat this the same as the bomb dash ability (and this can be applied to all abilities moving forward... let's add this to the design doc). So it will eventually be gated/locked but for now we are leaving it unlocked so we can get immediate manual testing feedback.


### 4. When an enemy leaves and re-enters the aura, should the damage timer reset or pick back up where it left off?

Answer:
Good question, I think it should reset.


### 5. For the placeholder look, do you want the ring to stay mostly red/orange, or should I push it harder toward bright yellow/white flame accents?

Answer:
I love the idea of flame accents. We can keep it red/orange with those flame accents would be good. Thanks!
