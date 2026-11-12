# Sith Policy

The Workshop's permanent-branch policy is intentionally exact:

- `master` — last released / NuGet-aligned baseline
- `development` — active work and dependency-build source

A repository is **COMPLIANT** only when its complete GitHub branch inventory contains exactly those two branch names. Protected and unprotected branches both count. No additional branches are permitted.

## The living badge

The badge tells a small visual story:

- **COMPLIANT:** the Sith Lord and apprentice appear in their training ground, locked in an endless lightsaber duel. Neither wins; the training never ends.
- **NON-COMPLIANT:** the training ground is empty.
- **UNVERIFIED:** the training ground is veiled because the branch inventory could not be verified.

The visual state is generated from the same evaluation that writes machine-readable evidence. The animation is decorative; status text remains explicit, and reduced-motion preferences disable movement where supported.

## Evaluator behavior

The `Sith Policy` GitHub Actions workflow:

1. Reads the full branch list using GitHub API pagination.
2. Compares the sorted branch inventory against the exact required set.
3. Writes `assets/sith-policy.json` as machine-readable evidence.
4. Renders `assets/sith-policy.svg` as the living README badge.
5. Fails the workflow unless the result is `COMPLIANT`.

The workflow runs after pushes to branches, on a daily reconciliation schedule, and on manual dispatch. For a push, it checks out and updates the branch receiving that push; a deletion event evaluates from the repository default branch. Scheduled and manual runs use the default branch. This branch-aware behavior prevents a development evaluation from attempting to write its evidence to master. API failures are reported as `UNVERIFIED`, never as compliant.

## Status meanings

- `COMPLIANT`: exactly `master` and `development` exist.
- `NON-COMPLIANT`: the inventory is readable but does not exactly match the required set.
- `UNVERIFIED`: GitHub could not provide a complete inventory.

## Scope and release boundary

This evaluator verifies branch names and count. It does not silently impose rules about branch protection, merge strategy, release tags, or publishing. Build, test, and pack may run independently. NuGet publication remains separately gated and requires explicit owner authorization; Sith compliance alone is not release authorization.
