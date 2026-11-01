<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# GrammarLimits — UML and use cases

## Class neighborhood

```mermaid
classDiagram
    class GrammarLimits {
      <<static>>
      +const resource limits
    }
```

## Use case

1. Construct a definition or payload through the public API.
2. The owning type checks collection and text limits before retaining the input.
3. The caller receives a clear argument or invalid-operation error when a bound is exceeded.

## Boundary

The limits bound representation size. They do not validate application meaning, provider behavior, permissions, or downstream execution.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
