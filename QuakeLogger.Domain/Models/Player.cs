using QuakeLogger.Domain.Interfaces.Models;
using System;

namespace QuakeLogger.Models
{
    public class Player : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Kills { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
