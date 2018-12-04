using Craft_Beer_Me.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Craft_Beer_Me.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult results(string ABV, string IBU, string SRM)
        {
            return View();
        }

        public ActionResult Recommended(string ABV, string IBU, string SRM)
        {
            double abv, ibu, srm;
            abv = double.Parse(ABV);
            ibu = double.Parse(IBU);
            srm = double.Parse(SRM);

            List<Brewery> breweries = GetBreweries(abv, ibu, srm);
            
            ViewBag.Breweries = breweries;
            return View();
            
        }

        public List<Brewery> GetBreweries(double abv, double ibu, double srm)
        {
            List<Brewery> breweries = new List<Brewery>();

            //This bool is to quickly switch between live db and local data
            bool isdbDown = true;

            //gets results for each of out 14 craft brewries
            if (isdbDown)
            {
                for (int i = 0; i < 5; i++)
                {
                    breweries = LocalBrewery(abv, ibu, srm);
                }
            }
            else
            {
                for (int i = 1; i < 15; i++)
                {
                    //string urlString = "https://sandbox-api.brewerydb.com/v2/" + "brewery/" + BreweryId(i) +  "/beers?key=5049b9309015a193f513d52c4d9c0003";

                    //test url
                    string urlString = "https://sandbox-api.brewerydb.com/v2/" + "brewery/" + "AqEUBQ" + "/beers?key=5049b9309015a193f513d52c4d9c0003";

                    HttpWebRequest request = WebRequest.CreateHttp(urlString);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader rd = new StreamReader(response.GetResponseStream());
                    string beerData = rd.ReadToEnd();

                    JObject beerJson = JObject.Parse(beerData);
                    //Valid beers get added
                                       
                    //api limits to 10 requests a second, this *should* solve that
                    Thread.Sleep(150);
                }
            }
            
            return breweries;
        }
        
        //designed to be used by a for loop to return each of our 14 breweries
        public string BreweryId(int id)
        {
            switch (id)
            {
                case 1:
                    //founders
                    return "Idm5Y5";
                    break;
                case 2:
                    //hopcat
                    return "HizvxH";
                    break;
                case 3:
                    //jolly pumpkin
                    return "pzWq1r";
                    break;
                case 4:
                    //the mitten
                    return "bdFoir";
                    break;
                case 5:
                    //harmony
                    return "P0oEwB";
                    break;
                case 6:
                    //elk brewing
                    return "sjblac";
                    break;
                case 7:
                    //perrin
                    return "Boa6td";
                    break;
                case 8:
                    //rockford brewing
                    return "U92Ctx";
                    break;
                case 9:
                    //brewery vivant
                    return "LFkVMc";
                    break;
                case 10:
                    //peoples cider
                    return "iebYze";
                    break;
                case 11:
                    //Schmohz
                    return "AVEsqU";
                    break;
                case 12:
                    //hideout
                    return "35YJeP";
                    break;
                case 13:
                    //Atwater
                    return "boTIWO";
                    break;
                case 14:
                    //new holland
                    return "AqEUBQ";
                    break;
                default:
                    break;
            }
            return null;
        }

        //builds brewery objects using local json data
        public List<Brewery> LocalBrewery(double abv, double ibu, double srm)
        {
            List<Brewery> localBrews = new List<Brewery>();

            string SchmozPath = @"C:\Users\GC Student\Source\Repos\Craft Beer Me\Craft Beer Me\Controllers\Schmoz.json";
            StreamReader rd = new StreamReader(SchmozPath);
            string beerData = rd.ReadToEnd();
            JObject SchmozJson = JObject.Parse(beerData);

            Brewery schmoz = MakeABrewery(SchmozJson, 1, abv, ibu, srm);
            if (schmoz != null)
            {
                localBrews.Add(schmoz);
            }
                        

            string JollyPath = @"C:\Users\GC Student\Source\Repos\Craft Beer Me\Craft Beer Me\Controllers\JollyPumpkin.json";
            StreamReader rd2 = new StreamReader(JollyPath);
            string JollyData = rd2.ReadToEnd();
            JObject JollyJson = JObject.Parse(JollyData);

            Brewery jolly = MakeABrewery(JollyJson, 2, abv, ibu, srm);
            if (jolly != null)
            {
                localBrews.Add(jolly);
            }

            string AtwaterPath =  @"C:\Users\GC Student\Source\Repos\Craft Beer Me\Craft Beer Me\Controllers\Atwater.json";
            StreamReader rd3 = new StreamReader(AtwaterPath);
            string AtwaterData = rd3.ReadToEnd();
            JObject AtwaterJson = JObject.Parse(AtwaterData);

            Brewery atwater = MakeABrewery(AtwaterJson, 3, abv, ibu, srm);
            if (atwater != null)
            {
                localBrews.Add(atwater);
            }

            string NewPath = @"C:\Users\GC Student\Source\Repos\Craft Beer Me\Craft Beer Me\Controllers\NewHolland2.json";
            StreamReader rd4 = new StreamReader(NewPath);
            string NewData = rd4.ReadToEnd();
            JObject NewJson = JObject.Parse(NewData);

            Brewery holland = (MakeABrewery(NewJson, 4, abv, ibu, srm));
            if (holland != null)
            {
                localBrews.Add(holland);
            }

            return localBrews;
        }

        //makes each new brewery object from JSON
        public Brewery MakeABrewery(JObject beerJson, int x, double abv, double ibu, double srm)
        {
            Brewery GrandCircus = new Brewery();

            
            //GrandCircus.BreweryID = beerJson[];

            switch (x)
            {
                case 1:
                    GrandCircus.Name = "Schmoz";
                    GrandCircus.Url = "http://www.schmoz.com";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/AVEsqU/upload_uRmLOu-squareLarge.png";
                    break;
                case 2:
                    GrandCircus.Name = "Jolly Pumpkin";
                    GrandCircus.Url = "http://brewery.jollypumpkin.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/pzWq1r/upload_2YHJS9-squareLarge.png";
                    break;
                case 3:
                    GrandCircus.Name = "Atwater";
                    GrandCircus.Url = "https://www.atwaterbeer.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/boTIWO/upload_qHbhaE-squareLarge.png";
                    break;
                case 4:
                    GrandCircus.Name = "New Holland";
                    GrandCircus.Url = "http://newhollandbrew.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/AqEUBQ/upload_0xEGxj-squareLarge.png";
                    break;
                default:
                    break;
            }

            List<Beer> menu = new List<Beer>();

            Array beerArray = beerJson["data"].ToArray();

            //beerArray.Length
            for (int i = 0; i < beerArray.Length; i++)
            {
                //Evaluate here
                Beer newBeer = new Beer();
                newBeer = MakeABeer(beerJson, i);
                if (LimitBeer(newBeer, abv, ibu, srm))
                {
                    menu.Add(newBeer);
                }
                
                
            }
            
            GrandCircus.Menu = menu;

            //Only display breweries that have at least one beer in their menu
            if (GrandCircus.Menu.Count > 0)
            {
                return GrandCircus;
            }
            else
            {
                return null;
            }
        }

        //fills the menu with valid beers based on user parameters
        public Beer MakeABeer(JObject beerJson, int x)
        {
            Beer craftBeer = new Beer();

            //Note: Not all JSON beers have the category 'Available' hmmm...


            craftBeer.BeerName = beerJson["data"][x]["name"].ToString();

            //Description
            if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["style"]["description"] != null)
            {
                craftBeer.Description = beerJson["data"][x]["style"]["description"].ToString();
            }
            else
            {
                craftBeer.Description = null;
            }

            //ABV
            if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["style"]["abvMin"] != null)
            {
                craftBeer.ABV = (double)beerJson["data"][x]["style"]["abvMin"];
            }
            else if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["style"]["abvMax"] != null)
            {
                craftBeer.ABV = (double)beerJson["data"][x]["style"]["abvMax"];
            }
            else if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["abv"] != null)
            {
                craftBeer.ABV = (double)beerJson["data"][x]["abv"];
            }
            else
            {
                craftBeer.ABV = 0;
            }

            //IBU
            if (beerJson["data"][x]["ibu"] != null)
            {
                craftBeer.IBU = (double)beerJson["data"][x]["ibu"];
            }
            else if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["style"]["ibuMin"] != null)
            {
                craftBeer.IBU = (double)beerJson["data"][x]["style"]["ibuMin"];
            }
            else if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["style"]["ibuMax"] != null)
            {
                craftBeer.IBU = (double)beerJson["data"][x]["style"]["ibuMax"];
            }
            else
            {

                craftBeer.IBU = 0;
            }

            //SRM
            if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["style"]["srmMin"] != null)
            {
                craftBeer.SRM = (double)beerJson["data"][x]["style"]["srmMin"];
            }
            else
            {
                craftBeer.SRM = 0;
            }

            if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["style"]["shortName"] != null)
            {
                craftBeer.CategoryName = beerJson["data"][x]["style"]["shortName"].ToString();
            }
            else
            {
                craftBeer.CategoryName = null;
            }

            //Picture
            if (beerJson["data"][x]["labels"] != null && beerJson["data"][x]["labels"]["medium"] != null)
            {
                craftBeer.Picture = beerJson["data"][x]["labels"]["medium"].ToString();
            }
            else
            {
                craftBeer.Picture = null;
            }


            //db.Beers.Add(craftBeer);
            //db.SaveChanges();


            return craftBeer;
        }

        //only adds beers into the menu based on user input
        //Beer is valid if it passes three tests based on switch statments
        public bool LimitBeer(Beer beer, double abv, double ibu, double srm)
        {
            int counter = 0;
            switch (abv)
            {
                case 1:
                    if (beer.ABV >= 1 && beer.ABV <= 5.1)
                    {
                        counter++;
                    }
                    break;
                case 2:
                    if (beer.ABV >= 5.1 && beer.ABV <= 9.1)
                    {
                        counter++;
                    }
                    break;
                case 3:
                    if (beer.ABV >= 9.1 )
                    {
                        counter++;
                    }
                    break;
                default:
                    break;
            }

            switch (ibu)
            {
                case 1:
                    if (beer.IBU <= 30 && beer.IBU > 0)
                    {
                        counter++;
                    }
                    break;
                case 2:
                    if (beer.IBU <= 60 && beer.IBU >= 30)
                    {
                        counter++;
                    }
                    break;
                case 3:
                    if (beer.IBU >= 60)
                    {
                        counter++;
                    }
                    break;
                default:
                    break;
            }

            switch (srm)
            {
                case 1:
                    if (beer.SRM <= 10 && beer.SRM > 0)
                    {
                        counter++;
                    }
                    break;
                case 2:
                    if (beer.SRM <= 21 && beer.IBU >= 10)
                    {
                        counter++;
                    }
                    break;
                case 3:
                    if (beer.SRM >= 21)
                    {
                        counter++;
                    }
                    break;
                default:
                    break;
            }

            if (counter == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}