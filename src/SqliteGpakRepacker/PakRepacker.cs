using Dapper;
using K4os.Compression.LZ4;
using System.Data.SQLite;

namespace SqliteGpakRepacker
{
    public static class PakRepacker
    {
        private const string CalligraphyPakFileName = "Calligraphy.sip";
        private const string ResourcePakFileName = "mu_cdata.sip";

        private static readonly byte[] CompressionBuffer = new byte[1024 * 1024 * 8];

        public static bool Repack(string filePath, string outputDirectory)
        {
            if (File.Exists(filePath) == false)
            {
                Console.WriteLine($"{filePath} not found");
                return false;
            }

            // Read and convert
            Console.WriteLine($"Reading SQLite pak {Path.GetFileName(filePath)}...");

            if (ReadSqlitePak(filePath, out PakFile calligraphyPak, out PakFile resourcePak) == false)
            {
                Console.WriteLine("Failed to read SQLite pak");
                return false;
            }

            Console.WriteLine("Finished reading SQLite pak");

            // Write converted paks
            Console.WriteLine($"Writing converted paks to {outputDirectory}...");

            if (WritePaks(outputDirectory, calligraphyPak, resourcePak) == false)
            {
                Console.WriteLine("Failed to write converted paks");
                return false;
            }

            Console.WriteLine("Finished writing converted paks");
            return true;
        }

        private static bool ReadSqlitePak(string filePath, out PakFile calligraphyPak, out PakFile resourcePak)
        {
            calligraphyPak = new();
            resourcePak = new();

            try
            {
                using SQLiteConnection connection = new($"Data Source={filePath}");

                SqliteVersion version = connection.QueryFirst<SqliteVersion>("SELECT v as Version, s as Description FROM ver");
                if (version == null)
                {
                    Console.WriteLine("Failed to read SQLite pak version");
                    return false;
                }

                switch (version.Version)
                {
                    case 1.5f:
                    case 1.6f:
                        Console.WriteLine($"Found known SQLite pak version: {version}");
                        break;

                    default:
                        Console.WriteLine($"Found unknown SQLite pak version: {version}\nPlease report this to the developers of this tool.");
                        return false;
                }

                bool isCompressed = version.Version >= 1.6f;

                IEnumerable<SqlitePakEntry> entries = connection.Query<SqlitePakEntry>("SELECT i as Id, n as Name, b as Blob, l as Length, s as SavedTime FROM data_tbl");
                foreach (SqlitePakEntry entry in entries)
                {
                    int uncompressedSize;
                    byte[] blob;

                    if (isCompressed)
                    {
                        uncompressedSize = entry.Length;
                        blob = entry.Blob;
                    }
                    else
                    {
                        // Older SQLite paks are uncompressed
                        uncompressedSize = entry.Blob.Length;
                        int compressedSize = LZ4Codec.Encode(entry.Blob, CompressionBuffer);
                        blob = CompressionBuffer.AsSpan(0, compressedSize).ToArray();
                    }

                    PakFile pakFile = entry.Name.StartsWith("Calligraphy", StringComparison.Ordinal) ? calligraphyPak : resourcePak;
                    pakFile.AddEntry((ulong)entry.Id, entry.Name, blob, uncompressedSize, entry.SavedTime);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        private static bool WritePaks(string outputDirectory, PakFile calligraphyPak, PakFile resourcePak)
        {
            if (Directory.Exists(outputDirectory) == false)
                Directory.CreateDirectory(outputDirectory);

            if (WritePak(outputDirectory, CalligraphyPakFileName, calligraphyPak) == false)
                return false;

            if (WritePak(outputDirectory, ResourcePakFileName, resourcePak) == false)
                return false;

            return true;
        }

        private static bool WritePak(string outputDirectory, string fileName, PakFile pakFile)
        {
            Console.WriteLine($"Writing {fileName} ({pakFile.EntryCount} entries)...");
            string filePath = Path.Combine(outputDirectory, fileName);

            try
            {
                using FileStream fs = File.OpenWrite(filePath);
                pakFile.WriteToStream(fs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine($"Failed to write {fileName}");
                return false;
            }

            return true;
        }
    }
}
