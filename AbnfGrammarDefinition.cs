namespace TheSingularityWorkshop.GrammarAi;

/// <summary>ABNF rule-definition operator.</summary>
public enum AbnfRuleOperator : byte { Define = 0, IncrementalDefine = 1 }
/// <summary>Case behavior for an ABNF character value.</summary>
public enum AbnfCharacterCase : byte { Insensitive = 0, Sensitive = 1 }
/// <summary>Radix used by an ABNF numeric value.</summary>
public enum AbnfNumericBase : byte { Binary = 2, Decimal = 10, Hexadecimal = 16 }
/// <summary>Shape of an ABNF numeric value.</summary>
public enum AbnfNumericForm : byte { Single = 0, Sequence = 1, Range = 2 }
/// <summary>ABNF standards profile supported by this representation.</summary>
public enum AbnfProfile : byte { Rfc5234Rfc7405 = 0 }
/// <summary>Stable kinds of structural ABNF validation findings.</summary>
public enum AbnfValidationCode : byte
{
    InvalidRuleName, DuplicateInitialDefinition, IncrementalDefinitionWithoutBase,
    UnresolvedRuleReference, ResourceLimitExceeded
}

/// <summary>A structural validation finding. It does not evaluate ABNF language membership.</summary>
public sealed record AbnfValidationIssue(AbnfValidationCode Code, string Message, string? RuleName = null);

/// <summary>Finite resource limits for an ABNF definition.</summary>
public static class AbnfLimits
{
    public const int MaxRules = 4096;
    public const int MaxRuleNameCharacters = 64;
    public const int MaxCharacterValueCharacters = 4096;
    public const int MaxProseValueCharacters = 4096;
    public const int MaxExpressionNodes = 65536;
    public const int MaxExpressionDepth = 128;
    public const int MaxElementsPerExpression = 1024;
    public const uint MaxFiniteRepetition = 1_000_000;
    public const int MaxNumericComponents = 4096;
}

/// <summary>Base type for immutable ABNF expressions.</summary>
public abstract class AbnfExpression { }

