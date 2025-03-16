using System.Text;

namespace SqliteGpakRepacker
{
    public class PakFile
    {
        private const uint Magic = 1196441931;  // KAPG
        private const uint Version = 1;

        private readonly Dictionary<string, Entry> _entries = new();

        public int EntryCount { get => _entries.Count; }

        public PakFile() { }

        public bool AddEntry(ulong fileHash, string filePath, byte[] blob, int uncompressedSize, int modTime)
        {
            Entry entry = new(fileHash, filePath, blob, uncompressedSize, modTime);
            return _entries.TryAdd(entry.FilePath, entry);
        }

        public bool WriteToStream(Stream stream)
        {
            Entry[] sortedEntries = _entries.Values.OrderBy(entry => entry.FileHash).ToArray();

            // Update offsets
            int offset = 0;
            foreach (Entry entry in sortedEntries)
            {
                entry.Offset = offset;
                offset += entry.CompressedSize;
            }

            using BinaryWriter writer = new(stream, Encoding.UTF8, true);

            writer.Write(Magic);
            writer.Write(Version);
            writer.Write(_entries.Count);

            foreach (Entry entry in sortedEntries)
            {
                writer.Write(entry.FileHash);
                writer.Write(entry.FilePath.Length);
                writer.Write(Encoding.UTF8.GetBytes(entry.FilePath));
                writer.Write(entry.ModTime);
                writer.Write(entry.Offset);
                writer.Write(entry.CompressedSize);
                writer.Write(entry.UncompressedSize);
            }

            foreach (Entry entry in sortedEntries)
                writer.Write(entry.Blob);

            return true;
        }

        private class Entry
        {
            public ulong FileHash { get; }
            public string FilePath { get; }
            public int ModTime { get; }
            public int Offset { get; set; }
            public int CompressedSize { get => Blob.Length; }
            public int UncompressedSize { get; }

            public byte[] Blob { get; }

            public Entry(ulong fileHash, string filePath, byte[] blob, int uncompressedSize, int modTime)
            {
                FileHash = fileHash;
                FilePath = filePath;
                Blob = blob;
                UncompressedSize = uncompressedSize;
                ModTime = modTime;
            }
        }
    }
}
