using System.Text.Json.Serialization;
using MHDataParser.FileFormats;

namespace MHDataParser.Prototypes.Markers
{
    /// <summary>
    /// This is a parent class for all other MarkerPrototypes.
    /// </summary>
    public class MarkerPrototype
    {
        [JsonPropertyOrder(1), JsonConverter(typeof(JsonStringEnumConverter))]
        public ResourcePrototypeHash ProtoNameHash { get; protected set; }    // DJB hash of the class name
        [JsonPropertyOrder(15)]
        public Vector3 Position { get; protected set; }
        [JsonPropertyOrder(16)]
        public Vector3 Rotation { get; protected set; }
    }

    public class MarkerSetPrototype
    {
        public MarkerPrototype[] Markers { get; }

        public MarkerSetPrototype(BinaryReader reader, bool useLegacyFormat)
        {
            Markers = new MarkerPrototype[reader.ReadInt32()];
            for (int i = 0; i < Markers.Length; i++)
                Markers[i] = ReadMarkerPrototype(reader, useLegacyFormat);
        }

        private MarkerPrototype ReadMarkerPrototype(BinaryReader reader, bool useLegacyFormat)
        {
            ResourcePrototypeHash hash = (ResourcePrototypeHash)reader.ReadUInt32();

            switch (hash)
            {
                case ResourcePrototypeHash.CellConnectorMarkerPrototype:
                    return new CellConnectorMarkerPrototype(reader);
                case ResourcePrototypeHash.DotCornerMarkerPrototype:
                    return new DotCornerMarkerPrototype(reader);
                case ResourcePrototypeHash.EntityMarkerPrototype:
                    return new EntityMarkerPrototype(reader, useLegacyFormat);
                case ResourcePrototypeHash.RoadConnectionMarkerPrototype:
                    return new RoadConnectionMarkerPrototype(reader);
                case ResourcePrototypeHash.ResourceMarkerPrototype:
                    return new ResourceMarkerPrototype(reader);
                case ResourcePrototypeHash.UnrealPropMarkerPrototype:
                    return new UnrealPropMarkerPrototype(reader);
                default:
                    throw new($"Unknown ResourcePrototypeHash {(uint)hash}");   // Throw an exception if there's a hash for a type we didn't expect
            }
        }
    }
}
