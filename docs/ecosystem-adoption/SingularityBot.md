# GrammarAi assessment for SingularityBot

**Consumer coordination:** [SingularityBot issue #12](https://github.com/TrentBest/TheSingularityWorkshop.SingularityBot/issues/12)  
**Disposition:** conditional; optional authoring/knowledge-graph adapter only  
**Source reviewed:** `TheSingularityWorkshop.SingularityBot` `development` at `1b5e6bdfa1110f663d0ba2aad354ac60874336f4`

## What exists

SingularityBot now has a real `BotDefinition` model and a strict JSON parsing/validation path. The artifact composes bot-owned runtime declarations, opening production plans, default goals, strategy transitions, and bounded AI consultation policy. Existing validators enforce schema version and domain-specific invariants. The project references CommandAi and HeadlessAi, but does not reference GrammarAi or ProtocolAi.

CommandAi is already consumed through a bot-owned vocabulary and `BotCommandProposalValidator`. That layer validates action/parameter identity and basic value semantics while deliberately leaving ontology resolution, live-state checks, scheduling, and SC2 legality downstream.

The BotDefinition documentation identifies a possible future GrammarAi role: exposing a typed graph of capabilities, goals, evidence types, strategy relationships, prerequisites, and constraints for Forge or LLM-assisted authoring. However, that graph is not yet a concrete GrammarAi consumer contract. The current BotDefinition JSON and its existing validators remain the authoritative bot-domain artifact.

## Conditional opportunity

Consider a thin optional authoring adapter only if Forge/LLM authoring needs a reusable structural graph that is independently consumed by more than one tool—for example, a versioned strategy/knowledge graph with typed capability, goal, evidence, prerequisite, and constraint relationships. In that case:
- GrammarAi validates the reusable graph structure and bounded composition.
- ProtocolAi may own stable vocabulary identities if/when adopted.
- SingularityBot owns game meaning, ontology resolution, feasibility, strategy validation, and runtime behavior.
- CommandAi remains the bounded action/proposal envelope.
- The host retains authorization and execution.

## Recommendation

Do not add GrammarAi to the bot runtime or duplicate the current BotDefinition JSON Schema and domain validators. Keep this as a conditional authoring-layer opportunity, with a real multi-tool producer/consumer example and mapping tests required before adoption. No SingularityBot source or dependency changes are proposed.


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
