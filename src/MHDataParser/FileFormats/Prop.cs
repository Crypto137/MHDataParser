using System.Text.Json.Serialization;
using MHDataParser.Prototypes;
using MHDataParser.Prototypes.Markers;

namespace MHDataParser.FileFormats
{
    public class PropPackagePrototype
    {
        public ResourceHeader Header { get; }
        public ProceduralPropGroupPrototype[] PropGroups { get; }

        public PropPackagePrototype(byte[] data)
        {
            using (MemoryStream stream = new(data))
            using (BinaryReader reader = new(stream))
            {
                Header = new(reader);

                PropGroups = new ProceduralPropGroupPrototype[reader.ReadUInt32()];
                for (int i = 0; i < PropGroups.Length; i++)
                    PropGroups[i] = new(reader);
            }
        }
    }

    public class ProceduralPropGroupPrototype
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResourcePrototypeHash ProtoNameHash { get; }
        public string NameId { get; }
        public string PrefabPath { get; }
        public Vector3 MarkerPosition { get; }
        public Vector3 MarkerRotation { get; }
        public MarkerSetPrototype Objects { get; }
        public NaviPatchSourcePrototype NaviPatchSource { get; }
        public ushort RandomRotationDegrees { get; }
        public ushort RandomPosition { get; }

        public ProceduralPropGroupPrototype(BinaryReader reader)
        {
            ProtoNameHash = (ResourcePrototypeHash)reader.ReadUInt32();
            NameId = reader.ReadFixedString32();
            PrefabPath = reader.ReadFixedString32();
            MarkerPosition = new(reader);
            MarkerRotation = new(reader);
            Objects = new(reader);
            NaviPatchSource = new(reader);
            RandomRotationDegrees = reader.ReadUInt16();
            RandomPosition = reader.ReadUInt16();
        }
    }
}
