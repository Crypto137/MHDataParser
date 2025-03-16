namespace SqliteGpakRepacker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
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
