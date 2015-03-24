using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SearchEngine.Modules;


namespace SearchEngine {
    public partial class Example : Form {
        public Example() {
            InitializeComponent();
        }

        private void Example_KeyDown(object sender, KeyEventArgs e) {
            MessageBox.Show(e.KeyValue.ToString());
        }

        private void button1_Click(object sender, EventArgs e) {
            ListOfFiles<FileStructure> ls = new ListOfFiles<FileStructure>();
            ls.IsNaturalSearchEnabled = true;
            ls.SearchString = this.textBox1.Text;
            
        }

        private void Example_Load(object sender, EventArgs e) {

        }

       
    }
}
