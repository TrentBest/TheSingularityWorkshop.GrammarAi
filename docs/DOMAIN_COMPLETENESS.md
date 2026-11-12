<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# GrammarAi domain completeness audit

**Audit scope:** current `development` implementation and documentation  
**Current package version:** `0.1.0-alpha.2`  
**Status:** integer-first typed structural mapping and the first versioned proposal codec are implemented; domain-specific maps and host integration remain in progress.

## Executive finding

GrammarAi's intended purpose is an AI-facing knowledge-mapping system, not a compiler or general-purpose source parser. ProtocolAi owns stable vocabulary identities; GrammarAi owns typed structural node kinds, contextual roles/slots, composition, and structural validation; the host owns identifier allocation, scope, active context, interpretation, trust, policy, and execution. The LLM proposes structures; proposals remain untrusted until validated by the host.

The implementation provides the existing production-grammar foundation and a typed graph model: immutable node/slot definitions, required/optional/repeated cardinality, ordered graph relationships, structural validation, and deterministic integer-first proposal exchange. Slots now explicitly target either GrammarAi node kinds or opaque ProtocolAi categories; relationships carry either a child instance or a qualified external protocol/member reference. The codec emits tagged v2 tuples and continues to read legacy v1 nested-node proposals. External values fail closed without a host-supplied resolver bound to the exact request catalog/capability snapshot. The QuickStart's identifier registry remains illustrative, not a production identity service.

Cross-repository ownership: ProtocolAi owns global vocabulary/category/member identities. GrammarAi stores only opaque, qualified category/member references and owns contextual slot roles. The host resolver validates membership and exposure against the exact request catalog/capability snapshot. Global category metadata and contextual grammatical roles are distinct; GrammarAi does not duplicate the former.



See the [Knowledge Mapping Model](KNOWLEDGE_MAPPING_MODEL.md) for the intended architecture and conceptual C# control-flow mapping. Compiler/parser integrations may be useful optional adapters, but they are not the definition of GrammarAi.

This audit deliberately separates implemented behavior from capabilities that need design and implementation.

## What exists today

| Capability | Current state | Evidence / boundary |
|---|---|---|
| Integer-backed grammar identity and start symbol | Implemented | `GrammarBuilder`, `GrammarDefinition` |
| Nonterminals and ordered productions | Implemented | `GrammarRule`, `GrammarSymbol` |
| Alternative productions | Implemented | Multiple rules may share a left-hand-side nonterminal |
| Sequences | Implemented | Ordered right-hand-side symbols |
| Epsilon productions | Implemented | Empty right-hand side |
| Direct and mutual recursion | Representable | Structural construction; no parser consumes it |
| Structural validation | Implemented | Start symbol, nonterminal references, unique rule IDs, bounded collections |
| Reachability analysis | Implemented | `FindUnreachableNonTerminals` |
| External terminal references | Implemented | `GrammarProtocolReference` |
| Host-supplied external-link discovery | Implemented | `FindUnresolvedProtocolReferences` |
| Deterministic description | Implemented | `Describe` (diagnostic only; not wire format) |
| Typed node-kind definitions | Implemented | `GrammarNodeDefinition` |
| Contextual typed slots and cardinality | Implemented | `GrammarSlotDefinition` |
| Versioned structural schema | Implemented | `GrammarStructureDefinition.Version` |
| Integer-addressed graph instances | Implemented | `GrammarNodeInstance`, `GrammarRelationship`, `GrammarStructureInstance` |
| Ordered relationships, shared nodes, and cycles | Implemented | `GrammarStructureInstance` validation and tests |
| Canonical integer-first proposal codec | Implemented | `GrammarStructureProposalCodec`; canonical tagged v2 output and legacy v1 nested-node input |
| Wire and schema compatibility checks | Implemented | Separate wire version and definition ID/version validation |
| Bounded proposal input and graph size | Implemented | `GrammarLimits` |
| External category slot targets and qualified external values | Implemented | `GrammarSlotTarget`, `GrammarRelationship`, `IGrammarExternalReferenceResolver`; membership/exposure remains host supplied |
| Host-owned identifier reuse example | Illustrative example | `examples/GrammarAi.QuickStart/Program.cs`; not a production registry |
| Request-snapshot-bound external resolver example | Illustrative example | `HostCatalogSnapshot` validates qualified references; `HostExchangeEnvelope` retains snapshot identity outside GrammarAi |
| Observer condition/gate-intent schema | Implemented as a separate bounded model | `ObserverGrammarDefinition` and versioned JSON codec |
| Lexical/token grammar | Not implemented | No token recognizer or lexical rule model |
| Literal terminals / token spellings | Not implemented in the generic model | Terminals are ProtocolAi references |
| Grammar import/export formats | Not implemented | No EBNF, BNF, ABNF, PEG, ANTLR, or tree-sitter importer |
| Lexer / tokenizer | Not implemented | No source-text scanning API |
| Parser / parse tree / diagnostics | Not implemented | No source-text parsing API |
| Bounded C# expression, type, declaration, attribute, and generic-constraint grammar resources | Partially implemented | `CSharpExpressionGrammar` (39 productions), `CSharpTypeGrammar` (21), `CSharpDeclarationGrammar` (98), `CSharpMetadataGrammar` (40), and `CSharpTypeDeclarationGrammar` (25); host-supplied ProtocolAi identities; opaque cross-resource references; no parser or semantic analysis |
| URI-reference grammar resource | Implemented as a bounded RFC 3986 URI-reference component grammar (26 productions) over host-owned token identities | Opaque authority/path/query/fragment tokenization and component lexical conformance remain host responsibilities; this is not an ABNF importer or full RFC validator |
| ABNF representation | Immutable typed expression model and bounded structural validator implemented; focused tests added | RFC 5234/RFC 7405 expression distinctions, case-insensitive linking, source-ordered `=` / `=/` assembly, octet forms, ASCII character/prose constraints, and node/depth budgets are represented. Source importer, parser/recognizer, lowering adapter, and HTTP grammar remain unimplemented |
| XML document-structure grammar resource | Implemented as a bounded XML 1.0 document-structure grammar (19 productions) over host-owned token identities | Matching element names, lexical constraints, namespaces, entity/DTD processing, and cross-token well-formedness remain host responsibilities; this is not an XML parser or full conformance claim |
| Common language/format catalogue | Not included | No versioned standard grammar catalogue |
| Semantic analysis / type checking | Not implemented | Must remain distinct from syntax recognition |
| Incremental parsing / error recovery | Not implemented | Requires a parser design and source-position model |
| Provider constrained-output translation | Not implemented | Belongs in explicit adapters, not the core grammar model |

