# C# 11 grammar profile and compiler corpus

## Declared baseline

The bounded C# grammar resources in GrammarAi declare **C# 11** as their current language baseline through `CSharpGrammarProfile.WorkshopBaseline`. This is separate from package version, GrammarDefinition identity, structural schema version, and proposal codec wire version.

## Feature provenance

| Feature | Introduced | Profile behavior |
|---|---:|---|
| Init-only property/indexer accessors | C# 9 | Supported by C# 9 and later |
| Checked user-defined unary/binary operators | C# 11 | Supported by C# 11 and later |
| Unmanaged generic constraint | C# 7.3 | Represented in the metadata slice |
| Not-null generic constraint | C# 8 | Represented in the metadata slice |
| Default generic constraint | C# 9 | Represented in the metadata slice |

## Bounded structural slices

The declaration grammar now has a separate composable metadata grammar, `CSharpMetadataGrammar`, for:

- Attribute sections, optional target specifiers, comma-separated attributes, qualified attribute names, and optional argument lists.
- Generic parameter lists with optional `in`/`out` variance markers, and constraint clauses.
- Primary constraint families represented here: `class`, `class?`, `struct`, `unmanaged`, `notnull`, and `default`; type constraints and `new()` are separate alternatives.

The metadata grammar deliberately exposes attributes, generic parameter lists, and constraint clauses as independent roots. Generic parameter syntax includes optional variance markers; the host must restrict variance to legal declaration contexts (such as interfaces and delegates) and must not treat the grammar fragment as a declaration-kind validator. A host composes them into the correct declaration slots, supplies attribute arguments and type references as opaque structures, and handles declaration-specific placement. The structural map does not resolve attribute names or type parameters, enforce constraint ordering/satisfiability, or decide whether a particular constraint is legal for a particular declaration.

## Pinned compiler corpus

`CSharp11CompilerCorpusTests` is a **test-only** compiler-backed corpus pinned to Roslyn 4.11.0 and explicit language-version settings. Positive cases cover init-only accessors, checked unary/binary operator pairs, expression-bodied methods, optional parameters, indexers, custom events, constructor initializers, attributes, and representative generic constraints. Earlier-version cases prove init-only accessors are rejected by C# 8 and checked operators by C# 10. Negative C# 11 cases cover malformed indexers and attributes, wrong operator arity, incomplete custom events, and recursive constructor initializers.

The Roslyn dependency is confined to the test project, not the GrammarAi runtime or package.

**Evidence boundary:** this proves that representative snippets compile or fail under the pinned Microsoft compiler dialect. It does not prove GrammarAi parses source, that its structural maps cover all C# 11 syntax, or that a structurally valid proposal is semantically legal. GrammarAi remains a bounded structural mapping resource, not a source parser.

The profile is metadata; it does not filter or rewrite productions. Future language-version upgrades must update the profile, feature inventory, corpus, and source provenance together.
