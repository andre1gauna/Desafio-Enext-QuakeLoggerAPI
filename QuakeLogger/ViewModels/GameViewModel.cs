using QuakeLogger.API.ViewModels;
using QuakeLogger.Models;
using System.Collections.Generic;

namespace QuakeLogger.ViewModels
{
    public class GameViewModel
    {
        public int Id { get; set; }
        public int TotalKills { get; set; }        
        public List<PlayerViewModel> Players { get; set; }
    }
}
