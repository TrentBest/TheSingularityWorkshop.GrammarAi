namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Protocol vocabulary roles required by the bounded C# type grammar.</summary>
public enum CSharpTypeToken
{
    Identifier,
    Dot,
    LessThan,
    GreaterThan,
    Comma,
    OpenBracket,
    CloseBracket,
    Question,
    OpenParenthesis,
    CloseParenthesis
}

/// <summary>
/// Builds a reusable structural grammar for named, qualified, generic, nullable, array, and tuple C# types.
/// </summary>
/// <remarks>
/// This is a syntax-shape resource, not a parser or type resolver. The host supplies token identities from
/// its selected ProtocolAi catalog. It intentionally excludes aliases, pointer/function-pointer types,
/// tuple element names, nullable-reference annotations, and language-version-specific forms. Array rank
/// specifiers are represented as one or more comma-separated dimensions; semantic rank limits are external.
/// </remarks>
public static class CSharpTypeGrammar
{
    private const ulong Type = 1;
    private const ulong NullableType = 2;
    private const ulong ArrayType = 3;
    private const ulong NamedType = 4;
    private const ulong QualifiedName = 5;
    private const ulong TypeArgumentList = 6;
    private const ulong TupleType = 7;
    private const ulong TupleElements = 8;
    private const ulong ArrayRank = 9;

    /// <summary>Creates the bounded C# type grammar over host-owned ProtocolAi token identities.</summary>
    public static GrammarDefinition Create(
        ulong grammarId,
        ulong protocolCategoryId,
        IReadOnlyDictionary<CSharpTypeToken, ulong> tokenIds)
    {
        if (grammarId == 0) throw new ArgumentOutOfRangeException(nameof(grammarId));
        if (protocolCategoryId == 0) throw new ArgumentOutOfRangeException(nameof(protocolCategoryId));
        ArgumentNullException.ThrowIfNull(tokenIds);

        foreach (var token in Enum.GetValues<CSharpTypeToken>())
        {
            if (!tokenIds.TryGetValue(token, out var id) || id == 0)
                throw new ArgumentException($"A nonzero ProtocolAi symbol identity is required for token '{token}'.", nameof(tokenIds));
        }

        GrammarSymbol T(CSharpTypeToken token) =>
            GrammarSymbol.Terminal(new GrammarProtocolReference(protocolCategoryId, tokenIds[token]));
        GrammarSymbol N(ulong id) => GrammarSymbol.NonTerminal(id);

        return new GrammarBuilder(grammarId, "CSharpCoreTypes", Type)
            .Rule(1, Type, N(NullableType))
            .Rule(2, Type, N(ArrayType))
            .Rule(3, Type, N(NamedType))
            .Rule(4, Type, N(TupleType))
            .Rule(5, NullableType, N(NamedType), T(CSharpTypeToken.Question))
            .Rule(6, NullableType, N(ArrayType), T(CSharpTypeToken.Question))
            .Rule(7, ArrayType, N(NamedType), N(ArrayRank))
            .Rule(8, ArrayType, N(NamedType), N(ArrayRank), T(CSharpTypeToken.Question))
            .Rule(9, NamedType, N(QualifiedName))
            .Rule(10, NamedType, N(QualifiedName), T(CSharpTypeToken.LessThan), N(TypeArgumentList), T(CSharpTypeToken.GreaterThan))
            .Rule(11, QualifiedName, T(CSharpTypeToken.Identifier))
            .Rule(12, QualifiedName, N(QualifiedName), T(CSharpTypeToken.Dot), T(CSharpTypeToken.Identifier))
            .Rule(13, TypeArgumentList, N(Type))
            .Rule(14, TypeArgumentList, N(TypeArgumentList), T(CSharpTypeToken.Comma), N(Type))
            .Rule(15, TupleType, T(CSharpTypeToken.OpenParenthesis), N(TupleElements), T(CSharpTypeToken.CloseParenthesis))
            .Rule(16, TupleElements, N(Type), T(CSharpTypeToken.Comma), N(Type))
            .Rule(17, TupleElements, N(TupleElements), T(CSharpTypeToken.Comma), N(Type))
            .Rule(18, ArrayRank, T(CSharpTypeToken.OpenBracket), T(CSharpTypeToken.CloseBracket))
            .Rule(19, ArrayRank, T(CSharpTypeToken.OpenBracket), T(CSharpTypeToken.Comma), T(CSharpTypeToken.CloseBracket))
            .Rule(20, ArrayRank, N(ArrayRank), T(CSharpTypeToken.Comma))
            .Build();
    }
}
