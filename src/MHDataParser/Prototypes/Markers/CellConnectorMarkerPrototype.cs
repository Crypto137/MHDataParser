using System.Text.Json.Serialization;
using MHDataParser.FileFormats;

namespace MHDataParser.Prototypes.Markers
{
    public class CellConnectorMarkerPrototype : MarkerPrototype
    {
        [JsonPropertyOrder(2)]
        public Vector3 Extents { get; }

        public CellConnectorMarkerPrototype(BinaryReader reader)
        {
            ProtoNameHash = ResourcePrototypeHash.CellConnectorMarkerPrototype;

            Extents = new(reader);

            Position = new(reader);
            Rotation = new(reader);
        }
    }
}
