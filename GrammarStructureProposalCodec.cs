using System.Text;
using System.Text.Json;

namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Encodes and validates the compact integer-first wire representation of a structure instance.
/// </summary>
/// <remarks>
/// Wire shape: {"v":1,"g":9001,"gv":1,"n":[[instanceId,nodeKindId]],"r":[[parentId,slotId,childId]]}.
/// Property and tuple order are canonical. Diagnostic names are intentionally omitted; the receiver
/// resolves IDs through the exact versioned definition already active in its host context.
/// The codec validates structure only. It does not allocate IDs, resolve scope, establish trust,
/// interpret meaning, authorize actions, or execute anything.
/// </remarks>
public static class GrammarStructureProposalCodec
{
    /// <summary>The current wire-format version. This is independent of a structure definition's version.</summary>
    public const uint CurrentWireVersion = 1;

    /// <summary>Serializes a validated structure instance to canonical compact JSON.</summary>
    public static string Serialize(GrammarStructureInstance instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
        {
            writer.WriteStartObject();
            writer.WriteNumber("v", CurrentWireVersion);
            writer.WriteNumber("g", instance.Definition.Id);
            writer.WriteNumber("gv", instance.Definition.Version);

            writer.WriteStartArray("n");
            foreach (var node in instance.Nodes)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(node.Id);
                writer.WriteNumberValue(node.NodeDefinitionId);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();

            writer.WriteStartArray("r");
            foreach (var relationship in instance.Relationships)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(relationship.ParentInstanceId);
                writer.WriteNumberValue(relationship.SlotId);
                writer.WriteNumberValue(relationship.ChildInstanceId);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        var json = Encoding.UTF8.GetString(stream.ToArray());
        if (json.Length > GrammarLimits.MaxProposalCharacters)
            throw new ArgumentException($"Serialized proposal exceeds {GrammarLimits.MaxProposalCharacters} characters.", nameof(instance));
        return json;
    }

    /// <summary>
    /// Deserializes a proposal against the caller-selected active definition and returns a validated graph.
    /// </summary>
    /// <exception cref="ArgumentException">The wire data is malformed, oversized, incompatible, or structurally invalid.</exception>
    public static GrammarStructureInstance Deserialize(string json, GrammarStructureDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentNullException.ThrowIfNull(definition);
        if (json.Length > GrammarLimits.MaxProposalCharacters)
            throw new ArgumentException($"Proposal exceeds {GrammarLimits.MaxProposalCharacters} characters.", nameof(json));

        try
        {
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = 8 });
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                throw new ArgumentException("A proposal must be a JSON object.", nameof(json));

            var propertyNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in root.EnumerateObject())
            {
                if (!propertyNames.Add(property.Name))
                    throw new ArgumentException($"Duplicate proposal property '{property.Name}'.", nameof(json));
            }
            if (propertyNames.Count != 5 ||
                !propertyNames.SetEquals(["v", "g", "gv", "n", "r"]))
                throw new ArgumentException("A proposal must contain exactly v, g, gv, n, and r properties.", nameof(json));

            var wireVersion = root.GetProperty("v").GetUInt32();
            if (wireVersion != CurrentWireVersion)
                throw new ArgumentException($"Unsupported proposal wire version '{wireVersion}'.", nameof(json));

            var grammarId = root.GetProperty("g").GetUInt64();
            var grammarVersion = root.GetProperty("gv").GetUInt32();
            if (grammarId != definition.Id || grammarVersion != definition.Version)
                throw new ArgumentException(
                    $"Proposal definition {grammarId}@{grammarVersion} does not match active definition {definition.Id}@{definition.Version}.",
                    nameof(json));

            var nodeArray = root.GetProperty("n");
            var relationshipArray = root.GetProperty("r");
            if (nodeArray.ValueKind != JsonValueKind.Array || relationshipArray.ValueKind != JsonValueKind.Array)
                throw new ArgumentException("Proposal n and r values must be arrays.", nameof(json));
            if (nodeArray.GetArrayLength() > GrammarLimits.MaxStructureInstances)
                throw new ArgumentException($"Proposal exceeds {GrammarLimits.MaxStructureInstances} node instances.", nameof(json));
            if (relationshipArray.GetArrayLength() > GrammarLimits.MaxStructureRelationships)
                throw new ArgumentException($"Proposal exceeds {GrammarLimits.MaxStructureRelationships} relationships.", nameof(json));

            var nodes = new List<GrammarNodeInstance>(nodeArray.GetArrayLength());
            foreach (var item in nodeArray.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Array || item.GetArrayLength() != 2)
                    throw new ArgumentException("Each n entry must be a two-integer tuple.", nameof(json));
                nodes.Add(new GrammarNodeInstance(item[0].GetUInt64(), item[1].GetUInt64()));
            }

            var relationships = new List<GrammarRelationship>(relationshipArray.GetArrayLength());
            foreach (var item in relationshipArray.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Array || item.GetArrayLength() != 3)
                    throw new ArgumentException("Each r entry must be a three-integer tuple.", nameof(json));
                relationships.Add(new GrammarRelationship(
                    item[0].GetUInt64(), item[1].GetUInt64(), item[2].GetUInt64()));
            }

            return new GrammarStructureInstance(definition, nodes, relationships);
        }
        catch (JsonException exception)
        {
            throw new ArgumentException("Proposal is not valid bounded JSON.", nameof(json), exception);
        }
        catch (FormatException exception)
        {
            throw new ArgumentException("Proposal contains a value with an invalid integer representation.", nameof(json), exception);
        }
        catch (InvalidOperationException exception)
        {
            throw new ArgumentException("Proposal contains a value of an unexpected JSON type.", nameof(json), exception);
        }
    }
}
