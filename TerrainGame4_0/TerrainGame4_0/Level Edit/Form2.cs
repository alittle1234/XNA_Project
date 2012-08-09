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
    public partial class Form2 : Form
    {
        ParentGame parentGame;
        
        TextInputForm inputbox = new TextInputForm();

        public Form2(ParentGame game)
        {
            InitializeComponent();

            parentGame = game;
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = "Active Flag";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            parentGame.Editor.activeFlag.Label = label_box.Text;
        }

        

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //ParseText();
        }


        void ParseText()
        {            

            String[] box_text = ConditionBox.Text.Split(new char[] { '\n' });
            foreach (String text in box_text)
            {
                Condition cond = new Condition();
                cond.Description = text;
                parentGame.Editor.activeFlag.Conditions.Add(cond);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string input;

            if (!inputbox.Visible)
            {
                Point formLoc = this.Location;
                Point butnLoc = button1.Location;
                inputbox.Show();
                inputbox.Location = new Point(formLoc.X + butnLoc.X,
                    formLoc.Y + butnLoc.Y);
                inputbox.Location = new Point(inputbox.Location.X + 55,
                    inputbox.Location.Y);
            }
            else
            {
                inputbox.Hide();
                input = inputbox.input;
                inputbox.input = "";
                inputbox.textBox1.Text = "";

                Condition cond = new Condition();
                if (input.Length > 0)
                {
                    cond.Description = input;
                    parentGame.Editor.activeFlag.Conditions.Add(cond);
                    input = "";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string input;
            if (!inputbox.Visible)
            {
                Point formLoc = this.Location;
                Point butnLoc = button2.Location;
                inputbox.Show();
                inputbox.Location = new Point(formLoc.X + butnLoc.X,
                    formLoc.Y + butnLoc.Y);
                inputbox.Location = new Point(inputbox.Location.X + 55,
                    inputbox.Location.Y);
            }
            else
            {
                inputbox.Hide();
                input = inputbox.textBox1.Text;               
                
               
                foreach (Condition condition in parentGame.Editor.activeFlag.Conditions)
                    if (condition.Description == input)
                    {
                        parentGame.Editor.activeFlag.Conditions.Remove(condition);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("inpt: " + input + "   desc: " + condition.Description);
                    }

                inputbox.input = "";
                inputbox.textBox1.Text = "";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string input;

            if (!inputbox.Visible)
            {
                Point formLoc = this.Location;
                Point butnLoc = label_change.Location;
                inputbox.Show();
                inputbox.Location = new Point(formLoc.X + butnLoc.X,
                    formLoc.Y + butnLoc.Y);
                inputbox.Location = new Point(inputbox.Location.X + 55,
                    inputbox.Location.Y);
            }
            else
            {
                inputbox.Hide();
                input = inputbox.input;
                inputbox.input = "";
                inputbox.textBox1.Text = "";

                
                if (input.Length > 0)
                {
                    parentGame.Editor.activeFlag.Label = input;
                    input = "";
                }
            }
        }

        private void delay_change_Click(object sender, EventArgs e)
        {
            string input;

            if (!inputbox.Visible)
            {
                Point formLoc = this.Location;
                Point butnLoc = delay_change.Location;
                inputbox.Show();
                inputbox.Location = new Point(formLoc.X + butnLoc.X,
                    formLoc.Y + butnLoc.Y);
                inputbox.Location = new Point(inputbox.Location.X + 55,
                    inputbox.Location.Y);
            }
            else
            {
                inputbox.Hide();
                input = inputbox.input;
                inputbox.input = "";
                inputbox.textBox1.Text = "";

                
                if (input.Length > 0)
                {
                    parentGame.Editor.activeFlag.Delay = Convert.ToInt32(input);
                    input = "";
                }
            }
        }

        private void events_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            events_list.DataSource = parentGame.Editor.activeFlag.Events;

            int selectedIndex = events_list.SelectedIndex;


            parentGame.Editor.activeEvent = parentGame.Editor.activeFlag.Events[selectedIndex];
        }

        private void event_add_Click(object sender, EventArgs e)
        {
            // The Add button was clicked.
            GameEvent g_event = new GameEvent();
            g_event.EventType = GameEvent.GameEventType.PUT_ENEMY;

            Microsoft.Xna.Framework.Vector3 position = new Microsoft.Xna.Framework.Vector3(
                parentGame.Editor.activeCorner.X, 40, parentGame.Editor.activeCorner.Y);
            g_event.SpawnPoint = new EnemySpawn(position);

            parentGame.Editor.activeFlag.Events.Add(g_event); // <-- Any string you want

            //events_list.DataSource = null;           
            events_list.DataSource = parentGame.Editor.activeFlag.Events;
           
           

        }

        private void type_change_Click(object sender, EventArgs e)
        {
            string input;

            if (!inputbox.Visible)
            {
                Point formLoc = this.Location;
                Point butnLoc = type_change.Location;

                inputbox.Show();
                inputbox.Location = new Point(formLoc.X + butnLoc.X,
                    formLoc.Y + butnLoc.Y);
                inputbox.Location = new Point(inputbox.Location.X + 55,
                    inputbox.Location.Y);

                inputbox.textBox1.Text = parentGame.Editor.activeEvent.EventType.ToString();
            }
            else
            {
                inputbox.Hide();
                input = inputbox.input;
                inputbox.input = "";
                inputbox.textBox1.Text = "";


                if (input.Length > 0)
                {
                    parentGame.Editor.activeEvent.EventType = (GameEvent.GameEventType)Convert.ToInt32(input);
                    input = "";
                }
            }
        }

        private void event_remove_Click(object sender, EventArgs e)
        {
            int index = events_list.SelectedIndex;
            parentGame.Editor.activeFlag.Events.RemoveAt(index);
        }

        private void target_change_Click(object sender, EventArgs e)
        {
            int type = (int)parentGame.Editor.activeEvent.EventType;

            switch (type)
            {
                case 0:
                    break;
                case 1:
                    change_target_Espawn();
                    break;
            }
        }

        private void change_target_Espawn()
        {
            string input;

            if (!inputbox.Visible)
            {
                Point formLoc = this.Location;
                Point butnLoc = target_change.Location;

                inputbox.Show();
                inputbox.Location = new Point(formLoc.X + butnLoc.X,
                    formLoc.Y + butnLoc.Y);
                inputbox.Location = new Point(inputbox.Location.X + 55,
                    inputbox.Location.Y);

                inputbox.textBox1.Text = parentGame.Editor.activeEvent.SpawnPoint.Position.X.ToString();
                inputbox.textBox1.Text += ",";
                inputbox.textBox1.Text += parentGame.Editor.activeEvent.SpawnPoint.Position.Y.ToString();
                inputbox.textBox1.Text += ",";
                inputbox.textBox1.Text += parentGame.Editor.activeEvent.SpawnPoint.Position.Z.ToString();

            }
            else
            {
                inputbox.Hide();
                input = inputbox.input;
                inputbox.input = "";
                inputbox.textBox1.Text = "";


                if (input.Length > 0)
                {
                    parentGame.Editor.activeEvent.SpawnPoint.Position = ConvertStringToVec3(input);
                    input = "";
                }
            }
        }

        Microsoft.Xna.Framework.Vector3 ConvertStringToVec3(string input)
        {

            Microsoft.Xna.Framework.Vector3 position = new Microsoft.Xna.Framework.Vector3();

            string[] arra = new string[3];
            arra = input.Split(new char[] { ' ', ',' });
            position.X = Convert.ToSingle(arra[0]);
            position.Y = Convert.ToSingle(arra[1]);
            position.Z = Convert.ToSingle(arra[2]);

            return position;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            parentGame.Editor.activeEvent.EventType = (GameEvent.GameEventType)index;

            events_list.DataSource = parentGame.Editor.EventTypes;
        }

    }
}
