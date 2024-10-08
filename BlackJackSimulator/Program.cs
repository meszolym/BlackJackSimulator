using BlackJackSimulator.Models;
using System.Diagnostics;
using System.Threading.Channels;
using BlackJackSimulator.Models.Errors;

namespace BlackJackSimulator
{
    internal class Program
    {
        
        static void Main(string[] args)
        {


            int numTasks;
            int numGames;
            int numPlayers;

            string dir = "";

            if (args.Length == 4)
            {
                numTasks = int.Parse(args[0]);
                numGames = int.Parse(args[1]);
                numPlayers = int.Parse(args[2]);
                dir = args[3];
            }
            else
            {
                Console.Write("Enter the number of tasks (tables): ");
                numTasks = int.Parse(Console.ReadLine());
                Console.Write("Enter the number of games / table: ");
                numGames = int.Parse(Console.ReadLine());
                Console.Write("Enter the number of players: ");
                numPlayers = int.Parse(Console.ReadLine());
                Console.Write("Enter the path to the directory containing the strategy csv-s: ");
                dir = Console.ReadLine();
            }


            Strategy strategy = null;

            DirectoryPath.FromString(dir)
                .MapLeft(ex => new StrategyError(null, ex))
                .Bind(Strategy.FromDirectory)
                .Match(
                    Right: s => strategy = s,
                    Left: e =>
                    {
                        Console.WriteLine("Error loading strategy: " + e.InnerException.Match(
                            Some: e => e.Message,
                            None: () => ""));
                        Environment.Exit(1);
                    });

            double score = 0;
            object lockObj = new object();

            long comp = 0;
            double pct = 0;
            
            Stopwatch sw = new Stopwatch();
            Random rand = new Random();

            var tasks = Enumerable.Range(0, numTasks).Select(_ => new Task(() =>
            {
                var game = new Game(numPlayers, Game.DefaultNumberOfDecks, strategy, rand);

                for (int j = 0; j < numGames; j++)
                {
                    game.PlayRound();
                }

                double localScore = 0;
                game.players.Iter(player => localScore += player.Balance);

                lock (lockObj)
                {
                    comp++;
                    score += localScore;
                    if (comp % 5 == 0)
                        Console.WriteLine("Performed tasks: " + comp + " out of " + numTasks + " in " + sw.Elapsed);
                    
                }

            }));

            sw.Start();
            tasks.Iter(task => task.Start());

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch
            { }

            sw.Stop();
            Console.WriteLine("---");
            Console.WriteLine("Strategy: " + dir);
            Console.WriteLine("Total tables: " + numTasks);
            Console.WriteLine("Games / table: " + numGames);
            long totalgames = (long)numTasks * numGames;
            Console.WriteLine("Total number of games: " + totalgames);
            Console.WriteLine("Number of players: " + numPlayers);
            long totalhands = totalgames * numPlayers;
            Console.WriteLine("Total number of hands: " + totalhands);
            Console.WriteLine("Total earnings overall: " + score + " units");
            Console.WriteLine("Earnings / hand: " + (double)score / totalhands + " units/hand");
            Console.WriteLine("Runtime: " + sw.Elapsed + " (" + sw.Elapsed.TotalMinutes + " minutes)");
            int errorCount = tasks.Count(x => x.IsFaulted);
            if (errorCount > 0)
            {
                Console.WriteLine("Faulted: " + errorCount);
                /*Console.WriteLine("Errors:");
                foreach (var task in tasks.Where(x => x.Exception != null))
                {
                    Console.WriteLine(task.Exception);
                }*/
            }


            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
    }
}
