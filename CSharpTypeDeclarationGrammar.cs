namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Host-vocabulary roles required by the bounded C# type-declaration grammar.</summary>
public enum CSharpTypeDeclarationToken
{
    Identifier,
    Modifier,
    TypeReference,
    AttributeSectionReference,
    GenericParameterListReference,
    ConstraintClauseReference,
    BaseTypeListReference,
    TypeBodyReference,
    ParameterListReference,
    ClassKeyword,
    StructKeyword,
    InterfaceKeyword,
    RecordKeyword,
    DelegateKeyword,
    OpenParenthesis,
    CloseParenthesis,
    Semicolon
}

/// <summary>Builds bounded class, struct, interface, record, and delegate declaration shapes.</summary>
/// <remarks>
/// Attribute sections, generic parameter lists, constraints, base lists, type bodies, and delegate
/// parameter lists are opaque host-owned references. This resource composes those structures without
/// duplicating CSharpMetadataGrammar or member-declaration productions. Variance is syntactically
/// representable in the referenced generic parameter list; the host must reject it outside legal
/// interface/delegate contexts. The host also owns modifier legality, base-type semantics, generic
/// constraint ordering/satisfiability, and declaration-kind-specific validation. This is not a
/// complete C# grammar or source parser.
/// </remarks>
public static class CSharpTypeDeclarationGrammar
{
    private const ulong TypeDeclaration = 1;
    private const ulong ClassDeclaration = 2;
    private const ulong StructDeclaration = 3;
    private const ulong InterfaceDeclaration = 4;
    private const ulong RecordClassDeclaration = 5;
    private const ulong RecordStructDeclaration = 6;
    private const ulong DelegateDeclaration = 7;
    private const ulong AttributeSections = 8;
    private const ulong ModifierList = 9;
    private const ulong GenericParameters = 10;
    private const ulong BaseList = 11;
    private const ulong ConstraintClauses = 12;
    private const ulong RecordPrimaryParameters = 13;

