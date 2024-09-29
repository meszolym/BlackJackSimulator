using System.Diagnostics;

namespace BlackJackSimulator
{
    internal class Program
    {
        
        static void Main(string[] args)
        {


            int numTasks;
            int numGames;
            int numPlayers;
            string hardPath = "hard.csv";
            string softPath = "soft.csv";
            string pairPath = "pair.csv";

            if (args.Length == 6)
            {
                numTasks = int.Parse(args[0]);
                numGames = int.Parse(args[1]);
                numPlayers = int.Parse(args[2]);
                hardPath = args[3];
                softPath = args[4];
                pairPath = args[5];
            }
            else
            {
                Console.Write("Enter the number of tasks (tables): ");
                numTasks = int.Parse(Console.ReadLine());
                Console.Write("Enter the number of games / table: ");
                numGames = int.Parse(Console.ReadLine());
                Console.Write("Enter the number of players: ");
                numPlayers = int.Parse(Console.ReadLine());
                Console.Write("Enter the path of the hard total csv: ");
                hardPath = Console.ReadLine();
                Console.Write("Enter the path of the soft total csv: ");
                softPath = Console.ReadLine();
                Console.Write("Enter the path of the pair csv: ");
                pairPath = Console.ReadLine();
            }



            #region strategy

            //forrás: https://www.blackjackinfo.com/blackjack-basic-strategy-engine/?numdecks=6&soft17=s17&dbl=all&das=yes&surr=ns&peek=no

            StreamReader sr = new StreamReader(hardPath);
            string[] lines = sr.ReadToEnd().Split("\r\n");
            sr.Close();

            Strategy Strategy = new Strategy();

            string val = "";
            Action? act = null;
            for (int i = 1; i < 19; i++)
            {
                for (int j = 1; j < 11; j++)
                {
                    val = lines[i].Split(",")[j];
                    switch (val)
                    {
                        case "H":
                            act = Action.Hit;
                            break;
                        case "S":
                            act = Action.Stand;
                            break;
                        case "D":
                            act = Action.DoubleHit;
                            break;
                        case "DS":
                            act = Action.DoubleStand;
                            break;
                        case "P":
                            act = Action.Split;
                            break;
                        default:
                            throw new Exception("Should never get here");
                    }


                    Strategy.StrategySteps.Add(new ActionKey()
                    {
                        PlayerHandValue = lines[i].Split(",")[0],
                        DealerUpCardValue = lines[0].Split(",")[j]
                    }, act.Value);
                }
            }

            sr = new StreamReader(softPath);
            lines = sr.ReadToEnd().Split("\r\n");
            sr.Close();
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 11; j++)
                {
                    val = lines[i].Split(",")[j];
                    switch (val)
                    {
                        case "H":
                            act = Action.Hit;
                            break;
                        case "S":
                            act = Action.Stand;
                            break;
                        case "D":
                            act = Action.DoubleHit;
                            break;
                        case "DS":
                            act = Action.DoubleStand;
                            break;
                        case "P":
                            act = Action.Split;
                            break;
                        default:
                            throw new Exception("Should never get here");
                    }


                    Strategy.StrategySteps.Add(new ActionKey()
                    {
                        PlayerHandValue = lines[i].Split(",")[0],
                        DealerUpCardValue = lines[0].Split(",")[j]
                    }, act.Value);
                }
            }

            sr = new StreamReader(pairPath);
            lines = sr.ReadToEnd().Split("\r\n");
            sr.Close();
            for (int i = 1; i < 11; i++)
            {
                for (int j = 1; j < 11; j++)
                {
                    val = lines[i].Split(",")[j];
                    switch (val)
                    {
                        case "H":
                            act = Action.Hit;
                            break;
                        case "S":
                            act = Action.Stand;
                            break;
                        case "D":
                            act = Action.DoubleHit;
                            break;
                        case "DS":
                            act = Action.DoubleStand;
                            break;
                        case "P":
                            act = Action.Split;
                            break;
                        default:
                            throw new Exception("Should never get here");
                    }


                    Strategy.StrategySteps.Add(new ActionKey()
                    {
                        PlayerHandValue = lines[i].Split(",")[0],
                        DealerUpCardValue = lines[0].Split(",")[j]
                    }, act.Value);
                }
            }

            #endregion


            double score = 0;
            object lockObj = new object();

            long comp = 0;
            double pct = 0;
            List<Task> tasks = new List<Task>();
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < numTasks; i++)
            {
                tasks.Add(new Task(() =>
                {
                    Game g = new Game(numPlayers, Game.DefaultNumberOfDecks, Strategy);

                    for (int j = 0; j < numGames; j++)
                    {
                        g.PlayRound();
                    }

                    double localScore = 0;
                    
                    foreach (var p in g.players)
                    {
                        localScore += p.Balance;
                    }
                    lock (lockObj)
                    {
                        comp++;
                        score += localScore;
                        Console.WriteLine("Performed tasks: " + comp + " out of " + numTasks + " in " + sw.Elapsed);
                    }

                }));
            }

            sw.Start();
            foreach (var task in tasks)
            {
                task.Start();
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch
            { }
            sw.Stop();
            Console.WriteLine("---");
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
