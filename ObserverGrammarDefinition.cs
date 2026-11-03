using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Identifies a ProtocolAi event contract without copying or owning its payload type.</summary>
public sealed record ObserverProtocolReference
{
    public ObserverProtocolReference(string schema, int schemaVersion, string eventKind, string payloadType)
    {
        Schema = Required(schema, nameof(schema), 256);
        if (schemaVersion <= 0) throw new ArgumentOutOfRangeException(nameof(schemaVersion));
        SchemaVersion = schemaVersion;
        EventKind = Required(eventKind, nameof(eventKind), 128);
        PayloadType = Required(payloadType, nameof(payloadType), 256);
    }

    public string Schema { get; }
    public int SchemaVersion { get; }
    public string EventKind { get; }
    public string PayloadType { get; }

    internal static string Required(string value, string parameter, int max)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A non-empty value is required.", parameter);
        if (value.Length > max) throw new ArgumentException($"Value cannot exceed {max} characters.", parameter);
        return value;
    }
}

/// <summary>Exact repository candidate to which an observer definition is bound.</summary>
public sealed record ObserverCandidateScope
{
    public ObserverCandidateScope(string repository, string candidateRef, string candidateSha)
    {
        Repository = ObserverProtocolReference.Required(repository, nameof(repository), 256);
        var parts = Repository.Split('/');
        if (parts.Length != 2 || parts.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Repository must use canonical owner/name form.", nameof(repository));
        CandidateRef = ObserverProtocolReference.Required(candidateRef, nameof(candidateRef), 512);
        if (!CandidateRef.StartsWith("refs/", StringComparison.Ordinal) ||
            CandidateRef.Contains('*') || CandidateRef.Contains('?') || CandidateRef.Contains('[') || CandidateRef.Contains(']'))
            throw new ArgumentException("Candidate ref must be a fully qualified, non-wildcard Git ref.", nameof(candidateRef));
        CandidateSha = ObserverProtocolReference.Required(candidateSha, nameof(candidateSha), 64);
        if ((CandidateSha.Length != 40 && CandidateSha.Length != 64) || !CandidateSha.All(Uri.IsHexDigit))
            throw new ArgumentException("Candidate SHA must be a full 40- or 64-character hexadecimal Git object ID.", nameof(candidateSha));
        CandidateSha = CandidateSha.ToLowerInvariant();
    }

    public string Repository { get; }
    public string CandidateRef { get; }
    public string CandidateSha { get; }
}

/// <summary>Supported scalar evidence types. No arbitrary expression or executable value is accepted.</summary>
public enum ObserverEvidenceValueKind { Boolean, Integer, Text }

/// <summary>Supported evidence comparison operations.</summary>
public enum ObserverEvidenceOperator { Present, Equals }

/// <summary>A typed scalar literal used by a bounded evidence predicate.</summary>
public sealed record ObserverEvidenceValue
{
    public ObserverEvidenceValue(ObserverEvidenceValueKind kind, bool? boolean, long? integer, string? text)
    {
        if (!Enum.IsDefined(kind)) throw new ArgumentOutOfRangeException(nameof(kind));
        var populated = (boolean.HasValue ? 1 : 0) + (integer.HasValue ? 1 : 0) + (text is not null ? 1 : 0);
        if (populated != 1 ||
            (kind == ObserverEvidenceValueKind.Boolean && !boolean.HasValue) ||
            (kind == ObserverEvidenceValueKind.Integer && !integer.HasValue) ||
            (kind == ObserverEvidenceValueKind.Text && text is null))
            throw new ArgumentException("Exactly one value must be supplied and it must match the declared kind.");
        if (text is not null)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Evidence text cannot be empty.", nameof(text));
            if (text.Length > 1024) throw new ArgumentException("Evidence text cannot exceed 1024 characters.", nameof(text));
        }
        Kind = kind;
        Boolean = boolean;
        Integer = integer;
        Text = text;
    }

    public ObserverEvidenceValueKind Kind { get; }
    public bool? Boolean { get; }
    public long? Integer { get; }
    public string? Text { get; }

    public static ObserverEvidenceValue FromBoolean(bool value) => new(ObserverEvidenceValueKind.Boolean, value, null, null);
    public static ObserverEvidenceValue FromInteger(long value) => new(ObserverEvidenceValueKind.Integer, null, value, null);
    public static ObserverEvidenceValue FromText(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Evidence text cannot be empty.", nameof(value));
        if (value.Length > 1024) throw new ArgumentException("Evidence text cannot exceed 1024 characters.", nameof(value));
        return new(ObserverEvidenceValueKind.Text, null, null, value);
    }
}

