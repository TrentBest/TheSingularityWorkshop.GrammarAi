namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Host-owned vocabulary roles required by the bounded C# member declaration grammar.</summary>
public enum CSharpDeclarationToken
{
    Identifier,
    Modifier,
    TypeReference,
    ExpressionReference,
    BlockReference,
    Arrow,
    Assignment,
    OpenParenthesis,
    CloseParenthesis,
    Comma,
    Semicolon,
    OpenBrace,
    CloseBrace,
    GetAccessor,
    SetAccessor,
    InitAccessor
}

/// <summary>Builds a bounded grammar for C# method, property, field, and parameter declaration shapes.</summary>
/// <remarks>
/// TypeReference, ExpressionReference, and BlockReference are opaque host vocabulary references to
/// independently mapped structures; this grammar deliberately does not copy their productions. Modifier
/// and accessor tokens are likewise supplied by the host. Constructors, operators, events, indexers,
/// attributes, generic parameters, constraints, explicit interface implementations, 
/// parameter attributes/modifiers, and language-version policy are excluded.
/// </remarks>
public static class CSharpDeclarationGrammar
{
    private const ulong MemberDeclaration = 1;
    private const ulong MethodDeclaration = 2;
    private const ulong PropertyDeclaration = 3;
    private const ulong FieldDeclaration = 4;
    private const ulong ModifierList = 5;
    private const ulong ParameterList = 6;
    private const ulong ParameterTail = 7;
    private const ulong Parameter = 8;
    private const ulong AccessorList = 9;
    private const ulong Accessor = 10;

    /// <summary>Creates the bounded declaration grammar over host-owned ProtocolAi identities.</summary>
    public static GrammarDefinition Create(
        ulong grammarId,
        ulong protocolCategoryId,
        IReadOnlyDictionary<CSharpDeclarationToken, ulong> tokenIds)
    {
        if (grammarId == 0) throw new ArgumentOutOfRangeException(nameof(grammarId));
        if (protocolCategoryId == 0) throw new ArgumentOutOfRangeException(nameof(protocolCategoryId));
        ArgumentNullException.ThrowIfNull(tokenIds);

        foreach (var token in Enum.GetValues<CSharpDeclarationToken>())
        {
            if (!tokenIds.TryGetValue(token, out var id) || id == 0)
                throw new ArgumentException($"A nonzero ProtocolAi symbol identity is required for token '{token}'.", nameof(tokenIds));
        }

        GrammarSymbol T(CSharpDeclarationToken token) =>
            GrammarSymbol.Terminal(new GrammarProtocolReference(protocolCategoryId, tokenIds[token]));
        GrammarSymbol N(ulong id) => GrammarSymbol.NonTerminal(id);

        return new GrammarBuilder(grammarId, "CSharpMemberDeclarations", MemberDeclaration)
            .Rule(1, MemberDeclaration, N(MethodDeclaration))
            .Rule(2, MemberDeclaration, N(PropertyDeclaration))
            .Rule(3, MemberDeclaration, N(FieldDeclaration))
            .Rule(4, MethodDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.OpenParenthesis),
                N(ParameterList), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Semicolon))
            .Rule(5, MethodDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.OpenParenthesis),
                N(ParameterList), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(6, MethodDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.OpenParenthesis),
                N(ParameterList), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(7, PropertyDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.OpenBrace),
                N(AccessorList), T(CSharpDeclarationToken.CloseBrace))
            .Rule(8, FieldDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.Semicolon))
            .Rule(9, ModifierList)
            .Rule(10, ModifierList, N(ModifierList), T(CSharpDeclarationToken.Modifier))
            .Rule(11, ParameterList)
            .Rule(12, ParameterList, N(Parameter), N(ParameterTail))
            .Rule(13, ParameterTail)
            .Rule(14, ParameterTail, T(CSharpDeclarationToken.Comma), N(Parameter), N(ParameterTail))
            .Rule(15, Parameter, T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.Identifier))
            .Rule(16, Parameter, T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.Identifier),
                T(CSharpDeclarationToken.Assignment), T(CSharpDeclarationToken.ExpressionReference))
            .Rule(17, AccessorList, N(Accessor))
            .Rule(18, AccessorList, N(Accessor), N(Accessor))
            .Rule(19, Accessor, T(CSharpDeclarationToken.GetAccessor), T(CSharpDeclarationToken.Semicolon))
            .Rule(20, Accessor, T(CSharpDeclarationToken.SetAccessor), T(CSharpDeclarationToken.Semicolon))
            .Rule(21, Accessor, T(CSharpDeclarationToken.InitAccessor), T(CSharpDeclarationToken.Semicolon))
            .Rule(22, Accessor, T(CSharpDeclarationToken.GetAccessor), T(CSharpDeclarationToken.BlockReference))
            .Rule(23, Accessor, T(CSharpDeclarationToken.GetAccessor), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(24, Accessor, T(CSharpDeclarationToken.SetAccessor), T(CSharpDeclarationToken.BlockReference))
            .Rule(25, Accessor, T(CSharpDeclarationToken.SetAccessor), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(26, Accessor, T(CSharpDeclarationToken.InitAccessor), T(CSharpDeclarationToken.BlockReference))
            .Rule(27, Accessor, T(CSharpDeclarationToken.InitAccessor), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Build();
    }
}
