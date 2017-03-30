using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarlo
{
    public abstract class WinMethod
    {
        private int scale = 1000000;

        public abstract bool Beats(Player[] teamA, Player[] teamB, out double winchance);

        protected bool Wins(double winchance)
        {
            int estimate = (int)(winchance * (double)scale);
                       
            int random = Program.random.Next(scale);

            return random < estimate;
        }
    }
}
