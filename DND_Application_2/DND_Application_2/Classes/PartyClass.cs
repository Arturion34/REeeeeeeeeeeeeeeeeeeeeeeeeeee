using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND_Application_2
{
    public class PartyClass
    {
        private List<PlayerClass> partyMembers;

        public PartyClass()
        {
            partyMembers = new List<PlayerClass>();
        }

        public List<PlayerClass> getPartyMembers()
        {
            return partyMembers;
        }

        public void addPartyMember(PlayerClass toAdd)
        {
            partyMembers.Add(toAdd);
        }

        public void removePartyMember(PlayerClass toRemove)
        {
            partyMembers.Remove(toRemove);
        }
    }


}
