using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnDogClient
{
    class Player
    {
        public int TurnsSinceStun { get; set; }
        public bool Stunned { get; set; }
        public int StunnedDuration { get; set; }
        public bool ArmorBroken { get; set; }
        public bool Armored { get; set; }
        public bool Damaging { get; set; }
        public bool Berserk { get; set; }
        public bool Weak { get; set; }
        public bool Evasive { get; set; }
        public bool Slowed { get; set; }
        public bool Bleeding { get; set; }
        public bool Paralyzed { get; set; }
        public int BattleTurnsCounter { get; set; }
        public bool Dodged { get; set; }

        public int TotalDefenseBonus { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }
        public int Evasion { get; set; }
        private bool isVisible { get; set; } // for invisibility
        public int DEX { get; set; }
        public int ATT { get; set; }
        public int DEF { get; set; }
        public int WIS { get; set; }
        public bool BattleCompleted { get; set; }
        public int DamageTaken { get; set; }
        public bool Alive { get; set; }

        public int CalculateMPRegen (Player player)
        {
            float mpreg = player.WIS * 1.5f / 5;
            return Convert.ToInt32(Math.Round(mpreg, 0));
        }
    }
}
