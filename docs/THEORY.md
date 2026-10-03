# GrammarAI Theory

## The structural boundary after semantic identity

GrammarAI sits one step beyond ProtocolAI.

ProtocolAI establishes **WHAT** a known value means.

GrammarAI establishes **HOW** known identities may be arranged.

The model may remain probabilistic. The semantic and structural artifacts do not have to remain probabilistic.

```text
probabilistic model
        |
        v
candidate language
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
validation / adaptation
        |
        v
deterministic host representation
```

This is not a claim that GrammarAI constrains a model by itself. It is the abstract artifact a host or provider adapter can later use for that purpose.

---

## From vocabulary to language

A vocabulary tells us what symbols exist:

```text
[1001] People

[2001] Bob
[2002] Jane
```

A grammar tells us how those symbols can participate in structure:

```text
[4001] -> [1001:2001]
[4001] -> [1001:2002]
```

The central distinction is:

> **ProtocolAI defines the WHAT. GrammarAI defines the HOW.**

The grammar owns relationships.

The protocol owns semantic identity.

Neither needs to own the model.

---

## Grammar as deterministic structure

A grammar is useful because its structure can be reasoned about independently of the system that eventually consumes it.

A grammar can have:

- an identity;
- a start symbol;
- nonterminals;
- ordered production rules;
- references to external protocol symbols;
- deterministic validation;
- deterministic self-description.

That makes it a portable artifact.

```text
grammar definition
       |
       +--> inspect
       +--> validate
       +--> compare
       +--> serialize
       +--> translate
       +--> visualize
```

Those are properties of the artifact, not execution semantics.

---

## The grammar as a self-describing artifact

A grammar has its own identity:

```text
[3001] Greeting
```

a start symbol:

```text
start = [4001]
```

and rules:

```text
rule [5001] [4001] -> [1001:2001]
```

The structure can therefore describe itself.

That matters for AI because a host can inspect and present the same artifact to a human, a model adapter, a validator, a serializer, or another system without requiring each consumer to invent its own hidden interpretation.

---

## Grammar as a deprobabilization surface

ProtocolAI narrows **meaning**.

GrammarAI narrows **arrangement**.

That gives us two different forms of deprobabilization:

```text
probabilistic output
        |
        v
semantic resolution
   ProtocolAI
        |
        v
structural validation
   GrammarAI
        |
        v
host policy
        |
        v
execution
```

ProtocolAI can distinguish:

```text
known / unknown / invalid
```

GrammarAI can distinguish whether the resulting identities occupy a structure permitted by the grammar.

Neither step proves that the model's intent was correct.

They make the application's acceptance criteria explicit.

That is the important architectural improvement.

---

## Why this may matter for hallucination

Grammar constraints are often discussed as a way to reduce invalid model output.

GrammarAI should make a narrower claim.

It provides an abstract structural representation that a host can use to validate or translate model-facing structure.

Therefore:

```text
model proposes
      |
      v
candidate structure
      |
      v
grammar validation
      |
   +--+--+
   |     |
 valid invalid
   |     |
   v     v
continue reject
```

This can reduce the number of structurally invalid responses that reach later stages **if a host actually enforces the grammar**.

It does not guarantee semantic correctness.

A perfectly valid grammar can still contain a perfectly valid reference to the wrong object.

That is why GrammarAI and ProtocolAI remain separate.

---

## Context-free grammar as a reference point

GrammarAI intentionally resembles a conventional context-free grammar:

```text
nonterminal
    |
    +--> terminal
    |
    +--> nonterminal
    |
    +--> sequence
```

That provides a familiar theoretical foundation.

But the current package is not a parser generator, compiler, constrained decoder, or provider grammar exporter.

It is a structural representation.

Keeping that boundary small makes the artifact easier to reuse.

---

## External ownership is deliberate

GrammarAI references external protocol symbols as:

```text
[protocolId:symbolId]
```

For example:

