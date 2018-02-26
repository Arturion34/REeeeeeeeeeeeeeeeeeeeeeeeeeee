using DND_Application_2.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND_Application_2
{
    
    public class Monster
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DND_Application_2.Properties.Settings._5e_DataConnectionString"].ConnectionString;
        private int maxHP;
        private int currHP;
        private int ArmorClass;
        string Name;
        private int strength;
        private int dexterity;
        private int charisma;
        private int wisdom;
        private int constitution;
        private int intelligence;
        private int? walkspeed;
        private int? flyingspeed;
        private int? burrowspeed;
        private int? swimspeed;
        private string type;
        private string size;
        private string alignment;
        private string senses;
        private string skills;
        private string savingThrows;
        private string resistances = "ResistancesPlaceholder";
        private List <string> immunities;
        private List<string> Languages;
        private List<MonsterAction> actionList;

        public Monster(string Name)
        {
            this.Name = Name;
            Languages = new List<string>();
            populateMonsterInfo();
        }

        //Copy constructor
        public Monster(Monster toCopy)
        {
            this.Name = toCopy.Name;
        }


        private void populateMonsterInfo()
        {
            string monsterQuery = "SELECT * from Monster WHERE Name = '" + this.Name + "'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adaptor = new SqlDataAdapter(monsterQuery, con))
                {
                    DataTable monsterInfo = new DataTable();
                    adaptor.Fill(monsterInfo);

                    DataTableReader infoReader = monsterInfo.CreateDataReader();
                    DataRow testRow = monsterInfo.Rows[0];

                    //fill basic monster info
                    int monsterID = (int)monsterInfo.Rows[0]["MonsterID"];
                    type = (string)monsterInfo.Rows[0]["Type"];
                    size = (string)monsterInfo.Rows[0]["Size"];
                    alignment = (string)monsterInfo.Rows[0]["Alignment"];
                    maxHP = (int)monsterInfo.Rows[0]["MaxHitPoints"];
                    currHP = maxHP;
                    ArmorClass = (int)monsterInfo.Rows[0]["AC"];

                    //fill speeds;
                    walkspeed = (int?)monsterInfo.Rows[0]["LandSpeed"];

                    //flyspeed
                    try
                    {
                        flyingspeed = int.Parse(monsterInfo.Rows[0]["FlySpeed"].ToString());
                    }
                    catch (Exception e)
                    {
                        flyingspeed = null;
                    }

                    try
                    {
                        swimspeed = int.Parse(monsterInfo.Rows[0]["SwimSpeed"].ToString());
                    }
                    catch (Exception e)
                    {
                        swimspeed = null;
                    }

                    try
                    {
                        burrowspeed = int.Parse(monsterInfo.Rows[0]["BurrowSpeed"].ToString());
                    }catch ( Exception e)
                    {
                        burrowspeed = null;
                    }

                    //fill senses/skills
                    senses = monsterInfo.Rows[0]["Senses"].ToString();
                    skills = monsterInfo.Rows[0]["Skills"].ToString();

                    //fill saving throws
                    savingThrows = monsterInfo.Rows[0]["SavingThrows"].ToString();

                    //fill Languages
                    string langaugeQuery = "SELECT Name From Languages as c LEFT JOIN Languages_xref as a ON((C.LanguageID = a.LanguageID)) WHERE a.MonsterID = '" + monsterID + "'";
                    using (SqlConnection languageConnection = new SqlConnection(connectionString))
                    {
                        using (SqlDataAdapter languageAdaptor = new SqlDataAdapter(langaugeQuery, languageConnection))
                        {
                            DataTable languageTable = new DataTable();
                            languageAdaptor.Fill(languageTable);

                            foreach(DataRow languageRow in languageTable.Rows)
                            {
                                Languages.Add(((string)languageRow["Name"]));
                            }
                        }
                    }

                    //fill immunities
                    immunities = new List<string>();
                    string ImminutiesQuery = "SELECT Name From DamageTypes as c LEFT JOIN DamageTypes_xref as a ON((C.DamageTypeID = a.DamageTypeID) AND a.ResistanceIndicator = '1') WHERE a.MonsterID = '" + monsterID + "'";
                    using (SqlConnection ImmunitiesConnection = new SqlConnection(connectionString))
                    {
                        using (SqlDataAdapter languageAdaptor = new SqlDataAdapter(ImminutiesQuery, ImmunitiesConnection))
                        {
                            DataTable immunitiesTable = new DataTable();
                            languageAdaptor.Fill(immunitiesTable);

                            foreach (DataRow languageRow in immunitiesTable.Rows)
                            {
                                immunities.Add((((string)languageRow["Name"])));
                            }
                        }
                    }
                    //fill actions
                    string actionQuery = "Select * From MonsterActions Where MonsterID = '" + monsterID + "'";
                    actionList = new List<MonsterAction>();
                    using (SqlConnection ActionsConnection = new SqlConnection(connectionString))
                    {
                        using (SqlDataAdapter actionsAdaptor = new SqlDataAdapter(actionQuery, ActionsConnection))
                        {
                            DataTable actionsTable = new DataTable();
                            actionsAdaptor.Fill(actionsTable);

                            foreach (DataRow actionRow in actionsTable.Rows)
                            {
                                string actionName = actionRow["Name"].ToString();
                                string actionDescription = actionRow["Description"].ToString();
                                bool isLegendary = (bool)actionRow["LegendaryIndicator"];
                                int numberUses;
                                try
                                {
                                    numberUses = (int)actionRow["Uses"];
                                }catch (Exception e)
                                {
                                    numberUses = 0;
                                }

                                MonsterAction toAdd = new MonsterAction(actionName, isLegendary, actionDescription, numberUses);
                                actionList.Add(toAdd);
                            }
                        }
                    }

                    //fill resistances

                    //fill baseStats
                    strength = (int)monsterInfo.Rows[0]["Strength"];
                    intelligence = (int)monsterInfo.Rows[0]["Intelligence"];
                    dexterity = (int)monsterInfo.Rows[0]["Dexterity"];
                    wisdom = (int)monsterInfo.Rows[0]["Wisdom"];
                    constitution = (int)monsterInfo.Rows[0]["Constitution"];
                    charisma = (int)monsterInfo.Rows[0]["Charisma"];


                    

                    con.Close();
                }

            }
        }

        //public getter functions for all monster attributes
        public string getName()
        {
            return Name;
        }

        public string getMonsterType()
        {
            return type;
        }

        public int getMaxHP()
        {
            return maxHP;
        }

        public int getStrength()
        {
            return strength;
        }

        public int getDexterity()
        {
            return dexterity;
        }

        public int getConstitution()
        {
            return constitution;
        }

        public int getWisdom()
        {
            return wisdom;
        }

        public int getCharisma()
        {
            return charisma;
        }

        public string getAlignment()
        {
            return alignment;
        }

        public string getSize()
        {
            return size;
        }

        public int getIntelligence()
        {
            return intelligence;
        }

        public List<string> getLanguages()
        {
            return Languages;
        }

        public int? getWalkSpeed()
        {
            return walkspeed;
        }

        public int? getSwimSpeed()
        {
            return swimspeed;
        }

        public int? getFlySpeed()
        {
            return flyingspeed;
        }

        public int? getBurrowSpeed()
        {
            return burrowspeed;
        }

        public string getSenses()
        {
            return senses;
        }

        public string getSkills()
        {
            return skills;
        }

        public string getSavingThrows()
        {
            return savingThrows;
        }

        public string getResistances()
        {
            return resistances;
        }

        public List<string> getImmunities()
        {
            return immunities;
        }

        public int getCurrentHP()
        {
            return currHP;
        }

        public void setCurrentHP( int toSet)
        {
            currHP = toSet;
        }

        public int getArmorClass()
        {
            return ArmorClass;
        }

        public List<MonsterAction> getActions()
        {
            return actionList;
        }

        //Tostring function
        public override string ToString()
        {
            return Name;
        }

    }
}
