<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Chapter 4 — Structural validation and ownership boundaries

## Learning objectives

Separate internal grammar checks from external reference resolution, candidate conformance, and application authorization.

## 1. Four different questions

| Question | Responsible boundary |
|---|---|
| Is the grammar internally well-formed? | GrammarAi construction checks |
| Do external protocol references resolve? | Host-side linking against selected protocol definitions |
| Does a candidate response conform to the grammar? | A parser or validator that checks that candidate |
| Is the resulting action permitted and safe? | Host policy and application logic |

Passing an earlier question does not imply passing a later one.

## 2. What construction checks

The current definition enforces nonzero identities, a nonblank bounded name, bounded rule and symbol counts, unique rule IDs, a start symbol with a production, and productions for every referenced nonterminal. Rule right-hand sides and the final rule collection are immutable snapshots.

These checks establish internal structure. They do not load external protocol files or verify that a terminal's protocol and symbol IDs exist in a particular protocol version.

## 3. External protocol ownership

A terminal reference is a pair: protocol identity plus symbol identity. GrammarAi stores that reference without taking ownership of the protocol vocabulary. The host must resolve references against the exact protocol definitions it intends to use.

This avoids a hidden dependency on a protocol registry or a provider-specific service inside the grammar package.

## 4. Candidate validation is downstream

A \`GrammarDefinition\` is not a parser. It does not accept arbitrary generated text and return a conformance result. A consumer must use an actual parser, validator, or provider integration if candidate conformance is required.

Likewise, a candidate that conforms to a grammar may still be semantically wrong or unsafe. The host must apply its own policy and authorization checks before any consequential operation.

## 5. Resource bounds

The public \`GrammarLimits\` constants define current construction bounds:

| Limit | Current value |
|---|---:|
| Name length | 256 UTF-16 characters |
| Rules per grammar | 4,096 |
| Symbols per production | 1,024 |
| Aggregate right-hand symbols | 65,536 |

These are implementation limits, not claims about formal grammar complexity or provider limits.

## Evidence in the repository

- Structural checks and immutable rule snapshot: \`GrammarDefinition.cs\`.
- Input checks and builder behavior: \`GrammarBuilder.cs\` and \`GrammarRule.cs\`.
- External identity representation: \`GrammarProtocolReference.cs\`.
- Limits: \`GrammarLimits.cs\`.
- Tests: \`Referenced_nonterminals_must_have_productions\`, \`Start_symbol_must_have_a_production\`, \`Duplicate_rule_ids_are_rejected\`, \`Grammar_definition_and_production_inputs_are_bounded\`, and \`Grammar_builder_bounds_aggregate_production_symbols\` in \`tests/GrammarAi.Tests/GrammarDefinitionTests.cs\`.

## Check your understanding

A terminal reference can be structurally valid while pointing to a symbol that the host cannot resolve. That is a linking failure, not proof that GrammarAi owns or should silently repair the external vocabulary.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
