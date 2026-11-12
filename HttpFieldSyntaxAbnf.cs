namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Builds the bounded HTTP field-line syntax slice from RFC 9110 section 5.1.
/// </summary>
/// <remarks>
/// This resource models field-name, token, optional whitespace, field-value, and the
/// visible/obs-text octet classes needed by the generic field-line shape. It is an
/// ABNF definition, not a source importer, byte recognizer, HTTP message parser,
/// header-specific semantic validator, or message-framing implementation.
/// The host remains responsible for preserving the original octets and applying
/// field-specific semantics and protocol policy.
/// </remarks>
public static class HttpFieldSyntaxAbnf
{
    private static AbnfRuleReference R(string name) => new(name);
    private static AbnfCharacterValue C(string value) => new(value, AbnfCharacterCase.Sensitive);
    private static AbnfNumericValue Range(byte first, byte last) =>
        new(AbnfNumericBase.Hexadecimal, AbnfNumericForm.Range, [first, last]);
    private static AbnfAlternation Alt(params AbnfExpression[] choices) => new(choices);
    private static AbnfConcatenation Seq(params AbnfExpression[] elements) => new(elements);
    private static AbnfRepetition Repeat(uint minimum, uint? maximum, AbnfExpression element) =>
        new(minimum, maximum, element);

    /// <summary>Creates the RFC 9110 generic HTTP field-line ABNF definition.</summary>
    public static AbnfGrammarDefinition Create()
    {
        var tchar = Alt(
            C("!"), C("#"), C("$"), C("%"), C("&"), C("'"), C("*"), C("+"),
            C("-"), C("."), C("^"), C("_"), C("`"), C("|"), C("~"),
            R("DIGIT"), R("ALPHA"));

        var whitespace = Alt(R("SP"), R("HTAB"));
        var fieldVchar = Alt(R("VCHAR"), R("obs-text"));
        var fieldContentContinuation = Repeat(1, null, Alt(R("SP"), R("HTAB"), R("field-vchar")));

        return new AbnfGrammarDefinition([
            new("field-line", AbnfRuleOperator.Define,
                Seq(R("field-name"), C(":"), R("OWS"), R("field-value"), R("OWS"))),
            new("field-name", AbnfRuleOperator.Define, R("token")),
            new("token", AbnfRuleOperator.Define, Repeat(1, null, R("tchar"))),
            new("tchar", AbnfRuleOperator.Define, tchar),
            new("OWS", AbnfRuleOperator.Define, Repeat(0, null, whitespace)),
            new("field-value", AbnfRuleOperator.Define, Repeat(0, null, R("field-content"))),
            new("field-content", AbnfRuleOperator.Define,
                Seq(R("field-vchar"), new AbnfOption(fieldContentContinuation))),
            new("field-vchar", AbnfRuleOperator.Define, fieldVchar),
            new("obs-text", AbnfRuleOperator.Define, Range(0x80, 0xFF)),
            new("ALPHA", AbnfRuleOperator.Define, Alt(Range(0x41, 0x5A), Range(0x61, 0x7A))),
            new("DIGIT", AbnfRuleOperator.Define, Range(0x30, 0x39)),
            new("SP", AbnfRuleOperator.Define, new AbnfNumericValue(
                AbnfNumericBase.Hexadecimal, AbnfNumericForm.Single, [0x20])),
            new("HTAB", AbnfRuleOperator.Define, new AbnfNumericValue(
                AbnfNumericBase.Hexadecimal, AbnfNumericForm.Single, [0x09])),
            new("VCHAR", AbnfRuleOperator.Define, Range(0x21, 0x7E))
        ]);
    }
}
