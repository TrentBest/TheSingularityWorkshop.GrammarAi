# GrammarAi — Release Review Brief

> Phone-first review | Snapshot: 2026-10-22 UTC

## The decision in one minute

GrammarAi is the **HOW / structure layer**. It composes grammar symbols and ordered productions, validating internal consistency. ProtocolAi owns the meanings and identities of terminal symbols; GrammarAi stores references to those identities but does not load ProtocolAi definitions or prove that references resolve. The consuming host must perform that cross-package link step.

**Current assessment: 70% ready.** This is a qualitative working estimate, not a measured score. The latest source-boundary update passed the build-and-test job; publication was skipped as intended. Package version remains 0.1.0-alpha.2 on .NET 8. This is not publication approval.

- Candidate commit: [e15f61f6cc6c7a747eb8383ecf2a00e07cd56fb0](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/commit/e15f61f6cc6c7a747eb8383ecf2a00e07cd56fb0)
- CI: [run #39726325238](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/actions/runs/39726325238)
- Build/test passed; NuGet publish was skipped.
- No version change, PR merge, or branch deletion is authorized by this documentation work.

## What is implemented

- Grammar identity, name, and start nonterminal.
- Integer-backed nonterminal identities and ordered production rules.
- Terminal symbols carrying an external protocol/symbol identity pair.
- Validation for zero identities, duplicate rule IDs, missing start production, and nonterminals without productions.
- Recursive productions and epsilon (empty right-hand side) as supported structure.
- Defensive snapshots/read-only collections and deterministic self-description.
- Quickstart project, tests, consumer guide, examples, theory, reflection, and explanatory SVGs.

## Boundary in one picture

```text
ProtocolAi
  owns WHAT a terminal identity means
       |
       | (protocol ID, symbol ID) reference
       v
GrammarAi
  owns HOW identities may be arranged into productions
       |
       v
Host / adapter
  resolves external references, validates meaning, chooses behavior
       |
       v
Execution (outside both packages)
```

GrammarAi's internal validation is not a grammar compiler, termination proof, parser, constrained decoder, model client, or workflow engine. Recursive structure is allowed; termination under a future interpreter is not guaranteed. Epsilon is intentional and must not be removed accidentally during hardening.

## Release-critical review

1. **Public API:** builder inputs, default/uninitialized values, exception contracts, XML docs, naming, and stable collection behavior.
2. **Grammar correctness:** unique rule IDs, start symbol, missing nonterminals, recursive productions, and epsilon semantics.
3. **External references:** distinguish stored identity pairs from references proven to exist in a specific ProtocolAi definition. Linking belongs to the consumer today.
4. **Consumer/package proof:** run quickstart, test a clean consumer project, inspect package contents, and verify README, license, XML docs, version, and metadata.
5. **Release policy:** confirm .NET 8 support, release notes, package instructions, and Trusted Publishing configuration without enabling publication.

## Five-minute review path

1. [README — WHAT/HOW/host boundary](../README.md)
2. [Consuming GrammarAi](CONSUMING.md)
3. [Runnable examples](EXAMPLES.md)
4. [Theory](THEORY.md)
5. [Implementation roadmap](IMPLEMENTATION_ROADMAP.md)
6. [Reflection and limitations](REFLECTION.md)
7. [Release readiness](RELEASE_READINESS.md)
8. [Gallery and visual evidence](GALLERY.md)

## Recommendation

Keep GrammarAi a structural artifact library. Do not add protocol loading, provider-specific export, model execution, or workflow behavior just to make the alpha appear larger. Finish the API/package audit, verify the external-link boundary in tests/examples, and publish only after explicit owner approval.
