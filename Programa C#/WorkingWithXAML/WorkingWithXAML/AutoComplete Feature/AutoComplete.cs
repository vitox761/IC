using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
namespace AutoComplete.Classes
{
    public static class Suggester
    {
        private static bool _isNormalizing;
        private static int typedLetterCount;
        private static int totalLetterCount;
        private static string textToBeAnalysed;
        private static string textToBeIndexed;
        private static LuceneSearch luceneSearch;
        private static int overWeight;
        private static volatile bool _stopNow;


        public static void initializeSuggester()
        {
            luceneSearch = new LuceneSearch("lucene_index");
        }

        /// <summary>
        /// Index a word written or chosen by the user, incrementing its priority in next searchs
        /// </summary>
        /// <param name="word">Word to be indexed</param>
        public static void indexWord(string word)
        {

            string[] wordsToBeIndexed = Regex.Split(word, @"\W+");
            Thread indexingThread = new Thread(() => indexInThread(wordsToBeIndexed, true));
            indexingThread.Priority = ThreadPriority.Highest;
            indexingThread.Start();
        }

        /// <summary>
        /// Index one or more words (individually) separated by any non-word characters, incrementing its priority in next searchs 
        /// </summary>
        /// <param name="words">Text to be indexed</param>
        public static void indexText(string words)
        {
            string[] wordsToBeIndexed = Regex.Split(words, @"\W+");
            Thread indexingThread = new Thread(() => indexInThread(wordsToBeIndexed, false));
            indexingThread.Priority = ThreadPriority.Highest;
            indexingThread.Start();
        }

        /// <summary>
        /// Given a prefix or a similar word, retrieves a list of words starting from the prefix, or close to it, ordered by relevance
        /// </summary>
        /// <param name="prefix">Prefix or similar word to be processed</param>
        /// <returns></returns>
        public static List<string> getSuggestions(string prefix)
        {
            List<string> suggestions = new List<string>();
            IEnumerable<DataType> searchResults = new List<DataType>();
            try
            {
                searchResults = luceneSearch.Search(prefix);
            }
            catch
            {
                searchResults = new List<DataType>();
            }
            foreach (DataType word in searchResults.Reverse<DataType>())
            {
                suggestions.Add(word.Word);
            }
            return suggestions;
        }

        /// <summary>
        /// Save last word updates from the memory to the disk
        /// </summary>
        /// <returns></returns>
        public static bool saveInDisk()
        {
            try
            {
                luceneSearch.saveCurrentIndexesToDisk();
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Retrieves all words currently indexed in the base
        /// </summary>
        /// <returns>A list of words</returns>
        public static IEnumerable<string> getAllIndexes()
        {
            List<string> suggestions = new List<string>();
            IEnumerable<DataType> searchResults = new List<DataType>();
            try
            {
                searchResults = luceneSearch.GetAllIndexRecords();
            }
            catch
            {
                return new List<string>();
            }
            IEnumerable<DataType> query = searchResults.OrderBy(word => word.Weight);
            foreach (DataType word in query.Reverse<DataType>())
            {
                suggestions.Add(word.Word);
            }
            return suggestions;
        }

        /// <summary>
        /// Delete all indexes from the base
        /// </summary>
        /// <return>'true' if the operation was succesfull, 'false' otherwise.</returns>
        public static bool deleteAllIndexes()
        {
            try
            {
                luceneSearch.ClearLuceneIndex();
                return true;
            }
            catch
            { }
            return false;
        }

        /// <summary>
        /// Private function to insert an array of words into the base
        /// </summary>
        /// <param name="text">words separated in an array form</param>
        /// <param name="isUserEntry">if it's user entry, the weight will be additionally incremented (more relevant)</param>
        private static void indexInThread(string[] text, bool isUserEntry)
        {
            int numberOfWordsIndexed = 0;
            string InsertedWord = "";
            foreach (string word in text)
            {
                if (_stopNow)
                    break;
                numberOfWordsIndexed++;

                if (word.Length > 3)
                {
                    string finalWord = RemoveDiacritics(word);
                    InsertedWord = finalWord.ToLower();
                    overWeight = luceneSearch.AddUpdateLuceneIndex(finalWord, isUserEntry);

                    if (overWeight != 0 && !_isNormalizing)
                    {
                        _isNormalizing = true;
                        IEnumerable<DataType> wordsToBeCopied = luceneSearch.GetAllIndexRecords();
                        Thread normalizeThread = new Thread(() => Normalize(wordsToBeCopied));
                        normalizeThread.Start();
                    }
                }
            }

        }

        /// <summary>
        /// If a word achieved a predefined maximum weight, normalizes all the words' weights
        /// </summary>
        /// <param name="words"></param>
        private static void Normalize(IEnumerable<DataType> words)
        {
            LuceneSearch temporarySearch = new LuceneSearch();
            foreach (DataType data in words)
            {
                data.Weight /= 2;
                temporarySearch.AddLuceneIndex(data);
            }
            luceneSearch.changeDirectory(temporarySearch.CurrentDirectory);
            _isNormalizing = false;
        }

        /// <summary>
        /// Remove Diacritics from a string
        /// </summary>
        /// <param name="value">string value to remove diacritics</param>
        /// <returns>changed string with no diacritics</returns>
        private static string RemoveDiacritics(string value)
        {
            int j;
            string r = "";

            if (value == null)
            {
                return null;
            }

            value = value.ToLower();
            for (j = 0; j < value.Length; j++)
            {
                if ((value[j] == 'á') ||
                    (value[j] == 'â') ||
                    (value[j] == 'ã'))
                {
                    r = r + "a"; continue;
                }
                if ((value[j] == 'é') ||
                    (value[j] == 'ê'))
                {
                    r = r + "e"; continue;
                }
                if (value[j] == 'í')
                {
                    r = r + "i"; continue;
                }
                if ((value[j] == 'ó') ||
                    (value[j] == 'ô') ||
                    (value[j] == 'õ'))
                {
                    r = r + "o"; continue;
                }
                if ((value[j] == 'ú') ||
                    (value[j] == 'ü'))
                {
                    r = r + "u"; continue;
                }
                if (value[j] == 'ç')
                {
                    r = r + "c"; continue;
                }
                if (value[j] == 'ñ')
                {
                    r = r + "n"; continue;
                }

                r = r + value[j];
            }

            return r;
        }
    }
}
