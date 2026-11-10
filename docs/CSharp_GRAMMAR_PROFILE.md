# C# 11 grammar profile and compiler corpus

## Declared baseline

The bounded C# grammar resources in GrammarAi declare **C# 11** as their current language baseline through `CSharpGrammarProfile.WorkshopBaseline`. This is separate from package version, GrammarDefinition identity, structural schema version, and proposal codec wire version.

## Feature provenance

| Feature | Introduced | Profile behavior |
|---|---:|---|
| Init-only property/indexer accessors | C# 9 | Supported by C# 9 and later |
| Checked user-defined unary/binary operators | C# 11 | Supported by C# 11 and later |

## Pinned compiler corpus

`CSharp11CompilerCorpusTests` is a **test-only** compiler-backed corpus pinned to Roslyn 4.11.0 and explicit language-version settings. Positive cases cover init-only accessors, checked unary/binary operator pairs, expression-bodied methods, optional parameters, indexers, custom events, and constructor initializers. Earlier-version cases prove init-only accessors are rejected by C# 8 and checked operators by C# 10. Negative C# 11 cases cover malformed indexers, wrong operator arity, incomplete custom events, and recursive constructor initializers.

The Roslyn dependency is confined to the test project, not the GrammarAi runtime or package.

**Evidence boundary:** this proves that representative snippets compile or fail under the pinned Microsoft compiler dialect. It does not prove GrammarAi parses source, that its structural maps cover all C# 11 syntax, or that a structurally valid proposal is semantically legal. GrammarAi remains a bounded structural mapping resource, not a source parser.

The profile is metadata; it does not filter or rewrite productions. Future language-version upgrades must update the profile, feature inventory, corpus, and source provenance together.
