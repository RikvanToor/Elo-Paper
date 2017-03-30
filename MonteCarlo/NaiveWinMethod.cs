using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarlo
{
    public class NaiveWinMethod : OneVOneWinMethod
    {
        public override bool Beats(Player[] teamA, Player[] teamB, out double winChance)
        {
            winChance = ELO.WinChance(ELO.AverageSkill(teamA), ELO.AverageSkill(teamB));
            return Wins(winChance);
        }
    }
}
