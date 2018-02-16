using DND_Application_2.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DND_Application_2
{
    public partial class EncounterManager : Form
    {
        List<Point[]> initiativeBarShapes;
        List<Point> CenterOfArrows = new List<Point>();
        List<Label> arrowLabels = new List<Label>();
        List<Object> initiativeOrder;
        List<Object> encounterMonsters;
        List<Button> monsterListButtons;
        Panel monsterListPanel;
        Button nextTurn;
        Graphics pane;
        Boolean originalArrowsDrawn = false;
        int currentInitiative = 0;
        Panel currentSelected;
        List<Object> monsterListForButtons;
        Monster toFight;
        MonsterGroup toFightGroup;

        int numberPlayers = 2;
        int numberMonsters;
        int numberMonstersAndPlayers;
        int baseX = 200;
        int baseY = 20;
        int XDifference;
        int YDifference = 80;

        public EncounterManager()
        {

        }

        public EncounterManager(List<Object> encounterList)
        {
            originalArrowsDrawn = false;
            initiativeOrder = encounterList;
            monsterListButtons = new List<Button>();

            //define nextTurn Button
            nextTurn = new Button();
            nextTurn.Height = YDifference;
            nextTurn.Width = baseX - 20;
            nextTurn.Text = "Next";
            nextTurn.Click += new EventHandler(nextInitiative);
            this.Controls.Add(nextTurn);

            //set num monsters to 0
            numberMonsters = 0;
            monsterListForButtons = new List<Object>();

            //add up all members in encounter
            numberMonstersAndPlayers = encounterList.Count;

            //count number of Monsters/MonsterGroups
            foreach (Object item in encounterList)
            {
                if(item.GetType() == typeof(Monster) || item.GetType() == typeof(MonsterGroup))
                {
                    numberMonsters += 1;
                    monsterListForButtons.Add(item);
                }
            }

            //Format window to be full screen
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;

            //calculate width of triangles
            XDifference = ((2 * System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - (4 * baseX)) / ((2 * numberMonstersAndPlayers) + 1);

            InitializeComponent();

            //Draw Any necessary shapes
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.EncounterManager_Paint);

            monsterListPanel = new Panel();
            monsterListPanel.Width = 300;
            monsterListPanel.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 140;
            monsterListPanel.Location = new Point(20, 120);
            monsterListPanel.BorderStyle = BorderStyle.FixedSingle;

            //generate variable button size based on number of monsters and players
            int heightOfMonsterButton = monsterListPanel.Height / numberMonsters;
            for(int i = 0; i < numberMonsters; i++)
            {
                Button monsterLabelButton = new Button();
                monsterLabelButton.Height = heightOfMonsterButton;
                monsterLabelButton.Width = monsterListPanel.Width;
                monsterLabelButton.Location = new Point(0, heightOfMonsterButton * i);
                monsterLabelButton.Text = (encounterList[i]).ToString();
                monsterListButtons.Add(monsterLabelButton);
                monsterLabelButton.Click += new EventHandler(selectMonsterForInformationDisplay);
                monsterListPanel.Controls.Add(monsterLabelButton);
            }

            currentInitiative = 0;
            this.Controls.Add(monsterListPanel);

            //define the currentSelected Panel
            currentSelected = new Panel();
            currentSelected.Location = new Point(340, 120);
            currentSelected.BorderStyle = BorderStyle.FixedSingle;
            currentSelected.Height = Screen.PrimaryScreen.Bounds.Height - 140;
            currentSelected.Width = Screen.PrimaryScreen.Bounds.Width - currentSelected.Location.X - monsterListPanel.Location.X;

            this.Controls.Add(currentSelected);
        }

        //paint method. Paints most of the initiative bars
        private void EncounterManager_Paint(object sender, PaintEventArgs e)
        {
            arrowLabels = new List<Label>();
            initiativeBarShapes = new List<Point[]>();
            

            pane = e.Graphics;
            Pen BlackPen = new Pen(Color.Black, 4);
            SolidBrush Empty = new SolidBrush(Color.Empty);
            SolidBrush Yellow = new SolidBrush(Color.Gold);
            int currX = baseX;
            if (originalArrowsDrawn == false)
            {
                // middle panes
                
                    for (int i = 0; i < initiativeOrder.Count; i++)
                    {
                        Point[] midPoints =
                        {
                        new Point(currX, baseY),
                        new Point(currX + (XDifference/2), (baseY + YDifference/2)),
                        new Point(currX, baseY + YDifference),
                        new Point(currX + XDifference, baseY + YDifference),
                        new Point(currX + XDifference + XDifference/2, baseY + YDifference/2),
                        new Point(currX + XDifference, baseY)
                    };

                        //create Label for arrow
                        Label arrowLabel = new Label();
                        Object toLabel = initiativeOrder[i];
                        string labelText = "";
                        if (toLabel.GetType() == typeof(Monster))
                        {
                            labelText = ((Monster)toLabel).ToString();
                        }
                        else if (toLabel.GetType() == typeof(MonsterGroup))
                        {
                            labelText = ((MonsterGroup)toLabel).ToString();
                        }
                        else if (toLabel.GetType() == typeof(PlayerClass))
                        {
                            labelText = ((PlayerClass)toLabel).ToString();
                        }

                        arrowLabel.Text = labelText;

                        //define label size by string
                        using (Graphics g = CreateGraphics())
                        {
                            SizeF size = g.MeasureString(labelText.Replace(" ", "_"), arrowLabel.Font, 495);
                            arrowLabel.Height = (int)Math.Ceiling(size.Height);
                            arrowLabel.Width = (int)Math.Ceiling(size.Width);
                        }
                        arrowLabel.Location = new Point(currX + (XDifference / 4) * 3 - (arrowLabel.Width / 2), baseY + (YDifference / 2) - (arrowLabel.Height / 2));
                        arrowLabels.Add(arrowLabel);

                        this.Controls.Add(arrowLabel);

                        initiativeBarShapes.Add(midPoints);
                        pane.DrawPolygon(BlackPen, midPoints);
                        currX += XDifference;
                    }
            }
            
            //clear all initiative labels and arrows
            for(int i = 0; i < arrowLabels.Count; i++)
            {
                ((Label)(arrowLabels[i])).BackColor = Color.Empty;
                pane.FillPolygon(Empty, initiativeBarShapes[i]);
                arrowLabels[i].Refresh();
            }

            //((Label)(arrowLabels[currentInitiative])).BackColor = Color.Gold;
            ((Label)(arrowLabels[currentInitiative])).Refresh();
            pane.FillPolygon(Yellow, initiativeBarShapes[currentInitiative]);

            //add location for nextTurn Button
            nextTurn.Location = new Point(currX + (XDifference/2) + 10, baseY);
        }

        //function to move yellow portion of initiative bar
        private void nextInitiative(object sender, EventArgs e)
        {
            //increment initative, and draw yellow, erase previous yellow
            if(currentInitiative == numberMonstersAndPlayers - 1)
            {
                currentInitiative = 0;
            }
            else
            {
                currentInitiative += 1;
            }

            this.Refresh();
        }

        //setter for initative order
        private void setInitiativeOrderList(List<Object> toSet)
        {
            initiativeOrder = toSet;
        }

        //function for monster selection, call on monster getting its turn, 
        private void selectMonsterForInformationDisplay(object sender, EventArgs e)
        {
            //remove all controls from the panel
            currentSelected.Controls.Clear();

            //Define BasePanels
            Panel baseMonsterInfo = new Panel();
            Panel baseMonsterStats = new Panel();
            Panel baseMonsterSpeeds = new Panel();

            //define the selected Monster or Monster Group object
            Object currentlySelected = null;
            if(sender.GetType() == typeof(Button))
            {
                int index = monsterListButtons.FindIndex(i => i == ((Button)sender));
                currentlySelected = monsterListForButtons[index];
            }

            //check the type of the currently selected monster
            if(currentlySelected.GetType() == typeof(Monster))
            {
                //redefine as the actual type
                toFight = (Monster)currentlySelected;

                //Define Base Monster Information. Name, Type, Size, Alignment, Languages
                baseMonsterInfo.Width = 200;
                baseMonsterInfo.Height = 400;
                baseMonsterInfo.Location = new Point(20, 20);
                baseMonsterInfo.BorderStyle = BorderStyle.FixedSingle;

                //Define Monster Name
                Label monsterNameLabel = new Label();
                monsterNameLabel.Text = "Name :";
                monsterNameLabel.Location = new Point(0, 0);
                monsterNameLabel.BorderStyle = BorderStyle.FixedSingle;

                Label monsterName = new Label();
                monsterName.Text = toFight.getName();
                monsterName.Location = new Point(monsterNameLabel.Location.X + monsterNameLabel.Width, monsterNameLabel.Location.Y);
                monsterName.BorderStyle = BorderStyle.FixedSingle;

                baseMonsterInfo.Controls.Add(monsterNameLabel);
                baseMonsterInfo.Controls.Add(monsterName);

                //Define MonsterType
                Label monsterTypeLabel = new Label();
                monsterTypeLabel.Text = "Type :";
                monsterTypeLabel.Location = new Point(0, monsterNameLabel.Height);
                monsterTypeLabel.BorderStyle = BorderStyle.FixedSingle;

                Label monsterType = new Label();
                monsterType.Text = toFight.getMonsterType();
                monsterType.Location = new Point(monsterTypeLabel.Location.X + monsterTypeLabel.Width, monsterTypeLabel.Location.Y);
                monsterType.BorderStyle = BorderStyle.FixedSingle;

                baseMonsterInfo.Controls.Add(monsterTypeLabel);
                baseMonsterInfo.Controls.Add(monsterType);

                //Define MonsterSize
                Label monsterSizeLabel = new Label();
                monsterSizeLabel.Text = "Size :";
                monsterSizeLabel.Location = new Point(0, monsterNameLabel.Height * 2);
                monsterSizeLabel.BorderStyle = BorderStyle.FixedSingle;

                Label monsterSize = new Label();
                monsterSize.Text = toFight.getSize();
                monsterSize.Location = new Point(monsterSizeLabel.Location.X + monsterSizeLabel.Width, monsterSizeLabel.Location.Y);
                monsterSize.BorderStyle = BorderStyle.FixedSingle;

                baseMonsterInfo.Controls.Add(monsterSizeLabel);
                baseMonsterInfo.Controls.Add(monsterSize);

                //DefineMonsterLangauges
                Label monsterLanguagesLabel = new Label();
                monsterLanguagesLabel.Text = "Languages :";
                monsterLanguagesLabel.Height = 50;
                monsterLanguagesLabel.Location = new Point(0, monsterNameLabel.Height * 3);
                monsterLanguagesLabel.BorderStyle = BorderStyle.FixedSingle;

                TextBox monsterLanguages = new TextBox();
                string languages = "";
                foreach(string language in toFight.getLanguages())
                {
                    languages += language + "\r\n";
                }
                monsterLanguages.ReadOnly = true;
                monsterLanguages.ScrollBars = ScrollBars.Vertical;
                monsterLanguages.Text = languages;
                monsterLanguages.Multiline = true;
                monsterLanguages.Height = 50;
                monsterLanguages.Location = new Point(monsterLanguages.Location.X + monsterLanguagesLabel.Width, monsterLanguagesLabel.Location.Y);
                monsterLanguages.BorderStyle = BorderStyle.FixedSingle;

                baseMonsterInfo.Controls.Add(monsterLanguagesLabel);
                baseMonsterInfo.Controls.Add(monsterLanguages);

                //Define MonsterAlignment
                Label monsterAlignmentLabel = new Label();
                monsterAlignmentLabel.Text = "Alignment: ";
                monsterAlignmentLabel.Location = new Point(0, monsterNameLabel.Height * 3 + monsterLanguages.Height);
                monsterAlignmentLabel.BorderStyle = BorderStyle.FixedSingle;

                Label monsterAlignment = new Label();
                monsterAlignment.Text = toFight.getAlignment();
                monsterAlignment.Location = new Point(monsterAlignmentLabel.Location.X + monsterAlignmentLabel.Width, monsterAlignmentLabel.Location.Y);
                monsterAlignment.BorderStyle = BorderStyle.FixedSingle;

                baseMonsterInfo.Controls.Add(monsterAlignment);
                baseMonsterInfo.Controls.Add(monsterAlignmentLabel);

                //add baseMonsterInfo to panel
                baseMonsterInfo.BorderStyle = BorderStyle.FixedSingle;
                baseMonsterInfo.Height = monsterAlignmentLabel.Height + monsterLanguagesLabel.Height + monsterNameLabel.Height + monsterType.Height + monsterAlignment.Height;
                currentSelected.Controls.Add(baseMonsterInfo);

                //define Monster stats and modifiers
                baseMonsterStats.Location = new Point(baseMonsterInfo.Location.X, baseMonsterInfo.Location.Y + baseMonsterInfo.Height + 20);
                baseMonsterStats.BorderStyle = BorderStyle.FixedSingle;
                int labelWidth = baseMonsterInfo.Width / 3;

                //strength information
                Label strengthLabel = new Label();
                strengthLabel.Text = "STR :";
                strengthLabel.Location = new Point(0, 0);
                strengthLabel.Width = labelWidth;

                Label strengthstat = new Label();
                strengthstat.Text = (toFight.getStrength()).ToString();
                strengthstat.Location = new Point(strengthLabel.Location.X + strengthLabel.Width, strengthLabel.Location.Y);
                strengthstat.Width = labelWidth;

                Label strengthModifier = new Label();
                strengthModifier.Text = ((toFight.getStrength() - 10) / 2).ToString();
                strengthModifier.Location = new Point(strengthstat.Location.X + strengthstat.Width, strengthLabel.Location.Y);
                strengthModifier.Width = labelWidth;

                baseMonsterStats.Controls.Add(strengthLabel);
                baseMonsterStats.Controls.Add(strengthstat);
                baseMonsterStats.Controls.Add(strengthModifier);

                //Dexterity
                Label dexterityLabel = new Label();
                dexterityLabel.Text = "DEX :";
                dexterityLabel.Location = new Point(0, strengthLabel.Height);
                dexterityLabel.Width = labelWidth;

                Label dexterityStat = new Label();
                dexterityStat.Text = ((toFight.getDexterity()).ToString());
                dexterityStat.Location = new Point(dexterityLabel.Location.X + dexterityLabel.Width, dexterityLabel.Location.Y);
                dexterityStat.Width = labelWidth;

                Label dexterityModifier = new Label();
                dexterityModifier.Text = ((toFight.getDexterity() - 10) / 2).ToString();
                dexterityModifier.Location = new Point(dexterityStat.Location.X + dexterityStat.Width, dexterityLabel.Location.Y);
                dexterityModifier.Width = labelWidth;

                baseMonsterStats.Controls.Add(dexterityLabel);
                baseMonsterStats.Controls.Add(dexterityStat);
                baseMonsterStats.Controls.Add(dexterityModifier);

                //Constitution
                Label constitutionLabel = new Label();
                constitutionLabel.Text = "CON :";
                constitutionLabel.Location = new Point(0, dexterityLabel.Location.Y + dexterityLabel.Height);
                constitutionLabel.Width = labelWidth;

                Label constitutionStat = new Label();
                constitutionStat.Text = toFight.getConstitution().ToString();
                constitutionStat.Location = new Point(constitutionLabel.Location.X + constitutionLabel.Width, constitutionLabel.Location.Y);
                constitutionStat.Width = labelWidth;

                Label constitutionModifier = new Label();
                constitutionModifier.Text = ((toFight.getConstitution() - 10) / 2).ToString();
                constitutionModifier.Location = new Point(constitutionStat.Location.X + constitutionStat.Width, constitutionLabel.Location.Y);
                constitutionModifier.Width = labelWidth;

                baseMonsterStats.Controls.Add(constitutionLabel);
                baseMonsterStats.Controls.Add(constitutionStat);
                baseMonsterStats.Controls.Add(constitutionModifier);

                //Intelligence
                Label intelligenceLabel = new Label();
                intelligenceLabel.Text = "INT :";
                intelligenceLabel.Location = new Point(0, constitutionLabel.Location.Y + constitutionLabel.Height);
                intelligenceLabel.Width = labelWidth;

                Label intelligenceStat = new Label();
                intelligenceStat.Text = toFight.getIntelligence().ToString();
                intelligenceStat.Location = new Point(intelligenceLabel.Location.X + intelligenceLabel.Width, intelligenceLabel.Location.Y);
                intelligenceStat.Width = labelWidth;

                Label intelligenceModifier = new Label();
                intelligenceModifier.Text = ((toFight.getIntelligence() - 10) / 2).ToString();
                intelligenceModifier.Location = new Point(intelligenceStat.Location.X + intelligenceStat.Width, intelligenceLabel.Location.Y);
                intelligenceModifier.Width = labelWidth;

                baseMonsterStats.Controls.Add(intelligenceLabel);
                baseMonsterStats.Controls.Add(intelligenceStat);
                baseMonsterStats.Controls.Add(intelligenceModifier);

                //Wisdom
                Label wisdomLabel = new Label();
                wisdomLabel.Text = "WIS :";
                wisdomLabel.Location = new Point(0, intelligenceLabel.Location.Y + intelligenceLabel.Height);
                wisdomLabel.Width = labelWidth;

                Label wisdomStat = new Label();
                wisdomStat.Text = toFight.getWisdom().ToString();
                wisdomStat.Location = new Point(wisdomLabel.Location.X + wisdomLabel.Width, wisdomLabel.Location.Y);
                wisdomStat.Width = labelWidth;

                Label wisdomModifier = new Label();
                wisdomModifier.Text = ((toFight.getWisdom() - 10) / 2).ToString();
                wisdomModifier.Location = new Point(wisdomStat.Location.X + wisdomStat.Width, wisdomLabel.Location.Y);
                wisdomModifier.Width = labelWidth;

                baseMonsterStats.Controls.Add(wisdomLabel);
                baseMonsterStats.Controls.Add(wisdomStat);
                baseMonsterStats.Controls.Add(wisdomModifier);

                //Charisma
                Label charismaLabel = new Label();
                charismaLabel.Text = "CHA :";
                charismaLabel.Location = new Point(0, wisdomLabel.Location.Y + wisdomLabel.Height);
                charismaLabel.Width = labelWidth;

                Label charismaStat = new Label();
                charismaStat.Text = toFight.getCharisma().ToString();
                charismaStat.Location = new Point(charismaLabel.Location.X + charismaLabel.Width, charismaLabel.Location.Y);
                charismaStat.Width = labelWidth;

                Label charismaModifier = new Label();
                charismaModifier.Text = ((toFight.getCharisma() - 10) / 2).ToString();
                charismaModifier.Location = new Point(charismaStat.Location.X + charismaStat.Width, charismaLabel.Location.Y);
                charismaModifier.Width = labelWidth;

                baseMonsterStats.Controls.Add(charismaLabel);
                baseMonsterStats.Controls.Add(charismaStat);
                baseMonsterStats.Controls.Add(charismaModifier);

                //add monster stat box and size
                baseMonsterStats.Height = strengthLabel.Height + charismaLabel.Height + dexterityLabel.Height + constitutionLabel.Height + wisdomLabel.Height + intelligenceLabel.Height;
                baseMonsterStats.Width = strengthLabel.Width + strengthstat.Width + strengthModifier.Width;
                currentSelected.Controls.Add(baseMonsterStats);

                //speed panels
                baseMonsterSpeeds.Location = new Point(baseMonsterStats.Location.X, baseMonsterStats.Location.Y + baseMonsterStats.Height + 20);

                //List to store labels to allow for dynamic sizing
                List<Label> speedLabelList = new List<Label>();

                //Walk Speed
                if(toFight.getWalkSpeed() != null)
                {
                    Label walkLabel = new Label();
                    walkLabel.Text = "Walk :";

                    Label walkDistance = new Label();
                    walkDistance.Text = toFight.getWalkSpeed().ToString() + "ft.";

                    speedLabelList.Add(walkLabel);
                    speedLabelList.Add(walkDistance);
                }

                //Swimspeed
                if(toFight.getSwimSpeed() != null)
                {
                    Label swimLabel = new Label();
                    swimLabel.Text = "Swim :";

                    Label swimDistance = new Label();
                    swimDistance.Text = toFight.getSwimSpeed().ToString() + "ft.";

                    speedLabelList.Add(swimLabel);
                    speedLabelList.Add(swimDistance);
                }

                //burrowSpeed
                if(toFight.getBurrowSpeed() != null)
                {
                    Label burrowLabel = new Label();
                    burrowLabel.Text = "Burrow: ";

                    Label burrowDistance = new Label();
                    burrowDistance.Text = toFight.getBurrowSpeed().ToString() + "ft.";

                    speedLabelList.Add(burrowLabel);
                    speedLabelList.Add(burrowDistance);
                }

                //fly speed
                if(toFight.getFlySpeed() != null)
                {
                    Label flyLabel = new Label();
                    flyLabel.Text = "Fly: ";

                    Label flyDistance = new Label();
                    flyDistance.Text = toFight.getFlySpeed().ToString() + "ft.";

                    speedLabelList.Add(flyLabel);
                    speedLabelList.Add(flyDistance);
                }

                int baseSpeedLabelY = 0;
                //calculate size and location for speed label
                for(int i = 0; i < speedLabelList.Count; i++)
                {
                    baseSpeedLabelY = speedLabelList[i].Height;
                    if(i%2 == 0)
                    {
                        //this is the descriptor label
                        Label toAdd = speedLabelList[i];
                        toAdd.Location = new Point(0, baseSpeedLabelY * (i/2));

                        //this is the information label
                        Label toAddData = speedLabelList[i + 1];
                        toAddData.Location = new Point(toAdd.Width, baseSpeedLabelY * (i/2));

                        baseMonsterSpeeds.Controls.Add(toAdd);
                        baseMonsterSpeeds.Controls.Add(toAddData);
                    }
                }

                baseMonsterSpeeds.Height = speedLabelList[0].Height * 4;
                baseMonsterSpeeds.Width = speedLabelList[0].Width * 2;
                baseMonsterSpeeds.BorderStyle = BorderStyle.FixedSingle;

                currentSelected.Controls.Add(baseMonsterSpeeds);

                
                //Senses & skills
                Panel sensesAndSkillsPanel = new Panel();
                sensesAndSkillsPanel.Location = new Point(baseMonsterSpeeds.Location.X, baseMonsterSpeeds.Location.Y + baseMonsterSpeeds.Height + 20);
                sensesAndSkillsPanel.BorderStyle = BorderStyle.FixedSingle;
                sensesAndSkillsPanel.Width = baseMonsterStats.Width;

                Label SensesLabel = new Label();
                SensesLabel.Text = "Senses | Skills";
                SensesLabel.Location = new Point(0,0);
                SensesLabel.Width = sensesAndSkillsPanel.Width;
                SensesLabel.TextAlign = ContentAlignment.MiddleCenter;

                TextBox SensesDetails = new TextBox();
                SensesDetails.Text = toFight.getSenses().Replace(", ", "\r\n");
                SensesDetails.Height = 50;
                SensesDetails.Location = new Point(0, SensesLabel.Height);
                SensesDetails.Multiline = true;
                SensesDetails.Width = sensesAndSkillsPanel.Width;
                SensesDetails.ReadOnly = true;
                SensesDetails.ScrollBars = ScrollBars.Vertical;

                TextBox SkillsDetails = new TextBox();
                SkillsDetails.Text = toFight.getSkills();
                SkillsDetails.Height = 50;
                SkillsDetails.Location = new Point(0, SensesDetails.Location.Y + SensesDetails.Height);
                SkillsDetails.Multiline = true;
                SkillsDetails.Width = sensesAndSkillsPanel.Width;
                SkillsDetails.ReadOnly = true;
                SkillsDetails.ScrollBars = ScrollBars.Vertical;

                sensesAndSkillsPanel.Height = SensesLabel.Height + SensesDetails.Height + SkillsDetails.Height;
                sensesAndSkillsPanel.Controls.Add(SensesLabel);
                sensesAndSkillsPanel.Controls.Add(SensesDetails);
                sensesAndSkillsPanel.Controls.Add(SkillsDetails);

                currentSelected.Controls.Add(sensesAndSkillsPanel);

                //saving throws panel
                Panel monsterSavingThrows = new Panel();
                monsterSavingThrows.Location = new Point(sensesAndSkillsPanel.Location.X, sensesAndSkillsPanel.Location.Y + sensesAndSkillsPanel.Height + 20);
                monsterSavingThrows.Width = baseMonsterStats.Width;
                monsterSavingThrows.BorderStyle = BorderStyle.FixedSingle;

                Label savingThrowsLabel = new Label();
                savingThrowsLabel.Location = new Point(0, 0);
                savingThrowsLabel.Width = monsterSavingThrows.Width;
                savingThrowsLabel.Text = "Saving Throws";
                savingThrowsLabel.TextAlign = ContentAlignment.MiddleCenter;

                TextBox savingThrowInfo = new TextBox();
                savingThrowInfo.Text = toFight.getSavingThrows();
                savingThrowInfo.Height = 50;
                savingThrowInfo.Location = new Point(0, savingThrowsLabel.Height);
                savingThrowInfo.Multiline = true;
                savingThrowInfo.Width = monsterSavingThrows.Width;
                savingThrowInfo.ReadOnly = true;
                savingThrowInfo.ScrollBars = ScrollBars.Vertical;

                monsterSavingThrows.Height = savingThrowInfo.Height + savingThrowsLabel.Height;
                monsterSavingThrows.Controls.Add(savingThrowsLabel);
                monsterSavingThrows.Controls.Add(savingThrowInfo);

                currentSelected.Controls.Add(monsterSavingThrows);

                //Damage Resistances Panel
                Panel monsterResistances = new Panel();
                monsterResistances.Width = baseMonsterStats.Width;
                monsterResistances.BorderStyle = BorderStyle.FixedSingle;
                monsterResistances.Location = new Point(baseMonsterStats.Location.X, monsterSavingThrows.Location.Y + monsterSavingThrows.Height + 20);

                Label monsterResistanceLabel = new Label();
                monsterResistanceLabel.Text = "Resistances";
                monsterResistanceLabel.TextAlign = ContentAlignment.MiddleCenter;
                monsterResistanceLabel.Width = monsterResistances.Width;
                monsterResistanceLabel.Location = new Point(0, 0);

                TextBox monsterResistanceInformation = new TextBox();
                monsterResistanceInformation.Text = toFight.getResistances();
                monsterResistanceInformation.Height = 75;
                monsterResistanceInformation.Location = new Point(0, monsterResistanceLabel.Height);
                monsterResistanceInformation.Multiline = true;
                monsterResistanceInformation.Width = monsterResistances.Width;
                monsterResistanceInformation.ReadOnly = true;
                monsterResistanceInformation.ScrollBars = ScrollBars.Vertical;

                monsterResistances.Height = monsterResistanceLabel.Height + monsterResistanceInformation.Height;
                monsterResistances.Controls.Add(monsterResistanceLabel);
                monsterResistances.Controls.Add(monsterResistanceInformation);

                currentSelected.Controls.Add(monsterResistances);

                //monster Immunities
                Panel monsterImmunities = new Panel();
                monsterImmunities.Width = baseMonsterStats.Width;
                monsterImmunities.BorderStyle = BorderStyle.FixedSingle;
                monsterImmunities.Location = new Point(baseMonsterStats.Location.X, monsterResistances.Location.Y + monsterResistances.Height + 20);

                //define immunities list
                List<string> immunitiesList = new List<string>();
                string immunitiesListOne = "";
                string immunitiesListTwo = "";
                immunitiesList = toFight.getImmunities();
                for(int i = 0; i < immunitiesList.Count; i++)
                {
                    if(i%2 == 0)
                    {
                        immunitiesListOne += (immunitiesList[i]) + "\r\n";
                    }
                    else
                    {
                        immunitiesListTwo += (immunitiesList[i]) + "\r\n";
                    }
                }

                Label monsterImmunityiesLabel = new Label();
                monsterImmunityiesLabel.Text = "Immunities";
                monsterImmunityiesLabel.TextAlign = ContentAlignment.MiddleCenter;
                monsterImmunityiesLabel.Width = monsterResistances.Width;
                monsterImmunityiesLabel.Location = new Point(0, 0);

                TextBox monsterImmunitiesInfoOne = new TextBox();
                monsterImmunitiesInfoOne.Text = immunitiesListOne;
                monsterImmunitiesInfoOne.Height = 75;
                monsterImmunitiesInfoOne.Location = new Point(0, monsterResistanceLabel.Height);
                monsterImmunitiesInfoOne.Multiline = true;
                monsterImmunitiesInfoOne.Width = monsterResistances.Width/2;
                monsterImmunitiesInfoOne.ReadOnly = true;
                //monsterImmunitiesInfoOne.ScrollBars = ScrollBars.Vertical;

                TextBox monsterImmunitiesInfoTwo = new TextBox();
                monsterImmunitiesInfoTwo.Text = immunitiesListTwo;
                monsterImmunitiesInfoTwo.Height = 75;
                monsterImmunitiesInfoTwo.Location = new Point(monsterImmunitiesInfoOne.Width, monsterResistanceLabel.Height);
                monsterImmunitiesInfoTwo.Multiline = true;
                monsterImmunitiesInfoTwo.Width = monsterResistances.Width / 2;
                monsterImmunitiesInfoTwo.ReadOnly = true;
                //monsterImmunitiesInfoTwo.ScrollBars = ScrollBars.Vertical;

                monsterImmunities.Height = monsterImmunityiesLabel.Height + monsterImmunitiesInfoOne.Height;
                monsterImmunities.Controls.Add(monsterImmunityiesLabel);
                monsterImmunities.Controls.Add(monsterImmunitiesInfoTwo);
                monsterImmunities.Controls.Add(monsterImmunitiesInfoOne);

                currentSelected.Controls.Add(monsterImmunities);

                //MonsterHP&AC
                Panel monsterHPArmor = new Panel();
                monsterHPArmor.Location = new Point(baseMonsterInfo.Location.X + baseMonsterInfo.Width + 20, baseMonsterInfo.Location.Y);

                Label maxHPLabel = new Label();
                maxHPLabel.Text = "Max HP :";
                maxHPLabel.Location = new Point(0, 0);

                Label maxHPInfo = new Label();
                maxHPInfo.Text = toFight.getMaxHP().ToString();
                maxHPInfo.Location = new Point(maxHPLabel.Location.X + maxHPLabel.Width, maxHPLabel.Location.Y);

                monsterHPArmor.Controls.Add(maxHPLabel);
                monsterHPArmor.Controls.Add(maxHPInfo);
                
                //Curr HP management
                Label currHPLabel = new Label();
                currHPLabel.Text = "Current HP :";
                currHPLabel.Location = new Point(maxHPInfo.Location.X + maxHPInfo.Width + 20, 0);

                TextBox currHPInfo = new TextBox();
                currHPInfo.Text = toFight.getCurrentHP().ToString();
                currHPInfo.Location = new Point(currHPLabel.Location.X + currHPLabel.Width, 0);
                currHPInfo.TextChanged += new EventHandler(monsterHealthUpdate);

                monsterHPArmor.Controls.Add(currHPLabel);
                monsterHPArmor.Controls.Add(currHPInfo);

                //AC Labels
                Label ACLabel = new Label();
                ACLabel.Text = "Armor Class";
                ACLabel.Location = new Point(currHPInfo.Location.X + currHPInfo.Width + 20, 0);

                Label ACinfo = new Label();
                ACinfo.Text = toFight.getArmorClass().ToString();
                ACinfo.Location = new Point(ACLabel.Location.X + ACLabel.Width, 0);

                monsterHPArmor.Controls.Add(ACLabel);
                monsterHPArmor.Controls.Add(ACinfo);

                monsterHPArmor.Width = 0;
                foreach(Control item in monsterHPArmor.Controls)
                {
                    monsterHPArmor.Width += item.Width;
                }
                currentSelected.Controls.Add(monsterHPArmor);

            }
            else if(currentlySelected.GetType() == typeof(MonsterGroup))
            {
                //redefine as actual type
                toFightGroup = (MonsterGroup)currentlySelected;
            }
        }

        private void monsterHealthUpdate(object sender, EventArgs e)
        {
            TextBox eventThrower = (TextBox)sender;

            //check cases
            if (!checkStringForNonNumber(eventThrower.Text))
            {
                //contains non-numbers
                MessageBox.Show("HP must consist of numbers...");
                eventThrower.Text = toFight.getCurrentHP().ToString();
            }
            else if (string.IsNullOrEmpty(eventThrower.Text))
            {
                //empty textbox
            }
            else if(Convert.ToInt32(eventThrower.Text)  > toFight.getMaxHP())
            {
                //invalid amount greater than MAX HP
                MessageBox.Show("This is more than the max HP...");
                eventThrower.Text = toFight.getCurrentHP().ToString();
            }
            else
            {
                //valid entry, update
                toFight.setCurrentHP(Convert.ToInt32(eventThrower.Text));
            }
        }

        //returns false if contains non-number characters
        private bool checkStringForNonNumber(string testString)
        {
            bool toRet = true;

            foreach(char toTest in testString)
            {
                if (!char.IsNumber(toTest))
                {
                    //not a number, error
                    toRet = false;
                }
            }

            return toRet;
        }
    }
}
