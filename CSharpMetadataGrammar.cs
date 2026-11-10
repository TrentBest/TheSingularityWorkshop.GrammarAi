namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Host-vocabulary roles used by composable C# attribute and generic-constraint maps.</summary>
public enum CSharpMetadataToken
{
    Identifier,
    OpenBracket,
    CloseBracket,
    OpenParenthesis,
    CloseParenthesis,
    OpenAngle,
    CloseAngle,
    Colon,
    Comma,
    Dot,
    NullableMarker,
    Equals,
    TargetSpecifier,
    AttributeArgumentReference,
    TypeReference,
    ExpressionReference,
    ClassConstraint,
    StructConstraint,
    UnmanagedConstraint,
    NotNullConstraint,
    DefaultConstraint,
    NewKeyword,
    InKeyword,
    OutKeyword
}

/// <summary>Builds composable C# attribute and generic-constraint structures.</summary>
/// <remarks>
/// Attribute arguments and type/expression references are opaque host-owned structures. This grammar
/// describes sections, qualified attribute names, argument-list boundaries, generic parameter lists,
/// and constraint clauses. It does not bind attribute types, resolve generic parameters, enforce
/// constraint satisfiability/order, or attach metadata to a particular declaration kind; the host
/// composes these maps at the appropriate declaration slots. The root exposes each independently
/// composable structure as an alternative.
/// </remarks>
public static class CSharpMetadataGrammar
{
    private const ulong AttributeSection = 1;
    private const ulong AttributeTarget = 2;
    private const ulong AttributeList = 3;
    private const ulong Attribute = 4;
    private const ulong AttributeName = 5;
    private const ulong AttributeNameTail = 6;
    private const ulong AttributeArguments = 7;
    private const ulong AttributeArgumentTail = 8;
    private const ulong GenericParameterList = 9;
    private const ulong GenericParameterTail = 10;
    private const ulong GenericParameter = 11;
    private const ulong ConstraintClause = 12;
    private const ulong ConstraintList = 13;
    private const ulong ConstraintTail = 14;
    private const ulong TypeParameterName = 15;
    private const ulong TypeConstraint = 16;
    private const ulong PrimaryConstraint = 17;
    private const ulong ConstructorConstraint = 18;
    private const ulong MetadataRoot = 19;
    private const ulong TypeParameterVariance = 20;

