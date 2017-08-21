using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using Console = Colorful.Console;

namespace JohnDogClient
{
    class Item
    {
        public bool Weapon { get; set; }
        public float StunDuration { get; set; }
        public bool Shield { get; set; }
        // ManaCost if ability
        public int ManaCost { get; set; } 
        public bool Ability { get; set; }
        public bool Armor { get; set; }
        public bool Ring { get; set; }
        public int DefBonus { get; set; }
        public string Name { get; set; }
        public int MaxDamage { get; set; }
        public int MinDamage { get; set; }
        public float RateOfFire { get; set; }
        public int Tier { get; set; }
        public bool Untiered { get; set; }
        public string Description { get; set; }
        public bool Consumable { get; set; }
        public int ID { get; set; }

        public static int CalculateDMG(Item item, Player player)
        {
            Random rand = new Random();
            float ATTMultiplier = 0.5f + player.ATT / 50;               // Calculate damage multiplier
            float MinDMG = item.MinDamage * ATTMultiplier;
            float MaxDMG = item.MaxDamage * ATTMultiplier;
            return rand.Next(Convert.ToInt32(MinDMG), Convert.ToInt32(MaxDMG));
        }

        public static float CalculateDPS(Item item, Player player, bool isValue)
        {
            float step1 = item.MaxDamage + item.MinDamage;      // Calculate average damage (pt 1)
            float step2 = step1 / 2;                            // Calculate average damage (pt 2)
            float step3 = 0.5f + player.ATT / 50;               // Calculate damage multiplier
            float step4 = step2 * step3;                        // Multiply damage by multiplier
            float step5 = 1.5f + 6.5f * (player.DEX / 75);      // Calculate APS (attacks per second)
            return step4 * step5;                                // Calculate DPS by multiplying APS by damage
        }

        public static void CalculateDPS (Item item, Player player)
        {
            float step1 = item.MaxDamage + item.MinDamage;      // Calculate average damage (pt 1)
            float step2 = step1 / 2;                            // Calculate average damage (pt 2)
            float step3 = 0.5f + player.ATT / 50;               // Calculate damage multiplier
            float step4 = step2 * step3;                        // Multiply damage by multiplier
            float step5 = 1.5f + 6.5f * (player.DEX / 75);      // Calculate APS (attacks per second)
            float DPS = step4 * step5;                          // Calculate DPS by multiplying APS by damage
            Console.Write("\nDPS Average: ", Color.White);
            Console.Write(DPS);
        }

        public static void PrintDetails (Item item)
        {
            Console.Write("\nName: ", Color.Orange);
            Console.Write(item.Name);
            Console.Write("\nTier: ", Color.Orange);
            if (item.Untiered) Console.Write("Untiered", Color.Purple);
            else Console.Write(item.Tier);
            Console.Write("\nDamage: ", Color.Orange);
            Console.Write(item.MinDamage + " - " + item.MaxDamage);
            if (item.Weapon)
            {
                Console.Write("\nRate of Fire: ", Color.Orange);
                float FireRate = item.RateOfFire * 100;
                Console.Write(FireRate + "%");
            }
            Console.Write("\nDescription: ", Color.Orange);
            Console.Write(item.Description);
            if (item.Shield)
            {
                Console.Write("\nStun Duration: ", Color.Orange);
                Console.Write(item.StunDuration + " Turns");
            }
            if (item.Ability)
            {
                Console.Write("\nMana Cost: ", Color.Orange);
                Console.Write(item.ManaCost);
            }
            if (item.DefBonus > 0 || item.DefBonus < 0)
            {
                Console.Write("\nStat Bonuses: ", Color.Orange);
                Console.Write("+" + item.DefBonus + " DEF");
            }
            Console.Write("\n");
        }

        public static void PrintDetails(Item item, Color color)
        {
            Console.Write("\n\nName: ", Color.White);
            Console.Write(item.Name, color);
            Console.Write("\nTier: ", Color.White);
            if (item.Untiered) Console.Write("Untiered", Color.Purple);
            else Console.Write(item.Tier, color);
            Console.Write("\nDamage: ", Color.White);
            Console.Write(item.MinDamage + " - " + item.MaxDamage, color);
            Console.Write("\nRate of Fire: ", Color.White);
            float FireRate = item.RateOfFire * 100;
            Console.Write(FireRate + "%", color);
            Console.Write("\nDescription: ", Color.White);
            Console.Write(item.Description, color);
        }
    }
}
