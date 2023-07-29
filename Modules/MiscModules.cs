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
    public class MiscModules : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }

        private InteractionHandler _handler;

        // Constructor injection is also a valid way to access the dependencies
        public MiscModules(InteractionHandler handler)
        {
            _handler = handler;

        }

        [SlashCommand("pyth", "Pythagoras theorem")]
        public async Task Pythagoras(int x, int y)
        {
            await RespondAsync("Hypotenuse of " + x + " and " + y + " is **" + Math.Sqrt(x * x + y * y).ToString("#.##") + "**.");
        }

        [SlashCommand("help", "helps")]
        public async Task Help()
        {
            await RespondAsync("`create-character(character name, gsheet link)` creates or updates a character. Use this gsheet: https://docs.google.com/spreadsheets/d/1mxAAKOs7n4eW24M9QWmfQADDYm3xshvkYxZ2AwfJLLI/copy\n"
                 + "`pyth (x, y)` finds the hypotenuse of two sides, X and Y.");
        }

       
    }
}
