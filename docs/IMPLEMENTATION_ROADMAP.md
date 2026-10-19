# GrammarAi — Implementation and Documentation Roadmap

Purpose: living, evidence-based checklist for bringing implementation, tests, examples, and documentation to Workshop standard.

Status discipline: checked means verified in source/tests or by reproducible command. Design intention is not implementation evidence. Do not publish merely because a checklist section is complete.

## Release readiness
- **Current readiness:** 35% — preliminary estimate from README/theory review only; source, tests, CI, package metadata, and examples still need a complete audit.
- **Release-ready means:** coherent structural contract, tested validation, buildable examples, source-aligned docs, verified CI/package metadata, and explicit limitations.
- Publication authorization is separate from readiness.

## 00 — Definition of done
- [ ] Audit source tree, public API, tests, examples, project files, workflows, package metadata, and current NuGet artifact.
- [ ] Map every public type/member to purpose, invariants, validation, exceptions, and tests.
- [ ] Mark roadmap items implemented, missing, intentionally out of scope, or awaiting a decision.
- [ ] Ensure README, theory, SVGs, and examples agree with source.
- [ ] Label examples verified, source-shaped, or conceptual.
- [ ] Run restore, Release build, tests, coverage, pack, and quickstart.
- [ ] Verify links, versions, badges, license, and branch references.
- [ ] Keep master and development as the only maintained branches.
- [ ] Preserve boundary: GrammarAi describes structure; it does not execute it or own external vocabularies.

## 01 — Product contract and conceptual model
- [ ] Define GrammarAi as a provider-neutral, integer-addressed structural grammar over application-owned symbols.
- [ ] Explain WHAT/HOW/ACTION: ProtocolAi owns identity; GrammarAi owns relationships; host/adapter owns policy and execution.
- [ ] Define grammar, grammar ID, start symbol, nonterminal, terminal, protocol reference, production, rule ID, sequence, validation, and description.
- [ ] State that grammar owns its rules/nonterminals and references external identities without redefining them.
- [ ] Explain current formal model precisely; do not claim parsing, compilation, constrained decoding, or provider export unless implemented.
- [ ] Distinguish structural validity from semantic correctness, authorization, and execution permission.
- [ ] Maintain non-goals: inference, provider clients, prompt transport, parsers, execution engines, GUI rendering, and provider-specific formats.

## 02 — Grammar definition and identity
- [ ] Audit GrammarBuilder, grammar definition types, GrammarSymbol, GrammarProtocolReference, rule types, constructors, and public methods.
- [ ] Specify valid grammar IDs, names, start IDs, nonterminal IDs, rule IDs, and duplicate handling.
- [ ] Specify whether zero/negative IDs are legal and test the policy.
- [ ] Define whether rule order is significant and keep build, validation, description, and future serialization consistent.
- [ ] Verify built grammars are immutable and builder mutation cannot change them.
- [ ] Define equality semantics for grammars, symbols, rules, and external references.
- [ ] Specify deterministic ordering and formatting independent of hash enumeration.
- [ ] Document build-time failures and actionable diagnostics.

## 03 — Production rules and symbol semantics
- [ ] Define the exact supported right-hand-side model: terminal, nonterminal, and whether rules support one symbol or sequences.
- [ ] Document any single-symbol limitation rather than implying arbitrary context-free productions.
- [ ] Decide how epsilon productions are represented; test or mark unsupported.
- [ ] Decide whether recursive and mutually recursive productions are supported.
- [ ] Decide whether nonterminals may have multiple productions and define alternative ordering.
- [ ] Validate that start symbol has a production.
- [ ] Validate that every internal nonterminal reference has a production.
- [ ] Define treatment of unreachable nonterminals/rules: allowed, warning, or validation error.
- [ ] Define duplicate rule IDs, duplicate productions, and duplicate alternatives.
- [ ] Ensure terminal and nonterminal variants cannot be confused or formed into invalid states.
- [ ] Define how independent protocols may be referenced and how duplicate external references are treated.

