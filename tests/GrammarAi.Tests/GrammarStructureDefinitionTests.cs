using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class GrammarStructureDefinitionTests
{
    [Fact]
    public void If_else_if_else_schema_models_required_optional_and_repeated_relationships()
    {
        var structure = new GrammarStructureDefinition(
            9001,
            "ConditionalControlFlow",
            [
                new GrammarNodeDefinition(1, "IfStatement"),
                new GrammarNodeDefinition(2, "ElseIfClause"),
                new GrammarNodeDefinition(3, "ElseClause"),
                new GrammarNodeDefinition(4, "Expression"),
                new GrammarNodeDefinition(5, "Block")
            ],
            [
                new GrammarSlotDefinition(101, 1, "Condition", 4),
                new GrammarSlotDefinition(102, 1, "Body", 5),
                new GrammarSlotDefinition(103, 1, "ElseIf", 2, 0, null),
                new GrammarSlotDefinition(104, 1, "Else", 3, 0, 1),
                new GrammarSlotDefinition(201, 2, "Condition", 4),
                new GrammarSlotDefinition(202, 2, "Body", 5),
                new GrammarSlotDefinition(301, 3, "Body", 5)
            ]);

        var ifSlots = structure.GetSlots(1);

        Assert.Equal(new[] { "Condition", "Body", "ElseIf", "Else" }, ifSlots.Select(x => x.Name));
        Assert.Equal((0, (int?)null), (ifSlots[2].MinimumOccurrences, ifSlots[2].MaximumOccurrences));
        Assert.Equal((0, (int?)1), (ifSlots[3].MinimumOccurrences, ifSlots[3].MaximumOccurrences));
        Assert.Equal(new[] { "Body" }, structure.GetSlots(3).Select(x => x.Name));
    }

    [Fact]
    public void Node_and_slot_identity_spaces_are_grammar_owned_and_distinct_from_protocol_references()
    {
        var node = new GrammarNodeDefinition(2001, "Expression");
        var terminal = GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2001));

        Assert.Equal((ulong)2001, node.Id);
        Assert.Equal((ulong)2001, terminal.Id);
        Assert.NotEqual(node.GetType(), terminal.GetType());
    }

    [Fact]
    public void Structure_rejects_duplicate_node_and_slot_ids()
    {
        Assert.Throws<ArgumentException>(() => new GrammarStructureDefinition(
            1, "duplicate-nodes",
            [new GrammarNodeDefinition(1, "A"), new GrammarNodeDefinition(1, "B")],
            []));

        Assert.Throws<ArgumentException>(() => new GrammarStructureDefinition(
            1, "duplicate-slots",
            [new GrammarNodeDefinition(1, "A"), new GrammarNodeDefinition(2, "B")],
            [new GrammarSlotDefinition(3, 1, "First", 2), new GrammarSlotDefinition(3, 1, "Second", 2)]));
    }

    [Fact]
    public void Structure_rejects_slots_with_unknown_node_references()
    {
        Assert.Throws<ArgumentException>(() => new GrammarStructureDefinition(
            1, "unknown-owner",
            [new GrammarNodeDefinition(1, "A")],
            [new GrammarSlotDefinition(1, 2, "Child", 1)]));

        Assert.Throws<ArgumentException>(() => new GrammarStructureDefinition(
            1, "unknown-target",
            [new GrammarNodeDefinition(1, "A")],
            [new GrammarSlotDefinition(1, 1, "Child", 2)]));
    }

    [Fact]
    public void Slot_occurrence_bounds_are_validated()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GrammarSlotDefinition(1, 1, "Child", 2, -1, 1));
        Assert.Throws<ArgumentException>(() => new GrammarSlotDefinition(1, 1, "Child", 2, 2, 1));
    }

    [Fact]
    public void Structure_collections_are_immutable_snapshots()
    {
        var nodes = new List<GrammarNodeDefinition> { new(1, "A"), new(2, "B") };
        var slots = new List<GrammarSlotDefinition> { new(1, 1, "Child", 2) };
        var structure = new GrammarStructureDefinition(1, "snapshot", nodes, slots);

        nodes.Add(new GrammarNodeDefinition(3, "C"));
        slots.Clear();

        Assert.Equal(2, structure.Nodes.Count);
        Assert.Single(structure.Slots);
    }
}
