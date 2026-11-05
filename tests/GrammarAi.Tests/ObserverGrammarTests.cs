using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class ObserverGrammarTests
{
    private static ObserverGrammarDefinition CreateDefinition() =>
        new(
            "observer-build-tests",
            "1",
            new ObserverProtocolReference(
                "TheSingularityWorkshop.ProtocolAi.EventEnvelope",
                1,
                "ObserverRegistrationRequested",
                "ProtocolObserverRegistrationRequest"),
            new ObserverProtocolReference(
                "TheSingularityWorkshop.ProtocolAi.EventEnvelope",
                1,
                "SourceEvent",
                "ProtocolSourceEvent"),
            new ObserverCandidateScope(
                "TrentBest/Example",
                "refs/heads/development",
                new string('a', 40)),
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
            new ObserverGateTransitionIntent("gate-build-tests", "1"),
            ObserverNonSuccessBehavior.DiagnosticOnly);

    [Fact]
    public void Observer_definition_round_trips_through_versioned_json()
    {
        var original = CreateDefinition();

        var json = ObserverGrammarJson.Serialize(original);
        var restored = ObserverGrammarJson.Deserialize(json);

        Assert.Equal(ObserverGrammarDefinition.CurrentSchema, restored.Schema);
        Assert.Equal(1, restored.SchemaVersion);
        Assert.Equal(original.ObserverId, restored.ObserverId);
        Assert.Equal(original.RegistrationRequest.PayloadType, restored.RegistrationRequest.PayloadType);
        Assert.Equal(original.SourceEvent.EventKind, restored.SourceEvent.EventKind);
        Assert.Equal(original.Candidate.CandidateSha, restored.Candidate.CandidateSha);
        Assert.Equal(2, restored.Evidence.Count);
        Assert.Equal(ObserverEvidenceValue.FromBoolean(true), restored.Evidence[0].ExpectedValue);
        Assert.Equal("gate-build-tests", restored.SuccessAction.GateId);
    }

    [Fact]
    public void Candidate_binding_rejects_a_different_sha()
    {
        var definition = CreateDefinition();
        var different = new ObserverCandidateScope(
            "TrentBest/Example",
            "refs/heads/development",
            new string('b', 40));

        Assert.Throws<ArgumentException>(() => definition.ValidateCandidate(different));
    }

    [Fact]
    public void Candidate_binding_rejects_a_different_repository_or_ref()
    {
        var definition = CreateDefinition();

        Assert.Throws<ArgumentException>(() => definition.ValidateCandidate(
            new ObserverCandidateScope("Other/Example", "refs/heads/development", new string('a', 40))));
        Assert.Throws<ArgumentException>(() => definition.ValidateCandidate(
            new ObserverCandidateScope("TrentBest/Example", "refs/heads/master", new string('a', 40))));
    }

    [Fact]
    public void Evidence_completeness_rejects_missing_required_identity()
    {
        var definition = CreateDefinition();

        Assert.Throws<ArgumentException>(() =>
            definition.ValidateRequiredEvidence(["build-succeeded"]));
    }

    [Fact]
    public void Evidence_completeness_accepts_all_required_identities_without_evaluating_them()
    {
        var definition = CreateDefinition();

        definition.ValidateRequiredEvidence(["build-succeeded", "tests-passed", "extra-diagnostic"]);

        Assert.Equal(
            new[] { ObserverEvaluationState.True, ObserverEvaluationState.False, ObserverEvaluationState.Unknown, ObserverEvaluationState.Incomplete },
            Enum.GetValues<ObserverEvaluationState>());
    }

    [Fact]
    public void Equals_requires_a_typed_value_of_the_declared_kind()
    {
        Assert.Throws<ArgumentException>(() => new ObserverEvidencePredicate(
            "build",
            "buildSucceeded",
            ObserverEvidenceValueKind.Boolean,
            ObserverEvidenceOperator.Equals));

        Assert.Throws<ArgumentException>(() => new ObserverEvidencePredicate(
            "build",
            "buildSucceeded",
            ObserverEvidenceValueKind.Boolean,
            ObserverEvidenceOperator.Equals,
            ObserverEvidenceValue.FromText("true")));
    }

    [Fact]
    public void Present_does_not_accept_an_expected_value()
    {
        Assert.Throws<ArgumentException>(() => new ObserverEvidencePredicate(
            "build",
            "buildSucceeded",
            ObserverEvidenceValueKind.Boolean,
            ObserverEvidenceOperator.Present,
            ObserverEvidenceValue.FromBoolean(true)));
    }

    [Fact]
    public void Only_satisfied_is_accepted_as_a_success_transition_intent()
    {
        Assert.Throws<ArgumentException>(() => new ObserverGateTransitionIntent(
            "gate-build-tests",
            "1",
            "Closed"));
    }

    [Fact]
    public void Invalid_candidate_sha_is_rejected()
    {
        Assert.Throws<ArgumentException>(() => new ObserverCandidateScope(
            "TrentBest/Example",
            "refs/heads/development",
            "not-a-git-sha"));
    }

    [Fact]
    public void Duplicate_json_properties_are_rejected()
    {
        var json = ObserverGrammarJson.Serialize(CreateDefinition())
            .Replace("\"schema\":", "\"schema\":\"duplicate\",\"schema\":", StringComparison.Ordinal);

        Assert.Throws<FormatException>(() => ObserverGrammarJson.Deserialize(json));
    }

    [Fact]
    public void Unsupported_schema_version_is_rejected()
    {
        var json = ObserverGrammarJson.Serialize(CreateDefinition())
            .Replace("\"schemaVersion\":1", "\"schemaVersion\":2", StringComparison.Ordinal);

        Assert.Throws<FormatException>(() => ObserverGrammarJson.Deserialize(json));
    }

    [Fact]
    public void Unknown_json_fields_are_rejected()
    {
        var json = ObserverGrammarJson.Serialize(CreateDefinition());
        var altered = json[..^1] + ",\"unboundedExpression\":\"arbitrary code\"}";

        Assert.Throws<FormatException>(() => ObserverGrammarJson.Deserialize(altered));
    }

    [Fact]
    public void Wildcard_candidate_refs_are_rejected()
    {
        Assert.Throws<ArgumentException>(() => new ObserverCandidateScope(
            "TrentBest/Example",
            "refs/heads/*",
            new string('a', 40)));
    }

    [Fact]
    public void Failure_behavior_round_trips_separately_from_predicate_non_success_behavior()
    {
        var definition = CreateDefinition();
        var json = ObserverGrammarJson.Serialize(definition);
        var restored = ObserverGrammarJson.Deserialize(json);

        Assert.Equal(ObserverNonSuccessBehavior.DiagnosticOnly, restored.NonSuccessBehavior);
        Assert.Equal(ObserverFailureBehavior.DiagnosticOnly, restored.FailureBehavior);
    }

    [Fact]
    public void Oversized_observer_documents_are_rejected()
    {
        var oversized = "{\"schema\":\"TheSingularityWorkshop.GrammarAi.Observer\",\"schemaVersion\":1,\"padding\":\"" +
            new string('x', ObserverGrammarJson.MaxDocumentCharacters) + "\"}";

        Assert.Throws<FormatException>(() => ObserverGrammarJson.Deserialize(oversized));
    }

    [Fact]
    public void Evidence_predicate_count_is_bounded()
    {
        var evidence = Enumerable.Range(0, ObserverGrammarDefinition.MaxEvidencePredicates + 1)
            .Select(index => new ObserverEvidencePredicate(
                $"evidence-{index}",
                $"field-{index}",
                ObserverEvidenceValueKind.Boolean,
                ObserverEvidenceOperator.Present))
            .ToArray();

        Assert.Throws<ArgumentException>(() => new ObserverGrammarDefinition(
            "observer",
            "1",
            new ObserverProtocolReference("ProtocolAi.EventEnvelope", 1, "ObserverRegistrationRequested", "ProtocolObserverRegistrationRequest"),
            new ObserverProtocolReference("ProtocolAi.EventEnvelope", 1, "SourceEvent", "ProtocolSourceEvent"),
            new ObserverCandidateScope("owner/repo", "refs/heads/main", new string('c', 40)),
            evidence,
            new ObserverGateTransitionIntent("gate", "1"),
            ObserverNonSuccessBehavior.NoTransition));
    }

    [Fact]
    public void Protocol_reference_rejects_invalid_version_and_required_fields()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ObserverProtocolReference("schema", 0, "kind", "payload"));
        Assert.Throws<ArgumentException>(() => new ObserverProtocolReference(" ", 1, "kind", "payload"));
        Assert.Throws<ArgumentException>(() => new ObserverProtocolReference("schema", 1, "", "payload"));
        Assert.Throws<ArgumentException>(() => new ObserverProtocolReference("schema", 1, "kind", new string('x', 257)));
    }

    [Fact]
    public void Candidate_scope_rejects_noncanonical_repository_and_invalid_refs()
    {
        var sha = new string('a', 40);
        Assert.Throws<ArgumentException>(() => new ObserverCandidateScope("owner", "refs/heads/main", sha));
        Assert.Throws<ArgumentException>(() => new ObserverCandidateScope("owner//repo", "refs/heads/main", sha));
        Assert.Throws<ArgumentException>(() => new ObserverCandidateScope("owner/repo", "main", sha));
        Assert.Throws<ArgumentException>(() => new ObserverCandidateScope("owner/repo", "refs/heads/main?", sha));
        Assert.Throws<ArgumentException>(() => new ObserverCandidateScope("owner/repo", "refs/heads/main", new string('g', 40)));
        Assert.Throws<ArgumentException>(() => new ObserverCandidateScope("owner/repo", "refs/heads/main", new string('a', 39)));
    }

    [Fact]
    public void Candidate_scope_normalizes_sha_and_accepts_sha256()
    {
        var sha1 = new ObserverCandidateScope("owner/repo", "refs/heads/main", new string('A', 40));
        var sha256 = new ObserverCandidateScope("owner/repo", "refs/heads/main", new string('b', 64));
        Assert.Equal(new string('a', 40), sha1.CandidateSha);
        Assert.Equal(new string('b', 64), sha256.CandidateSha);
    }

    [Fact]
    public void Evidence_values_validate_kind_and_text_bounds()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ObserverEvidenceValue((ObserverEvidenceValueKind)99, true, null, null));
        Assert.Throws<ArgumentException>(() => new ObserverEvidenceValue(ObserverEvidenceValueKind.Boolean, null, null, null));
        Assert.Throws<ArgumentException>(() => ObserverEvidenceValue.FromText(" "));
        Assert.Throws<ArgumentException>(() => ObserverEvidenceValue.FromText(new string('x', 1025)));
        Assert.Equal(42L, ObserverEvidenceValue.FromInteger(42).Integer);
        Assert.Equal("ok", ObserverEvidenceValue.FromText("ok").Text);
        Assert.True(ObserverEvidenceValue.FromBoolean(true).Boolean);
    }

    [Fact]
    public void Evidence_predicates_reject_invalid_enums_and_required_identifiers()
    {
        Assert.Throws<ArgumentException>(() => new ObserverEvidencePredicate("", "field", ObserverEvidenceValueKind.Boolean, ObserverEvidenceOperator.Present));
        Assert.Throws<ArgumentException>(() => new ObserverEvidencePredicate("id", " ", ObserverEvidenceValueKind.Boolean, ObserverEvidenceOperator.Present));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ObserverEvidencePredicate("id", "field", (ObserverEvidenceValueKind)99, ObserverEvidenceOperator.Present));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ObserverEvidencePredicate("id", "field", ObserverEvidenceValueKind.Boolean, (ObserverEvidenceOperator)99));
    }

    [Fact]
    public void Observer_definition_rejects_empty_duplicate_and_null_predicates()
    {
        var valid = CreateDefinition();
        ObserverGrammarDefinition CreateWith(IReadOnlyList<ObserverEvidencePredicate> evidence) =>
            new("id", "1", valid.RegistrationRequest, valid.SourceEvent, valid.Candidate, evidence,
                valid.SuccessAction, ObserverNonSuccessBehavior.NoTransition);
        Assert.Throws<ArgumentException>(() => CreateWith(Array.Empty<ObserverEvidencePredicate>()));
        Assert.Throws<ArgumentException>(() => CreateWith(new ObserverEvidencePredicate[] { null! }));
        Assert.Throws<ArgumentException>(() => CreateWith(new[]
        {
            new ObserverEvidencePredicate("same", "one", ObserverEvidenceValueKind.Boolean, ObserverEvidenceOperator.Present),
            new ObserverEvidencePredicate("same", "two", ObserverEvidenceValueKind.Boolean, ObserverEvidenceOperator.Present)
        }));
    }

    [Fact]
    public void Observer_definition_rejects_invalid_behaviors_and_null_dependencies()
    {
        var valid = CreateDefinition();
        Assert.Throws<ArgumentNullException>(() => new ObserverGrammarDefinition("id", "1", null!, valid.SourceEvent, valid.Candidate,
            valid.Evidence, valid.SuccessAction, ObserverNonSuccessBehavior.NoTransition));
        Assert.Throws<ArgumentNullException>(() => new ObserverGrammarDefinition("id", "1", valid.RegistrationRequest, null!, valid.Candidate,
            valid.Evidence, valid.SuccessAction, ObserverNonSuccessBehavior.NoTransition));
        Assert.Throws<ArgumentNullException>(() => new ObserverGrammarDefinition("id", "1", valid.RegistrationRequest, valid.SourceEvent, null!,
            valid.Evidence, valid.SuccessAction, ObserverNonSuccessBehavior.NoTransition));
        Assert.Throws<ArgumentNullException>(() => new ObserverGrammarDefinition("id", "1", valid.RegistrationRequest, valid.SourceEvent, valid.Candidate,
            null!, valid.SuccessAction, ObserverNonSuccessBehavior.NoTransition));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ObserverGrammarDefinition("id", "1", valid.RegistrationRequest, valid.SourceEvent,
            valid.Candidate, valid.Evidence, valid.SuccessAction, (ObserverNonSuccessBehavior)99));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ObserverGrammarDefinition("id", "1", valid.RegistrationRequest, valid.SourceEvent,
            valid.Candidate, valid.Evidence, valid.SuccessAction, ObserverNonSuccessBehavior.NoTransition, (ObserverFailureBehavior)99));
        Assert.Throws<ArgumentNullException>(() => new ObserverGrammarDefinition("id", "1", valid.RegistrationRequest, valid.SourceEvent,
            valid.Candidate, valid.Evidence, null!, ObserverNonSuccessBehavior.NoTransition));
    }

    [Fact]
    public void Observer_validation_methods_reject_null_inputs_and_accept_matching_sha_case()
    {
        var definition = CreateDefinition();
        Assert.Throws<ArgumentNullException>(() => definition.ValidateCandidate(null!));
        Assert.Throws<ArgumentNullException>(() => definition.ValidateRequiredEvidence(null!));
        definition.ValidateCandidate(new ObserverCandidateScope("TrentBest/Example", "refs/heads/development", new string('A', 40)));
        Assert.Throws<ArgumentException>(() => definition.ValidateRequiredEvidence(new[] { "BUILD-SUCCEEDED", "tests-passed" }));
    }

    [Fact]
    public void Observer_json_rejects_empty_malformed_nonobject_and_missing_schema_documents()
    {
        Assert.Throws<ArgumentException>(() => ObserverGrammarJson.Deserialize(" "));
        Assert.Throws<FormatException>(() => ObserverGrammarJson.Deserialize("{"));
        Assert.Throws<FormatException>(() => ObserverGrammarJson.Deserialize("[]"));
        Assert.Throws<FormatException>(() => ObserverGrammarJson.Deserialize("{\"schemaVersion\":1}"));
        Assert.Throws<FormatException>(() => ObserverGrammarJson.Deserialize("{\"schema\":\"wrong\",\"schemaVersion\":1}"));
    }

    [Fact]
    public void Observer_json_rejects_duplicate_nested_properties_and_unknown_enum_values()
    {
        var json = ObserverGrammarJson.Serialize(CreateDefinition());
        var duplicateNested = json.Replace("\"observerId\":", "\"observerId\":\"duplicate\",\"observerId\":", StringComparison.Ordinal);
        Assert.Throws<FormatException>(() => ObserverGrammarJson.Deserialize(duplicateNested));
        var invalidEnum = json.Replace("\"DiagnosticOnly\"", "\"RunArbitraryCode\"", StringComparison.Ordinal);
        Assert.Throws<FormatException>(() => ObserverGrammarJson.Deserialize(invalidEnum));
    }

}
