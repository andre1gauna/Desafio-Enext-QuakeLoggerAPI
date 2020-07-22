using QuakeLogger.Domain.Interfaces.Models;
using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuakeLogger.Domain.Models
{
    public class KillMethod : IEntity
    {
        public int Id { get; set; }
        public string NameId { get; set; }
        public int Count { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public override bool Equals(object obj)
        {
            return this.NameId == ((KillMethod)obj).NameId;
        }

        public override int GetHashCode()
        {
            return (this.NameId.ToString() + '|' + this.Count.ToString()).GetHashCode();
        }

    }
}
