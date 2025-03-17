namespace SqliteGpakRepacker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("SqliteGpakRepacker");
                Console.WriteLine("Repacks legacy SQLite-based GPAK archives into the format used by later (1.29+) versions of the game.");
                Console.WriteLine("Usage: SqliteGpakRepacker.exe [filePath] [optional outputDirectory]");
            }
            else
            {
                string outputDirectory = args.Length > 1 ? args[1] : "Output";
                PakRepacker.Repack(args[0], outputDirectory);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
