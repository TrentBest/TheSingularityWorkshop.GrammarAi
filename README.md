# TheSingularityWorkshop.GrammarAi

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NuGet version](https://img.shields.io/nuget/v/TheSingularityWorkshop.GrammarAi?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.GrammarAi)
[![NuGet downloads](https://img.shields.io/nuget/dt/TheSingularityWorkshop.GrammarAi?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.GrammarAi)
[![Build](https://img.shields.io/github/actions/workflow/status/TrentBest/TheSingularityWorkshop.GrammarAi/build.yml?branch=master&style=flat-square&logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/actions/workflows/build.yml)
[![Code Coverage](https://img.shields.io/codecov/c/github/TrentBest/TheSingularityWorkshop.GrammarAi?style=flat-square)](https://codecov.io/gh/TrentBest/TheSingularityWorkshop.GrammarAi)

**The structure layer for self-defining, integer-backed AI protocols.**

<p align="center">
  <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.GrammarAi/master/docs/images/grammar-ai-money-shot.svg" alt="GrammarAI architecture: ProtocolAI identities become structured through GrammarAI" width="1100">
</p>

<p align="center"><strong>ProtocolAI gives meaning an address. GrammarAI gives those addresses a language.</strong></p>

> **GrammarAI defines how application-owned identities may be connected without owning the identities or the model that eventually consumes them.**

---

## If you only have a minute

GrammarAI answers:

> **How can these things be connected?**

ProtocolAI answers:

> **What is this thing?**

Together:

```text
ProtocolAI                         GrammarAI
WHAT                               HOW
identity                           structure
vocabulary                         productions
owned symbols                      relationships
     |                                  |
     +------------ references ----------+
                      |
                      v
             abstract AI structure
                      |
                      v
                 host / adapter
```

The smallest useful GrammarAI program is:

```csharp
using TheSingularityWorkshop.GrammarAi;

var grammar = new GrammarBuilder(
        3001,
        "Greeting",
        4001)
    .Rule(
        5001,
        4001,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(1001, 2001)))
    .Rule(
        5002,
        4001,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(1001, 2002)))
    .Build();

Console.WriteLine(grammar.Describe());
```

It produces a self-description such as:

```text
[3001] Greeting start=[4001]
  rule [5001] [4001] -> [1001:2001]
  rule [5002] [4001] -> [1001:2002]
```

**That is the core idea. The rest of this document progressively explains the boundary, the API, the architecture, and the current limits.**

### Go directly to the depth you need

| I want to... | Read |
|---|---|
| Install and use GrammarAI | [Consuming GrammarAI](docs/CONSUMING.md) |
| See concrete patterns | [Examples](docs/EXAMPLES.md) |
| Understand why the boundary exists | [Theory](docs/THEORY.md) |
| Understand what is intentionally unresolved | [Reflection](docs/REFLECTION.md) |

---

# 1. What GrammarAI actually does

GrammarAI is a deliberately small structural layer.

It gives an application a way to describe:

- a grammar identity;
- a start symbol;
- integer-backed nonterminals;
- ordered production rules;
- references to externally owned protocol symbols;
- deterministic structural validation;
- deterministic self-description.

```text
application meaning
        |
        v
   ProtocolAI
      WHAT
        |
        v
   GrammarAI
       HOW
        |
        v
 host / adapter
        |
        v
 model / system
```

**GrammarAI stops at the structural boundary.**

Deeper: [Theory — The deterministic boundary](docs/THEORY.md#the-deterministic-boundary-after-the-model).

---

# 2. What GrammarAI does not do

This boundary is just as important.

GrammarAI does **not** own:

- an LLM client;
- model selection;
- inference;
- tokenization;
- prompt transport;
- a universal protocol vocabulary;
- REST/OpenAPI;
- GUI rendering;
- MicroBundle hosting;
- tool execution;
- application workflows;
- provider-specific grammar formats.

So this is intentional:

```text
GrammarAI
    |
    +--> "Here is the structure."
```

This is not:

```text
GrammarAI
    |
    +--> call a model
    +--> execute a tool
    +--> render a GUI
    +--> run an application
```

Those concerns belong downstream.

Deeper: [Reflection — What is deliberately not decided](docs/REFLECTION.md#what-is-deliberately-not-decided).

---

# 3. WHAT → HOW

A vocabulary and a grammar solve different problems.

Suppose an application owns:

```text
[1001] People

[2001] Bob
[2002] Jane
```

That tells us what the symbols mean.

It does not tell us how they may participate in a structure.

GrammarAI can describe:

```text
[4001] -> [1001:2001]
[4001] -> [1001:2002]
```

Now there is a relationship.

**The grammar owns the relationship. The protocol owns the meaning.**

```text
ProtocolAI
   |
   | WHAT
   | [1001:2001] = Bob
   |
   v
GrammarAI
   |
   | HOW
   | [4001] -> [1001:2001]
   |
   v
structured representation
```

Deeper: [Theory — From vocabulary to language](docs/THEORY.md#from-vocabulary-to-language).

---

# 4. Ownership: reference, don't copy

GrammarAI represents an external terminal as:

```text
[protocolId:symbolId]
```

For example:

```text
[1001:2001]
```

means:

```text
protocol = 1001
symbol   = 2001
```

GrammarAI can therefore say:

> This grammar position references symbol 2001 from protocol 1001.

It does **not** say:

> GrammarAI owns symbol 2001.

That keeps the structural layer from becoming a second vocabulary system.

```text
ProtocolAI
    |
    +-- owns [1001:2001]
    +-- owns [1001:2002]

GrammarAI
    |
    +-- owns [4001]
    +-- owns production [5001]
    +-- references [1001:2001]
    +-- references [1001:2002]
```

Deeper: [Consuming GrammarAI — Protocol identities](docs/CONSUMING.md#2-start-with-protocol-identities).

---

# 5. The two fundamental symbol types

## Nonterminal

A nonterminal belongs to the grammar:

```csharp
var greeting = GrammarSymbol.NonTerminal(4001);
```

A referenced nonterminal must have a production rule.

## Protocol terminal

A protocol terminal belongs to an external vocabulary:

```csharp
var bob = GrammarSymbol.Terminal(
    new GrammarProtocolReference(1001, 2001));
```

So:

```text
[4001]
  |
  +--> [1001:2001]
```

means the grammar references an external protocol identity; it does not redefine it.

Deeper: [Examples — One protocol symbol](docs/EXAMPLES.md#example-1--one-protocol-symbol).

---

# 6. Build a grammar

A grammar currently has three central pieces:

1. its identity;
2. its start symbol;
3. its production rules.

```csharp
var grammar = new GrammarBuilder(
        3001,
        "Greeting",
        4001)
    .Rule(
        5001,
        4001,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(1001, 2001)))
    .Rule(
        5002,
        4001,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(1001, 2002)))
    .Build();
```

Conceptually:

```text
[3001] Greeting
    |
    +-- start = [4001]
             |
             +-- rule [5001] -> [1001:2001]
             +-- rule [5002] -> [1001:2002]
```

The builder creates the structural artifact.

It does not turn that artifact into an execution engine.

Deeper: [Consuming GrammarAI — Define a grammar](docs/CONSUMING.md#3-define-a-grammar).

---

# 7. Grammars can be nested

A grammar can reference another nonterminal:

```csharp
var grammar = new GrammarBuilder(
        3001,
        "Greeting",
        4001)
    .Rule(
        5001,
        4001,
        GrammarSymbol.NonTerminal(4002))
    .Rule(
        5002,
        4002,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(1001, 2001)))
    .Build();
```

That produces:

```text
[4001]
   |
   v
[4002]
   |
   v
[1001:2001]
```

This is structural composition, not an execution instruction.

Deeper: [Examples — Nested structure](docs/EXAMPLES.md#example-3--nested-structure).

---

# 8. A grammar describes itself

The grammar exposes a deterministic description:

```csharp
Console.WriteLine(grammar.Describe());
```

For example:

```text
[3001] Greeting start=[4001]
  rule [5001] [4001] -> [1001:2001]
  rule [5002] [4001] -> [1001:2002]
```

That makes the grammar inspectable as data.

A self-describing structure can eventually be:

- diagnosed;
- documented;
- tested;
- serialized;
- compared;
- versioned;
- visualized;
- translated;
- composed.

Those capabilities do not need to be forced into the current core.

Deeper: [Theory — The grammar as a self-describing artifact](docs/THEORY.md#the-grammar-as-a-self-describing-artifact).

---

# 9. Why integer-backed?

The integer is an address, not the meaning.

```text
[3001] Greeting
start = [4001]
rule  = [5001]
terminal = [1001:2001]
```

That makes identity explicit and transportable.

The same identity can survive later operations such as:

- serialization;
- composition;
- inspection;
- comparison;
- storage;
- versioning;
- translation between hosts.

The owning system supplies the meaning.

GrammarAI supplies the structural address and relationship.

---

# 10. Why this matters for AI

An AI-facing structure eventually crosses a boundary.

The model may be probabilistic.

The application may need deterministic structure.

So the architecture can separate:

```text
probabilistic language
        |
        v
owned identity
   ProtocolAI
        |
        v
owned structure
   GrammarAI
        |
        v
host translation
        |
        v
validated / constrained representation
        |
        v
execution
```

GrammarAI is not claiming to solve every step.

It provides the structural artifact that later steps can consume.

Deeper: [Theory — Provider neutrality](docs/THEORY.md#provider-neutrality).

---

# 11. Provider neutrality

The grammar should not contain:

```text
if OpenAI ...
if Anthropic ...
if Gemini ...
```

That would make GrammarAI a provider integration layer.

Instead:

```text
                     GrammarAI
                         |
                  abstract grammar
                         |
             +-----------+-----------+
             |           |           |
             v           v           v
        provider A  provider B   validator
          adapter      adapter      adapter
             |           |           |
             +-----------+-----------+
                         |
                         v
                        host
```

The provider-specific representation belongs downstream.

Deeper: [Reflection — The likely next architectural pressure](docs/REFLECTION.md#the-likely-next-architectural-pressure).

---

# 12. Grammar is not execution

This distinction is fundamental.

A rule such as:

```text
[4001] -> [1001:2001]
```

does **not** mean:

> Execute Bob.

It means:

> This structural position may resolve to the symbol owned by protocol 1001 with symbol identity 2001.

The grammar describes a legal structural relationship.

A host decides what that relationship means operationally.

That prevents GrammarAI from quietly becoming a workflow engine.

Deeper: [Theory — Grammar is not execution](docs/THEORY.md#grammar-is-not-execution).

---

# 13. Independent protocols can meet at the grammar

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

The grammar becomes the structural meeting point while neither protocol loses ownership of its vocabulary.

The current alpha deliberately stops before assigning execution semantics to that combination.

Deeper: [Examples — Combining independent protocols](docs/EXAMPLES.md#example-4--combining-independent-protocols).

---

# 14. The Workshop stack

Within The Singularity Workshop:

```text
                    DOMAIN
                       |
                       v
                  ProtocolAI
                     WHAT
                       |
                       v
                  GrammarAI
                      HOW
                       |
                       v
              provider / host adapter
                       |
                       v
                     model
                       |
                       v
                 host validation
                       |
                       v
                    execution
```

The principle is simple:

> A foundation should know the shape of its capability without becoming coupled to every environment that may use it.

GrammarAI provides structure.

The host provides environment and execution.

A future composition layer can assemble those capabilities.

---

# 15. Deprobabilization in one sentence

**ProtocolAI reduces semantic ambiguity; GrammarAI reduces structural ambiguity.**

Neither package makes an LLM deterministic. Together they give a host explicit artifacts against which model output can be resolved and validated before execution.

See **[GrammarAI Theory](docs/THEORY.md)** for the deeper argument and its limits.

---

# 16. Current alpha boundary

**Current source version: `0.1.0-alpha.2`.**

This release establishes:

- grammar identity;
- integer start symbols;
- integer nonterminals;
- ordered production rules;
- external protocol references;
- deterministic self-description;
- structural validation of referenced nonterminals.

It does **not** establish:

- grammar parsing;
- grammar compilation;
- provider-specific grammar export;
- constrained decoding;
- protocol negotiation;
- grammar version negotiation;
- execution semantics;
- a model client;
- a runtime workflow engine.

Those are future composition questions.

Deeper: [Reflection](docs/REFLECTION.md).

---

# 17. What happens when you consume it?

The intended flow is:

```text
1. Obtain or define protocol identities
                |
                v
2. Reference those identities
                |
                v
3. Define grammar nonterminals
                |
                v
4. Add production rules
                |
                v
5. Build the grammar
                |
                v
6. Inspect / validate
                |
                v
7. Give the artifact to a host or adapter
```

The first six steps are where the current package is useful.

The final step deliberately belongs outside the core.

---

# 18. Install and use

```bash
dotnet add package TheSingularityWorkshop.GrammarAi --version 0.1.0-alpha.2
```

Or:

```xml
<PackageReference Include="TheSingularityWorkshop.GrammarAi" Version="0.1.0-alpha.2" />
```

Then:

```csharp
using TheSingularityWorkshop.GrammarAi;

var grammar = new GrammarBuilder(
        3001,
        "Greeting",
        4001)
    .Rule(
        5001,
        4001,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(1001, 2001)))
    .Build();

Console.WriteLine(grammar.Describe());
```

For the complete practical guide: [Consuming GrammarAI](docs/CONSUMING.md).

For runnable patterns: [Examples](docs/EXAMPLES.md).

---

# 19. Try the repository example

The repository contains an executable quickstart:

```bash
dotnet run --project examples/GrammarAi.QuickStart/GrammarAi.QuickStart.csproj
```

The example intentionally stays small.

The goal is to make the structural boundary obvious before adding infrastructure.

---

# 20. Development

```bash
dotnet restore TheSingularityWorkshop.GrammarAi.slnx
dotnet build TheSingularityWorkshop.GrammarAi.slnx --configuration Release
dotnet test TheSingularityWorkshop.GrammarAi.slnx --configuration Release
dotnet pack TheSingularityWorkshop.GrammarAi.csproj --configuration Release --output ./artifacts
```

The public workflow restores, builds, tests with coverage, packs the NuGet artifact, and publishes it through NuGet Trusted Publishing.

---

# 21. Documentation map

The README is the **map**.

The linked documents are the **rooms**.

| Need | Go here |
|---|---|
| Install and consume | [CONSUMING.md](docs/CONSUMING.md) |
| Concrete patterns | [EXAMPLES.md](docs/EXAMPLES.md) |
| Architectural reasoning | [THEORY.md](docs/THEORY.md) |
| Current limits and open questions | [REFLECTION.md](docs/REFLECTION.md) |
| Package | [NuGet](https://www.nuget.org/packages/TheSingularityWorkshop.GrammarAi) |
| Source | [GitHub](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi) |

The README answers **what is this?**

The linked documents answer **how does it work?**

The source answers **exactly how is it implemented?**

That gives a reader a high-level path without throwing away the detail needed by someone who wants to understand the machinery.

---

# 22. Architectural invariant

> **GrammarAI describes how protocol symbols may be connected without owning the symbols' meaning or the model that consumes the structure.**

In one line:

```text
ProtocolAI = WHAT
GrammarAI  = HOW
Host       = EXECUTION
```

That is the boundary.

Everything else composes around it rather than blurring it.

---

## Resources

- **NuGet:** https://www.nuget.org/packages/TheSingularityWorkshop.GrammarAi
- **Source:** https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi
- **Theory:** [docs/THEORY.md](docs/THEORY.md)
- **Examples:** [docs/EXAMPLES.md](docs/EXAMPLES.md)
- **Consuming:** [docs/CONSUMING.md](docs/CONSUMING.md)
- **Reflection:** [docs/REFLECTION.md](docs/REFLECTION.md)
- **ProtocolAI:** https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi
- **FSM_API:** https://www.nuget.org/packages/TheSingularityWorkshop.FSM_API
- **MicroBundleDomain:** https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain

<p align="center">
  <a href="https://github.com/TrentBest/FSM_API">
    <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" height="200">
  </a>
</p>

<p align="center">
  <em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br>
  <strong>Because state shouldn't be a mess.</strong>
</p>
