using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace DND_Application_2
{
    public partial class FormMain : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DND_Application_2.Properties.Settings._5e_DataConnectionString"].ConnectionString;
        ListBox campaignSelectorList;

        public FormMain()
        {
            InitializeComponent();

            this.Width = 300;
            this.Height = 300;

            Label campaignSelectionLabel = new Label();
            campaignSelectionLabel.Width = 250;
            campaignSelectionLabel.Height = 30;
            campaignSelectionLabel.Location = new Point(15, 5);
            campaignSelectionLabel.TextAlign = ContentAlignment.MiddleCenter;
            campaignSelectionLabel.Text = "Please select an existing campaign or create a new campaign to begin.";
            this.Controls.Add(campaignSelectionLabel);

            campaignSelectorList = new ListBox();
            campaignSelectorList.Width = 250;
            campaignSelectorList.Height = 175;
            campaignSelectorList.Location = new Point(15, 40);
            this.Controls.Add(campaignSelectorList);

            //Populate Campaing Listbox
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adaptor = new SqlDataAdapter("SELECT * FRom Campaigns", con))
                {
                    DataTable campaignTable = new DataTable();
                    adaptor.Fill(campaignTable);

                    campaignSelectorList.DisplayMember = "CampaignName";
                    campaignSelectorList.ValueMember = "CampaignID";
                    campaignSelectorList.DataSource = campaignTable;
                    con.Close();
                }

            }

            //Button to select campaign
            Button selectCampaign = new Button();
            selectCampaign.Location = new Point(140, 220);
            selectCampaign.Click += new EventHandler(OpenNewEventWindow);
            selectCampaign.Width = 125;
            selectCampaign.Text = "Select Campaign";
            this.Controls.Add(selectCampaign);
        }

        private void OpenNewEventWindow(object sender, EventArgs e)
        {
            //get selected campaign ID
            int CampaignID = (int)campaignSelectorList.SelectedValue;
            PartyClass selectedCampaignParty = new PartyClass();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adaptor = new SqlDataAdapter("SELECT * FRom PlayerBase WHERE CampaignId = '" + CampaignID + "'", con))
                {
                    DataTable playerTable = new DataTable();
                    adaptor.Fill(playerTable);

                    foreach(DataRow PlayerInfo in playerTable.Rows)
                    {
                        PlayerClass toAdd = new PlayerClass(PlayerInfo["PlayerName"].ToString(), (int)PlayerInfo["CharacterLevel"], PlayerInfo["CharacterName"].ToString());
                        selectedCampaignParty.addPartyMember(toAdd);
                    }

                    con.Close();
                }

            }

            RandomOrBuiltEventForm newEventBuilder = new RandomOrBuiltEventForm();
            newEventBuilder.setCampaignParty(selectedCampaignParty);
            newEventBuilder.Visible = true;
        }
    }

}



