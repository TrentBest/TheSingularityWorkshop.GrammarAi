# TheSingularityWorkshop.GrammarAi

**Self-defining grammar structures for connecting integer-backed AI protocols.**

GrammarAI is the **structure layer** above ProtocolAI.

ProtocolAI answers:

> **What is this?**

GrammarAI answers:

> **How can these things be connected?**

A tool can define its vocabulary in ProtocolAI and then define how those protocol symbols may be composed. The grammar itself is integer-backed and self-describing.

## Core model

```text
ProtocolAI
    |
    | lexicon / values
    v
GrammarAI
    |
    | productions / structure
    v
AI-facing host
    |
    v
LLM
```

A grammar contains:

- an integer grammar identity;
- an integer start symbol;
- integer nonterminals;
- production rules;
- references to terminal symbols supplied by ProtocolAI.

This is deliberately close to the traditional idea of a grammar: a production connects a left-hand-side nonterminal to an ordered sequence of terminals and/or nonterminals.

## Example

Suppose ProtocolAI defines:

```text
[1001] People

[2001] bobId = "Bob"
[2002] janeId = "Jane"
```

GrammarAI can define a structure that connects those symbols:

```csharp
var grammar = new GrammarBuilder(3001, "Greeting", 4001)
    .Rule(
        5001,
        4001,
        GrammarSymbol.Terminal(new ProtocolReference(1001, 2001)))
    .Rule(
        5002,
        4001,
        GrammarSymbol.Terminal(new ProtocolReference(1001, 2002)))
    .Build();
```

The grammar can describe itself:

```text
[3001] Greeting start=[4001]
  rule [5001] [4001] -> [1001:2001]
  rule [5002] [4001] -> [1001:2002]
```

The important architectural point is that **GrammarAI does not own the People vocabulary**. It references the protocol that owns it.

## Self-definition

The package is intended for tools that construct their own AI-facing nomenclature and structure at runtime.

```text
tool
 |
 +-- defines vocabulary --------> ProtocolAI
 |
 +-- defines structure ---------> GrammarAI
 |
 +-- supplies both to ----------> AI host
                                  |
                                  v
                                 LLM
```

That means a new tool does not need a hard-coded global grammar catalog. It can define the words, values, and then the rules connecting them.

## Design boundaries

GrammarAI owns:

- grammar identity;
- start-symbol identity;
- integer nonterminals;
- production rules;
- references to ProtocolAI symbols;
- deterministic grammar descriptions.

GrammarAI does **not** own:

- an LLM client;
- model selection;
- tokenization;
- prompt transport;
- protocol vocabularies;
- MicroBundle hosting;
- REST/OpenAPI;
- GUI manifestation.

Those belong to other layers.

## Development

```bash
dotnet restore TheSingularityWorkshop.GrammarAi.slnx
dotnet build TheSingularityWorkshop.GrammarAi.slnx --configuration Release
dotnet test ../TheSingularityWorkshop.GrammarAi.Tests/TheSingularityWorkshop.GrammarAi.Tests.csproj --configuration Release
dotnet pack TheSingularityWorkshop.GrammarAi.csproj --configuration Release --output ./artifacts
```

## License

MIT. See LICENSE.txt.

---

## Resources & Support

- Core NuGet: https://www.nuget.org/packages/TheSingularityWorkshop.GrammarAi
- Source Code: https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi
- ProtocolAI: https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi
- FSM_API: https://www.nuget.org/packages/TheSingularityWorkshop.FSM_API
- MicroBundleDomain: https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain

<p align="center">
  <a href="https://github.com/TrentBest/FSM_API">
    <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" height="200">
  </a>
</p>

<p align="center">
  <em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br>
  <strong>Because state shouldn't be a mess.</strong>
</p>
