# Bomb Dash Implementation Questions

This file captures the remaining implementation questions for adding the `Bomb Dash` player ability.

Please answer inline under each section. Short answers are totally fine.

## Current Shared Understanding

- `Bomb Dash` should become a real dash ability the player can equip from the abilities menu.
- Longer term, it should be unlocked after defeating the horned rabbit boss.
- For now, you want it available for testing rather than gated away.
- The behavior idea is: when the player uses `Bomb Dash`, they leave a trail of bombs behind them in the direction they dashed.
- Those bombs should explode in a similar style/order to the existing enemy bomb behavior.

## Questions

### 1. Unlock Timing

Should `Bomb Dash` be:

- available by default for now, with the real unlock hook added later
- available by default for now, but also have the real boss-unlock hook implemented now so we can flip it later
- fully gated immediately behind defeating the horned rabbit boss

Recommended:
- available by default for now, but also have the real boss-unlock hook implemented now so we can flip it later

Answer:

Yes your recommendation was my intent, so: available by default for now, but also have the real boss-unlock hook implemented now so we can flip it later

### 2. Bomb Trail Pattern

When you say “same way and ordering as the enemy ones,” which enemy pattern should the player version follow?

Options:
- horned rabbit elite style: bombs dropped in the dash trail over time
- horned rabbit boss style: a spread/pattern placed after movement
- a simplified custom player version inspired by the enemy trail

Recommended:
- horned rabbit elite style: bombs dropped in the dash trail over time

Answer:

Right again, it should be "horned rabbit elite style: bombs dropped in the dash trail over time"... however we will want to add upgrades for the dash ability but we'll go with this for now. Eventually they can unlock the boss pattern and then later other patterns.

### 3. Self-Damage

Should the player be hurt by their own `Bomb Dash` explosions?

Options:
- yes
- no

Recommended:
- no

Answer:

Correct again, we are on the same wave length. The answer is no, no friendly fire.

### 4. Explosion Interactions

What should `Bomb Dash` explosions affect?

Options:
- enemies only
- enemies and grass/destructible foliage
- enemies, grass, and anything else explosive/projectile attacks already affect

Recommended:
- enemies and grass/destructible foliage

Answer:

So I think for now is enemies and grass/destructible foliage.

## One-Line Reply Shortcut

If you want to answer quickly, you can use a one-line format like:

`real unlock later, elite trail yes, no self-damage, damage enemies + grass`