    /// <summary>Creates the bounded type-declaration grammar over host-owned ProtocolAi identities.</summary>
    public static GrammarDefinition Create(
        ulong grammarId,
        ulong protocolCategoryId,
        IReadOnlyDictionary<CSharpTypeDeclarationToken, ulong> tokenIds)
    {
        if (grammarId == 0) throw new ArgumentOutOfRangeException(nameof(grammarId));
        if (protocolCategoryId == 0) throw new ArgumentOutOfRangeException(nameof(protocolCategoryId));
        ArgumentNullException.ThrowIfNull(tokenIds);

        foreach (var token in Enum.GetValues<CSharpTypeDeclarationToken>())
            if (!tokenIds.TryGetValue(token, out var id) || id == 0)
                throw new ArgumentException($"A nonzero ProtocolAi symbol identity is required for token '{token}'.", nameof(tokenIds));

        GrammarSymbol T(CSharpTypeDeclarationToken token) =>
            GrammarSymbol.Terminal(new GrammarProtocolReference(protocolCategoryId, tokenIds[token]));
        GrammarSymbol N(ulong id) => GrammarSymbol.NonTerminal(id);

        return new GrammarBuilder(grammarId, "CSharpTypeDeclarations", TypeDeclaration)
            .Rule(1, TypeDeclaration, N(ClassDeclaration))
            .Rule(2, TypeDeclaration, N(StructDeclaration))
            .Rule(3, TypeDeclaration, N(InterfaceDeclaration))
            .Rule(4, TypeDeclaration, N(RecordClassDeclaration))
            .Rule(5, TypeDeclaration, N(RecordStructDeclaration))
            .Rule(6, TypeDeclaration, N(DelegateDeclaration))
            .Rule(7, AttributeSections)
            .Rule(8, AttributeSections, N(AttributeSections), T(CSharpTypeDeclarationToken.AttributeSectionReference))
            .Rule(9, ModifierList)
            .Rule(10, ModifierList, N(ModifierList), T(CSharpTypeDeclarationToken.Modifier))
            .Rule(11, GenericParameters)
            .Rule(12, GenericParameters, T(CSharpTypeDeclarationToken.GenericParameterListReference))
            .Rule(13, BaseList)
            .Rule(14, BaseList, T(CSharpTypeDeclarationToken.BaseTypeListReference))
            .Rule(15, ConstraintClauses)
            .Rule(16, ConstraintClauses, T(CSharpTypeDeclarationToken.ConstraintClauseReference),
                N(ConstraintClauses))
            .Rule(17, RecordPrimaryParameters)
            .Rule(18, RecordPrimaryParameters, T(CSharpTypeDeclarationToken.OpenParenthesis),
                T(CSharpTypeDeclarationToken.ParameterListReference), T(CSharpTypeDeclarationToken.CloseParenthesis))
            .Rule(19, ClassDeclaration, N(AttributeSections), N(ModifierList),
                T(CSharpTypeDeclarationToken.ClassKeyword), T(CSharpTypeDeclarationToken.Identifier),
                N(GenericParameters), N(BaseList), N(ConstraintClauses), T(CSharpTypeDeclarationToken.TypeBodyReference))
            .Rule(20, StructDeclaration, N(AttributeSections), N(ModifierList),
                T(CSharpTypeDeclarationToken.StructKeyword), T(CSharpTypeDeclarationToken.Identifier),
                N(GenericParameters), N(BaseList), N(ConstraintClauses), T(CSharpTypeDeclarationToken.TypeBodyReference))
            .Rule(21, InterfaceDeclaration, N(AttributeSections), N(ModifierList),
                T(CSharpTypeDeclarationToken.InterfaceKeyword), T(CSharpTypeDeclarationToken.Identifier),
                N(GenericParameters), N(BaseList), N(ConstraintClauses), T(CSharpTypeDeclarationToken.TypeBodyReference))
            .Rule(22, RecordClassDeclaration, N(AttributeSections), N(ModifierList),
                T(CSharpTypeDeclarationToken.RecordKeyword), T(CSharpTypeDeclarationToken.Identifier),
                N(GenericParameters), N(RecordPrimaryParameters), N(BaseList), N(ConstraintClauses),
                T(CSharpTypeDeclarationToken.TypeBodyReference))
            .Rule(23, RecordClassDeclaration, N(AttributeSections), N(ModifierList),
                T(CSharpTypeDeclarationToken.RecordKeyword), T(CSharpTypeDeclarationToken.ClassKeyword),
                T(CSharpTypeDeclarationToken.Identifier), N(GenericParameters), N(RecordPrimaryParameters),
                N(BaseList), N(ConstraintClauses), T(CSharpTypeDeclarationToken.TypeBodyReference))
            .Rule(24, RecordStructDeclaration, N(AttributeSections), N(ModifierList),
                T(CSharpTypeDeclarationToken.RecordKeyword), T(CSharpTypeDeclarationToken.StructKeyword),
                T(CSharpTypeDeclarationToken.Identifier), N(GenericParameters), N(RecordPrimaryParameters),
                N(BaseList), N(ConstraintClauses), T(CSharpTypeDeclarationToken.TypeBodyReference))
            .Rule(25, DelegateDeclaration, N(AttributeSections), N(ModifierList),
                T(CSharpTypeDeclarationToken.DelegateKeyword), T(CSharpTypeDeclarationToken.TypeReference),
                T(CSharpTypeDeclarationToken.Identifier), N(GenericParameters),
                T(CSharpTypeDeclarationToken.OpenParenthesis), T(CSharpTypeDeclarationToken.ParameterListReference),
                T(CSharpTypeDeclarationToken.CloseParenthesis), N(ConstraintClauses),
                T(CSharpTypeDeclarationToken.Semicolon))
            .Build();
    }
}
