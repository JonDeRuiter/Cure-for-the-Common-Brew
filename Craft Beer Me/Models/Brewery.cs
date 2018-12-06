using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Craft_Beer_Me.Models
{
    public class Brewery
    {
        public int ID { get; set; }
        public List<Beer> Menu { get; set; }
        public string Url { get; set; }
        public string PictureUrl { get; set; }
        public string Name { get; set; }
        public string BreweryID { get; set; }

        public Brewery()
        {

        }
        public Brewery(List<Beer> Menu, string Url, string PictureUrl, string Name, string BreweryId)
        {
            this.Menu = Menu;
            this.Url = Url;
            this.PictureUrl = PictureUrl;
            this.Name = Name;
            this.BreweryID = BreweryId;
        }
        
    }

    public class BreweryContext : DbContext
    {
        public DbSet<Brewery> Breweries { get; set; }
    }
}

