using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CPR_Roller
{


    public class Character
    {
        private static Dictionary<string,string> populateValues()
        {
            Dictionary<string, string> returnable = new Dictionary<string, string>();
            foreach (var item in typeof(Character).GetProperties())
            {
                string name = item.Name;
                returnable.Add(name.ToUpper(), name);
            }
            return returnable;
        }

        //Key = normalized variable name, value = actual variable name
        [JsonIgnore]
        public Dictionary<string, string> variables = new Dictionary<string,string>(populateValues());
        //Tombstone
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string PlayerId { get; set; }
        
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Role { get; set; }
        public string Eurobucks { get; set; }
        public string InfluencePoints { get; set; } 
        public string Reputation { get; set; }


        //Core stats
        public string BODY { get; set; }
        public string DEX { get; set; }
        public string MOVE { get; set; }    
        public string REF { get; set; }
        public string TECH { get; set; }
        public string INT { get; set; }
        public string WILL { get; set; }
        public string EMP { get; set; }
        public string COOL { get; set; }
        public string LUCK { get; set; }

        //Defensive stats
        public string MaxHP { get; set; }
        public string CurHP { get; set; }
        public string DeathSave { get; set; }
        public bool Fatigue { get; set; }
        public string BodyArmour { get; set; }
        public string HeadArmour { get; set; }
        public List<string> Injuries { get; set; }

        //Role Abilities
        public string CharismaticImpact { get; set; }
        public string CombatAwareness { get; set; }
        public string Interface { get; set; }
        public string Maker { get; set; }
        public string Medicine { get; set; }
        public string Credibility { get; set; }
        public string Teamwork { get; set; }
        public string Backup { get; set; }
        public string Operator { get; set; }
        public string Moto { get; set; }


        //Skills
        //Combat skill
        public string Brawling { get; set; }
        public string Evasion { get; set; }
        public string MeleeWeapon { get; set; }
        public string Aikodo { get; set; }
        public string Karate { get; set; }
        public string Judo { get; set; }
        public string Taekwondo { get; set; }
        public string Archery { get; set; }
        public string Autofire { get; set; }
        public string Handgun { get; set; }
        public string HeavyWeapons { get; set; }
        public string ShoulderArms { get; set; }
        //Physical Skills
        public string Athletics { get; set; }
        public string Contortionist { get; set; }
        public string Dance { get; set; }
        public string Endurance { get; set; }
        public string ResistTortureAndDrugs { get; set; }
        public string Stealth { get; set; }
        public string DriveLandVehicle { get; set; }
        public string PilotAirVehicle { get; set; }
        public string PilotSeaVehicle { get; set; }
        public string Riding { get; set; }
        //Technique Skills
        public string AirVehicletech { get; set; }
        public string BasicTech { get; set; }
        public string CyberTech { get; set; }
        public string Demolitions { get; set; }
        public string Electronics { get; set; }
        public string FirstAid { get; set; }
        public string Forgery { get; set; }
        public string LandVehicleTech { get; set; }
        public string MedicalTech { get; set; }
        public string PaintDrawSculpt { get; set; }
        public string Paramedic { get; set; }
        public string PhotographyFilm { get; set; }
        public string PickLock { get; set; }
        public string PickPocket { get; set; }
        public string SeaVehicleTech { get; set; }
        public string Surgery { get; set; }
        public string WeaponsTech { get; set; }
        //Performance Skills
        public string Acting { get; set; }
        //Mental Skills
        public string Concentration { get; set; }
        public string ConcealRevealObject { get; set; }
        public string LipReading { get; set; }
        public string Perception { get; set; }
        public string Tracking { get; set; }
        public string Bribery { get; set; }
        public string Conversation { get; set; }
        public string Haggle { get; set; }
        public string HumanPerception { get; set; }
        public string Interrogation { get; set; }
        public string Persuasion { get; set; }
        public string PersonalGrooming { get; set; }
        public string Streetwise { get; set; }
        public string Trading { get; set; }
        public string WardrobeAndStyle { get; set; }
        //KnowledgeSkills
        public string Accounting { get; set; }
        public string AnimalHandling { get; set; }
        public string Bureaucracy { get; set; }
        public string Business { get; set; }
        public string Composition { get; set; }
        public string Criminology { get; set; }
        public string Cryptography { get; set; }
        public string Deduction { get; set; }
        public string Education { get; set; }
        public string Gamble { get; set; }
        public string LibrarySearch { get; set; }
        public string Tactics { get; set; }
        public string WildernessSurvival { get; set; }
        public string StreetSlang { get; set; }
        public string Language1 { get; set; }
        public string Language2 { get; set; }
        public string Language3 { get; set; }
        public string Language4 { get; set; }
        public string Language5 { get; set; }
        public string Language6 { get; set; }
        public string Language7 { get; set; }
        public string Language8 { get; set; }
        public string LocalExpert1 { get; set; }
        public string LocalExpert2 { get; set; }
        public string LocalExpert3 { get; set; }
        public string LocalExpert4 { get; set; }
        public string LocalExpert5 { get; set; }
        public string LocalExpert6 { get; set; }
        public string Sciences1 { get; set; }
        public string Sciences2 { get; set; }
        public string Sciences3 { get; set; }
        public string Sciences4 { get; set; }
        public string Sciences5 { get; set; }
        public string Sciences6 { get; set; }
        public string Sciences7 { get; set; }
        public string Sciences8{ get; set; }

    }
}
