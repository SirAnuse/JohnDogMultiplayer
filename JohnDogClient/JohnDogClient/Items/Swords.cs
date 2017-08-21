using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnDogClient
{
    class Swords
    {
        private static int ItemID = 0;
        public static Item ShortSword = new Item();
        public static Item BroadSword = new Item();
        public static Item Saber = new Item();
        public static Item LongSword = new Item();
        public static void SetSwords ()
        {
            // Short Sword
            ItemID = 3;
            ShortSword.Name = "Short Sword";
            ShortSword.Description = "A steel short sword.";
            ShortSword.Tier = 0;
            ShortSword.MinDamage = 45;
            ShortSword.MaxDamage = 90;
            ShortSword.RateOfFire = 1f;
            ShortSword.Weapon = true;
            JohnDogClient.Items.Add(ItemID, ShortSword);
            // Broadsword
            ItemID = 4;
            BroadSword.Name = "Broad Sword";
            BroadSword.Description = "A broad-bladed steel sword.";
            BroadSword.Tier = 1;
            BroadSword.MinDamage = 60;
            BroadSword.MaxDamage = 105;
            BroadSword.RateOfFire = 1f;
            BroadSword.Weapon = true;
            JohnDogClient.Items.Add(ItemID, BroadSword);
            // Saber
            ItemID = 5;
            Saber.Name = "Saber";
            Saber.Description = "A single-edged light sword.";
            Saber.Tier = 2;
            Saber.MinDamage = 75;
            Saber.MaxDamage = 105;
            Saber.RateOfFire = 1f;
            Saber.Weapon = true;
            JohnDogClient.Items.Add(ItemID, Saber);
            // Long Sword
            ItemID = 6;
            LongSword.Name = "Long Sword";
            LongSword.Description = "A well-made sword with a double edged blade.";
            LongSword.Tier = 3;
            LongSword.MinDamage = 75;
            LongSword.MaxDamage = 125;
            LongSword.RateOfFire = 1f;
            LongSword.Weapon = true;
            JohnDogClient.Items.Add(ItemID, LongSword);
        }
    }
}
