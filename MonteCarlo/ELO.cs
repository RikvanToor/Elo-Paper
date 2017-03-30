using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarlo
{
    public static class ELO
    {
        public class Queue
        {
            int[] elos;
            int count;
            bool full;

            public Queue(int size)
            {
                elos = new int[size];
                full = false;
                count = 0;
            }

            public void Add(int elo)
            {
                for(int i = elos.Length - 1; i > 0; i--)
                {
                    elos[i] = elos[i - 1];
                }

                elos[0] = elo;

                if (!full)
                {
                    count++;
                    full = count >= elos.Length;
                }
            }

            public double standardDeviation(Player p)
            {
                if (!full)
                    return 1000;                

                double totaldeviation = 0;
                foreach(int i in elos)
                {
                    totaldeviation += Math.Pow(p.trueskill - i, 2);
                }

                double variance = totaldeviation / elos.Length;
                return Math.Sqrt(variance);
            }
        }

        public static Queue recentGames;

        
        public static double WinChance(double ratingA, double ratingB)
        {
            return 1 / (1 + Math.Pow(10, (ratingA - ratingB) / 400));
        }

        public static Player randomPlayer(Player p, int lowerBound, int upperBound, int offset)
        {
            int min = Math.Max(p.rating - offset, lowerBound);
            int max = Math.Min(p.rating + offset, upperBound);

            int rating = Program.random.Next(min, max);
            int trueskill = Program.random.Next(rating - offset, rating + offset);

            if (min > max)
                throw new Exception();

            return new Player(rating, trueskill);
        }


        public static bool Converged(Player p, int bound, int checks)
        {
            //return Math.Abs(p.rating - p.trueskill) < bound;
            return recentGames.standardDeviation(p) < bound;
        }        

        public static double AverageSkill(Player[] team)
        {
            double total = 0;

            foreach (Player p in team)
                total += p.trueskill;

            return total / team.Length;
        }

        public static double AverageRating(Player[] team)
        {
            double total = 0;

            foreach (Player p in team)
                total += p.rating;

            return total / team.Length;
        }
    }
}