## 04 — External protocol references and integration
- [ ] Audit GrammarProtocolReference(protocolId, symbolId) and GrammarSymbol.Terminal(...) paths.
- [ ] Confirm references do not require GrammarAi to depend on ProtocolAi's implementation assembly.
- [ ] Document that a reference identifies an external symbol but does not prove the symbol exists in a local definition.
- [ ] Decide what validation is local and what requires a host-supplied registry/resolver.
- [ ] Add cross-package integration tests where the public contract permits.
- [ ] Test malformed, duplicate, and cross-protocol references.
- [ ] Keep ownership clear when combining independent protocols.
- [ ] Verify AnyApp/ecosystem examples against actual source and branch paths.
- [ ] Maintain the [cross-repository usage map](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/ECOSYSTEM_USAGE_MAP.md): distinguish direct consumers, composition examples, carrier boundaries, legacy copies, and candidates.
- [ ] Avoid upward dependencies on FSM_COS, AnyApp, providers, or UI projects.

## 05 — Structural validation
- [ ] Inventory Build and validation behavior from source; distinguish construction checks from explicit validation.
- [ ] Define invalid conditions, diagnostics, ordering, and fail-fast versus aggregate behavior.
- [ ] Test missing start productions, missing nonterminal productions, invalid IDs, duplicate IDs, malformed symbols, and empty grammars.
- [ ] Decide whether unreachable rules, recursive cycles, duplicate alternatives, and unused declarations are errors, warnings, or allowed.
- [ ] Use cycle-safe graph traversal if validation/inspection walks recursive relationships.
- [ ] Ensure validation terminates predictably for large or adversarial input.
- [ ] Separate structural validity from external protocol resolution.
- [ ] Ensure validation is deterministic and non-mutating.
- [ ] Document exactly what successful validation guarantees and does not guarantee.

## 06 — Self-description, comparison, and lifecycle
- [ ] Audit Describe() and determine whether output is diagnostic prose or stable machine-readable contract.
- [ ] Test stable ordering and formatting if deterministic description is promised.
- [ ] Document escaping of names/values so output cannot become ambiguous.
- [ ] Decide whether canonical representation/fingerprint is needed for equality, caching, comparison, or serialization.
- [ ] Do not treat Describe() as a wire format without an explicit tested contract.
- [ ] Decide whether grammar diff/comparison belongs in core or separate tooling.
- [ ] Document artifact lifecycle: build, inspect, validate, hand off; no hidden execution.
- [ ] Ensure visualization consumes the same grammar artifact rather than a divergent decorative model.

## 07 — Robustness and security
- [ ] Treat imported/generated grammars as untrusted data.
- [ ] Ensure references never trigger actions or grant authorization.
- [ ] Test huge rule sets, deep nesting, recursive cycles, duplicate-heavy data, and pathological names.
- [ ] Check integer overflow and recursive traversal stack risks.
- [ ] Confirm no credentials, network transport, dynamic execution, or hidden global mutable state.
- [ ] Document thread-safety only where source and tests establish it.
- [ ] Add property/fuzz-style tests for rule graphs and symbol combinations where practical.
- [ ] Record realistic limits/complexity where known; do not invent performance guarantees.

## 08 — Testing strategy
- [ ] Map every public API member and validation invariant to tests.
- [ ] Cover simple terminal production, nested nonterminals, alternatives, and multi-protocol references.
- [ ] Cover missing start symbols/productions, duplicate IDs, malformed IDs, and invalid symbol variants.
- [ ] Cover recursion and cycles according to the chosen policy.
- [ ] Test deterministic Describe() output and rule ordering.
- [ ] Test immutability and builder isolation.
- [ ] Test external-reference behavior separately from registry-level resolution.
- [ ] Ensure tests do not depend on order or machine-local settings.
- [ ] Verify coverage targets production assembly and meaningful public behavior.
- [ ] Run Release tests and record actual results.

