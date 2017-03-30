using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarlo
{
    class Program
    {
        public static int convergeBound = 20, offset = 50, lowerBound = 1000, upperBound = 2800, convergeCheckCount = 10, beatBound = 100;
        public static long games = 0;
        static int debugRounds = 10;
        static int standardRating = 1500;
        static WinMethod winMethod;
        public static Random random;
        static int[] defaultValues = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public static List<long> stats;
        public static List<int> elos;

        static void Main(string[] args)
        {            
            random = new Random();
            ELO.recentGames = new ELO.Queue(convergeCheckCount);

            while (true)
            {
                Console.WriteLine("Do you want to interact or get statistics? (interact/stats)");
                string answer = Console.ReadLine();
                switch (answer.ToLower())
                {
                    case "interact":
                        Interact();
                        break;
                    case "stats":
                        GetStats();
                        break;
                    default:
                        Console.WriteLine("I don't know what you mean.");
                        break;
                }
            }
        }

        static void Interact()
        {
            Console.WriteLine("Hi there handsome. Wanna have some fun?");
            while (true)
            {               

                Console.WriteLine("Tell me what you'd like the team size to be");
                string input = Console.ReadLine();
                int teamSize = int.Parse(input);

                if (teamSize <= 0)
                {
                    Console.WriteLine("I guess an integer greater than 0 is too much to ask.");
                    continue;
                }

                bool gotmethod = false;
                while (!gotmethod)
                    gotmethod = GotMethod();

                stats = new List<long>();
                StartSimulation(teamSize, offset);
                Console.WriteLine("It took {0} games to converge", games);
            }
        }

        static void GetStats()
        {
            Sims: //Extract Simulation Number
            Console.WriteLine("How many simulations per size would you like?");
            string answer = Console.ReadLine();
            int sims;
            if (answer == "")
                sims = 1000;
            else if (!int.TryParse(answer, out sims))
                goto Sims;

            TeamSize: //Extract Team Sizes
            Console.WriteLine("Give a list of team sizes (integers separated by spaces)");
            answer = Console.ReadLine();
            string[] sizes = answer.Split(' ');
            int[] values = new int[sizes.Length];
            if (answer == "")
                values = defaultValues;
            else
            {
                for(int i = 0; i < sizes.Length; i++)
                {
                    string s = sizes[i];
                    if(!int.TryParse(s, out values[i]))
                    {
                        Console.WriteLine("Incorrect input");
                        goto TeamSize;
                    }

                }
            }

            //Extract Method Type
            bool gotmethod = false;
            while (!gotmethod)
                gotmethod = GotMethod();

            bool gotcompetence = false;
            while (!gotcompetence)
                gotcompetence = GotCompetence();

            foreach (int i in values)
            {
                stats = new List<long>();
                long simGames = 0;
                Console.WriteLine($"Starting {sims} simulations for a teamsize of {i}..");
                for (int j = 0; j < sims; j++)
                {
                    elos = new List<int>();
                    StartSimulation(i, offset);
                    simGames += games - convergeCheckCount;
                }

                long averageConvGames = simGames / sims;
                double standardDeviation, lower, upper;
                GenerateStats((double)averageConvGames, out standardDeviation, out lower, out upper);
                            

                Console.WriteLine($"simGames: {simGames}; teamsize: {i}");
                Console.WriteLine($"Simulations Complete. Average numbers of games to converge to true skill: {averageConvGames}");
                Console.WriteLine($"standard deviation: {standardDeviation}");
                Console.WriteLine($"95% Confidence Interval: {lower}, {upper} \n");
            }            
        }

        private static bool GotMethod()
        {
            Console.WriteLine("what would you like the win method to be?");
            string method = Console.ReadLine();
            switch (method.ToLower())
            {
                case "naive":
                    winMethod = new NaiveWinMethod();
                    return true;
                case "moba":
                    winMethod = new MobaWinMethod();
                    return true;
                case "1v1":
                    winMethod = new OneVOneWinMethod();
                    return true;
                default:
                    Console.WriteLine("Not a valid method");
                    return false;
             }
        }

        private static bool GotCompetence()
        {
            Console.WriteLine("what would you like the competence value to be?");
            string value = Console.ReadLine();
            return int.TryParse(value, out offset);
        }

        static void StartSimulation(int teamSize, int offset)
        {
            games = 0;
            

            Player player = new Player(standardRating, random.Next(lowerBound, upperBound));         

            while(!Converged(player, convergeBound, (int)Math.Round(elos.Count*0.05)))
            {
                //if (games % debugRounds == 0)
                //    Console.WriteLine("#Games: {2}, Player rating: {0}, Player true skill: {1}", player.rating, player.trueskill, games);
                StartGame(teamSize, ref player);
                //Console.WriteLine(player.rating);
            }

            //Console.WriteLine("True skill: " + player.trueskill);

            stats.Add(games - convergeCheckCount);           
        }

        static bool Converged(Player p, int convergeBound, int nrOfGames)
        {
            if (elos.Count < 200)
                return false;
            int[] arr = elos.ToArray();
            return standardDeviation(p, arr, elos.Count - nrOfGames - 1, elos.Count) < convergeBound;
        }

        static double standardDeviation(Player p, int[] arr, int firstIndex, int lastIndex)
        {
            double totaldeviation = 0;
            for(int i = firstIndex; i<lastIndex;i++)
            {
                int j = arr[i];
                totaldeviation += Math.Pow(p.trueskill - j, 2);
            }

            double variance = totaldeviation / elos.Count;
            return Math.Sqrt(variance);
        }

        static void StartGame(int teamSize, ref Player p)
        {
            games++;
            ELO.recentGames.Add(p.rating);
            Player[] enemies = new Player[teamSize];
            Player[] allies = new Player[teamSize];

            //Console.WriteLine($"Enemies:");
            for (int i = 0; i < teamSize; i++)
            {
                enemies[i] = ELO.randomPlayer(p, lowerBound, upperBound, offset);
                //Console.WriteLine($"- player {i} => rating: {enemies[i].rating}; trueskill: {enemies[i].trueskill}");
            }

            //Console.WriteLine("Allies:");
            //Console.WriteLine($"- you => rating: {p.rating}; trueskill: {p.trueskill}");
            allies[0] = p;
            for (int j = 1; j < teamSize; j++)
            {
                allies[j] = ELO.randomPlayer(p, lowerBound, upperBound, offset);
                //Console.WriteLine($"- player {j} => rating: {allies[j].rating}; trueskill: {allies[j].trueskill}");
            }

            
            double winchance;
            bool win = winMethod.Beats(allies, enemies, out winchance);
            //Console.WriteLine($"win: {win}");
            
            p.rating = Clamp(NewRating(p.rating, win, winchance), lowerBound, upperBound);
            elos.Add(p.rating);
        }

        static int NewRating(int oldRating, bool win, double winchance)
        {
            return oldRating + (int)(((win ? 1d : 0d) - winchance) * K(oldRating));
        }
        
        static double K(int oldrating)
        {
            if (oldrating < 2000)
                return 16;
            else if (oldrating < 2200)
                return 12;
            else
                return 10;
        }

        static int Clamp(int value, int lower, int upper)
        {
            if (value < lower)
                return lower;
            else if (value > upper)
                return upper;
            return value;
        }

        static void GenerateStats(double average, out double standardDeviation, out double lower, out double upper)
        {
            double totaldeviation = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                totaldeviation += Math.Pow(average - (double)stats[i], 2);
            }

            double variance = totaldeviation / stats.Count;
            standardDeviation = Math.Sqrt(variance);
            double boundiff = 1.96 * standardDeviation / Math.Sqrt(stats.Count);
            lower = average - boundiff;
            upper = average + boundiff;
        }

    }     
}
