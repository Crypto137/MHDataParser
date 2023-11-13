namespace MHDataParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (GameDatabase.IsInitialized == false)
            {
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Type 'help' for a list of available commands");

            while (true)
            {
                string input = Console.ReadLine();
                CommandHandler.HandleCommand(input);
            }
        }
    }
}
