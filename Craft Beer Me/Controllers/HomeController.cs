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

      

        public ActionResult results(bool Atwater, bool Brewery, bool Elk, bool Founders, bool Harmony, bool Hideout, bool Hopcat, bool Jolly, bool New, bool Peoples, bool Perrin, bool Rockford, bool Schmohz, bool The)
        {

            string breweries = "";

            if (Atwater == true)
            {
                breweries += "Atwater Brewery/";
            }

            if (Brewery == true)
            {
                breweries += "Brewery Vivant/";
            }

            if (Elk == true)
            {
                breweries += "Elk Brewing/";
            }

            if (Founders == true)
            {
                breweries += "Founders Brewery/";
            }

            if (Harmony == true)
            {
                breweries += "Harmony Brewing/";
            }

            if (Hideout == true)
            {
                breweries += "Hideout Brewing/";
            }

            if (Hopcat == true)
            {
                breweries += "Hopcate Brewing/";
            }

            if (Jolly == true)
            {
                breweries += "Jolly Pumpkin Brewing/";
            }

            if (New == true)
            {
                breweries += "New Holland Knickerbocker/";
            }

            if (Peoples == true)
            {
                breweries += "Peoples Cider/";
            }

            if (Perrin == true)
            {
                breweries += "Perrin Brewery/";
            }

            if (Rockford == true)
            {
                breweries += "Rockfrod Brewing/";
            }

            if (Schmohz == true)
            {
                breweries += "Schmohz Brewing/";
            }

            if (The)
            {
                breweries += "The Mitten Brewing/";
            }
            ViewBag.ToVisit = breweries;

            //return RedirectToAction("https://www.google.com/maps/dir//");
            return View();
        }

        public ActionResult Recommended()
        {
         
            return View();
            
        }

        public ActionResult BeerNums(string ABV, string IBU, string SRM, string flavor)
        {
            double abv, ibu, srm;
            abv = double.Parse(ABV);
            ibu = double.Parse(IBU);
            srm = double.Parse(SRM);

            List<Brewery> breweries = GetBreweries(abv, ibu, srm, flavor);
            if (breweries.Count() > 0)
            {

            }
            Session["breweries"] = breweries;
            return RedirectToAction("Recommended");

        }

        public ActionResult styleSearch(string style)
        {
            if(style != "1")
            {
                double abv = 0;
                double ibu = 0;
                double srm = 0;
                string flavor = style;

                List<Brewery> breweries = GetBreweries(abv, ibu, srm, flavor);

                Session["breweries"] = breweries;
                return RedirectToAction("Recommended");
            }
            else
            {
               return RedirectToAction("/Index");
            }
            
        }

        //Begins the process of creating brewery objects and directs it to either local data or live data
        public List<Brewery> GetBreweries(double abv, double ibu, double srm, string flavor)
        {
            List<Brewery> breweries = new List<Brewery>();

            //This bool is to quickly switch between live db and local data
            bool isdbDown = true;

            //gets results for each of out 14 craft brewries
            if (isdbDown)
            {
                for (int i = 0; i < 5; i++)
                {
                    breweries = LocalBrewery(abv, ibu, srm, flavor);
                }
            }
            else
            {
                for (int i = 1; i < 15; i++)
                {

                    //test url
                    string urlString = "https://api.brewerydb.com/v2/" + "brewery/" + "pzWq1r" + "/beers?key=";


                    HttpWebRequest request = WebRequest.CreateHttp(urlString);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader rd = new StreamReader(response.GetResponseStream());
                    string beerData = rd.ReadToEnd();
                    JObject beerJson = JObject.Parse(beerData);


                    Brewery apiBrewery = MakeABrewery(beerJson, 2, abv, ibu, srm, flavor);
                    if (apiBrewery != null)
                    {
                       breweries.Add(apiBrewery);
                    }
                   
                                       

                    //api limits to 10 requests a second, this *should* solve that
                    //Thread.Sleep(150);
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
        public List<Brewery> LocalBrewery(double abv, double ibu, double srm, string flavor)
        {
            List<Brewery> localBrews = new List<Brewery>();

           
            string localPath = LocalFilePath(3);

            //need to fix file paths for all ***
     
           string SchmozPath = localPath + @"\Schmohz JSON.json";


            StreamReader rd = new StreamReader(SchmozPath);
            string beerData = rd.ReadToEnd();
            JObject SchmozJson = JObject.Parse(beerData);

            Brewery schmoz = MakeABrewery(SchmozJson, 1, abv, ibu, srm, flavor);
            if (schmoz != null)
            {
                localBrews.Add(schmoz);
            }


            string JollyPath = localPath + @"\Jolly Pumpkin JSON.json";


            StreamReader rd2 = new StreamReader(JollyPath);
            string JollyData = rd2.ReadToEnd();
            JObject JollyJson = JObject.Parse(JollyData);

            Brewery jolly = MakeABrewery(JollyJson, 2, abv, ibu, srm, flavor);
            if (jolly != null)
            {
                localBrews.Add(jolly);
            }


            string AtwaterPath = localPath + @"\Atwater JSON.json";


            StreamReader rd3 = new StreamReader(AtwaterPath);
            string AtwaterData = rd3.ReadToEnd();
            JObject AtwaterJson = JObject.Parse(AtwaterData);

            Brewery atwater = MakeABrewery(AtwaterJson, 3, abv, ibu, srm, flavor);
            if (atwater != null)
            {
                localBrews.Add(atwater);
            }

            string NewPath = localPath + @"\New Holland JSON.json";

            StreamReader rd4 = new StreamReader(NewPath);
            string NewData = rd4.ReadToEnd();
            JObject NewJson = JObject.Parse(NewData);

            Brewery holland = (MakeABrewery(NewJson, 4, abv, ibu, srm, flavor));
            if (holland != null)
            {
                localBrews.Add(holland);
            }
            string VivantPath = localPath + @"\Brewery Vivant JSON.json";

            StreamReader rd5 = new StreamReader(VivantPath);
            string VivantData = rd5.ReadToEnd();
            JObject VivantJson = JObject.Parse(VivantData);
            Brewery vivant = (MakeABrewery(VivantJson, 5, abv, ibu, srm, flavor));
            if (vivant != null)
            {
                localBrews.Add(vivant);
            }

            string ElkPath = localPath + @"\Elk Brewing JSON.json";

            StreamReader rd6 = new StreamReader(ElkPath);
            string ElkData = rd6.ReadToEnd();
            JObject ElkJson = JObject.Parse(ElkData);
            Brewery elk = (MakeABrewery(ElkJson, 6, abv, ibu, srm, flavor));
            if (elk != null)
            {
                localBrews.Add(elk);
            }

            string FoundersPath = localPath + @"\Founders JSON.json";

            StreamReader rd7 = new StreamReader(FoundersPath);
            string FoundersData = rd7.ReadToEnd();
            JObject FoundersJson = JObject.Parse(FoundersData);
            Brewery founders = (MakeABrewery(FoundersJson, 7, abv, ibu, srm, flavor));
            if (founders != null)
            {
                localBrews.Add(founders);
            }

            string HarmonyPath = localPath + @"\Harmony JSON.json";

            StreamReader rd8 = new StreamReader(HarmonyPath);
            string HarmonyData = rd8.ReadToEnd();
            JObject HarmonyJson = JObject.Parse(HarmonyData);
            Brewery harmony = (MakeABrewery(HarmonyJson, 8, abv, ibu, srm, flavor));
            if (harmony != null)
            {
                localBrews.Add(harmony);
            }

            string HideoutPath = localPath + @"\Hideout JSON.json";

            StreamReader rd9 = new StreamReader(HideoutPath);
            string HideoutData = rd9.ReadToEnd();
            JObject HideoutJson = JObject.Parse(HideoutData);
            Brewery hideout = (MakeABrewery(HideoutJson, 9, abv, ibu, srm, flavor));
            if (hideout != null)
            {
                localBrews.Add(hideout);
            }

            string PeoplesPath = localPath + @"\Peoples Cider JSON.json";

            StreamReader rd10 = new StreamReader(PeoplesPath);
            string PeoplesData = rd10.ReadToEnd();
            JObject PeoplesJson = JObject.Parse(PeoplesData);
            Brewery peoples = (MakeABrewery(PeoplesJson, 10, abv, ibu, srm, flavor));
            if (peoples != null)
            {
                localBrews.Add(peoples);
            }

            string PerrinPath = localPath + @"\Perrin JSON.json";

            StreamReader rd11 = new StreamReader(PerrinPath);
            string PerrinData = rd11.ReadToEnd();
            JObject PerrinJson = JObject.Parse(PerrinData);
            Brewery perrin = (MakeABrewery(PerrinJson, 11, abv, ibu, srm, flavor));
            if (perrin != null)
            {
                localBrews.Add(perrin);
            }

            string RockPath = localPath + @"\Rockford Brewing JSON.json";

            StreamReader rd12 = new StreamReader(RockPath);
            string RockData = rd12.ReadToEnd();
            JObject RockJson = JObject.Parse(RockData);
            Brewery rock = (MakeABrewery(RockJson, 12, abv, ibu, srm, flavor));
            if (rock != null)
            {
                localBrews.Add(rock);
            }

            string MittenPath = localPath + @"\The Mitten JSON.json";

            StreamReader rd13 = new StreamReader(MittenPath);
            string MittenData = rd13.ReadToEnd();
            JObject MittenJson = JObject.Parse(MittenData);
            Brewery mitten = (MakeABrewery(MittenJson, 13, abv, ibu, srm, flavor));
            if (mitten != null)
            {
                localBrews.Add(mitten);
            }

            string HopcatPath = localPath + @"\hopcat json.json";

            StreamReader rd14 = new StreamReader(HopcatPath);
            string HopcatData = rd14.ReadToEnd();
            JObject HopcatJson = JObject.Parse(HopcatData);
            Brewery hopcat = (MakeABrewery(HopcatJson, 14, abv, ibu, srm, flavor));
            if (hopcat != null)
            {
                localBrews.Add(hopcat);
            }



            return localBrews;
        }

        //switches json filepaths because this 
        public static string LocalFilePath(int x)
        {
            if (x == 1)
            {
                return @"C:\Users\katea\Source\Repos\Final\Cure-for-the-Common-Brew\Craft Beer Me\Controllers";
            }
            else if (x == 2)
            {
                return @"C:\Users\GC Student\Source\Repos\Craft Beer Me\Craft Beer Me\Controllers\";
            }
            else if (x ==3) 
            {
                return @"C:\Users\mfilice\source\repos\Cure for the Common Brew\Cure-for-the-Common-Brew\Craft Beer Me\Controllers";
            }
            else
            {
                return "x";
            }
        }

        //makes each new brewery object from JSON
        public Brewery MakeABrewery(JObject beerJson, int x, double abv, double ibu, double srm, string flavor)
        {
            Brewery GrandCircus = new Brewery();
            
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
                case 5:
                    GrandCircus.Name = "Brewery Vivant";
                    GrandCircus.Url = "http://www.breweryvivant.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/LFkVMc/upload_GhuNYz-squareLarge.png";
                    break;
                case 6:
                    GrandCircus.Name = "Elk Brewing";
                    GrandCircus.Url = "http://elkbrewing.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/sjblac/upload_2QxSy3-squareLarge.png";
                    break;
                case 7:
                    GrandCircus.Name = "Founders";
                    GrandCircus.Url = "http://www.foundersbrewing.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/Idm5Y5/upload_O8MoRg-squareLarge.png";
                    break;
                case 8:
                    GrandCircus.Name = "Harmony";
                    GrandCircus.Url = "https://harmonybeer.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/P0oEwB/upload_5Ngoxq-squareLarge.png";
                    break;
                case 9:
                    GrandCircus.Name = "Hideout";
                    GrandCircus.Url = "http://hideoutbrewing.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/35YJeP/upload_eNle75-squareLarge.png";
                    break;
                case 10:
                    GrandCircus.Name = "The People's Cider";
                    GrandCircus.Url = "http://www.thepeoplescider.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/iebYze/upload_bv6mpy-squareLarge.png";
                    break;
                case 11:
                    GrandCircus.Name = "Perrin Brewing Company";
                    GrandCircus.Url = "http://www.perrinbrewing.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/Boa6td/upload_6ADB1F-squareLarge.png";
                    break;
                case 12:
                    GrandCircus.Name = "Rockford Brewing Company";
                    GrandCircus.Url = "https://www.rockfordbrewing.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/U92Ctx/upload_6NaOBl-squareLarge.png";
                    break;
                case 13:
                    GrandCircus.Name = "The Mitten";
                    GrandCircus.Url = "http://www.mittenbrewing.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/bdFoir/upload_NNuOnt-squareLarge.png";
                    break;
                case 14:
                    GrandCircus.Name = "HopCat";
                    GrandCircus.Url = "http://hopcat.com/";
                    GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/HizvxH/upload_oqijUs-squareLarge.png";
                    break;
                default:
                    break;

            }
            
            GrandCircus.Menu = FillaMenu(beerJson, abv, ibu, srm, flavor);

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

        //makes a menu of beer objects that conform to search parameters
        public List<Beer> FillaMenu(JObject beerJson, double abv, double ibu, double srm, string flavor)
        {
            List<Beer> menu = new List<Beer>();

            //if the user doesn't put anything flavor is set to a space so that every result is true
          

            //this array is created for the sole purpose of finding the lenght of the data object
            Array beerArray = beerJson["data"].ToArray();
            for (int i = 0; i < beerArray.Length; i++)
            {
                //Evaluate here
                Beer newBeer = new Beer();
                newBeer = MakeABeer(beerJson, i);
                if (flavor == null)
                {
                    if(LimitBeer(newBeer, abv, ibu, srm))
                    {
                        menu.Add(newBeer);
                    }
                    
                }
                else if (abv == 0 )
                {
                    if (LimitBeer(newBeer, flavor))
                    {
                        menu.Add(newBeer);
                    }
                    
                }
               else if (LimitBeer(newBeer, abv, ibu, srm, flavor))
                {
                    menu.Add(newBeer);
                }
                
            }
            
          if (menu != null)
            {


            }
            menu = RandoSort(menu);

            return menu;
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
            if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["abv"] != null)
            {
                craftBeer.ABV = (double)beerJson["data"][x]["abv"];
            }
            else if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["style"]["abvMin"] != null)
            {
                craftBeer.ABV = (double)beerJson["data"][x]["style"]["abvMin"];
            }
            else if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["style"]["abvMax"] != null)
            {
                craftBeer.ABV = (double)beerJson["data"][x]["style"]["abvMax"];

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
            else if (beerJson["data"][x]["style"] != null 
                && beerJson["data"][x]["style"]["ibuMin"] != null)
            {
                craftBeer.IBU = (double)beerJson["data"][x]["style"]["ibuMin"];
            }
            else if (beerJson["data"][x]["style"] != null 
                && beerJson["data"][x]["style"]["ibuMax"] != null)
            {
                craftBeer.IBU = (double)beerJson["data"][x]["style"]["ibuMax"];
            }
            else
            {

                craftBeer.IBU = 0;
            }

            //SRM
            if (beerJson["data"][x]["style"] != null 
                && beerJson["data"][x]["style"]["srmMin"] != null)
            {
                craftBeer.SRM = (double)beerJson["data"][x]["style"]["srmMin"];
            }
            else
            {
                craftBeer.SRM = 0;
            }

            if (beerJson["data"][x]["style"] != null 
                && beerJson["data"][x]["style"]["name"] != null)
            {
                craftBeer.CategoryName = beerJson["data"][x]["style"]["name"].ToString();
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
        //Beer is valid if it passes all four tests based on switch statments
        public bool LimitBeer(Beer beer, double abv, double ibu, double srm, string flavor)
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
                    if (beer.SRM <= 8 && beer.SRM > 1)
                    {
                        counter++;
                    }
                    break;
                case 2:
                    if (beer.SRM <= 8 && beer.IBU >= 16)
                    {
                        counter++;
                    }
                    break;
                case 3:
                    if (beer.SRM >= 16 && beer.IBU >= 24)
                    {
                        counter++;
                    }
                    break;
                case 4:
                    if (beer.SRM >= 24 && beer.IBU >= 32)
                    {
                        counter++;
                    }
                    break;
                case 5:
                    if (beer.SRM >= 32 && beer.IBU >= 40)
                    {
                        counter++;
                    }
                    break;
                default:
                    break;
            }

            if (beer.Description != null)
            {
                if (beer.Description.Contains(flavor))
                {
                    counter++;
                }
            }

            if (counter == 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Overload for adding only valid beers this one does not use flavor as a parameter
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
                    if (beer.ABV >= 9.1)
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
                    if (beer.SRM <= 8 && beer.SRM > 1)
                    {
                        counter++;
                    }
                    break;
                case 2:
                    if (beer.SRM <= 8 && beer.IBU >= 16)
                    {
                        counter++;
                    }
                    break;
                case 3:
                    if (beer.SRM >= 16 && beer.IBU >= 24)
                    {
                        counter++;
                    }
                    break;
                case 4:
                    if (beer.SRM >= 24 && beer.IBU >= 32)
                    {
                        counter++;
                    }
                    break;
                case 5:
                    if (beer.SRM >= 32 && beer.IBU >= 40)
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

        //Overload for adding only valid beers this one by style
        public bool LimitBeer(Beer beer, string style)
        {
            if (beer.CategoryName.Contains(style))
            {
                return true;
            }
            return false;
        }

        //randomly sorts the beer list so that the top six results are not the same every time
        public static List<Beer> RandoSort(List<Beer> beers)
        {
            Random r = new Random();
            int n = beers.Count;

            for (int i = beers.Count - 1; i > 1; i--)
            {
                int rnd = r.Next(i + 1);

                Beer value = beers[rnd];
                beers[rnd] = beers[i];
                beers[i] = value;
            }
            return beers;
        }
    }
}