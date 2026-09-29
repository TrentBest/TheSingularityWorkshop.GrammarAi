# GrammarAI AI Exchange

## Grammar is the shape the model is allowed to speak

ProtocolAI defines **WHAT** the application knows.

GrammarAI defines **HOW** those identities may be composed.

An AI exchange needs both.

```text
ProtocolAI
    |
    | vocabulary / meaning
    v
GrammarAI
    |
    | legal structure
    v
AI Exchange
    |
    v
LLM / human interaction
```

GrammarAI therefore participates in the exchange without becoming an LLM client.

---

## The exchange has two paths

The same GrammarAI definition should support:

- a copy/paste interaction with a human-facing LLM;
- a connected provider adapter.

```text
                         GrammarAI
                            |
                    structured definition
                            |
                +-----------+-----------+
                |                       |
                v                       v
          COPY / PASTE             PROVIDER ADAPTER
                |                       |
                +-----------+-----------+
                            |
                            v
                           LLM
                            |
                            v
                    model response
                            |
                +-----------+-----------+
                |                       |
                v                       v
             pasted                 connected
             response               response
                |                       |
                +-----------+-----------+
                            |
                            v
                    host validation
```

Transport does not change grammar semantics.

---

## What the model receives

A useful model-facing exchange can expose:

1. the ProtocolAI vocabulary;
2. the GrammarAI definition;
3. selected runtime context;
4. the current request;
5. explicit response expectations.

For example:

```text
PROTOCOL
[1001] People
  [2001] bobId = "Bob"
  [2002] janeId = "Jane"

GRAMMAR
[3001] Greeting start=[4001]
  rule [5001] [4001] -> [1001:2001]
  rule [5002] [4001] -> [1001:2002]

CONTEXT
selectedPerson = [1001:2001]

REQUEST
Generate a greeting for the selected person.
```

The exact envelope syntax is a host concern until the exchange contract is finalized.

The important property is that the model receives **meaning and legal structure separately**.

---

## Grammar is a constraint description, not an execution engine

GrammarAI can describe legal structure.

It should not:

- call a provider;
- store an API key;
- send HTTP;
- select a model;
- execute a command;
- mutate application state.

A future adapter may translate GrammarAI into:

- a provider grammar;
- a JSON Schema;
- a constrained-output representation;
- a local parser;
- another execution-specific structural format.

That translation belongs at the boundary.

```text
GrammarAI
    |
    v
provider-neutral structure
    |
    +--> Provider A format
    +--> Provider B format
    +--> local parser
    +--> clipboard instructions
```

This keeps the grammar portable.

---

## Human-readable and machine-readable forms

The Workshop should deliberately maintain two manifestations of the same grammar:

### Human-readable

Optimized for:

- copy/paste;
- debugging;
- documentation;
- inspection;
- teaching.

### Machine-readable

Optimized for:

- validation;
- transport;
- serialization;
- provider adapters;
- FSM_COS composition.

They must describe the same semantic object.

```text
             GrammarDefinition
                    |
          +---------+---------+
          |                   |
          v                   v
     human form          machine form
          |                   |
          +---------+---------+
                    |
                    v
              same semantics
```

---

## Response validation

The host should never treat arbitrary model text as an already-valid grammar instance.

The intended pipeline is:

```text
LLM response
     |
     v
transport decode
     |
     v
grammar validation
     |
  +--+--+
  |     |
valid  invalid
  |     |
  v     v
ProtocolAI    clarification /
resolution    rejection
  |
  v
host policy
```

GrammarAI establishes whether the structure is legal.

ProtocolAI establishes what referenced identities mean.

The host decides what the result is allowed to do.

---

## Contextual interaction

A contextual session should be able to retain a stable semantic frame rather than repeatedly rebuilding it from natural-language prose.

Conceptually:

```text
SESSION
  |
  +-- Protocol definitions
  +-- Grammar definitions
  +-- selected context
  +-- prior accepted exchanges
  +-- current request
  |
  v
provider / clipboard
  |
  v
response
  |
  v
validated semantic result
```

This is one place where the phrase **interaction at speed** becomes technically meaningful: the host can maintain a stable context and send only the delta that changes between interactions.

That is an architectural direction, not yet an implementation guarantee.

---

## API keys stay above GrammarAI

Credentials belong to the host/provider boundary.

```text
GUI / host
   |
   +-- provider
   +-- model
   +-- endpoint
   +-- credential
   |
   v
provider adapter
   |
   v
GrammarAI translation
   |
   v
LLM
```

GrammarAI should never receive or retain an API key.

A provider adapter can be swapped without changing the grammar definition.

---

## Future composition

The intended Workshop progression is:

```text
ProtocolAI
   WHAT
     |
     v
GrammarAI
   HOW
     |
     v
AI Exchange
   WHAT + HOW + CONTEXT + REQUEST
     |
     v
provider adapter OR clipboard
     |
     v
LLM
     |
     v
validated result
     |
     v
FSM_COS / host
```

GrammarAI's role is therefore stable even as the transport changes.

