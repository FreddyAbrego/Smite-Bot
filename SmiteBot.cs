using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace SmiteBot
{
    public class SmiteBot
    {
        public SmiteDev smiteDev { get; private set; } 
        public SQLiteToolBox SQLiteTB{ get; private set; } 
        public List<God> Gods { get; private set; }                 // used to hold list of gods from database or TODO: check once a day for new god from HiRez API, if any
        public List<Player> Players { get; private set; }           // used to hold list of searched players from database or to hold newly searched player TODO: update players from HiRezAPI on search

        public List<God> Hunters { get; private set; }              // holds hunter gods
        public List<God> Assassins { get; private set; }            // holds assassin gods
        public List<God> Guardians { get; private set; }            // holds guardian gods
        public List<God> Warriors { get; private set; }             // holds warrior gods
        public List<God> Mages { get; private set; }                // holds mage gods
        public List<God> FreeRotation  { get; private set; }        // holds gods currently in free rotation

        public List<string> roles { get; private set; }             // used to hold roles i.e. mid, support, adc, solo, jungle
        public List<string> classes { get; private set; }           // used to hold classes i.e. mage, hunter, warrior, guardian, assassin

        public bool godsChecked { get; set; } 

        // This creates a Singelton of the class
        // An object that only exists once
        // It is used by the Main Discord Class:
        //                to restart the session after 15 minutes
        //                to save the Gods and Players List to database on console close
        // It is also used by the Commands Class:
        //                  To access the Gods and Players Database
        //                  
        private static SmiteBot instance; 
        // The instance of object: only exists once
        // If the object already exists then return that object
        // Otherwise create an instance of te object
        public static SmiteBot Instance 
        {
            get
            {
                if (instance == null)
                {
                    instance = new SmiteBot();
                }
                return instance;
            }
        }
        private SmiteBot()
        {
            smiteDev = new SmiteDev();
            SQLiteTB = new SQLiteToolBox();
            Gods = new List<God>();
            Players = new List<Player>();

            Hunters = new List<God>();
            Assassins = new List<God>();
            Guardians = new List<God>();
            Warriors = new List<God>();
            Mages = new List<God>();

            FreeRotation = new List<God>();
            roles = new List<string>();
            classes = new List<string>();
            godsChecked = false;

            readFromGodDatabase();
            readFromPlayerDatabase();
            checkForNewGod(); // should set godsChecked = true;

            createGodLists();
            setupRolesClasses();
        }

        public void checkForNewGod()
        {
            if (godsChecked == false)
            {
                smiteDev.getGods(Gods);
                godsChecked = true;
            }
        }

        private void readFromGodDatabase()
        {
            DataTable dtbGods = new DataTable();
            string query = String.Format(@"SELECT * FROM Gods");
            SQLiteTB.FillDataTable(query, ref dtbGods);
            Gods = (from row in dtbGods.AsEnumerable()
                    select new God
                    {
                        Id = Convert.ToInt16(row["GodId"]),
                        Name = Convert.ToString(row["Name"]),
                        Title = Convert.ToString(row["Title"]),
                        Roles = Convert.ToString(row["Roles"]),
                        Type = Convert.ToString(row["Type"]),
                        Pantheon = Convert.ToString(row["Pantheon"]),
                        Lore = Convert.ToString(row["Lore"]),
                        FreeRotation = Convert.ToBoolean(row["FreeRotation"]),
                        GodIcon = Convert.ToString(row["GodIcon"])
                    }
                   ).ToList();
            
        }
        private void readFromPlayerDatabase()
        {
            DataTable dtbPlayers = new DataTable();
            string query = String.Format(@"SELECT * FROM Players");
            SQLiteTB.FillDataTable(query, ref dtbPlayers);
            Players = (from row in dtbPlayers.AsEnumerable()
                       select new Player
                       {
                           Id = Convert.ToInt32(row["PlayerId"]),
                           Name = Convert.ToString(row["Name"]),
                           Status = Convert.ToString(row["Status"]),
                           AccountCreated = Convert.ToString(row["AccountCreated"]),
                           LastLogin = Convert.ToString(row["LastLogin"]),
                           Level = Convert.ToInt32(row["Level"]),
                           Wins = Convert.ToInt32(row["Wins"]),
                           Losses = Convert.ToInt32(row["Losses"]),
                           Leaves = Convert.ToInt32(row["Leaves"]),
                           MasteryLevel = Convert.ToInt32(row["MasteryLevel"]),
                           Clan = Convert.ToString(row["Clan"]),
                       }
                   ).ToList();
        }
        public void saveToGodsDatabase()
        {
            foreach (God g in Gods)
            {
                // Used for SQL Statement to break the ' singlequote
                g.Lore = g.Lore.Replace("'", "''");
                if (g.Name == "Chang'e")
                    g.Name = g.Name.Replace("'", "''");
                string query = (String.Format(@"INSERT INTO Gods (
                                                    GodId, 
                                                    Name, 
                                                    Title, 
                                                    Roles, 
                                                    Type, 
                                                    Pantheon, 
                                                    Lore,
                                                    FreeRotation,
                                                    GodIcon) 
                                                SELECT {0},'{1}','{2}','{3}','{4}','{5}','{6}', {7},'{8}'
                                                WHERE NOT EXISTS(SELECT GodId FROM Gods WHERE GodId = {9})", g.Id, g.Name, g.Title, g.Roles, g.Type, g.Pantheon, g.Lore, (g.FreeRotation ? 1 : 0), g.GodIcon, g.Id));
                SQLiteTB.InsertInto(query);
            }
        }
        public void saveToPlayersDatabase()
        {
            foreach (Player p in Players)
            {
                if (p.checkedApi == true)
                {
                    string query = (String.Format(@"UPDATE Players 
                                                      SET
                                                      PlayerID = {0},  
                                                      Name = '{1}', 
                                                      Status = '{2}', 
                                                      AccountCreated = '{3}', 
                                                      LastLogin = '{4}', 
                                                      Level = {5}, 
                                                      Wins = {6},
                                                      Losses = {7},
                                                      Leaves = {8},
                                                      MasteryLevel = {9},
                                                      Clan = '{10}'
                                            WHERE PlayerId = {11};", p.Id, p.Name, p.Status, p.AccountCreated, p.LastLogin, p.Level, p.Wins, p.Losses, p.Leaves, p.MasteryLevel, p.Clan, p.Id));
                    SQLiteTB.Update(query);
                }
                if (p.newPlayer == true)
                {
                    string query = (String.Format(@"INSERT INTO Players (
                                                      PlayerID,
                                                      Name,
                                                      Status,
                                                      AccountCreated, 
                                                      LastLogin,
                                                      Level,
                                                      Wins,
                                                      Losses,
                                                      Leaves,
                                                      MasteryLevel,
                                                      Clan)
                                            SELECT {0},'{1}','{2}','{3}','{4}',{5},{6},{7},{8},{9},'{10}'
                                            WHERE NOT EXISTS (SELECT PlayerID FROM Players WHERE PlayerId = {11})", p.Id, p.Name, p.Status, p.AccountCreated, p.LastLogin, p.Level, p.Wins, p.Losses, p.Leaves, p.MasteryLevel, p.Clan, p.Id));
                    SQLiteTB.Update(query);
                }
            }
        }
       
        public string getPatchNotes()
        {
            return "";
        }

       private void createGodLists()
        {
            Hunters.AddRange(findGods(" Hunter"));
            Assassins.AddRange(findGods(" Assassin"));
            Guardians.AddRange(findGods(" Guardian"));
            Warriors.AddRange(findGods(" Warrior"));
            Mages.AddRange(findGods(" Mage"));
        }
        private void setupRolesClasses()
        {
            roles.Add("ADC");
            roles.Add("Jungle");
            roles.Add("Middle Lane");
            roles.Add("Solo Lane");
            roles.Add("Support");

            classes.Add("Hunter");
            classes.Add("Assassin");
            classes.Add("Guardian");
            classes.Add("Warrior");
            classes.Add("Mage");
        }
        private Player findPlayer(string playerName)
        {
            string searchedPlayerName;
            string regex = @"\[.*\]";
            Player result = new Player();
            // searches through the players list if found to return
            for (int row = 0; row < Players.Count; row++)
            {
                searchedPlayerName = Regex.Replace(Players[row].Name, regex, "");
                if (searchedPlayerName.ToLower().Equals(playerName.ToLower()))
                {
                    result = Players[row];
                    return result;
                }
            }
            // otherwise get the player from smite api
            result = smiteDev.getPlayer(playerName, Players);
            return result;
        }
        // used to make the List for classes and gets free rotation
       
        private List<God> findGods(string godclass)
        {         
            List<God> results = new List<God>();
            for (int row = 0; row < Gods.Count; row++)
            {
                if (Gods[row].Roles.Equals(godclass))
                {
                    results.Add(Gods[row]);
                    if (Gods[row].FreeRotation)
                        FreeRotation.Add(Gods[row]);
                }
            }
            return results;
        }
    }
}
