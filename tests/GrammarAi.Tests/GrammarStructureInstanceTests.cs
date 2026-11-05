using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class GrammarStructureInstanceTests
{
    private static GrammarStructureDefinition CreateConditionalSchema() => new(
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

    [Fact]
    public void Conditional_instance_preserves_else_if_order_and_optional_else()
    {
        var schema = CreateConditionalSchema();
        var instance = new GrammarStructureInstance(
            schema,
            [
                new GrammarNodeInstance(1000, 1),
                new GrammarNodeInstance(1001, 4),
                new GrammarNodeInstance(1002, 5),
                new GrammarNodeInstance(1003, 2),
                new GrammarNodeInstance(1004, 4),
                new GrammarNodeInstance(1005, 5),
                new GrammarNodeInstance(1006, 2),
                new GrammarNodeInstance(1007, 4),
                new GrammarNodeInstance(1008, 5),
                new GrammarNodeInstance(1009, 3),
                new GrammarNodeInstance(1010, 5)
            ],
            [
                new GrammarRelationship(1000, 101, 1001),
                new GrammarRelationship(1000, 102, 1002),
                new GrammarRelationship(1000, 103, 1003),
                new GrammarRelationship(1000, 103, 1006),
                new GrammarRelationship(1000, 104, 1009),
                new GrammarRelationship(1003, 201, 1004),
                new GrammarRelationship(1003, 202, 1005),
                new GrammarRelationship(1006, 201, 1007),
                new GrammarRelationship(1006, 202, 1008),
                new GrammarRelationship(1009, 301, 1010)
            ]);

        var elseIfs = instance.GetRelationships(1000, 103);

        Assert.Equal(new ulong[] { 1003, 1006 }, elseIfs.Select(link => link.ChildInstanceId));
        Assert.Equal(11, instance.Nodes.Count);
        Assert.Equal(10, instance.Relationships.Count);
        Assert.True(instance.TryGetNode(1006, out var node));
        Assert.Equal((ulong)2, node!.NodeDefinitionId);
    }

    [Fact]
    public void Optional_else_can_be_omitted()
    {
        var instance = new GrammarStructureInstance(
            CreateConditionalSchema(),
            [new GrammarNodeInstance(1, 1), new GrammarNodeInstance(2, 4), new GrammarNodeInstance(3, 5)],
            [new GrammarRelationship(1, 101, 2), new GrammarRelationship(1, 102, 3)]);

        Assert.Empty(instance.GetRelationships(1, 104));
    }

    [Fact]
    public void Instance_rejects_missing_required_relationship()
    {
        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            CreateConditionalSchema(),
            [new GrammarNodeInstance(1, 1), new GrammarNodeInstance(2, 4)],
            [new GrammarRelationship(1, 101, 2)]));
    }

    [Fact]
    public void Instance_rejects_wrong_target_node_kind()
    {
        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            CreateConditionalSchema(),
            [new GrammarNodeInstance(1, 1), new GrammarNodeInstance(2, 5), new GrammarNodeInstance(3, 5)],
            [new GrammarRelationship(1, 101, 2), new GrammarRelationship(1, 102, 3)]));
    }

    [Fact]
    public void Instance_rejects_slot_owned_by_another_node_kind()
    {
        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            CreateConditionalSchema(),
            [
                new GrammarNodeInstance(1, 1), new GrammarNodeInstance(2, 4),
                new GrammarNodeInstance(3, 5), new GrammarNodeInstance(4, 2),
                new GrammarNodeInstance(5, 4), new GrammarNodeInstance(6, 5)
            ],
            [
                new GrammarRelationship(1, 101, 2),
                new GrammarRelationship(1, 102, 3),
                new GrammarRelationship(4, 101, 5),
                new GrammarRelationship(4, 202, 6)
            ]));
    }

    [Fact]
    public void Instance_rejects_duplicate_instance_ids_and_unknown_endpoints()
    {
        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            CreateConditionalSchema(),
            [new GrammarNodeInstance(1, 1), new GrammarNodeInstance(1, 4)],
            []));

        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            CreateConditionalSchema(),
            [new GrammarNodeInstance(1, 1), new GrammarNodeInstance(2, 4), new GrammarNodeInstance(3, 5)],
            [new GrammarRelationship(1, 101, 2), new GrammarRelationship(1, 102, 99)]));
    }

    [Fact]
    public void Graph_semantics_allow_shared_nodes_and_cycles()
    {
        var schema = new GrammarStructureDefinition(
            1, "Graph",
            [new GrammarNodeDefinition(1, "A"), new GrammarNodeDefinition(2, "B")],
            [
                new GrammarSlotDefinition(10, 1, "Next", 2, 0, null),
                new GrammarSlotDefinition(11, 2, "Back", 1, 0, null)
            ]);

        var instance = new GrammarStructureInstance(
            schema,
            [new GrammarNodeInstance(100, 1), new GrammarNodeInstance(101, 2)],
            [new GrammarRelationship(100, 10, 101), new GrammarRelationship(101, 11, 100)]);

        Assert.Equal(2, instance.Relationships.Count);
    }
}
