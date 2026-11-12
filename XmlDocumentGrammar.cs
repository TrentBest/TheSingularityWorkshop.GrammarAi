namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Vocabulary roles required by the bounded XML 1.0 document-structure grammar.</summary>
public enum XmlDocumentGrammarToken
{
    XmlDeclaration,
    DoctypeDeclaration,
    Comment,
    ProcessingInstruction,
    StartTag,
    EmptyElementTag,
    EndTag,
    CharacterData,
    Reference,
    CDataSection
}

/// <summary>
/// Builds a bounded XML 1.0 document-structure grammar over host-supplied ProtocolAi token identities.
/// </summary>
/// <remarks>
/// The resource models document/prolog ordering, miscellaneous prolog and epilog items, element
/// nesting, empty elements, and the content-item alternatives. Start/end/empty tags and declaration
/// tokens are opaque lexical components. Matching element names, attribute uniqueness, namespace
/// constraints, entity expansion, DTD validity, character legality, and complete XML well-formedness
/// remain host responsibilities. This is not an XML tokenizer, parser, DTD processor, or full XML
/// conformance claim.
/// </remarks>
public static class XmlDocumentGrammar
{
    private const ulong Document = 1;
    private const ulong Prolog = 2;
    private const ulong MiscList = 3;
    private const ulong Misc = 4;
    private const ulong Element = 5;
    private const ulong Content = 6;
    private const ulong ContentItem = 7;

    /// <summary>Creates the bounded XML document-structure grammar using host-supplied token identities.</summary>
    /// <param name="grammarId">Nonzero identity assigned by the host to this grammar resource.</param>
    /// <param name="protocolCategoryId">Nonzero ProtocolAi category identity containing the XML component tokens.</param>
    /// <param name="tokenIds">Complete mapping from each XML token role to its nonzero ProtocolAi member identity.</param>
    /// <returns>An immutable, integer-first grammar definition.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The grammar or category identity is zero.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="tokenIds"/> is null.</exception>
    /// <exception cref="ArgumentException">A required token identity is missing or zero.</exception>
    public static GrammarDefinition Create(
        ulong grammarId,
        ulong protocolCategoryId,
        IReadOnlyDictionary<XmlDocumentGrammarToken, ulong> tokenIds)
    {
        if (grammarId == 0) throw new ArgumentOutOfRangeException(nameof(grammarId));
        if (protocolCategoryId == 0) throw new ArgumentOutOfRangeException(nameof(protocolCategoryId));
        ArgumentNullException.ThrowIfNull(tokenIds);

        foreach (var token in Enum.GetValues<XmlDocumentGrammarToken>())
        {
            if (!tokenIds.TryGetValue(token, out var id) || id == 0)
                throw new ArgumentException($"A nonzero ProtocolAi symbol identity is required for token '{token}'.", nameof(tokenIds));
        }

        GrammarSymbol T(XmlDocumentGrammarToken token) =>
            GrammarSymbol.Terminal(new GrammarProtocolReference(protocolCategoryId, tokenIds[token]));
        GrammarSymbol N(ulong id) => GrammarSymbol.NonTerminal(id);

        return new GrammarBuilder(grammarId, "XML-1.0-Document-Structure-Bounded", Document)
            .Rule(1, Document, N(Prolog), N(Element), N(MiscList))
            .Rule(2, Prolog, T(XmlDocumentGrammarToken.XmlDeclaration), N(MiscList),
                T(XmlDocumentGrammarToken.DoctypeDeclaration), N(MiscList))
            .Rule(3, Prolog, T(XmlDocumentGrammarToken.XmlDeclaration), N(MiscList))
            .Rule(4, Prolog, N(MiscList), T(XmlDocumentGrammarToken.DoctypeDeclaration), N(MiscList))
            .Rule(5, Prolog, N(MiscList))
            .Rule(6, MiscList)
            .Rule(7, MiscList, N(MiscList), N(Misc))
            .Rule(8, Misc, T(XmlDocumentGrammarToken.Comment))
            .Rule(9, Misc, T(XmlDocumentGrammarToken.ProcessingInstruction))
            .Rule(10, Element, T(XmlDocumentGrammarToken.EmptyElementTag))
            .Rule(11, Element, T(XmlDocumentGrammarToken.StartTag), N(Content),
                T(XmlDocumentGrammarToken.EndTag))
            .Rule(12, Content)
            .Rule(13, Content, N(Content), N(ContentItem))
            .Rule(14, ContentItem, N(Element))
            .Rule(15, ContentItem, T(XmlDocumentGrammarToken.CharacterData))
            .Rule(16, ContentItem, T(XmlDocumentGrammarToken.Reference))
            .Rule(17, ContentItem, T(XmlDocumentGrammarToken.CDataSection))
            .Rule(18, ContentItem, T(XmlDocumentGrammarToken.Comment))
            .Rule(19, ContentItem, T(XmlDocumentGrammarToken.ProcessingInstruction))
            .Build();
    }
}
