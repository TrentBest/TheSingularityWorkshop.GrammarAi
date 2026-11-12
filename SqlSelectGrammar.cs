namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Vocabulary roles required by the bounded SQL SELECT structural grammar.</summary>
public enum SqlSelectGrammarToken
{
    Select,
    Distinct,
    All,
    Star,
    Comma,
    From,
    Identifier,
    As,
    Join,
    Inner,
    Left,
    Right,
    Outer,
    On,
    Where,
    And,
    Or,
    Not,
    OpenParen,
    CloseParen,
    Equals,
    NotEquals,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    Is,
    Null,
    Literal,
    Parameter
}

/// <summary>
/// Builds a bounded, dialect-neutral SQL SELECT core over host-owned ProtocolAi token identities.
/// </summary>
/// <remarks>
/// The resource describes a deliberately small structural subset: projection lists, one FROM
/// source, inner/left/right joins with ON predicates, optional WHERE clauses, boolean composition,
/// parenthesized predicates, comparisons, and IS [NOT] NULL. It is not a complete SQL standard
/// or vendor-dialect grammar. Tokenization, quoted identifiers, literal and parameter syntax,
/// identifier resolution, type compatibility, join semantics, authorization, and query execution
/// remain host responsibilities. Dialect-specific extensions must use separately versioned resources.
/// </remarks>
public static class SqlSelectGrammar
{
    private const ulong Query = 1;
    private const ulong SelectStatement = 2;
    private const ulong SelectModifier = 3;
    private const ulong SelectList = 4;
    private const ulong SelectItem = 5;
    private const ulong Expression = 6;
    private const ulong TableReference = 7;
    private const ulong JoinList = 8;
    private const ulong Join = 9;
    private const ulong JoinType = 10;
    private const ulong WhereOptional = 11;
    private const ulong BooleanExpression = 12;
    private const ulong Predicate = 13;
    private const ulong ComparisonOperator = 14;

