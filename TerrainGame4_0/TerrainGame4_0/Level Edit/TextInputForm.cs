using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerrainTutorial
{
    public partial class TextInputForm : Form
    {
        public string input = "";

        public TextInputForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            input = textBox1.Text;
        }
    }
}