## C# language profile and provenance

The current bounded C# slices declare a **C# 11 baseline** through CSharpGrammarProfile.WorkshopBaseline. The descriptor keeps the language version separate from GrammarAi's own grammar/schema versions and exposes feature availability for init-only accessors (C# 9+) and checked user-defined operators (C# 11+). The composable CSharpMetadataGrammar adds 40 productions for attribute sections/targets/qualified names/argument boundaries, generic parameter lists with optional variance markers, and representative constraint families (class/class?, struct, unmanaged, notnull, default, type, and new()). These metadata structures are separate roots for host composition; they do not resolve names or enforce declaration-specific placement, constraint ordering, or semantic satisfiability. A test-only pinned Roslyn 4.11.0 corpus now includes representative attributes and generic constraints alongside the earlier declaration fixtures. This remains metadata and compiler-dialect evidence, not a parser or full C# 11 conformance claim.

## The key formalism decision

The current production model is closest to a context-free grammar (CFG): a nonterminal expands to an ordered sequence of terminals and nonterminals, and multiple productions express alternatives.

That is a good foundational formalism, but it is not equivalent to every grammar notation or parser technology.

| Formalism / technology | What it contributes | Relationship to current model |
|---|---|---|
| BNF | Context-free production notation | Closest direct fit |
| EBNF | BNF plus grouping, optionality, repetition, and shorthand | Can be normalized into CFG productions, subject to explicit expansion limits |
| ABNF | Internet-protocol grammar notation, including lexical conventions and repetition/ranges | Needs an importer and careful terminal/byte model |
| PEG | Ordered choice and parsing-expression semantics | Not equivalent to CFG alternatives; needs its own semantic model or explicit conversion restrictions |
| Regular expressions | Regular-language recognition | Useful for lexical rules, but not a replacement for a full syntactic grammar |
| ANTLR grammars | Lexer/parser rules, modes, channels, actions, predicates | Import requires a supported subset; actions and predicates must not be executed |
| tree-sitter grammars | Incremental concrete-syntax parsing, precedence/conflict metadata, external scanners | Requires a richer adapter model; cannot be flattened blindly into current productions |

