using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND_Application_2
{
    public class PlayerClass
    {
        private string playerName;
        private int playerLevel;
        private string characterName;

        public PlayerClass()
        {
            playerName = null;
            playerLevel = 0;
            characterName = null;
        }

        public PlayerClass(string Name, int level, string charName)
        {
            playerLevel = level;
            playerName = Name;
            characterName = charName;
        }

        public int getPlayerLevel()
        {
            return playerLevel;
        }

        public string getPlayerName()
        {
            return playerName;
        }

        public string getCharacterName()
        {
            return characterName;
        }

        public void setCharacterName(string toSet)
        {
            characterName = toSet;
        }

        public void setCharacterLevel(int toSet)
        {
            playerLevel = toSet;
        }

        public void setPlayerName(string toSet)
        {
            playerName = toSet;
        }

        public override string ToString()
        {
            return characterName;
        }
    }
}
