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
}
