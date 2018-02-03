using DND_Application_2.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DND_Application_2
{
    public partial class InitiativeCreation : Form
    {
        private List<Object> initiativeOrder;
        private List<Object> monsterList;
        private List<PlayerClass> playerList;
        private List<ComboBox> dropDownsList;

        public InitiativeCreation()
        {
            InitializeComponent();

        }

        public InitiativeCreation(List<Object> monsterListToSet, List<PlayerClass> playerListtoSet)
        {
            this.AutoScroll = true;
            this.Width = 300;
            this.Height = 500;
            monsterList = monsterListToSet;
            playerList = playerListtoSet;
            dropDownsList = new List<ComboBox>();
            initiativeOrder = new List<Object>();

            Label MonsterTextLabel = new Label();
            MonsterTextLabel.Text = "Monsters";
            MonsterTextLabel.Location = new Point(0, 5);
            MonsterTextLabel.Width = this.Width;
            MonsterTextLabel.Height = 15;
            MonsterTextLabel.TextAlign = ContentAlignment.BottomCenter;
            MonsterTextLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(MonsterTextLabel);

            //strictly for underlining purposes
            Label underlineLabel = new Label();
            underlineLabel.Text = "------------------------------------------------------";
            underlineLabel.Location = new Point(0, 20);
            underlineLabel.Width = this.Width;
            underlineLabel.Height = 10;
            underlineLabel.TextAlign = ContentAlignment.TopLeft;
            underlineLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(underlineLabel);

            int baseLabelY = 45;
            int diffLabelY = 25;
            foreach (Object toDetermine in monsterList)
            {
                //create label for each monster
                Label monsterLabel = new Label();
                monsterLabel.Location = new Point(25, baseLabelY);
                monsterLabel.Height = 20;
                if(toDetermine.GetType() == typeof(Monster))
                {
                    monsterLabel.Text = ((Monster)toDetermine).getName();
                }else if(toDetermine.GetType() == typeof(MonsterGroup))
                {
                    monsterLabel.Text = ((MonsterGroup)toDetermine).ToString();
                }
                
                monsterLabel.TextAlign = ContentAlignment.MiddleCenter;
                this.Controls.Add(monsterLabel);

                //create dropdown;
                ComboBox monsterInitiativeDropdown = new ComboBox();
                monsterInitiativeDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
                monsterInitiativeDropdown.Location = new Point(125, baseLabelY);
                monsterInitiativeDropdown.Width = 40;
                monsterInitiativeDropdown.Height = 10;

                //populate dropdown with numbers
                for(int i = 1; i <= monsterList.Count + playerList.Count; i++)
                {
                    monsterInitiativeDropdown.Items.Add(i);
                }
                dropDownsList.Add(monsterInitiativeDropdown);
                this.Controls.Add(monsterInitiativeDropdown);

                baseLabelY += diffLabelY;
            }

            Label PlayerTextLabel = new Label();
            PlayerTextLabel.Text = "Players";
            PlayerTextLabel.Location = new Point(0, baseLabelY);
            PlayerTextLabel.Width = this.Width;
            PlayerTextLabel.Height = 15;
            PlayerTextLabel.TextAlign = ContentAlignment.BottomCenter;
            PlayerTextLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(PlayerTextLabel);

            baseLabelY += 15;
            //strictly for underlining purposes
            Label underlinePlayerLabel = new Label();
            underlinePlayerLabel.Text = "------------------------------------------------------";
            underlinePlayerLabel.Location = new Point(0, baseLabelY);
            underlinePlayerLabel.Width = this.Width;
            underlinePlayerLabel.Height = 10;
            underlinePlayerLabel.TextAlign = ContentAlignment.TopLeft;
            underlinePlayerLabel.BorderStyle = BorderStyle.None;
            this.Controls.Add(underlinePlayerLabel);

            baseLabelY += diffLabelY;

            foreach (PlayerClass currPlayer in playerList)
            {
                //create label for each monster
                Label playerLabel = new Label();
                playerLabel.Location = new Point(25, baseLabelY);
                playerLabel.Height = 20;
                playerLabel.Text = currPlayer.getCharacterName();
                playerLabel.TextAlign = ContentAlignment.MiddleCenter;
                this.Controls.Add(playerLabel);

                //create dropdown;
                ComboBox playerInitiativeDropDown = new ComboBox();
                playerInitiativeDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
                playerInitiativeDropDown.Location = new Point(125, baseLabelY);
                playerInitiativeDropDown.Width = 40;
                playerInitiativeDropDown.Height = 10;

                //populate dropdown with numbers
                for (int i = 1; i <= monsterList.Count + playerList.Count; i++)
                {
                    playerInitiativeDropDown.Items.Add(i);
                }
                dropDownsList.Add(playerInitiativeDropDown);
                this.Controls.Add(playerInitiativeDropDown);

                baseLabelY += diffLabelY;
            }

            Button beginEncounterButton = new Button();
            beginEncounterButton.Text = "GO";
            beginEncounterButton.Width = this.Width;
            beginEncounterButton.Location = new Point(0, baseLabelY + diffLabelY);
            if(baseLabelY + diffLabelY + beginEncounterButton.Height > this.Height)
            {
                this.Height = baseLabelY + diffLabelY + beginEncounterButton.Height;
            }
            beginEncounterButton.Click += new EventHandler(beginEncounter);
            this.Controls.Add(beginEncounterButton);
        }

        //setter function for player
        private void setPlayerList(List<PlayerClass> toSet)
        {
            playerList = toSet;
        }

        //setter function for monsterList
        private void setMonsterList(List<Object> toSet)
        {
            monsterList = toSet;
        }

        //function to begin encounter after initative is set
        private void beginEncounter(object sender, EventArgs e)
        {
            //create ordered list of monsters and players
            initiativeOrder = new List<object>();
            int remainingMonstersAndPlayers = monsterList.Count + playerList.Count;
            int currentOrderFinder = 1;
            while (remainingMonstersAndPlayers > 0)
            {
                
                int currentOrderIndex = 0;

                int currentIndex = 0;
                foreach(ComboBox item in dropDownsList)
                {
                    
                    if(currentOrderFinder == (int)item.SelectedItem)
                    {
                        currentOrderIndex = currentIndex;
                    }
                    currentIndex++;
                }

                if(currentOrderIndex < monsterList.Count)
                {
                    initiativeOrder.Add(monsterList[currentOrderIndex]);
                }
                else
                {
                    initiativeOrder.Add(playerList[currentOrderIndex - monsterList.Count]);
                }

                remainingMonstersAndPlayers--;
                currentOrderFinder++;

            }

            EncounterManager beginEncounter = new EncounterManager(initiativeOrder);
            beginEncounter.Visible = true;
        }

    }
}
