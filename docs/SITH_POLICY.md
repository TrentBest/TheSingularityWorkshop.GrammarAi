# Sith Policy

The Workshop's permanent-branch policy is intentionally simple and exact:

- `master` — release-matching branch
- `development` — active development and dependency source

A repository is **COMPLIANT** only when its complete GitHub branch inventory contains exactly those two branch names. Protected and unprotected branches both count. No additional branches are permitted.

## The living badge

The badge tells a tiny visual story:

- **COMPLIANT:** the Sith Lord and apprentice appear in their training ground, locked in an endless lightsaber duel. Neither wins; the training never ends.
- **NON-COMPLIANT:** the training ground is empty. The duelists are absent because the repository has violated the policy.
- **UNVERIFIED:** the training ground is veiled/obscured. We do not show the duel when the branch inventory could not be verified.

The visual state is generated from the same evaluation that writes the machine-readable evidence. The animation is decorative; the status text remains explicit, and reduced-motion preferences disable movement where supported.

## Evaluator behavior

The `Sith Policy` GitHub Actions workflow:

1. Reads the full branch list using GitHub API pagination.
2. Compares the sorted branch inventory against the exact required set.
3. Writes `assets/sith-policy.json` as machine-readable evidence.
4. Renders `assets/sith-policy.svg` as the living README badge.
5. Fails the workflow unless the result is `COMPLIANT`.

The workflow runs after pushes to any branch, on a daily reconciliation schedule, and on manual dispatch. The badge is updated on the repository's default branch. API failures are reported as `UNVERIFIED`, never as compliant.

## Status meanings

- `COMPLIANT`: exactly `master` and `development` exist.
- `NON-COMPLIANT`: the inventory is readable but does not exactly match the required set.
- `UNVERIFIED`: GitHub could not provide a complete inventory.

## Scope

This first implementation verifies branch names and count only. It does not silently impose additional rules about branch protection, merge strategy, release tags, or publishing. NuGet publishing remains separately gated and requires explicit authorization.
