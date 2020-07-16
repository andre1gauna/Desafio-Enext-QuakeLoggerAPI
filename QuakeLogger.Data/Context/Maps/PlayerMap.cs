using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuakeLogger.Models;

namespace QuakeLogger.Data.Context.Maps
{
    class PlayerMap : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {

            builder.HasKey(x => x.Id);            

        }
    }
}
