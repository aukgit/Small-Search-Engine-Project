using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using System.Threading;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
namespace SearchEngine.Modules {


    /// <summary>
    /// Folder, Files, Reading , Writing, and addition Search in shortterms.
    /// </summary>
    class Folder {

        AxShockwaveFlashObjects.AxShockwaveFlash Flashx;
        FlashPropertise Flash;
        public List<string> directories;
        public string[] InludingExtensionList { get; set; }
        public string[] ExcludingExtensionList { get; set; }

        public ListOfFiles<FileStructure> FilesList { get; set; }
        public Folder() {

        }

        public Folder(AxShockwaveFlashObjects.AxShockwaveFlash flash,
                      ListOfFiles<FileStructure> filesList,
                      string[] includingExtenstion,
                      string[] excludingExtenstion
                    ) {
            Flash = new FlashPropertise(flash);
            InludingExtensionList = includingExtenstion;
            ExcludingExtensionList = excludingExtenstion;
            FilesList = filesList;
        }
        //SearchEngineEntities db = new SearchEngineEntities();
        /// <summary>
        /// Returns all nested directories including the given one. O(n = all the nested directories)
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public List<string> getNestedDirectories(string directory, List<string> directories) {
            //if directory not exist or null return the existing list;

            if (!Directory.Exists(directory.ToLower()) || String.IsNullOrEmpty(directory))
                return directories;

            directory = directory.ToLower();
            directories.Add(directory); // add current directory

            try {
                string[] tempDirectories = Directory.GetDirectories(directory);
                //parallel operation
                foreach (var dir in tempDirectories) {
                    //for each directory

                    directories = getNestedDirectories(dir, directories);
                }
            } catch (Exception ex) {
                ExceptionHandle.Handle(ex);
            }
            return directories;
        }

        private bool IsAllExist(string InText, List<string> list) {
            if (list == null || list.Count == 0) {
                return false;
            }
            foreach (var item in list) {
                if (InText.IndexOf(item) == -1) {
                    return false;
                }
            }
            return true;
        }

