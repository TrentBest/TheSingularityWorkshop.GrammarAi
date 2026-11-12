using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class HttpFieldSyntaxAbnfTests
{
    [Fact]
    public void Rfc9110_field_line_slice_is_linked_and_bounded()
    {
        var grammar = HttpFieldSyntaxAbnf.Create();

        Assert.Equal(14, grammar.Definitions.Count);
        Assert.Empty(grammar.Validate());
    }

    [Fact]
    public void Field_line_has_name_colon_optional_whitespace_value_and_trailing_whitespace()
    {
        var grammar = HttpFieldSyntaxAbnf.Create();
        var fieldLine = Assert.IsType<AbnfConcatenation>(grammar.Definitions[0].Expression);

        Assert.Collection(fieldLine.Elements,
            item => Assert.Equal("field-name", Assert.IsType<AbnfRuleReference>(item).Name),
            item => Assert.Equal(":", Assert.IsType<AbnfCharacterValue>(item).Value),
            item => Assert.Equal("OWS", Assert.IsType<AbnfRuleReference>(item).Name),
            item => Assert.Equal("field-value", Assert.IsType<AbnfRuleReference>(item).Name),
            item => Assert.Equal("OWS", Assert.IsType<AbnfRuleReference>(item).Name));
    }

    [Fact]
    public void Token_requires_one_or_more_tchar_and_field_name_reuses_token()
    {
        var grammar = HttpFieldSyntaxAbnf.Create();
        var token = Assert.IsType<AbnfRepetition>(grammar.Definitions.Single(x => x.Name == "token").Expression);

        Assert.Equal(1u, token.Minimum);
        Assert.Null(token.Maximum);
        Assert.Equal("tchar", Assert.IsType<AbnfRuleReference>(token.Element).Name);
        Assert.Equal("token", Assert.IsType<AbnfRuleReference>(
            grammar.Definitions.Single(x => x.Name == "field-name").Expression).Name);
    }

    [Fact]
    public void Field_value_preserves_empty_value_and_field_content_whitespace_rules()
    {
        var grammar = HttpFieldSyntaxAbnf.Create();
        var value = Assert.IsType<AbnfRepetition>(
            grammar.Definitions.Single(x => x.Name == "field-value").Expression);
        var content = Assert.IsType<AbnfConcatenation>(
            grammar.Definitions.Single(x => x.Name == "field-content").Expression);
        var optional = Assert.IsType<AbnfOption>(content.Elements[1]);
        var continuation = Assert.IsType<AbnfRepetition>(optional.Expression);

        Assert.Equal(0u, value.Minimum);
        Assert.Null(value.Maximum);
        Assert.Equal("field-content", Assert.IsType<AbnfRuleReference>(value.Element).Name);
        Assert.Equal(1u, continuation.Minimum);
        Assert.Null(continuation.Maximum);
    }

    [Fact]
    public void Octet_ranges_are_explicit_and_preserve_obs_text_boundary()
    {
        var grammar = HttpFieldSyntaxAbnf.Create();
        var obsText = Assert.IsType<AbnfNumericValue>(
            grammar.Definitions.Single(x => x.Name == "obs-text").Expression);
        var vchar = Assert.IsType<AbnfNumericValue>(
            grammar.Definitions.Single(x => x.Name == "VCHAR").Expression);

        Assert.Equal(new byte[] { 0x80, 0xFF }, obsText.Values);
        Assert.Equal(new byte[] { 0x21, 0x7E }, vchar.Values);
        Assert.Equal(AbnfNumericForm.Range, obsText.Form);
        Assert.Equal(AbnfNumericBase.Hexadecimal, obsText.Base);
    }
}
