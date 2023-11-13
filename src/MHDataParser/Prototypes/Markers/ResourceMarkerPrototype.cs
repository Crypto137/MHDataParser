using System.Text.Json.Serialization;
using MHDataParser.FileFormats;

namespace MHDataParser.Prototypes.Markers
{
    public class ResourceMarkerPrototype : MarkerPrototype
    {
        [JsonPropertyOrder(2)]
        public string Resource { get; }

        public ResourceMarkerPrototype(BinaryReader reader)
        {
            ProtoNameHash = ResourcePrototypeHash.ResourceMarkerPrototype;

            Resource = reader.ReadFixedString32();

            Position = new(reader);
            Rotation = new(reader);
        }
    }
}
