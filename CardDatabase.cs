using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetFormatDeckChecker
{
    internal class CardDatabase
    {
        #region Constants
        private readonly string UpdateUrl = @"https://db.ygoprodeck.com/api/v7/checkDBVer.php";
        private readonly string UpdateDir = @"checkDBVer.php.json";
        private readonly string CardUrl = @"https://db.ygoprodeck.com/api/v7/cardinfo.php";
        private readonly string CardDir = @"cardinfo.php.json";
        #endregion


        /// <summary>
        /// Get the curret instance of the Card Database
        /// </summary>
        public static CardDatabase Instance
        {
            get
            {
                if(instance == null)
                    instance = new CardDatabase();
                return instance;
            }
        }

        private static CardDatabase instance;

        private Dictionary<string, string> cards;


        /// <summary>
        /// A Database of Yu-Gi-Oh! Cards
        /// </summary>
        private CardDatabase()
        {
            Setup();
        }


        /// <summary>
        /// Set up the Card Database
        /// </summary>
        private void Setup()
        {
            // Get current version of Database from API
            string webUpdate = WebGetString(UpdateUrl);
            string json = string.Empty;

            // If exists, is up to date?
            if (File.Exists(CardDir) && File.Exists(UpdateDir))
            {
                string localVersion = File.ReadAllText(UpdateDir);
                if (localVersion.Equals(webUpdate))
                    json = File.ReadAllText(CardDir);
            }

            // If does not exist / not up to date.
            if (json.Equals(string.Empty))
            {
                File.WriteAllText(UpdateDir, webUpdate);
                json = WebGetString(CardUrl);
                File.WriteAllText(CardDir, json);
            }

            // Find Cards
            GetCardIDs(json);
        }

        /// <summary>
        /// Get a string from a URL
        /// </summary>
        /// <param name="url">Url to request string from</param>
        /// <returns>String if retrieved otherwise empty string</returns>
        private string WebGetString(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var request = client.GetStringAsync(url);
                if (!request.IsFaulted)
                    return request.Result;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get all Card IDs from JSON and a TSV File
        /// </summary>
        /// <param name="json">JSON of cards</param>
        private void GetCardIDs(string json)
        {
            cards = new Dictionary<string, string>();

            // Read from JSON
            JArray jsonArray = (JArray)JObject.Parse(json)["data"];
            foreach(JObject jsonObject in jsonArray)
            {
                string name = jsonObject.SelectToken("name").ToString();
                foreach(JObject altArt in (JArray)jsonObject.SelectToken("card_images"))
                    cards.Add(altArt.SelectToken("id").ToString(), name);
            }

            // Read from manual overrides file
            if (File.Exists("ManualAliasFixes.tsv"))
            {
                string[] lines = File.ReadAllLines("ManualAliasFixes.tsv");
                foreach(string line in lines)
                {
                    string[] splits = line.Split('\t');
                    if (splits.Length == 2)
                        if (!cards.ContainsKey(splits[0]))
                            cards.Add(splits[0], splits[1]);
                        else
                            Console.Write("{0} is already present in list, can remove {0} {1}", splits[0], splits[1]);
                }
            }
        }


        /// <summary>
        /// Get the name of a card from it's ID
        /// </summary>
        /// <param name="id">ID of a card</param>
        /// <returns>Name of card if found, otherwise empty string</returns>
        public string GetName(string id)
        {
            if (cards.ContainsKey(id))
                return cards[id];
            Console.WriteLine("\tCould not find a card name matching ID {0}", id);
            return string.Empty;
        }
    }
}
