using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPR_Roller;
using System.Threading.Tasks;
using System.Reflection;
using Google.Apis.Sheets.v4;
using System.Text.RegularExpressions;
using Google.Apis.Sheets.v4.Data;
using System.Text.Unicode;
using Discord;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace InteractionFramework.Modules
{
    public class CharacterModules : InteractionModuleBase<SocketInteractionContext>
    {

        public InteractionService Commands { get; set; }

        private InteractionHandler _handler;
        private SheetsService _gsheetService;
        private Dictionary<string, List<int>> guns = new Dictionary<string, List<int>>();

        // Constructor injection is also a valid way to access the dependencies
        public CharacterModules(InteractionHandler handler)
        {
            _handler = handler;
            _gsheetService = new GoogleSheetsHelper().Service;

            //Index 0 is the guns damagein D6s
            //assault rifle

            guns.Add("pistol", new List<int>()
            {
                13, 15, 20, 25, 30, 30, 10000, 10000
            });
            guns.Add("smg", new List<int>()
            {
                15, 13, 15, 20, 25, 25, 10000, 10000
            });
            guns.Add("shotgun", new List<int>()
            {
                13, 15, 20, 25, 30, 35, 10000, 10000
            });
            guns.Add("assaultrifle", new List<int>()
            {
                17, 16, 15, 13, 15, 20, 25, 30
            });
            guns.Add("sniperrifle", new List<int>()
            {
                30, 25, 25, 25, 15, 16, 17, 20
            });
            guns.Add("bowcrossbow", new List<int>()
            {
                15, 13, 15, 17, 20, 22, 1000, 1000
            });
            guns.Add("grenadelauncher", new List<int>()
            {
                16, 15, 15, 17, 20, 22, 25, 1000
            });
            guns.Add("rocketlauncher", new List<int>()
            {
                17, 16, 15, 15, 20, 20, 25, 30
            });
        }

        [SlashCommand("create-character", "Creates a character")]
        public async Task CreateCharacter([Summary(description: "The characters name")] string name,
            [Summary(description: "A link to the gsheet")]string gsheet)
        {
            //This command can take a bit, so let's not have it timeout immediately.
            RequestOptions options = new RequestOptions();
            options.Timeout = 25000;
            await DeferAsync(false, options);
            //Get player ID
            ulong playerId = Context.User.Id;

            //Get spreadsheet Id
            Regex idSplitter = new Regex(@"https:\/\/docs\.google\.com\/spreadsheets\/d\/(.*)\/(.*)");
            Match match = idSplitter.Match(gsheet);
            string spreadsheetId = match.Groups[1].Value;
            if (spreadsheetId != "")
            {
                //Get the char sheet
                var charSheet = GetSpreadsheet(spreadsheetId).Sheets[0];

                //Create a grid to more easily work with the sheet
                List<List<string>> table = new List<List<string>>();
                for(int c = 0; c < 20; c++)
                {
                    table.Add(new List<string>());
                    for(int r = 0; r < 90; r++)
                    {
                        table[c].Add(new string(""));
                    }
                }

                //r for row
                for(int r = 0; r < charSheet.Data[0].RowData.Count; r++)
                {
                    for(int c = 0; c < charSheet.Data[0].RowData[r].Values.Count; c++)
                    {
                        table[c][r] = (charSheet.Data[0].RowData[r].Values[c].FormattedValue ?? "NULL");
                    }
                }
                Character newChar = SetCharacterValues(table, Context.User.Id.ToString());

                await FollowupAsync("Creating a new character named '" + name +
                    "' for " + Context.User.Username + ". Name on sheet is " + table[0][1]);
            }
            else
            {
                await FollowupAsync("Invalid gsheet link");
            }
        }


        [SlashCommand("sr", "Rolls a skill")]
        public async Task RollSkill(string skill, int bonus)
        {
            Character curChar = GetCharacter(Context.User.Id.ToString());
            Random rand = new Random();
            //Get the character we're using based on ID from the cache
            
            //normalize the input
            skill = skill.ToUpper();
            string foundSkill = "";
            if(curChar.variables.TryGetValue(skill, out foundSkill)) { }
            //Find the skill. 

            try
            {
                int skillBonus = int.Parse((string)(typeof(Character).GetProperty(foundSkill).GetValue(curChar, null)));
                int diceResult = rand.Next(1, 11);
                string outputString = "";
                int crit = 0;
                outputString += String.Format("{0}: (D10) {1} + {2} + {3}", curChar.variables[skill], diceResult, skillBonus, bonus);
                if (diceResult == 1)
                {
                    crit = rand.Next(-1, -11);
                    outputString += " - (D10)" + crit;
                }
                else if (diceResult == 10)
                {
                    crit = rand.Next(1, 11);
                    outputString += " + (D10)" + crit;
                }
                outputString += String.Format(" = {0}", diceResult + bonus + skillBonus + crit);
                await RespondAsync(outputString);
                //Gets the value based on an exact match of the variable name

            }catch(Exception ex)
            {
                await RespondAsync("Exception thrown: " + ex.Message);
            }
        }

        [SlashCommand("mattack", "Rolls an attack.")]
        public async Task mattack(string gunName, int bonus)
        {
            Random rand = new Random();
            List<int> ranges = new List<int>();
            gunName = gunName.ToLower();
            List<int> damageRolls = new List<int>();
            int damageDice = 0;

            switch (gunName)
            {
                case "ar":
                case "assault rifle":
                    ranges = guns["assaultrifle"];
                    gunName = "Assault Rifle";
                    damageDice = 5;
                    break;
                case "mediumpistol":
                case "mpistol":
                    ranges = guns["pistol"];
                    gunName = "Medium Pistol";
                    damageDice = 2;
                    break;
                case "heavypistol":
                case "hpistol":
                    ranges = guns["pistol"];
                    gunName = "Heavy Pistol";
                    damageDice = 3;
                    break;
                case "veryheavypistol":
                case "vhpistol":
                case "vhp":
                    ranges = guns["pistol"];
                    gunName = "Very Heavy Pistol";
                    damageDice = 4;
                    break;
                case "smg":
                    ranges = guns["smg"];
                    gunName = "Submachine Gun";
                    damageDice = 2;
                    break;
                case "hsmg":
                    ranges = guns["smg"];
                    gunName = "Heavy Submachine Gun";
                    damageDice = 3;
                    break;
                case "shotgun":
                    ranges = guns["shotgun"];
                    gunName = "Shotgun";
                    damageDice = 5;
                    break;
                case "sniper":
                case "sniperrifle":
                    ranges = guns["sniperrifle"];
                    gunName = "Sniper Rifle";
                    damageDice = 5;
                    break;
                case "bow":
                case "crossbow":
                    ranges = guns["bowandcrossbow"];
                    gunName = "why are you using this weapon";
                    damageDice = 4;
                    break;
                case "grenadelauncher":
                    ranges = guns["grenadelauncher"];
                    gunName = "Grenade Launcher";
                    damageDice = 6;
                    break;
                case "rocketlauncher":
                case "rpg":
                    ranges = guns["rocketlauncher"];
                    gunName = "Rocket Launcher";
                    damageDice = 8;
                    break;

            }
            for (int i = 0; i < damageDice; i++)
            {
                damageRolls.Add(rand.Next(1, 7));
            }
            int hitRoll = rand.Next(1, 11) + bonus;
            string damageRollsString = "";
            for (int i = 0; i < damageRolls.Count; i++)
            {
                if (damageRolls[i] == 6)
                {
                    damageRollsString += "**6**";
                }
                else
                {
                    damageRollsString += damageRolls[i];
                }
                if (i != damageRolls.Count - 1)
                {
                    damageRollsString += ", ";
                }
            }
            string hitString = "";
            hitString += "(D10)";
            if (hitRoll - bonus == 1)
            {
                int critFail = rand.Next(1, 11);
                hitString += "**1**+" + bonus + "-(D10)" + critFail;
                hitRoll -= critFail;
            }
            else if (hitRoll - bonus == 10)
            {
                int critSuccess = rand.Next(1, 11);
                hitString += "**10**+" + bonus + "+(D10)" + critSuccess;
                hitRoll += critSuccess;
            }
            else
            {
                hitString += hitRoll - bonus + "+" + bonus;
            }
            await RespondAsync(String.Format("{0}: {1} = {2}\n" +
                "Ranges: {3} | {4} | {5} | {6} | {7}\n" +
                "Damage: **{10}** {8}d6({9})"
                , gunName, hitString, hitRoll,
                hitRoll < ranges[0] ? "~~0-6~~" : "**0-6**",
                hitRoll < ranges[1] ? "~~7-12~~" : "**7-12**",
                hitRoll < ranges[2] ? "~~13-25~~" : "**13-25**",
                hitRoll < ranges[3] ? "~~26-50~~" : "**26-50**",
                hitRoll < ranges[4] ? "~~51-100~~" : "**51-100**",
                damageDice, damageRollsString, damageRolls.Sum()
                )); ;
        }

        [SlashCommand("attack", "Rolls an attack using your attack modifier")]
        public async Task attack(string gunName, int bonus = 0)
        {

            int skillBonus = -1;

            switch (gunName)
            {
                case "ar":
                case "assault rifle":

                case "shotgun":

                case "sniper":
                case "sniperrifle":
                    skillBonus = int.Parse(GetCharacter(Context.User.Id.ToString()).ShoulderArms);
                    break;
                //pistols
                case "mediumpistol":
                case "mpistol":
                case "heavypistol":
                case "hpistol":
                case "veryheavypistol":
                case "vhpistol":
                case "vhp":
                case "smg":
                case "hsmg":
                    skillBonus = int.Parse(GetCharacter(Context.User.Id.ToString()).Handgun);
                    break;
                case "bow":
                    skillBonus = int.Parse(GetCharacter(Context.User.Id.ToString()).Archery);
                    break;
                case "grenadelauncher":
                case "rocketlauncher":
                case "rpg":
                    skillBonus = int.Parse(GetCharacter(Context.User.Id.ToString()).HeavyWeapons);
                    break;
            }
            await mattack(gunName, skillBonus + bonus);
        }
        
        private Character GetCharacter(string playerId)
        {
            Character curChar = null;
            if (!_handler._cachedCharacters.TryGetValue(Context.User.Id.ToString(), out curChar))
            {
                string json = File.ReadAllText(Context.User.Id.ToString() + ".json");
                curChar = JsonSerializer.Deserialize<Character>(json);
                _handler._cachedCharacters.Add(curChar.PlayerId, curChar);
            }
            return curChar;
        }
        private Character SetCharacterValues(List<List<string>> list, string playerId) 
        { 
            Character character = new Character();
            character.PlayerId = playerId;
            character.Name = list[0][1];
            character.Alias = list[1][1];
            character.Role = list[2][1];

            character.Eurobucks = list[6][1];
            character.InfluencePoints = list[6][2];
            character.Reputation = list[6][3];

            character.BODY = list[1][12];
            character.DEX = list[1][13];
            character.MOVE = list[1][14];
            character.REF = list[1][15];
            character.TECH = list[1][16];
            character.INT = list[1][17];
            character.WILL = list[1][18];
            character.EMP = list[1][19];
            character.COOL = list[1][20];
            character.LUCK = list[1][4]; //this is looking at max luck, not current luck

            character.CurHP = list[3][25];
            character.MaxHP = list[5][25];
            character.DeathSave = list[3][26];

            character.BodyArmour = list[3][30]; //gets the SP currently recorded
            character.HeadArmour = list[3][29];

            character.Injuries = new List<string>(8)
            {
                list[0][33],
                list[0][34],
                list[0][35],
                list[0][36],
                list[0][37],
                list[0][38],
                list[0][39],
                list[0][40]
            };

            character.CharismaticImpact = list[2][5];
            character.CombatAwareness = list[2][6];
            character.Interface = list[2][7];
            character.Maker = list[2][8];
            character.Medicine = list[2][9];
            character.Credibility = list[5][5];
            character.Teamwork = list[5][6];
            character.Backup = list[5][7];
            character.Operator = list[5][8];
            character.Moto = list[5][9];

            character.Brawling = list[11][3];
            character.Evasion = list[11][4];
            character.MeleeWeapon = list[11][5];
            character.Aikodo = list[11][7];
            character.Karate = list[11][8];
            character.Judo = list[11][9];
            character.Taekwondo = list[11][10];

            character.Archery = list[11][12];
            character.Autofire = list[11][13];
            character.Handgun = list[11][14];
            character.HeavyWeapons = list[11][15];
            character.ShoulderArms = list[11][16];

            character.Athletics = list[11][19];
            character.Contortionist = list[11][20];
            character.Dance = list[11][21];
            character.Endurance = list[11][22];
            character.ResistTortureAndDrugs = list[11][23];
            character.Stealth = list[11][24];

            character.DriveLandVehicle = list[11][26];
            character.PilotAirVehicle = list[11][27];
            character.PilotSeaVehicle = list[11][28];
            character.Riding = list[11][29];

            character.AirVehicletech = list[11][31];
            character.BasicTech = list[11][32];
            character.CyberTech = list[11][33];
            character.Demolitions = list[11][34];
            character.Electronics = list[11][35];
            character.FirstAid = list[11][36];
            character.Forgery = list[11][37];
            character.LandVehicleTech = list[11][38];
            character.MedicalTech = list[11][39];
            character.PaintDrawSculpt = list[11][40];
            character.Paramedic = list[11][41];
            character.PhotographyFilm = list[11][42];
            character.PickLock = list[11][43];
            character.PickPocket = list[11][44];
            character.SeaVehicleTech = list[11][45];
            character.Surgery = list[11][46];
            character.WeaponsTech = list[11][47];

            character.Acting = list[11][49];

            character.Concentration = list[17][3];
            character.ConcealRevealObject = list[17][4];
            character.LipReading = list[17][5];
            character.Perception = list[17][6];
            character.Tracking = list[17][7];
            character.Bribery = list[17][9];
            character.Conversation = list[17][10];
            character.Haggle = list[17][11];
            character.HumanPerception = list[17][12];
            character.Interrogation = list[17][13];
            character.Persuasion = list[17][14];
            character.PersonalGrooming = list[17][15];
            character.Streetwise = list[17][16];
            character.Trading = list[17][17];
            character.WardrobeAndStyle = list[17][18];
            character.Accounting = list[17][21];
            character.AnimalHandling = list[17][22];
            character.Bureaucracy = list[17][23];
            character.Business = list[17][24];
            character.Composition = list[17][25];
            character.Criminology = list[17][26];
            character.Cryptography = list[17][27];
            character.Deduction = list[17][28];
            character.Education = list[17][29];
            character.Gamble = list[17][30];
            character.LibrarySearch = list[17][31];
            character.Tactics = list[17][32];
            character.WildernessSurvival = list[17][33];
            character.StreetSlang = list[17][35];
            string jsonString = JsonSerializer.Serialize(character);
            string fileName = playerId + ".json";
            File.WriteAllText(fileName, jsonString);
            _handler._cachedCharacters.Add(character.PlayerId, character);
            return character;
        }


        private Spreadsheet GetSpreadsheet(string spreadsheetId)
        {

            // The DataFilters used to select which ranges to retrieve from
            // the spreadsheet.
            List<DataFilter> dataFilters = new List<DataFilter>();
            DataFilter df = new DataFilter();
            df.GridRange = new GridRange
            {
                StartRowIndex = 0,
                StartColumnIndex = 0,
                EndRowIndex = 90,
                EndColumnIndex = 20,
                SheetId = 0 //The first sheet created seems to always have ID 0. This'll need testing, but im leaving it for now.
            };
            dataFilters.Add(df);

            // True if grid data should be returned.
            // This parameter is ignored if a field mask was set in the request.
            bool includeGridData = true;

            // TODO: Assign values to desired properties of `requestBody`:
            GetSpreadsheetByDataFilterRequest requestBody = new GetSpreadsheetByDataFilterRequest();
            requestBody.DataFilters = dataFilters;
            requestBody.IncludeGridData = includeGridData;

            SpreadsheetsResource.GetByDataFilterRequest request = _gsheetService.Spreadsheets.GetByDataFilter(requestBody, spreadsheetId);

            return request.Execute();

        }
    }
}
