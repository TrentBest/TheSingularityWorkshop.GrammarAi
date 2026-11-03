<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Chapter 1 — From vocabulary to language

## Learning objectives

Distinguish a vocabulary from a grammar, explain why a grammar is a separate artifact, and identify the ownership boundary between ProtocolAi and GrammarAi.

## 1. A vocabulary answers “what?”

A protocol vocabulary gives identities to concepts. For example:

\`\`\`text
Protocol 1001 — People
  2001 — Bob
  2002 — Jane
\`\`\`

Those identifiers are meaningful only within the protocol that owns them. GrammarAi does not redefine those meanings.

## 2. A grammar answers “how?”

A grammar describes how symbols may be arranged:

\`\`\`text
[4001] -> [1001:2001]
[4001] -> [1001:2002]
\`\`\`

The two rules are alternatives for nonterminal \`4001\`. They are not a sequence containing both people. A sequence is expressed on one right-hand side:

\`\`\`text
[4001] -> [1001:2001] [1001:2002]
\`\`\`

The order of symbols in that sequence is significant.

## 3. Why keep the artifacts separate?

ProtocolAi owns the identity and meaning of protocol symbols. GrammarAi owns grammar identities, nonterminals, production rules, and the arrangement of references to external protocol symbols. This separation lets a grammar refer to a vocabulary without copying or taking ownership of it.

The host remains responsible for connecting those identities to application meaning, policy, and behavior.

## 4. What this does—and does not—guarantee

A grammar makes an intended structure explicit and inspectable. It does not make a model's intent correct, prove that a referenced symbol is appropriate, parse arbitrary text, or authorize an action. Those are separate responsibilities.

A host may use a grammar as input to a parser, validator, or provider adapter, but the current GrammarAi core is a structural representation rather than those downstream systems.

## Evidence in the repository

- Public types: \`GrammarDefinition.cs\`, \`GrammarBuilder.cs\`, \`GrammarSymbol.cs\`, and \`GrammarProtocolReference.cs\`.
- External-reference example: \`Builder_connects_nonterminals_to_external_protocol_symbols\` in \`tests/GrammarAi.Tests/GrammarDefinitionTests.cs\`.
- Type contracts: [GrammarDefinition](../../GrammarDefinition.md), [GrammarSymbol](../../GrammarSymbol.md), and [GrammarProtocolReference](../../GrammarProtocolReference.md).

## Check your understanding

1. Which artifact owns the meaning of a protocol symbol?
2. What is the difference between two rules with the same left-hand side and two symbols on one right-hand side?
3. Does a structurally valid grammar authorize an operation?

Answers: ProtocolAi owns the symbol meaning; separate rules are alternatives while symbols in one right-hand side form an ordered sequence; and no, authorization belongs to the host.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
