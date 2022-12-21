using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetFormatDeckChecker
{
    internal class Deck
    {
        /// <summary>
        /// Main Deck Cards
        /// </summary>
        public Dictionary<string, int> Main { get { return main; } }
        private Dictionary<string,int> main = new Dictionary<string,int>();

        /// <summary>
        /// Side Deck Cards
        /// </summary>
        public Dictionary<string, int> Side { get { return main; } }
        private Dictionary<string,int> side = new Dictionary<string,int>();

        /// <summary>
        /// Find all the the uniquely named cards in the deck
        /// </summary>
        public List<string> UniqueNames
        {
            get
            {
                List<string> uniqueNames = new List<string>();

                // Find all unique names in the deck
                foreach (string card in main.Keys)
                    if (!uniqueNames.Contains(card))
                        uniqueNames.Add(card);
                foreach (string card in side.Keys)
                    if (!uniqueNames.Contains(card))
                        uniqueNames.Add(card);

                // Sort Alphabetically
                uniqueNames.Sort();
                return uniqueNames;
            }
        }

        /// <summary>
        /// Check if a deck is valid in the format
        /// </summary>
        /// <returns></returns>
        public List<string> IssueCards()
        {
            List<char> startingCharacters = new List<char>();
            List<char> overusedCharacters = new List<char>();
            List<string> issueCards = new List<string>();
            foreach (string name in UniqueNames)
            {
                if (startingCharacters.Contains(name[0]))
                    overusedCharacters.Add(name[0]);
                else
                    startingCharacters.Add(name[0]);
            }

            // Valid Decks
            if (overusedCharacters.Count != 0)
            {
                foreach(string name in UniqueNames)
                {
                    if (overusedCharacters.Contains(name[0]))
                    {
                        issueCards.Add(name);
                    }
                }
            }

            return issueCards;
        }

        /// <summary>
        /// A Yu-Gi-Oh! Deck
        /// </summary>
        /// <param name="filepath">Filepath to a Deck</param>
        public Deck(string filepath)
        {
            if (File.Exists(filepath))
                if(filepath.EndsWith(".ydk"))
                {
                    string[] lines = File.ReadAllLines(filepath);
                    bool siding = false;
                    foreach(string line in lines)
                        if (line.Equals("!side"))
                            siding = true;
                        else if (!line.StartsWith('#') && !string.IsNullOrWhiteSpace(line))
                            AddCard(siding, line);
                }
        }


        /// <summary>
        /// Add a card to the deck
        /// </summary>
        /// <param name="siding">Is this card sided?</param>
        /// <param name="id">ID of the card</param>
        private void AddCard(bool siding, string id)
        {
            string name = CardDatabase.Instance.GetName(id);
            if (!siding)
                if (main.ContainsKey(name))
                    main[name]++;
                else
                    main.Add(name, 1);
            else
                if (side.ContainsKey(name))
                side[name]++;
            else
                side.Add(name, 1);
        }
    }
}
