namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Vocabulary members required by the bounded RFC 3986 URI-reference grammar.</summary>
public enum UriReferenceGrammarToken
{
    Scheme,
    Authority,
    Segment,
    SegmentNz,
    SegmentNzNoColon,
    Query,
    Fragment,
    Colon,
    DoubleSlash,
    Slash,
    QuestionMark,
    Hash
}

/// <summary>
/// Builds a bounded RFC 3986 URI-reference component grammar over host-owned ProtocolAi token identities.
/// </summary>
/// <remarks>
/// The resource models URI/reference alternatives, hierarchical path forms, optional query/fragment
/// components, and ordered path segments. Authority, path-segment, scheme, query, and fragment values
/// arrive as opaque host-tokenized components. This is not an ABNF importer, URI tokenizer, percent-
/// encoding validator, IP-literal/host parser, normalization engine, or full RFC 3986 conformance claim.
/// The host is responsible for the lexical rules and validation of each opaque component.
/// </remarks>
public static class UriReferenceGrammar
{
    private const ulong UriReference = 1;
    private const ulong Uri = 2;
    private const ulong RelativeReference = 3;
    private const ulong HierarchicalPart = 4;
    private const ulong RelativePart = 5;
    private const ulong AuthorityPath = 6;
    private const ulong PathAbempty = 7;
    private const ulong PathAbsolute = 8;
    private const ulong PathRootless = 9;
    private const ulong PathNoScheme = 10;
    private const ulong PathEmpty = 11;
    private const ulong PathTail = 12;
    private const ulong QueryOptional = 13;
    private const ulong FragmentOptional = 14;

    /// <summary>Creates the bounded RFC 3986 URI-reference grammar using host-supplied token identities.</summary>
    /// <param name="grammarId">Nonzero identity assigned by the host to this grammar resource.</param>
    /// <param name="protocolCategoryId">Nonzero ProtocolAi category identity containing the URI component tokens.</param>
    /// <param name="tokenIds">Complete mapping from each URI token role to its nonzero ProtocolAi member identity.</param>
    /// <returns>An immutable, integer-first grammar definition.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The grammar or category identity is zero.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="tokenIds"/> is null.</exception>
    /// <exception cref="ArgumentException">A required token identity is missing or zero.</exception>
    public static GrammarDefinition Create(
        ulong grammarId,
        ulong protocolCategoryId,
        IReadOnlyDictionary<UriReferenceGrammarToken, ulong> tokenIds)
    {
        if (grammarId == 0) throw new ArgumentOutOfRangeException(nameof(grammarId));
        if (protocolCategoryId == 0) throw new ArgumentOutOfRangeException(nameof(protocolCategoryId));
        ArgumentNullException.ThrowIfNull(tokenIds);

        foreach (var token in Enum.GetValues<UriReferenceGrammarToken>())
        {
            if (!tokenIds.TryGetValue(token, out var id) || id == 0)
                throw new ArgumentException($"A nonzero ProtocolAi symbol identity is required for token '{token}'.", nameof(tokenIds));
        }

        GrammarSymbol T(UriReferenceGrammarToken token) =>
            GrammarSymbol.Terminal(new GrammarProtocolReference(protocolCategoryId, tokenIds[token]));
        GrammarSymbol N(ulong id) => GrammarSymbol.NonTerminal(id);

        return new GrammarBuilder(grammarId, "URI-Reference-RFC3986-Bounded", UriReference)
            .Rule(1, UriReference, N(Uri))
            .Rule(2, UriReference, N(RelativeReference))
            .Rule(3, Uri, T(UriReferenceGrammarToken.Scheme), T(UriReferenceGrammarToken.Colon),
                N(HierarchicalPart), N(QueryOptional), N(FragmentOptional))
            .Rule(4, RelativeReference, N(RelativePart), N(QueryOptional), N(FragmentOptional))
            .Rule(5, HierarchicalPart, N(AuthorityPath))
            .Rule(6, HierarchicalPart, N(PathAbsolute))
            .Rule(7, HierarchicalPart, N(PathRootless))
            .Rule(8, HierarchicalPart, N(PathEmpty))
            .Rule(9, AuthorityPath, T(UriReferenceGrammarToken.DoubleSlash),
                T(UriReferenceGrammarToken.Authority), N(PathAbempty))
            .Rule(10, RelativePart, N(AuthorityPath))
            .Rule(11, RelativePart, N(PathAbsolute))
            .Rule(12, RelativePart, N(PathNoScheme))
            .Rule(13, RelativePart, N(PathEmpty))
            .Rule(14, PathAbempty)
            .Rule(15, PathAbempty, N(PathAbempty), T(UriReferenceGrammarToken.Slash),
                T(UriReferenceGrammarToken.Segment))
            .Rule(16, PathAbsolute, T(UriReferenceGrammarToken.Slash))
            .Rule(17, PathAbsolute, T(UriReferenceGrammarToken.Slash),
                T(UriReferenceGrammarToken.SegmentNz), N(PathTail))
            .Rule(18, PathRootless, T(UriReferenceGrammarToken.SegmentNz), N(PathTail))
            .Rule(19, PathNoScheme, T(UriReferenceGrammarToken.SegmentNzNoColon), N(PathTail))
            .Rule(20, PathEmpty)
            .Rule(21, PathTail)
            .Rule(22, PathTail, T(UriReferenceGrammarToken.Slash),
                T(UriReferenceGrammarToken.Segment), N(PathTail))
            .Rule(23, QueryOptional)
            .Rule(24, QueryOptional, T(UriReferenceGrammarToken.QuestionMark),
                T(UriReferenceGrammarToken.Query))
            .Rule(25, FragmentOptional)
            .Rule(26, FragmentOptional, T(UriReferenceGrammarToken.Hash),
                T(UriReferenceGrammarToken.Fragment))
            .Build();
    }
}
