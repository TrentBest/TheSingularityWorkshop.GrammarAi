# Branch Reconciliation Ledger

**Review date:** 2026-10-22 UTC  
**Scope:** `master` versus `development` and the extant feature/documentation branches  
**Disposition:** Audit only. No merge, cherry-pick, reset, PR closure, or branch deletion has been performed.

## Current branch evidence

- `development`: `ab1b830ecb9b209bea5a002acb866aa607c60874`
- `master`: `e7bc7bc451fc39bde764fa3cc428d8e0f314258e`
- GitHub comparison: `development` is 53 commits ahead and 4 commits behind `master`.
- Latest CI for the current development head: [run #39717089392](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/actions/runs/39717089392) — passed.

## Work found only on master

| Commit | Change | Reconciliation treatment |
|---|---|---|
| `3be72d91f9c8` | Link ProtocolAi domain roadmap and examples | Preserve the cross-package relationship, but verify links and avoid implying GrammarAi owns ProtocolAi behavior. |
| `4d8988a8b1be` | Add shared Workshop moniker | Preserve the moniker and accessible image alternative in the final README. |
| `c6f6bec42de5` | Standardize README identity heading | Preserve the standard identity heading. |
| `e7bc7bc451fc` | Link shared Workshop documentation standard | Preserve the link, pointing at the canonical FSM_COS `master` documentation standard. |

The master README is 849 lines; development's README is 874 lines. The differences are not merely line-count changes: inspect content before deciding what to carry forward.

## Feature branches and open pull requests

These branches remain intact:

- `chore/standardize-ci-grammar-ai` — open draft PR [#6](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/pull/6).
- `docs/grammar-ai-progressive-readme` — existing README alternative; compare before retiring.
- `docs/readme-standard` — open PR [#7](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/pull/7).
- `feature/ai-exchange-foundation` — open PR [#1](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/pull/1). Unique AI Exchange design work has been preserved in `docs/AI_EXCHANGE.md` and `docs/images/ai-exchange-stack.svg` on development, with an explicit statement that orchestration, provider adapters, and response parsing are not shipped GrammarAi APIs.
- `fix/restore-nuget-trusted-publisher` — review the history for any still-useful workflow fix before disposition.
- `release/alpha2-ci-theory` — review unique theory/release notes before disposition.

No PR has been merged or closed as part of this audit.

## Release-critical differences

- Both branches declare `0.1.0-alpha.2` and target .NET 8.
- Development's workflow pins NuGet Trusted Publishing to an immutable commit SHA; master uses a mutable version tag. Preserve the safer development workflow.
- Both workflows currently hard-disable NuGet publication.
- Development has no CS1591 suppression in the project file; master does. Treat resulting XML-documentation warnings as work to resolve rather than restoring the suppression without review.
- The AI Exchange diagram is design direction, not runtime evidence or a current API diagram.

## Required before declaring branch reconciliation complete

- [ ] Review master-only README material and preserve verified examples/cross-links.
- [ ] Inspect every listed feature branch for unique source, tests, docs, or workflow improvements.
- [ ] Review PRs #6, #7, and #1 without merging them as part of this audit.
- [ ] Verify grammar invariants, protocol-reference semantics, package contents, and failure cases against tests.
- [ ] Re-run CI on the exact reconciled candidate.
- [ ] Obtain repository-owner approval before merging or deleting any branch.

**This ledger is not authorization to delete any branch or close any PR.**
