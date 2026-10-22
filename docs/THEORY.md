<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

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

## Reading a production: alternatives are not sequences

A production has a left-hand nonterminal and an ordered right-hand sequence:

```text
[4001] -> [1001:2001] [1002:3001]
```

Read this as: when the grammar expands nonterminal `[4001]` using this rule, the resulting structure contains the terminal from protocol `1001` with symbol ID `2001`, followed by the terminal from protocol `1002` with symbol ID `3001`.

The order is part of the structure. Reversing those two symbols describes a different sequence.

Two productions with the same left-hand side describe alternatives:

```text
[4001] -> [1001:2001]
[4001] -> [1001:2002]
```

This means the grammar offers two ways to expand `[4001]`; it does not mean both right-hand sides are concatenated. That distinction matters when a person is reading a grammar, when a host builds a validator, or when an adapter translates the definition for another system.

An empty right-hand side represents **epsilon**: the rule produces an empty sequence. It is not the same as a missing rule. A missing production leaves a nonterminal undefined in the grammar; epsilon is an explicit production whose result is empty.

## The exact boundary of structural validation

Building a GrammarAI definition checks a finite set of internal invariants:

- the grammar, start symbol, rule IDs, left-hand nonterminals, and symbol IDs use nonzero identities;
- rule IDs are unique within the definition;
- the start symbol appears as the left-hand side of a production;
- every nonterminal referenced on a rule's right-hand side has at least one production;
- the built definition exposes an immutable snapshot of its ordered rules.

The boundary is just as important as the checks. GrammarAI does **not** load external protocol definitions while building a grammar. A terminal reference records the owning protocol ID and symbol ID; it does not prove that the referenced protocol or symbol exists, that the symbol is suitable for the current task, or that two separately authored artifacts agree about its meaning. A host must resolve and link those references against the vocabulary it intends to use.

Structural validation also does not parse arbitrary text or prove that a model response conforms to the grammar. A grammar definition is the structure to inspect, translate, or use as input to a separate validator or provider adapter. Those downstream mechanisms are not supplied by the current GrammarAI core.

Keep these four questions distinct:

| Question | Responsible boundary |
|---|---|
| Is the grammar internally well-formed? | GrammarAI's structural construction checks |
| Do its terminal references resolve in the intended protocol definitions? | Host-side linking/resolution |
| Does a candidate response conform to the grammar? | A parser or validator that actually checks the response |
| Is the resulting meaning authorized and safe to act on? | Host policy and application logic |

Passing one question does not imply passing the next. In particular, structural validity is not semantic truth, and semantic resolution is not authorization.

## A consumer example: define a small command language

Imagine an application that lets an AI propose one of two operations on one of two known workshop stations. The application wants to accept a *structured choice*, not treat fluent prose as permission to operate equipment.

For illustration, suppose two application-owned protocols contain these identities:

```text
Protocol 7100 — Actions
  [7101] inspect
  [7102] prepare

Protocol 7200 — Stations
  [7201] forge
  [7202] laboratory
```

A grammar can define the allowed combinations without copying or taking ownership of those identities:

```text
[4001] Request -> [4002] [4003]

[4002] Action ->
    [7100:7101]
    [7100:7102]

[4003] Station ->
    [7200:7201]
    [7200:7202]
```

This is a conceptual grammar sketch, not a claim that the package parses the text shown here. The rule for `Request` describes an ordered pair: an action followed by a station. The alternatives under `Action` mean that the action position may be `inspect` or `prepare`; they do not mean both actions happen. The same distinction applies to the station alternatives.

The consumer's application flow is then explicit:

1. **Define the vocabulary.** The application owns what each action and station identity means.
2. **Define the allowed shape.** GrammarAI describes which identities may occupy each position and in what order.
3. **Link the definitions.** The host verifies that each referenced protocol and symbol exists in the exact versions it intends to use. GrammarAI's structural build does not perform this external lookup.
4. **Check the candidate.** A real parser, validator, or provider integration must check whether a candidate conforms to the grammar; the grammar definition alone does not validate arbitrary model text.
5. **Apply policy.** The host checks the user's permissions, station state, operational preconditions, and whether the proposed action requires confirmation.
6. **Execute only through the host.** A structurally valid `prepare + forge` proposal is still only a proposal. GrammarAI neither authorizes nor performs it.

The practical benefit is not that the model becomes trustworthy. It is that the application can state and inspect its intended vocabulary and structure, keep ownership boundaries clear, and reject a proposal at the layer responsible for the failed condition. A structurally valid proposal can still be inappropriate, unauthorized, or unsafe.

Use this pattern when a consumer needs a portable definition of *which combinations are expressible*—for example, a small command language, a workflow request, or a structured interaction between independently owned domains. Do not use GrammarAI alone as a parser, permission system, or execution engine.

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

---

<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://github.com/TrentBest.png?size=200" alt="The Singularity Workshop on GitHub" width="200">
  </a>
</p>

<p align="center">
  <a href="https://github.com/TrentBest">GitHub</a> ·
  <a href="https://coderlegion.com/">Coder Legion</a> ·
  <a href="https://www.patreon.com/c/TheSingularityWorkshop">Patreon</a> ·
  <a href="https://www.paypal.com/donate/?hosted_button_id=3Z7263LCQMV9J">PayPal</a>
  <br>
  <a href="https://github.com/TrentBest/FSM_API">FSM_API</a> ·
  <a href="https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization">FSM_Serialization</a> ·
  <a href="https://github.com/TrentBest/WebPage">WebPage</a>
</p>

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
