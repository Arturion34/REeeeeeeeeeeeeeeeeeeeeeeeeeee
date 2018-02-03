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
        List<Monster> encounterMonsters;
        PartyClass currentParty;
        Panel monsterListPanel;
        Button nextTurn;
        Graphics pane;
        Boolean originalArrowsDrawn = false;
        int currentInitiative = 0;

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

            //define nextTurn Button
            nextTurn = new Button();
            nextTurn.Height = YDifference;
            nextTurn.Width = baseX - 20;
            nextTurn.Text = "Next";
            nextTurn.Click += new EventHandler(nextInitiative);
            this.Controls.Add(nextTurn);

            //set num monsters to 0
            numberMonsters = 0;
            List<Object> monsterListForButtons = new List<Object>();

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
                monsterListPanel.Controls.Add(monsterLabelButton);
            }

            currentInitiative = 0;
            this.Controls.Add(monsterListPanel);
        }

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

        private void setInitiativeOrderList(List<Object> toSet)
        {
            initiativeOrder = toSet;
        }

    }
}
