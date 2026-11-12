using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class XmlDocumentGrammarTests
{
    private static IReadOnlyDictionary<XmlDocumentGrammarToken, ulong> Tokens() =>
        Enum.GetValues<XmlDocumentGrammarToken>().ToDictionary(token => token, token => (ulong)token + 900);

    [Fact]
    public void Grammar_covers_document_prolog_elements_and_recursive_content()
    {
        var grammar = XmlDocumentGrammar.Create(6001, 1201, Tokens());

        Assert.Equal("XML-1.0-Document-Structure-Bounded", grammar.Name);
        Assert.Equal(1UL, grammar.StartSymbol);
        Assert.Equal(19, grammar.Rules.Count);
        Assert.Empty(grammar.FindUnreachableNonTerminals());

        Assert.All(grammar.Rules.SelectMany(rule => rule.RightHandSide)
            .Where(symbol => symbol.ProtocolReference.HasValue),
            symbol => Assert.Equal(1201UL, symbol.ProtocolReference!.Value.ProtocolId));

        // The document has one root element, with optional miscellaneous epilog items.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 1 &&
            rule.RightHandSide.Select(symbol => symbol.Id).SequenceEqual(new[] { 2UL, 5UL, 3UL }));

        // Prolog supports an optional XML declaration and doctype in the represented order.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 2 &&
            rule.RightHandSide.Count == 4 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[XmlDocumentGrammarToken.XmlDeclaration] &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == Tokens()[XmlDocumentGrammarToken.DoctypeDeclaration]);

        // Elements are either empty tags or paired start/content/end structures.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 5 &&
            rule.RightHandSide.Count == 1 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[XmlDocumentGrammarToken.EmptyElementTag]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 5 &&
            rule.RightHandSide.Select(symbol => symbol.Id).SequenceEqual(new[] { 0UL, 6UL, 0UL }) &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[XmlDocumentGrammarToken.StartTag] &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == Tokens()[XmlDocumentGrammarToken.EndTag]);

        // Nested elements and all XML content component families remain composable.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 7 && rule.RightHandSide.Count == 1 &&
            rule.RightHandSide[0].Id == 5);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 6 &&
            rule.RightHandSide.Select(symbol => symbol.Id).SequenceEqual(new[] { 6UL, 7UL }));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 7 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[XmlDocumentGrammarToken.CDataSection]);
    }

    [Fact]
    public void Missing_or_zero_token_identity_is_rejected()
    {
        var tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens.Remove(XmlDocumentGrammarToken.StartTag);
        Assert.Throws<ArgumentException>(() => XmlDocumentGrammar.Create(1, 1, tokens));

        tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens[XmlDocumentGrammarToken.Reference] = 0;
        Assert.Throws<ArgumentException>(() => XmlDocumentGrammar.Create(1, 1, tokens));
    }

    [Fact]
    public void Grammar_and_category_identity_must_be_nonzero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => XmlDocumentGrammar.Create(0, 1, Tokens()));
        Assert.Throws<ArgumentOutOfRangeException>(() => XmlDocumentGrammar.Create(1, 0, Tokens()));
    }
}
