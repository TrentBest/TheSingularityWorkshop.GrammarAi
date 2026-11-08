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
            [
                new GrammarNodeInstance(100, 1),
                new GrammarNodeInstance(101, 2),
                new GrammarNodeInstance(102, 1)
            ],
            [
                new GrammarRelationship(100, 10, 101),
                new GrammarRelationship(102, 10, 101),
                new GrammarRelationship(101, 11, 100)
            ]);

        Assert.Equal(3, instance.Relationships.Count);
        Assert.Equal((ulong)101, instance.GetRelationships(100, 10).Single().ChildInstanceId);
        Assert.Equal((ulong)101, instance.GetRelationships(102, 10).Single().ChildInstanceId);
    }

    [Fact]
    public void Proposal_codec_round_trips_canonical_integer_first_structure()
    {
        var schema = CreateConditionalSchema();
        var instance = new GrammarStructureInstance(
            schema,
            [
                new GrammarNodeInstance(1, 1),
                new GrammarNodeInstance(2, 4),
                new GrammarNodeInstance(3, 5),
                new GrammarNodeInstance(4, 2),
                new GrammarNodeInstance(5, 4),
                new GrammarNodeInstance(6, 5)
            ],
            [
                new GrammarRelationship(1, 101, 2),
                new GrammarRelationship(1, 102, 3),
                new GrammarRelationship(1, 103, 4),
                new GrammarRelationship(4, 201, 5),
                new GrammarRelationship(4, 202, 6)
            ]);

        var json = GrammarStructureProposalCodec.Serialize(instance);
        var decoded = GrammarStructureProposalCodec.Deserialize(json, schema);

        Assert.Equal(
            "{\"v\":2,\"g\":9001,\"gv\":1,\"n\":[[1,1],[2,4],[3,5],[4,2],[5,4],[6,5]],\"r\":[[1,101,0,2],[1,102,0,3],[1,103,0,4],[4,201,0,5],[4,202,0,6]]}",
            json);
        Assert.Equal(instance.Nodes, decoded.Nodes);
        Assert.Equal(instance.Relationships, decoded.Relationships);
        Assert.Equal(json, GrammarStructureProposalCodec.Serialize(decoded));
    }

    [Fact]
    public void Proposal_codec_rejects_a_stale_definition_version()
    {
        var schema = CreateConditionalSchema();
        var proposal = "{\"v\":1,\"g\":9001,\"gv\":2,\"n\":[],\"r\":[]}";
        Assert.Throws<ArgumentException>(() => GrammarStructureProposalCodec.Deserialize(proposal, schema));
    }

    [Fact]
    public void Proposal_codec_rejects_unknown_wire_versions_and_duplicate_properties()
    {
        var schema = CreateConditionalSchema();
        Assert.Throws<ArgumentException>(() => GrammarStructureProposalCodec.Deserialize(
            "{\"v\":3,\"g\":9001,\"gv\":1,\"n\":[],\"r\":[]}", schema));
        Assert.Throws<ArgumentException>(() => GrammarStructureProposalCodec.Deserialize(
            "{\"v\":2,\"v\":2,\"g\":9001,\"gv\":1,\"n\":[],\"r\":[]}", schema));
    }

    [Fact]
    public void Proposal_codec_rejects_invalid_structure_after_decoding()
    {
        var schema = CreateConditionalSchema();
        var proposal = "{\"v\":1,\"g\":9001,\"gv\":1,\"n\":[[1,1],[2,5],[3,5]],\"r\":[[1,101,2],[1,102,3]]}";
        Assert.Throws<ArgumentException>(() => GrammarStructureProposalCodec.Deserialize(proposal, schema));
    }
    [Fact]
    public void Proposal_codec_continues_to_read_legacy_v1_nested_relationships()
    {
        var schema = CreateConditionalSchema();
        const string legacy = "{\"v\":1,\"g\":9001,\"gv\":1,\"n\":[[1,1],[2,4],[3,5]],\"r\":[[1,101,2],[1,102,3]]}";
        var decoded = GrammarStructureProposalCodec.Deserialize(legacy, schema);
        Assert.Equal(new ulong[] { 2 }, decoded.GetRelationships(1, 101).Select(x => x.ChildInstanceId));
        Assert.StartsWith("{\"v\":2,", GrammarStructureProposalCodec.Serialize(decoded));
    }

    [Fact]
    public void External_member_ids_are_qualified_by_category_identity()
    {
        var schema = new GrammarStructureDefinition(
            702, "QualifiedValues",
            [new GrammarNodeDefinition(1, "Request")],
            [
                new GrammarSlotDefinition(10, 1, "Repository", GrammarSlotTarget.ExternalProtocolCategory(900), 0, null),
                new GrammarSlotDefinition(11, 1, "Trigger", GrammarSlotTarget.ExternalProtocolCategory(901), 0, null)
            ]);
        var resolver = new MultiCategoryResolver();
        var instance = new GrammarStructureInstance(
            schema,
            [new GrammarNodeInstance(100, 1)],
            [
                new GrammarRelationship(100, 10, new GrammarProtocolReference(900, 42)),
                new GrammarRelationship(100, 11, new GrammarProtocolReference(901, 42))
            ],
            resolver);
        Assert.Equal((ulong)900, instance.Relationships[0].ExternalReference!.Value.ProtocolId);
        Assert.Equal((ulong)901, instance.Relationships[1].ExternalReference!.Value.ProtocolId);
    }

    [Fact]
    public void Proposal_codec_rejects_unknown_v2_relationship_tags()
    {
        var schema = CreateConditionalSchema();
        const string invalid = "{\"v\":2,\"g\":9001,\"gv\":1,\"n\":[[1,1],[2,4],[3,5]],\"r\":[[1,101,9,2],[1,102,0,3]]}";
        Assert.Throws<ArgumentException>(() => GrammarStructureProposalCodec.Deserialize(invalid, schema));
    }


    [Fact]
    public void External_values_round_trip_as_canonical_v2_and_keep_category_qualified_identity()
    {
        var schema = new GrammarStructureDefinition(
            703, "QualifiedExternalValues",
            [new GrammarNodeDefinition(1, "Request")],
            [
                new GrammarSlotDefinition(10, 1, "Repository", GrammarSlotTarget.ExternalProtocolCategory(900), 1, 1),
                new GrammarSlotDefinition(11, 1, "Trigger", GrammarSlotTarget.ExternalProtocolCategory(901), 1, 1)
            ]);
        var resolver = new MultiCategoryResolver();
        var instance = new GrammarStructureInstance(
            schema,
            [new GrammarNodeInstance(100, 1)],
            [
                new GrammarRelationship(100, 10, new GrammarProtocolReference(900, 42)),
                new GrammarRelationship(100, 11, new GrammarProtocolReference(901, 42))
            ],
            resolver);

        var json = GrammarStructureProposalCodec.Serialize(instance);
        var decoded = GrammarStructureProposalCodec.Deserialize(json, schema, resolver);

        Assert.Equal("""{"v":2,"g":703,"gv":1,"n":[[100,1]],"r":[[100,10,1,900,42],[100,11,1,901,42]]}""", json);
        Assert.Equal(instance.Relationships, decoded.Relationships);
        Assert.Equal(json, GrammarStructureProposalCodec.Serialize(decoded));
    }

    [Fact]
    public void Mixed_relationship_values_preserve_global_order_and_apply_cardinality_per_slot()
    {
        var schema = new GrammarStructureDefinition(
            704, "MixedRelationshipOrder",
            [new GrammarNodeDefinition(1, "Root"), new GrammarNodeDefinition(2, "Child")],
            [
                new GrammarSlotDefinition(10, 1, "ExternalValues", GrammarSlotTarget.ExternalProtocolCategory(900), 0, 2),
                new GrammarSlotDefinition(11, 1, "NestedValues", GrammarSlotTarget.Node(2), 1, 2)
            ]);
        var resolver = new MultiCategoryResolver();
        var relationships = new[]
        {
            new GrammarRelationship(100, 10, new GrammarProtocolReference(900, 42)),
            new GrammarRelationship(100, 11, 101),
            new GrammarRelationship(100, 10, new GrammarProtocolReference(900, 43)),
            new GrammarRelationship(100, 11, 102)
        };
        var instance = new GrammarStructureInstance(
            schema,
            [new GrammarNodeInstance(100, 1), new GrammarNodeInstance(101, 2), new GrammarNodeInstance(102, 2)],
            relationships,
            resolver);

        Assert.Equal(relationships, instance.Relationships);
        Assert.True(instance.Relationships[0].IsExternal);
        Assert.False(instance.Relationships[1].IsExternal);
        Assert.True(instance.Relationships[2].IsExternal);
        Assert.False(instance.Relationships[3].IsExternal);
        Assert.Equal(new ulong[] { 101, 102 }, instance.GetRelationships(100, 11).Select(x => x.ChildInstanceId));
        Assert.Equal(new ulong[] { 42, 43 }, instance.GetRelationships(100, 10).Select(x => x.ExternalReference!.Value.SymbolId));

        // External values do not satisfy the required nested-node slot.
        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            schema,
            [new GrammarNodeInstance(100, 1), new GrammarNodeInstance(101, 2)],
            [new GrammarRelationship(100, 10, new GrammarProtocolReference(900, 42))],
            resolver));

        // Nested-node values obey the same maximum-occurrence rule.
        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            schema,
            [
                new GrammarNodeInstance(100, 1),
                new GrammarNodeInstance(101, 2),
                new GrammarNodeInstance(102, 2),
                new GrammarNodeInstance(103, 2)
            ],
            [
                new GrammarRelationship(100, 11, 101),
                new GrammarRelationship(100, 11, 102),
                new GrammarRelationship(100, 11, 103)
            ],
            resolver));

        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            schema,
            [new GrammarNodeInstance(100, 1), new GrammarNodeInstance(101, 2), new GrammarNodeInstance(102, 2)],
            [
                new GrammarRelationship(100, 10, new GrammarProtocolReference(900, 42)),
                new GrammarRelationship(100, 10, new GrammarProtocolReference(900, 43)),
                new GrammarRelationship(100, 10, new GrammarProtocolReference(900, 44)),
                new GrammarRelationship(100, 11, 101)
            ],
            resolver));
    }

    [Fact]
    public void External_values_require_a_host_resolver()
    {
        var schema = new GrammarStructureDefinition(
            705, "ResolverRequired",
            [new GrammarNodeDefinition(1, "Request")],
            [new GrammarSlotDefinition(10, 1, "Value", GrammarSlotTarget.ExternalProtocolCategory(900), 0, 1)]);

        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            schema,
            [new GrammarNodeInstance(100, 1)],
            [new GrammarRelationship(100, 10, new GrammarProtocolReference(900, 42))]));
    }

    [Theory]
    [InlineData(GrammarExternalReferenceStatus.UnknownProtocol)]
    [InlineData(GrammarExternalReferenceStatus.UnknownSymbol)]
    [InlineData(GrammarExternalReferenceStatus.NotExposed)]
    public void External_values_fail_closed_for_unresolved_or_unexposed_references(GrammarExternalReferenceStatus status)
    {
        var schema = new GrammarStructureDefinition(
            706, "ResolverOutcomes",
            [new GrammarNodeDefinition(1, "Request")],
            [new GrammarSlotDefinition(10, 1, "Value", GrammarSlotTarget.ExternalProtocolCategory(900), 0, 1)]);

        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            schema,
            [new GrammarNodeInstance(100, 1)],
            [new GrammarRelationship(100, 10, new GrammarProtocolReference(900, 42))],
            new FixedStatusResolver(status)));
    }

    [Fact]
    public void External_values_reject_a_category_that_does_not_match_the_slot_target()
    {
        var schema = new GrammarStructureDefinition(
            707, "WrongCategory",
            [new GrammarNodeDefinition(1, "Request")],
            [new GrammarSlotDefinition(10, 1, "Value", GrammarSlotTarget.ExternalProtocolCategory(900), 0, 1)]);

        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            schema,
            [new GrammarNodeInstance(100, 1)],
            [new GrammarRelationship(100, 10, new GrammarProtocolReference(901, 42))],
            new MultiCategoryResolver()));
    }

    [Fact]
    public void Relationship_value_case_must_match_the_slot_target_case()
    {
        var schema = new GrammarStructureDefinition(
            708, "TargetCase",
            [new GrammarNodeDefinition(1, "Root"), new GrammarNodeDefinition(2, "Child")],
            [
                new GrammarSlotDefinition(10, 1, "External", GrammarSlotTarget.ExternalProtocolCategory(900), 0, 1),
                new GrammarSlotDefinition(11, 1, "Nested", GrammarSlotTarget.Node(2), 0, 1)
            ]);

        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            schema,
            [new GrammarNodeInstance(100, 1), new GrammarNodeInstance(101, 2)],
            [new GrammarRelationship(100, 10, 101)]));

        Assert.Throws<ArgumentException>(() => new GrammarStructureInstance(
            schema,
            [new GrammarNodeInstance(100, 1), new GrammarNodeInstance(101, 2)],
            [new GrammarRelationship(100, 11, new GrammarProtocolReference(900, 42))],
            new MultiCategoryResolver()));
    }

    [Fact]
    public void Proposal_codec_rejects_wrong_v1_and_v2_relationship_tuple_arities()
    {
        var schema = CreateConditionalSchema();
        Assert.Throws<ArgumentException>(() => GrammarStructureProposalCodec.Deserialize(
            """{"v":1,"g":9001,"gv":1,"n":[],"r":[[1,101,2,3]]}""", schema));
        Assert.Throws<ArgumentException>(() => GrammarStructureProposalCodec.Deserialize(
            """{"v":2,"g":9001,"gv":1,"n":[],"r":[[1,101,0,2,3]]}""", schema));
        Assert.Throws<ArgumentException>(() => GrammarStructureProposalCodec.Deserialize(
            """{"v":2,"g":9001,"gv":1,"n":[],"r":[[1,101,1,900]]}""", schema));
    }

    [Fact]
    public void Proposal_codec_rejects_oversized_payloads_before_parsing()
    {
        var oversized = new string(' ', GrammarLimits.MaxProposalCharacters + 1);
        Assert.Throws<ArgumentException>(() => GrammarStructureProposalCodec.Deserialize(oversized, CreateConditionalSchema()));
    }

    private sealed class FixedStatusResolver(GrammarExternalReferenceStatus status) : IGrammarExternalReferenceResolver
    {
        public GrammarExternalReferenceStatus Resolve(GrammarProtocolReference reference) => status;
    }

    private sealed class MultiCategoryResolver : IGrammarExternalReferenceResolver
    {
        public GrammarExternalReferenceStatus Resolve(GrammarProtocolReference reference) =>
            (reference.ProtocolId is 900 or 901) && (reference.SymbolId is 42 or 43 or 44)
                ? GrammarExternalReferenceStatus.ResolvedAndExposed
                : GrammarExternalReferenceStatus.UnknownSymbol;
    }

}
