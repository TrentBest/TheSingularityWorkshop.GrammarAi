# C# grammar profile

## Declared baseline

The bounded C# grammar resources in GrammarAi declare **C# 11** as their current language baseline through CSharpGrammarProfile.WorkshopBaseline.

This is separate from:
- GrammarAi package version;
- an individual GrammarDefinition identity;
- a structural schema version;
- the proposal codec wire version.

Those versions answer different compatibility questions and must not be conflated.

## Feature provenance represented by the profile

| Feature | Introduced | Profile behavior |
|---|---:|---|
| Init-only property/indexer accessors | C# 9 | Supported by C# 9 and later |
| Checked user-defined unary/binary operators | C# 11 | Supported by C# 11 and later |

The profile exposes Supports and RequireSupported so a host can check a selected feature against the declared language version before using a grammar slice.

## Deliberate boundary

The profile is **metadata**, not a compiler dialect implementation. It does not:
- filter or rewrite productions in CSharpDeclarationGrammar;
- tokenize or parse C# source;
- validate operator legality for particular types;
- establish complete conformance with the C# 11 specification;
- imply that the expression, type, or declaration slices cover every C# 11 construct.

The current maps remain bounded structural resources. The host owns compiler-grade validation, semantic analysis, authorization, and execution.

## Next conformance step

Before expanding the C# maps substantially, add a pinned positive/negative corpus tied to the C# 11 baseline. Each case should identify the grammar slice and expected structural acceptance/rejection. Any future language-version upgrade should update the profile, source provenance, feature inventory, and corpus together.
