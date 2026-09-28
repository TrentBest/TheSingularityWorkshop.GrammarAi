# GrammarAI Theory

## The deterministic boundary after the model

GrammarAI sits one step beyond ProtocolAI in the same funnel.

ProtocolAI turns candidate language into owned identities. GrammarAI constrains how those identities may be arranged.

```text
probabilistic model
        |
        v
ProtocolAI
   WHAT / identity
        |
        v
GrammarAI
   HOW / structure
        |
        v
host validation
        |
        v
deterministic protocol
```

The model remains probabilistic. The protocol representation does not have to remain probabilistic.

---

## From vocabulary to language

ProtocolAI gives a system a vocabulary.

That is not yet a language.

A vocabulary tells us what symbols exist:

```text
[1001] People

[2001] Bob
[2002] Jane
```

A grammar tells us how those symbols can participate in a structure:

```text
[4001] -> [1001:2001]
[4001] -> [1001:2002]
```

This creates the central distinction:

> **ProtocolAI defines the WHAT. GrammarAI defines the HOW.**

---

## A grammar is a relationship system

GrammarAI does not own the meaning of `Bob`.

It owns the rule that says a particular grammar symbol may resolve to a protocol symbol:

```text
Grammar nonterminal
      |
      v
[4001]
      |
      +----> [1001:2001]
      |
      +----> [1001:2002]
```

The grammar therefore connects independently owned semantic vocabularies.

That is the architectural reason protocol references carry both:

- protocol identity;
- symbol identity.

---

## The grammar as a self-describing artifact

A grammar has its own identity:

```text
[3001] Greeting
```

It has a start symbol:

```text
start = [4001]
```

It has rules:

```text
rule [5001] [4001] -> [1001:2001]
```

The complete structure can therefore be represented as data.

That matters because a grammar may eventually be:

- serialized;
- compared;
- versioned;
- generated;
- translated;
- visualized;
- supplied to a model host;
- composed with another capability.

The grammar does not need to know which of those things will happen.

---

## Context-free grammar as a useful reference point

A conventional context-free grammar defines a language through production rules.

GrammarAI intentionally resembles that model because it provides a useful conceptual foundation:

```text
nonterminal
    |
    +--> terminal
    |
    +--> nonterminal
    |
    +--> sequence
```

But the current package should not be confused with a complete parser generator or compiler grammar system.

The current alpha is a **structural representation**.

Parsing, compilation, execution semantics, and provider-specific translation remain separate questions.

---

## Why this matters for AI

Modern AI interfaces increasingly use structured generation.

A model may be asked to produce:

- JSON;
- a function call;
- a tool invocation;
- a schema-constrained response;
- grammar-constrained text.

OpenAI's current documentation explicitly describes grammar-constrained tool output and structured outputs. [OpenAI Function Calling](https://developers.openai.com/api/docs/guides/function-calling) and [Structured Outputs](https://developers.openai.com/api/docs/guides/structured-outputs)

GrammarAI asks a complementary architectural question:

> **Can the application own an abstract grammar independently of the model provider?**

If yes, then the grammar becomes a reusable artifact.

A provider adapter can translate it later.

---

## Provider neutrality

A grammar should not contain:

```text
if OpenAI ...
if Anthropic ...
if Gemini ...
```

That would turn the grammar into a provider integration layer.

Instead:

```text
GrammarAI
    |
    v
abstract grammar
    |
    +---- provider adapter A
    +---- provider adapter B
    +---- provider adapter C
```

The provider-specific representation belongs downstream.

This is the same composition principle used throughout the Workshop ecosystem.

---

## Grammar is not execution

A rule can describe:

```text
[4001] -> [1001:2001]
```

It does not mean:

> Execute Bob.

It means:

> This structural position may resolve to the symbol owned by protocol 1001.

Execution semantics belong to the host.

This distinction prevents a grammar from becoming a hidden workflow engine.

---

## Grammar and protocol composition

The conceptual stack is:

```text
Protocol A
   |
   +-- symbols

Protocol B
   |
   +-- symbols

       \\
        \\
         v

      Grammar
         |
         +-- nonterminals
         +-- productions
         +-- references
         |
         v
    structured protocol
```

The grammar is therefore a composition surface.

It connects things without becoming the owner of the things it connects.

---

## What a future lifecycle might look like

```text
DEFINE PROTOCOLS
       |
       v
DEFINE GRAMMAR
       |
       v
DESCRIBE STRUCTURE
       |
       v
ADAPT TO HOST
       |
       v
CONSTRAIN / GENERATE
       |
       v
DECODE REFERENCES
       |
       v
EXECUTE
```

Only the first three stages belong to the current alpha.

That is deliberate.

---

## Questions the alpha leaves open

### Grammar identity
How should grammar IDs be allocated?

### Versioning
Can a grammar evolve while preserving compatibility?

### Validation
How much structural validation belongs in the definition layer?

### Cycles
Should recursive productions be supported explicitly?

### Composition
How should two independently authored grammars be combined?

### Translation
How should GrammarAI map to Lark, regex, JSON Schema, or another provider representation?

### Runtime
Should grammar evaluation ever become an execution concern?

### Model behavior
How should generated output be validated against the abstract grammar?

These questions should be answered by the architecture rather than smuggled into convenience APIs.

---

## Architectural invariant

> **GrammarAI describes how protocol symbols may be connected without owning the symbols' meaning or the model that consumes the structure.**

ProtocolAI provides the lexicon.

GrammarAI provides the structure.

The host provides execution.

That separation is the point.
