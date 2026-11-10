namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Host-owned vocabulary roles required by the bounded C# member declaration grammar.</summary>
public enum CSharpDeclarationToken
{
    Identifier,
    Modifier,
    TypeReference,
    ExpressionReference,
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

/// <summary>
/// Builds a bounded grammar for method, property, field, and parameter declaration shapes.
/// </summary>
/// <remarks>
/// TypeReference and ExpressionReference are opaque host vocabulary references to independently mapped
/// type/expression structures; this grammar deliberately does not copy their productions. Modifier and
/// accessor tokens are likewise supplied by the host. This is a declaration-shape resource, not a complete
/// C# declaration grammar: constructors, operators, events, indexers, attributes, generic parameters,
/// constraints, explicit interface implementations, bodies, and language-version policy are excluded.
/// </remarks>
public static class CSharpDeclarationGrammar
{
    private const ulong MemberDeclaration = 1;
    private const ulong MethodDeclaration = 2;
    private const ulong PropertyDeclaration = 3;
    private const ulong FieldDeclaration = 4;
    private const ulong ModifierList = 5;
    private const ulong ParameterList = 6;
    private const ulong Parameter = 7;
    private const ulong ParameterTail = 8;
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
            .Rule(5, PropertyDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.OpenBrace),
                N(AccessorList), T(CSharpDeclarationToken.CloseBrace))
            .Rule(6, FieldDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.Semicolon))
            .Rule(7, ModifierList)
            .Rule(8, ModifierList, N(ModifierList), T(CSharpDeclarationToken.Modifier))
            .Rule(9, ParameterList)
            .Rule(10, ParameterList, N(Parameter), N(ParameterTail))
            .Rule(11, ParameterTail)
            .Rule(12, ParameterTail, T(CSharpDeclarationToken.Comma), N(Parameter), N(ParameterTail))
            .Rule(13, Parameter, T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.Identifier))
            .Rule(14, Parameter, T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.Identifier),
                T(CSharpDeclarationToken.Semicolon), T(CSharpDeclarationToken.ExpressionReference))
            .Rule(15, AccessorList, N(Accessor))
            .Rule(16, AccessorList, N(Accessor), N(Accessor))
            .Rule(17, Accessor, T(CSharpDeclarationToken.GetAccessor), T(CSharpDeclarationToken.Semicolon))
            .Rule(18, Accessor, T(CSharpDeclarationToken.SetAccessor), T(CSharpDeclarationToken.Semicolon))
            .Rule(19, Accessor, T(CSharpDeclarationToken.InitAccessor), T(CSharpDeclarationToken.Semicolon))
            .Build();
    }
}
