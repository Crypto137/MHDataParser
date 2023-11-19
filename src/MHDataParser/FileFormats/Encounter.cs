using MHDataParser.Prototypes;
using MHDataParser.Prototypes.Markers;

namespace MHDataParser.FileFormats
{
    public class EncounterPrototype
    {
        public ResourceHeader Header { get; }
        public PrototypeGuid PopulationMarkerGuid { get; }
        public string ClientMap { get; }
        public MarkerSetPrototype MarkerSet { get; }
        public NaviPatchSourcePrototype NaviPatchSource { get; }

        public EncounterPrototype(byte[] data)
        {
            using (MemoryStream stream = new(data))
            using (BinaryReader reader = new(stream))
            {
                Header = new(reader);
                PopulationMarkerGuid = (PrototypeGuid)reader.ReadUInt64();
                ClientMap = reader.ReadFixedString32();
                MarkerSet = new(reader);
                NaviPatchSource = new(reader);
            }
        }
    }
}