**Do not silently treat these formalisms as interchangeable.** An importer must declare its source formalism, supported subset, conversion behavior, and every unsupported or lossy construct.

## Common grammar families to support

The catalogue should be modular and versioned. It should not mean that every grammar is compiled into the core assembly.

| Priority | Grammar family | Why it matters | Important qualification |
|---|---|---|---|
| 1 | JSON | Small, bounded, ubiquitous structured data format | The RFC 8259 production grammar is implemented over host-owned token identities; lexing, parsing, and lexical conformance remain outside GrammarAi |
| 1 | URI / HTTP field and message syntax (ABNF) | Real-world internet protocol syntax | ABNF, byte/text boundaries, and protocol-version differences matter |
| 1 | XML | Common document markup and structured data | XML well-formedness includes constraints beyond a naïve CFG |
| 2 | SQL | Common query language | Must be dialect-specific: SQL standard, PostgreSQL, SQLite, SQL Server, etc. |
| 2 | C# | Core Workshop ecosystem language | Separate lexical grammar, syntactic grammar, contextual keywords, and semantic/compiler rules |
| 2 | JavaScript / ECMAScript | Common application and web language | Versioned specification; lexical goals and contextual parsing behavior matter |
| 2 | Python | Common general-purpose language | Indentation-sensitive lexical structure and version-specific syntax |
| 3 | Java, Go, Rust | Common compiled languages | Each needs its own versioned lexical and syntactic definition |
| 3 | YAML, TOML, Markdown | Common configuration/document formats | These have significant lexical, indentation, context, or semantic rules |
| 3 | Protocol Buffers, GraphQL, regular-expression dialects | Common schema/query/sub-language families | Dialect and version are part of grammar identity |

This is a support roadmap, not a claim that these grammars are currently present.

## C# specifically

A usable C# grammar package needs more than a file named `CSharp.g4`.

It needs a pinned C# language version and a traceable source, and it must distinguish:

1. **Lexical grammar:** identifiers, keywords, contextual keywords, literals, operators, comments, whitespace, Unicode and escape rules.
2. **Syntactic grammar:** compilation units, namespaces, declarations, types, statements, expressions, patterns, queries, attributes, and related productions.
3. **Context-sensitive interpretation:** cases where tokenization or syntactic interpretation depends on context.
4. **Semantic/compiler rules:** name binding, overload resolution, type checking, definite assignment, accessibility, and other rules that are not simply grammar recognition.

The C# grammar can recognize syntactic structure; it cannot replace Roslyn or claim that syntactically accepted source is semantically valid C#.

The recommended first delivery is a separately versioned C# grammar resource/package with a documented language version, provenance, supported subset, and conformance corpus. A later Roslyn adapter can provide compiler-grade syntax and semantic services without making the grammar core depend on Roslyn.

## Remaining knowledge-mapping work

1. Continue coordinating category metadata with ProtocolAi. GrammarAi now consumes opaque category/member identities; ProtocolAi remains the identity owner and no category metadata is duplicated here.
2. Define compatibility and migration policy between schema versions; the current codec rejects mismatches rather than migrating them.
3. The QuickStart now demonstrates a host resolver bound to one illustrative catalog snapshot and keeps snapshot identity in a host exchange envelope. Replace the sample data with real ProtocolAi identity allocation, persistence, and cross-request context in a consuming host.
4. Add more reusable domain maps beyond the If/ElseIf/Else control-flow slice. The RFC 8259 JSON grammar, bounded RFC 3986 URI-reference grammar, and bounded XML 1.0 document-structure grammar are now non-C# domain resources. ABNF now has an immutable typed expression model and bounded validator, with source import and recognition still future work; HTTP field syntax and HTTP/1.1 message syntax remain separate future resources, alongside SQL dialects, Python, JavaScript/TypeScript, and YAML. The bounded C# resources remain independent structural slices, not a parser or full language conformance claim.
5. Define a stable root/entry-point and graph-profile policy only where a consuming domain requires one; the core graph model intentionally allows shared nodes and cycles.
6. Expand proposal diagnostics and adversarial tests while keeping semantic validation, authorization, and execution in the host.

## Optional grammar-resource and parser ecosystem

### A. Grammar identity and provenance

A reusable grammar needs more than an integer ID and display name. Define a versioned descriptor containing at least:

