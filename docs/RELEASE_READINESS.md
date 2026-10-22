<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# GrammarAI Release Readiness

## Branch reconciliation

See [BRANCH_RECONCILIATION.md](BRANCH_RECONCILIATION.md) for the evidence-backed `master`/`development` and feature-branch preservation checklist. It is an audit record, not permission to merge, close PRs, or delete branches.

## Current package status

The project on `development` declares **`0.1.0-alpha.2`** and targets **.NET 8 (`net8.0`)**. This checklist tracks evidence for a future package release; it does not authorize publication or change the package version.

GrammarAI is the structural layer: it composes integer-backed grammar rules and references protocol-owned terminal identities. It does not own the vocabulary, call a model, interpret application meaning, or execute commands.

## Latest verified CI baseline — 2026-10-22 UTC

Latest green `development` commit: `2e8c18bcbb0dbae5e44d6640cdebef02f55a8556`. [Workflow run #39723634834](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/actions/runs/39723634834) passed for this exact commit, including restore, Release build, tests/coverage, package creation, packaged documentation-path verification, and a clean consumer smoke test. NuGet publication was skipped and remains hard-disabled. The AI Exchange document and SVG are explicitly labelled as design direction, separate from the implemented grammar-definition API.

This is the latest verified green baseline, not publication approval. This readiness-document update must itself pass CI before its new head is treated as verified.

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

---

<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://github.com/TrentBest.png?size=200" alt="The Singularity Workshop on GitHub" width="200">
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
