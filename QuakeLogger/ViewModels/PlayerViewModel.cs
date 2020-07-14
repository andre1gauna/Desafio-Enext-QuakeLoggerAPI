using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuakeLogger.API.ViewModels
{
    public class PlayerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Kills { get; set; }
        public int GameId { get; set; }
    }
}
