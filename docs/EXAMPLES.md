# GrammarAI Examples

These examples demonstrate the intended consumption pattern: **reference an externally owned vocabulary, define structure around it, and keep execution outside the grammar.**

## Example 1 — One protocol symbol

Assume another system owns:

`text
[1001] People
[2001] Bob
`

GrammarAI can reference Bob:

`csharp
var bob = GrammarSymbol.Terminal(
    new GrammarProtocolReference(1001, 2001));
`

The grammar now has a terminal whose meaning belongs to protocol 1001.

## Example 2 — Alternatives

A grammar can allow different protocol symbols in the same structural position:

`csharp
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
`

The structure is:

`text
[4001]
  |
  +--> [1001:2001]
  |
  +--> [1001:2002]
`

The grammar owns the alternatives.

ProtocolAI owns the meaning of each terminal.

## Example 3 — Nested structure

Nonterminals let the grammar build relationships:

`csharp
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
`

Conceptually:

`text
[4001] WorkshopAction
   |
   v
[4002]
   |
   v
[7100:7101]
`

This is a structural relationship, not an execution instruction.

## Example 4 — Combining independent protocols

Imagine:

`text
Protocol A
[7100] Commands
[7101] move

Protocol B
[7200] Objects
[7201] forge
`

A grammar can reference both:

`csharp
var grammar = new GrammarBuilder(
        7300,
        "MoveCommand",
        7400)
    .Rule(
        7501,
        7400,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(7100, 7101)))
    .Rule(
        7502,
        7400,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(7200, 7201)))
    .Build();
`

The resulting artifact can describe relationships between independently owned vocabularies.

The current alpha does not define the semantics of combining those rules into an executable command. That belongs to a host layer.

## Example 5 — Self-description

`csharp
Console.WriteLine(grammar.Describe());
`

Example:

`text
[7300] MoveCommand start=[7400]
  rule [7501] [7400] -> [7100:7101]
  rule [7502] [7400] -> [7200:7201]
`

The grammar is therefore inspectable as data.

## Example 6 — The WHAT → HOW pipeline

The two packages can be understood together:

`text
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
`

This is the central consumption story.

## Example 7 — A provider-neutral boundary

The grammar should remain independent of whichever model host eventually consumes it:

`text
                    GrammarAI
                        |
                        v
                 abstract grammar
                  /      |      \
                 /       |       \
                v        v        v
           provider A  provider B  validator
                \       |       /
                 \      |      /
                    host
`

A future adapter can translate the abstract grammar into a concrete provider representation.

That adapter is deliberately not part of the current alpha.
