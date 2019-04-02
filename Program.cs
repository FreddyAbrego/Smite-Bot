using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
    public class Program
    {
        public static DiscordSocketClient client;
        public static CommandService commands { get; set; }
        public SmiteBot smiteBot = SmiteBot.Instance;

        public Timer SessionTimer = new Timer(900000); // 900000 ms = 15 minutes
        public Timer CheckDataBase = new Timer(86400000); // 86400000 ms = 24 Hours
        public static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();         // connect to discord
            commands = new CommandService();            // stores commands to be used
            await InstallCommands();    
            client.Log += Log;
            //Your bot's token goes here
            string token = "";

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            
            SessionTimer.Start();
            CheckDataBase.Start();

            SessionTimer.Elapsed += restartSession;
            CheckDataBase.Elapsed += resetCheck;
            ConsoleEventHooker.Closed += ConsoleEventHooker_Closed;
            await Task.Delay(-1);
            var d = client.Guilds;
            foreach (var guild in d)
            {
                Console.WriteLine(guild.Name);
            }
        }

        // Resets the Daily check for Each Player 
        // and for the god list
        // If new god is released it will check the API
        // Then see the new god and add it to the List
        // Player check gets reset daily to get updated stats
        // If they player hasn't done .player (playername) 
        // it won't update the player
        // Will keep Request Limit Low
        private void resetCheck(object sender, ElapsedEventArgs e)
        {
            foreach (Player p in smiteBot.Players)
            {
                p.checkedApi = false;
            }
            smiteBot.godsChecked = false;
        }
        private void restartSession(object sender, ElapsedEventArgs e)
        {
            smiteBot.smiteDev.createSession();
            smiteBot.checkForNewGod();
        }
        private void ConsoleEventHooker_Closed(object sender, EventArgs e)
        {
            smiteBot.saveToGodsDatabase();
            smiteBot.saveToPlayersDatabase();
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
        private async Task HandleCommand(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;
            int pos = 0;

            if (msg.HasCharPrefix('.', ref pos))
            {
                var context = new CommandContext(client, msg);
                var result = await commands.ExecuteAsync(context, pos);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    Console.WriteLine(result.ErrorReason);
            }
            else
            {
                CommandContext context = new CommandContext(client, msg);
                await HandleMessage(context);
            }
        }
        public async Task HandleMessage(CommandContext context)
        {
            //MessageReceived Handler
        }
        private Task Log(LogMessage logmessage)
        {
            Console.WriteLine(logmessage.ToString());
            return Task.CompletedTask;
        }
    }
    public class Commands : ModuleBase
    {
        public SmiteBot smiteBot = SmiteBot.Instance;
        Random rand = new Random();
        
        [Command("god")]
        [Alias("rg")]
        [Summary("Chooses a god from all gods")]
        public async Task God()
        {
            int randomGodIndex = rand.Next(smiteBot.Gods.Count);
            God randomGod = smiteBot.Gods[randomGodIndex];
            var eb = new EmbedBuilder();
            eb.WithDescription(Context.User.Mention + " " + randomGod.Name + " has been chosen.");
            await Context.Channel.SendMessageAsync("", false, eb);
        }

        [Command("hunter")]
        [Alias("rh")]
        [Summary("Chooses a hunter from all gods")]
        public async Task Hunter()
        {
            int randomGodIndex = rand.Next(smiteBot.Hunters.Count);
            God randomGod = smiteBot.Hunters[randomGodIndex];
            var eb = new EmbedBuilder();
            eb.WithDescription(Context.User.Mention + " " + randomGod.Name + " has been chosen.");
            await Context.Channel.SendMessageAsync("", false, eb);
        }

        [Command("assassin")]
        [Alias("ra")]
        [Summary("Chooses a assassin from all gods")]
        public async Task Assassin()
        {
            int randomGodIndex = rand.Next(smiteBot.Assassins.Count);
            God randomGod = smiteBot.Assassins[randomGodIndex];
            var eb = new EmbedBuilder();
            eb.WithDescription(Context.User.Mention + " " + randomGod.Name + " has been chosen.");
            await Context.Channel.SendMessageAsync("", false, eb);
        }

        [Command("guardian")]
        [Alias("rg")]
        [Summary("Chooses a guardian from all gods")]
        public async Task Guardian()
        {
            int randomGodIndex = rand.Next(smiteBot.Guardians.Count);
            God randomGod = smiteBot.Guardians[randomGodIndex];
            var eb = new EmbedBuilder();
            eb.WithDescription(Context.User.Mention + " " + randomGod.Name + " has been chosen.");
            await Context.Channel.SendMessageAsync("", false, eb);
        }

        [Command("warrior")]
        [Alias("rw")]
        [Summary("Chooses a warrior from all gods")]
        public async Task Warrior()
        {
            int randomGodIndex = rand.Next(smiteBot.Warriors.Count);
            God randomGod = smiteBot.Warriors[randomGodIndex];
            var eb = new EmbedBuilder();
            eb.WithDescription(Context.User.Mention + " " + randomGod.Name + " has been chosen.");
            await Context.Channel.SendMessageAsync("", false, eb);
        }

        [Command("mage")]
        [Alias("rm")]
        [Summary("Chooses a mage from all gods")]
        public async Task Mage()
        {
            int randomGodIndex = rand.Next(smiteBot.Mages.Count);
            God randomGod = smiteBot.Mages[randomGodIndex];
            var eb = new EmbedBuilder();
            eb.WithDescription(Context.User.Mention + " " + randomGod.Name + " has been chosen.");
            await Context.Channel.SendMessageAsync("", false, eb);
        }

        [Command("role")]
        [Alias("rr")]
        [Summary("Chooses a role for you")]
        public async Task Role()
        {
            int randomRoleIndex = rand.Next(smiteBot.roles.Count);
            string randomRole = smiteBot.roles[randomRoleIndex];
            await Context.Channel.SendMessageAsync(Context.User.Mention + " " + randomRole + " has been chosen.");
        }

        [Command("class")]
        [Alias("rc")]
        [Summary("Chooses a class for you")]
        public async Task GetClass()
        {
            int randomClassIndex = rand.Next(smiteBot.classes.Count);
            string randomClass = smiteBot.classes[randomClassIndex];
            await Context.Channel.SendMessageAsync(Context.User.Mention + " " + randomClass + " has been chosen.");
        }

        [Command("rotation")]
        [Alias("freerotation")]
        [Summary("Shows which gods are currently in free rotation")]
        public async Task Rotation()
        {
            string rotationString = "```\r\n";
            rotationString += "Current Gods in rotation:\r\n";
            foreach (God g in smiteBot.FreeRotation)
                rotationString += g.Name + "\r\n";
            rotationString += "```";
            await Context.Channel.SendMessageAsync(rotationString);
        }

        [Command("player")]
        [Summary("Find Player and return results: .player (player name)")]
        public async Task Player([Remainder] string playerName)
        {
            string playerString = "```";
            Player foundPlayer = new Player();
            foundPlayer = findPlayer(playerName);
            if (foundPlayer.Id == 0)
                playerString += "Player Not Found\r\n```";
            else
            {
                foreach (PropertyInfo prop in typeof(Player).GetProperties())
                {
                    if (!(prop.Name == "Id" || prop.Name == "checkedAPI"))
                        playerString += String.Format("{0}: {1}\r\n", prop.Name, prop.GetValue(foundPlayer, null));
                }
                playerString += "```";
            }
            await Context.Channel.SendMessageAsync(playerString);
        }

        [Command("help")]
        [Summary("Show other commands")]
        public async Task Help()
        {
            string helpMessage = "```.god  - Chooses a god from all gods \r\n";
            helpMessage += ".hunter - Chooses a hunter from all gods \r\n";                          
            helpMessage += ".assassin - Chooses an assassin from all gods \r\n";
            helpMessage += ".guardian - Chooses a guardian fraom all gods \r\n";
            helpMessage += ".warrior - Chooses a warrior from all gods \r\n";
            helpMessage += ".mage - Chooes a mage from all gods \r\n";
            helpMessage += ".rotation - Shows Gods currently in free rotation \r\n";
            helpMessage += ".player (player name) search for player and show stats \r\n";
            helpMessage += ".role - Chooses role: Mid, Solo, Jungle, Support, Adc \r\n";
            helpMessage += ".class - Chooses a class: Mage, Guardian, Assassin, Hunter, Warrior\r\n";
            helpMessage += "```";
            await Context.Channel.SendMessageAsync(helpMessage); 
        }

        // used to test db connection
        [Command("save")]
        public async Task Save()
        {
            smiteBot.saveToGodsDatabase();
            smiteBot.saveToPlayersDatabase();
            await Task.CompletedTask;
        }

        // Functions for commands to use
        private Player findPlayer(string playerName)
        {
            string searchedPlayerName;
            string regex = @"\[.*\]";
            for (int row = 0; row < smiteBot.Players.Count; row++)
            {
                searchedPlayerName = Regex.Replace(smiteBot.Players[row].Name, regex, "");
                if (searchedPlayerName.ToLower().Equals(playerName.ToLower()))
                {
                    if (smiteBot.Players[row].checkedApi == false)
                    {
                        smiteBot.smiteDev.updatePlayer(smiteBot.Players[row]);
                        return smiteBot.Players[row];
                    }
                    else
                    {
                        return smiteBot.Players[row];
                    }
                }
            }
            return smiteBot.smiteDev.getPlayer(playerName, smiteBot.Players);
        }
        
