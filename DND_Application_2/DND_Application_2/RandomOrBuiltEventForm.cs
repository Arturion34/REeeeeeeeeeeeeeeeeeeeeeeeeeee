using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace DND_Application_2
{
    public partial class RandomOrBuiltEventForm : Form
    {
        //Control Definitions
        public RadioButton RandomSelect;
        public RadioButton BuildSelect;
        public Panel EncounterParametersBuild;
        public Panel EncounterParametersRandom;
        string connectionString = ConfigurationManager.ConnectionStrings["DND_Application_2.Properties.Settings._5e_DataConnectionString"].ConnectionString;
        ListBox potentialMonsters;
        ListBox selectedMonsters;
        ComboBox BuildTerrainList;
        TextBox BuildExperienceCounter;
        int BuildEncounterExperience = 0;
        PartyClass campaignParty;
        int partyEasyThreshold = 0;
        int partyMediumThreshold = 0;
        int partyHardThreshold = 0;
        int partyDeadlyThreshold = 0;
        RadioButton EasyBuildSelected;
        RadioButton MediumBuildSelected;
        RadioButton HardBuildSelected;
        RadioButton DeadlyBuildSelected;

        public RandomOrBuiltEventForm()
        {
            InitializeComponent();

            //Panel to control Random vs Built option
            Panel EncounterTypePanel = new Panel();
            EncounterTypePanel.Width = 500;
            EncounterTypePanel.Height = 25;
            EncounterTypePanel.Location = new Point(0, 0);
            EncounterTypePanel.BorderStyle = BorderStyle.FixedSingle;

            RandomSelect = new RadioButton();
            RandomSelect.Location = new Point(150, 0);
            RandomSelect.Text = "Random";
            RandomSelect.CheckedChanged += new EventHandler(RandomBuiltSelectChanged);
            EncounterTypePanel.Controls.Add(RandomSelect);

            BuildSelect = new RadioButton();
            BuildSelect.Location = new Point(300, 0);
            BuildSelect.Text = "Build";
            BuildSelect.CheckedChanged += new EventHandler(RandomBuiltSelectChanged);
            EncounterTypePanel.Controls.Add(BuildSelect);

            this.Controls.Add(EncounterTypePanel);

            //Panel for build parameters, these depend on build vs. Random
            EncounterParametersBuild = new Panel();
            EncounterParametersBuild.Height = 375;
            EncounterParametersBuild.Width = 500;
            EncounterParametersBuild.Location = new Point(0, 25);
            EncounterParametersBuild.Visible = false;
            this.Controls.Add(EncounterParametersBuild);

            //Build Dropdown list of Terrain Types
            BuildTerrainList = new ComboBox();
            BuildTerrainList.DropDownStyle = ComboBoxStyle.DropDownList;
            BuildTerrainList.Location = new Point(75, 50);
            BuildTerrainList.SelectionChangeCommitted += new EventHandler(TerrainSelectedChange);

            //Populate dropdown list
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adaptor = new SqlDataAdapter("SELECT * FRom Terrain", con))
                {
                    DataTable terrainTable = new DataTable();
                    adaptor.Fill(terrainTable);

                    BuildTerrainList.DisplayMember = "Name";
                    BuildTerrainList.ValueMember = "TerrainID";
                    BuildTerrainList.DataSource = terrainTable;
                    con.Close();
                }

            }

            //Label for dropdown
            Label BuildTerrainLabel = new Label();
            BuildTerrainLabel.Text = "Terrain";
            BuildTerrainLabel.Location = new Point(10, 55);
            BuildTerrainLabel.Width = 50;

            EncounterParametersBuild.Controls.Add(BuildTerrainLabel);

            //Populate possible monsterList
            potentialMonsters = new ListBox();
            potentialMonsters.Location = new Point(10, 80);
            potentialMonsters.Width = BuildTerrainLabel.Width + BuildTerrainList.Width + 15;
            potentialMonsters.Height = 300;

            populateMonsterList(1);

            EncounterParametersBuild.Controls.Add(potentialMonsters);

            //Selected Monsters Listbox
            selectedMonsters = new ListBox();
            selectedMonsters.Width = BuildTerrainLabel.Width + BuildTerrainList.Width + 15;
            selectedMonsters.Location = new Point(500 - 25 - selectedMonsters.Width, 80);
            selectedMonsters.ValueMember = "MonsterID";
            selectedMonsters.DisplayMember = "MonsterName";
            selectedMonsters.Height = 300;

            EncounterParametersBuild.Controls.Add(selectedMonsters);

            //Buttons to add/remove monsters
            Button addSelectedMonster = new Button();
            addSelectedMonster.Text = "+";
            addSelectedMonster.Height = 40;
            addSelectedMonster.Width = 40;
            addSelectedMonster.Font = new Font(addSelectedMonster.Font.FontFamily, 30);
            addSelectedMonster.TextAlign = ContentAlignment.MiddleCenter;
            addSelectedMonster.FlatAppearance.BorderSize = 0;
            addSelectedMonster.FlatStyle = FlatStyle.Flat;
            addSelectedMonster.Location = new Point(222, 150);
            addSelectedMonster.UseCompatibleTextRendering = true;
            addSelectedMonster.Click += new EventHandler(addMonsterToSelectedList);

            EncounterParametersBuild.Controls.Add(addSelectedMonster);

            Button removeSelectedMonster = new Button();
            removeSelectedMonster.Text = "-";
            removeSelectedMonster.Height = 40;
            removeSelectedMonster.Width = 40;
            removeSelectedMonster.Font = new Font(removeSelectedMonster.Font.FontFamily, 40);
            removeSelectedMonster.TextAlign = ContentAlignment.MiddleCenter;
            removeSelectedMonster.FlatAppearance.BorderSize = 0;
            removeSelectedMonster.FlatStyle = FlatStyle.Flat;
            removeSelectedMonster.Location = new Point(222, 190);
            removeSelectedMonster.UseCompatibleTextRendering = true;
            removeSelectedMonster.Click += new EventHandler(removeMonsterFromSelectedList);

            EncounterParametersBuild.Controls.Add(removeSelectedMonster);

            //Experience Counter
            BuildExperienceCounter = new TextBox();
            BuildExperienceCounter.Enabled = false;
            BuildExperienceCounter.Text = "" + BuildEncounterExperience;
            BuildExperienceCounter.Location = new Point(selectedMonsters.Location.X + selectedMonsters.Width - BuildExperienceCounter.Width, BuildTerrainList.Location.Y);

            Label BuildExperienceLabel = new Label();
            BuildExperienceLabel.Text = "Experience";
            BuildExperienceLabel.Location = new Point(selectedMonsters.Location.X, BuildTerrainLabel.Location.Y);

            EncounterParametersBuild.Controls.Add(BuildExperienceCounter);
            EncounterParametersBuild.Controls.Add(BuildExperienceLabel);

            //RadioButtons to signify current difficulty
            EasyBuildSelected = new RadioButton();
            EasyBuildSelected.Text = "Easy";
            EasyBuildSelected.Location = new Point(25, 5);
            EasyBuildSelected.Enabled = false;

            MediumBuildSelected = new RadioButton();
            MediumBuildSelected.Text = "Medium";
            MediumBuildSelected.Location = new Point(150, 5);
            MediumBuildSelected.Enabled = false;

            HardBuildSelected = new RadioButton();
            HardBuildSelected.Text = "Hard";
            HardBuildSelected.Location = new Point(275, 5);
            HardBuildSelected.Enabled = false;

            DeadlyBuildSelected = new RadioButton();
            DeadlyBuildSelected.Text = "Deadly";
            DeadlyBuildSelected.Location = new Point(400, 5);
            DeadlyBuildSelected.Enabled = false;

            EncounterParametersBuild.Controls.Add(EasyBuildSelected);
            EncounterParametersBuild.Controls.Add(MediumBuildSelected);
            EncounterParametersBuild.Controls.Add(HardBuildSelected);
            EncounterParametersBuild.Controls.Add(DeadlyBuildSelected);
            EncounterParametersBuild.Controls.Add(BuildTerrainList);

            //Panel for Random parameters, these depend on build vs. Random
            EncounterParametersRandom = new Panel();
            EncounterParametersRandom.Height = 375;
            EncounterParametersRandom.Width = 500;
            EncounterParametersRandom.Location = new Point(0, 25);

            ComboBox TerrainDropDownList = new ComboBox();
            TerrainDropDownList.DropDownStyle = ComboBoxStyle.DropDownList;
            TerrainDropDownList.Location = new Point(250, 100);

            //Populate dropdown list
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using(SqlDataAdapter adaptor = new SqlDataAdapter("SELECT * FRom Terrain", con))
                {
                    DataTable terrainTable = new DataTable();
                    adaptor.Fill(terrainTable);

                    TerrainDropDownList.DisplayMember = "Name";
                    TerrainDropDownList.ValueMember = "TerrainID";
                    TerrainDropDownList.DataSource = terrainTable;
                    con.Close();
                }
                
            }

            //Radiobuttons for randomGeneratedDifficultySelection
            RadioButton EasySelected = new RadioButton();
            EasySelected.Text = "Easy";
            EasySelected.Location = new Point(25, 5);

            RadioButton MediumSelected = new RadioButton();
            MediumSelected.Text = "Medium";
            MediumSelected.Location = new Point(150, 5);

            RadioButton HardSelected = new RadioButton();
            HardSelected.Text = "Hard";
            HardSelected.Location = new Point(275, 5);

            RadioButton DeadlySelected = new RadioButton();
            DeadlySelected.Text = "Deadly";
            DeadlySelected.Location = new Point(400, 5);

            EasySelected.Checked = true;

            EncounterParametersRandom.Controls.Add(EasySelected);
            EncounterParametersRandom.Controls.Add(MediumSelected);
            EncounterParametersRandom.Controls.Add(HardSelected);
            EncounterParametersRandom.Controls.Add(DeadlySelected);

            //set the default random/build value
            RandomSelect.Checked = true;

            Label TerrainDropDownLabel = new Label();
            TerrainDropDownLabel.Text = "Encounter Terrain";
            TerrainDropDownLabel.Location = new Point(150, 105);
            EncounterParametersRandom.Controls.Add(TerrainDropDownLabel);
            EncounterParametersRandom.Controls.Add(TerrainDropDownList);
            this.Controls.Add(EncounterParametersRandom);

            //Button to start Encounter
            Button BeginEncounter = new Button();
            BeginEncounter.Text = "GO";
            BeginEncounter.Height = 100;
            BeginEncounter.Width = 500;
            BeginEncounter.Location = new Point(0, 400);

                //RadioButtons to 
            this.Controls.Add(BeginEncounter);
        }
        
        //function to add monstername to selected Monster list
        private void addMonsterToSelectedList(object sender, EventArgs e)
        {
            int xpToAdd = 0;
            string monsterName = potentialMonsters.GetItemText(potentialMonsters.SelectedItem);
            string Query = "SELECT Monster.Experience from Monster WHERE Name Like '" + monsterName + "'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand getEXPSQL = new SqlCommand(Query, con);
                getEXPSQL.CommandType = CommandType.Text;
                getEXPSQL.CommandText = Query;
                xpToAdd = (int)getEXPSQL.ExecuteScalar();

                con.Close();
            }
            BuildEncounterExperience += xpToAdd;
            selectedMonsters.Items.Add(monsterName);
            calculateBuildEncounterExperience();
        }

        //function to remove monster name from selected monster list
        private void removeMonsterFromSelectedList(object sender, EventArgs e)
        {
            if(selectedMonsters.SelectedItems.Count > 0)
            {
                int xpToRemove = 0;
                string monsterName = selectedMonsters.GetItemText(selectedMonsters.SelectedItem);
                string Query = "SELECT Monster.Experience from Monster WHERE Name Like '" + monsterName + "'";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand getEXPSQL = new SqlCommand(Query, con);
                    getEXPSQL.CommandType = CommandType.Text;
                    getEXPSQL.CommandText = Query;
                    xpToRemove = (int)getEXPSQL.ExecuteScalar();

                    con.Close();
                }
                selectedMonsters.Items.RemoveAt(selectedMonsters.SelectedIndex);
                BuildEncounterExperience -= xpToRemove;
                calculateBuildEncounterExperience();
            }
        }

        //Function to change panel based on random/build mode for encounter
        private void RandomBuiltSelectChanged (object sender, EventArgs e)
        {
            if (RandomSelect.Checked)
            {
                EncounterParametersRandom.Visible = true;
                EncounterParametersBuild.Visible = false;
            }
            else
            {
                EncounterParametersBuild.Visible = true;
                EncounterParametersRandom.Visible = false;
            }
        }

        //function to call when change in terrain selection in build mode
        private void TerrainSelectedChange(object sender, EventArgs e)
        {
            populateMonsterList((int)BuildTerrainList.SelectedValue);
        }

        //function for when build terrain list selected value changes
        private void populateMonsterList(int TerrainID)
        {
            string monsterQuery;
            if (TerrainID == 1)
            {
                monsterQuery = " SELECT * from Monster";
            }
            else
            {
                monsterQuery = "SELECT Monster.Name FROM Terrain_xref INNER JOIN Monster ON Terrain_xref.MonsterID = Monster.MonsterID WHERE Terrain_xref.TerrainID = " + TerrainID;
            }
             
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adaptor = new SqlDataAdapter(monsterQuery, con))
                {
                    DataTable potentialMonstersTable = new DataTable();
                    adaptor.Fill(potentialMonstersTable);

                    potentialMonsters.DisplayMember = "Name";
                    potentialMonsters.ValueMember = "MonsterID";
                    potentialMonsters.DataSource = potentialMonstersTable;
                    con.Close();
                }

            }
        }

        private void calculatePartyXPDifficultyLevels(PartyClass selectedParty)
        {
            partyEasyThreshold = 0;
            partyMediumThreshold = 0;
            partyHardThreshold = 0;
            partyDeadlyThreshold = 0;

            foreach (PlayerClass currentPlayer in selectedParty.getPartyMembers())
            {
                string Query = "SELECT * from ExperienceThresholds WHERE Id_Level = '" + currentPlayer.getPlayerLevel() + "'";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter adaptor = new SqlDataAdapter(Query, con))
                    {
                        DataTable experienceTable = new DataTable();
                        adaptor.Fill(experienceTable);

                        foreach(DataRow ExperienceRow in experienceTable.Rows)
                        {
                            partyEasyThreshold += (int)ExperienceRow["Easy"];
                            partyMediumThreshold += (int)ExperienceRow["Medium"];
                            partyHardThreshold += (int)ExperienceRow["Hard"];
                            partyDeadlyThreshold += (int)ExperienceRow["Deadly"];
                        }

                        con.Close();
                    }
                }
            }
        }

        //function to apply experience multiplier and set value in textbox
        private void calculateBuildEncounterExperience()
        {
            calculatePartyXPDifficultyLevels(campaignParty);
            int actualExperience = 0;
            int numberofMonsters = selectedMonsters.Items.Count;
            if(numberofMonsters <= 1)
            {
                actualExperience = BuildEncounterExperience;
            }else if(numberofMonsters == 2)
            {
                actualExperience = (int)((double)BuildEncounterExperience * 1.5);
            }else if(numberofMonsters >= 3 && numberofMonsters <= 6)
            {
                actualExperience = BuildEncounterExperience * 2;
            }else if(numberofMonsters >= 7 && numberofMonsters <= 10)
            {
                actualExperience = (int)((double)BuildEncounterExperience * 2.5);
            }else if(numberofMonsters >= 11 && numberofMonsters <= 14)
            {
                actualExperience = BuildEncounterExperience * 3;
            }
            else
            {
                actualExperience = BuildEncounterExperience * 4;
            }

            //check appropriate radiobutton difficulty
            if(actualExperience >= partyEasyThreshold && actualExperience < partyMediumThreshold)
            {
                EasyBuildSelected.Select();
            }else if(actualExperience >= partyEasyThreshold && actualExperience < partyHardThreshold)
            {
                MediumBuildSelected.Select();
            }else if(actualExperience >= partyHardThreshold && actualExperience < partyDeadlyThreshold)
            {
                HardBuildSelected.Select();
            }
            else if(actualExperience >= partyDeadlyThreshold)
            {
                DeadlyBuildSelected.Select();
            }
            else
            {
                EasyBuildSelected.Checked = false;
                MediumBuildSelected.Checked = false;
                HardBuildSelected.Checked = false;
                DeadlyBuildSelected.Checked = false;
            }

            BuildExperienceCounter.Text = "" + actualExperience;
        }

        public void setCampaignParty(PartyClass toSet)
        {
            campaignParty = toSet;
        }

        
    }
}
