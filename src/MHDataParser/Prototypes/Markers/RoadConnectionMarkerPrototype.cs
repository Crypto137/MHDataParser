using System.Text.Json.Serialization;
using MHDataParser.FileFormats;

namespace MHDataParser.Prototypes.Markers
{
    public class RoadConnectionMarkerPrototype : MarkerPrototype
    {
        [JsonPropertyOrder(2)]
        public Vector3 Extents { get; }

        public RoadConnectionMarkerPrototype(BinaryReader reader)
        {
            ProtoNameHash = ResourcePrototypeHash.RoadConnectionMarkerPrototype;

            Extents = new(reader);

            Position = new(reader);
            Rotation = new(reader);
        }
    }
}