/// <summary>One typed requirement over a named field in an external evidence packet referenced by protocol provenance.</summary>
public sealed record ObserverEvidencePredicate
{
    public ObserverEvidencePredicate(string evidenceId, string evidenceField, ObserverEvidenceValueKind valueKind,
        ObserverEvidenceOperator operation, ObserverEvidenceValue? expectedValue = null)
    {
        EvidenceId = ObserverProtocolReference.Required(evidenceId, nameof(evidenceId), 128);
        EvidenceField = ObserverProtocolReference.Required(evidenceField, nameof(evidenceField), 256);
        if (!Enum.IsDefined(valueKind)) throw new ArgumentOutOfRangeException(nameof(valueKind));
        if (!Enum.IsDefined(operation)) throw new ArgumentOutOfRangeException(nameof(operation));
        if (operation == ObserverEvidenceOperator.Equals && expectedValue is null)
            throw new ArgumentException("Equals requires a typed expected value.", nameof(expectedValue));
        if (operation == ObserverEvidenceOperator.Present && expectedValue is not null)
            throw new ArgumentException("Present does not accept an expected value.", nameof(expectedValue));
        if (expectedValue is not null && expectedValue.Kind != valueKind)
            throw new ArgumentException("Expected value type must match the evidence value kind.", nameof(expectedValue));
        ValueKind = valueKind;
        Operation = operation;
        ExpectedValue = expectedValue;
    }

    public string EvidenceId { get; }
    public string EvidenceField { get; }
    public ObserverEvidenceValueKind ValueKind { get; }
    public ObserverEvidenceOperator Operation { get; }
    public ObserverEvidenceValue? ExpectedValue { get; }
}

/// <summary>Named intent to request a gate transition; it does not change the gate.</summary>
public sealed record ObserverGateTransitionIntent
{
    public ObserverGateTransitionIntent(string gateId, string gateVersion, string successState = "Satisfied")
    {
        GateId = ObserverProtocolReference.Required(gateId, nameof(gateId), 256);
        GateVersion = ObserverProtocolReference.Required(gateVersion, nameof(gateVersion), 128);
        SuccessState = ObserverProtocolReference.Required(successState, nameof(successState), 64);
        if (!string.Equals(SuccessState, "Satisfied", StringComparison.Ordinal))
            throw new ArgumentException("The first observer grammar version may only express the Satisfied success intent.", nameof(successState));
    }

    public string GateId { get; }
    public string GateVersion { get; }
    public string SuccessState { get; }
}

/// <summary>Defines what a consumer should do when the predicate is not definitively true.</summary>
public enum ObserverNonSuccessBehavior { NoTransition, DiagnosticOnly }

/// <summary>Defines how a failed, cancelled, or otherwise unsuccessful source event is handled.</summary>
public enum ObserverFailureBehavior { NoTransition, DiagnosticOnly }

/// <summary>Versioned, bounded observer condition/action data. It never evaluates evidence or executes an action.</summary>
public sealed record ObserverGrammarDefinition
{
    public const string CurrentSchema = "TheSingularityWorkshop.GrammarAi.Observer";
    public const int CurrentSchemaVersion = 1;
    public const int MaxEvidencePredicates = 64;

    public ObserverGrammarDefinition(string observerId, string observerVersion, ObserverProtocolReference sourceEvent,
        ObserverCandidateScope candidate, IReadOnlyList<ObserverEvidencePredicate> evidence,
        ObserverGateTransitionIntent successAction, ObserverNonSuccessBehavior nonSuccessBehavior,
        ObserverFailureBehavior failureBehavior = ObserverFailureBehavior.DiagnosticOnly)
    {
        ObserverId = ObserverProtocolReference.Required(observerId, nameof(observerId), 256);
        ObserverVersion = ObserverProtocolReference.Required(observerVersion, nameof(observerVersion), 64);
        SourceEvent = sourceEvent ?? throw new ArgumentNullException(nameof(sourceEvent));
        Candidate = candidate ?? throw new ArgumentNullException(nameof(candidate));
        ArgumentNullException.ThrowIfNull(evidence);
        var predicates = evidence.Take(MaxEvidencePredicates + 1).ToArray();
        if (predicates.Length == 0) throw new ArgumentException("At least one evidence predicate is required.", nameof(evidence));
        if (predicates.Length > MaxEvidencePredicates)
            throw new ArgumentException($"An observer cannot contain more than {MaxEvidencePredicates} evidence predicates.", nameof(evidence));
        if (predicates.Any(x => x is null)) throw new ArgumentException("Evidence predicates cannot contain null items.", nameof(evidence));
        if (predicates.Select(x => x.EvidenceId).Distinct(StringComparer.Ordinal).Count() != predicates.Length)
            throw new ArgumentException("Evidence identifiers must be unique within an observer.", nameof(evidence));
        Evidence = Array.AsReadOnly(predicates);
        SuccessAction = successAction ?? throw new ArgumentNullException(nameof(successAction));
        if (!Enum.IsDefined(nonSuccessBehavior)) throw new ArgumentOutOfRangeException(nameof(nonSuccessBehavior));
        NonSuccessBehavior = nonSuccessBehavior;
        if (!Enum.IsDefined(failureBehavior)) throw new ArgumentOutOfRangeException(nameof(failureBehavior));
        FailureBehavior = failureBehavior;
    }

