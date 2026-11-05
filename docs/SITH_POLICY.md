# Sith Policy

The Workshop's permanent-branch policy is intentionally simple and exact:

- `master` — release-matching branch
- `development` — active development and dependency source

A repository is **COMPLIANT** only when its complete GitHub branch inventory contains exactly those two branch names. Protected and unprotected branches both count. No additional permanent branches are permitted.

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

The SVG includes animation as decoration only; its text and color communicate the status without motion. Reduced-motion preferences disable animation where supported.

## Scope

This first implementation verifies branch names and count only. It does not silently impose additional rules about branch protection, merge strategy, release tags, or publishing. NuGet publishing remains separately gated and requires explicit authorization.
