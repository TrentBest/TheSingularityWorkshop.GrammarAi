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

The C# mapping is split into independently composable resources:

- `CSharpExpressionGrammar`: 39 productions for a bounded precedence/associativity hierarchy, including assignment, conditional/null-coalescing, logical/bitwise, equality/relational/shift/arithmetic, unary, and recursive postfix member/invocation/element access.
- `CSharpTypeGrammar`: 21 productions for qualified names, generic arguments, nullable forms, array ranks, and tuple types.
- `CSharpDeclarationGrammar`: 98 productions for selected member declarations. It includes methods, properties, fields, constructors and initializers, parameters/defaults, indexers, field-like/custom events, ordinary and checked unary/binary operators, implicit/explicit conversions, explicit-interface methods/properties/indexers/custom events, and block/expression-bodied finalizers.
- `CSharpMetadataGrammar`: 40 productions for attribute sections/targets, qualified attribute names and argument boundaries, generic parameter lists with optional `in`/`out` variance, and constraint families including `class`, `class?`, `struct`, `unmanaged`, `notnull`, `default`, type constraints, and `new()`.
- `CSharpTypeDeclarationGrammar`: 25 productions composing opaque host-owned attribute-section, generic-parameter-list, constraint-clause, base-type-list, type-body, and delegate-parameter-list references into class, struct, interface, record class/struct, and delegate shapes. Positional record parameter lists are represented explicitly.

All token identities are supplied by the host from ProtocolAi. Cross-resource types, expressions, blocks, metadata, and explicit-interface specifiers are opaque references; GrammarAi does not duplicate their maps. Parameter attributes have a distinct parameter-level slot. Declaration-level attribute attachment is represented across all listed member families. Metadata roots remain independently composable rather than being attached automatically to every declaration production.

The C# 11 profile is explicit metadata. It does not filter productions or establish complete compiler conformance. Variance legality, attribute target legality, finalizer name/type-context legality, explicit-interface resolution, modifier/parameter legality, constraint ordering/satisfiability, name resolution, and semantic legality remain host/compiler responsibilities. These are bounded structural slices, not a complete C# language definition or source parser.

## Pinned compiler corpus

`CSharp11CompilerCorpusTests` is a **test-only** compiler-backed corpus pinned to Roslyn 4.11.0 and explicit language-version settings. Positive cases cover representative member forms, attributes, generic constraints, generic type declarations, variance, positional records, explicit-interface implementations, and block/expression-bodied finalizers. Earlier-version cases check selected feature gates; negative C# 11 cases cover malformed declarations and attributes, wrong operator arity, incomplete custom events, and recursive constructor initializers.

The Roslyn dependency is confined to the test project, not the GrammarAi runtime or package.

**Evidence boundary:** this proves that representative snippets compile or fail under the pinned Microsoft compiler dialect. It does not prove GrammarAi parses source, that its structural maps cover all C# 11 syntax, or that a structurally valid proposal is semantically legal. GrammarAi remains a bounded structural mapping resource, not a source parser.

The profile is metadata; it does not filter or rewrite productions. Future language-version upgrades must update the profile, feature inventory, corpus, and source provenance together.

## Follow-up inventory (2026-11-11)

- Declaration-level attributes were attached to the represented member families without moving metadata syntax into the declaration grammar.
- Explicit-interface member alternatives use an opaque host-owned interface-specifier reference; ordinary modifier lists are intentionally absent from those forms.
- Finalizers have block and expression-body alternatives with a dedicated tilde token role.
- The production counts above reflect the current implementation after these follow-ups.
