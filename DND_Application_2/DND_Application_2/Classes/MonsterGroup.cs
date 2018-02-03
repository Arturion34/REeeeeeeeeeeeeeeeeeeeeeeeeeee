using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND_Application_2.Classes
{
    class MonsterGroup
    {
        List<Monster> listOfMonsters;
        Monster thisMonsterGroup;
        int count;

        //default contstructor
        public MonsterGroup()
        {
            this.listOfMonsters = null;
            this.thisMonsterGroup = null;
            this.count = 0;
        }

        //constructor with group and count
        //creates a group of the same monster, thisGroup
        //count is the number of monsters in the group
        public MonsterGroup(Monster thisGroup, int count)
        {
            listOfMonsters = new List<Monster>();
            for(int i = 0; i < count; i++)
            {
                listOfMonsters.Add(new Monster(thisGroup.getName()));
            }
            this.count = count;
            this.thisMonsterGroup = thisGroup;
        }

        public int getCount()
        {
            return count;
        }

        public Monster getGroupMonster()
        {
            return thisMonsterGroup;
        }

        //toString Override
        public override string ToString()
        {
            return thisMonsterGroup.getName() + " (" + count + ")";
        }
    }
}
