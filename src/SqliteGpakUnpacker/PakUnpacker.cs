using Dapper;
using System.Data.SQLite;

namespace SqliteGpakUnpacker
{
    public static class PakUnpacker
    {
        public static void Unpack(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                Console.WriteLine($"{filePath} not found");
                return;
            }

            Console.WriteLine($"Unpacking {filePath}...");

            PakFile pakFile = new();

            try
            {
                using (SQLiteConnection connection = new($"Data Source={filePath}"))
                {
                    var entries = connection.Query<PakEntry>("SELECT i as Id, n as Name, b as Blob, l as Length, s as SavedTime FROM data_tbl");
                    foreach (PakEntry entry in entries)
                        pakFile.AddEntry(entry);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            pakFile.SaveToDisk("Output");
            Console.WriteLine($"Found and extracted {pakFile.Count} PAK entries");
        }
    }
}
