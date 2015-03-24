using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SearchEngine.Modules {
    class CoreSearchAlgorithm {

        #region Searching Text Organize Algorithms

        private Folder folder = new Folder();
        /// <summary>
        /// Organize(generate searching sentences BigO : Files x Lines)
        /// and generate searching words , sentences and hashing 
        /// words if necessary.
        /// </summary>
        /// <param name="Files">a list of file structure.</param>
        /// <param name="wordsSortedDictionary">Should we generate the SearchingWords dictionary in sorted order or not. 
        /// BigO(Files x Words ln Words) (</param>
        /// <param name="hashingNeeded">Should we generate the Hashing algorithm for each words by the first letter. 
        /// Distinct : BigO(File X Words ^ 2) ; Without Distinct : BigO(File X Words)</param>
        /// <param name="hashingDistinct">If we do hashing should we only keep distinct.
        /// Distinct : BigO(File X Words ^ 2) ; Without Distinct : BigO(File X Words)</param>
        public void OrganizeMethods(ListOfFiles<FileStructure> Files, bool wordsSortedDictionary = false, bool hashingNeeded = false, bool hashingDistinct = false) {
            // BigO : Files x Lines
            GenerateSearchingSentence(Files);
            if (wordsSortedDictionary) {
                //BigO(Files x Words ln Words)
                GenerateSearchingWords(Files);
            }

            if (hashingNeeded) {
                //Distinct : BigO(File X Words ^ 2) ; Without Distinct : BigO(File X Words)
                GenerateHashingWords(Files, hashingDistinct);
            }
        }

        public void SearchAndAdd(ListOfFiles<FileStructure> fileList,
            bool getContents,
            string[] InludingExtensionList,
            string[] ExcludingExtensionList,
            DateTime startDate,
            DateTime endDate,
            int dateType = -1,
            int FileSizeType = -1,
            long fileSizeStart = -1,
            long fileSizeEnd = -1,
            bool lookForDate = false,
            AxShockwaveFlashObjects.AxShockwaveFlash flx = null) {

            //if (fileList == null || fileList.Count == 0) {
            //    return;
            //}

            if (FileSizeType == FileSizeTypeStructure.KB) {
                fileSizeStart = fileSizeStart * (int)FileSizeTypeStructure.KB_1;
                fileSizeEnd = fileSizeEnd * (int)FileSizeTypeStructure.KB_1;
            } else if (FileSizeType == FileSizeTypeStructure.MB) {
                fileSizeStart = fileSizeStart * (int)FileSizeTypeStructure.MB_1;
                fileSizeEnd = fileSizeEnd * (int)FileSizeTypeStructure.MB_1;
            } else if (FileSizeType == FileSizeTypeStructure.GB) {
                fileSizeStart = fileSizeStart * (int)FileSizeTypeStructure.GB_1;
                fileSizeEnd = fileSizeEnd * (int)FileSizeTypeStructure.GB_1;
            } else {
                fileSizeStart = -1;
                fileSizeEnd = -1;
            }


            foreach (var filestr in fileList) {
                //new Thread(() => {
                var isExtensionExist = false;
                if (InludingExtensionList != null) {
                    isExtensionExist = InludingExtensionList.Any(n => n == filestr.Extension);
                }
                var isDisableExtensionExist = false;
                if (ExcludingExtensionList != null) {
                    isDisableExtensionExist = ExcludingExtensionList.Any(n => n == filestr.Extension);
                }
                bool addFile = false;
                bool isExtensionRight = false;
                bool isSizeRight = false;
                bool isDateRight = false;
                if (isExtensionExist && !isDisableExtensionExist) {
                    isExtensionRight = true;
                }

                if (filestr.FileName == "Biology_2_2_2.txt") {
                    string swe = filestr.FileName;
                }

                if (FileSizeType != FileSizeTypeStructure.NoLimit) {
                    if (filestr.SizeBytes >= fileSizeStart && filestr.SizeBytes <= fileSizeEnd) {
                        isSizeRight = true; //only if size meets the condition then add the file.
                    }
                } else {
                    isSizeRight = true; //no size limit
                }

                if (lookForDate) {
                    if (dateType == DateTypeStructure.Modified_Date) {
                        DateTime dt = filestr.ModifiedDate.Date;
                        if (dt != null && (dt >= startDate.Date && dt <= endDate.Date)) {
                            isDateRight = true;
                        }
                    } else if (dateType == DateTypeStructure.Created_Date) {
                        DateTime dt = filestr.CreatedDate.Date;
                        if (dt != null && (dt >= startDate.Date && dt <= endDate.Date)) {
                            isDateRight = true;
                        }
                    } else if (dateType == DateTypeStructure.Access_Date) {
                        DateTime dt = filestr.LastAccessDate.Date;
                        if (dt != null && (dt >= startDate.Date && dt <= endDate.Date)) {
                            isDateRight = true;
                        }
                    }
                } else {
                    isDateRight = true;
                }

                if (isSizeRight && isDateRight && isExtensionRight) {
                    addFile = true;
                }

                if (addFile) {
                    new Thread(() => {
                        if (folder.IsMatchInText(filestr.FileName, fileList)) {
                            FlashPropertise.addListItemStatic(filestr.FileName, filestr.ExactLocation, flx);
                        } else if (getContents && !filestr.IsContentEmpty && folder.IsMatchInText(filestr.Content, fileList)) {
                            FlashPropertise.addListItemStatic(filestr.FileName, filestr.ExactLocation, flx);
                        } else if (getContents && filestr.SizeBytes <= FileSizeTypeStructure.MB_1 * 3) {
                            filestr.Content = folder.ReadFile(filestr.ExactLocation, filestr.SizeBytes);
                            if (filestr.IsContentEmpty) { }
                            fileList.IsSavingNecessary = true;
                        }
                    }).Start();

                }
                //}).Start();
            }
        }



        /// <summary>
        /// Generate Sentences based on enter. O(Files x Lines)
        /// </summary>
        /// <param name="Files">a list of file structure.</param>
        public void GenerateSearchingSentence(ListOfFiles<FileStructure> Files) {
            if (!Files.IsSentenceGenerated && Files.Count > 0) {
                Files.AsParallel().ForAll(file => {
                    if (!file.IsContentEmpty) {
                        file.SearchingSentences = file.Content.Split(char.ConvertFromUtf32(13).ToCharArray());
                    }
                });
                Files.IsSentenceGenerated = true;
                Files.GeneratedTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Generate words based on any delimiter(,.<>@#$!%^&*()-=_:;'\"[]{}~! etc...)
        /// Return the words as a sorted list (abc,bcd,cde ...)
        /// BigO(Files x Words ln Words)
        /// </summary>
        /// <param name="Files">a list of file structure.</param>
        public void GenerateSearchingWords(ListOfFiles<FileStructure> Files) {
            if (!Files.IsWordsGenerated && Files.Count > 0) {
                char[] split = ",.<>@#$!%^&*()-=_:;'\"[]{}~! \n".ToCharArray();

                Files.AsParallel().ForAll(file => {
                    if (!file.IsContentEmpty) {
                        // O(Words + Words ln Words + Words) = O(Words ln Words)
                        file.SearchingWords = file.Content.Split(split).OrderBy(m => m).ToArray();
                    }
                });
                Files.IsWordsGenerated = true;
                Files.GeneratedTime = DateTime.Now;

            }
        }

        /// <summary>
        /// Prerequisite: GenerateSearchingWords() must execute before it.
        /// Generate hashing words based on 'a' => all words starting from a. 
        /// 'b' => all words starting from b etc...
        /// Starting with number words will be distributed in the '#' char.
        /// Distinct : O(File X Words ^ 2) ; Without Distinct : O(File X Words)
        /// </summary>
        /// <param name="Files">a list of file structure.</param>
        public void GenerateHashingWords(ListOfFiles<FileStructure> Files, bool OnlyKeepDistinct = true) {
            if (!Files.IsHashingGenerated && Files.IsWordsGenerated && Files.Count > 0) {
                // O(File X Words ^ 2 X 27) => O(File.Words ^ 2)
                Files.AsParallel().ForAll(file => {
                    if (!file.IsContentEmpty) {
                        file.HashingWords = new List<HashingOrganize>();
                        foreach (var word in file.SearchingWords) {
                            char firstLetter = word.ToLower()[0];
                            if (firstLetter >= '0' && firstLetter <= '9') {
                                // if it is a number then 
                                // keep inside #
                                firstLetter = '#';
                            }
                            var hashingObj = file.HashingWords.Where(w => w.FirstLetter == firstLetter).FirstOrDefault();
                            if (hashingObj != null) {
                                if (hashingObj.Words == null) {
                                    // new list 
                                    hashingObj.Words = new List<string>();
                                }

                                if (OnlyKeepDistinct) {

                                    // already char exist in the dictionary
                                    // so add word only, if the word is not already exist.

                                    //if word is not already exist in the dictionary.
                                    var wordExist = hashingObj.Words.Exists(w => w == word);
                                    if (!wordExist) {
                                        // not already exist .
                                        // add to the dictionary.
                                        hashingObj.Words.Add(word);
                                    }
                                } else {
                                    // do not check the repeated words.
                                    hashingObj.Words.Add(word);
                                }
                            } else {
                                // if the letter not already present then 
                                HashingOrganize hO = new HashingOrganize() {
                                    FirstLetter = firstLetter,
                                    Words = new List<string>()
                                };
                                // adding the word to the dictionary directly.
                                hO.Words.Add(word);
                                // now adding the object to the final list of HashingOrganize
                                file.HashingWords.Add(hO);
                            }
                        }

                    }
                });
                Files.IsHashingGenerated = true;
                Files.GeneratedTime = DateTime.Now;
            }
        }
        #endregion

        #region Searching

        #endregion
    }
}
