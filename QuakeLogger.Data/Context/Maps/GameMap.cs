using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuakeLogger.Models;

namespace QuakeLogger.Data.Context.Maps
{
    class GameMap : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {

            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Players)
                .WithOne(x => x.Game);

        }
    }
}
