using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnDogClient
{
    class Shields
    {
        public static Item WoodenShield = new Item
        {
            ID = 1,
            Name = "Wooden Shield", Description = "A shield made of a sturdy oak wood.",
            Tier = 0, MinDamage = 110, MaxDamage = 180, DefBonus = 2,
            ManaCost = 80, StunDuration = 3, Shield = true, Ability = true
        };
        public static Item IronShield = new Item
        {
            ID = 2,
            Name = "Iron Shield", Description = "A well-made, reliable shield, forged with iron.",
            Tier = 1, MinDamage = 200, MaxDamage = 280, DefBonus = 4,
            ManaCost = 85, StunDuration = 3, Shield = true, Ability = true
        };
        public static Item SteelShield = new Item
        {
            ID = 7,
            Name = "Steel Shield", Description = "A shield made of high quality steel.",
            Tier = 2, MinDamage = 450, MaxDamage = 570, DefBonus = 6,
            ManaCost = 90, StunDuration = 3, Shield = true, Ability = true
        };

        public static void SetShields()
        {
            JohnDogClient.Items.Add(1, WoodenShield);
            JohnDogClient.Items.Add(2, IronShield);
            JohnDogClient.Items.Add(7, SteelShield);
        }
    }
}
