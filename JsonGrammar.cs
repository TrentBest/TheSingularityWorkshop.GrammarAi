namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Vocabulary members required by the RFC 8259 JSON grammar.</summary>
public enum JsonGrammarToken
{
    BeginObject,
    EndObject,
    BeginArray,
    EndArray,
    Colon,
    Comma,
    String,
    Number,
    True,
    False,
    Null
}

/// <summary>
/// Builds the bounded RFC 8259 JSON value grammar over host-owned ProtocolAi token identities.
/// </summary>
/// <remarks>
/// This resource describes JSON production structure only. The host supplies token identities from its
/// selected ProtocolAi catalog. Tokenization, string escape validation, number lexical validation,
/// Unicode handling, source locations, parsing, and semantic application policy remain outside GrammarAi.
/// Whitespace is lexical trivia and is not represented as a JSON value production.
/// </remarks>
public static class JsonGrammar
{
    private const ulong Value = 1;
    private const ulong Object = 2;
    private const ulong Members = 3;
    private const ulong Member = 4;
    private const ulong Array = 5;
    private const ulong Elements = 6;

    /// <summary>Creates the RFC 8259 JSON value grammar using host-supplied vocabulary identities.</summary>
    /// <param name="grammarId">Nonzero identity assigned by the host to this grammar resource.</param>
    /// <param name="protocolCategoryId">Nonzero ProtocolAi category identity containing the JSON tokens.</param>
    /// <param name="tokenIds">Complete mapping from each JSON token role to its nonzero ProtocolAi member identity.</param>
    /// <returns>An immutable, integer-first grammar definition.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The grammar or category identity is zero.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="tokenIds"/> is null.</exception>
    /// <exception cref="ArgumentException">A required token identity is missing or zero.</exception>
    public static GrammarDefinition Create(
        ulong grammarId,
        ulong protocolCategoryId,
        IReadOnlyDictionary<JsonGrammarToken, ulong> tokenIds)
    {
        if (grammarId == 0) throw new ArgumentOutOfRangeException(nameof(grammarId));
        if (protocolCategoryId == 0) throw new ArgumentOutOfRangeException(nameof(protocolCategoryId));
        ArgumentNullException.ThrowIfNull(tokenIds);

        foreach (var token in Enum.GetValues<JsonGrammarToken>())
        {
            if (!tokenIds.TryGetValue(token, out var id) || id == 0)
                throw new ArgumentException($"A nonzero ProtocolAi symbol identity is required for token '{token}'.", nameof(tokenIds));
        }

        GrammarSymbol T(JsonGrammarToken token) =>
            GrammarSymbol.Terminal(new GrammarProtocolReference(protocolCategoryId, tokenIds[token]));
        GrammarSymbol N(ulong id) => GrammarSymbol.NonTerminal(id);

        return new GrammarBuilder(grammarId, "JSON-RFC8259", Value)
            .Rule(1, Value, N(Object))
            .Rule(2, Value, N(Array))
            .Rule(3, Value, T(JsonGrammarToken.String))
            .Rule(4, Value, T(JsonGrammarToken.Number))
            .Rule(5, Value, T(JsonGrammarToken.True))
            .Rule(6, Value, T(JsonGrammarToken.False))
            .Rule(7, Value, T(JsonGrammarToken.Null))
            .Rule(8, Object, T(JsonGrammarToken.BeginObject), T(JsonGrammarToken.EndObject))
            .Rule(9, Object, T(JsonGrammarToken.BeginObject), N(Members), T(JsonGrammarToken.EndObject))
            .Rule(10, Members, N(Member))
            .Rule(11, Members, N(Members), T(JsonGrammarToken.Comma), N(Member))
            .Rule(12, Member, T(JsonGrammarToken.String), T(JsonGrammarToken.Colon), N(Value))
            .Rule(13, Array, T(JsonGrammarToken.BeginArray), T(JsonGrammarToken.EndArray))
            .Rule(14, Array, T(JsonGrammarToken.BeginArray), N(Elements), T(JsonGrammarToken.EndArray))
            .Rule(15, Elements, N(Value))
            .Rule(16, Elements, N(Elements), T(JsonGrammarToken.Comma), N(Value))
            .Build();
    }
}
