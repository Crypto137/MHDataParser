namespace MHDataParser.FileFormats
{
    public class AssetType
    {
        public CalligraphyHeader Header { get; }
        public Asset[] Assets { get; }

        public AssetType(byte[] data, AssetTypeId assetTypeId)
        {
            using (MemoryStream stream = new(data))
            using (BinaryReader reader = new(stream))
            {
                Header = new(reader);

                Assets = new Asset[reader.ReadUInt16()];
                for (int i = 0; i < Assets.Length; i++)
                {
                    Assets[i] = new(reader);

                    GameDatabase.StringRefManager.AddDataRef(Assets[i].Id, Assets[i].Name);
                    GameDatabase.AddAssetIdLookup(Assets[i].Id, assetTypeId);
                }
            }
        }
    }

    public class Asset
    {
        public StringId Id { get; }
        public AssetGuid Guid { get; }
        public byte Flags { get; }
        public string Name { get; }

        public Asset(BinaryReader reader)
        {
            Id = (StringId)reader.ReadUInt64();
            Guid = (AssetGuid)reader.ReadUInt64();
            Flags = reader.ReadByte();
            Name = reader.ReadFixedString16();
        }
    }
}
