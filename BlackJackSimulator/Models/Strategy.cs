using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using Action = BlackJackSimulator.Models.Enums.Action;
using static LanguageExt.Prelude;
using System.Reflection.Metadata.Ecma335;
using System.Reactive.Linq;


namespace BlackJackSimulator.Models
{

    public record StrategyError(Option<string> Message, Option<Exception> InnerException);

    public class Strategy
    {
        record struct ActionKey
        {
            public string PlayerHandValue;
            public string DealerUpCardValue;
        }

        //use FromDirectory
        private Strategy(Dictionary<ActionKey, Action> strategyDictionary) => strategySteps = strategyDictionary.ToImmutableDictionary();

        ImmutableDictionary<ActionKey, Action> strategySteps;

        internal Option<Action> GetAction(string playerHandVal, string dealerUpCardVal) => strategySteps.FirstOrDefault(
            x => x.Key.PlayerHandValue == playerHandVal
                 && x.Key.DealerUpCardValue == dealerUpCardVal).Value;

        internal static Either<StrategyError, Strategy> FromDirectory(DirectoryPath directoryPath)
        {
            Dictionary<ActionKey, Action> stratDict = new();

            return Try(() =>
            {
                Directory.EnumerateFiles(directoryPath.PathString, "*.csv").Iter(file =>
                {
                    using var sr = new StreamReader(file);
                    var lines = sr.ReadToEnd().Split("\r\n");

                    for (int i = 1; i < lines.Length; i++)
                    {
                        var line = lines[i].Split(",");
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
                });
                return new Strategy(stratDict);
            }).ToEither(ex => new StrategyError("Cannot create strategy", ex));
        }

    }
}