    public string Schema => CurrentSchema;
    public int SchemaVersion => CurrentSchemaVersion;
    public string ObserverId { get; }
    public string ObserverVersion { get; }
    public ObserverProtocolReference SourceEvent { get; }
    public ObserverCandidateScope Candidate { get; }
    public IReadOnlyList<ObserverEvidencePredicate> Evidence { get; }
    public ObserverGateTransitionIntent SuccessAction { get; }
    public ObserverNonSuccessBehavior NonSuccessBehavior { get; }
    public ObserverFailureBehavior FailureBehavior { get; }

    /// <summary>Rejects an event whose repository, ref, or candidate SHA differs from this observer's exact binding.</summary>
    public void ValidateCandidate(ObserverCandidateScope actual)
    {
        ArgumentNullException.ThrowIfNull(actual);
        if (!string.Equals(Candidate.Repository, actual.Repository, StringComparison.Ordinal) ||
            !string.Equals(Candidate.CandidateRef, actual.CandidateRef, StringComparison.Ordinal) ||
            !string.Equals(Candidate.CandidateSha, actual.CandidateSha, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Event candidate does not match the observer's exact repository/ref/SHA binding.", nameof(actual));
    }

    /// <summary>Rejects an evidence packet that omits any evidence identity required by this definition.</summary>
    /// <remarks>This checks presence only. It does not verify evidence authenticity or evaluate predicate truth.</remarks>
    public void ValidateRequiredEvidence(IEnumerable<string> availableEvidenceIds)
    {
        ArgumentNullException.ThrowIfNull(availableEvidenceIds);
        var available = new HashSet<string>(availableEvidenceIds, StringComparer.Ordinal);
        var missing = Evidence.Select(item => item.EvidenceId).Where(id => !available.Contains(id)).ToArray();
        if (missing.Length > 0)
            throw new ArgumentException($"Evidence packet is incomplete; missing: {string.Join(", ", missing)}.", nameof(availableEvidenceIds));
    }
}

/// <summary>Explicit result vocabulary shared with ProtocolAi; values are data, not computed here.</summary>
public enum ObserverEvaluationState { True, False, Unknown, Incomplete }

/// <summary>Strict JSON codec for the versioned observer grammar document.</summary>
public static class ObserverGrammarJson
{
    public const int MaxDocumentCharacters = 65_536;

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        MaxDepth = 16,
        Converters = { new JsonStringEnumConverter() }
    };

    public static string Serialize(ObserverGrammarDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        return JsonSerializer.Serialize(definition, Options);
    }

    public static ObserverGrammarDefinition Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Observer grammar JSON is required.", nameof(json));
        if (json.Length > MaxDocumentCharacters)
            throw new FormatException($"Observer grammar JSON cannot exceed {MaxDocumentCharacters} characters.");
        try
        {
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = 16 });
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                throw new FormatException("Observer grammar must be a JSON object.");
            if (!document.RootElement.TryGetProperty("schema", out var schema) ||
                schema.GetString() != ObserverGrammarDefinition.CurrentSchema ||
                !document.RootElement.TryGetProperty("schemaVersion", out var version) ||
                !version.TryGetInt32(out var schemaVersion) ||
                schemaVersion != ObserverGrammarDefinition.CurrentSchemaVersion)
                throw new FormatException("Unsupported or missing observer grammar schema/version.");
            RejectDuplicateProperties(document.RootElement);
            var result = JsonSerializer.Deserialize<ObserverGrammarDefinition>(json, Options)
                ?? throw new FormatException("Observer grammar cannot be null.");
            if (!string.Equals(result.Schema, ObserverGrammarDefinition.CurrentSchema, StringComparison.Ordinal) ||
                result.SchemaVersion != ObserverGrammarDefinition.CurrentSchemaVersion)
                throw new FormatException("Unsupported observer grammar schema or version.");
            return result;
        }
        catch (JsonException ex)
        {
            throw new FormatException("Observer grammar JSON is malformed or contains unknown fields.", ex);
        }
    }

    private static void RejectDuplicateProperties(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in element.EnumerateObject())
            {
                if (!names.Add(property.Name)) throw new FormatException($"Duplicate JSON property '{property.Name}' is not allowed.");
                RejectDuplicateProperties(property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
            foreach (var item in element.EnumerateArray()) RejectDuplicateProperties(item);
    }
}
