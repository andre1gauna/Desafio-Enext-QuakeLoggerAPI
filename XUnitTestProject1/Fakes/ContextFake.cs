using Microsoft.EntityFrameworkCore;
using QuakeLogger.Data.Context;

namespace QuakeLogger.Tests.Fakes
{
    public class ContextFake
    {
        private readonly string _prefixDataBase;

        public ContextFake(string prefixDataBase)
        {
            _prefixDataBase = prefixDataBase;
        }

        public QuakeLoggerContext GetContext(string dataBaseName)
        {
            var options = new DbContextOptionsBuilder<QuakeLoggerContext>()
                .UseInMemoryDatabase(_prefixDataBase + "_" + dataBaseName)
                .Options;
            return new QuakeLoggerContext(options);
        }
    }

}