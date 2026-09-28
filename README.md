# TheSingularityWorkshop.GrammarAi

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NuGet version](https://img.shields.io/nuget/v/TheSingularityWorkshop.GrammarAi?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.GrammarAi)
[![NuGet downloads](https://img.shields.io/nuget/dt/TheSingularityWorkshop.GrammarAi?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.GrammarAi)
[![Build](https://img.shields.io/github/actions/workflow/status/TrentBest/TheSingularityWorkshop.GrammarAi/verify.yml?branch=master&style=flat-square&logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/actions/workflows/verify.yml)
[![Code Coverage](https://img.shields.io/codecov/c/github/TrentBest/TheSingularityWorkshop.GrammarAi?style=flat-square)](https://codecov.io/gh/TrentBest/TheSingularityWorkshop.GrammarAi)

**The structure layer for self-defining, integer-backed AI protocols.**

ProtocolAI answers:

> **What is this?**

GrammarAI answers:

> **How can these things be connected?**

The package explores a deliberately small idea: a tool can define a vocabulary independently from the rules that describe how that vocabulary may be composed.

That separation matters because modern AI systems are moving toward structured generation and grammar-constrained interfaces. GrammarAI asks what it looks like when the **grammar itself is a self-describing, integer-addressed artifact** rather than a provider-specific configuration.

---

## WHAT → HOW

The conceptual split is:

```text
             +----------------+
             |      Tool      |
             +--------+-------+
                      |
               defines vocabulary
                      |
                      v
             +----------------+
             |   ProtocolAI   |
             |      WHAT      |
             +--------+-------+
                      |
              symbol references
                      |
                      v
             +----------------+
             |   GrammarAI    |
             |       HOW      |
             +--------+-------+
                      |
                      v
                 AI-facing host
                      |
                      v
                     LLM
```

ProtocolAI owns the vocabulary.

GrammarAI owns the structure.

The LLM remains outside both packages.

---

## Why grammar belongs here

A vocabulary can tell us:

```text
[1001] People

[2001] bobId  = "Bob"
[2002] janeId = "Jane"
```

But that does not tell us how those symbols may be arranged.

A grammar can say:

```text
[3001] Greeting
  start = [4001]

  [5001] [4001] -> [1001:2001]
  [5002] [4001] -> [1001:2002]
```

The grammar owns the **relationships**.

The protocol owns the **meaning**.

---

## A grammar is a structure, not a vocabulary

GrammarAI deliberately references protocol symbols instead of copying their definitions.

```text
ProtocolAI
    |
    | [1001:2001] = Bob
    |
    v
GrammarAI
    |
    | [4001] -> [1001:2001]
    |
    v
structured protocol
```

That keeps ownership clear.

If the People vocabulary changes, GrammarAI does not silently become the owner of People.

It continues to reference the protocol that owns the symbol.

---

## The API

```csharp
var grammar = new GrammarBuilder(3001, "Greeting", 4001)
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

The definition describes itself:

```text
[3001] Greeting start=[4001]
  rule [5001] [4001] -> [1001:2001]
  rule [5002] [4001] -> [1001:2002]
```

The important part is not the syntax.

It is the ownership boundary:

```text
[1001:2001]
    |
    +--> ProtocolAI owns the symbol

[4001]
    |
    +--> GrammarAI owns the nonterminal
```

---

## Nonterminals and terminals

GrammarAI distinguishes between two kinds of symbol:

### Nonterminal

An integer-backed grammar symbol that must have a production rule.

```text
[4001]
```

### Protocol terminal

A symbol supplied by an external protocol.

```text
[1001:2001]
```

This produces a simple composition model:

```text
Grammar
 |
 +-- nonterminal
 |      |
 |      +--> another nonterminal
 |
 +-- protocol terminal
        |
        +--> external protocol symbol
```

The grammar defines structure without importing the external vocabulary.

---

## Why integer-backed grammar?

Grammar identity is explicit:

```text
[3001] Greeting
```

Its start symbol is explicit:

```text
start = [4001]
```

Its rules are explicit:

```text
[5001] [4001] -> ...
```

And protocol terminals retain both identities:

```text
[1001:2001]
```

That makes the grammar itself a data structure that can be described, serialized, inspected, compared, or eventually supplied to another composition layer.

---

## Modern structured AI context

Structured model output is now a mainstream application pattern. Current OpenAI documentation describes schema-adherent Structured Outputs, and its function-calling documentation includes context-free grammars for constraining custom tool output. [OpenAI Structured Outputs](https://developers.openai.com/api/docs/guides/structured-outputs) and [Function Calling](https://developers.openai.com/api/docs/guides/function-calling)

GrammarAI is not an implementation of a model provider's grammar format.

It is an independent C# representation of the structural layer.

That distinction is intentional.

A provider adapter can eventually translate a GrammarAI definition into whatever representation a target model host requires.

The grammar itself should not need to know which model provider will consume it.

---

## Self-defining structure

The same philosophy used by ProtocolAI applies here.

A tool should be able to define:

1. its vocabulary;
2. its nonterminals;
3. its productions;
4. its references to external protocol symbols.

The package should not contain a universal catalog of commands or domains.

```text
Tool
 |
 +-- ProtocolAI ----> WHAT
 |
 +-- GrammarAI  ----> HOW
 |
 +-- Host        ----> EXECUTION
```

That is the architectural seam.

---

## GrammarAI does not own the model

GrammarAI does not own:

- an LLM client;
- model selection;
- tokenization;
- inference;
- prompt transport;
- protocol vocabularies;
- MicroBundle hosting;
- REST/OpenAPI;
- GUI manifestation;
- tool execution.

It owns the structural description.

---

## Current alpha boundary

**Version: `0.1.0-alpha.1`**

The current release establishes:

- grammar identity;
- integer start symbols;
- integer nonterminals;
- ordered production rules;
- external protocol references;
- deterministic self-description;
- validation of referenced nonterminals.

It does not yet establish:

- grammar parsing;
- grammar compilation;
- provider-specific grammar export;
- constrained decoding;
- protocol negotiation;
- grammar version negotiation;
- execution semantics.

Those are future composition questions.

See:

- [GrammarAI Theory](docs/THEORY.md)
- [GrammarAI Reflection](docs/REFLECTION.md)

---

## The Workshop stack

GrammarAI sits above the lexicon boundary and below execution:

```text
Domain meaning
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
protocol / provider adapter
      |
      v
tool host
      |
      v
FSM / GUI / Experience
```

This keeps the grammar reusable.

The same grammar can eventually be translated for different execution environments without putting those environments into the grammar package.

---

## Development

```bash
dotnet restore TheSingularityWorkshop.GrammarAi.slnx
dotnet build TheSingularityWorkshop.GrammarAi.slnx --configuration Release
dotnet test tests/GrammarAi.Tests/GrammarAi.Tests.csproj --configuration Release
dotnet pack TheSingularityWorkshop.GrammarAi.csproj --configuration Release --output ./artifacts
```

A manual verification workflow builds, tests with coverage, and packs the NuGet artifact.

---

## Documentation

- [Theory](docs/THEORY.md)
- [Reflection](docs/REFLECTION.md)

---

## License

MIT. See [LICENSE.txt](LICENSE.txt).

---

## 🔗 Resources & Support

- **NuGet:** [TheSingularityWorkshop.GrammarAi](https://www.nuget.org/packages/TheSingularityWorkshop.GrammarAi)
- **Source:** [GitHub](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi)
- **Theory:** [docs/THEORY.md](docs/THEORY.md)
- **Reflection:** [docs/REFLECTION.md](docs/REFLECTION.md)
- **ProtocolAI:** [TheSingularityWorkshop.ProtocolAi](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi)
- **FSM_API:** [TheSingularityWorkshop.FSM_API](https://www.nuget.org/packages/TheSingularityWorkshop.FSM_API)
- **MicroBundleDomain:** [TheSingularityWorkshop.MicroBundleDomain](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain)

<p align="center">
  <a href="https://github.com/TrentBest/FSM_API">
    <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" height="200">
  </a>
</p>

<p align="center">
  <em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br>
  <strong>Because state shouldn't be a mess.</strong>
</p>
