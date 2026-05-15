# JSON-Driven Gameplay Sample v5 (MonoGame-Friendly)

This version adds repetition suppression and recent-history tracking.

## Included features

- JSON-driven dialogue, hints, and journal content
- Localization-ready content folders
- Strongly typed ID constants
- Cross-file validation
- Runtime debug inspection
- Recent-history tracking
- Repetition suppression for content selection

## Main design goal

Make the game feel dynamic and varied without needing a local AI model.

## How repetition suppression works

1. Match all valid entries
2. Remove entries used recently when possible
3. Select from the remaining pool
4. If all candidates were used recently, allow the full pool
5. Record the selected ID back into recent history

## Suggested next upgrade

A strong next step would be adding save/load support for recent history or wiring the debug info into a real MonoGame overlay.
