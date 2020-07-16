using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuakeLogger.Domain.Models
{
    public class GamePlayer
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
