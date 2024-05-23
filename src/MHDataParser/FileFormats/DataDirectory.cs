namespace MHDataParser.FileFormats
{
    /// <summary>
    /// An abstract class for data directory entries of all types.
    /// </summary>
    public abstract class DataRecord { }

    public class DataDirectory
    {
        private Dictionary<PrototypeId, PrototypeRecord> _prototypeRecordDict;

        public CalligraphyHeader Header { get; }
        public DataRecord[] Records { get; }

        public DataDirectory(byte[] data)
        {
            using (MemoryStream stream = new(data))
            using (BinaryReader reader = new(stream))
            {
                Header = new(reader);

                int numRecords = Header.Version == 11 ? reader.ReadInt32() : reader.ReadInt16();
                Records = new DataRecord[numRecords];

                switch (Header.Magic)
                {
                    case "CDR":     // Curve
                        for (int i = 0; i < Records.Length; i++)
                            Records[i] = new CurveRecord(reader);
                        break;
                    case "TDR":     // Asset Type
                        for (int i = 0; i < Records.Length; i++)
                            Records[i] = new AssetTypeRecord(reader);
                        break;
                    case "BDR":     // Blueprint
                        for (int i = 0; i < Records.Length; i++)
                            Records[i] = new BlueprintRecord(reader);
                        break;
                    case "PDR":     // Prototype
                        _prototypeRecordDict = new();
                        for (int i = 0; i < Records.Length; i++)
                        {
                            PrototypeRecord record = new(reader);
                            Records[i] = record;
                            _prototypeRecordDict.Add(record.Id, record);
                        }

                        break;
                    case "RDR":     // Replacement
                        for (int i = 0; i < Records.Length; i++)
                            Records[i] = new ReplacementRecord(reader);
                        break;
                }
            }
        }

        public PrototypeRecord GetPrototypeRecord(PrototypeId prototypeId)
        {
            if (_prototypeRecordDict == null) return null;

            if (_prototypeRecordDict.TryGetValue(prototypeId, out PrototypeRecord record) == false)
                return null;

            return record;
        }
    }

    public class CurveRecord : DataRecord     // CDR
    {
        public CurveId Id { get; }
        public CurveGuid Guid { get; }
        public CurveRecordFlags Flags { get; }
        public string FilePath { get; }

        public CurveRecord(BinaryReader reader)
        {
            Id = (CurveId)reader.ReadUInt64();
            Guid = (CurveGuid)reader.ReadUInt64();
            Flags = (CurveRecordFlags)reader.ReadByte();
            FilePath = reader.ReadFixedString16().Replace('\\', '/');

            GameDatabase.CurveRefManager.AddDataRef(Id, FilePath);
        }
    }

    public class AssetTypeRecord : DataRecord     // TDR
    {
        public AssetTypeId Id { get; }
        public AssetTypeGuid Guid { get; }
        public AssetTypeRecordFlags Flags { get; }
        public string FilePath { get; }

        public AssetTypeRecord(BinaryReader reader)
        {
            Id = (AssetTypeId)reader.ReadUInt64();
            Guid = (AssetTypeGuid)reader.ReadUInt64();
            Flags = (AssetTypeRecordFlags)reader.ReadByte();
            FilePath = reader.ReadFixedString16().Replace('\\', '/');

            GameDatabase.AssetTypeRefManager.AddDataRef(Id, FilePath);
        }
    }

    public class BlueprintRecord : DataRecord // BDR
    {
        public BlueprintId Id { get; }
        public BlueprintGuid Guid { get; }
        public BlueprintRecordFlags Flags { get; }
        public string FilePath { get; }

        public BlueprintRecord(BinaryReader reader)
        {
            Id = (BlueprintId)reader.ReadUInt64();
            Guid = (BlueprintGuid)reader.ReadUInt64();
            Flags = (BlueprintRecordFlags)reader.ReadByte();
            FilePath = reader.ReadFixedString16().Replace('\\', '/');

            GameDatabase.BlueprintRefManager.AddDataRef(Id, FilePath);
        }
    }

    public class PrototypeRecord : DataRecord // PDR
    {
        public PrototypeId Id { get; }
        public PrototypeGuid Guid { get; }
        public PrototypeId BlueprintId { get; }
        public PrototypeRecordFlags Flags { get; }
        public string FilePath { get; }

        public PrototypeRecord(BinaryReader reader)
        {
            Id = (PrototypeId)reader.ReadUInt64();
            Guid = (PrototypeGuid)reader.ReadUInt64();
            BlueprintId = (PrototypeId)reader.ReadUInt64();
            Flags = (PrototypeRecordFlags)reader.ReadByte();
            FilePath = reader.ReadFixedString16().Replace('\\', '/');

            GameDatabase.PrototypeRefManager.AddDataRef(Id, FilePath);
        }
    }

    public class ReplacementRecord : DataRecord   // RDR
    {
        public ulong OldGuid { get; }
        public ulong NewGuid { get; }
        public string Name { get; }

        public ReplacementRecord(BinaryReader reader)
        {
            OldGuid = reader.ReadUInt64();
            NewGuid = reader.ReadUInt64();
            Name = reader.ReadFixedString16();
        }
    }
}