```text
[1001:2001]
```

GrammarAI can therefore express a relationship without copying the vocabulary into the grammar.

```text
ProtocolAI
    |
    +-- owns [1001:2001]
    +-- owns [1001:2002]

GrammarAI
    |
    +-- owns [4001]
    +-- owns rule [5001]
    +-- references [1001:2001]
    +-- references [1001:2002]
```

This is the same ownership discipline used throughout The Singularity Workshop:

> **Reference a capability you do not own; own only the structure that is yours.**

---

## Provider neutrality

A grammar should not contain provider-specific branches:

```text
if OpenAI ...
if Anthropic ...
if Gemini ...
```

Instead:

```text
                  GrammarAI
                      |
                abstract grammar
                      |
          +-----------+-----------+
          |           |           |
          v           v           v
      adapter A   adapter B   validator
          |           |           |
          +-----------+-----------+
                      |
                      v
                     host
```

The adapter translates the abstract artifact into the concrete representation expected by its environment.

That keeps the core reusable.

---

## Grammar is not execution

A rule such as:

```text
[4001] -> [1001:2001]
```

does not mean:

> Execute Bob.

It means:

> This structural position may resolve to the symbol owned by protocol 1001 with symbol identity 2001.

Execution semantics belong to the host.

This prevents a grammar from quietly becoming a workflow engine.

---

## Independent protocols can meet at the grammar

Imagine:

```text
Protocol A
[7100] Commands
[7101] move

Protocol B
[7200] Objects
[7201] forge
```

A grammar can reference both.

```text
Protocol A ----+
               |
               +--> Grammar --> structured artifact
               |
Protocol B ----+
```

The grammar becomes the structural meeting point while neither protocol loses ownership of its vocabulary.

That is a small but powerful composition primitive.

---

## The future exchange

The conceptual AI exchange becomes:

```text
ProtocolAI
    WHAT exists
       |
       v
GrammarAI
    HOW it may be arranged
       |
       v
context + request
       |
       v
provider / human interaction
       |
       v
candidate response
       |
       v
validation
       |
       v
host policy
       |
       v
execution
```

This is deliberately not an AI framework.

It is a set of explicit boundaries around one.

---

## What GrammarAI can make deterministic

GrammarAI can make the following deterministic when the host uses it:

- grammar identity;
- symbol relationships;
- production ordering;
- referenced protocol identities;
- structural validation;
- self-description.

It cannot make deterministic:

- model intent;
- model reasoning;
- provider inference;
- human interpretation;
- application policy;
- execution side effects.

That distinction should remain visible in every future integration.

---

## The likely adapter boundary

The most interesting future layer is translation:

```text
             GrammarAI
                 |
          abstract grammar
                 |
       +---------+---------+
       |         |         |
       v         v         v
    provider   validator  serializer
     adapter     adapter    adapter
       |         |         |
       +---------+---------+
                 |
                 v
               Host
```

The adapter can evolve without forcing GrammarAI to know every provider's grammar format.

This is exactly the kind of boundary that allows a tiny primitive to remain useful as the surrounding system grows.

---

## Questions the alpha intentionally leaves open

- How are grammar IDs allocated?
- How are grammar versions negotiated?
- Should recursive productions be explicitly supported?
- How are independently authored grammars composed?
- How should abstract grammar map to provider-specific constraints?
- How should generated output be validated?
- Which structural guarantees should be enforced before a host sees a response?
- Can grammar-constrained generation measurably reduce structural hallucination classes?
- What are the token and latency tradeoffs?

These questions belong to future layers and experiments rather than hidden assumptions in the current core.

---

## Architectural invariant

> **GrammarAI describes how protocol identities may be connected without owning their meaning, the model that generated them, or the execution that follows.**

In one line:

```text
ProtocolAI = WHAT
GrammarAI  = HOW
Host       = POLICY + EXECUTION
```

The smallness is intentional.

The interesting part is the boundary.
