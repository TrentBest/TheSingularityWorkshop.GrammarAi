# GrammarAi across the Workshop ecosystem

GrammarAi is a reusable structural contract, not a mandatory dependency for every repository. Adopt it where a consumer needs to describe which typed things may be connected, in what role, with what cardinality and order. Keep local runtime models where they are already clearer or performance-critical.

## Integration candidates

| Priority | Consumer | Candidate use | Boundary / first proof |
|---|---|---|---|
| 1 | CiAi | Workflow plans: repository, trigger, steps, evidence, dependencies, conditions, matrix entries, provenance | GrammarAi describes plan shape and references ProtocolAi IDs. CiAi owns workflow semantics, evidence policy, provider mapping, runtime and authorization. Prove against the existing CiWorkflowPlanValidator; do not replace it. |
| 2 | CommandAi | Typed command structure: instruction, target, arguments, nested values, ordered operations | GrammarAi can define permitted shape; CommandAi remains the compact command-proposal envelope and typed-value contract. Host validates, authorizes and dispatches. Avoid a second competing command serialization format. |
| 3 | FSM_COS / MicroBundleDomain | Experience/microbundle composition: capability, dependency, configuration role, provider, lifecycle relation | GrammarAi can describe an AI-proposed composition or authoring draft. MicroBundleDomain owns canonical domain contracts; FSM_COS owns resolution, arbitration, load planning and execution. Do not make GrammarAi a runtime dependency unless a real consumer needs it. |
| 4 | Ontology | Typed relationship/profile validation over ontology addresses | GrammarAi may provide a schema for allowed predicates/roles and cardinality. Ontology remains the canonical semantic address/index/relationship store. Do not duplicate OntologyRelationship or turn grammar slots into ontology predicates. |
| 5 | Gestures | Activity definitions: phases, roles, prerequisites, participants, synchronization and ordered procedure steps | GrammarAi can validate an AI-authored draft shape. Gestures owns physical procedure semantics, time, evaluation and actor/world contracts; Physics remains authoritative for physical outcomes. |
| 6 | Economy | Offers, accounts, resources, exchange events, policies, market/competition structures | GrammarAi can constrain authoring and proposal shape. Economy owns accounting, policy, simulation and settlement. Never treat structural validity as financial authorization. |
| 7 | Physics / Learning | Experiment definitions: inputs, assumptions, parameters, stages, observations and result references | Useful for authoring and explanation maps, not numerical execution. Physics owns units, equations, solvers, determinism and validation. Learning owns pedagogy and lesson progression. |

## Architectural rule

Use a GrammarAi schema only when all three are true:

1. The shape is shared across multiple producers/consumers or is proposed by an AI/human authoring surface.
2. The shape has meaningful typed relationships, cardinality, ordering, or versioned compatibility.
3. A consumer benefits from compact integer-addressed exchange and independent structural validation.

Otherwise, keep the simpler local C# type. GrammarAi is not an ontology database, parser, workflow engine, serializer replacement, or authorization system.

## Cross-package identity contract

- ProtocolAi owns stable vocabulary/category/member identities.
- GrammarAi owns node-kind IDs, slot IDs, typed composition, and structural validation.
- A GrammarAi slot may target a nested GrammarAi node kind or an opaque external ProtocolAi category.
- An external relationship carries the qualified pair (protocol/category ID, member symbol ID). Member IDs are not globally unique by themselves.
- The host supplies a resolver bound to the exact request catalog/capability snapshot. GrammarAi fails closed if external values are present and no resolver is supplied.
- The consuming domain owns meaning and policy. The host owns trust, authorization, approval and execution.

## Suggested adoption order

1. CiAi: a non-executing workflow-plan proposal using the existing CiAi schema and validator as the oracle.
2. CommandAi: compare shape descriptions, but retain CommandAi's current proposal wire contract.
3. MicroBundleDomain/FSM_COS: composition-authoring draft only, with runtime validation remaining authoritative.
4. Ontology and Gestures: prove one small profile each before considering shared package references.
5. Economy, Physics and Learning: adopt only after a concrete authoring workflow demonstrates value.

Each integration should begin with a schema/adapter proof in the consumer repository, not a new mandatory dependency edge. Pin exact GrammarAi package/commit versions and retain consumer-owned semantic validation.

---
<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