/// <summary>ABNF alternation (language union, not prioritized choice).</summary>
public sealed class AbnfAlternation : AbnfExpression
{
    public AbnfAlternation(IEnumerable<AbnfExpression> choices) => Choices = AbnfCollections.Copy(choices, nameof(choices), 2);
    public IReadOnlyList<AbnfExpression> Choices { get; }
}
/// <summary>Ordered ABNF concatenation.</summary>
public sealed class AbnfConcatenation : AbnfExpression
{
    public AbnfConcatenation(IEnumerable<AbnfExpression> elements) => Elements = AbnfCollections.Copy(elements, nameof(elements), 1);
    public IReadOnlyList<AbnfExpression> Elements { get; }
}
/// <summary>ABNF repetition. A null maximum means unbounded language repetition, not unbounded evaluation.</summary>
public sealed class AbnfRepetition : AbnfExpression
{
    public AbnfRepetition(uint minimum, uint? maximum, AbnfExpression element)
    {
        if (maximum is { } max && (minimum > max || max > AbnfLimits.MaxFiniteRepetition))
            throw new ArgumentOutOfRangeException(nameof(maximum), "Finite repetition bounds must be ordered and within the configured limit.");
        if (minimum > AbnfLimits.MaxFiniteRepetition)
            throw new ArgumentOutOfRangeException(nameof(minimum));
        Element = element ?? throw new ArgumentNullException(nameof(element));
        Minimum = minimum; Maximum = maximum;
    }
    public uint Minimum { get; }
    public uint? Maximum { get; }
    public AbnfExpression Element { get; }
}
/// <summary>Reference to an ABNF rule name; linking is case-insensitive.</summary>
public sealed class AbnfRuleReference : AbnfExpression
{
    public AbnfRuleReference(string name) { AbnfNames.RequireValid(name, nameof(name)); Name = name; }
    public string Name { get; }
}
/// <summary>ASCII character string with explicit RFC 5234/RFC 7405 case behavior.</summary>
public sealed class AbnfCharacterValue : AbnfExpression
{
    public AbnfCharacterValue(string value, AbnfCharacterCase caseMode)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Length > AbnfLimits.MaxCharacterValueCharacters) throw new ArgumentOutOfRangeException(nameof(value));
        if (value.Any(c => c > 0x7f)) throw new ArgumentException("ABNF character values are ASCII; Unicode/UTF-8 conversion is not implicit.", nameof(value));
        if (!Enum.IsDefined(caseMode)) throw new ArgumentOutOfRangeException(nameof(caseMode));
        Value = value; CaseMode = caseMode;
    }
    public string Value { get; }
    public AbnfCharacterCase CaseMode { get; }
}
/// <summary>Numeric octet value. Range form has two inclusive endpoints; sequence form is nonempty.</summary>
public sealed class AbnfNumericValue : AbnfExpression
{
    public AbnfNumericValue(AbnfNumericBase numberBase, AbnfNumericForm form, IEnumerable<byte> values)
    {
        if (!Enum.IsDefined(numberBase)) throw new ArgumentOutOfRangeException(nameof(numberBase));
        if (!Enum.IsDefined(form)) throw new ArgumentOutOfRangeException(nameof(form));
        var copy = AbnfCollections.Copy(values, nameof(values), 1);
        if (copy.Count > AbnfLimits.MaxNumericComponents) throw new ArgumentOutOfRangeException(nameof(values));
        if (form == AbnfNumericForm.Single && copy.Count != 1) throw new ArgumentException("A single numeric value contains exactly one octet.", nameof(values));
        if (form == AbnfNumericForm.Range && (copy.Count != 2 || copy[0] > copy[1])) throw new ArgumentException("A numeric range contains exactly two ordered inclusive endpoints.", nameof(values));
        Form = form; Base = numberBase; Values = copy;
    }
    public AbnfNumericBase Base { get; }
    public AbnfNumericForm Form { get; }
    public IReadOnlyList<byte> Values { get; }
}
/// <summary>Opaque ABNF prose value. It is descriptive and is never treated as an executable recognizer.</summary>
public sealed class AbnfProseValue : AbnfExpression
{
    public AbnfProseValue(string description)
    {
        ArgumentNullException.ThrowIfNull(description);
        if (description.Length == 0 || description.Length > AbnfLimits.MaxProseValueCharacters) throw new ArgumentOutOfRangeException(nameof(description));
        Description = description;
    }
    public string Description { get; }
}
/// <summary>Explicit grouping retained for source fidelity and diagnostics.</summary>
public sealed class AbnfGroup : AbnfExpression
{
    public AbnfGroup(AbnfExpression expression) => Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    public AbnfExpression Expression { get; }
}
/// <summary>Explicit optional expression, equivalent in recognition to repetition 0..1.</summary>
public sealed class AbnfOption : AbnfExpression
{
    public AbnfOption(AbnfExpression expression) => Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    public AbnfExpression Expression { get; }
}
/// <summary>One source-ordered definition or incremental contribution to an ABNF rule.</summary>
public sealed class AbnfRuleDefinition
{
    public AbnfRuleDefinition(string name, AbnfRuleOperator @operator, AbnfExpression expression)
    {
        AbnfNames.RequireValid(name, nameof(name));
        if (!Enum.IsDefined(@operator)) throw new ArgumentOutOfRangeException(nameof(@operator));
        Name = name; Operator = @operator; Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }
    public string Name { get; }
    public AbnfRuleOperator Operator { get; }
    public AbnfExpression Expression { get; }
}

/// <summary>An immutable ABNF definition pinned to RFC 5234 plus RFC 7405 structural semantics.</summary>
/// <remarks>This model validates structure and rule links only. It does not import source, parse input, or recognize strings.</remarks>
public sealed class AbnfGrammarDefinition
{
    private readonly IReadOnlyList<AbnfRuleDefinition> _definitions;
    public AbnfGrammarDefinition(IEnumerable<AbnfRuleDefinition> definitions, AbnfProfile profile = AbnfProfile.Rfc5234Rfc7405)
    {
        if (!Enum.IsDefined(profile)) throw new ArgumentOutOfRangeException(nameof(profile));
        _definitions = AbnfCollections.Copy(definitions, nameof(definitions), 1, AbnfLimits.MaxRules);
        if (_definitions.Count > AbnfLimits.MaxRules) throw new ArgumentOutOfRangeException(nameof(definitions));
        Profile = profile;
    }
    public AbnfProfile Profile { get; }
    /// <summary>Gets source-ordered initial and incremental rule definitions.</summary>
    public IReadOnlyList<AbnfRuleDefinition> Definitions => _definitions;

