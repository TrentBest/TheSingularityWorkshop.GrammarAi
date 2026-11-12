using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class AbnfGrammarDefinitionTests
{
    [Fact]
    public void Rule_names_link_case_insensitively_and_preserve_source_spelling()
    {
        var grammar = new AbnfGrammarDefinition([
            new("Start", AbnfRuleOperator.Define, new AbnfRuleReference("TOKEN")),
            new("token", AbnfRuleOperator.Define, new AbnfCharacterValue("abc", AbnfCharacterCase.Insensitive))
        ]);

        Assert.Empty(grammar.Validate());
        Assert.Equal("Start", grammar.Definitions[0].Name);
        Assert.Equal("TOKEN", Assert.IsType<AbnfRuleReference>(grammar.Definitions[0].Expression).Name);
    }

    [Fact]
    public void Incremental_definitions_append_only_after_initial_definition()
    {
        var invalid = new AbnfGrammarDefinition([
            new("item", AbnfRuleOperator.IncrementalDefine, new AbnfCharacterValue("a", AbnfCharacterCase.Insensitive))
        ]);
        Assert.Contains(invalid.Validate(), x => x.Code == AbnfValidationCode.IncrementalDefinitionWithoutBase);

        var valid = new AbnfGrammarDefinition([
            new("item", AbnfRuleOperator.Define, new AbnfCharacterValue("a", AbnfCharacterCase.Insensitive)),
            new("ITEM", AbnfRuleOperator.IncrementalDefine, new AbnfCharacterValue("b", AbnfCharacterCase.Sensitive))
        ]);
        Assert.Empty(valid.Validate());
    }

    [Fact]
    public void Duplicate_initial_definitions_are_rejected_by_validation()
    {
        var grammar = new AbnfGrammarDefinition([
            new("name", AbnfRuleOperator.Define, new AbnfCharacterValue("a", AbnfCharacterCase.Insensitive)),
            new("NAME", AbnfRuleOperator.Define, new AbnfCharacterValue("b", AbnfCharacterCase.Insensitive))
        ]);
        Assert.Contains(grammar.Validate(), x => x.Code == AbnfValidationCode.DuplicateInitialDefinition);
    }

    [Fact]
    public void Unresolved_references_are_reported_without_evaluating_them()
    {
        var grammar = new AbnfGrammarDefinition([
            new("root", AbnfRuleOperator.Define, new AbnfRuleReference("missing"))
        ]);
        var issue = Assert.Single(grammar.Validate());
        Assert.Equal(AbnfValidationCode.UnresolvedRuleReference, issue.Code);
    }

    [Fact]
    public void Alternation_and_concatenation_preserve_order_and_immutable_copies()
    {
        var source = new List<AbnfExpression>
        {
            new AbnfCharacterValue("x", AbnfCharacterCase.Insensitive),
            new AbnfCharacterValue("y", AbnfCharacterCase.Sensitive)
        };
        var expression = new AbnfAlternation(source);
        source.Clear();

        Assert.Equal(2, expression.Choices.Count);
        Assert.Equal("x", Assert.IsType<AbnfCharacterValue>(expression.Choices[0]).Value);
        Assert.Throws<NotSupportedException>(() => ((IList<AbnfExpression>)expression.Choices).Clear());
    }

    [Fact]
    public void Repetition_supports_exact_bounded_and_unbounded_forms()
    {
        var atom = new AbnfCharacterValue("x", AbnfCharacterCase.Insensitive);
        Assert.Equal((3u, (uint?)3u), (new AbnfRepetition(3, 3, atom).Minimum, new AbnfRepetition(3, 3, atom).Maximum));
        Assert.Null(new AbnfRepetition(0, null, atom).Maximum);
        Assert.Throws<ArgumentOutOfRangeException>(() => new AbnfRepetition(3, 2, atom));
        Assert.Throws<ArgumentOutOfRangeException>(() => new AbnfRepetition(0, AbnfLimits.MaxFiniteRepetition + 1, atom));
    }

    [Fact]
    public void Numeric_values_distinguish_single_sequence_and_inclusive_range()
    {
        Assert.Single(new AbnfNumericValue(AbnfNumericBase.Decimal, AbnfNumericForm.Single, [255]).Values);
        Assert.Equal(new byte[] { 13, 10 }, new AbnfNumericValue(AbnfNumericBase.Decimal, AbnfNumericForm.Sequence, [13, 10]).Values);
        Assert.Equal(new byte[] { 0, 255 }, new AbnfNumericValue(AbnfNumericBase.Hexadecimal, AbnfNumericForm.Range, [0, 255]).Values);
        Assert.Throws<ArgumentException>(() => new AbnfNumericValue(AbnfNumericBase.Decimal, AbnfNumericForm.Range, [9, 8]));
        Assert.Throws<ArgumentException>(() => new AbnfNumericValue(AbnfNumericBase.Binary, AbnfNumericForm.Single, [1, 2]));
    }

    [Fact]
    public void Character_values_are_ascii_and_case_mode_is_explicit()
    {
        Assert.Equal(AbnfCharacterCase.Sensitive,
            new AbnfCharacterValue("HTTP", AbnfCharacterCase.Sensitive).CaseMode);
        Assert.Throws<ArgumentException>(() => new AbnfCharacterValue("café", AbnfCharacterCase.Sensitive));
        Assert.Throws<ArgumentException>(() => new AbnfCharacterValue(new string(new[] { 'l', (char)10, 'b' }), AbnfCharacterCase.Sensitive));
        Assert.Throws<ArgumentException>(() => new AbnfCharacterValue("quote" + (char)34 + "inside", AbnfCharacterCase.Sensitive));
        Assert.Throws<ArgumentOutOfRangeException>(() => new AbnfCharacterValue("x", (AbnfCharacterCase)99));
    }

    [Fact]
    public void Prose_values_remain_opaque_annotations()
    {
        var prose = new AbnfProseValue("application-defined meaning");
        var grammar = new AbnfGrammarDefinition([new("root", AbnfRuleOperator.Define, prose)]);
        Assert.Empty(grammar.Validate());
        Assert.Equal("application-defined meaning", prose.Description);
    }

    [Fact]
    public void Explicit_group_and_option_nodes_are_preserved()
    {
        var group = new AbnfGroup(new AbnfAlternation([
            new AbnfCharacterValue("a", AbnfCharacterCase.Insensitive),
            new AbnfCharacterValue("b", AbnfCharacterCase.Insensitive)]));
        var option = new AbnfOption(group);
        Assert.IsType<AbnfGroup>(Assert.IsType<AbnfOption>(option).Expression);
    }

    [Fact]
    public void Expression_nesting_depth_is_bounded_during_validation()
    {
        AbnfExpression expression = new AbnfCharacterValue("x", AbnfCharacterCase.Insensitive);
        for (var i = 0; i < AbnfLimits.MaxExpressionDepth; i++)
            expression = new AbnfGroup(expression);

        var grammar = new AbnfGrammarDefinition([
            new("root", AbnfRuleOperator.Define, expression)
        ]);

        Assert.Contains(grammar.Validate(), x => x.Code == AbnfValidationCode.ResourceLimitExceeded);
    }

    [Fact]
    public void Constructor_enforces_expression_collection_and_profile_limits()
    {
        Assert.Throws<ArgumentException>(() => new AbnfAlternation([new AbnfCharacterValue("a", AbnfCharacterCase.Insensitive)]));
        Assert.Throws<ArgumentException>(() => new AbnfConcatenation([]));
        Assert.Throws<ArgumentOutOfRangeException>(() => new AbnfGrammarDefinition(
            Enumerable.Range(0, AbnfLimits.MaxRules + 1)
                .Select(i => new AbnfRuleDefinition("r" + i, AbnfRuleOperator.Define, new AbnfCharacterValue("", AbnfCharacterCase.Insensitive)))));
    }
}
