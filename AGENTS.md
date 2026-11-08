<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# GrammarAi — repository instructions for AI agents

## Before changing code or documentation

1. Read the [FSM_COS Documentation Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/DOCUMENTATION_STANDARD.md). It is the normative Workshop standard; do not create a competing local standard.
2. Read this repository's README, documentation index, problem-domain and theory/architecture documents where present.
3. Inspect the current `development` head, open pull requests, relevant source, tests, and CI before making assumptions.
4. Read implementation and tests before changing public contracts. Trace documentation claims to the exact source and test evidence.
5. Keep this file aligned with the actual package boundary. If a behavior is proposed but not implemented, label it as intended or future work.

## Branch and release policy

The permanent branch model is exactly:
- `development` — active work and the next release candidate.
- `master` — the previously released baseline until the owner-authorized release operation replaces it.

Work directly on `development`. Do not create persistent feature or handoff branches. Do not delete branches or rewrite refs. Preserve and inspect existing branch/PR work; do not blindly merge stale snapshots. Do not publish to NuGet unless the owner explicitly authorizes that specific package/version release. Keep publication disabled by default in CI.

## Workshop documentation requirements

- Teach the reader from a recognizable problem to the concept, rationale, diagram/example, implementation, verification, limitations, and next reading.
- Provide end-user guidance and theory that explains why the boundary exists; do not substitute marketing copy for explanation.
- Keep README, quickstarts, type guides, UML/use cases, examples, tests, and source consistent.
- Inventory applicable documentation views and mark not-applicable views with a reason. Do not claim repository-wide generated UML, AST coverage, or runtime evidence unless implemented and verified.
- Identify diagrams as source-derived, human-authored, conceptual, or runtime-captured. Include scope and limitations.
- Mark examples **Verified**, **Source-checked**, or **Conceptual** according to actual evidence.
- Every edited Markdown document must retain the animated moniker at the top and the exact canonical Workshop PNG footer:
  `https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png`
- Never substitute Avatar.jpg, a personal avatar, or an SVG moniker for the footer.

## GrammarAi-specific boundary

GrammarAi is an AI-facing knowledge-mapping layer, not a compiler project. ProtocolAi owns stable vocabulary identity; GrammarAi owns typed structural relationships, grammar nodes, roles/slots, production composition, and structural constraints that let an AI reference known concepts compactly. The host owns identifier allocation/scope, active context, interpretation, policy, authorization, and execution.

ProtocolAi owns stable vocabulary identity, including the opaque protocol/category and member identities referenced by GrammarAi. GrammarAi does not depend on ProtocolAi or duplicate its metadata: it stores qualified external references and lets the host resolve membership and exposure against the exact catalog/capability snapshot used for the request. A GrammarSlotDefinition's contextual role (for example, Condition, Body, Repository, or Trigger) is GrammarAi-owned and is not the global category identity.

Do not make compiler parsing, code generation, Roslyn replacement, or source tokenization the primary definition of GrammarAi. Compiler/parser integrations may be optional adapters. A valid grammar-shaped proposal does not establish semantic correctness, safety, authorization, or permission to execute.

## Cross-package and trust boundaries

- ProtocolAi owns application-defined vocabulary and symbol identity.
- GrammarAi owns grammar structure and composition over externally owned protocol references. External category/member references are opaque; the host-supplied resolver verifies membership and exposure against the request's exact catalog/capability snapshot.
- CommandAi owns versioned command-proposal representation and its codec.
- HeadlessAi owns configured AI endpoint request/response communication.
- The consuming host owns interpretation, trust decisions, policy, authorization, approval, memory/state, and execution.

Do not add sibling package dependencies merely because capabilities compose. Verify the actual public contracts and dependency direction first. Never imply that a valid identity, grammar, proposal, or model response grants authority or executes an action.

## Verification and reporting

- Run the narrowest relevant tests first, then the full repository verification workflow when practical.
- Report exact commit, commands/workflows, pass/fail outcomes, and remaining coverage gaps.
- A build is not proof of release readiness; a green badge is not an explanation of test scope.
- Do not claim live-provider interoperability unless it was actually exercised with the relevant provider and safe test credentials.
- CiAi is outside the scope of these repositories unless the owner explicitly expands the task.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
