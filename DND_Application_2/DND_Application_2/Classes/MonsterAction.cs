using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND_Application_2.Classes
{
    public class MonsterAction
    {
        //base parameters
        //actionName = name of action
        //isLegendary = legendary identifier
        //description = description of action and what happens
        //numberUses = the number of times an action may be used (0 if no limit)
        private string actionName;
        private bool isLegendary;
        private string description;
        private int numberUses;

        public MonsterAction()
        {
            actionName = "";
            isLegendary = false;
            description = "";
            numberUses = 0;
        }

        public MonsterAction(string newActionName, string newActionDescription)
        {
            actionName = newActionName;
            description = newActionDescription;
            isLegendary = false;
            numberUses = 0;
        }

        public MonsterAction(string newActionName, bool legendaryToggle, string newActionDescription, int actionNumberUses)
        {
            actionName = newActionName;
            isLegendary = legendaryToggle;
            description = newActionDescription;
            numberUses = actionNumberUses;
        }

        //getters and setters
        public string getActionName()
        {
            return actionName;
        }

        public bool getIsLegendary()
        {
            return isLegendary;
        }

        public string getActionDescription()
        {
            return description;
        }

        public int getNumberUses()
        {
            return numberUses;
        }

        public void setIsLegendary(bool toSet)
        {
            isLegendary = toSet;
        }

        public void setNumberUses(int toSet)
        {
            numberUses = toSet;
        }

        public void setActionName(string toSet)
        {
            actionName = toSet;
        }

        public void setActionDescription(string toSet)
        {
            description = toSet;
        }
    }
}
