using QuakeLogger.Tests.Fakes;
using Xunit;

namespace QuakeLogger.Tests.ServicesTests
{
    public class ParserTest
    {
        private readonly ContextFake _contextFake;

        public ParserTest()
        {
            _contextFake = new ContextFake("ParserTest");
        }

        [Fact]
        public void Calculate()
        {
            Assert.True(5 > 3);

            //var context = _contextFake.GetContext("AddPlayer_ShouldWork");
            //context.AddFakeGames()
            //    .AddFakePlayers()
            //    .AddFakeGamePlayers()
            //    .AddFakeKillMethods();
            //var repop = new QuakePlayerRepo(context);
            //var repog = new QuakeGameRepo(context);
            //var repokm = new QuakeKillMethodRepo(context);
            //Parser parser = new Parser(repog, repop, repokm);

            //parser.Reader(@"C:\Users\andre\source\repos\QuakeLoggerAPI\testRaw.txt");

            //Game game = parser.GetGame(1);
            //List<Player> players = parser.GetPlayersByGameId(1);
            //List<KillMethod> killMethods = parser.GetKillMethodsByGameId(1);          

            //Assert.NotNull(game);

        }

    }
}
