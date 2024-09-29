namespace BlackJackSimulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game g = new Game(Game.DefaultNumberOfPlayers, Game.DefaultNumberOfDecks,new Strategy());

            for (int i = 0; i < 1000; i++)
            {
                g.PlayRound();
            }
        }
    }
}
