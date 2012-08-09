using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Xna.Framework.Input;

namespace TerrainTutorial
{
    public partial class Form1 : Form
    {
        ParentGame parentGame;

        public Form1(ParentGame game)
        {
            parentGame = game;

            InitializeComponent();
            
        }
        

        private void progressBar1_Click(object sender, EventArgs e)
        {         
            int mouseX = Control.MousePosition.X;
            int minMouseX = this.Location.X + progressBar1.Location.X;
            int maxMouseX = this.Location.X + progressBar1.Location.X + progressBar1.Width;

            float ratio = 100 * (((float)mouseX - (float)minMouseX) / ((float)maxMouseX - (float)minMouseX));
            parentGame.lightPower = ratio/100 * parentGame.maxLightPower;

            label2.Text = parentGame.lightPower.ToString();

            progressBar1.Value = (int)ratio;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Preferences";

            label2.Text = parentGame.lightPower.ToString();
            label4.Text = parentGame.ambientPower.ToString();

            textBox4.Text = parentGame.lightDir.X.ToString();
            textBox2.Text = parentGame.lightDir.Y.ToString();
            textBox3.Text = parentGame.lightDir.Z.ToString();

            label15.Text = parentGame.NormalThreshold.ToString();
            progressBar3.Value = (int)parentGame.NormalThreshold;

            label16.Text = parentGame.DepthThreshold.ToString();
            progressBar4.Value = (int)parentGame.DepthThreshold;

            label17.Text = parentGame.NormalSensitivity.ToString();
            progressBar5.Value = (int)parentGame.NormalSensitivity;

            label18.Text = parentGame.DepthSensitivity.ToString();
            progressBar6.Value = (int)parentGame.DepthSensitivity;

            label19.Text = parentGame.EdgeWidth.ToString();
            progressBar8.Value = (int)parentGame.EdgeWidth;

            label20.Text = parentGame.EdgeIntensity.ToString();
            progressBar7.Value = (int)parentGame.EdgeIntensity;
        
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            float single = Convert.ToSingle(textBox4.Text);
            parentGame.lightDir.X = single;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            parentGame.lightDir.Y = Convert.ToSingle(textBox2.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            parentGame.lightDir.Z = Convert.ToSingle(textBox3.Text);
        }

        private void progressBar3_Click(object sender, EventArgs e)
        {
            int mouseX = Control.MousePosition.X;
            int minMouseX = this.Location.X + progressBar3.Location.X;
            int maxMouseX = this.Location.X + progressBar3.Location.X + progressBar3.Width;

            float ratio = 100 * (((float)mouseX - (float)minMouseX) 
                / ((float)maxMouseX - (float)minMouseX));

            parentGame.NormalThreshold = ratio / 100 * progressBar3.Maximum;

            label15.Text = parentGame.NormalThreshold.ToString();

            progressBar3.Value = (int)parentGame.NormalThreshold;
        }

        private void progressBar4_Click(object sender, EventArgs e)
        {
            ProgressBar progBar = progressBar4;
            float variable = parentGame.DepthThreshold;

            int mouseX = Control.MousePosition.X;
            int minMouseX = this.Location.X + progBar.Location.X;
            int maxMouseX = this.Location.X + progBar.Location.X + progBar.Width;

            float ratio = 100 * (((float)mouseX - (float)minMouseX)
                / ((float)maxMouseX - (float)minMouseX));

            variable = ratio / 100 * progBar.Maximum;

            progBar.Value = (int)variable;

            label16.Text = variable.ToString();
            progressBar4 = progBar;
            parentGame.DepthThreshold = variable;
        }

        private void progressBar5_Click(object sender, EventArgs e)
        {
            ProgressBar progBar = progressBar5;
            float variable = parentGame.NormalSensitivity;

            int mouseX = Control.MousePosition.X;
            int minMouseX = this.Location.X + progBar.Location.X;
            int maxMouseX = this.Location.X + progBar.Location.X + progBar.Width;

            float ratio = 100 * (((float)mouseX - (float)minMouseX)
                / ((float)maxMouseX - (float)minMouseX));

            variable = ratio / 100 * progBar.Maximum;

            progBar.Value = (int)variable;

            label17.Text = variable.ToString();
            progressBar5 = progBar;
            parentGame.NormalSensitivity = variable;
        }

        private void progressBar6_Click(object sender, EventArgs e)
        {
            ProgressBar progBar = progressBar6;
            float variable = parentGame.DepthSensitivity;

            int mouseX = Control.MousePosition.X;
            int minMouseX = this.Location.X + progBar.Location.X;
            int maxMouseX = this.Location.X + progBar.Location.X + progBar.Width;

            float ratio = 100 * (((float)mouseX - (float)minMouseX)
                / ((float)maxMouseX - (float)minMouseX));

            variable = ratio / 100 * progBar.Maximum;

            progBar.Value = (int)variable;

            label18.Text = variable.ToString();
            progressBar6 = progBar;
            parentGame.DepthSensitivity = variable;
        }

        private void progressBar8_Click(object sender, EventArgs e)
        {
            ProgressBar progBar = progressBar8;
            float variable = parentGame.EdgeWidth;

            int mouseX = Control.MousePosition.X;
            int minMouseX = this.Location.X + progBar.Location.X;
            int maxMouseX = this.Location.X + progBar.Location.X + progBar.Width;

            float ratio = 100 * (((float)mouseX - (float)minMouseX)
                / ((float)maxMouseX - (float)minMouseX));

            variable = ratio / 100 * progBar.Maximum;

            progBar.Value = (int)variable;

            label19.Text = variable.ToString();
            progressBar8 = progBar;
            parentGame.EdgeWidth = variable;
        }

        private void progressBar7_Click(object sender, EventArgs e)
        {
            ProgressBar progBar = progressBar7;
            float variable = parentGame.EdgeIntensity;

            int mouseX = Control.MousePosition.X;
            int minMouseX = this.Location.X + progBar.Location.X;
            int maxMouseX = this.Location.X + progBar.Location.X + progBar.Width;

            float ratio = 100 * (((float)mouseX - (float)minMouseX)
                / ((float)maxMouseX - (float)minMouseX));

            variable = ratio / 100 * progBar.Maximum;

            progBar.Value = (int)variable;

            label20.Text = variable.ToString();
            progressBar7 = progBar;
            parentGame.EdgeIntensity = variable;
        }

        
    }
}
