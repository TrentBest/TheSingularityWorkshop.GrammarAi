# GrammarAI Release Readiness

## Branch reconciliation

See [BRANCH_RECONCILIATION.md](BRANCH_RECONCILIATION.md) for the evidence-backed `master`/`development` and feature-branch preservation checklist. It is an audit record, not permission to merge, close PRs, or delete branches.

## Current package status

The project on `development` declares **`0.1.0-alpha.2`** and targets **.NET 8 (`net8.0`)**. This checklist tracks evidence for a future package release; it does not authorize publication or change the package version.

GrammarAI is the structural layer: it composes integer-backed grammar rules and references protocol-owned terminal identities. It does not own the vocabulary, call a model, interpret application meaning, or execute commands.

## Latest verified candidate — 2026-10-22 UTC

Candidate `development` commit: `91de4d93825c9323edc9085cfcaee967ddb75381`. [Workflow run #39716994290](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/actions/runs/39716994290) passed for this exact commit, including restore, Release build, tests/coverage, package creation, documentation-path verification, and clean consumer smoke test. NuGet publication was skipped and remains hard-disabled. The AI Exchange document and SVG are explicitly labelled as design direction, separate from the implemented grammar-definition API. This status update is documentation-only and its resulting commit must also pass CI.

## Verified foundation

- Grammar definitions validate nonzero identities, unique rule IDs, the start symbol, and referenced nonterminals.
- Recursive productions are allowed as structure; the grammar does not attempt to prove termination.
- Empty right-hand sides are intentionally supported and described as epsilon (`ε`).
- Built definitions and rule right-hand sides take defensive snapshots and expose read-only collections.
- `Describe()` has deterministic output covered by tests.
- Tests cover external protocol terminals, missing nonterminal productions, duplicate rule IDs, builder snapshot immutability, recursive rules, deterministic descriptions, default/uninitialized symbols, and epsilon.
- The README and consuming, examples, theory, and reflection guides describe the WHAT/HOW/host boundary.
- Public API XML documentation was expanded and the `CS1591` suppression removed on `development`; the workflow for commit `2eb99d09f9ba6f1020d12e888b1226a8fbf6b6af` passed.
- CI builds, tests, packs, verifies package documentation paths, and runs a clean consumer smoke test. NuGet publication was skipped.

These checks are evidence of a sound foundation, not a completed release audit.

## Before the next package release

### API and correctness

- [ ] Review every public API for argument validation, exception behavior, XML documentation, and naming consistency.
- [ ] Confirm malformed/default values and protocol-reference edge cases are tested and intentional.
- [ ] Verify all exposed collections remain immutable from a consumer's perspective.
- [ ] Confirm examples compile against the exact public API and intended package version.
- [ ] Review recursive and epsilon grammar semantics; do not remove epsilon support as an accidental validation change.
- [ ] Decide whether additional XML documentation warnings remain and address them without suppressing the warning globally.

### Package and consumer experience

- [ ] Confirm whether .NET 8 only is the intended target-framework support policy.
- [ ] Inspect the generated `.nupkg` and `.snupkg` (if produced), including README, license, XML documentation, and the intended documentation/image assets.
- [ ] Verify package metadata, release notes, and installation instructions against the release being prepared.
- [ ] Verify the published NuGet version and package page before making claims about distribution status.
- [ ] Verify NuGet Trusted Publishing configuration without changing it as part of this audit.
- [x] Publication is hard-disabled in the workflow pending explicit owner authorization.

### Branch and review hygiene

- [ ] Reconcile open pull requests and preserve unique documentation before recommending branch cleanup.
- [ ] Keep only `master` and `development` after the user has reviewed the cleanup candidates; do not delete branches automatically.
- [x] The current candidate passed CI and the consumer smoke test in workflow run #39716621201; the final release candidate must be rechecked after remaining changes.

## Release recommendation

**GrammarAI remains pre-release until the remaining checks are evidenced.** The latest verified CI run is green, including the change that enables missing-public-XML-documentation warnings. This does not establish package compatibility, framework policy, NuGet trust configuration, or readiness to remove the alpha label.

1. Complete the API and package audits.
2. Reconcile open PRs and stale branches without losing unique work.
3. Decide the supported framework and compatibility contract.
4. Prepare and review the intended package artifact.
5. Publish only after explicit authorization.
