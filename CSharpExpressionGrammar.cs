namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Vocabulary members required by the layered C# expression grammar.</summary>
public enum CSharpExpressionToken
{
    Identifier,
    Literal,
    OpenParenthesis,
    CloseParenthesis,
    Assignment,
    LogicalOr,
    LogicalAnd,
    Equality,
    Relational,
    Additive,
    Multiplicative,
    Unary,
    Dot,
    Comma,
    OpenBracket,
    CloseBracket,
    NullCoalescing,
    ConditionalQuestion,
    Colon,
    BitwiseOr,
    BitwiseXor,
    BitwiseAnd,
    Shift
}

/// <summary>
/// Builds a compact, precedence-layered C# expression grammar over host-owned ProtocolAi token identities.
/// </summary>
/// <remarks>
/// This is a structural expression grammar, not a C# parser or semantic model. The host supplies the
/// vocabulary category and member IDs from its selected ProtocolAi catalog. Productions encode precedence
/// and associativity; token spelling, overload resolution, type checking, and language-version policy remain
/// outside GrammarAi. This slice includes bitwise and shift operators, null-coalescing and conditional
/// expressions, plus postfix member access, invocation, and element access.
/// </remarks>
public static class CSharpExpressionGrammar
{
    private const ulong Expression = 1;
    private const ulong Assignment = 2;
    private const ulong Conditional = 3;
    private const ulong NullCoalescing = 4;
    private const ulong LogicalOr = 5;
    private const ulong LogicalAnd = 6;
    private const ulong BitwiseOr = 7;
    private const ulong BitwiseXor = 8;
    private const ulong BitwiseAnd = 9;
    private const ulong Equality = 10;
    private const ulong Relational = 11;
    private const ulong Shift = 12;
    private const ulong Additive = 13;
    private const ulong Multiplicative = 14;
    private const ulong Unary = 15;
    private const ulong Primary = 16;
    private const ulong Postfix = 17;
    private const ulong ArgumentList = 18;

    /// <summary>Creates a version-independent grammar definition for the core expression hierarchy.</summary>
    public static GrammarDefinition Create(
        ulong grammarId,
        ulong protocolCategoryId,
        IReadOnlyDictionary<CSharpExpressionToken, ulong> tokenIds)
    {
        if (grammarId == 0) throw new ArgumentOutOfRangeException(nameof(grammarId));
        if (protocolCategoryId == 0) throw new ArgumentOutOfRangeException(nameof(protocolCategoryId));
        ArgumentNullException.ThrowIfNull(tokenIds);

        foreach (var token in Enum.GetValues<CSharpExpressionToken>())
        {
            if (!tokenIds.TryGetValue(token, out var id) || id == 0)
                throw new ArgumentException($"A nonzero ProtocolAi symbol identity is required for token '{token}'.", nameof(tokenIds));
        }

        GrammarSymbol T(CSharpExpressionToken token) =>
            GrammarSymbol.Terminal(new GrammarProtocolReference(protocolCategoryId, tokenIds[token]));
        GrammarSymbol N(ulong id) => GrammarSymbol.NonTerminal(id);

        return new GrammarBuilder(grammarId, "CSharpCoreExpressions", Expression)
            .Rule(1, Expression, N(Assignment))
            .Rule(2, Assignment, N(Conditional))
            .Rule(3, Assignment, T(CSharpExpressionToken.Identifier), T(CSharpExpressionToken.Assignment), N(Assignment))
            .Rule(4, Conditional, N(NullCoalescing))
            .Rule(5, Conditional, N(NullCoalescing), T(CSharpExpressionToken.ConditionalQuestion), N(Expression), T(CSharpExpressionToken.Colon), N(Expression))
            .Rule(6, NullCoalescing, N(LogicalOr))
            .Rule(7, NullCoalescing, N(LogicalOr), T(CSharpExpressionToken.NullCoalescing), N(NullCoalescing))
            .Rule(8, LogicalOr, N(LogicalAnd))
            .Rule(9, LogicalOr, N(LogicalOr), T(CSharpExpressionToken.LogicalOr), N(LogicalAnd))
            .Rule(10, LogicalAnd, N(BitwiseOr))
            .Rule(11, LogicalAnd, N(LogicalAnd), T(CSharpExpressionToken.LogicalAnd), N(BitwiseOr))
            .Rule(12, BitwiseOr, N(BitwiseXor))
            .Rule(13, BitwiseOr, N(BitwiseOr), T(CSharpExpressionToken.BitwiseOr), N(BitwiseXor))
            .Rule(14, BitwiseXor, N(BitwiseAnd))
            .Rule(15, BitwiseXor, N(BitwiseXor), T(CSharpExpressionToken.BitwiseXor), N(BitwiseAnd))
            .Rule(16, BitwiseAnd, N(Equality))
            .Rule(17, BitwiseAnd, N(BitwiseAnd), T(CSharpExpressionToken.BitwiseAnd), N(Equality))
            .Rule(18, Equality, N(Relational))
            .Rule(19, Equality, N(Equality), T(CSharpExpressionToken.Equality), N(Relational))
            .Rule(20, Relational, N(Shift))
            .Rule(21, Relational, N(Relational), T(CSharpExpressionToken.Relational), N(Shift))
            .Rule(22, Shift, N(Additive))
            .Rule(23, Shift, N(Shift), T(CSharpExpressionToken.Shift), N(Additive))
            .Rule(24, Additive, N(Multiplicative))
            .Rule(25, Additive, N(Additive), T(CSharpExpressionToken.Additive), N(Multiplicative))
            .Rule(26, Multiplicative, N(Unary))
            .Rule(27, Multiplicative, N(Multiplicative), T(CSharpExpressionToken.Multiplicative), N(Unary))
            .Rule(28, Unary, N(Postfix))
            .Rule(29, Unary, T(CSharpExpressionToken.Unary), N(Unary))
            .Rule(30, Primary, T(CSharpExpressionToken.Identifier))
            .Rule(31, Primary, T(CSharpExpressionToken.Literal))
            .Rule(32, Primary, T(CSharpExpressionToken.OpenParenthesis), N(Expression), T(CSharpExpressionToken.CloseParenthesis))
            .Rule(33, Postfix, N(Primary))
            .Rule(34, Postfix, N(Postfix), T(CSharpExpressionToken.Dot), T(CSharpExpressionToken.Identifier))
            .Rule(35, Postfix, N(Postfix), T(CSharpExpressionToken.OpenParenthesis), T(CSharpExpressionToken.CloseParenthesis))
            .Rule(36, Postfix, N(Postfix), T(CSharpExpressionToken.OpenParenthesis), N(ArgumentList), T(CSharpExpressionToken.CloseParenthesis))
            .Rule(37, Postfix, N(Postfix), T(CSharpExpressionToken.OpenBracket), N(Expression), T(CSharpExpressionToken.CloseBracket))
            .Rule(38, ArgumentList, N(Assignment))
            .Rule(39, ArgumentList, N(ArgumentList), T(CSharpExpressionToken.Comma), N(Assignment))
            .Build();
    }
}