    /// <summary>Creates the metadata grammar using host-owned ProtocolAi symbol identities.</summary>
    public static GrammarDefinition Create(
        ulong grammarId,
        ulong protocolCategoryId,
        IReadOnlyDictionary<CSharpMetadataToken, ulong> tokenIds)
    {
        if (grammarId == 0) throw new ArgumentOutOfRangeException(nameof(grammarId));
        if (protocolCategoryId == 0) throw new ArgumentOutOfRangeException(nameof(protocolCategoryId));
        ArgumentNullException.ThrowIfNull(tokenIds);

        foreach (var token in Enum.GetValues<CSharpMetadataToken>())
            if (!tokenIds.TryGetValue(token, out var id) || id == 0)
                throw new ArgumentException($"A nonzero ProtocolAi symbol identity is required for token '{token}'.", nameof(tokenIds));

        GrammarSymbol T(CSharpMetadataToken token) =>
            GrammarSymbol.Terminal(new GrammarProtocolReference(protocolCategoryId, tokenIds[token]));
        GrammarSymbol N(ulong id) => GrammarSymbol.NonTerminal(id);

        return new GrammarBuilder(grammarId, "CSharpAttributesAndGenericConstraints", MetadataRoot)
            .Rule(1, MetadataRoot, N(AttributeSection))
            .Rule(2, MetadataRoot, N(GenericParameterList))
            .Rule(3, MetadataRoot, N(ConstraintClause))
            .Rule(4, AttributeSection, T(CSharpMetadataToken.OpenBracket), N(AttributeTarget),
                N(AttributeList), T(CSharpMetadataToken.CloseBracket))
            .Rule(5, AttributeTarget)
            .Rule(6, AttributeTarget, T(CSharpMetadataToken.TargetSpecifier), T(CSharpMetadataToken.Colon))
            .Rule(7, AttributeList, N(Attribute))
            .Rule(8, AttributeList, N(Attribute), T(CSharpMetadataToken.Comma), N(AttributeList))
            .Rule(9, Attribute, N(AttributeName), N(AttributeArguments))
            .Rule(10, AttributeName, T(CSharpMetadataToken.Identifier), N(AttributeNameTail))
            .Rule(11, AttributeNameTail)
            .Rule(12, AttributeNameTail, T(CSharpMetadataToken.Dot),
                T(CSharpMetadataToken.Identifier), N(AttributeNameTail))
            .Rule(13, AttributeArguments)
            .Rule(14, AttributeArguments, T(CSharpMetadataToken.OpenParenthesis),
                N(AttributeArgumentTail), T(CSharpMetadataToken.CloseParenthesis))
            .Rule(15, AttributeArgumentTail)
            .Rule(16, AttributeArgumentTail, T(CSharpMetadataToken.AttributeArgumentReference))
            .Rule(17, AttributeArgumentTail, T(CSharpMetadataToken.AttributeArgumentReference),
                T(CSharpMetadataToken.Comma), N(AttributeArgumentTail))
            .Rule(18, GenericParameterList, T(CSharpMetadataToken.OpenAngle),
                N(GenericParameter), N(GenericParameterTail), T(CSharpMetadataToken.CloseAngle))
            .Rule(19, GenericParameterTail)
            .Rule(20, GenericParameterTail, T(CSharpMetadataToken.Comma),
                N(GenericParameter), N(GenericParameterTail))
            .Rule(21, GenericParameter, N(TypeParameterVariance), N(TypeParameterName))
            .Rule(22, TypeParameterName, T(CSharpMetadataToken.Identifier))
            .Rule(23, ConstraintClause, T(CSharpMetadataToken.Identifier),
                T(CSharpMetadataToken.Colon), N(ConstraintList))
            .Rule(24, ConstraintList, N(TypeConstraint))
            .Rule(25, ConstraintList, N(TypeConstraint), T(CSharpMetadataToken.Comma),
                N(ConstraintTail))
            .Rule(26, ConstraintTail, N(TypeConstraint))
            .Rule(27, ConstraintTail, N(TypeConstraint), T(CSharpMetadataToken.Comma),
                N(ConstraintTail))
            .Rule(28, TypeConstraint, N(PrimaryConstraint))
            .Rule(29, TypeConstraint, T(CSharpMetadataToken.TypeReference))
            .Rule(30, TypeConstraint, N(ConstructorConstraint))
            .Rule(31, PrimaryConstraint, T(CSharpMetadataToken.ClassConstraint))
            .Rule(32, PrimaryConstraint, T(CSharpMetadataToken.ClassConstraint),
                T(CSharpMetadataToken.NullableMarker))
            .Rule(33, PrimaryConstraint, T(CSharpMetadataToken.StructConstraint))
            .Rule(34, PrimaryConstraint, T(CSharpMetadataToken.UnmanagedConstraint))
            .Rule(35, PrimaryConstraint, T(CSharpMetadataToken.NotNullConstraint))
            .Rule(36, PrimaryConstraint, T(CSharpMetadataToken.DefaultConstraint))
.Rule(37, ConstructorConstraint, T(CSharpMetadataToken.NewKeyword),
                T(CSharpMetadataToken.OpenParenthesis), T(CSharpMetadataToken.CloseParenthesis))
            // Variance markers are represented structurally; the host restricts them to
            // declaration contexts where C# permits variance (for example interfaces/delegates).
            .Rule(38, TypeParameterVariance)
            .Rule(39, TypeParameterVariance, T(CSharpMetadataToken.InKeyword))
            .Rule(40, TypeParameterVariance, T(CSharpMetadataToken.OutKeyword))
            .Build();
    }
}
