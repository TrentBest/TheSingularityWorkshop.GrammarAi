<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# GrammarAi Gallery

GrammarAi's visual explains the boundary between protocol-owned vocabulary and grammar-owned structure: ProtocolAi answers what a symbol means, while GrammarAi describes how symbol identities may be composed. The current SVG is an authored explanatory diagram, not source-derived UML or a live runtime trace.

## Visual tour

### Give meaning a language

![GrammarAi architecture: ProtocolAi identities become structured through GrammarAi](images/grammar-ai-money-shot.svg)

The diagram shows protocol terminals, grammar rules, and the host-side consumer. GrammarAi owns production structure; it does not own external vocabularies, call a model, or execute the resulting content.

### Exchange composition (design direction)

![GrammarAi AI Exchange boundary — design illustration, not a shipped exchange API](images/ai-exchange-stack.svg)

This is a conceptual view preserved from the exchange-foundation work. Provider adapters, response parsing, and the exchange orchestration are not implemented by GrammarAi itself.

## See actual behavior

From the repository root:

```bash
dotnet run --project examples/GrammarAi.QuickStart/GrammarAi.QuickStart.csproj
dotnet test tests/GrammarAi.Tests/GrammarAi.Tests.csproj --configuration Release
```

The quick start uses the public API. Tests exercise protocol-terminal references, nonterminal validation, recursive productions, epsilon, deterministic descriptions, and immutable snapshots. The SVG is the orientation aid; the example and tests are the evidence.

## Detailed recipe

Read [Examples](EXAMPLES.md), [Consuming GrammarAi](CONSUMING.md), [Theory](THEORY.md), and [Release Readiness](RELEASE_READINESS.md).

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
