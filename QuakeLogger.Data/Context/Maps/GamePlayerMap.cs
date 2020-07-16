using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuakeLogger.Domain.Models;


namespace QuakeLogger.Data.Context.Maps
{
    class GamePlayerMap : IEntityTypeConfiguration<GamePlayer>
    {
        public void Configure(EntityTypeBuilder<GamePlayer> builder)
        {

            builder.HasKey(x => new { x.GameId, x.PlayerId });

            builder.HasOne(au => au.Player)
                .WithMany(ur => ur.GamePlayers)
                .HasForeignKey(au => au.PlayerId);

            builder.HasOne(ar => ar.Game)
                            .WithMany(ur => ur.GamePlayers)
                            .HasForeignKey(ar => ar.GameId);                  

        }
    }
}
