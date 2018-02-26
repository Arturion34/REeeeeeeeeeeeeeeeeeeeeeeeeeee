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

        int numberMonsters;
        int numberMonstersAndPlayers;

        //Arrow Generation Variables
        int baseX = 200;
        int baseY = 20;
        int XDifference;
        int YDifference = 80;

        //Formating Variables
        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        int baseGap = 20;

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
            monsterListPanel.Width = (int)(((0.15625))*((double)screenWidth));
            monsterListPanel.Height = (int)((double)screenHeight*0.870);
            monsterListPanel.Location = new Point(baseGap, (int)((double)screenHeight*(0.1111)));
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
            currentSelected.Location = new Point(baseGap*2 + monsterListPanel.Width, monsterListPanel.Location.Y);
            currentSelected.BorderStyle = BorderStyle.FixedSingle;
            currentSelected.Height = (int)((double)screenHeight*0.87037);
            currentSelected.Width = screenWidth - 3*baseGap - monsterListPanel.Width;

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

                //Monster Name
                Label monsterName = new Label();
                monsterName.Text = toFight.getName();
                monsterName.Location = new Point((int)((double)currentSelected.Width * 0.0128205128205128), (int)((double)currentSelected.Height * 0.0214132762312634));
                monsterName.Width = 624;
                monsterName.Height = 137;
                monsterName.BorderStyle = BorderStyle.FixedSingle;
                monsterName.TextAlign = ContentAlignment.MiddleCenter;
                monsterName.Font = getResizedText(toFight.getName(), monsterName.Height, monsterName.Width, monsterName.Font);
                currentSelected.Controls.Add(monsterName);

                //Define Base Monster Information. Name, Type, Size, Alignment, Languages

                currentSelected.Controls.Add(buildMonsterBaseInfoPanel());
                currentSelected.Controls.Add(buildMonsterStatPanel());
                currentSelected.Controls.Add(buildMovementSpeedPanel());
                currentSelected.Controls.Add(buildSensesAndSkillsPanel());
                currentSelected.Controls.Add(buildSavingThrowsPanel());
                currentSelected.Controls.Add(buildResistancesPanel());
                currentSelected.Controls.Add(buildImmunitiesPanel());
                currentSelected.Controls.Add(buildArmorClassPanel());
                currentSelected.Controls.Add(buildMaxHPPanel());
                currentSelected.Controls.Add(buildCurrentHPPanel());
                currentSelected.Controls.Add(buildMonsterActions());
                currentSelected.Controls.Add(buildMonsterNotes());

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

        //function for creating notes panel
        private Panel buildMonsterNotes()
        {
            Panel monsterNotesPanel = new Panel();
            monsterNotesPanel.Height = 312;
            monsterNotesPanel.Width = 975;
            monsterNotesPanel.Location = new Point((int)((double)currentSelected.Width * 0.3384615384615385), (int)((double)currentSelected.Height * 0.1889721627408994));
            monsterNotesPanel.BorderStyle = BorderStyle.FixedSingle;

            return monsterNotesPanel;
        }

        //function for creating actions panel
        private Panel buildMonsterActions()
        {
            Panel monsterActionsPanel = new Panel();
            monsterActionsPanel.Height = 312;
            monsterActionsPanel.Width = 975;
            monsterActionsPanel.Location = new Point((int)((double)currentSelected.Width * 0.3384615384615385), (int)((double)currentSelected.Height * 0.5444325481798715));
            monsterActionsPanel.BorderStyle = BorderStyle.FixedSingle;

            List<MonsterAction> actionList = toFight.getActions();
            List<Button> actionButtons = new List<Button>();

            for(int i = 0; i < actionList.Count; i++)
            {
                //define current Action
                MonsterAction currAction = actionList[i];

                Button actionSelect = new Button();
                actionSelect.Width = monsterActionsPanel.Width / actionList.Count;
                actionSelect.Height = 30;
                actionSelect.Text = currAction.getActionName();
                actionSelect.Location = new Point(i * (monsterActionsPanel.Width / actionList.Count), 0);

                //if number of uses is limited, include in name
                if(currAction.getNumberUses() != 0)
                {
                    actionSelect.Text += "[" + currAction.getNumberUses() + "]";
                }

                //if action is legendary, cover in gold
                if (currAction.getIsLegendary())
                {
                    actionSelect.BackColor = Color.Gold;
                }

                //add actionButton to list
                actionButtons.Add(actionSelect);
                monsterActionsPanel.Controls.Add(actionSelect);
            }

            return monsterActionsPanel;
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

        //Function to build base monster info panel
        private Panel buildMonsterBaseInfoPanel()
        {
            Panel baseMonsterInfo = new Panel();
            baseMonsterInfo.Width = 234;
            baseMonsterInfo.Height = 156;
            baseMonsterInfo.Location = new Point((int)((double)currentSelected.Width * 0.0128205128205128), (int)((double)currentSelected.Height * 0.1895074946466809));
            baseMonsterInfo.BorderStyle = BorderStyle.FixedSingle;

            //Define MonsterType
            Label monsterTypeLabel = new Label();
            monsterTypeLabel.Text = "Type :";
            monsterTypeLabel.Location = new Point(0, 0);
            monsterTypeLabel.Height = baseMonsterInfo.Height / 5;
            monsterTypeLabel.Width = baseMonsterInfo.Width / 2;
            monsterTypeLabel.BorderStyle = BorderStyle.FixedSingle;

            Label monsterType = new Label();
            monsterType.Text = toFight.getMonsterType();
            monsterType.Location = new Point(monsterTypeLabel.Location.X + monsterTypeLabel.Width, monsterTypeLabel.Location.Y);
            monsterType.Height = baseMonsterInfo.Height / 5;
            monsterType.Width = baseMonsterInfo.Width / 2;
            monsterType.BorderStyle = BorderStyle.FixedSingle;

            baseMonsterInfo.Controls.Add(monsterTypeLabel);
            baseMonsterInfo.Controls.Add(monsterType);

            //Define MonsterSize
            Label monsterSizeLabel = new Label();
            monsterSizeLabel.Text = "Size :";
            monsterSizeLabel.Location = new Point(0, monsterTypeLabel.Height);
            monsterSizeLabel.Height = baseMonsterInfo.Height / 5;
            monsterSizeLabel.Width = baseMonsterInfo.Width / 2;
            monsterSizeLabel.BorderStyle = BorderStyle.FixedSingle;

            Label monsterSize = new Label();
            monsterSize.Text = toFight.getSize();
            monsterSize.Location = new Point(monsterSizeLabel.Location.X + monsterSizeLabel.Width, monsterSizeLabel.Location.Y);
            monsterSize.Height = baseMonsterInfo.Height / 5;
            monsterSize.Width = baseMonsterInfo.Width / 2;
            monsterSize.BorderStyle = BorderStyle.FixedSingle;

            baseMonsterInfo.Controls.Add(monsterSizeLabel);
            baseMonsterInfo.Controls.Add(monsterSize);

            //DefineMonsterLangauges
            Label monsterLanguagesLabel = new Label();
            monsterLanguagesLabel.Text = "Languages :";
            monsterLanguagesLabel.Height = 2 * (baseMonsterInfo.Height / 5);
            monsterLanguagesLabel.Width = baseMonsterInfo.Width / 2;
            monsterLanguagesLabel.Location = new Point(0, monsterTypeLabel.Height * 2);
            monsterLanguagesLabel.BorderStyle = BorderStyle.FixedSingle;

            TextBox monsterLanguages = new TextBox();
            string languages = "";
            foreach (string language in toFight.getLanguages())
            {
                languages += language + "\r\n";
            }
            monsterLanguages.ReadOnly = true;
            monsterLanguages.ScrollBars = ScrollBars.Vertical;
            monsterLanguages.Text = languages;
            monsterLanguages.Multiline = true;
            monsterLanguages.Height = 2 * (baseMonsterInfo.Height / 5);
            monsterLanguages.Width = baseMonsterInfo.Width / 2;
            monsterLanguages.Location = new Point(monsterLanguages.Location.X + monsterLanguagesLabel.Width, monsterLanguagesLabel.Location.Y);
            monsterLanguages.BorderStyle = BorderStyle.FixedSingle;

            baseMonsterInfo.Controls.Add(monsterLanguagesLabel);
            baseMonsterInfo.Controls.Add(monsterLanguages);

            //Define MonsterAlignment
            Label monsterAlignmentLabel = new Label();
            monsterAlignmentLabel.Text = "Alignment: ";
            monsterAlignmentLabel.Location = new Point(0, monsterTypeLabel.Height * 2 + monsterLanguages.Height);
            monsterAlignmentLabel.Height = baseMonsterInfo.Height / 5;
            monsterAlignmentLabel.Width = baseMonsterInfo.Width / 2;
            monsterAlignmentLabel.BorderStyle = BorderStyle.FixedSingle;

            Label monsterAlignment = new Label();
            monsterAlignment.Text = toFight.getAlignment();
            monsterAlignment.Location = new Point(monsterAlignmentLabel.Location.X + monsterAlignmentLabel.Width, monsterAlignmentLabel.Location.Y);
            monsterAlignment.BorderStyle = BorderStyle.FixedSingle;
            monsterAlignment.Height = baseMonsterInfo.Height / 5;
            monsterAlignment.Width = baseMonsterInfo.Width / 2;

            baseMonsterInfo.Controls.Add(monsterAlignment);
            baseMonsterInfo.Controls.Add(monsterAlignmentLabel);

            //add baseMonsterInfo to panel
            baseMonsterInfo.BorderStyle = BorderStyle.FixedSingle;

            return baseMonsterInfo;
        }

        //Function to build base stat panel
        private Panel buildMonsterStatPanel()
        {
            //define Monster stats and modifiers
            Panel baseMonsterStats = new Panel();
            baseMonsterStats.Location = new Point((int)((double)currentSelected.Width * 0.0128205128205128), (int)((double)currentSelected.Height * 0.3779443254817987));
            baseMonsterStats.Height = 156;
            baseMonsterStats.Width = 234;
            baseMonsterStats.BorderStyle = BorderStyle.FixedSingle;
            int labelWidth = baseMonsterStats.Width / 3;
            int statLabelHeight = baseMonsterStats.Height / 6;

            //strength information
            Label strengthLabel = new Label();
            strengthLabel.Text = "STR :";
            strengthLabel.Location = new Point(0, 0);
            strengthLabel.Width = labelWidth;
            strengthLabel.Height = statLabelHeight;

            Label strengthstat = new Label();
            strengthstat.Text = (toFight.getStrength()).ToString();
            strengthstat.Location = new Point(strengthLabel.Location.X + strengthLabel.Width, strengthLabel.Location.Y);
            strengthstat.Width = labelWidth;
            strengthstat.Height = statLabelHeight;

            Label strengthModifier = new Label();
            strengthModifier.Text = ((toFight.getStrength() - 10) / 2).ToString();
            strengthModifier.Location = new Point(strengthstat.Location.X + strengthstat.Width, strengthLabel.Location.Y);
            strengthModifier.Width = labelWidth;
            strengthModifier.Height = statLabelHeight;

            baseMonsterStats.Controls.Add(strengthLabel);
            baseMonsterStats.Controls.Add(strengthstat);
            baseMonsterStats.Controls.Add(strengthModifier);

            //Dexterity
            Label dexterityLabel = new Label();
            dexterityLabel.Text = "DEX :";
            dexterityLabel.Location = new Point(0, strengthLabel.Height);
            dexterityLabel.Width = labelWidth;
            dexterityLabel.Height = statLabelHeight;

            Label dexterityStat = new Label();
            dexterityStat.Text = ((toFight.getDexterity()).ToString());
            dexterityStat.Location = new Point(dexterityLabel.Location.X + dexterityLabel.Width, dexterityLabel.Location.Y);
            dexterityStat.Width = labelWidth;
            dexterityStat.Height = statLabelHeight;

            Label dexterityModifier = new Label();
            dexterityModifier.Text = ((toFight.getDexterity() - 10) / 2).ToString();
            dexterityModifier.Location = new Point(dexterityStat.Location.X + dexterityStat.Width, dexterityLabel.Location.Y);
            dexterityModifier.Width = labelWidth;
            dexterityModifier.Height = statLabelHeight;

            baseMonsterStats.Controls.Add(dexterityLabel);
            baseMonsterStats.Controls.Add(dexterityStat);
            baseMonsterStats.Controls.Add(dexterityModifier);

            //Constitution
            Label constitutionLabel = new Label();
            constitutionLabel.Text = "CON :";
            constitutionLabel.Location = new Point(0, dexterityLabel.Location.Y + dexterityLabel.Height);
            constitutionLabel.Width = labelWidth;
            constitutionLabel.Height = statLabelHeight;

            Label constitutionStat = new Label();
            constitutionStat.Text = toFight.getConstitution().ToString();
            constitutionStat.Location = new Point(constitutionLabel.Location.X + constitutionLabel.Width, constitutionLabel.Location.Y);
            constitutionStat.Width = labelWidth;
            constitutionStat.Height = statLabelHeight;

            Label constitutionModifier = new Label();
            constitutionModifier.Text = ((toFight.getConstitution() - 10) / 2).ToString();
            constitutionModifier.Location = new Point(constitutionStat.Location.X + constitutionStat.Width, constitutionLabel.Location.Y);
            constitutionModifier.Width = labelWidth;
            constitutionModifier.Height = statLabelHeight;

            baseMonsterStats.Controls.Add(constitutionLabel);
            baseMonsterStats.Controls.Add(constitutionStat);
            baseMonsterStats.Controls.Add(constitutionModifier);

            //Intelligence
            Label intelligenceLabel = new Label();
            intelligenceLabel.Text = "INT :";
            intelligenceLabel.Location = new Point(0, constitutionLabel.Location.Y + constitutionLabel.Height);
            intelligenceLabel.Width = labelWidth;
            intelligenceLabel.Height = statLabelHeight;

            Label intelligenceStat = new Label();
            intelligenceStat.Text = toFight.getIntelligence().ToString();
            intelligenceStat.Location = new Point(intelligenceLabel.Location.X + intelligenceLabel.Width, intelligenceLabel.Location.Y);
            intelligenceStat.Width = labelWidth;
            intelligenceStat.Height = statLabelHeight;

            Label intelligenceModifier = new Label();
            intelligenceModifier.Text = ((toFight.getIntelligence() - 10) / 2).ToString();
            intelligenceModifier.Location = new Point(intelligenceStat.Location.X + intelligenceStat.Width, intelligenceLabel.Location.Y);
            intelligenceModifier.Width = labelWidth;
            intelligenceModifier.Height = statLabelHeight;

            baseMonsterStats.Controls.Add(intelligenceLabel);
            baseMonsterStats.Controls.Add(intelligenceStat);
            baseMonsterStats.Controls.Add(intelligenceModifier);

            //Wisdom
            Label wisdomLabel = new Label();
            wisdomLabel.Text = "WIS :";
            wisdomLabel.Location = new Point(0, intelligenceLabel.Location.Y + intelligenceLabel.Height);
            wisdomLabel.Width = labelWidth;
            wisdomLabel.Height = statLabelHeight;

            Label wisdomStat = new Label();
            wisdomStat.Text = toFight.getWisdom().ToString();
            wisdomStat.Location = new Point(wisdomLabel.Location.X + wisdomLabel.Width, wisdomLabel.Location.Y);
            wisdomStat.Width = labelWidth;
            wisdomStat.Height = statLabelHeight;

            Label wisdomModifier = new Label();
            wisdomModifier.Text = ((toFight.getWisdom() - 10) / 2).ToString();
            wisdomModifier.Location = new Point(wisdomStat.Location.X + wisdomStat.Width, wisdomLabel.Location.Y);
            wisdomModifier.Width = labelWidth;
            wisdomModifier.Height = statLabelHeight;

            baseMonsterStats.Controls.Add(wisdomLabel);
            baseMonsterStats.Controls.Add(wisdomStat);
            baseMonsterStats.Controls.Add(wisdomModifier);

            //Charisma
            Label charismaLabel = new Label();
            charismaLabel.Text = "CHA :";
            charismaLabel.Location = new Point(0, wisdomLabel.Location.Y + wisdomLabel.Height);
            charismaLabel.Width = labelWidth;
            charismaLabel.Height = statLabelHeight;

            Label charismaStat = new Label();
            charismaStat.Text = toFight.getCharisma().ToString();
            charismaStat.Location = new Point(charismaLabel.Location.X + charismaLabel.Width, charismaLabel.Location.Y);
            charismaStat.Width = labelWidth;
            charismaStat.Height = statLabelHeight;

            Label charismaModifier = new Label();
            charismaModifier.Text = ((toFight.getCharisma() - 10) / 2).ToString();
            charismaModifier.Location = new Point(charismaStat.Location.X + charismaStat.Width, charismaLabel.Location.Y);
            charismaModifier.Width = labelWidth;
            charismaModifier.Height = statLabelHeight;

            baseMonsterStats.Controls.Add(charismaLabel);
            baseMonsterStats.Controls.Add(charismaStat);
            baseMonsterStats.Controls.Add(charismaModifier);

            return baseMonsterStats;
        }

        //Function to build the movement speed panel
        private Panel buildMovementSpeedPanel()
        {
            Panel baseMonsterSpeeds = new Panel();
            baseMonsterSpeeds.Height = 98;
            baseMonsterSpeeds.Width = 234;
            baseMonsterSpeeds.Location = new Point((int)((double)currentSelected.Width * 0.1756410256410256), (int)((double)currentSelected.Height * 0.1895074946466809));

            //List to store labels to allow for dynamic sizing
            List<Label> speedLabelList = new List<Label>();

            //Walk Speed
            if (toFight.getWalkSpeed() != null)
            {
                Label walkLabel = new Label();
                walkLabel.Text = "Walk :";

                Label walkDistance = new Label();
                walkDistance.Text = toFight.getWalkSpeed().ToString() + "ft.";

                speedLabelList.Add(walkLabel);
                speedLabelList.Add(walkDistance);
            }

            //Swimspeed
            if (toFight.getSwimSpeed() != null)
            {
                Label swimLabel = new Label();
                swimLabel.Text = "Swim :";

                Label swimDistance = new Label();
                swimDistance.Text = toFight.getSwimSpeed().ToString() + "ft.";

                speedLabelList.Add(swimLabel);
                speedLabelList.Add(swimDistance);
            }

            //burrowSpeed
            if (toFight.getBurrowSpeed() != null)
            {
                Label burrowLabel = new Label();
                burrowLabel.Text = "Burrow: ";

                Label burrowDistance = new Label();
                burrowDistance.Text = toFight.getBurrowSpeed().ToString() + "ft.";

                speedLabelList.Add(burrowLabel);
                speedLabelList.Add(burrowDistance);
            }

            //fly speed
            if (toFight.getFlySpeed() != null)
            {
                Label flyLabel = new Label();
                flyLabel.Text = "Fly: ";

                Label flyDistance = new Label();
                flyDistance.Text = toFight.getFlySpeed().ToString() + "ft.";

                speedLabelList.Add(flyLabel);
                speedLabelList.Add(flyDistance);
            }

            int baseSpeedLabelY = baseMonsterSpeeds.Height / (speedLabelList.Count / 2);
            //calculate size and location for speed label
            for (int i = 0; i < speedLabelList.Count; i++)
            {
                if (i % 2 == 0)
                {
                    //this is the descriptor label
                    Label toAdd = speedLabelList[i];
                    toAdd.Height = baseSpeedLabelY;
                    toAdd.Location = new Point(0, baseSpeedLabelY * (i / 2));

                    //this is the information label
                    Label toAddData = speedLabelList[i + 1];
                    toAddData.Height = baseSpeedLabelY;
                    toAddData.Location = new Point(toAdd.Width, baseSpeedLabelY * (i / 2));

                    baseMonsterSpeeds.Controls.Add(toAdd);
                    baseMonsterSpeeds.Controls.Add(toAddData);
                }
            }
            baseMonsterSpeeds.BorderStyle = BorderStyle.FixedSingle;

            return baseMonsterSpeeds;
        }

        //function to build senses and skills panels
        private Panel buildSensesAndSkillsPanel()
        {
            Panel sensesAndSkillsPanel = new Panel();
            sensesAndSkillsPanel.Location = new Point((int)((double)currentSelected.Width * 0.0128205128205128), (int)((double)currentSelected.Height * 0.5658458244111349));
            sensesAndSkillsPanel.BorderStyle = BorderStyle.FixedSingle;
            sensesAndSkillsPanel.Width = 234;
            sensesAndSkillsPanel.Height = 156;

            Label SensesLabel = new Label();
            SensesLabel.Text = "Senses | Skills";
            SensesLabel.Location = new Point(0, 0);
            SensesLabel.Width = sensesAndSkillsPanel.Width;
            SensesLabel.TextAlign = ContentAlignment.MiddleCenter;

            TextBox SensesDetails = new TextBox();
            SensesDetails.Text = toFight.getSenses().Replace(", ", "\r\n");
            SensesDetails.Height = (sensesAndSkillsPanel.Height - SensesLabel.Height) / 2;
            SensesDetails.Location = new Point(0, SensesLabel.Height);
            SensesDetails.Multiline = true;
            SensesDetails.Width = sensesAndSkillsPanel.Width;
            SensesDetails.ReadOnly = true;
            SensesDetails.ScrollBars = ScrollBars.Vertical;

            TextBox SkillsDetails = new TextBox();
            SkillsDetails.Text = toFight.getSkills();
            SkillsDetails.Height = (sensesAndSkillsPanel.Height - SensesLabel.Height) / 2;
            SkillsDetails.Location = new Point(0, SensesDetails.Location.Y + SensesDetails.Height);
            SkillsDetails.Multiline = true;
            SkillsDetails.Width = sensesAndSkillsPanel.Width;
            SkillsDetails.ReadOnly = true;
            SkillsDetails.ScrollBars = ScrollBars.Vertical;

            sensesAndSkillsPanel.Controls.Add(SensesLabel);
            sensesAndSkillsPanel.Controls.Add(SensesDetails);
            sensesAndSkillsPanel.Controls.Add(SkillsDetails);

            return sensesAndSkillsPanel;
        }

        //function to build Resistances panel
        private Panel buildResistancesPanel()
        {
            Panel monsterResistances = new Panel();
            monsterResistances.Width = 234;
            monsterResistances.Height = 117;
            monsterResistances.BorderStyle = BorderStyle.FixedSingle;
            monsterResistances.Location = new Point((int)((double)currentSelected.Width * 0.1756410256410256), (int)((double)currentSelected.Height * 0.4614561027837259));

            Label monsterResistanceLabel = new Label();
            monsterResistanceLabel.Text = "Resistances";
            monsterResistanceLabel.TextAlign = ContentAlignment.MiddleCenter;
            monsterResistanceLabel.Width = monsterResistances.Width;
            monsterResistanceLabel.Location = new Point(0, 0);

            TextBox monsterResistanceInformation = new TextBox();
            monsterResistanceInformation.Text = toFight.getResistances();
            monsterResistanceInformation.Height = monsterResistances.Height - monsterResistanceLabel.Height;
            monsterResistanceInformation.Location = new Point(0, monsterResistanceLabel.Height);
            monsterResistanceInformation.Multiline = true;
            monsterResistanceInformation.Width = monsterResistances.Width;
            monsterResistanceInformation.ReadOnly = true;
            monsterResistanceInformation.ScrollBars = ScrollBars.Vertical;

            monsterResistances.Controls.Add(monsterResistanceLabel);
            monsterResistances.Controls.Add(monsterResistanceInformation);

            return monsterResistances;
        }

        //function to build immunities Panel
        private Panel buildImmunitiesPanel()
        {
            //monster Immunities
            Panel monsterImmunities = new Panel();
            monsterImmunities.Width = 234;
            monsterImmunities.Height = 117;
            monsterImmunities.BorderStyle = BorderStyle.FixedSingle;
            monsterImmunities.Location = new Point((int)((double)currentSelected.Width * 0.1756410256410256), (int)((double)currentSelected.Height * 0.3147751605995717));

            //define immunities list
            List<string> immunitiesList = new List<string>();
            string immunitiesListOne = "";
            string immunitiesListTwo = "";
            immunitiesList = toFight.getImmunities();
            for (int i = 0; i < immunitiesList.Count; i++)
            {
                if (i % 2 == 0)
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
            monsterImmunityiesLabel.Width = monsterImmunities.Width;
            monsterImmunityiesLabel.Location = new Point(0, 0);

            TextBox monsterImmunitiesInfoOne = new TextBox();
            monsterImmunitiesInfoOne.Text = immunitiesListOne;
            monsterImmunitiesInfoOne.Height = monsterImmunities.Height - monsterImmunityiesLabel.Height;
            monsterImmunitiesInfoOne.Location = new Point(0, monsterImmunityiesLabel.Height);
            monsterImmunitiesInfoOne.Multiline = true;
            monsterImmunitiesInfoOne.Width = monsterImmunities.Width / 2;
            monsterImmunitiesInfoOne.ReadOnly = true;
            //monsterImmunitiesInfoOne.ScrollBars = ScrollBars.Vertical;

            TextBox monsterImmunitiesInfoTwo = new TextBox();
            monsterImmunitiesInfoTwo.Text = immunitiesListTwo;
            monsterImmunitiesInfoTwo.Height = monsterImmunities.Height - monsterImmunityiesLabel.Height;
            monsterImmunitiesInfoTwo.Location = new Point(monsterImmunitiesInfoOne.Width, monsterImmunityiesLabel.Height);
            monsterImmunitiesInfoTwo.Multiline = true;
            monsterImmunitiesInfoTwo.Width = monsterImmunities.Width / 2;
            monsterImmunitiesInfoTwo.ReadOnly = true;
            //monsterImmunitiesInfoTwo.ScrollBars = ScrollBars.Vertical;

            monsterImmunities.Controls.Add(monsterImmunityiesLabel);
            monsterImmunities.Controls.Add(monsterImmunitiesInfoTwo);
            monsterImmunities.Controls.Add(monsterImmunitiesInfoOne);

            return monsterImmunities;
        }

        //function to build Armor Class Panel
        private Panel buildArmorClassPanel()
        {
            Panel monsterArmor = new Panel();
            monsterArmor.Height = 137;
            monsterArmor.Width = 234;
            monsterArmor.Location = new Point((int)((double)currentSelected.Width * 0.4256410256410256), (int)((double)currentSelected.Height * 0.0214132762312634));
            monsterArmor.BorderStyle = BorderStyle.FixedSingle;

            //AC Labels
            Label ACLabel = new Label();
            ACLabel.Text = "AC:";
            ACLabel.Width = monsterArmor.Width / 2;
            ACLabel.Height = monsterArmor.Height;
            ACLabel.Location = new Point(0, 0);
            ACLabel.Font = getResizedText(ACLabel.Text, ACLabel.Height, ACLabel.Width, ACLabel.Font);

            Label ACinfo = new Label();
            ACinfo.Text = toFight.getArmorClass().ToString();
            ACinfo.Width = monsterArmor.Width / 2;
            ACinfo.Height = monsterArmor.Height;
            ACinfo.Location = new Point(ACLabel.Location.X + ACLabel.Width, 0);
            ACinfo.Font = getResizedText(ACinfo.Text, ACinfo.Height, ACinfo.Width, ACinfo.Font);

            monsterArmor.Controls.Add(ACLabel);
            monsterArmor.Controls.Add(ACinfo);

            return monsterArmor;
        }

        //function to build maxHP panel
        private Panel buildMaxHPPanel()
        {
            Panel maxHP = new Panel();
            maxHP.Width = 234;
            maxHP.Height = 137;
            maxHP.Location = new Point((int)((double)currentSelected.Width * 0.5884615384615384), (int)((double)currentSelected.Height * 0.0214132762312634));
            maxHP.BorderStyle = BorderStyle.FixedSingle;

            Label maxHPLabel = new Label();
            maxHPLabel.Text = "Max HP :";
            maxHPLabel.Width = maxHP.Width / 2;
            maxHPLabel.Height = maxHP.Height;
            maxHPLabel.Location = new Point(0, 0);
            maxHPLabel.Font = getResizedText(maxHPLabel.Text, maxHPLabel.Height, maxHPLabel.Width, maxHPLabel.Font);

            Label maxHPInfo = new Label();
            maxHPInfo.Text = toFight.getMaxHP().ToString();
            maxHPInfo.Width = maxHP.Width / 2;
            maxHPInfo.Height = maxHP.Height;
            maxHPInfo.Location = new Point(maxHPLabel.Location.X + maxHPLabel.Width, maxHPLabel.Location.Y);
            maxHPInfo.Font = getResizedText(maxHPInfo.Text, maxHPInfo.Height, maxHPInfo.Width, maxHPInfo.Font);

            maxHP.Controls.Add(maxHPLabel);
            maxHP.Controls.Add(maxHPInfo);

            return maxHP;
        }

        //function to build Current HP Panel
        private Panel buildCurrentHPPanel()
        {
            Panel currentHP = new Panel();
            currentHP.Width = 234;
            currentHP.Height = 137;
            currentHP.Location = new Point((int)((double)currentSelected.Width * 0.7512820512820512), (int)((double)currentSelected.Height * 0.0214132762312634));
            currentHP.BorderStyle = BorderStyle.FixedSingle;

            Label currHPLabel = new Label();
            currHPLabel.Text = "HP";
            currHPLabel.Height = currentHP.Height;
            currHPLabel.Width = currentHP.Width / 2;
            currHPLabel.Location = new Point(0, 0);
            currHPLabel.Font = getResizedText(currHPLabel.Text, currHPLabel.Height, currHPLabel.Width, currHPLabel.Font);

            TextBox currHPInfo = new TextBox();
            currHPInfo.Text = toFight.getCurrentHP().ToString();
            currHPInfo.Multiline = true;
            currHPInfo.Width = currentHP.Width / 2;
            currHPInfo.Height = currentHP.Height;
            currHPInfo.Location = new Point(currHPLabel.Location.X + currHPLabel.Width, 0);
            currHPInfo.Font = getResizedText("00", currHPInfo.Height, currHPInfo.Width, currHPInfo.Font);
            currHPInfo.TextChanged += new EventHandler(monsterHealthUpdate);


            currentHP.Controls.Add(currHPLabel);
            currentHP.Controls.Add(currHPInfo);

            return currentHP;
        }

        //function to build Saving Throws Panel
        private Panel buildSavingThrowsPanel()
        {
            Panel monsterSavingThrows = new Panel();
            monsterSavingThrows.Location = new Point((int)((double)currentSelected.Width * 0.1756410256410256), (int)((double)currentSelected.Height * 0.6081370449678801));
            monsterSavingThrows.Width = 234;
            monsterSavingThrows.Height = 117;
            monsterSavingThrows.BorderStyle = BorderStyle.FixedSingle;

            Label savingThrowsLabel = new Label();
            savingThrowsLabel.Location = new Point(0, 0);
            savingThrowsLabel.Width = monsterSavingThrows.Width;
            savingThrowsLabel.Text = "Saving Throws";
            savingThrowsLabel.TextAlign = ContentAlignment.MiddleCenter;

            TextBox savingThrowInfo = new TextBox();
            savingThrowInfo.Text = toFight.getSavingThrows();
            savingThrowInfo.Height = monsterSavingThrows.Height - savingThrowsLabel.Height;
            savingThrowInfo.Location = new Point(0, savingThrowsLabel.Height);
            savingThrowInfo.Multiline = true;
            savingThrowInfo.Width = monsterSavingThrows.Width;
            savingThrowInfo.ReadOnly = true;
            savingThrowInfo.ScrollBars = ScrollBars.Vertical;

            monsterSavingThrows.Controls.Add(savingThrowsLabel);
            monsterSavingThrows.Controls.Add(savingThrowInfo);

            return monsterSavingThrows;
        }

        /*
         * Function: getResizedText
         * Purpose: This function dynamically determines the font size to fill a box
         * Parameters:
         *      string toGetSize : the string to populate the text box
         *      int height : the height of the area to fill
         *      int width : the width of the area to fill
         *      Font currentFont : the current font being used
         */
        private Font getResizedText(string toGetSize, int frameHeight, int frameWidth, Font currentFont)
        {
            Font toReturn;

            //calculate size of string to fit box
            Size stringSize = new Size();
            stringSize = TextRenderer.MeasureText(toFight.getName().Replace(" ", "_"), currentFont);
            double yRatio = (double)frameHeight / (double)stringSize.Height;
            double xRatio = (double)frameWidth / (double)stringSize.Width;

            if (yRatio > xRatio)
            {
                toReturn = new Font(currentFont.Name, (float)(currentFont.Size * xRatio));
            }
            else
            {
                toReturn = new Font(currentFont.Name, (float)(currentFont.Size * yRatio));
            }

            return toReturn;
        }
    }
}
