using System.Reflection;

namespace SqliteGpakUnpacker
{
    public class PakFile
    {
        private readonly Dictionary<string, PakEntry> _entryDict = new();

        public int Count { get => _entryDict.Count; }

        public void AddEntry(PakEntry entry)
        {
            _entryDict.Add(entry.Name, entry);
        }

        public void SaveToDisk(string outputDirectoryName)
        {
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string outputDirectory = Path.Combine(assemblyDirectory, outputDirectoryName);

            foreach (PakEntry entry in _entryDict.Values.OrderBy(entry => entry.Name))
            {
                string filePath = Path.Combine(outputDirectory, entry.Name);
                string fileDirectory = Path.GetDirectoryName(filePath);
                if (Directory.Exists(fileDirectory) == false)
                    Directory.CreateDirectory(fileDirectory);

                Console.WriteLine(entry.Name);
                File.WriteAllBytes(filePath, entry.Blob);
            }
        }
    }
}
