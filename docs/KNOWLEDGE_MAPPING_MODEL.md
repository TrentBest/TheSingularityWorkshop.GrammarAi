<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# GrammarAi — Knowledge Mapping Model

## 1. Purpose: represent meaning and structure, not compile source

GrammarAi is an AI-facing knowledge-mapping layer. It is not intended to become a compiler, compiler front end, or general-purpose source-code execution engine.

The goal is to let an AI refer to known concepts and their permitted relationships using stable integer identities and compact structures. Human-readable names and descriptions help define and inspect the map; they are not required to be repeated in every model exchange.

| Layer | Owns | Does not own |
|---|---|---|
| ProtocolAi | Vocabulary identity: protocol IDs, symbol IDs, names, values, and globally shared vocabulary metadata when defined | Grammar productions or application execution |
| GrammarAi | Grammar-owned node kinds, contextual roles/slots, nonterminals, productions, ordering, composition, structural validation, and opaque qualified references to external categories/members | Global vocabulary identity/category metadata, host catalog membership/exposure, or authorization |
| Host/application | Registry, context, prompt construction, identifier lifecycle, validation policy, and interpretation | The reusable grammar definition itself |
| LLM | Selects and composes mapped structures; proposes new identifiers when permitted | Authority to execute a proposal |

## 2. Token identity versus grammatical role

A source-language spelling such as the C# keyword if is not the structure. It is one representation of a known vocabulary item.

A ProtocolAi symbol can provide a stable identity for that item. GrammarAi then defines the role that identity can occupy and the relationships in which it participates.

ProtocolAi owns the stable identity of vocabulary categories/protocols and their member symbols. GrammarAi does not infer or duplicate that metadata. A GrammarAi structural slot may target one opaque external category identity, and each external relationship carries a qualified pair: category/protocol ID plus member symbol ID. The host-supplied resolver checks that pair against the exact catalog/capability snapshot used for the request.

A category identity and a grammatical role are different dimensions. The category identifies the namespace in which a vocabulary member is defined; the GrammarAi slot identifies the contextual role the value plays in one structure (for example, Condition, Body, Repository, or Trigger). The same member number may appear under two category IDs without aliasing. The host decides whether a category/member pair exists and is exposed; GrammarAi checks only the qualified target and graph shape.

## 3. Grammar as a typed relationship map

The existing production model provides grammar-owned nonterminal IDs; ordered production rules; alternative productions for the same left-hand side; ordered symbols within each production; protocol-qualified external terminal references; epsilon and recursive composition.

The typed structural model now adds grammar-owned node-kind IDs, contextual slot IDs, target-kind constraints, cardinality, and ordered graph relationships. A schema can express relationships such as:

- this node is an IfStatement;
- it has one required condition/expression slot;
- it has one required body slot;
- it may have zero or more ordered ElseIfClause children;
- it may have one optional ElseClause;
- the ElseClause has a body but no condition;
- each branch is a distinct node with an explicit role and order.

This is more expressive than merely listing C# punctuation tokens. It maps the conceptual shape of a construct while retaining links to the vocabulary used to express it.

The C# language examples are deliberately split into independent resources. `CSharpExpressionGrammar` describes a bounded precedence and associativity hierarchy for expressions; `CSharpTypeGrammar` describes qualified names, generic arguments, nullable forms, array ranks, and tuple types; `CSharpDeclarationGrammar` describes method/property/field signatures, constructor declarations with optional `this`/`base` initializers, parameter lists/defaults, separate constructor argument lists, and get/set/init accessors with semicolon, block, or expression bodies. All three consume host-supplied ProtocolAi identities. Declaration type, expression, and block references are opaque links to independently mapped resources, not duplicated productions. These are bounded structural grammar resources, not a complete or version-pinned C# language definition: lexical rules, operators, events, indexers, attributes, generic constraints, contextual keywords, type binding, compiler diagnostics, and language-version conformance remain outside this slice.

## 4. Worked example: if / else-if / else

The following illustrates the implemented structural model. The example node and slot IDs are local to this illustrative schema; applications define and version their own maps.

Source-like input:

~~~csharp
if (conditionA)
{
    DoA();
}
else if (conditionB)
{
    DoB();
}
else
{
    DoFallback();
}
~~~

Knowledge-map shape:

~~~text
IfStatement
├── Condition -> Expression(conditionA)
├── Body      -> Block(DoA)
├── ElseIf[0]
│   ├── Condition -> Expression(conditionB)
│   └── Body      -> Block(DoB)
└── Else
    └── Body      -> Block(DoFallback)
~~~

A compact conceptual identity representation might look like:

~~~text
[IfStatement]
  [Condition] -> [ExpressionNode: identifier#A]
  [Body]      -> [BlockNode: invocation#DoA]
  [ElseIf]    -> [ElseIfClause]
                  [Condition] -> [ExpressionNode: identifier#B]
                  [Body]      -> [BlockNode: invocation#DoB]
  [Else]      -> [ElseClause]
                  [Body]      -> [BlockNode: invocation#DoFallback]
~~~

The canonical wire contract is now implemented by GrammarStructureProposalCodec. It carries a wire version, structure-definition ID/version, node tuples, and relationship tuples; see [Typed Structural Schema](STRUCTURAL_SCHEMA.md) for the exact format.

The v2 relationship tags distinguish nested GrammarAi nodes from external ProtocolAi values. External references are accepted only when a host-supplied IGrammarExternalReferenceResolver.Resolve(GrammarProtocolReference) returns ResolvedAndExposed; UnknownProtocol, UnknownSymbol, and NotExposed are rejected. The host binds that resolver to the exact catalog/capability snapshot used for the model request. The snapshot identifier remains in the host envelope, not in GrammarAi state. The codec reads legacy v1 nested-node payloads and emits canonical v2. Old v1 readers cannot consume v2, so the host must negotiate the wire version.

The QuickStart demonstrates the bounded expression, type, and member-declaration grammar resources, then instantiates the complete conditional shape rather than only an if node: one required condition and body, an ordered repeated ElseIf slot, and an optional single Else slot. It serializes and validates that graph, then separately demonstrates host-scoped identifier reuse and a snapshot-bound external ProtocolAi reference. These examples are illustrative host adapters, not a production registry or language-wide C# map.

The important properties are:
- node kinds and slot roles have stable identities;
- relationships are explicit and typed;
- repeated clauses preserve order;
- optional and required relationships are distinguishable;
- variable names are references to host-managed identities, not repeated vocabulary definitions;
- the model can return a compact structure that the host resolves using its current registry.

## 5. C# expression grammar: precedence as reusable structure

The first C# expansion is a bounded production grammar, separate from the control-flow graph schema. `CSharpExpressionGrammar.Create` builds a recursive grammar over host-supplied ProtocolAi token identities. It includes assignment, conditional and null-coalescing expressions, logical OR/AND, bitwise OR/XOR/AND, equality, relational, shifts, additive, multiplicative, unary, identifiers, literals, and parenthesized expressions.

The production layers encode precedence directly: multiplicative expressions nest below additive, shift, relational, equality, bitwise, logical, null-coalescing, conditional, and assignment layers. Binary operator layers use left recursion to express left associativity; assignment and null-coalescing recurse on their right side. Conditional true/false branches recurse through the expression start symbol. Parenthesized expressions also recurse back to that start symbol.

The host passes one category/protocol ID and a complete mapping from `CSharpExpressionToken` roles to ProtocolAi member IDs. GrammarAi validates that every required role has a nonzero identity, but does not own the token catalog, token spelling, language-version selection, or lexical recognition.

This is deliberately a core expression subset, not a complete C# expression grammar. The grammar now includes recursive postfix member access, invocation (with empty or comma-separated argument lists), and element access. The postfix layer binds more tightly than unary operators and can compose chains such as `receiver.Member(arguments)[index]`. Casts, conditional access, lambdas, patterns, query expressions, named/ref/out arguments, and newer language-version features remain future slices. The grammar describes production structure; it does not perform parsing, binding, type checking, overload resolution, or code generation.

The QuickStart prints this grammar alongside the typed If/ElseIf/Else graph. The grammar's production model is the appropriate recursive representation for precedence layers; the typed graph remains useful for explicit contextual roles, cardinality, and ordered relationships.

## 6. New identifiers and conversation context

Not every identifier can be known in advance. A newly introduced variable, method, type, or application-specific name may arrive as text.

The host should own the lifecycle:

1. Receive a model proposal containing a new identifier value.
2. Validate that the proposal is permitted in the current task/context.
3. Allocate or retrieve a stable ProtocolAi identity in the appropriate namespace.
4. Add the mapping to the active context or registry.
5. Include the identity-to-value mapping in subsequent model context as needed.
6. Resolve later references by integer identity rather than repeatedly transmitting the original spelling.

This is context/registry management, not a reason for GrammarAi to own global mutable state. Identity allocation, collision handling, scope, lifetime, and persistence belong to the host or vocabulary owner.

A name's identity should be scoped carefully. Two variables named value in different lexical scopes are not necessarily the same symbol. The mapping contract must distinguish identity from display spelling.

## 7. The AI exchange

The target is not “send the whole C# source to a compiler grammar.” The target is “give the model a compact map of the relevant vocabulary and structural rules, then receive a compact structured proposal.”

A typical exchange has four conceptual parts:

1. Vocabulary manifest: protocol ID, symbol IDs, categories/roles, and only the descriptions needed for the current task.
2. Grammar manifest: grammar identity/version, node/production IDs, slot rules, cardinality, ordering, and allowed alternatives.
3. Context manifest: currently known identifiers and their stable references, scoped to the task.
4. Proposal: integer references and structure, plus literal strings only where a genuinely new value must be introduced.

The host must reject unknown IDs, invalid structure, out-of-scope identifiers, stale versions, and proposals that violate application policy. A structurally valid proposal is not automatically correct, safe, authorized, or executable.

## 8. C# is a vocabulary and structure resource, not a compiler target

A C# mapping can be useful without implementing Roslyn or compiling C#.

The resource should identify relevant vocabulary and map constructs such as declarations and modifiers; types, generic parameters, and constraints; expressions and operators (a first precedence-layered core expression grammar is now implemented); statements and control flow; invocation/member access; object and collection creation; patterns, lambdas, async/await; namespaces, using directives, and attributes.

The map should be layered. A task that only needs conditional control flow should not need the entire C# language definition in the prompt.

The upstream C# language version and provenance still matter for accuracy. Compiler-grade parsing, binding, type checking, diagnostics, and code generation remain optional host integrations—not the definition of GrammarAi.

## 9. Immediate domain gaps

| Gap | Why it matters | Direction |
|---|---|---|
| External category references | GrammarAi stores qualified opaque category/member IDs; the host resolver validates membership and exposure | Keep global category identity in ProtocolAi and contextual slot role in GrammarAi |
| Cross-repository category metadata | GrammarAi consumes opaque category/member IDs; ProtocolAi remains the identity owner | Coordinate the concrete ProtocolAi category metadata contract; GrammarAi must not duplicate it |
| Typed grammar nodes and slots | Implemented as grammar-owned node kinds and contextual slots | Extend the map with domain-specific structural definitions |
| Optional/repeated relationships | Implemented through slot cardinality and ordered graph relationships | Preserve ordering and constraints in future domain maps |
| Stable grammar and mapping versions | Definition ID/version and independent wire version are implemented | Define compatibility and explicit migration policy between versions |
| Compact deterministic exchange | Implemented by GrammarStructureProposalCodec | Keep canonical encoding stable within a wire version |
| Identifier lifecycle and scope | Example demonstrates host-owned scoped reuse; no production registry service is included | Host owns persistence, collision handling, scope, and context manifest |
| Structural proposal validation | Implemented for identity, slots, cardinality, order, definition version, and resource limits | Add host-specific scope and policy validation outside GrammarAi |
| C# mapping resources | Core expression (39), type (21), and member declaration (27) production slices plus If/ElseIf/Else structure are implemented | Expand declaration families and define explicit language-version pinning; keep compiler semantics outside GrammarAi |
| Cross-request continuity | Later turns must reuse known identities | Host supplies the active mapping/context on each request |

## 10. Recommended implementation sequence

1. **Completed:** preserve ProtocolAi vocabulary/category/member identity separately from GrammarAi node-kind, slot, and instance identities. GrammarAi consumes opaque qualified references; the host resolver validates them against the request snapshot. Any richer global category metadata remains ProtocolAi-owned.
2. **Completed:** implement typed structural roles, required/optional/repeated cardinality, and ordered graph relationships.
3. **Completed:** implement a compact deterministic proposal contract with separate definition and wire versions.
4. **Completed first vertical slice:** define and test IfStatement, ElseIfClause, ElseClause, Condition, and Body.
5. **Example added:** demonstrate host-owned scoped identifier reuse across requests. This is illustrative; production persistence and allocation remain host responsibilities.
6. **Implemented:** add independently composable expression, type, and member-declaration grammar resources over host-supplied ProtocolAi token identities. Continue with additional declaration families and explicit language-version policy; do not attempt a full compiler grammar.
7. Add other common domains using the same reusable structural model: JSON, HTTP/URI, SQL dialects, Python, JavaScript/TypeScript, XML, YAML, and domain-specific command formats.

## 11. Explicit non-goals

- Building a C# compiler or replacing Roslyn.
- Implementing code generation as a GrammarAi responsibility.
- Treating a model response as trusted or executable.
- Owning application variable scope, identifier allocation, or conversation memory.
- Requiring source text tokenization as the primary route to use the knowledge map.
- Assuming every programming language shares one universal grammar.
- Making a diagnostic Describe() string into a wire format without a separately versioned contract.

## Verification status

The typed structural schema, graph instance model, versioned integer-first proposal codec, scoped host-registry example, and expanded 39-production C# core expression grammar are implemented and covered by repository CI. The If/ElseIf/Else graph and expression grammar are focused slices, not a complete C# language map. A structurally valid proposal remains untrusted data: semantic interpretation, identifier persistence, scope, authorization, and execution belong to the host.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
