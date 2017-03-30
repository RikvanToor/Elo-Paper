using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarlo
{
    public class OneVOneWinMethod : WinMethod
    {
        public override bool Beats(Player[] teamA, Player[] teamB, out double winChance)
        {
            double totalChance = 0;
            int teamSize = teamA.Length;

            for (int p = 0; p < teamSize; p++)
                totalChance += ELO.WinChance(teamA[p].trueskill, teamB[p].trueskill);

            double averageChance = totalChance / (double)teamSize;
            winChance = averageChance;

            return Wins(averageChance);
        }
    }
}
