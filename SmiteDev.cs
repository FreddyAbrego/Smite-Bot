using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace SmiteBot
{
    public class SmiteDev
    {
        // Enter your devID and authKey from the smite api developer
        private static string devID = "";
        private static string authKey = "";
        private static string timeStamp;
        private static string urlPrefix = "http://api.smitegame.com/smiteapi.svc/";
        // holds session id for HiRez API
        public static string session { get; set; }
        private string signature { get; set; }
        public SmiteDev()
        {
            createSession();
        }
        
        public Player getPlayer(string searchedPlayer, List<Player> Players)
        {
            timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string signature = GetMD5Hash(devID + "getplayer" + authKey + timeStamp);            
            WebRequest request = WebRequest.Create(urlPrefix + "getplayerjson/" + devID + "/" + signature + "/" + session + "/" + timeStamp + "/" + searchedPlayer);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            // get more things to return later on
            using (var web = new WebClient())
            {
                web.Encoding = System.Text.Encoding.UTF8;
                var jsonString = responseFromServer;
                jsonString = jsonString.TrimStart('[');
                jsonString = jsonString.TrimEnd(']');

                var jss = new JavaScriptSerializer();

                JToken token = JObject.Parse(jsonString);
                Player player = new Player();
                player.Id = (int)token.SelectToken("Id");
                player.Name = (string)token.SelectToken("Name");
                player.Status = (string)token.SelectToken("Personal_Status_Message");

                DateTime date = Convert.ToDateTime((string)token.SelectToken("Created_Datetime"));
                date = date.ToLocalTime();
                player.AccountCreated = date.ToString();                
                
                date = Convert.ToDateTime(String.Format((string)token.SelectToken("Last_Login_Datetime")));
                date = date.ToLocalTime();
                player.LastLogin = date.ToString();       

                player.Level = (int)token.SelectToken("Level");
                player.Wins = (int)token.SelectToken("Wins");
                player.Losses = (int)token.SelectToken("Losses");
                player.Leaves = (int)token.SelectToken("Leaves");
                player.MasteryLevel = (int)token.SelectToken("MasteryLevel");
                player.Clan = (string)token.SelectToken("Team_Name");
                player.newPlayer = true;

                if (!(player.Id == 0))
                    Players.Add(player);
                return player;
            }
        }
        public void updatePlayer(Player player)
        {
            string regex = @"\[.*\]";
            string searchedPlayer = Regex.Replace(player.Name, regex, "");

            timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string signature = GetMD5Hash(devID + "getplayer" + authKey + timeStamp);
            WebRequest request = WebRequest.Create(urlPrefix + "getplayerjson/" + devID + "/" + signature + "/" + session + "/" + timeStamp + "/" + searchedPlayer);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            // get more things to return later on
            using (var web = new WebClient())
            {
                web.Encoding = System.Text.Encoding.UTF8;
                var jsonString = responseFromServer;
                jsonString = jsonString.TrimStart('[');
                jsonString = jsonString.TrimEnd(']');

                var jss = new JavaScriptSerializer();

                JToken token = JObject.Parse(jsonString);

                player.Id = (int)token.SelectToken("Id");
                player.Name = (string)token.SelectToken("Name");
                player.Status = (string)token.SelectToken("Personal_Status_Message");

                DateTime date = Convert.ToDateTime((string)token.SelectToken("Created_Datetime"));
                date = date.ToLocalTime();
                player.AccountCreated = date.ToString();

                date = Convert.ToDateTime(String.Format((string)token.SelectToken("Last_Login_Datetime")));
                date = date.ToLocalTime();
                player.LastLogin = date.ToString();                

                player.conquestRank = getRank(player, (int)token.SelectToken("Tier_Conquest"));
                player.joustRank = getRank(player, (int)token.SelectToken("Tier_Joust"));
                player.duelRank = getRank(player, (int)token.SelectToken("Tier_Conquest"));

                string playerIcon = (string)token.SelectToken("Avatar_URL");

                player.Level = (int)token.SelectToken("Level");
                player.Wins = (int)token.SelectToken("Wins");
                player.Losses = (int)token.SelectToken("Losses");
                player.Leaves = (int)token.SelectToken("Leaves");
                player.MasteryLevel = (int)token.SelectToken("MasteryLevel");
                player.Clan = (string)token.SelectToken("Team_Name");
                player.checkedApi = true;
            }
        }
        public string getRank(Player p, int rank)
        {
            switch (rank)
            {
                case 0: return "Qualifying";
                case 1: return "Bronze V";
                case 2: return "Bronze IV";
                case 3: return "Bronze III";
                case 4: return "Bronze II";
                case 5: return "Bronze I";
                case 6: return "Silver V";
                case 7: return "Silver IV";
                case 8: return "Silver III";
                case 9: return "Silver II";
                case 10: return "Silver I";
                case 11: return "Gold V";
                case 12: return "Gold IV";
                case 13: return "Gold III";
                case 14: return "Gold II";
                case 15: return "Gold I";
                case 16: return "Platinum V";
                case 17: return "Platinum IV";
                case 18: return "Platinum III";
                case 19: return "Platinum II";
                case 20: return "Platinum I";
                case 21: return "Diamond V";
                case 22: return "Diamond IV";
                case 23: return "Diamond III";
                case 24: return "Diamond II";
                case 25: return "Diamond I";
                case 26: return "Master";
                case 27: return "Grandmaster";
            }
            return "";
        }
        public void getGods(List<God> Gods)
        {
            Gods.Clear();
            timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string signature = GetMD5Hash(devID + "getgods" + authKey + timeStamp);
            string languageCode = "1";            
            WebRequest request = WebRequest.Create(urlPrefix + "getgodsjson/" + devID + "/" + signature + "/" + session + "/" + timeStamp + "/" + languageCode);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            using (var web = new WebClient())
            {
                web.Encoding = System.Text.Encoding.UTF8;
                var jsonString = responseFromServer;
                // Trims [] from entire string
                // When using JObject.Parse(str) below these [] caused issues
                jsonString = jsonString.TrimStart('[');
                jsonString = jsonString.TrimEnd(']');
                // end trim

                // As it is an entire god list with no delimiter one is needed
                // ??? is choosen as delimeter as it does not exist otherwise in json string
                jsonString = jsonString.Replace("null},", "null}???");
                // sets delimeter
                string[] delimiter = { "???" };
                // splits the string into a List of strings containing each god's data
                List<string> godStr = jsonString.Split(delimiter, StringSplitOptions.None).ToList();
                // checks for the amount of Gods and godStr, if new god is released then the foreach will run
                if (godStr.Count != Gods.Count) 
                // creates god object for each god
                    foreach (string str in godStr)
                    {
                        JToken token = JObject.Parse(str);
                        God god = new God();
                        god.Id = (int)token.SelectToken("id");
                        god.Name = (string)token.SelectToken("Name");
                        god.Title = (string)token.SelectToken("Title");
                        god.Roles = (string)token.SelectToken("Roles");
                        god.Type = (string)token.SelectToken("Type");
                        god.Pantheon = (string)token.SelectToken("Pantheon");
                        god.Lore = (string)token.SelectToken("Lore");
                        string fr = (string)token.SelectToken("OnFreeRotation");
                        if (fr == "true")
                            god.FreeRotation = true;
                        // if already exists in List (used for checking for new gods)
                        // do not add to god list : does not allow duplicates
                        if (!Gods.Contains(god) && god.Id != 0)
                            Gods.Add(god);
                        else
                            god = null; // temporary god will be set to destroy
                        
                        god.GodIcon = System.Text.RegularExpressions.Regex.Replace((string)token.SelectToken("godIcon_URL"), "\\\\", String.Empty);
                       
                    }
            }
        }

        private static string GetMD5Hash(string input)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(input);
            bytes = md5.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2").ToLower());
            }
            return sb.ToString();
        }

        public void createSession()
        {
            timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            signature = GetMD5Hash(devID + "createsession" + authKey + timeStamp);
            Console.WriteLine("New session created " + DateTime.Now.ToString());
            WebRequest request = WebRequest.Create(urlPrefix + "createsessionjson/" + devID + "/" + signature + "/" + timeStamp);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            using (var web = new WebClient())
            {
                web.Encoding = System.Text.Encoding.UTF8;
                var jsonString = responseFromServer;
                var jss = new JavaScriptSerializer();
                var g = jss.Deserialize<SessionInfo>(jsonString);

                session = g.session_id;
            }
        }
    }

    public class SessionInfo
    {
        public string ret_msg { get; set; }
        public string session_id { get; set; }
        public string timestamp { get; set; }
    }


}
