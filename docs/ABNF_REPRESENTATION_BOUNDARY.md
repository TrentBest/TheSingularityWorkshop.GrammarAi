<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# ABNF representation boundary

**Status:** immutable expression model and bounded structural validator implemented on `development`; source importer, parser/recognizer, lowering adapter, and HTTP grammar are not implemented.

## Decision

Do not lower Internet-protocol ABNF directly into the existing `GrammarDefinition` CFG model and call the result equivalent. The current CFG model remains useful for production resources whose terminals have already been classified by a host. ABNF needs a distinct, typed expression representation if GrammarAi is to preserve the source formalism faithfully.

The first deliverable is an immutable ABNF syntax/semantic model and bounded validation contract. That model is now implemented and covered by focused tests. A source-text importer is a separate later adapter. An HTTP grammar resource should be built only after that model has explicit conformance tests.

## Why the existing CFG is not a lossless ABNF representation

The current production rule stores a left-hand nonterminal and an ordered sequence of terminal/nonterminal symbols. Multiple rules express alternatives and an empty right-hand side expresses epsilon. It has no native representation for:

- bounded or unbounded repetition as a first-class operator;
- ABNF numeric octet values, ranges, or sequences;
- case-insensitive versus case-sensitive character values;
- case-insensitive rule-name resolution;
- incremental alternatives (`=/`) and their rule-assembly order;
- opaque prose values, which are descriptive and cannot be mechanically recognized;
- the distinction between octet-oriented terminals and Unicode text.

Some ABNF constructs can be expanded into CFG productions, but expansion can be large or unbounded and can erase source-level distinctions. A conversion is acceptable only when its preconditions and semantics are explicit; it must never be the default interpretation.

## Standards profile

The initial profile should be explicitly pinned to:

- RFC 5234 core ABNF syntax and core rule definitions;
- RFC 7405 case-sensitive and case-insensitive string extensions.

The profile must identify the standard revision and the exact supported feature set. HTTP grammar resources must separately pin the relevant HTTP specification and protocol version. URI syntax remains its own RFC 3986 resource; it is not silently redefined as HTTP grammar.

## Proposed typed representation

The public API in `AbnfGrammarDefinition.cs` implements the following type shape. The compact record-style sketch below is explanatory; the actual API uses constructor-validated sealed classes with immutable read-only properties.

~~~csharp
public enum AbnfRuleOperator : byte
{
    Define = 0,          // =
    IncrementalDefine = 1 // =/
}

public abstract record AbnfExpression;

public sealed record AbnfAlternation(
    IReadOnlyList<AbnfExpression> Choices) : AbnfExpression;

public sealed record AbnfConcatenation(
    IReadOnlyList<AbnfExpression> Elements) : AbnfExpression;

public sealed record AbnfRepetition(
    uint Minimum,
    uint? Maximum,
    AbnfExpression Element) : AbnfExpression;

public sealed record AbnfRuleReference(string Name) : AbnfExpression;

public sealed record AbnfCharacterValue(
    string Value,
    AbnfCharacterCase CaseMode) : AbnfExpression;

public enum AbnfNumericForm : byte
{
    Single = 0,
    Sequence = 1,
    Range = 2
}

public sealed record AbnfNumericValue(
    AbnfNumericBase Base,
    AbnfNumericForm Form,
    IReadOnlyList<byte> Values) : AbnfExpression;

public sealed record AbnfProseValue(string Description) : AbnfExpression;
~~~

The implementation uses defensive immutable collection copies, validates enum values, ASCII names and character values, value lengths, nesting depth, expression-node counts, and repetition bounds. `AbnfGroup` and `AbnfOption` preserve explicit source structure. `AbnfLimits` publishes the finite budgets. `AbnfGrammarDefinition.Validate()` checks case-insensitive rule linking, initial/incremental definition ordering, duplicate initial definitions, unresolved references, and expression budgets. The validator does not parse source text, recognize input, or interpret prose values.

### Required semantic distinctions

| ABNF construct | Required representation |
|---|---|
| Rule name | ASCII ABNF rule identifier, resolved case-insensitively |
| `=` | Initial rule definition |
| `=/` | Ordered incremental alternative append; only legal after a prior definition |
| `/` | Ordered alternation |
| Whitespace-separated elements | Ordered concatenation |
| `n element` | Exact repetition n..n |
| `n*m element` | Bounded repetition; either bound may be omitted |
| `* element` | Repetition 0..unbounded |
| `[ elements ]` | Optional expression, equivalent in recognition to 0..1 repetition |
| Default quoted string | Case-insensitive ASCII character value under RFC 5234 |
| RFC 7405 `%i` / `%s` | Explicit case-insensitive / case-sensitive character value |
| `%b`, `%d`, `%x` | Numeric octet sequence or inclusive range, with base retained |
| `<prose>` | Opaque, non-executable prose annotation; not a recognizer |

