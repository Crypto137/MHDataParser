using System.Text.Json.Serialization;
using MHDataParser.FileFormats;
using MHDataParser.Prototypes;

namespace MHDataParser.JsonOutput
{
    public class NaviPatchPrototypeJson
    {
        public Vector3[] Points { get; }
        public NaviPatchEdgePrototypeJson[] Edges { get; }

        public NaviPatchPrototypeJson(NaviPatchPrototype prototype)
        {
            Points = prototype.Points;
            Edges = prototype.Edges.Select(edge => new NaviPatchEdgePrototypeJson(edge)).ToArray();
        }
    }

    public class NaviPatchEdgePrototypeJson
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResourcePrototypeHash ProtoNameHash { get; }
        public uint Index0 { get; }
        public uint Index1 { get; }
        public string Flags0 { get; }
        public string Flags1 { get; }

        public NaviPatchEdgePrototypeJson(NaviPatchEdgePrototype prototype)
        {
            ProtoNameHash = prototype.ProtoNameHash;
            Index0 = prototype.Index0;
            Index1 = prototype.Index1;
            Flags0 = string.Join("|", prototype.Flags0);
            Flags1 = string.Join("|", prototype.Flags1);
        }
    }
}
