using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchEngine {

    struct FileSizeTypeStructure {
        public const short Bytes = 0;
        public const short KB = 1;
        public const short MB = 2;
        public const short GB = 3;
        public const short NoLimit = 4;
        public const long KB_1 = 1024;
        public const long MB_1 = 1048576;
        /// <summary>
        /// 
        /// </summary>
        public const long GB_1 = 1073741824;
    }

    struct DateTypeStructure {
        public const int Modified_Date = 0;
        public const int Access_Date = 1;
        public const int Created_Date = 2;
   
    }

}
