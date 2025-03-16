namespace SqliteGpakRepacker
{
    public class SqlitePakEntry
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public byte[] Blob { get; set; }
        public int Length { get; set; }
        public int SavedTime { get; set; }
    }
}
