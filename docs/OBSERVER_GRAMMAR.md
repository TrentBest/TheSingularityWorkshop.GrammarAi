<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# Observer condition and gate-intent grammar

## Purpose

GrammarAi's observer grammar is a versioned, bounded data model for describing when a named success intent may be requested. It is deliberately not a workflow engine, policy engine, evidence verifier, or command executor.

The first schema is:

- Schema: `TheSingularityWorkshop.GrammarAi.Observer`
- Version: `1`

It is an observer-specific model alongside the existing general grammar engine. It does not replace or change `GrammarDefinition`, `GrammarBuilder`, or external protocol terminal references.

## Ownership boundaries

| Concern | Owner |
|---|---|
| Event envelope, event-kind and typed payload identities, correlation, candidate provenance | ProtocolAi |
| Observer condition/action structure and deterministic structural validation | GrammarAi |
| Evidence production, evidence authenticity checks, CI verdicts | CiAi |
| Atomic command proposal representation | CommandAi |
| Durable observer lifecycle, delivery/replay, policy, approval, authorization and execution | Foundry / host |

GrammarAi refers to the ProtocolAi envelope schema, version, event kind, and payload type by identity strings. It does not copy ProtocolAi DTOs or take a runtime dependency on ProtocolAi.

The candidate binding is exact: canonical `owner/repository`, ref, and full Git object SHA. The consumer maps those values from ProtocolAi's `ProtocolCandidateProvenance` and must reject any mismatch. The SHA carried by the event is not proof that the evidence is trustworthy.

Evidence predicates refer to named fields in an external evidence packet referenced by protocol provenance. GrammarAi validates the declared field identity, scalar type, operator, and expected-value shape. It does not open the evidence reference, authenticate the producer, or decide whether the evidence value is true.

## Bounded predicate model

Version 1 supports only:

- `Present`: the named evidence field must be present; it has no expected literal.
- `Equals`: the named evidence field must equal one typed scalar literal.
- Scalar types: Boolean, Integer, and Text.
- A bounded list of up to 64 distinct evidence identities.
- A single named success action targeting a versioned gate.
- Separate explicit handling for predicate non-success and failed/cancelled source events.

There is no embedded expression language, arbitrary code, callback, script, unbounded nesting, dynamic target, or free-form action. The success intent is limited to the ProtocolAi-compatible `Satisfied` state in this schema version.

The definition separately records behavior for predicate non-success (`False`, `Unknown`, or `Incomplete`) and failed/cancelled source events. A failure diagnostic cannot satisfy the success predicate. The JSON document is capped at 65,536 characters.

The definition also records the non-success behavior. A consumer must preserve the distinct evaluation states:

| State | Meaning |
|---|---|
| True | All required conditions have been authoritatively evaluated as satisfied |
| False | At least one condition is authoritatively false |
| Unknown | The available facts do not establish true or false |
| Incomplete | One or more required evidence identities are absent |

GrammarAi does not calculate these states. It validates the definition and can check that a candidate binding matches and that required evidence identities are present. CiAi/host owns the evidence verdict. Unknown, False, and Incomplete must never be treated as True.

## Example

```csharp
using TheSingularityWorkshop.GrammarAi;

var observer = new ObserverGrammarDefinition(
    observerId: "build-and-tests",
    observerVersion: "1",
    sourceEvent: new ObserverProtocolReference(
        "TheSingularityWorkshop.ProtocolAi.EventEnvelope",
        1,
        "SourceEvent",
        "ProtocolSourceEvent"),
    candidate: new ObserverCandidateScope(
        "TrentBest/Example",
        "refs/heads/development",
        "0123456789abcdef0123456789abcdef01234567"),
    evidence:
    [
        new ObserverEvidencePredicate(
            "build-succeeded",
            "buildSucceeded",
            ObserverEvidenceValueKind.Boolean,
            ObserverEvidenceOperator.Equals,
            ObserverEvidenceValue.FromBoolean(true)),
        new ObserverEvidencePredicate(
            "tests-passed",
            "passedTestIds",
            ObserverEvidenceValueKind.Text,
            ObserverEvidenceOperator.Present)
    ],
    successAction: new ObserverGateTransitionIntent(
        "gate-build-tests",
        "1"),
    nonSuccessBehavior: ObserverNonSuccessBehavior.DiagnosticOnly);

var json = ObserverGrammarJson.Serialize(observer);
var restored = ObserverGrammarJson.Deserialize(json);
```

The identifiers in the example are illustrative. The consumer resolves the event and evidence references against the actual ProtocolAi/CiAi contract versions. It must also compare the incoming provenance repository, ref, and SHA to the exact candidate binding:

```csharp
observer.ValidateCandidate(new ObserverCandidateScope(
    provenance.Repository,
    provenance.CandidateRef,
    provenance.CandidateSha));

observer.ValidateRequiredEvidence(availableEvidenceIds);
```

These checks establish structural scope and completeness only. They do not establish provider identity, evidence integrity, successful CI, or permission to mutate a gate.

A valid definition emits only a named transition intent. It does not create a ProtocolAi gate-transition event, update gate state, or dispatch a CommandAi proposal. The host performs those operations only after its own verification and authorization.

## Strict input behavior

The JSON codec rejects:

- unsupported or missing schema/version;
- malformed JSON, duplicate properties, and unknown fields;
- non-object roots and excessive JSON nesting;
- invalid enum values or type-incompatible expected literals;
- malformed repository/SHA scope and wildcard candidate refs;
- empty or duplicate evidence identities;
- missing expected values for `Equals`, or unexpected values for `Present`;
- unsupported success states and unbounded evidence lists.

CommandAi integration remains intentionally separate. The current CommandAi/1 proposal representation is not silently expanded here; GrammarAi will bind to its actual public shape once the cross-repository contract is settled.

## Deliberate non-goals

This model does not implement observer registration delivery, durable subscriptions, scheduling, replay or deduplication, stale-event ordering, CI evidence evaluation, provider authentication, authorization, human approval, command execution, or gate persistence. Those responsibilities remain with CiAi and the host/Foundry.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
