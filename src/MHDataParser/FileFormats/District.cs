using MHDataParser.Prototypes;
using MHDataParser.Prototypes.Markers;

namespace MHDataParser.FileFormats
{
    public class DistrictPrototype
    {
        public ResourceHeader Header { get; }
        public MarkerSetPrototype CellMarkerSet { get; }
        public MarkerSetPrototype MarkerSet { get; }                 // Size is always 0 in all of our files
        public PathCollectionPrototype PathCollection { get; }

        public DistrictPrototype(byte[] data, bool useLegacyFormat)
        {
            using (MemoryStream stream = new(data))
            using (BinaryReader reader = new(stream))
            {
                Header = new(reader);
                CellMarkerSet = new(reader, useLegacyFormat);
                MarkerSet = new(reader, useLegacyFormat);
                PathCollection = new(reader);
            }
        }
    }
}
