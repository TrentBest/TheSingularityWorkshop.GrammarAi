<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Branch Reconciliation Ledger

**Review date:** 2026-11-12 UTC (refreshed 06:11 UTC)  
**Scope:** permanent branches and master-only history  
**Disposition:** audit and preservation only. No merge, reset, force-push, or branch deletion has been performed.

## Live branch evidence

- `master`: `5d8ffa0cc80d570380ecbab3b10e79a9f695dcba`
- `development`: `12e62e079e6dbaa4243aec68477c6583ea1f0583`
- Live branch inventory: exactly `master` and `development` (pagination exhausted; no further branches).
- Live open pull requests: zero.
- GitHub comparison from the merge base `0455d9d180b10b86bce5ab13b5aa6ee0a133b60a`: development is **461 commits ahead** and **29 commits behind** master.
- The branch count is compliant; the histories are **not reconciled**. The current branch inventory contains no noncanonical branch, so there is no surplus branch to mark READY FOR OWNER DELETION. The ahead/behind counts are history facts, not permission or evidence that merging is safe.

## Master-only history and net file differences

The 29 master-only commits were enumerated. Most are repeated Sith evidence refreshes; the distinct changes include the initial Sith evaluator/badge, README badge/documentation changes, shared Workshop identity/standard links, a ProtocolAi usage-map link, and an earlier C# expression grammar test. The branch comparison shows a small net set of file differences rather than 29 independent source changes.

The relevant content was inspected and dispositioned:

- `.github/workflows/sith-policy.yml`: the master version checks out the default branch for every push. The development version is the corrected branch-aware workflow: push events check out/update the pushed branch, delete events use the default branch, and schedule/manual runs use the default branch. Preserve the development implementation; do not copy the older master workflow over it.
- `README.md`: master has the earlier long-form README. Development has the newer 22-section guide and current API/domain links. The Sith badge has now been placed in the development README badge group, and the README's release description and footer alt text were corrected.
- `assets/sith-policy.json`: master evidence is stale (2026-11-11); development evidence is current for the successful 2026-11-12 evaluation at `658834f`. Do not copy stale evidence.
- `assets/sith-policy.svg`: the inspected master and development assets are identical.
- `docs/SITH_POLICY.md`: policy documentation has been carried into development and corrected to describe branch-aware evidence updates and the separate explicit NuGet authorization boundary.
- `tests/GrammarAi.Tests/CSharpExpressionGrammarTests.cs`: master has an older 20-production expression test; development has the current 39-production expression grammar tests covering postfix member access, invocation, and element access. Preserve the development tests; do not restore the older test file.

No master-only content discovered in this audit justifies merging or resetting either permanent branch. This is a content disposition, not a claim that the histories have been reconciled.

## Development work represented by the current line

- Integer-first production grammar and external ProtocolAi references.
- Typed structural node kinds, contextual slots, graph instances, cardinality/order validation, and bounded deterministic proposal encoding.
- External category slots with qualified identities, host-supplied resolution, tagged v2 tuples, and legacy v1 read compatibility.
- Observer-condition and gate-transition-intent schema, with evidence evaluation and authorization remaining host-owned.
- Bounded composable C# expression, type, metadata, declaration, and type-declaration grammar resources.
- Updated observer integration boundaries, explicit FSM_Foundry workflow ownership guidance, a fail-closed live Sith publish preflight, Sith policy documentation, and current branch reconciliation evidence.

## Verification

- Package GrammarAI run [#43166182280](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/actions/runs/43166182280) on commit `c0fd7bc833417da7ebfa830d6234c43c23cfc85e`: **success**. Build, tests with coverage, package creation, packaged-documentation checks, clean-consumer installation, and artifact uploads all passed. The NuGet publish job was **skipped**.
- Sith Policy run [#43166182284](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/actions/runs/43166182284) on the same commit: **success**. It refreshed branch evidence in the subsequent evidence-only commit `12e62e079e6dbaa4243aec68477c6583ea1f0583`.
- The package verification therefore covers the latest source/documentation commit; the only later change is the Sith evidence refresh. A superseded overlapping Sith run encountered a non-fast-forward push while several documentation commits were being written in quick succession; the final serialized run succeeded and refreshed evidence. No source changes were lost and no package was published.
- No package was published. No merge, release, deployment, or branch deletion occurred.

## Remaining work

- [ ] Review the full patch-level history of the 29 master-only commits if a future reconciliation/merge is proposed; this ledger records the enumerated commit subjects and the net content dispositions above.
- [x] Confirm the Foundry-boundary documentation and publish-preflight workflow on the exact latest code/documentation commit; Package GrammarAI and Sith Policy both passed, with only a subsequent Sith evidence refresh.
- [ ] Audit the complete public API and package contents before any future release candidate.
- [ ] Reconcile version/readme/release notes only after the public contract is stable.
- [ ] Obtain explicit repository-owner approval before merging, resetting, force-pushing, or deleting anything.

**Branch inventory is compliant (exactly `master` + `development`); there are no noncanonical branches and therefore no READY FOR OWNER DELETION entries. Branch history remains divergent (development +461 / master +29); preserve both heads and defer any reconciliation to an explicit owner decision. Foundry lifecycle ownership is explicit in the consuming and AI Exchange guides. The manual publish preflight verifies the live inventory and fresh matching Sith evidence; the NuGet publish job remains hard-disabled. Package verification passed and publication was skipped.**

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
