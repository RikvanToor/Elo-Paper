using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarlo
{
    class MobaWinMethod : WinMethod
    {
        public override bool Beats(Player[] teamA, Player[] teamB, out double winChance)
        {
            int individualWins = 0;
            int teamSize = teamA.Length;

            for (int p = 0; p < teamSize; p++)
            {
                double chance = teamA[p].trueskill - teamB[p].trueskill > Program.beatBound ? 1 : (teamA[p].trueskill - teamB[p].trueskill < -Program.beatBound ? 0 : ELO.WinChance(teamA[p].trueskill, teamB[p].trueskill));
                if (Wins(chance))
                {
                    individualWins++;
                    //Console.WriteLine($"allied player {p} beat enemy player {p}");
                }
                else
                {
                    //Console.WriteLine($"allied player {p} lost to enemy player {p}");
                }
                
            }

            double relativeWins = (double)individualWins / (double)teamSize;
            //Console.WriteLine($"relativeWins: {relativeWins}");
            double averageSkillA = ELO.AverageSkill(teamA);
            double averageSkillB = ELO.AverageSkill(teamB);
            winChance = ELO.WinChance(averageSkillA, averageSkillB);
            //Console.WriteLine($"average Skill team A: {averageSkillA}");
            //Console.WriteLine($"average Skill team B: {averageSkillB}");
            //Console.WriteLine($"winChance: {winChance}");
            double averageChance = (relativeWins + winChance) / 2;
            //Console.WriteLine($"averageChance: {averageChance}");

            return Wins(averageChance);
        }
    }
}
