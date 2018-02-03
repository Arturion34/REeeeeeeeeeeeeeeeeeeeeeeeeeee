using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND_Application_2
{
    
    public class Monster
    {
        private int maxHP;
        string Name;
        private int strength;
        private int dexterity;
        private int charisma;
        private int wisdom;
        private int constitution;
        private int intelligence;
        private string type;
        private string size;
        private string alignment;
        private List<string> Languages;

        public Monster(string Name)
        {
            this.Name = Name;
        }

        //Copy constructor
        public Monster(Monster toCopy)
        {
            this.Name = toCopy.Name;
        }

        public string getName()
        {
            return Name;
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
