<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# GrammarAi — Release Review Brief

> Phone-first review | Snapshot: 2026-11-20 UTC

## The decision in one minute

GrammarAi is the **HOW / structure layer**. It composes grammar symbols and ordered productions, validating internal consistency. ProtocolAi owns the meanings and identities of terminal symbols; GrammarAi stores references to those identities but does not load ProtocolAi definitions or prove that references resolve. The consuming host must perform that cross-package link step.

**Assessment at this snapshot: pre-release; no numeric readiness percentage is asserted.** The candidate is `0.1.0-alpha.3` targeting .NET 10 (`net10.0`). The already-published `0.1.0-alpha.2` package is the prior .NET 8 build and remains consumable by .NET 10 applications. Package workflow #44654565337 passed every build/test/coverage/pack/clean-consumer step on `eaebd301d1b417f5c8c6a241ffdbeb8ebacafb3b`; the current `development` HEAD `848aed468300df89d57b1b6a248fc3a1995a9d6e` differs only by the Sith evidence JSON refresh. There were no check-run/status contexts on that snapshot HEAD itself. The package publication job was skipped and remains hard-disabled. This is not publication approval.

- Current development HEAD: [848aed468300df89d57b1b6a248fc3a1995a9d6e](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/commit/848aed468300df89d57b1b6a248fc3a1995a9d6e)
- Package CI: [run #44654565337](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/actions/runs/44654565337) on the unchanged source/package tree at parent commit `eaebd301d1b417f5c8c6a241ffdbeb8ebacafb3b`.
- The only later commit refreshes `assets/sith-policy.json`; no source or package input changed. Current HEAD has no check-run/status contexts.
- Build, tests/coverage, Codecov, pack, package documentation, clean-consumer verification, and artifact uploads passed. NuGet publish was skipped and remains hard-disabled.
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
5. **Release policy:** verify the .NET 10-only source/package candidate, release notes, package instructions, and Trusted Publishing configuration without enabling publication.

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
---

<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200">
  </a>
</p>

<p align="center">
  <a href="https://github.com/TrentBest">GitHub</a> ·
  <a href="https://coderlegion.com/">Coder Legion</a> ·
  <a href="https://www.patreon.com/c/TheSingularityWorkshop">Patreon</a> ·
  <a href="https://www.paypal.com/donate/?hosted_button_id=3Z7263LCQMV9J">PayPal</a>
  <br>
  <a href="https://github.com/TrentBest/FSM_API">FSM_API</a> ·
  <a href="https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization">FSM_Serialization</a> ·
  <a href="https://github.com/TrentBest/WebPage">WebPage</a>
</p>

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
