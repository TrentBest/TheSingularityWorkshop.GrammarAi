<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# GrammarLimits

## Purpose

Limits applied to grammar rule collections and ordered production symbols.

## Declaration and ownership

- **Declaration:** public static class `GrammarLimits`.
- **Source:** [GrammarLimits.cs](GrammarLimits.cs).
- **Responsibility:** Centralizes explicit, testable representation limits so callers can reason about bounded input.
- **Non-goals:** These constants do not establish semantic validity, authorization, or execution safety.

## Contract and use

The public constants are enforced by the owning constructors/builders. Treat them as part of the current representation contract; do not silently bypass them in host adapters.

## Verification

See the package test project for boundary and over-limit regression cases.

## Related documentation

- [Companion UML and use cases](./GrammarLimitsUML.md)

- [Problem domain](docs/PROBLEM_DOMAIN.md)
- [Documentation index](DOCUMENTATION_INDEX.md)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
