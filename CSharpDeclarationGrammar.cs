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
    InitAccessor,
    ThisKeyword,
    BaseKeyword,
    Colon,
    OpenBracket,
    CloseBracket,
    EventKeyword,
    AddAccessor,
    RemoveAccessor,
    OperatorKeyword,
    OperatorToken,
    ImplicitKeyword,
    ExplicitKeyword,
    CheckedKeyword,
    CheckedOperatorToken,
    AttributeSectionReference,
    GenericParameterListReference,
    ConstraintClauseReference,
    ParameterAttributeSectionReference,
    ExplicitInterfaceSpecifierReference
}

/// <summary>Builds a bounded grammar for selected C# member declaration shapes.</summary>
/// <remarks>
/// TypeReference, ExpressionReference, and BlockReference are opaque host vocabulary references to
/// independently mapped structures; this grammar deliberately does not copy their productions. Modifier
/// and accessor tokens are likewise supplied by the host. Constructor argument lists use expression
/// references, not parameter declarations. Field-like events and custom add/remove event accessors are
/// represented separately from property accessors. Ordinary operator overloads and implicit/explicit conversion operators
/// are represented as distinct declaration shapes; operand and conversion semantics remain host-owned. Declaration-level
/// attributes are composed for methods, properties, fields, constructors, indexers, events, operators, and conversions;
/// generic method metadata and parameter attributes use opaque host references. Generic constraints and parameter modifiers are not composed into this member slice. Explicit-interface specifiers are opaque host references; this slice does not resolve interface identity or implementation compatibility. The target language baseline is
/// described separately by CSharpGrammarProfile; that metadata does not make this bounded slice a complete or
/// compiler-conformance-validated C# grammar.
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
    private const ulong ConstructorDeclaration = 11;
    private const ulong ConstructorInitializer = 12;
    private const ulong ConstructorArgumentList = 13;
    private const ulong IndexerDeclaration = 14;
    private const ulong EventDeclaration = 15;
    private const ulong EventAccessorList = 16;
    private const ulong ConversionDeclaration = 18;
    private const ulong UnaryOperatorDeclaration = 19;
    private const ulong BinaryOperatorDeclaration = 20;
    private const ulong DeclarationAttributes = 21;
    private const ulong GenericMethodMetadata = 22;
    private const ulong GenericMethodConstraintTail = 23;
    private const ulong ParameterAttributes = 24;

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
            .Rule(15, Parameter, N(ParameterAttributes), T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.Identifier))
            .Rule(16, Parameter, N(ParameterAttributes), T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.Identifier),
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
            .Rule(28, MemberDeclaration, N(ConstructorDeclaration))
            .Rule(29, ConstructorDeclaration, N(ModifierList), T(CSharpDeclarationToken.Identifier),
                T(CSharpDeclarationToken.OpenParenthesis), N(ParameterList),
                T(CSharpDeclarationToken.CloseParenthesis), N(ConstructorInitializer),
                T(CSharpDeclarationToken.BlockReference))
            .Rule(30, ConstructorDeclaration, N(ModifierList), T(CSharpDeclarationToken.Identifier),
                T(CSharpDeclarationToken.OpenParenthesis), N(ParameterList),
                T(CSharpDeclarationToken.CloseParenthesis), N(ConstructorInitializer),
                T(CSharpDeclarationToken.Arrow), T(CSharpDeclarationToken.ExpressionReference),
                T(CSharpDeclarationToken.Semicolon))
            .Rule(31, ConstructorDeclaration, N(ModifierList), T(CSharpDeclarationToken.Identifier),
                T(CSharpDeclarationToken.OpenParenthesis), N(ParameterList),
                T(CSharpDeclarationToken.CloseParenthesis), N(ConstructorInitializer),
                T(CSharpDeclarationToken.Semicolon))
            .Rule(32, ConstructorInitializer)
            .Rule(33, ConstructorInitializer, T(CSharpDeclarationToken.Colon),
                T(CSharpDeclarationToken.ThisKeyword), T(CSharpDeclarationToken.OpenParenthesis),
                N(ConstructorArgumentList), T(CSharpDeclarationToken.CloseParenthesis))
            .Rule(34, ConstructorInitializer, T(CSharpDeclarationToken.Colon),
                T(CSharpDeclarationToken.BaseKeyword), T(CSharpDeclarationToken.OpenParenthesis),
                N(ConstructorArgumentList), T(CSharpDeclarationToken.CloseParenthesis))
            .Rule(35, ConstructorArgumentList)
            .Rule(36, ConstructorArgumentList, T(CSharpDeclarationToken.ExpressionReference))
            .Rule(37, ConstructorArgumentList, T(CSharpDeclarationToken.ExpressionReference),
                T(CSharpDeclarationToken.Comma), N(ConstructorArgumentList))
            .Rule(38, MemberDeclaration, N(IndexerDeclaration))
            .Rule(39, IndexerDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.ThisKeyword), T(CSharpDeclarationToken.OpenBracket),
                N(ParameterList), T(CSharpDeclarationToken.CloseBracket), T(CSharpDeclarationToken.OpenBrace),
                N(AccessorList), T(CSharpDeclarationToken.CloseBrace))
            .Rule(40, MemberDeclaration, N(EventDeclaration))
            .Rule(41, EventDeclaration, N(ModifierList), T(CSharpDeclarationToken.EventKeyword),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.Identifier),
                T(CSharpDeclarationToken.Semicolon))
            .Rule(42, EventDeclaration, N(ModifierList), T(CSharpDeclarationToken.EventKeyword),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.Identifier),
                T(CSharpDeclarationToken.OpenBrace), N(EventAccessorList), T(CSharpDeclarationToken.CloseBrace))
            .Rule(43, EventAccessorList, T(CSharpDeclarationToken.AddAccessor),
                T(CSharpDeclarationToken.BlockReference), T(CSharpDeclarationToken.RemoveAccessor),
                T(CSharpDeclarationToken.BlockReference))
            .Rule(44, EventAccessorList, T(CSharpDeclarationToken.RemoveAccessor),
                T(CSharpDeclarationToken.BlockReference), T(CSharpDeclarationToken.AddAccessor),
                T(CSharpDeclarationToken.BlockReference))
            .Rule(45, MemberDeclaration, N(UnaryOperatorDeclaration))
            .Rule(46, MemberDeclaration, N(BinaryOperatorDeclaration))
            .Rule(47, UnaryOperatorDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.OperatorToken),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter),
                T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(48, UnaryOperatorDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.OperatorToken),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter),
                T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(49, BinaryOperatorDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.OperatorToken),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter), T(CSharpDeclarationToken.Comma),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(50, BinaryOperatorDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.OperatorToken),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter), T(CSharpDeclarationToken.Comma),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(51, ConversionDeclaration, N(ModifierList), T(CSharpDeclarationToken.ImplicitKeyword),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter), T(CSharpDeclarationToken.CloseParenthesis),
                T(CSharpDeclarationToken.BlockReference))
            .Rule(52, ConversionDeclaration, N(ModifierList), T(CSharpDeclarationToken.ImplicitKeyword),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter), T(CSharpDeclarationToken.CloseParenthesis),
                T(CSharpDeclarationToken.Arrow), T(CSharpDeclarationToken.ExpressionReference),
                T(CSharpDeclarationToken.Semicolon))
            .Rule(53, ConversionDeclaration, N(ModifierList), T(CSharpDeclarationToken.ExplicitKeyword),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter), T(CSharpDeclarationToken.CloseParenthesis),
                T(CSharpDeclarationToken.BlockReference))
            .Rule(54, ConversionDeclaration, N(ModifierList), T(CSharpDeclarationToken.ExplicitKeyword),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter), T(CSharpDeclarationToken.CloseParenthesis),
                T(CSharpDeclarationToken.Arrow), T(CSharpDeclarationToken.ExpressionReference),
                T(CSharpDeclarationToken.Semicolon))
            .Rule(55, MemberDeclaration, N(ConversionDeclaration))
            // C# checked user-defined operator forms use a distinct host vocabulary token.
            // The host maps CheckedOperatorToken only to operators permitted in checked declarations.
            .Rule(56, UnaryOperatorDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.CheckedKeyword),
                T(CSharpDeclarationToken.CheckedOperatorToken), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(57, UnaryOperatorDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.CheckedKeyword),
                T(CSharpDeclarationToken.CheckedOperatorToken), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(58, BinaryOperatorDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.CheckedKeyword),
                T(CSharpDeclarationToken.CheckedOperatorToken), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.Comma), N(Parameter),
                T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(59, BinaryOperatorDeclaration, N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.OperatorKeyword), T(CSharpDeclarationToken.CheckedKeyword),
                T(CSharpDeclarationToken.CheckedOperatorToken), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.Comma), N(Parameter),
                T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            // Metadata remains independently defined; declaration slots consume opaque references.
            .Rule(60, DeclarationAttributes)
            .Rule(61, DeclarationAttributes, N(DeclarationAttributes), T(CSharpDeclarationToken.AttributeSectionReference))
            .Rule(62, GenericMethodMetadata)
            .Rule(63, GenericMethodMetadata, T(CSharpDeclarationToken.GenericParameterListReference), N(GenericMethodConstraintTail))
            .Rule(64, GenericMethodConstraintTail)
            .Rule(65, GenericMethodConstraintTail, T(CSharpDeclarationToken.ConstraintClauseReference), N(GenericMethodConstraintTail))
            .Rule(66, MethodDeclaration, N(DeclarationAttributes), N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), N(GenericMethodMetadata), T(CSharpDeclarationToken.OpenParenthesis),
                N(ParameterList), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Semicolon))
            .Rule(67, MethodDeclaration, N(DeclarationAttributes), N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), N(GenericMethodMetadata), T(CSharpDeclarationToken.OpenParenthesis),
                N(ParameterList), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(68, MethodDeclaration, N(DeclarationAttributes), N(ModifierList), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), N(GenericMethodMetadata), T(CSharpDeclarationToken.OpenParenthesis),
                N(ParameterList), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(69, ParameterAttributes)
            .Rule(70, ParameterAttributes, N(ParameterAttributes), T(CSharpDeclarationToken.ParameterAttributeSectionReference))

            // Declaration attributes are attached at the declaration boundary. The host still
            // validates target legality and resolves the opaque attribute-section structures.
            .Rule(71, PropertyDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.Identifier),
                T(CSharpDeclarationToken.OpenBrace), N(AccessorList), T(CSharpDeclarationToken.CloseBrace))
            .Rule(72, ConstructorDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.OpenParenthesis),
                N(ParameterList), T(CSharpDeclarationToken.CloseParenthesis), N(ConstructorInitializer),
                T(CSharpDeclarationToken.BlockReference))
            .Rule(73, ConstructorDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.OpenParenthesis),
                N(ParameterList), T(CSharpDeclarationToken.CloseParenthesis), N(ConstructorInitializer),
                T(CSharpDeclarationToken.Arrow), T(CSharpDeclarationToken.ExpressionReference),
                T(CSharpDeclarationToken.Semicolon))
            .Rule(74, ConstructorDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.OpenParenthesis),
                N(ParameterList), T(CSharpDeclarationToken.CloseParenthesis), N(ConstructorInitializer),
                T(CSharpDeclarationToken.Semicolon))
            .Rule(75, IndexerDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.ThisKeyword),
                T(CSharpDeclarationToken.OpenBracket), N(ParameterList), T(CSharpDeclarationToken.CloseBracket),
                T(CSharpDeclarationToken.OpenBrace), N(AccessorList), T(CSharpDeclarationToken.CloseBrace))
            // Complete the declaration-attribute attachment inventory for the existing member kinds.
            .Rule(76, FieldDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.Identifier),
                T(CSharpDeclarationToken.Semicolon))
            .Rule(77, EventDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.EventKeyword), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.Semicolon))
            .Rule(78, EventDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.EventKeyword), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.OpenBrace),
                N(EventAccessorList), T(CSharpDeclarationToken.CloseBrace))
            .Rule(79, UnaryOperatorDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.OperatorToken), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(80, UnaryOperatorDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.OperatorToken), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(81, UnaryOperatorDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.CheckedKeyword), T(CSharpDeclarationToken.CheckedOperatorToken),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter), T(CSharpDeclarationToken.CloseParenthesis),
                T(CSharpDeclarationToken.BlockReference))
            .Rule(82, UnaryOperatorDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.CheckedKeyword), T(CSharpDeclarationToken.CheckedOperatorToken),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter), T(CSharpDeclarationToken.CloseParenthesis),
                T(CSharpDeclarationToken.Arrow), T(CSharpDeclarationToken.ExpressionReference),
                T(CSharpDeclarationToken.Semicolon))
            .Rule(83, BinaryOperatorDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.OperatorToken), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.Comma), N(Parameter),
                T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(84, BinaryOperatorDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.OperatorToken), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.Comma), N(Parameter),
                T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(85, BinaryOperatorDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.CheckedKeyword), T(CSharpDeclarationToken.CheckedOperatorToken),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter), T(CSharpDeclarationToken.Comma),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(86, BinaryOperatorDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.CheckedKeyword), T(CSharpDeclarationToken.CheckedOperatorToken),
                T(CSharpDeclarationToken.OpenParenthesis), N(Parameter), T(CSharpDeclarationToken.Comma),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(87, ConversionDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.ImplicitKeyword), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(88, ConversionDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.ImplicitKeyword), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(89, ConversionDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.ExplicitKeyword), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(90, ConversionDeclaration, N(DeclarationAttributes), N(ModifierList),
                T(CSharpDeclarationToken.ExplicitKeyword), T(CSharpDeclarationToken.OperatorKeyword),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.OpenParenthesis),
                N(Parameter), T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            // Explicit-interface member names are represented by one opaque host-owned
            // specifier reference (interface type plus separator), not reparsed here.
            // C# explicit implementations require bodies; semicolon-only method declarations
            // are intentionally excluded.
            .Rule(91, MethodDeclaration, N(DeclarationAttributes), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.ExplicitInterfaceSpecifierReference), T(CSharpDeclarationToken.Identifier),
                N(GenericMethodMetadata), T(CSharpDeclarationToken.OpenParenthesis), N(ParameterList),
                T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.BlockReference))
            .Rule(92, MethodDeclaration, N(DeclarationAttributes), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.ExplicitInterfaceSpecifierReference), T(CSharpDeclarationToken.Identifier),
                N(GenericMethodMetadata), T(CSharpDeclarationToken.OpenParenthesis), N(ParameterList),
                T(CSharpDeclarationToken.CloseParenthesis), T(CSharpDeclarationToken.Arrow),
                T(CSharpDeclarationToken.ExpressionReference), T(CSharpDeclarationToken.Semicolon))
            .Rule(93, PropertyDeclaration, N(DeclarationAttributes), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.ExplicitInterfaceSpecifierReference), T(CSharpDeclarationToken.Identifier),
                T(CSharpDeclarationToken.OpenBrace), N(AccessorList), T(CSharpDeclarationToken.CloseBrace))
            .Rule(94, IndexerDeclaration, N(DeclarationAttributes), T(CSharpDeclarationToken.TypeReference),
                T(CSharpDeclarationToken.ExplicitInterfaceSpecifierReference), T(CSharpDeclarationToken.ThisKeyword),
                T(CSharpDeclarationToken.OpenBracket), N(ParameterList), T(CSharpDeclarationToken.CloseBracket),
                T(CSharpDeclarationToken.OpenBrace), N(AccessorList), T(CSharpDeclarationToken.CloseBrace))
            .Rule(95, EventDeclaration, N(DeclarationAttributes), T(CSharpDeclarationToken.EventKeyword),
                T(CSharpDeclarationToken.TypeReference), T(CSharpDeclarationToken.ExplicitInterfaceSpecifierReference),
                T(CSharpDeclarationToken.Identifier), T(CSharpDeclarationToken.OpenBrace),
                N(EventAccessorList), T(CSharpDeclarationToken.CloseBrace))
            .Build();
    }
}