- stable grammar identity;
- grammar name and language/dialect;
- grammar version and source-language version;
- formalism (CFG, EBNF, ABNF, PEG, etc.);
- source URI, publisher, license, and retrieval/revision identity;
- encoding and lexical model;
- dependencies/imports;
- declared feature subset and unsupported constructs;
- deterministic content hash.

The descriptor must distinguish a grammar's own version from the version of the language it describes.

### B. Lexical model

Define tokens and lexical rules separately from syntactic productions. The model needs to describe token identity, spelling/pattern, trivia, precedence/longest-match behavior, modes or lexical states where supported, and source spans. Do not assume every terminal is a single ProtocolAi symbol or that every language token is a fixed string.

### C. Formalism and normalization

Choose a canonical internal representation and explicit conversion contracts. CFG normalization must preserve alternatives, sequences, epsilon, recursion, and precedence where representable. PEG ordered choice must not be silently converted into unordered CFG alternatives. ABNF byte ranges and repetition need explicit semantics.

### D. Parsing and diagnostics

A parser capability needs a defined input model, token stream, parse result/tree, source locations, diagnostics, ambiguity policy, cancellation and resource limits, and failure/error-recovery behavior. Algorithm selection (LL, LR, Earley, GLR, PEG/packrat, or a hybrid) must follow the supported formalism rather than being assumed universal.

### E. Conformance and versioning

Every bundled grammar needs:
- pinned upstream source and license review;
- language/dialect version;
- positive and negative conformance corpus;
- regression tests for known ambiguities and edge cases;
- a documented unsupported-feature list;
- reproducible import/build and content hash;
- explicit update policy.

### F. Composition and ecosystem boundaries

Keep the ownership model clear:
- ProtocolAi owns semantic vocabulary identities.
- GrammarAi owns grammar definitions, formalism metadata, composition and structural validation.
- A future lexer/parser capability consumes grammar definitions and source text.
- Roslyn, ANTLR, tree-sitter, provider APIs, and other engines belong behind explicit adapters.
- The host owns semantic interpretation, trust, authorization, and execution.

The existing observer grammar remains a separate, bounded domain schema. It should not be mistaken for a general-purpose grammar parser or be forced into the CFG model without a demonstrated need.

## Optional grammar-resource and parser roadmap

The following items describe a possible adjacent grammar-resource/parser ecosystem. They are not the implementation sequence or release gate for GrammarAi's core knowledge-mapping domain.

1. **Define optional resource contracts:** formalism taxonomy, grammar descriptor/provenance, token model, versioning, limits, and import/export contracts.
2. **Prove the pipeline with JSON:** import a pinned JSON grammar, tokenize, parse, produce a source-located tree, and run positive/negative conformance tests.
3. **Prove internet grammar support with ABNF:** URI and selected HTTP syntax, with explicit byte/character behavior and unsupported ABNF constructs.
4. **Add EBNF import and normalization:** preserve semantics and reject unsupported conversions rather than approximating silently.
5. **Add a C# grammar resource:** pin a language version and source; validate the grammar against a substantial corpus. Add a Roslyn adapter for compiler-grade syntax/semantics.
6. **Expand the catalogue:** SQL dialects, ECMAScript, Python, XML, then Java/Go/Rust and configuration/schema formats according to demand.
7. **Add optional parser-engine adapters:** PEG, ANTLR, tree-sitter, and provider constrained-output targets only where their semantics can be preserved and tested.

## Completion rule

GrammarAi's core knowledge-mapping domain is complete when its identity ownership, typed structural composition, versioned deterministic proposal contract, bounded structural validation, host integration boundary, and representative domain maps are documented and tested. A structurally valid proposal is not proof of semantic correctness, authorization, or executability.

Lexical grammars, importers, parsers, source diagnostics, Roslyn, ANTLR, and tree-sitter integrations are optional adjacent capabilities or separate adapters. They may be valuable resources, but they are **not** prerequisites for the core AI-facing knowledge-mapping domain and must not redefine GrammarAi as a compiler project.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>


### Finalizer declaration follow-up (2026-11-11)

- Extended `CSharpDeclarationGrammar` from 95 to 98 productions with a destructor nonterminal and block/expression-body finalizer alternatives.
- Added the `Tilde` token role, structural assertions, and a Roslyn C# 11 fixture covering both body forms.
- Static constructors remain representable by the existing constructor shape with host-supplied modifiers; host validation owns modifier and parameter legality.
- Finalizer name/type-context legality remains host-owned.
