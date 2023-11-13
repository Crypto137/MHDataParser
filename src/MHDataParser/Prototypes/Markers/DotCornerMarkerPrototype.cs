using System.Text.Json.Serialization;
using MHDataParser.FileFormats;

namespace MHDataParser.Prototypes.Markers
{
    public class DotCornerMarkerPrototype : MarkerPrototype
    {
        [JsonPropertyOrder(2)]
        public Vector3 Extents { get; }

        public DotCornerMarkerPrototype(BinaryReader reader)
        {
            ProtoNameHash = ResourcePrototypeHash.DotCornerMarkerPrototype;

            Extents = new(reader);

            Position = new(reader);
            Rotation = new(reader);
        }
    }
}
