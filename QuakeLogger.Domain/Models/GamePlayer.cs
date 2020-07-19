using QuakeLogger.Models;

namespace QuakeLogger.Domain.Models
{
    public class GamePlayer
    {
        public int GameId { get; set; }
        public Game Game { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }

        private int _kills;
        public int Kills
        {
            get
            {
                return _kills;
            }
            set
            {
                _kills = value;
                if (_kills < 0)
                    Kills = 0;
            }
        }
    }
}
