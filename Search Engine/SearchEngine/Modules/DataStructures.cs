using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.ComponentModel;

namespace SearchEngine.Modules {
    [Serializable]
    public class ListOfFiles<FileStructure> : List<FileStructure> {

        /// <summary>
        /// Saving this List to files
        /// </summary>
        public bool IsSavingNecessary = false;
        
       
        /// <summary>
        /// Gets all given words in the search phrase.
        /// </summary>
        public List<string> AllWordsArray { get; private set; }

        /// <summary>
        /// Gets all given words with plural terms
        /// </summary>
        public List<string> AllWordsAndPluralsArray { get; private set; }

        private string[] SkipSequences = {"i","are","you","doing","were", "was","doing", "to","this","these"};

        /// <summary>
        /// All Words - Skip words + replaced Plurals
        /// </summary>
        public List<string> PluralsWordsList { get; private set; }

        /// <summary>
        /// All Words - Skip words + replaced singulars
        /// </summary>
        public List<string> SingularWordsList { get; private set; }

        /// <summary>
        /// Gets exact search phrase in double quotations.
        /// </summary>
        public string ExactSearchText { get; private set; }

        /// <summary>
        /// Gets only the text combining text to search with operator 'AND"
        /// </summary>
        public string CombiningSearchText { get; private set; }
        /// <summary>
        /// Gets all words that should searched with 'OR' operator
        /// </summary>
        public string OrSearchText { get; private set; }
        /// <summary>
        /// Gtes all words that should searched with 'NOT' operator
        /// </summary>
        public string NotSearchText {
            get;
            private set;
        }

        /// <summary>
        /// List of Directories to search files for.
        /// </summary>
        public List<string> ListOfSearchingDirectories { get; set; }

        /// <summary>
        /// List of folders are already in the memory.
        /// </summary>
        public List<string> FolderAlreadyRead { get; set; }

    
        

        
        public bool IsWordsGenerated { get; set; }
        public bool IsSentenceGenerated { get; set; }
        public bool IsHashingGenerated { get; set; }

        /// <summary>
        /// Prerequisite: IsSortingEnabled must be enabled.
        /// Sort the files words based on a list of alphabet hashing table.
        /// Where 'a' => 'abcd' , 'awfef' etc.
        /// This algorithm is not good for programming script search.
        /// </summary>
        public bool IsHashingEnabled { get; set; }
        /// <summary>
        /// Sort the files words based on a to z.
        /// This algorithm is not good for programming script search.
        /// </summary>
        public bool IsSortingEnabled { get; set; }

        /// <summary>
        /// Look for exact phrases when something is double quoted.
        /// This algorithm is not good for programming script search.
        /// </summary>
        public bool IsNaturalSearchEnabled { get; set; }

        /// <summary>
        /// Save the information to the database.
        /// </summary>
        public bool IsSaveToDatabase { get; set; }

        /// <summary>
        /// Try to search from database if it is 7 day old.
        /// </summary>
        public bool IsSearchFromDatabaseEnabled { get; set; }

        /// <summary>
        /// Database info how many days old accepts.
        /// </summary>
        [DefaultValue(7)]
        public int DaysOld { get; set; }

        /// <summary>
        /// Last time of execution of any methods 
        /// (Generated Sentence,Words or Hashing);
        /// </summary>
        public DateTime GeneratedTime { get; set; }


        /// <summary>
        /// All joining text 'i','want', 'to' from "I want to"
        /// </summary>
        private List<string> searchTextCombiningList = new List<string>();
        private List<string> searchTextOrList = new List<string>();
        private List<string> searchTextNotList = new List<string>();


        private string searchingText = "";

        private bool isSearchTextEmpty() {
            return String.IsNullOrEmpty(searchingText) || String.IsNullOrWhiteSpace(searchingText);
        }

         /// <summary>
        /// Get the searching text in an organize way
        /// hello, world , "I" am here because => am because hello here I world
        /// Container
        /// </summary>
        private string _searchingTextAsOrganizeString;
        /// <summary>
        /// Get the searching text in an organize way
        /// hello, world , "I" am here because => am because hello here I world
        /// </summary>
        public string GetSearchingTextAsOrganizeString { get { return _searchingTextAsOrganizeString; } }

