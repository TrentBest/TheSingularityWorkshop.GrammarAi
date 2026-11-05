<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# GrammarAi — Knowledge Mapping Model

## 1. Purpose: represent meaning and structure, not compile source

GrammarAi is an AI-facing knowledge-mapping layer. It is not intended to become a compiler, compiler front end, or general-purpose source-code execution engine.

The goal is to let an AI refer to known concepts and their permitted relationships using stable integer identities and compact structures. Human-readable names and descriptions help define and inspect the map; they are not required to be repeated in every model exchange.

| Layer | Owns | Does not own |
|---|---|---|
| ProtocolAi | Vocabulary identity: protocol IDs, symbol IDs, names, and mapped values | Grammar productions or application execution |
| GrammarAi | Structural categories, nonterminals, productions, ordering, composition, and grammar validation | The meaning/identity of external vocabulary symbols or host authorization |
| Host/application | Registry, context, prompt construction, identifier lifecycle, validation policy, and interpretation | The reusable grammar definition itself |
| LLM | Selects and composes mapped structures; proposes new identifiers when permitted | Authority to execute a proposal |

## 2. Token identity versus grammatical role

A source-language spelling such as the C# keyword if is not the structure. It is one representation of a known vocabulary item.

A ProtocolAi symbol can provide a stable identity for that item. GrammarAi then defines the role that identity can occupy and the relationships in which it participates.

The current ProtocolAi ProtocolSymbol contract provides an ID, name, and value. It does not currently expose a first-class category field. Therefore, categories such as keyword, operator, punctuation, literal, identifier, expression-kind, or statement-kind must not be assumed to exist in ProtocolAi today.

The design must make category explicit somewhere in the mapping contract. Two approaches require evaluation:

1. ProtocolAi owns a category/metadata vocabulary that classifies its symbols globally.
2. GrammarAi owns typed references or category constraints that classify the role a referenced ProtocolAi symbol plays in a particular grammar.

These approaches are not equivalent. The first classifies a vocabulary item globally; the second classifies its grammatical role contextually. A token may have one lexical identity but participate in several structural contexts. The API should preserve that distinction rather than duplicate or mutate ProtocolAi identity.

## 3. Grammar as a typed relationship map

The existing production model gives us a useful foundation: grammar-owned nonterminal IDs; ordered production rules; alternative productions for the same left-hand side; ordered symbols within each production; protocol-qualified external terminal references; epsilon and recursive composition.

For the intended AI mapping, the next conceptual step is to make structural roles legible and compact. A production should ultimately express relationships such as:

- this node is an IfStatement;
- it has one required condition/expression slot;
- it has one required body slot;
- it may have zero or more ordered ElseIfClause children;
- it may have one optional ElseClause;
- the ElseClause has a body but no condition;
- each branch is a distinct node with an explicit role and order.

This is more expressive than merely listing C# punctuation tokens. It maps the conceptual shape of a construct while retaining links to the vocabulary used to express it.

## 4. Worked example: if / else-if / else

The following is a conceptual mapping, not a claim that the current public API already exposes these exact node types or field names.

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

The exact wire encoding is deliberately not specified here. A readable sketch is not a stable serialization contract.

The important properties are:
- node kinds and slot roles have stable identities;
- relationships are explicit and typed;
- repeated clauses preserve order;
- optional and required relationships are distinguishable;
- variable names are references to host-managed identities, not repeated vocabulary definitions;
- the model can return a compact structure that the host resolves using its current registry.

## 5. New identifiers and conversation context

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

## 6. The AI exchange

The target is not “send the whole C# source to a compiler grammar.” The target is “give the model a compact map of the relevant vocabulary and structural rules, then receive a compact structured proposal.”

A typical exchange has four conceptual parts:

1. Vocabulary manifest: protocol ID, symbol IDs, categories/roles, and only the descriptions needed for the current task.
2. Grammar manifest: grammar identity/version, node/production IDs, slot rules, cardinality, ordering, and allowed alternatives.
3. Context manifest: currently known identifiers and their stable references, scoped to the task.
4. Proposal: integer references and structure, plus literal strings only where a genuinely new value must be introduced.

The host must reject unknown IDs, invalid structure, out-of-scope identifiers, stale versions, and proposals that violate application policy. A structurally valid proposal is not automatically correct, safe, authorized, or executable.

## 7. C# is a vocabulary and structure resource, not a compiler target

A C# mapping can be useful without implementing Roslyn or compiling C#.

The resource should identify relevant vocabulary and map constructs such as declarations and modifiers; types, generic parameters, and constraints; expressions and operators; statements and control flow; invocation/member access; object and collection creation; patterns, lambdas, async/await; namespaces, using directives, and attributes.

The map should be layered. A task that only needs conditional control flow should not need the entire C# language definition in the prompt.

The upstream C# language version and provenance still matter for accuracy. Compiler-grade parsing, binding, type checking, diagnostics, and code generation remain optional host integrations—not the definition of GrammarAi.

## 8. Immediate domain gaps

| Gap | Why it matters | Direction |
|---|---|---|
| First-class symbol categories/roles | Current ProtocolAi symbols expose ID/name/value only | Decide global vocabulary category versus grammar-contextual role |
| Typed grammar nodes and slots | Existing nonterminals/productions are generic | Model node kinds, named roles, cardinality, and ordering |
| Optional/repeated relationships | Ordered productions alone do not clearly express semantic slot cardinality | Add explicit structural constraints without losing production semantics |
| Stable grammar and mapping versions | Prompt and response must agree on the same map | Version definitions and reject stale references |
| Compact deterministic exchange | Description text is diagnostic, not a wire contract | Define canonical integer-first serialization separately |
| Identifier lifecycle and scope | New names must persist across requests without conflating scopes | Host-owned interning, scope, and context manifest |
| Structural proposal validation | Model output is untrusted | Validate IDs, slots, cardinality, order, references, and limits |
| C# mapping resource | Gives a practical first language domain | Build a layered, version-pinned mapping from relevant constructs |
| Cross-request continuity | Later turns must reuse known identities | Host supplies the active mapping/context on each request |

## 9. Recommended implementation sequence

1. Specify the identity boundary: inspect ProtocolAi's current symbol contract and decide where categories live. Do not silently add categories to GrammarAi if they are globally owned vocabulary metadata.
2. Specify typed structural roles: define node kinds, slots, required/optional/repeated cardinality, and order.
3. Specify the compact proposal contract: define stable IDs, grammar version, protocol references, unknown-ID handling, and canonical encoding.
4. Implement and test a small control-flow map: IfStatement, ElseIfClause, ElseClause, Condition, and Body.
5. Add host-side identifier/context examples: demonstrate first introduction of a variable and reuse by integer identity on the next exchange.
6. Expand the C# mapping by useful construct families; do not attempt a full compiler grammar before the mapping contract is proven.
7. Add other common domains using the same reusable structural model: JSON, HTTP/URI, SQL dialects, Python, JavaScript/TypeScript, XML, YAML, and domain-specific command formats.

## 10. Explicit non-goals

- Building a C# compiler or replacing Roslyn.
- Implementing code generation as a GrammarAi responsibility.
- Treating a model response as trusted or executable.
- Owning application variable scope, identifier allocation, or conversation memory.
- Requiring source text tokenization as the primary route to use the knowledge map.
- Assuming every programming language shares one universal grammar.
- Making a diagnostic Describe() string into a wire format without a separately versioned contract.

## Verification status

This document defines the intended knowledge-mapping direction. The existing public implementation remains the smaller structural grammar foundation. The conceptual C# control-flow sketch is not yet an implemented API or serialized contract.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
