using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models;
using LanguageExt;
using Action = BlackJackSimulator.Models.Action;
using static LanguageExt.Prelude;
using System.Reflection.Metadata.Ecma335;
using System.Reactive.Linq;


namespace BlackJackSimulator
{

    public record StrategyError(Option<string> Message, Exception InnerException);
    
    internal class Strategy
    {
        struct ActionKey
        {
            internal string PlayerHandValue;
            internal string DealerUpCardValue;
        }

        //use FromDirectory
        private Strategy(Dictionary<ActionKey, Action> stratDict) => StrategySteps = stratDict.ToImmutableDictionary();

        ImmutableDictionary<ActionKey, Action> StrategySteps;

        internal static Either<StrategyError, Strategy> FromDirectory(DirectoryPath directoryPath)
        {
            Dictionary<ActionKey, Action> stratDict = new();

            return Try(() =>
                   {
                        Directory.EnumerateFiles(directoryPath.PathString, "*.csv").Iter(file =>
                        {
                            using (var sr = new StreamReader(file))
                            {
                                string[] lines = sr.ReadToEnd().Split("\r\n");
                                string val = "";

                                for (int i = 1; i < lines.Length; i++)
                                {
                                    string[] line = lines[i].Split(",");
                                    for (int j = 1; j < line.Length; j++)
                                    {
                                        Action? act = null;
                                        switch (line[j])
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

                                        stratDict.Add(new ActionKey()
                                        {
                                            PlayerHandValue = lines[i].Split(",")[0],
                                            DealerUpCardValue = lines[0].Split(",")[j]
                                        }, act.Value);
                                    }
                                }
                            }
                        });
                        return new Strategy(stratDict);
                   }).ToEither(ex => new StrategyError("Cannot create strategy", ex));

            #region strategy

            //StreamReader sr = new StreamReader(hardPath);
            //string[] lines = sr.ReadToEnd().Split("\r\n");
            //sr.Close();

            //Strategy Strategy = new Strategy();

            //string val = "";
            //Action? act = null;
            //for (int i = 1; i < 19; i++)
            //{
            //    for (int j = 1; j < 11; j++)
            //    {
            //        val = lines[i].Split(",")[j];
            //        switch (val)
            //        {
            //            case "H":
            //                act = Action.Hit;
            //                break;
            //            case "S":
            //                act = Action.Stand;
            //                break;
            //            case "D":
            //                act = Action.DoubleHit;
            //                break;
            //            case "DS":
            //                act = Action.DoubleStand;
            //                break;
            //            case "P":
            //                act = Action.Split;
            //                break;
            //            default:
            //                throw new Exception("Should never get here");
            //        }


            //        Strategy.StrategySteps.Add(new ActionKey()
            //        {
            //            PlayerHandValue = lines[i].Split(",")[0],
            //            DealerUpCardValue = lines[0].Split(",")[j]
            //        }, act.Value);
            //    }
            //}

            //sr = new StreamReader(softPath);
            //lines = sr.ReadToEnd().Split("\r\n");
            //sr.Close();
            //for (int i = 1; i < 9; i++)
            //{
            //    for (int j = 1; j < 11; j++)
            //    {
            //        val = lines[i].Split(",")[j];
            //        switch (val)
            //        {
            //            case "H":
            //                act = Action.Hit;
            //                break;
            //            case "S":
            //                act = Action.Stand;
            //                break;
            //            case "D":
            //                act = Action.DoubleHit;
            //                break;
            //            case "DS":
            //                act = Action.DoubleStand;
            //                break;
            //            case "P":
            //                act = Action.Split;
            //                break;
            //            default:
            //                throw new Exception("Should never get here");
            //        }


            //        Strategy.StrategySteps.Add(new ActionKey()
            //        {
            //            PlayerHandValue = lines[i].Split(",")[0],
            //            DealerUpCardValue = lines[0].Split(",")[j]
            //        }, act.Value);
            //    }
            //}

            //sr = new StreamReader(pairPath);
            //lines = sr.ReadToEnd().Split("\r\n");
            //sr.Close();
            //for (int i = 1; i < 11; i++)
            //{
            //    for (int j = 1; j < 11; j++)
            //    {
            //        val = lines[i].Split(",")[j];
            //        switch (val)
            //        {
            //            case "H":
            //                act = Action.Hit;
            //                break;
            //            case "S":
            //                act = Action.Stand;
            //                break;
            //            case "D":
            //                act = Action.DoubleHit;
            //                break;
            //            case "DS":
            //                act = Action.DoubleStand;
            //                break;
            //            case "P":
            //                act = Action.Split;
            //                break;
            //            default:
            //                throw new Exception("Should never get here");
            //        }


            //        Strategy.StrategySteps.Add(new ActionKey()
            //        {
            //            PlayerHandValue = lines[i].Split(",")[0],
            //            DealerUpCardValue = lines[0].Split(",")[j]
            //        }, act.Value);
            //    }
            //}

            #endregion
        }


        internal Option<Action> GetAction(Hand hand, Card dealerUpCard)
        {
            string handVal = hand.GetValue().ToString();
            string dealerVal = dealerUpCard.GetValue().ToString();
            
            return StrategySteps.FirstOrDefault(
                x => x.Key.PlayerHandValue == handVal
                && x.Key.DealerUpCardValue == dealerVal).Value;
        }


    }
}