        /// <summary>
        /// Gets the whole search text and 
        /// sets the whole searching text.
        /// </summary>
        public string SearchString {
            get {             
                return searchingText;
            }



            set {
                SearchEngineEntities2 db = new SearchEngineEntities2();
                searchingText = value.ToLower();
                char[] splitBasedOnWords = "\"'\' ;:|\\><!@#$%^&*()~!.?*".ToCharArray();
                
                //remove string tag
                searchingText = searchingText.Replace("<string>", "");
                searchingText = searchingText.Replace("</string>", "");

                AllWordsArray = searchingText.Split(splitBasedOnWords).Where(m=> m != "").ToList();
                AllWordsArray = AllWordsArray.OrderBy(n => n).ToList();

                _searchingTextAsOrganizeString = String.Join(",", AllWordsArray);
                
                AllWordsAndPluralsArray = new List<string>();
                AllWordsAndPluralsArray.Capacity = 50;
                //remove repeated words
                AllWordsArray = AllWordsArray.Distinct().ToList();
                var allwordsWithoutSkip = AllWordsArray.Except(SkipSequences).ToList();
                PluralsWordsList = new List<string>();
                SingularWordsList = new List<string>();
                foreach (var word in allwordsWithoutSkip)
	            {
                    PluralDictionary plural;

                    plural = db.PluralDictionaries.FirstOrDefault(n => n.Single == word);

                    PluralDictionary singular = null;
                    if (plural == null) {
                       singular = db.PluralDictionaries.FirstOrDefault(n => n.Plural == word);
                    }


                    if (plural != null) {
                        SingularWordsList.Add(plural.Plural.ToLower());
                    } else if (singular != null) {
                        PluralsWordsList.Add(singular.Single.ToLower());
                    } else {
                        string wordx = word.ToLower();
                        PluralsWordsList.Add(wordx);
                        SingularWordsList.Add(wordx);
                    }
	            }
        
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search">Searching text</param>
        /// <param name="wordToLookFor">The word to look for and extract information based on natural instinct</param>
        /// <returns></returns>
        //private string getExtractionOfWord(ref string search, string wordToLookFor) {
        //    string extraction = "";
        //    if (!isSearchTextEmpty()) {



        //        //var doubleQuoteIndex = s.IndexOf('\"');

        //        var lookForWordIndex = search.IndexOf(wordToLookFor);
        //        if (lookForWordIndex > -1) {
        //            // found 
        //            var indexInArray = Array.IndexOf(AllWordsArray, wordToLookFor);
        //            var nextWord = AllWordsArray[indexInArray + 1];
        //            var actualIndexOfNextWord = search.IndexOf(nextWord);
        //            var doubleQuoteIndex = search.IndexOf('\"');

        //            // so there should be no word between not and the actualIndexOfNextWord
        //            // if there is a double quote between then 
        //            // extract the double quote.
        //            if (doubleQuoteIndex < actualIndexOfNextWord) { 
        //                // so double quote comes between.
        //                extraction += getExactSearchPhrase(ref search);
        //                ConsoleLog("Extraction On Not Double Quote : ", extraction);
        //                ConsoleLog("Text after extraction of Not : ", search);
                    
        //            }

        //        }
        //    }
        //    return extraction;
        //}

        private string getExactSearchPhrase(ref string s, int startIndex = 0) {
            string extract = "";
            if (!isSearchTextEmpty()) {

                var doubleQuoteIndex = s.IndexOf('\"', startIndex);
                if (doubleQuoteIndex > -1) {
                    //double quote exist at least one time
                    var doubleQuoteIndexEndingPoint = s.IndexOf('\"', doubleQuoteIndex + 1);
                    if (doubleQuoteIndexEndingPoint > 0) {
                        // we have 2 double quotes
                        // now extract the double quoted text
                        int len = doubleQuoteIndexEndingPoint - doubleQuoteIndex;
                        extract = s.Substring(doubleQuoteIndex + 1, len - 1);
                        ConsoleLog("Full Text : ",s);
                        ConsoleLog("Extraction On Double Quote : ", extract);
                        // now remove exact from the text
                        s = s.Remove(doubleQuoteIndex, doubleQuoteIndexEndingPoint + 2);
                    }
                }
            }
            return extract;
        }

       



        void ConsoleLog(string CaptionFrom, string value) {
            Console.Out.WriteLine(CaptionFrom + " : " + value);
        }

    }
    [Serializable]
    public class FileStructure {
        
        public long ID { get; set; }
        public string FileName { get; set; }
        public string ExactLocation { get; set; }
        public string MD5 { get; set; }
        public long SizeBytes { get; set; }
        public string Extension { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime LastAccessDate { get; set; }
        public string Folder { get; set; }

        /// <summary>
        /// File content if less than 3 MB
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Divided by space, comma , semicolon , or any other punctuation
        /// </summary>
        public string[] SearchingWords { get; set; }
        /// <summary>
        /// Divided by lines based on 'Enter'
        /// </summary>
        public string[] SearchingSentences { get; set; }

        /// <summary>
        /// Words Organized By Hashing System of 'a','b','c' etc...
        /// </summary>
        public List<HashingOrganize> HashingWords { get; set; }

        private byte isContentEmpty = 0;

        /// <summary>
        /// Found as a file
        /// </summary>
        public bool IsFound { get; set; }

        public string FoundPresentString { get; set; }

        public bool IsFolderFound { get; set; }


        /// <summary>
        /// Returns if content is empty and saves it in a field.
        /// </summary>
        public bool IsContentEmpty {
            get {
                if (isContentEmpty == 0) {
                    if (String.IsNullOrEmpty(Content) || String.IsNullOrWhiteSpace(Content)) {
                        isContentEmpty = 1;
                    } else {
                        isContentEmpty = 2;
                    }
                }
                return (isContentEmpty == 1);
            }
        }



    }

    

    public class HashingOrganize {
        public char FirstLetter { get; set; }
        public List<string> Words { get; set; }
    }

}