    /// <summary>Creates the bounded SQL SELECT grammar using host-supplied token identities.</summary>
    /// <param name="grammarId">Nonzero identity assigned by the host to this grammar resource.</param>
    /// <param name="protocolCategoryId">Nonzero ProtocolAi category identity containing the SQL tokens.</param>
    /// <param name="tokenIds">Complete mapping from each SQL token role to its nonzero ProtocolAi member identity.</param>
    /// <returns>An immutable, integer-first grammar definition.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The grammar or category identity is zero.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="tokenIds"/> is null.</exception>
    /// <exception cref="ArgumentException">A required token identity is missing or zero.</exception>
    public static GrammarDefinition Create(
        ulong grammarId,
        ulong protocolCategoryId,
        IReadOnlyDictionary<SqlSelectGrammarToken, ulong> tokenIds)
    {
        if (grammarId == 0) throw new ArgumentOutOfRangeException(nameof(grammarId));
        if (protocolCategoryId == 0) throw new ArgumentOutOfRangeException(nameof(protocolCategoryId));
        ArgumentNullException.ThrowIfNull(tokenIds);

        foreach (var token in Enum.GetValues<SqlSelectGrammarToken>())
        {
            if (!tokenIds.TryGetValue(token, out var id) || id == 0)
                throw new ArgumentException($"A nonzero ProtocolAi symbol identity is required for token '{token}'.", nameof(tokenIds));
        }

        GrammarSymbol T(SqlSelectGrammarToken token) =>
            GrammarSymbol.Terminal(new GrammarProtocolReference(protocolCategoryId, tokenIds[token]));
        GrammarSymbol N(ulong id) => GrammarSymbol.NonTerminal(id);

        return new GrammarBuilder(grammarId, "SQL-SELECT-Core-Bounded", Query)
            .Rule(1, Query, N(SelectStatement))
            .Rule(2, SelectStatement, T(SqlSelectGrammarToken.Select), N(SelectModifier),
                N(SelectList), T(SqlSelectGrammarToken.From), N(TableReference),
                N(JoinList), N(WhereOptional))
            .Rule(3, SelectModifier)
            .Rule(4, SelectModifier, T(SqlSelectGrammarToken.Distinct))
            .Rule(5, SelectModifier, T(SqlSelectGrammarToken.All))
            .Rule(6, SelectList, N(SelectItem))
            .Rule(7, SelectList, N(SelectList), T(SqlSelectGrammarToken.Comma), N(SelectItem))
            .Rule(8, SelectItem, T(SqlSelectGrammarToken.Star))
            .Rule(9, SelectItem, N(Expression))
            .Rule(10, SelectItem, N(Expression), T(SqlSelectGrammarToken.As), T(SqlSelectGrammarToken.Identifier))
            .Rule(11, SelectItem, N(Expression), T(SqlSelectGrammarToken.Identifier))
            .Rule(12, Expression, T(SqlSelectGrammarToken.Identifier))
            .Rule(13, Expression, T(SqlSelectGrammarToken.Literal))
            .Rule(14, Expression, T(SqlSelectGrammarToken.Parameter))
            .Rule(15, TableReference, T(SqlSelectGrammarToken.Identifier))
            .Rule(16, TableReference, T(SqlSelectGrammarToken.Identifier),
                T(SqlSelectGrammarToken.As), T(SqlSelectGrammarToken.Identifier))
            .Rule(17, TableReference, T(SqlSelectGrammarToken.Identifier), T(SqlSelectGrammarToken.Identifier))
            .Rule(18, JoinList)
            .Rule(19, JoinList, N(JoinList), N(Join))
            .Rule(20, Join, N(JoinType), T(SqlSelectGrammarToken.Join), N(TableReference),
                T(SqlSelectGrammarToken.On), N(BooleanExpression))
            .Rule(21, JoinType)
            .Rule(22, JoinType, T(SqlSelectGrammarToken.Inner))
            .Rule(23, JoinType, T(SqlSelectGrammarToken.Left), T(SqlSelectGrammarToken.Outer))
            .Rule(24, JoinType, T(SqlSelectGrammarToken.Right), T(SqlSelectGrammarToken.Outer))
            .Rule(25, WhereOptional)
            .Rule(26, WhereOptional, T(SqlSelectGrammarToken.Where), N(BooleanExpression))
            .Rule(27, BooleanExpression, N(Predicate))
            .Rule(28, BooleanExpression, N(BooleanExpression), T(SqlSelectGrammarToken.And), N(BooleanExpression))
            .Rule(29, BooleanExpression, N(BooleanExpression), T(SqlSelectGrammarToken.Or), N(BooleanExpression))
            .Rule(30, BooleanExpression, T(SqlSelectGrammarToken.Not), N(BooleanExpression))
            .Rule(31, BooleanExpression, T(SqlSelectGrammarToken.OpenParen),
                N(BooleanExpression), T(SqlSelectGrammarToken.CloseParen))
            .Rule(32, Predicate, N(Expression), N(ComparisonOperator), N(Expression))
            .Rule(33, Predicate, N(Expression), T(SqlSelectGrammarToken.Is), T(SqlSelectGrammarToken.Null))
            .Rule(34, Predicate, N(Expression), T(SqlSelectGrammarToken.Is),
                T(SqlSelectGrammarToken.Not), T(SqlSelectGrammarToken.Null))
            .Rule(35, ComparisonOperator, T(SqlSelectGrammarToken.Equals))
            .Rule(36, ComparisonOperator, T(SqlSelectGrammarToken.NotEquals))
            .Rule(37, ComparisonOperator, T(SqlSelectGrammarToken.LessThan))
            .Rule(38, ComparisonOperator, T(SqlSelectGrammarToken.LessThanOrEqual))
            .Rule(39, ComparisonOperator, T(SqlSelectGrammarToken.GreaterThan))
            .Rule(40, ComparisonOperator, T(SqlSelectGrammarToken.GreaterThanOrEqual))
            .Build();
    }
}