        public bool IsMatchInText(String InText, ListOfFiles<FileStructure> fileslist) {
            InText = InText.ToLower();
            if (String.IsNullOrWhiteSpace(InText)) {
                return false;
            }
            if (InText.IndexOf(fileslist.SearchString) > -1) {
                return true;
            }

            if (IsAllExist(InText, fileslist.AllWordsArray)) {
                return true;
            }

            if (IsAllExist(InText, fileslist.SingularWordsList)) {
                return true;
            }

            if (IsAllExist(InText, fileslist.PluralsWordsList)) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns all nested directories including the given one with searched results. O(n = all the nested directories)
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public List<string> getNestedDirectories2(
            string directory,
            ref long FileNumberID,
            ref float percentIncrease,
            DateTime startDate,
            DateTime endDate,
            int dateType = -1,
            bool getContents = false,
            int FileSizeType = -1,
            long fileSizeStart = -1,
            long fileSizeEnd = -1,
            bool lookForDate = false) {
            //if directory not exist or null return the existing list;

            if (!Directory.Exists(directory.ToLower()) || String.IsNullOrEmpty(directory))
                return directories;

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


            directory = directory.ToLower();
            directories.Add(directory); // add current directory
            if (FilesList.FolderAlreadyRead == null) {
                FilesList.FolderAlreadyRead = new List<string>();
            }

            FilesList.FolderAlreadyRead.Add(directory);

            new Thread(() => {
                if (IsMatchInText(directory, FilesList)) {
                    Flash.addListItem("(Directory) : " + directory, directory);
                }
            }).Start();


            #region Files Read in a directory

            var files = Directory.GetFiles(directory).ToArray();

            long count = files.Count();

            bool doNotContinue = false;
            float progressPercent2 = 0;
            float directoryLoadPercentage = 0;
            if (count == 0) {
                count = 1;
                doNotContinue = true;
            }
            if (directoryLoadPercentage == 0) {
                progressPercent2 = count / 100F;
            } else {
                progressPercent2 = directoryLoadPercentage / count;
            }

            float percentIncrease2 = percentIncrease;
            long FileID = FileNumberID;

            if (!doNotContinue) {
                //files.AsEnu(file => {
                foreach (var file in files) {
                    //new Thread(() => {
                    FileInfo fileinfo = null;
                    FileStructure filestr = null;
                   
                    fileinfo = new FileInfo(file);
                    filestr = new FileStructure() {
                        ID = ++FileID,
                        ExactLocation = fileinfo.FullName,
                        FileName = fileinfo.Name,
                        Folder = directory,
                        Extension = fileinfo.Extension,
                        CreatedDate = fileinfo.CreationTime,
                        ModifiedDate = fileinfo.LastWriteTime,
                        LastAccessDate = fileinfo.LastAccessTime,
                        SizeBytes = fileinfo.Length
                    };
                    
                    if (filestr != null) {
                        FilesList.Add(filestr);
                    }
                    if (filestr.FileName == "Technology_2_2.txt") {
                        var found = true;
                    }

                    //new Thread(() => {

                    if (!String.IsNullOrWhiteSpace(fileinfo.Extension) && filestr.Extension.IndexOf('.') > -1) {
                        filestr.Extension = filestr.Extension.Remove(0, 1);
                    }

                    var isExtensionExist = false;
                    if (InludingExtensionList != null) {
                        isExtensionExist = this.InludingExtensionList.Any(n => n == filestr.Extension);
                    }
                    var isDisableExtensionExist = false;
                    if (ExcludingExtensionList != null) {
                        isDisableExtensionExist = this.ExcludingExtensionList.Any(n => n == filestr.Extension);
                    }
                    bool addFile = false;
                    bool isExtensionRight = false;
                    bool isSizeRight = false;
                    bool isDateRight = false;
                    if (isExtensionExist && !isDisableExtensionExist) {
                        isExtensionRight = true;
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
                        filestr.Content = ReadFile(file, filestr.SizeBytes);
                        if (filestr.IsContentEmpty) { }
                    }



                    if (addFile) {
                        //if found by the size
                        new Thread(() => {
                            if (IsMatchInText(filestr.FileName, FilesList)) {
                                Flash.addListItem(filestr.FileName, file);
                            } else if ( getContents && !filestr.IsContentEmpty && IsMatchInText(filestr.Content, FilesList)) {
                                Flash.addListItem(filestr.FileName, file);
                            } else if (getContents && filestr.SizeBytes <= FileSizeTypeStructure.MB_1 * 3) {
                                filestr.Content = this.ReadFile(filestr.ExactLocation, filestr.SizeBytes);
                                if (filestr.IsContentEmpty) { }
                                FilesList.IsSavingNecessary = true;
                            }
                        }).Start();
                    }
                    //}).Start();
                    //percentIncrease2 += progressPercent2;

                    //Flash.setProgress(percentIncrease2);
                    //}).Start();
                } //files
               
            }
            FileNumberID = FileID;
            percentIncrease = percentIncrease2;
            //need to learn how to send type of type as list to remove the null
            if (directoryLoadPercentage == 0) {
                Flash.setProgress(0);
            }
            #endregion

            try {
                string[] tempDirectories = Directory.GetDirectories(directory);
                //parallel operation
                foreach (var dir in tempDirectories) {


                    directories = getNestedDirectories2(
                            dir,
                            ref FileNumberID,
                            ref percentIncrease,
                            startDate,
                            endDate,
                            dateType,
                            getContents,
                            FileSizeType,
                            fileSizeStart,
                            fileSizeEnd,
                            lookForDate);
                }
            } catch (Exception ex) {
                ExceptionHandle.Handle(ex);
            }
            return directories;
        }

        #region File Read Write and binaries

        // Convert an object to a byte array
        public byte[] ObjectToByteArray(Object obj) {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        // Convert a byte array to an Object
        public Object ReadFromBinaryObject(byte[] arrBytes) {
            if (arrBytes == null || arrBytes.Length == 0) {
                return null;
            }
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }

        public void WriteObjectAsBinaryIntoFile(string fileNamelocation, object FilesList) {
            // Write data to Test.data.
            //new Thread(() => {
            if (File.Exists(fileNamelocation)) {
                File.Delete(fileNamelocation);
            }
            // write files into binary
            try {
                FileStream fs = new FileStream(fileNamelocation, FileMode.CreateNew);
                // Create the writer for data.
                BinaryWriter w = new BinaryWriter(fs);
                byte[] BinaryObj = this.ObjectToByteArray(FilesList);
                w.Write(BinaryObj);
                w.Close();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

            //}).Start();
        }


        public object ReadObjectFromBinaryFile(string fileNamelocation) {
            if (File.Exists(fileNamelocation)) {
                try {
                    byte[] fileBytes = File.ReadAllBytes(fileNamelocation);
                    return this.ReadFromBinaryObject(fileBytes);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    return null;
                }

            } else {
                return null;
            }
        }
        #endregion


        /// <summary>
        /// Read all files from a directory list
        /// </summary>
        /// <param name="FilesList"></param>
        /// <param name="directories"></param>
        /// <param name="GenerateMD5"></param>
        /// <param name="getConents"></param>
        /// <param name="Extensions"></param>
        /// <param name="fileSizeStart"></param>
        /// <param name="fileSizeEnd"></param>
        /// <param name="FileSizeType"></param>
        /// <param name="flash"></param>
        /// <returns></returns>
        public ListOfFiles<FileStructure> getFiles(
                    ListOfFiles<FileStructure> FilesList,
                    List<string> directories,
                    DateTime startDate,
                    DateTime endDate,
                    int dateType = -1,
                    bool lookForDates = false,
                    bool GenerateMD5 = false,
                    bool getConents = false,
                    string IncludeExtensions = "",
                    string ExcludeExtensions = "",
                    int fileSizeStart = -1,
                    int fileSizeEnd = -1,
                    int FileSizeType = -1) {


            EncryptionGenerate md5 = new EncryptionGenerate();
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
                FileSizeType = -1;
            }
            if (directories.Count == 0) {
                return FilesList;
            }
            int CountDirectories = directories.Count;
            float progressPercent = 100F / CountDirectories; // per folder percentage
            float percentIncrease = 0;

            //List of extensions
            string[] IncludeExtList = null;
            string[] ExcludeExtList = null;
            if (!String.IsNullOrWhiteSpace(IncludeExtensions)) {
                IncludeExtList = IncludeExtensions.Split(',');
            }

            if (!String.IsNullOrWhiteSpace(ExcludeExtensions)) {
                ExcludeExtList = ExcludeExtensions.Split(',');
            }

            long FileNumberID = 0;
            //int threadCount = 0;
            //Thread[] threads; //new Thread[CountDirectories];
            directories.AsParallel().ForAll(directory => {
                //new Thread(() => {
                FilesList = getFile(FilesList,
                                    directory,
                                    ref FileNumberID,
                                    startDate,
                                    endDate,
                                    ref percentIncrease,
                                    dateType,
                                    lookForDates,
                                    progressPercent,
                                    GenerateMD5,
                                    getConents,
                                    IncludeExtList,
                                    ExcludeExtList,
                                    fileSizeStart,
                                    fileSizeEnd,
                                    FileSizeType
                                    );
                //}).Start();
            });//directory


            //need to learn how to send type of type as list to remove the null
            //foreach (var tr in threads) {
            //    tr.Join();
            //}
            Flash.setProgress(0);
            return FilesList;
        }


        /// <summary>
        /// Read a single file
        /// </summary>
        /// <param name="FilesList"></param>
        /// <param name="directory"></param>
        /// <param name="FileNumberID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="percentIncrease"></param>
        /// <param name="dateType"></param>
        /// <param name="lookForDates"></param>
        /// <param name="directoryLoadPercentage"></param>
        /// <param name="GenerateMD5"></param>
        /// <param name="getConents"></param>
        /// <param name="IncludeExtensions"></param>
        /// <param name="ExcludeExtensions"></param>
        /// <param name="fileSizeStart"></param>
        /// <param name="fileSizeEnd"></param>
        /// <param name="FileSizeType"></param>
        /// <param name="flash"></param>
        /// <returns></returns>
        public ListOfFiles<FileStructure> getFile(
            ListOfFiles<FileStructure> FilesList,
            string directory,
            ref long FileNumberID,
            DateTime startDate,
            DateTime endDate,
            ref float percentIncrease,
            int dateType = -1,
            bool lookForDates = false,
            float directoryLoadPercentage = 0,
            bool GenerateMD5 = false,
            bool getConents = false,
            string[] IncludeExtensions = null,
            string[] ExcludeExtensions = null,
            int fileSizeStart = -1,
            int fileSizeEnd = -1,
            int FileSizeType = -1
          ) {

            //decimal progressPercent = 100 / (decimal)directories.Count; // per folder percentage
            //decimal percentIncrease = 0;
            //directories
            //    .AsParallel()
            //    .WithDegreeOfParallelism(20)
            //    .ForAll(directory => { 
            //        //counting all files number
            //        filesCount += Directory.GetFiles(directory).Count();
            //    });
            EncryptionGenerate md5 = null;
            if (GenerateMD5) {
                md5 = new EncryptionGenerate();
            }




            var files = Directory.GetFiles(directory).ToArray();
            //FolderLocation folderlocation = new FolderLocation() {
            //    Location = directory,
            //    Dated = DateTime.Now
            //};

            //Thread.Sleep(2000);
            // adding folder location entity object to the ORM model
            //db.FolderLocations.AddObject(folderlocation);
            // Save& committing Database Changes
            //db.SaveChanges();
            // running it parallel
            long count = files.Count();

            bool doNotContinue = false;
            float progressPercent2 = 0;
            if (count == 0) {
                count = 1;
                doNotContinue = true;
            }
            if (directoryLoadPercentage == 0) {
                progressPercent2 = count / 100F;
            } else {
                progressPercent2 = directoryLoadPercentage / count;
            }

            //float percentIncrease2 = percentIncrease;
            //long FileID = FileNumberID;

            if (!doNotContinue) {
                //files.AsParallel().ForAll(file => {
                foreach (var file in files) {

                    bool addFile = false;
                    FileInfo fileinfo = new FileInfo(file);
                    FileStructure filestr = new FileStructure() {
                        ID = ++FileNumberID,
                        ExactLocation = fileinfo.FullName,
                        FileName = fileinfo.Name,
                        Folder = directory,

                        CreatedDate = fileinfo.CreationTime,
                        ModifiedDate = fileinfo.LastWriteTime,
                        LastAccessDate = fileinfo.LastAccessTime,
                        SizeBytes = fileinfo.Length
                    };
                    if (filestr.FileName.IndexOf('.') > -1) {
                        filestr.Extension = fileinfo.Extension.Remove(0, 1);
                    }

                    if (FileSizeType != -1) {
                        if (filestr.SizeBytes >= fileSizeStart && filestr.SizeBytes <= fileSizeEnd) {
                            addFile = true; //only if size meets the condition then add the file.
                        }
                    } else {
                        addFile = true; //no size limit
                    }

                    if (GenerateMD5) {
                        filestr.MD5 = md5.GetMD5FromFile(file);
                    }

                    if (getConents) {
                        filestr.Content = ReadFile(file, filestr.SizeBytes);
                        FilesList.IsSavingNecessary = true;
                        if (filestr.IsContentEmpty) { }
                    }


                    if (filestr != null) {
                        FilesList.Add(filestr);
                    }
                    if (addFile) {
                        //if found by the size

                    }
                    percentIncrease += progressPercent2;

                    Flash.setProgress(percentIncrease);

                }; //files
                //progressbar.Value += 1;
                if (FilesList.FolderAlreadyRead == null) {
                    FilesList.FolderAlreadyRead = new List<string>();
                }
                //after reading the directory.
                if (!FilesList.FolderAlreadyRead.Exists(n => n == directory)) {
                    //if not exist then add
                    FilesList.FolderAlreadyRead.Add(directory);
                }
            }

            //need to learn how to send type of type as list to remove the null
            if (directoryLoadPercentage == 0) {
                Flash.setProgress(0);
            }

            //FileNumberID = FileID;
            //percentIncrease = percentIncrease2;

            return FilesList;
        }

        /// <summary>
        /// Read file context based on conditions and division method of 4000
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileSizeInBytes"></param>
        /// <param name="FileContext"></param>
        public string ReadFile(string path, long fileSizeInBytes) {
            if (File.Exists(path)) {
                if (fileSizeInBytes <= FileSizeTypeStructure.MB_1 * 3) {
                    try {
                        return File.ReadAllText(path);
                    } catch (Exception) {
                        return "";
                    }
                }
            } else {
                ExceptionHandle.Handle(new Exception("File does not exist at the location : " + path));
            }
            return "";
        }





    }
}
