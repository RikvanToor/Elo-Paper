using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarlo
{
    public class Player
    {
        public int rating, trueskill;

        public Player(int rating, int trueskill)
        {
            this.rating = rating;
            this.trueskill = trueskill;
        }
    }
}
