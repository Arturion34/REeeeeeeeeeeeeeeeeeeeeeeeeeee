using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DND_Application_2.Classes
{
    public partial class AddMonsterGroup : Form
    {

        int count;
        NumericUpDown getCountBox;

        public AddMonsterGroup()
        {
            InitializeComponent();
        }

        public AddMonsterGroup(Monster toAdd)
        {
            this.Height = 92;
            this.Width = 250;

            Label addPrompt = new Label();
            addPrompt.Location = new Point(0, 5);
            addPrompt.Width = 170;
            addPrompt.Text = "How many would you like to add?";
            this.Controls.Add(addPrompt);

            getCountBox = new NumericUpDown();
            getCountBox.Location = new Point(170, 5);
            getCountBox.Width = 50;
            this.Controls.Add(getCountBox);

            Button acceptCount = new Button();
            acceptCount.Text = "OK";
            acceptCount.Location = new Point(0, 30);
            acceptCount.Width = 233;
            acceptCount.Click += new EventHandler(okClick);
            this.Controls.Add(acceptCount);

        }

        public int getCount()
        {
            return count;
        }

        //function for clicking ok
        private void okClick(object sender, EventArgs e)
        {
            count = (int)getCountBox.Value;
            if(count > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                //display error, count must be greater than 0
            }
        }
    }
}
