using QuakeLogger.API.ViewModels;
using QuakeLogger.Models;
using System.Collections.Generic;

namespace QuakeLogger.ViewModels
{
    public class GameViewModel<PlayerViewModel>
    {
        public int Id { get; set; }
        public int TotalKills { get; set; }
        public IEnumerable<PlayerViewModel> Players { get; set; }
    }
}
