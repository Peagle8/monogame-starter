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

The world should expand into multiple distinct areas rather than one endlessly extended map.

Planned structure:

1. the player progresses through several areas
2. each area ends with a boss gate
3. defeating the boss opens progression to the next area
4. transition to the next area happens through an exit path the player moves into
5. the next area then loads in

This gives world building a gameplay spine instead of adding content without progression structure.

## Visual And Presentation Philosophy

Presentation should support gameplay clarity first.

Current philosophy:

1. do not overinvest in graphics before the combat loop is fun
2. keep improving readability and feedback even with placeholder visuals
3. reserve larger art and animation passes for after the game loop and progression feel solid

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
