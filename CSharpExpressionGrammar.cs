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
    Unary
}

/// <summary>
/// Builds a compact, precedence-layered C# expression grammar over host-owned ProtocolAi token identities.
/// </summary>
/// <remarks>
/// This is a structural expression grammar, not a C# parser or semantic model. The host supplies the
/// vocabulary category and member IDs from its selected ProtocolAi catalog. Productions encode precedence
/// and associativity; token spelling, overload resolution, type checking, and language-version policy remain
/// outside GrammarAi. Invocation and member access are intentionally separate future slices.
/// </remarks>
public static class CSharpExpressionGrammar
{
    private const ulong Expression = 1;
    private const ulong Assignment = 2;
    private const ulong LogicalOr = 3;
    private const ulong LogicalAnd = 4;
    private const ulong Equality = 5;
    private const ulong Relational = 6;
    private const ulong Additive = 7;
    private const ulong Multiplicative = 8;
    private const ulong Unary = 9;
    private const ulong Primary = 10;

    /// <summary>
    /// Creates a version-independent grammar definition for the core expression precedence hierarchy.
    /// </summary>
    /// <param name="grammarId">Host-assigned nonzero grammar identity.</param>
    /// <param name="protocolCategoryId">ProtocolAi category/protocol identity containing the supplied token members.</param>
    /// <param name="tokenIds">Complete mapping from each token role to its nonzero ProtocolAi symbol identity.</param>
    public static GrammarDefinition Create(
        ulong grammarId,
        ulong protocolCategoryId,
        IReadOnlyDictionary<CSharpExpressionToken, ulong> tokenIds)
    {
        if (grammarId == 0) throw new ArgumentOutOfRangeException(nameof(grammarId));
        if (protocolCategoryId == 0) throw new ArgumentOutOfRangeException(nameof(protocolCategoryId));
        ArgumentNullException.ThrowIfNull(tokenIds);

        var required = Enum.GetValues<CSharpExpressionToken>();
        foreach (var token in required)
        {
            if (!tokenIds.TryGetValue(token, out var id) || id == 0)
                throw new ArgumentException($"A nonzero ProtocolAi symbol identity is required for token '{token}'.", nameof(tokenIds));
        }

        GrammarSymbol T(CSharpExpressionToken token) =>
            GrammarSymbol.Terminal(new GrammarProtocolReference(protocolCategoryId, tokenIds[token]));
        GrammarSymbol N(ulong id) => GrammarSymbol.NonTerminal(id);

        return new GrammarBuilder(grammarId, "CSharpCoreExpressions", Expression)
            .Rule(1, Expression, N(Assignment))
            .Rule(2, Assignment, N(LogicalOr))
            .Rule(3, Assignment, T(CSharpExpressionToken.Identifier), T(CSharpExpressionToken.Assignment), N(Assignment))
            .Rule(4, LogicalOr, N(LogicalAnd))
            .Rule(5, LogicalOr, N(LogicalOr), T(CSharpExpressionToken.LogicalOr), N(LogicalAnd))
            .Rule(6, LogicalAnd, N(Equality))
            .Rule(7, LogicalAnd, N(LogicalAnd), T(CSharpExpressionToken.LogicalAnd), N(Equality))
            .Rule(8, Equality, N(Relational))
            .Rule(9, Equality, N(Equality), T(CSharpExpressionToken.Equality), N(Relational))
            .Rule(10, Relational, N(Additive))
            .Rule(11, Relational, N(Relational), T(CSharpExpressionToken.Relational), N(Additive))
            .Rule(12, Additive, N(Multiplicative))
            .Rule(13, Additive, N(Additive), T(CSharpExpressionToken.Additive), N(Multiplicative))
            .Rule(14, Multiplicative, N(Unary))
            .Rule(15, Multiplicative, N(Multiplicative), T(CSharpExpressionToken.Multiplicative), N(Unary))
            .Rule(16, Unary, N(Primary))
            .Rule(17, Unary, T(CSharpExpressionToken.Unary), N(Unary))
            .Rule(18, Primary, T(CSharpExpressionToken.Identifier))
            .Rule(19, Primary, T(CSharpExpressionToken.Literal))
            .Rule(20, Primary, T(CSharpExpressionToken.OpenParenthesis), N(Expression), T(CSharpExpressionToken.CloseParenthesis))
            .Build();
    }
}
