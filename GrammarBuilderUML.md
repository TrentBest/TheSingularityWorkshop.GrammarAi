<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# GrammarBuilder — UML and use cases

## Class relationships

The authoring consumer is above the focal class. The builder accumulates rules and produces a validated immutable definition.

```mermaid
classDiagram
    direction TB
    class GrammarAuthor {
      <<consumer>>
      +DeclareRules()
      +BuildDefinition()
    }
    class GrammarBuilder {
      +Id : ulong
      +Name : string
      +StartSymbol : ulong
      +Rule(ruleId, leftHandSide, rightHandSide) GrammarBuilder
      +Build() GrammarDefinition
    }
    class GrammarRule {
      +Id : ulong
      +LeftHandSide : ulong
      +RightHandSide : IReadOnlyList~GrammarSymbol~
    }
    class GrammarDefinition {
      +Id : ulong
      +Name : string
      +StartSymbol : ulong
      +Rules : IReadOnlyList~GrammarRule~
      +Describe() string
    }
    GrammarAuthor ..> GrammarBuilder : authors with
    GrammarBuilder ..> GrammarRule : accumulates
    GrammarBuilder ..> GrammarDefinition : creates
```

## Use cases

### Declare productions
Call `Rule(ruleId, leftHandSide, rightHandSide)`. The builder appends a new `GrammarRule` and returns itself for chaining. Duplicate rule IDs are rejected when the complete definition is built, rather than by the builder's `Rule` method.

### Build and continue authoring
Call `Build()` to obtain an immutable snapshot. The definition is independent of later additions to the builder's rule list.

### Understand validation boundaries
The builder checks basic grammar metadata at construction and delegates whole-grammar structural validation to `GrammarDefinition`. It does not resolve external protocol references, parse text, or authorize actions.

See [`GrammarBuilder.md`](GrammarBuilder.md) for the class card and detailed responsibility boundary.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
