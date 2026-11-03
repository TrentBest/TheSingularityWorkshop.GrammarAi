<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# GrammarAI Examples

These examples demonstrate the intended consumption pattern: **reference an externally owned vocabulary, define structure around it, and keep execution outside the grammar.**

## Example 1 — One protocol symbol

Assume another system owns:

```text
[1001] People
[2001] Bob
```

GrammarAI can reference Bob:

```csharp
var bob = GrammarSymbol.Terminal(
    new GrammarProtocolReference(1001, 2001));
```

The grammar now has a terminal whose meaning belongs to protocol 1001.

## Example 2 — Alternatives

A grammar can allow different protocol symbols in the same structural position:

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

The structure is:

```text
[4001]
  |
  +--> [1001:2001]
  |
  +--> [1001:2002]
```

The grammar owns the alternatives.

ProtocolAI owns the meaning of each terminal.

## Example 3 — Nested structure

Nonterminals let the grammar build relationships:

```csharp
var grammar = new GrammarBuilder(
        3001,
        "WorkshopAction",
        4001)
    .Rule(
        5001,
        4001,
        GrammarSymbol.NonTerminal(4002))
    .Rule(
        5002,
        4002,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(7100, 7101)))
    .Build();
```

Conceptually:

```text
[4001] WorkshopAction
   |
   v
[4002]
   |
   v
[7100:7101]
```

This is a structural relationship, not an execution instruction.

## Example 4 — Combining independent protocols

Imagine:

```text
Protocol A
[7100] Commands
[7101] move

Protocol B
[7200] Objects
[7201] forge
```

A grammar can reference both:

```csharp
var grammar = new GrammarBuilder(
        7300,
        "MoveCommand",
        7400)
    .Rule(
        7501,
        7400,
        GrammarSymbol.NonTerminal(7401),
        GrammarSymbol.NonTerminal(7402))
    .Rule(
        7502,
        7401,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(7100, 7101)))
    .Rule(
        7503,
        7402,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(7200, 7201)))
    .Build();
```

The start production expands to an ordered pair: the action terminal from Protocol A followed by the station terminal from Protocol B. The grammar references both independently owned vocabularies without copying their meanings.

This remains a structural description, not an executable command. The host decides what the pair means, whether it is permitted, and whether any action may be taken.

## Example 5 — Self-description

```csharp
Console.WriteLine(grammar.Describe());
```

Example:

```text
[7300] MoveCommand start=[7400]
  rule [7501] [7400] -> [7100:7101]
  rule [7502] [7400] -> [7200:7201]
```

The grammar is therefore inspectable as data.

## Example 6 — The WHAT → HOW pipeline

The two packages can be understood together:

```text
                 DOMAIN
                   |
                   v
          +----------------+
          |   ProtocolAI   |
          |      WHAT      |
          +-------+--------+
                  |
          [protocol:symbol]
                  |
                  v
          +----------------+
          |   GrammarAI    |
          |      HOW       |
          +-------+--------+
                  |
             productions
                  |
                  v
            host / adapter
                  |
                  v
                model
```

This is the central consumption story.

## Example 7 — A provider-neutral boundary

The grammar should remain independent of whichever model host eventually consumes it:

```text
                    GrammarAI
                        |
                        v
                 abstract grammar
                  /      |      \\
                 /       |       \\
                v        v        v
           provider A  provider B  validator
                \\       |       /
                 \\      |      /
                    host
```

A future adapter can translate the abstract grammar into a concrete provider representation.

That adapter is deliberately not part of the current alpha.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>

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
