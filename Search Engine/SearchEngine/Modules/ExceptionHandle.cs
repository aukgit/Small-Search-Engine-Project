using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace SearchEngine.Modules {
    class ExceptionHandle {
        public static void Handle(Exception ex) {
            MessageBox.Show("Error Message : " + ex.Message, "Exception", MessageBoxButtons.OK);
        }
    }
}
