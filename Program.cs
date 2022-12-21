namespace AlphabetFormatDeckChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Read all files in Decks folder
            string currentDirectory = Directory.GetCurrentDirectory() + "\\Decks";
            if (!Directory.Exists(currentDirectory))
            {
                Directory.CreateDirectory(currentDirectory);
                Console.WriteLine("\"Decks\" folder was missing. Has been created.");
            }
            else
                foreach (string file in Directory.EnumerateFiles(currentDirectory, "*.ydk"))
                {
                    string filename = file.Remove(0, currentDirectory.Count() + 1);
                    filename = filename.Remove(filename.Length - 4, 4);
                    Console.Write(filename);
                    List<string> issues = new Deck(file).IssueCards();
                    if (issues.Any())
                    {
                        Console.WriteLine(" has issues with:");
                        foreach (string card in issues)
                        {
                            Console.WriteLine("\t{0}", card);
                        }
                    }
                    else
                        Console.WriteLine(" is valid.");
                    Console.WriteLine();
                }

            Console.Write("Press X to close");
            while (Console.ReadKey().Key != ConsoleKey.X) ;
        }
    }
}