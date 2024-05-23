namespace SqliteGpakUnpacker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: SqliteGpakUnpacker.exe [filePath]");
                return;
            }

            PakUnpacker.Unpack(args[0]);
            Console.ReadKey();
        }
    }
}