## 09 — Examples and living demonstrations
- [ ] Verify QuickStart against source and package version.
- [ ] Provide a first-minute example building and describing a real grammar with expected output.
- [ ] Demonstrate one terminal, nested nonterminal, alternatives, independent protocols, and invalid diagnostics where supported.
- [ ] Label unsupported sequence/epsilon/recursive examples as conceptual.
- [ ] Create source-aligned SVG showing protocol-owned identity vs grammar-owned rules.
- [ ] Add lifecycle diagram: define → build → validate → describe → hand off.
- [ ] Keep assets version-controlled, accessible, legible in dark/light themes, and captioned.
- [ ] Consider a small generated or fixture-backed visualization to reduce documentation drift.
- [ ] Add troubleshooting for real validation errors and boundary misunderstandings.

## 10 — Documentation set
- [ ] Align README with Workshop sequence: ✳️ 00 identity, money-shot visual, 🟦 01 What, 🟣 02 Why, 🩵 03 How, 🟢 04 See it in a minute, 🟪 05 docs/theory.
- [ ] Keep README a concise map; move deep detail to focused documents.
- [ ] Audit CONSUMING.md, EXAMPLES.md, THEORY.md, and REFLECTION.md for accuracy, duplication, and missing topics.
- [ ] Add/maintain architecture and API/contract references with ownership and validation semantics.
- [ ] Add development/verification guide and reader-oriented documentation index.
- [ ] Explain the concept to non-coders using a concrete example of identities and allowed arrangements.
- [ ] State limits: a grammar cannot guarantee correct intent, authorization, or safe execution.
- [ ] Separate current source, released package, intended architecture, and future work.
- [ ] Verify links, snippets, commands, package version, SVGs, alt text, badges, and footer.

## 11 — Package, CI, and release gate
- [ ] Verify frameworks, package ID/version, dependencies, nullable settings, analyzers, XML docs, license, metadata, and packed README.
- [ ] Verify development dependency/artifact behavior and release dependency behavior.
- [ ] Verify branch triggers, workflow permissions, coverage, trusted publishing, and publish gates.
- [ ] Prove development CI cannot accidentally publish.
- [ ] Run restore, Release build, tests, coverage, pack, and package inspection.
- [ ] Test packed package in a clean consumer project.
- [ ] Distinguish published package version from source version.
- [ ] Verify release notes explain contract changes and limitations.
- [ ] Mark 100% only with evidence for every release criterion; publication remains a separate action.

## 12 — Decisions to resolve, not silently assume
- [ ] Is the formalism a small grammar graph or a fuller context-free grammar?
- [ ] Are right-hand sides sequences, single symbols, or both?
- [ ] Are epsilon productions, recursion, and mutual recursion required?
- [ ] Should unreachable declarations and duplicate alternatives warn or fail?
- [ ] Should external references be validated by an optional registry interface?
- [ ] What should canonical serialization/versioning look like, and which package owns it?
- [ ] Should provider exporters live in independent adapter packages?
- [ ] Which structural guarantees must hold before a host accepts model output?
- [ ] What minimum contract does AnyApp need to demonstrate semantic exchange without moving execution into GrammarAi?

## Progress log
| Date | Change | Evidence | Readiness |
|---|---|---|---|
| 2026-10-18 | Initial roadmap created after reviewing README, theory, and shared documentation standard. Full source/test/CI audit remains outstanding. | [README](../README.md) · [Theory](THEORY.md) · [Documentation Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/master/DOCUMENTATION_STANDARD.md) | 35% |
| 2026-10-19 | Audited grammar construction, immutable rule snapshots, protocol-qualified terminals, recursion behavior, theory claims, tests, project metadata, and CI. Fixed push/PR workflow triggers to target only `master` and `development`; build/test execution and package inspection remain outstanding. | [GrammarDefinition.cs](../GrammarDefinition.cs) · [Grammar tests](../tests/GrammarAi.Tests/GrammarDefinitionTests.cs) · [CI workflow](../.github/workflows/build.yml) | 38% |
| 2026-10-19 | Cross-repository search confirmed AnyApp direct use, WebPage composition-level use, FSM_UserIO/FSM_COS carrier boundaries, and a separate legacy implementation in The. Added a prioritized candidate map without treating candidates as existing dependencies. | [ProtocolAi ecosystem usage map](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/ECOSYSTEM_USAGE_MAP.md) | 38% |
