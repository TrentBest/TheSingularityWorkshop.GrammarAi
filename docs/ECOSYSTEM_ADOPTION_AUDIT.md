# GrammarAi ecosystem adoption audit

**Audit date:** 2026-11-14  
**Working branch:** `development`  
**Coordination register:** [GrammarAi #20](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/issues/20)  
**Purpose:** decide where GrammarAi adds real structural value without creating unnecessary dependencies or moving domain authority.

## Executive decision

GrammarAi should not become a mandatory package across the Workshop. It is useful when a real producer/consumer boundary needs a versioned, typed structure with meaningful roles, slots, ordering, cardinality, or nested composition—especially when an AI or authoring surface proposes that structure and a host must validate it independently.

The current repository coordination already contains 27 consumer-specific issues. They are not 27 approved integrations. They are requests for each owner to confirm **adopt**, **adapter-only**, **document-only**, or **no current use**. The audit found all 27 linked issues still open on 2026-11-14. No consumer issue should be duplicated.

## Adoption rule

Recommend GrammarAi only when all of the following are true:

1. There is a real shared structure or an AI-/author-authored proposal—not merely an ordinary local configuration object.
2. Its relationships, roles, ordering, cardinality, or compatibility need independent structural validation.
3. A consumer can test the mapping against its actual domain model and a pinned GrammarAi revision.
4. The domain repository can retain its own meaning, truth, semantic invariants, quality rules, security, policy, and runtime behavior.

If a local C# model or an existing domain validator already expresses the contract more clearly, keep it. GrammarAi must not become a parser, database, query engine, workflow engine, physics solver, renderer, policy engine, evidence verifier, authorization layer, or executor.

## Confirmed architectural ownership

- **ProtocolAi:** canonical vocabulary and protocol/category/member identities.
- **GrammarAi:** grammar identity, node kinds, slots, productions, typed composition, bounded structural validation, and structural proposal representation.
- **Consumer domain:** meaning, truth, domain invariants, semantic validation, quality, and runtime behavior.
- **Host / FSM_COS / Foundry:** request context, trust, policy, authorization, orchestration, and execution according to their respective boundaries.
- **CiAi:** CI provider evidence, completeness, evidence verification, and gate decisions.
- **CommandAi:** atomic command and proposal contracts; host remains responsible for authorization and execution.

A structurally valid proposal is not thereby true, safe, physically valid, policy-compliant, authorized, or executable.

## Repository-by-repository disposition

This table is an initial triage, not a claim that every source tree has been exhaustively inspected. The linked consumer issue is the coordination point; the consumer owner must confirm the disposition and identify the real test.

| Repository | Initial disposition | Concrete structural opportunity | Boundary / next proof |
|---|---|---|---|
| [CiAi #59](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/59) | **Adopt; implementation underway** | Versioned CI workflow plan: ordered steps, dependencies, triggers, typed inputs/outputs, capabilities, side effects, expected evidence roles | CiAi retains workflow semantics, live evidence verification, gate mathematics, and fail-closed authority. Test real CiWorkflowPlan data against the pinned GrammarAi schema. GrammarAi #14 remains a separate consumer-proof/coordination issue. |
| [StoryBuilder #8](https://github.com/TrentBest/StoryBuilder/issues/8) | **Best next domain pilot** | TimelineGrammar over actual StoryProject, TimelineEvent, and PassageAnchor models | StoryBuilder retains chronology interpretation, canon, provenance, observer knowledge, narrative quality, and persistence. Require real-model consumer tests, not duplicate DTOs. |
| [CommandAi #10](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/issues/10) | **Adopt as optional structural composition** | Grammar describing bounded collections of atomic commands, action slots, parameter slots, and nested proposal shape | CommandAi retains atomic command contracts, codecs, limits, and semantics. Do not replace its wire format or require GrammarAi at runtime absent proof. |
| [ProtocolAi #15](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/issues/15) | **Foundational counterpart; not a GrammarAi consumer** | Stable vocabulary/category/member identities referenced by GrammarAi | Keep ProtocolAi as identity owner. Confirm qualified references and compatibility rules; do not create a competing vocabulary registry. |
| [FSM_UserIO #12](https://github.com/TrentBest/FSM_UserIO/issues/12) | **Strong candidate; confirm real shared contract** | Typed semantic interaction request/response and parameterized intent envelope | FSM_UserIO retains interaction meaning and lifecycle; Gestures retains recognition; host retains trust and policy. Test an actual SemanticIntent/response. |
| [Experiences #60](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/60) | **Conditional; authoring/adapter first** | Experience manifest and composition-closure graph | Experiences retains identity, artifact integrity, publication, materialization, and lifecycle. GrammarAi must not verify hashes or authorize capabilities. Test an actual manifest. |
| [FSM_COS #55](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/issues/55) | **Conditional; prefer narrow adapter** | AI-/author-proposed capability and composition declaration | FSM_COS retains composition, catalog resolution, context, trust, security, and policy. Do not add GrammarAi to the runtime core without a consumer proof. |
| [FSM_Foundry #37](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/issues/37) | **Conditional; no workflow-engine overlap** | Declarative orchestration/step-plan shape where shared authoring exists | Foundry retains scheduling, lifecycle, state progression, retries, and execution. CiAi owns evidence gates; CommandAi owns atomic commands. |
| [MicroBundleDomain #27](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/issues/27) | **Conditional; schema adapter only** | Authoring draft for capability/port/configuration relationships | MicroBundleDomain retains canonical MicroBundle contracts and meaning. GrammarAi must not become a mandatory domain dependency. |
| [MicroBundleRepository #36](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/36) | **No direct runtime dependency indicated** | Possibly an interchange-only artifact-reference/dependency declaration | Repository owns storage, retrieval, versioning, indexing, and integrity. GrammarAi must not access storage or verify hashes. |
| [MicroBundleIngestor #5](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleIngestor/issues/5) | **Conditional; envelope adapter only** | Ingestion request/result or source-description structure | Ingestor owns acquisition, inspection, validation, trust, and publication behavior. GrammarAi must not ingest or trust content. |
| [Ontology #8](https://github.com/TrentBest/TheSingularityWorkshop.Ontology/issues/8) | **Complementary; document/adapter only** | Grammar structures may reference ontology identities | Ontology retains semantic identity and relationship truth. Do not collapse ontology predicates into grammar slots or add a dependency in the wrong direction. |
| [Gestures #35](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/issues/35) | **Conditional; authoring draft only** | Ordered gesture/activity phases and typed actor/target/action references | Gestures retains recognition, temporal/spatial interpretation, and physical procedure evaluation; Physics owns physical validity. Structure is not proof of intent. |
| [Learning #3](https://github.com/TrentBest/TheSingularityWorkshop.Learning/issues/3) | **Conditional; choose one artifact** | Lesson/activity/lab/observation composition | Learning retains pedagogy, adaptation, assessment, grading, and mastery. Start with one actual lesson or lab model, not a parallel curriculum schema. |
| [Physics #51](https://github.com/TrentBest/TheSingularityWorkshop.Physics/issues/51) | **Conditional; symbolic authoring only** | Bounded symbolic expression/model shape | Physics retains units, dimensional analysis, physical validity, solvers, numerical methods, and simulation. GrammarAi is not a math engine. |
| [Economy #28](https://github.com/TrentBest/TheSingularityWorkshop.Economy/issues/28) | **Conditional; proposal shape only** | Offer/order/market or simulation configuration structure | Economy retains currency, pricing, accounting, transaction rules, risk, and policy. Structural validity never authorizes a transaction. |
| [Renderer #36](https://github.com/TrentBest/TheSingularityWorkshop.Renderer/issues/36) | **No direct runtime dependency indicated** | Possible interchange grammar for a renderer document/request | Renderer remains a MicroBundle and owns rendering semantics. Keep its consumption model; GrammarAi must not become a renderer or NuGet consumption dependency. |
| [Visualize #47](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/issues/47) | **Conditional; only if a shared authored scene contract emerges** | Typed scene/representation graph with references to domain objects | Visualize owns world/visual representation behavior; Physics owns physical models; Ontology owns semantic identity. No simulation or rendering responsibility moves to GrammarAi. |
| [GUI #79](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues/79) | **Conditional; authoring/interchange only** | UI composition, component slots, view declarations, and bindings | GUI owns presentation and interaction mechanics. Canonical domain state stays in domain repositories. |
| [AnyApp #59](https://github.com/TrentBest/AnyApp/issues/59) | **Adapter-only unless host-authored manifests need shared validation** | Host configuration and composition-request envelope | AnyApp retains user/session context, trust, policy, authorization, native catalog/materialization, and hosting. Experiences/FSM_COS own their respective semantics. |
| [WebApp #16](https://github.com/TrentBest/TheSingularityWorkshop.WebApp/issues/16) | **Adapter-only / share with AnyApp** | Browser request envelope or host configuration shared with sibling hosts | WebApp retains browser hosting and web-specific behavior. Avoid a WebApp-only grammar that duplicates the AnyApp contract. |
| [ServerApp #12](https://github.com/TrentBest/TheSingularityWorkshop.ServerApp/issues/12) | **Conditional; protocol/manifest adapter** | Server-role manifest or shared session/message envelope | ServerApp retains server behavior, identity/context, persistence, security, and network execution. ProtocolAi owns message identities. |
| [MyVR #32](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/32) | **Adapter-only / share with Experience and UserIO contracts** | Experience request and interaction envelope | MyVR retains device/runtime and spatial interaction behavior; Gestures/FSM_UserIO own interaction semantics; Visualize owns representation. |
| [Profiles #7](https://github.com/TrentBest/TheSingularityWorkshop.Profiles/issues/7) | **No direct dependency indicated; privacy review required** | Only a privacy-safe, reusable preference/profile declaration | Profiles retains consent, privacy, access, and lifecycle. Do not expose sensitive profile semantics through generic grammar. |
| [DataWarehouse #8](https://github.com/TrentBest/TheSingularityWorkshop.DataWarehouse/issues/8) | **No direct runtime dependency indicated** | Possibly a typed record/query envelope at an external interchange boundary | DataWarehouse retains storage, indexing, retrieval, and persistence; Ontology retains semantic identity. GrammarAi is not a query engine. |
| [Hermit #4](https://github.com/TrentBest/TheSingularityWorkshop.Hermit/issues/4) | **No current use until a concrete task/object contract exists** | Possible task-plan or object/constraint declaration | Hermit retains orchestration and manipulation; Physics owns physical validity; host owns authorization. Avoid speculative dependency. |
| [FSM_NeuralNetworks #3](https://github.com/TrentBest/TheSingularityWorkshop.FSM_NeuralNetworks/issues/3) | **No direct runtime dependency indicated** | Possible model/training configuration envelope | NeuralNetworks retains architecture, tensor semantics, training/inference, GPU execution, and performance. GrammarAi is not a tensor/model validator. |
| [SingularityBot #12](https://github.com/TrentBest/TheSingularityWorkshop.SingularityBot/issues/12) | **Conditional; prefer CommandAi/ProtocolAi integration first** | Bot capability or interaction-plan declaration shared across hosts/agents | SingularityBot retains strategy and agent behavior. CommandAi owns atomic commands; host retains authorization and execution. Do not duplicate its existing strategy model. |

## Existing coordination state

The 27 consumer issues above were created before this audit and were all verified open on 2026-11-14. They are the correct places for repository owners to respond; do not create duplicate issues.

The coordination register also records existing work that should be connected rather than reimplemented:

- CiAi #59 is the active implementation tracker for the typed workflow-plan integration.
- GrammarAi #14 tracks the observer-command/gate-predicate consumer contract proof; the remaining test work is in CiAi, not GrammarAi.
- CommandAi #10 coordinates the optional composition boundary; CommandAi's existing proposal envelope remains its own contract.
- StoryBuilder #8 is the most useful next independent domain pilot because it names real existing domain models.
- The other consumer issues should be triaged into adopt, adapter-only, document-only, or no current use based on actual source-backed examples and tests.

## Repositories not yet individually source-audited

The connected 27-repository register is the current Workshop adoption scope. The authenticated account also exposes historical, external, and utility repositories (including older FSM, BIM/Revit, Unity, bot, and demo projects). Their repository names were enumerated, but they have not all been inspected at source level in this pass. Do not infer that they need GrammarAi. Review those repositories only when they contain an active AI-facing or cross-repository structural contract.

## Recommended sequence

1. Complete the CiAi proof already underway without moving evidence or authorization ownership into GrammarAi.
2. Use StoryBuilder #8 as the next independent, real-domain pilot.
3. Resolve CommandAi #10 against its existing atomic proposal and codec contracts.
4. Confirm whether FSM_UserIO #12 has a shared interaction contract with an actual consumer.
5. Treat Experiences, FSM_COS, Foundry, and MicroBundleDomain as authoring/adapter opportunities first; require real manifest data before adding dependencies.
6. Close out the remaining consumer issues with an explicit disposition. Prefer **no current use** over speculative package references.

## Verification checklist for every adoption

- Pin the exact GrammarAi revision or package version.
- Map the consumer's real domain object—not a test-only lookalike.
- Identify every ProtocolAi category/member identity and qualify external references.
- Bind resolution to the exact request/catalog/capability snapshot.
- Test valid structures and malformed, unknown, disabled, stale, wrong-category, and over-limit cases as applicable.
- Keep semantic/domain validation in the consumer.
- Prove structural validity cannot authorize, execute, grade, verify evidence, or assert domain truth.
- Record the exact consumer commit and test/CI run.
- Do not merge, publish, release, deploy, or delete branches without the required separate authorization.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