    /// <summary>Validates names, source-order rule assembly, references, and finite structural budgets.</summary>
    public IReadOnlyList<AbnfValidationIssue> Validate()
    {
        var issues = new List<AbnfValidationIssue>();
        var declared = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var initialSeen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        long nodeCount = 0;
        foreach (var definition in _definitions)
        {
            if (!AbnfNames.IsValid(definition.Name))
                issues.Add(new(AbnfValidationCode.InvalidRuleName, $"Invalid ABNF rule name '{definition.Name}'.", definition.Name));
            if (definition.Operator == AbnfRuleOperator.Define)
            {
                if (!initialSeen.Add(definition.Name))
                    issues.Add(new(AbnfValidationCode.DuplicateInitialDefinition, $"Rule '{definition.Name}' has more than one initial '=' definition.", definition.Name));
                declared.Add(definition.Name);
            }
            else if (!declared.Contains(definition.Name))
                issues.Add(new(AbnfValidationCode.IncrementalDefinitionWithoutBase, $"Rule '{definition.Name}' uses '=/’ before an initial '=' definition.", definition.Name));

        }

        foreach (var definition in _definitions)
            Walk(definition.Expression, definition.Name, 1, ref nodeCount, issues, declared);

        return Array.AsReadOnly(issues.ToArray());
    }

    private static void Walk(AbnfExpression expression, string ruleName, int depth, ref long count,
        List<AbnfValidationIssue> issues, HashSet<string>? declared)
    {
        count++;
        if (count > AbnfLimits.MaxExpressionNodes || depth > AbnfLimits.MaxExpressionDepth)
        {
            if (!issues.Any(x => x.Code == AbnfValidationCode.ResourceLimitExceeded))
                issues.Add(new(AbnfValidationCode.ResourceLimitExceeded, "ABNF expression node or nesting-depth limit exceeded.", ruleName));
            return;
        }
        if (expression is AbnfRuleReference reference && declared is not null && !declared.Contains(reference.Name))
            issues.Add(new(AbnfValidationCode.UnresolvedRuleReference, $"Rule reference '{reference.Name}' is unresolved.", ruleName));

        IEnumerable<AbnfExpression> children = expression switch
        {
            AbnfAlternation a => a.Choices,
            AbnfConcatenation c => c.Elements,
            AbnfRepetition r => [r.Element],
            AbnfGroup g => [g.Expression],
            AbnfOption o => [o.Expression],
            _ => []
        };
        foreach (var child in children) Walk(child, ruleName, depth + 1, ref count, issues, declared);
    }
}

internal static class AbnfNames
{
    public static void RequireValid(string name, string parameter)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (!IsValid(name)) throw new ArgumentException("An ABNF rule name must start with an ASCII letter and contain only ASCII letters, digits, or hyphens.", parameter);
    }
    public static bool IsValid(string name) =>
        name.Length is > 0 and <= AbnfLimits.MaxRuleNameCharacters &&
        IsLetter(name[0]) &&
        name.All(c => IsLetter(c) || c is >= '0' and <= '9' || c == '-');
    private static bool IsLetter(char c) => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z';
}

internal static class AbnfCollections
{
    public static IReadOnlyList<T> Copy<T>(IEnumerable<T> values, string parameter, int minimum, int maximum = AbnfLimits.MaxElementsPerExpression)
    {
        ArgumentNullException.ThrowIfNull(values);
        var list = new List<T>(Math.Min(maximum, 16));
        foreach (var value in values)
        {
            if (value is null) throw new ArgumentException("Collections cannot contain null values.", parameter);
            if (list.Count == maximum) throw new ArgumentOutOfRangeException(parameter);
            list.Add(value);
        }
        if (list.Count < minimum) throw new ArgumentException($"At least {minimum} item(s) are required.", parameter);
        return Array.AsReadOnly(list.ToArray());
    }
}
