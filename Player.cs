using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmiteBot
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string AccountCreated { get; set; }
        public string LastLogin { get; set; }
        public int Level { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Leaves { get; set; }
        public int MasteryLevel { get; set; }
        public string Clan { get; set; }
        public bool checkedApi = false;
        public bool newPlayer = false;
        public string conquestRank;
        public string duelRank;
        public string joustRank;
    }
}