Rule references must be linked case-insensitively while preserving the source spelling for diagnostics. Rule names follow the ABNF ASCII rule-name shape: an initial letter followed by letters, digits, or hyphens. Incremental alternatives must be accumulated in source order, and a `=/` without a preceding definition must be rejected. Core rules are ordinary named definitions supplied by the selected profile, not hard-coded special cases in the expression evaluator.

Alternation is language union, not PEG-style prioritized choice. Preserve source order for deterministic descriptions and provenance, but never give an earlier ABNF alternative a parsing priority that changes recognition. A numeric value must have exactly one form: a single octet, a nonempty sequence of octets, or an inclusive two-endpoint range. Do not mix sequence and range forms in one numeric value.

Numeric values are octets (0..255), not Unicode scalar values. A range is inclusive and must have exactly two ordered endpoints. A numeric sequence is an exact ordered octet sequence and must be nonempty. A single numeric value has exactly one octet. Character-value encoding and matching must be explicitly ASCII/octet-based; no implicit UTF-8 or Unicode normalization is permitted.

## Bounds and validation

The first implementation should define finite limits for:
- rule count and total expression nodes;
- rule-name and character/prose-value lengths;
- maximum expression nesting depth;
- alternatives and concatenation elements per expression;
- repetition bounds and numeric components;
- imported source size and line length (for a future importer).

An omitted repetition maximum means unbounded language repetition; it does not authorize unbounded evaluation or expansion. Any recognizer/normalizer must apply independent execution and output limits.

Structural validation should distinguish:
1. malformed ABNF representation (invalid names, empty required lists, invalid bounds, invalid octet values);
2. invalid rule assembly (duplicate initial definitions, incremental definition before initial definition);
3. unresolved rule references;
4. profile-unsupported syntax;
5. prose values that are intentionally opaque.

Validation must not pretend to evaluate prose values. The host or a domain-specific adapter may attach an explicit interpretation, but GrammarAi must not infer one.

## Lowering to the existing CFG model

A future normalization adapter may lower a subset to `GrammarDefinition` only when semantics are preserved:

- alternation and concatenation map naturally to alternatives and ordered productions;
- epsilon is representable;
- finite repetition can be expanded only under a declared expansion budget;
- unbounded repetition can be represented recursively, but the resulting grammar must retain the same terminal alphabet and recognition semantics;
- optionality may be represented by epsilon plus the inner expression.

Do not lower automatically when the target cannot preserve:
- octet-level matching;
- case sensitivity;
- ABNF rule-name equivalence;
- unresolved/prose values;
- profile-specific lexical rules.

The lowering result must carry provenance to the source ABNF profile and identify every conversion decision. A normalized CFG is a derived artifact, not the canonical ABNF definition.

## HTTP scope: separate syntax layers

Do not treat “HTTP grammar” as one universal grammar.

1. **HTTP field syntax:** begin with a narrowly selected RFC 9110 field-value/common syntax slice. Preserve OWS/RWS/BWS distinctions and byte-oriented field-value constraints. Do not infer field semantics or header-specific validity from ABNF alone.
2. **HTTP/1.1 message syntax:** treat RFC 9112 request/status lines, field sections, and message framing as a separately versioned resource. Framing and body interpretation include rules that cannot be reduced to a generic field grammar.
3. **HTTP/2 and HTTP/3:** do not reuse HTTP/1.1 textual message grammar for their binary framing. They require separate protocol-specific resources and host-owned decoding.

The URI-reference grammar remains a separate reusable component. HTTP request-target forms may reference URI components, but that composition must be specified by the HTTP version's own grammar and semantics.

## First implementation sequence

1. Freeze the representation contract and public API naming after review.
2. Implement the immutable ABNF expression model and bounded structural validation only; no source importer or parser.
3. Add tests for each expression kind, rule-name case-insensitivity, incremental alternatives, exact/ranged/unbounded repetition, octet boundaries, RFC 7405 string modes, malformed bounds, unknown references, limits, and deterministic description.
4. Add a small pinned RFC 9110 field-syntax resource only after the representation model is tested against the upstream ABNF text.
5. Add HTTP/1.1 message syntax as a separate resource with its own fixtures and version provenance.
6. Keep parsing, source locations, tokenization, header-specific semantics, message framing decisions, and execution outside the core representation.

## Ownership

- ProtocolAi continues to own shared vocabulary/category/member identities.
- GrammarAi owns the formal ABNF expression representation and structural linking/validation.
- A future importer owns source-text interpretation and provenance capture.
- A future recognizer/parser is a separate capability with independent resource limits.
- The host owns HTTP semantics, request context, trust, policy, authorization, and execution.

This design adds no ProtocolAi dependency and does not expand GrammarAi into a compiler or network stack.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
