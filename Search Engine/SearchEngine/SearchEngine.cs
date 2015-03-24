using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using SearchEngine.Modules;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace SearchEngine {
    public partial class SearchEngine : Form {
        #region Declarations



        static string AppPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        List<string> SearchDirectories = new List<string>();
        List<string> TempSearchDirectories = new List<string>();
        string strLocations = "";

        /// <summary>
        /// database
        /// </summary>
        //SearchEngineEntities db = new SearchEngineEntities();

        /// <summary>
        /// For using search engine algorithm
        /// </summary>
        CoreSearchAlgorithm searchAlgorithm = new CoreSearchAlgorithm();


        Folder folder;

        /// <summary>
        /// Started and end time of the operations
        /// </summary>
        //DateTime startedTime, endTime;

        /// <summary>
        /// for run the method("GenerateFileNames") to get files in the background
        /// </summary>
        Thread threadGetFileNames;

        string CustomDataLocation = AppPath + @"\Data\";
        string MovieLocation = AppPath + @"\Processor.type";
        /// <summary>
        /// all files list with all additional & required information
        /// </summary>
        ListOfFiles<FileStructure> FilesList;

        string[] ExcludingExtensionList, IncludingExtensionList;

        /// <summary>
        /// database
        /// </summary>
        SearchEngineEntities2 db = new SearchEngineEntities2();
        /// <summary>
        /// Flash properties extender (Custom Class)
        /// </summary>
        FlashPropertise Flash;
        #endregion

        public SearchEngine() {
            InitializeComponent();
            threadGetFileNames = new Thread(() => GetDirectory());

            //SearchDirectories.Add(@"C:\Users\Alim\Desktop\Test 1");
            //SearchDirectories.Add(@"F:\TvShow\Mentlist Unordered");
            //SearchDirectories.Add(@"E:\Working");
            //SearchDirectories.Add(@"\\172.195.16.8\One Tera Byte New Hardrive");
            //SearchDirectories.Add(@"\\172.195.16.8\Animated Movie");
            //SearchDirectories.Add(@"\\172.195.16.8\Tv show 2");
            //SearchDirectories.Add(@"\\172.195.16.8\Games Collection 3");
            //SearchDirectories.Add(@"\\172.195.16.8\Hindi Movie");
            //SearchDirectories.Add(@"\\172.195.16.7\2012 Enlish Movie");


            if (!Directory.Exists(CustomDataLocation)) {
                Directory.CreateDirectory(CustomDataLocation);
            }
            Flash = new FlashPropertise(SearchEngineInterfaceFlash);
            int size = 10 * 1024 * 1024;
            FilesList = new ListOfFiles<FileStructure>();

            FilesList.Capacity = size;

            folder = new Folder(SearchEngineInterfaceFlash, FilesList, IncludingExtensionList, ExcludingExtensionList);

        }


        private string GetDirectory() {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                return dialog.SelectedPath.ToString();
            } else {
                return null;
            }
        }

        private void RemoveSearchingDirectories() {
            new Thread(() => {
                string loc = CustomDataLocation + "SearchingDirectories.text";
                try {
                    File.Delete(loc);
                } catch (Exception) {
                }
            }).Start();
        }

        private void SaveSearchingDirectories() {
            new Thread(() => {
                string loc = CustomDataLocation + "SearchingDirectories.text";
                folder.WriteObjectAsBinaryIntoFile(loc, SearchDirectories);
            }).Start();
        }

        private void LoadSearchingDirectories() {
            //new Thread(() => {
            string loc = CustomDataLocation + "SearchingDirectories.text";
            if (File.Exists(loc)) {
                SearchDirectories = (List<string>)folder.ReadObjectFromBinaryFile(loc);
                strLocations = String.Join(" ; ", SearchDirectories);
                string req = "<invoke name='setLocation' returnType='void'><arguments><string>" + strLocations + "</string></arguments></invoke>";

                Flash.CallFunction(req);
            }
            //}).Start();
        }



        /// <summary>
        /// Load all nested file names in the fileList List 
        /// </summary>
        /// <param name="startingSize"></param>
        /// <param name="endingSize"></param>
        /// <param name="sizeType"></param>
        public void GenerateFileNames(DateTime startDate,
            DateTime endDate,
            int dateType = -1,
            bool lookForDates = false,
            bool getContents = false,
            long startingSize = -1,
            long endingSize = -1,
            int sizeType = -1,
            string includingExtension = "",
            string excudingExtension = "",
            bool searchFromCache = false,
            int expireCacheDays = -1
            ) {
            List<string> nestedDirectories = new List<string>();
            Flash.clearList();

            folder.directories = new List<string>();
            long FileID = 0;
            bool PreviousListSame = false;
            bool NeedToSearchAgain = false;
            TempSearchDirectories = TempSearchDirectories.OrderBy(n => n).ToList();
            SearchDirectories = SearchDirectories.OrderBy(n => n).ToList();
            if (TempSearchDirectories.SequenceEqual(SearchDirectories)) {
                PreviousListSame = true;
            } else {
                TempSearchDirectories = new List<string>();
                TempSearchDirectories = TempSearchDirectories.Concat(SearchDirectories).ToList();
            }

            if (PreviousListSame) {
                // there is no need to load again just search
                NeedToSearchAgain = true;

                //if (foundInDatabase && File.Exists(folderSavelocation) && File.Exists(fileSavelocation)) {
                goto Searching;
                //} 
            } else {
                int size = 10 * 1024 * 1024;
                FilesList = new ListOfFiles<FileStructure>();

                FilesList.Capacity = size;
                FilesList.SearchString = Flash.GetSearchText();
                folder.FilesList = FilesList;
            }

            SearchDirectories.ForEach(searchDiretory => {

                //writing into the database.
                //look if that folder exist.
                int lastIndexSlash = searchDiretory.LastIndexOf('\\');

                string directoryName = "";
                if (lastIndexSlash > -1) {
                    directoryName = searchDiretory.Substring(lastIndexSlash + 1, searchDiretory.Length - lastIndexSlash - 1);
                }

                // look for this directoryName in the database.
                // in the database DirectoryName has been set to clustered index.
                // so it will be faster than search for exact location.

                var foundDirectories = db.FolderTypes.Where(m => m.FolderName == directoryName).ToList();
                var CurrentDirectoryFromDatabase = foundDirectories.FirstOrDefault(m => m.FolderExactLocation == searchDiretory);
                float percentIncrease = 0;

                bool foundInDatabase = CurrentDirectoryFromDatabase != null;
                bool CacheExpired = false;
                string folderSavelocation = "";
                string fileSavelocation = "";
                // found in database
                if (foundInDatabase) {
                    folderSavelocation = CustomDataLocation + CurrentDirectoryFromDatabase.FolderID + ".txt";
                    fileSavelocation = CustomDataLocation + CurrentDirectoryFromDatabase.FolderID + "_files.txt";
                }

                if (foundInDatabase && CurrentDirectoryFromDatabase.SuccessfullyCreated) {
                    var diff = DateTime.Now - CurrentDirectoryFromDatabase.LastEntry;
                    var days = diff.TotalDays / 24;
                    bool fileNotExist = !File.Exists(folderSavelocation);
                    bool daysOver = days > expireCacheDays && searchFromCache;
                    bool regularCacheForHours = !searchFromCache && (diff.TotalMinutes > 50);
                    if (fileNotExist || daysOver || regularCacheForHours) {
                        CacheExpired = true;
                    }

                    if (lookForDates) {
                        CacheExpired = true;

                    }
                } else {
                    CacheExpired = true;
                }

                if (!foundInDatabase) {
                    // create entry
                    // create folder into database
                    FolderType foldertype = new FolderType() {
                        FolderExactLocation = searchDiretory,
                        FolderName = directoryName,
                        LastEntry = DateTime.Now,
                        ExcludeExt = excudingExtension,
                        IncludeExt = includingExtension,
                        FileSizeLimit = sizeType,
                        StartingLimit = startingSize,
                        EndingLimit = endingSize,
                        SearchingText = FilesList.GetSearchingTextAsOrganizeString,
                        SuccessfullyCreated = false
                    };
                    db.FolderTypes.AddObject(foldertype);

                    db.SaveChanges();
                    // new folder and load
                    folder.getNestedDirectories2(
                           searchDiretory,
                           ref FileID,
                           ref percentIncrease,
                           startDate,
                           endDate,
                           dateType,
                           getContents,
                           sizeType,
                           startingSize,
                           endingSize,
                           lookForDates);



                    //creating time , folder locations reassigned
                    folderSavelocation = CustomDataLocation + foldertype.FolderID + ".txt";
                    fileSavelocation = CustomDataLocation + foldertype.FolderID + "_files.txt";


                    nestedDirectories = folder.directories;

                    Flash.StopInditerminateSateProcessing();
                    Flash.FlashStopGlobalTimer();

                    //if again read then write it into file system
                    FilesList.IsSavingNecessary = false;
                    folder.WriteObjectAsBinaryIntoFile(folderSavelocation, nestedDirectories);
                    folder.WriteObjectAsBinaryIntoFile(fileSavelocation, FilesList);

                    foldertype.SuccessfullyCreated = true;

                    db.SaveChanges();
                } else {
                    //found in database
                    if (CacheExpired) {
                        // if cache is expired than 3 days then load again
                        //expired and load again

                        CurrentDirectoryFromDatabase.SuccessfullyCreated = false;

                        db.SaveChanges();

                        folder.getNestedDirectories2(
                            searchDiretory,
                            ref FileID,
                            ref percentIncrease,
                            startDate,
                            endDate,
                            dateType,
                            getContents,
                            sizeType,
                            startingSize,
                            endingSize,
                            lookForDates);
                        nestedDirectories = folder.directories;

                        Flash.StopInditerminateSateProcessing();
                        Flash.FlashStopGlobalTimer();

                        //if again read then write it into file system
                        new Thread(() => {
                            FilesList.IsSavingNecessary = false;

                            folder.WriteObjectAsBinaryIntoFile(folderSavelocation, nestedDirectories);
                            folder.WriteObjectAsBinaryIntoFile(fileSavelocation, FilesList);
                        }).Start();

                        //update database with date.
                        CurrentDirectoryFromDatabase.LastEntry = DateTime.Now;
                        CurrentDirectoryFromDatabase.SearchingText = FilesList.GetSearchingTextAsOrganizeString;
                        CurrentDirectoryFromDatabase.IncludeExt = includingExtension;
                        CurrentDirectoryFromDatabase.ExcludeExt = excudingExtension;
                        CurrentDirectoryFromDatabase.StartingLimit = startingSize;
                        CurrentDirectoryFromDatabase.EndingLimit = endingSize;
                        CurrentDirectoryFromDatabase.FileSizeLimit = sizeType;
                        CurrentDirectoryFromDatabase.SuccessfullyCreated = true;
                        db.SaveChanges();
                    } else {
                        // if cache exist then load it from file.

                        var cachedDirectories = ((List<string>)folder.ReadObjectFromBinaryFile(folderSavelocation));
                        if (cachedDirectories != null) {
                            nestedDirectories = nestedDirectories.Concat(cachedDirectories).ToList();
                        }

                        var cachedFiles = ((ListOfFiles<FileStructure>)folder.ReadObjectFromBinaryFile(fileSavelocation));
                        if (cachedFiles != null) {
                            cachedFiles.AsParallel().ForAll(cacheFile => {
                                FilesList.Add(cacheFile);
                            });
                        }
                        if (FilesList.IsSavingNecessary) {
                            new Thread(() => {
                                FilesList.IsSavingNecessary = false;
                                folder.WriteObjectAsBinaryIntoFile(folderSavelocation, nestedDirectories);
                                folder.WriteObjectAsBinaryIntoFile(fileSavelocation, FilesList);
                            }).Start();
                        }
                        NeedToSearchAgain = true;
                    }

                }


            });

        Searching:
            if (NeedToSearchAgain) {
                searchAlgorithm.SearchAndAdd(FilesList,
                                            getContents,
                                            IncludingExtensionList,
                                            ExcludingExtensionList,
                                            startDate,
                                            endDate,
                                            dateType,
                                            sizeType,
                                            startingSize,
                                            endingSize,
                                            lookForDates,
                                            SearchEngineInterfaceFlash);
            }

            Flash.StopInditerminateSateProcessing();
            Flash.FlashStopGlobalTimer();

        }

        private void SearchEngineInterfaceFlash_FSCommand(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FSCommandEvent e) {
            switch (e.command) {
                case "exit":
                    if (threadGetFileNames.IsAlive) {
                        threadGetFileNames.Abort();
                    }

                    this.Close();

                    break;

                case "search-clicked":
                    string textForSearch = e.args;
                    //on enter or button pressed.
                    if (SearchDirectories.Count > 0) {
                        if (threadGetFileNames.IsAlive) {
                            threadGetFileNames.Abort();
                        }

                        threadGetFileNames = new Thread(() => {
                            long startSize = (long)Properties.Settings.Default.startingSize;
                            long endSize = (long)Properties.Settings.Default.endingSize;
                            DateTime startDate = Properties.Settings.Default.startingDate;
                            DateTime endDate = Properties.Settings.Default.endingDate;
                            string inlcudeExt = Properties.Settings.Default.searchingExtension;
                            string excludeExt = Properties.Settings.Default.searchingDisableExtension;
                            int sizeType = Properties.Settings.Default.searchingSizeType;
                            int dateType = Properties.Settings.Default.searchingDatesType;
                            bool lookForDates = Properties.Settings.Default.lookForDates;
                            bool getContents = Properties.Settings.Default.searchForContent;
                            int cacheExpire = Properties.Settings.Default.expireCacheDays;
                            bool searchFromCache = Properties.Settings.Default.searchFromCache;
                            if (!String.IsNullOrWhiteSpace(inlcudeExt)) {
                                IncludingExtensionList = inlcudeExt.Split(',');
                            }

                            if (!String.IsNullOrWhiteSpace(excludeExt)) {
                                ExcludingExtensionList = excludeExt.Split(',');
                            }
                            folder.InludingExtensionList = IncludingExtensionList;
                            folder.ExcludingExtensionList = ExcludingExtensionList;

                            FilesList.SearchString = Flash.GetSearchText();
                            GenerateFileNames(startDate,
                                                endDate,
                                                dateType,
                                                lookForDates,
                                                getContents,
                                                startSize,
                                                endSize,
                                                sizeType,
                                                inlcudeExt,
                                                excludeExt,
                                                searchFromCache,
                                                cacheExpire);
                        });

                        Flash.StartInditerminateSateProcessing("Folder");

                        threadGetFileNames.Start();


                    } else {
                        goto Browse;
                    }

                    break;
                case "clear-cache":
                    try {
                        if (Directory.Exists(CustomDataLocation)) {
                            Directory.Delete(CustomDataLocation, true);
                        }
                        Directory.CreateDirectory(CustomDataLocation);
                        SaveSearchingDirectories();

                    } catch (Exception ex) {
                        MessageBox.Show(ex.Message, "Error");
                    }

                    break;
                case "minimize":

                    this.WindowState = FormWindowState.Minimized;
                    break;

                case "foundlist-click":

                    break;
                case "foundlist-dblclick":
                    string locationOfFile = e.args;

                    Process.Start("explorer.exe", "/select," + locationOfFile);
                    break;

                case "openFile":
                    locationOfFile = e.args;
                    Process.Start(locationOfFile);
                    break;
                case "openFolder":
                    locationOfFile = e.args;
                    Process.Start("explorer.exe", "/select," + locationOfFile);
                    break;
                case "openFileInEditor":
                    locationOfFile = e.args;
                    Process.Start(locationOfFile);
                    break;
                case "browse":
                //Flash.addListItem("Hello", "World");
                //Flash.addListItem("Hello2", "World2");
                //string s2 = Flash.getListItemAt(0);
                //Flash.clearList();
                //MessageBox.Show(s2);
                Browse:
                    string location = GetDirectory();
                    if (location != null && !SearchDirectories.Exists(m => m == location)) {
                        if (strLocations != "") {
                            strLocations += "  ;  ";
                            //} else {
                            //    filesList.ListOfDirectories = new List<string>();
                        }
                        strLocations += location;
                        SearchDirectories.Add(location);
                        SearchEngineInterfaceFlash.CallFunction("<invoke name='setLocation' returnType='void'><arguments><string>" + strLocations + "</string></arguments></invoke>");
                    }
                    SaveSearchingDirectories();
                    //filesList.ListOfDirectories = SearchDirectories;
                    //GenerateFileNames();

                    break;
                case "browse-clear":
                    strLocations = "";
                    SearchDirectories.Clear();
                    SearchEngineInterfaceFlash.CallFunction("<invoke name='setLocation' returnType='void'><arguments><string>" + "Location: ?" + "</string></arguments></invoke>");
                    Flash.clearList();
                    RemoveSearchingDirectories();
                    Flash.StopInditerminateSateProcessing();
                    Flash.FlashStopGlobalTimer();
                    break;
                case "load-setting":
                    //string s = SearchEngineInterfaceFlash.CallFunction("<invoke name='getSetting'></invoke>");
                    //MessageBox.Show(s);
                    LoadSearchingDirectories();
                    string[] setting = new string[14];
                    setting[0] = Properties.Settings.Default.searchingDisableExtension;
                    setting[1] = Properties.Settings.Default.searchingExtension;
                    setting[2] = Properties.Settings.Default.searchingSizeType.ToString();
                    setting[3] = Properties.Settings.Default.startingSize.ToString();
                    setting[4] = Properties.Settings.Default.endingSize.ToString();
                    setting[5] = Properties.Settings.Default.lookForDates.ToString();
                    setting[6] = Properties.Settings.Default.searchingDatesType.ToString();
                    setting[7] = Properties.Settings.Default.startingDate.ToString();
                    setting[8] = Properties.Settings.Default.endingDate.ToString();
                    setting[9] = Properties.Settings.Default.searchForContent.ToString();
                    setting[10] = Properties.Settings.Default.contentFoundMany.ToString();
                    setting[11] = Properties.Settings.Default.searchFromDatabase.ToString();
                    setting[12] = Properties.Settings.Default.searchFromCache.ToString();
                    setting[13] = Properties.Settings.Default.expireCacheDays.ToString();

                    string s = "";

                    foreach (var item in setting) {
                        if (s.ToString() != "") {
                            s += ";";
                        } else {
                        }
                        s += item;
                    }

                    //MessageBox.Show(s);
                    //Clipboard.Clear();
                    //Clipboard.SetText(s);
                    string request = "<invoke name='setSetting' returnType='void'><arguments><string>" + s + "</string></arguments></invoke>";

                    Flash.CallFunction(request);
                    break;
                case "save-setting":
                    string settingStr = SearchEngineInterfaceFlash.CallFunction("<invoke name='getSetting'></invoke>");
                    //MessageBox.Show(settingStr);
                    XDocument xdoc = XDocument.Parse(settingStr);
                    var list = xdoc.Descendants("string").ToList();
                    //foreach (var item in list) {
                    //    MessageBox.Show(item.Value.ToString());
                    //}
                    Properties.Settings.Default.searchingDisableExtension = list[0].Value;
                    Properties.Settings.Default.searchingExtension = list[1].Value;

                    try {
                        Properties.Settings.Default.lookForDates = Boolean.Parse(list[5].Value);
                        Properties.Settings.Default.searchForContent = Boolean.Parse(list[9].Value);
                        Properties.Settings.Default.contentFoundMany = Boolean.Parse(list[10].Value);
                        Properties.Settings.Default.searchFromDatabase = Boolean.Parse(list[11].Value);
                        Properties.Settings.Default.searchFromCache = Boolean.Parse(list[12].Value);

                    } catch (Exception ex) {
                        MessageBox.Show("Message : " + ex.Message.ToString(), "Error - Boolean", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    try {
                        Properties.Settings.Default.searchingSizeType = int.Parse(list[2].Value);
                        Properties.Settings.Default.startingSize = int.Parse(list[3].Value);
                        Properties.Settings.Default.endingSize = int.Parse(list[4].Value);
                        Properties.Settings.Default.searchingDatesType = int.Parse(list[6].Value);
                        Properties.Settings.Default.expireCacheDays = int.Parse(list[13].Value); //expireCache

                    } catch (Exception ex) {
                        MessageBox.Show("Message : " + ex.Message.ToString(), "Error - Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    try {
                        Properties.Settings.Default.startingDate = DateTime.Parse(list[7].Value);
                    } catch (Exception ex) {
                        MessageBox.Show("Message : " + ex.Message.ToString(), "Error - Starting Date", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    try {
                        Properties.Settings.Default.endingDate = DateTime.Parse(list[8].Value);
                    } catch (Exception ex) {
                        MessageBox.Show("Message : " + ex.Message.ToString(), "Error - Ending Date", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Properties.Settings.Default.Save();
                    break;

                default:
                    break;
            }
        }

        private void SearchEngineInterfaceFlash_Enter(object sender, EventArgs e) {
            
        }

        private void SearchEngine_Load(object sender, EventArgs e) {
            this.SearchEngineInterfaceFlash.Movie = MovieLocation;

        }
    }
}
