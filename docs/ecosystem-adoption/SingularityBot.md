# GrammarAi assessment for SingularityBot

**Consumer coordination:** [SingularityBot issue #12](https://github.com/TrentBest/TheSingularityWorkshop.SingularityBot/issues/12)  
**Disposition:** no current use; revisit at a real external strategy interchange boundary  
**Source reviewed:** `TheSingularityWorkshop.SingularityBot` `master` at `34844c385ff0cb09cedbcc7ee81a692e700c01df`

## Evidence

The repository README explicitly describes the bot as foundation-in-progress and says the strategy format is currently architecture/documentation plus an illustrative JSON file, not a working or ladder-ready bot. `docs/strategy-format.md` describes a proposed versioned declarative strategy format and compilation pipeline, while `strategies/terran-opening.example.json` is explicitly illustrative and not executable.

The reviewed source tree does not yet contain a strategy compiler, strategy-domain model, grammar integration, or a project/package reference to GrammarAi. The architecture's mention of GrammarAi/ProtocolAi as vocabulary support is a planned responsibility, not proof of a current GrammarAi consumer.

## Recommendation

Do not add GrammarAi solely because a future strategy format is planned. First implement the strategy schema and compiler against the actual game ontology and runtime, then determine whether its structure is already adequately expressed by its own versioned JSON contract.

Reconsider GrammarAi only if a distinct structural language is authored by an independent editor/agent and consumed by multiple tools, or if reusable nested strategy composition requires bounded structural validation beyond the strategy schema. Avoid duplicating the strategy validator.

Ownership remains:
- ProtocolAi: canonical protocol and vocabulary identities.
- GrammarAi, if later justified: reusable structural composition and bounded structural validation.
- SingularityBot: strategy semantics, ontology mapping, game-state interpretation, goals, constraints, tactics, compilation, and bot behavior.
- CommandAi: atomic command/proposal contracts.
- Host/runtime: authorization and execution.
- CiAi/HeadlessAi: build and replay evaluation evidence.

No SingularityBot source or dependency changes are proposed.


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
