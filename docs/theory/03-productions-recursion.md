<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Chapter 3 — Productions, recursion, and reachability

## Learning objectives

Read alternatives and sequences correctly, distinguish epsilon from a missing production, and understand what recursion and reachability analysis mean in the current implementation.

## 1. Alternatives and sequences

Two productions with the same left-hand side are alternatives:

\`\`\`text
[4001] -> [1001:2001]
[4001] -> [1001:2002]
\`\`\`

One production with two right-hand symbols is a sequence:

\`\`\`text
[4001] -> [1001:2001] [1002:3001]
\`\`\`

Do not read the first example as “both symbols.” Each rule is a separate way to expand the same nonterminal.

## 2. Epsilon

An empty right-hand side is an explicit epsilon production:

\`\`\`text
[4001] -> ε
\`\`\`

It means the production contributes an empty sequence. It is different from a nonterminal that has no production at all. The former is an intentional rule; the latter is an incomplete structural reference and is rejected during construction.

## 3. Recursive productions

A production may refer to its own left-hand nonterminal, or to another nonterminal that eventually refers back to it. The current structural model permits direct and mutual recursion. This is a statement about representing relationships; it does not claim that GrammarAi implements a parser or guarantees termination for a consumer that later processes recursive structures.

## 4. Reachability analysis

\`GrammarDefinition.FindUnreachableNonTerminals()\` starts at the declared start symbol and follows nonterminal references through every production. It reports nonterminals that are not reached.

The result is an analysis finding, not a construction error. A grammar may intentionally retain reusable fragments that are not reachable from its current start symbol. Findings are returned in first-declaration order, and analysis does not mutate the grammar.

The implementation uses an iterative queue and a visited set. This makes the graph walk cycle-safe for direct and mutual recursion.

## Evidence in the repository

- Production and epsilon representation: \`GrammarRule.cs\` and \`GrammarDefinition.Describe()\`.
- Reachability implementation: \`GrammarDefinition.FindUnreachableNonTerminals()\` in \`GrammarDefinition.cs\`.
- Tests in \`tests/GrammarAi.Tests/GrammarDefinitionTests.cs\`:
  - \`Empty_production_is_explicit_epsilon\`
  - \`Recursive_nonterminals_are_allowed_as_structure\`
  - \`Reachability_follows_alternatives_and_ordered_sequences\`
  - \`Reachability_terminates_for_mutually_recursive_nonterminals\`
  - \`Unreachable_nonterminals_are_reported_in_first_declaration_order\`
  - \`Epsilon_start_production_has_no_unreachable_nonterminals\`

## Check your understanding

An unreachable nonterminal is not necessarily malformed. It may be an intentional fragment. A nonterminal referenced by a production but absent from every left-hand side, however, violates the current construction invariant.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
