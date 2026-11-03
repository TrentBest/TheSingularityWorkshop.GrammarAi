<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Chapter 2 — The grammar model

## Learning objectives

Identify the grammar's core data, explain the role of a start nonterminal, and read a production rule.

## 1. The definition

A \`GrammarDefinition\` is an immutable, self-describing grammar. Its principal data are a nonzero grammar identity, a human-readable name, a nonzero start nonterminal, and an ordered collection of production rules.

The start symbol identifies the nonterminal from which a consumer begins interpreting the grammar. The definition stores rules in declaration order and exposes them as a read-only collection.

## 2. A production

A production has a left-hand nonterminal and an ordered right-hand sequence:

\`\`\`text
rule 5001: [4001] -> [1001:2001] [1002:3001]
\`\`\`

The left side is the nonterminal being expanded. The right side is the sequence produced by that rule. Here the first terminal reference precedes the second.

A \`GrammarRule\` is one production. Multiple rules may share a left-hand side; those rules describe alternative productions.

## 3. Symbol kinds

GrammarAi currently models two symbol kinds:

| Kind | Meaning | Identity owner |
|---|---|---|
| Nonterminal | A grammar-local structural node that must have a production | Grammar |
| Protocol terminal | A reference to an external protocol symbol | Referenced protocol |

A \`GrammarSymbol\` exposes whether it is a nonterminal or a protocol symbol. A protocol terminal carries a \`GrammarProtocolReference\` containing both protocol ID and symbol ID.

## 4. Builder and immutable snapshot

\`GrammarBuilder\` is the construction surface. Calls to \`Rule\` accumulate productions; \`Build\` validates the accumulated structure and returns a \`GrammarDefinition\`.

The built definition is independent of later additions to the builder. A consumer can retain a built artifact without that artifact changing when the original builder is reused.

## Evidence in the repository

- Data model: \`GrammarDefinition.cs\`, \`GrammarRule.cs\`, \`GrammarSymbol.cs\`, \`GrammarProtocolReference.cs\`.
- Construction API: \`GrammarBuilder.cs\`.
- Tests: \`Builder_snapshot_is_immutable_after_subsequent_builder_changes\`, \`Builder_connects_nonterminals_to_external_protocol_symbols\`, and \`Description_is_deterministic\` in \`tests/GrammarAi.Tests/GrammarDefinitionTests.cs\`.
- Diagrams: [GrammarDefinition UML](../../GrammarDefinitionUML.md) and [GrammarBuilder UML](../../GrammarBuilderUML.md).

## Check your understanding

Given \`[4001] -> [4002] [4003]\`, the right side is one ordered sequence containing two nonterminals. It is not two alternative rules. A second rule beginning with \`[4001]\` would add an alternative.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
