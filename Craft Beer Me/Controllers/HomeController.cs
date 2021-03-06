﻿using Craft_Beer_Me.Models;
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
        private BreweryContext db = new BreweryContext();
        BeerComparison BeerComparisons = new BeerComparison();

        public ActionResult Index()
        {
            //this makes the app load correctly from either HomeController or the Index
            //PopulateDB();
            string currentUrl = Request.Url.AbsoluteUri;

            if (currentUrl.Contains("/index"))
            {
                return View();
            }
            return RedirectToAction("/index");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult StepTwo()
        {
            return View();
        }

        public ActionResult StepThree()
        {
            return View();
        }

        public ActionResult QualitySearch()
        {
            return View();
        }

        public ActionResult SearchByStyle()
        {
            return View();
        }

        public ActionResult PopularSearch()
        {
            return View();
        }

        public ActionResult Flight()
        {
            return View();
        }

        //google maps redirect
        public ActionResult googleTour(string Atwater, string Vivant, string Elk, string Founders,
            string Harmony, string Hideout, string Hopcat, string Jolly, string Holland, string Peoples,
            string Perrin, string Rockford, string Schmohz, string Mitten)
        {
            string breweries = SelfGuidedTour(Atwater, Vivant, Elk, Founders, Harmony,
                Hideout, Hopcat, Jolly, Holland, Peoples, Perrin, Rockford, Schmohz, Mitten);

            string mapsGoogle = "https://www.google.com/maps/dir/my+location/" + breweries;

            return Redirect(mapsGoogle);
        }

        //the "error" page
        public ActionResult results()
        {
            return View();
        }

        //the view where we show the list of breweries with the list of beers also has links to 
        public ActionResult Recommended()
        {
            return View();
        }

        public ActionResult KnownBrews()
        {
            return View();
        }

        //This is the start of finding a users tastes
        public ActionResult PicBrews(int brews)
        {
            //creates and returns this Brewery's object with all beers. 
            Brewery brewery = db.Breweries.Find(brews);
            brewery.Menu = FillaMenu(brews, 1);
            //Displays beers and allows user to select 2-4 that they enjoy from the brewery
            //With a table & checkbox? with a search bar and suggested list?
            Session["barMenu"] = brewery;
            return View();
        }

        public ActionResult IbuAbvSrmResults(Beer beer1, Beer beer2)
        {
            
           abvStyle(beer1, beer2);
           IbuStyle(beer1, beer2);
           SrmStyle(beer1, beer2);
            
           List<string> topScores = scoreSorter();
            ViewBag.Pref = FindBeerPreference(topScores);
           return View();
        }

        public string FindBeerPreference(List<string> topScores)
        {
            if (topScores[0] == "ABV")
            {
                if (BeerComparisons.ABVCat == 1)
                {
                    return "";
                }
                else if (BeerComparisons.ABVCat == 2)
                {
                    return "";
                }
                else if (BeerComparisons.ABVCat == 3)
                {
                    return "You seem to like beers with very high alcohol content./nThat is an ABV about 9%.";
                }
            }
            return "";
        }

        public List<string> scoreSorter()
        {
            List<string> topScores = new List<string>();
            if (BeerComparisons.ABVScore > BeerComparisons.IBUScore && BeerComparisons.ABVScore > BeerComparisons.SRMScore)
            {
                topScores.Add("ABV");
                if (BeerComparisons.IBUScore > BeerComparisons.SRMScore)
                {
                    topScores.Add("IBU");
                    topScores.Add("SRM");
                }
                else
                {
                    topScores.Add("SRM");
                    topScores.Add("IBU");
                }
            }
            else if (BeerComparisons.IBUScore > BeerComparisons.ABVScore && BeerComparisons.IBUScore > BeerComparisons.SRMScore)
            {
                topScores.Add("IBU");
                if (BeerComparisons.ABVScore > BeerComparisons.SRMScore)
                {
                    topScores.Add("ABV");
                    topScores.Add("SRM");
                }
                else
                {
                    topScores.Add("SRM");
                    topScores.Add("ABV");
                }
            }
            else if (BeerComparisons.SRMScore > BeerComparisons.ABVScore && BeerComparisons.SRMScore > BeerComparisons.IBUScore)
            {
                topScores.Add("SRM");
                if (BeerComparisons.ABVScore > BeerComparisons.IBUScore)
                {
                    topScores.Add("ABV");
                    topScores.Add("IBU");
                }
                else
                {
                    topScores.Add("IBU");
                    topScores.Add("ABV");

                }
            }
            else
            {
                topScores.Add("Equal");
                topScores.Add("Equal");
                topScores.Add("Equal");

            }
            return topScores;
        }
        //This action takes each quality from user input and finds the appropriate beers
        public ActionResult BeerNums(string ABV, string IBU, string SRM, string flavor)
        {

            double abv, ibu, srm;
            abv = double.Parse(ABV);
            ibu = double.Parse(IBU);
            srm = double.Parse(SRM);

            List<Brewery> breweries = MakeBreweryList(abv, ibu, srm, flavor);
            if (breweries.Count() == 0)
            {
                return RedirectToAction("results");
            }
            Session["breweries"] = breweries;
            return RedirectToAction("Recommended");

        }

        public ActionResult styleSearch(string style)
        {
            if (style != "1")
            {
                double abv = 0;
                double ibu = 0;
                double srm = 0;
                string flavor = style;

                List<Brewery> breweries = MakeBreweryList(abv, ibu, srm, flavor);

                Session["breweries"] = breweries;
                return RedirectToAction("Recommended");
            }
            else
            {
                return RedirectToAction("/Index");
            }

        }

        public ActionResult FlightExplorer(int flighty)
        {
            Session["Cider"] = null;
            if (flighty == 10)
            {
                Brewery brewery = db.Breweries.Find(flighty);
                List<Beer> empty = new List<Beer>();
                brewery.Menu = empty;
                Session["breweries"] = brewery;
                Session["Cider"] = "Sadly, we were not able to populate a flight for you because they only have hard cider." +
                    " I'm sure it's great, but we're all disappointed.";
                return View();
            }
            else
            {
                Brewery brewery = db.Breweries.Find(flighty);
                brewery.Menu = FillaMenu(flighty);
                Session["breweries"] = brewery;
                return View();
            }

        }



        //Begins the process of creating brewery objects and directs it to either local data or live data
        public List<JObject> GetJson(double abv, double ibu, double srm, string flavor)
        {
            List<JObject> jObjects = new List<JObject>();

            //This bool is to quickly switch between live db and local data
            bool isAPIDown = true;

            //gets results for each of out 14 craft brewries
            if (isAPIDown)
            {
                jObjects = LocalBrewery(abv, ibu, srm, flavor);
            }
            else
            {
                //test url
                foreach (Brewery b in db.Breweries)
                {
                    string urlString = "https://api.brewerydb.com/v2/" + "brewery/" + b.BreweryID + "/beers?key=";
                    HttpWebRequest request = WebRequest.CreateHttp(urlString);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader rd = new StreamReader(response.GetResponseStream());
                    string beerData = rd.ReadToEnd();
                    JObject beerJson = JObject.Parse(beerData);
                    jObjects.Add(beerJson);
                }

                //api limits to 10 requests a second, this *should* solve that
                Thread.Sleep(150);

            }

            return jObjects;
        }

        //Overload of GetJson to access a single brewery by db index only set to local JSON at the moment
        public JObject GetJson(int x)
        {
            JObject justOne = new JObject();
            string jPath = "";

            switch (x)
            {
                case 1:
                    jPath = @"\Schmohz JSON.json";
                    break;
                case 2:
                    jPath = @"\Jolly Pumpkin JSON.json";
                    break;
                case 3:
                    jPath = @"\Atwater JSON.json";
                    break;
                case 4:
                    jPath = @"\New Holland JSON.json";
                    break;
                case 5:
                    jPath = @"\Brewery Vivant JSON.json";
                    break;
                case 6:
                    jPath = @"\Elk Brewing JSON.json";
                    break;
                case 7:
                    jPath = @"\Founders JSON.json";
                    break;
                case 8:
                    jPath = @"\Harmony JSON.json";
                    break;
                case 9:
                    jPath = @"\Hideout JSON.json";
                    break;
                case 10:
                    jPath = @"\Peoples Cider JSON.json";
                    break;
                case 11:
                    jPath = @"\Perrin JSON.json";
                    break;
                case 12:
                    jPath = @"\Rockford Brewing JSON.json";
                    break;
                case 13:
                    jPath = @"\The Mitten JSON.json";
                    break;
                case 14:
                    jPath = @"\hopcat json.json";
                    break;
                default:
                    break;
            }

            string localPath = LocalFilePath(1) + jPath;
            justOne = GetJSONFromLocal(localPath);


            return justOne;
        }

        //Does the stream reading and converting to JSON
        public JObject GetJSONFromLocal(string path)
        {
            StreamReader rd = new StreamReader(path);
            string beerData = rd.ReadToEnd();
            JObject Json = JObject.Parse(beerData);

            return Json;
        }

        //builds brewery objects using local json data
        public List<JObject> LocalBrewery(double abv, double ibu, double srm, string flavor)
        {
            List<JObject> localBrews = new List<JObject>();

            string localPath = LocalFilePath(1);

            string SchmozPath = localPath + @"\Schmohz JSON.json";
            JObject SchmozJson = GetJSONFromLocal(SchmozPath);

            localBrews.Add(SchmozJson);

            string JollyPath = localPath + @"\Jolly Pumpkin JSON.json";
            JObject JollyJson = GetJSONFromLocal(JollyPath);

            localBrews.Add(JollyJson);

            string AtwaterPath = localPath + @"\Atwater JSON.json";
            JObject AtwaterJson = GetJSONFromLocal(AtwaterPath);

            localBrews.Add(AtwaterJson);

            string NewPath = localPath + @"\New Holland JSON.json";
            JObject NewJson = GetJSONFromLocal(NewPath);

            localBrews.Add(NewJson);

            string VivantPath = localPath + @"\Brewery Vivant JSON.json";
            JObject VivantJson = GetJSONFromLocal(VivantPath);

            localBrews.Add(VivantJson);

            string ElkPath = localPath + @"\Elk Brewing JSON.json";
            JObject ElkJson = GetJSONFromLocal(ElkPath);

            localBrews.Add(ElkJson);

            string FoundersPath = localPath + @"\Founders JSON.json";
            JObject FoundersJson = GetJSONFromLocal(FoundersPath);

            localBrews.Add(FoundersJson);

            string HarmonyPath = localPath + @"\Harmony JSON.json";
            JObject HarmonyJson = GetJSONFromLocal(HarmonyPath);

            localBrews.Add(HarmonyJson);

            string HideoutPath = localPath + @"\Hideout JSON.json";
            JObject HideoutJson = GetJSONFromLocal(HideoutPath);

            localBrews.Add(HideoutJson);

            string PeoplesPath = localPath + @"\Peoples Cider JSON.json";
            JObject PeoplesJson = GetJSONFromLocal(PeoplesPath);
            ;
            localBrews.Add(PeoplesJson);

            string PerrinPath = localPath + @"\Perrin JSON.json";
            JObject PerrinJson = GetJSONFromLocal(PerrinPath);

            localBrews.Add(PerrinJson);

            string RockPath = localPath + @"\Rockford Brewing JSON.json";
            JObject RockJson = GetJSONFromLocal(RockPath);

            localBrews.Add(RockJson);

            string MittenPath = localPath + @"\The Mitten JSON.json";
            JObject MittenJson = GetJSONFromLocal(MittenPath);

            localBrews.Add(MittenJson);

            string HopcatPath = localPath + @"\hopcat json.json";
            JObject HopcatJson = GetJSONFromLocal(HopcatPath);

            localBrews.Add(HopcatJson);

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
            else if (x == 3)
            {
                return @"C:\Users\mfilice\source\repos\Cure for the Common Brew\Cure-for-the-Common-Brew\Craft Beer Me\Controllers";
            }
            else
            {
                return "x";
            }
        }

        //Creates list of breweries that give returns on beer search parameters

        //uses Database to pull the brewery objects
        public List<Brewery> MakeBreweryList(double abv, double ibu, double srm, string flavor)
        {
            List<Brewery> breweries = new List<Brewery>();
            Brewery GrandCircus = new Brewery();
            for (int i = 1; i < 15; i++)
            {
                GrandCircus = db.Breweries.Find(i);

                GrandCircus.Menu = FillaMenu(i, abv, ibu, srm, flavor);

                //Only display breweries that have at least one beer in their menu
                if (GrandCircus.Menu.Count > 0)
                {
                    breweries.Add(GrandCircus);
                }
            }
            return breweries;
        }

        //overload for FillaMenu that takes one json to make one brewery
        public List<Beer> FillaMenu(int x)
        {
            List<Beer> menu = new List<Beer>();
            JObject beerJson = GetJson(x);
            List<Beer> ale = new List<Beer>();
            List<Beer> pilsner = new List<Beer>();
            List<Beer> lager = new List<Beer>();
            List<Beer> belgian = new List<Beer>();
            List<Beer> porter = new List<Beer>();
            List<Beer> stout = new List<Beer>();

            Array beerArray = beerJson["data"].ToArray();
            for (int i = 0; i < beerArray.Length; i++)
            {
                //Evaluate here
                Beer newBeer = new Beer();
                newBeer = MakeABeer(beerJson, i);
                if (LimitBeer(newBeer, "ale"))
                {
                    ale.Add(newBeer);
                }
                else if (LimitBeer(newBeer, "pils"))
                {
                    pilsner.Add(newBeer);
                }
                else if (LimitBeer(newBeer, "lager"))
                {
                    lager.Add(newBeer);
                }
                else if (LimitBeer(newBeer, "Belgian"))
                {
                    belgian.Add(newBeer);
                }
                else if (LimitBeer(newBeer, "Porter"))
                {
                    porter.Add(newBeer);
                }
                else if (LimitBeer(newBeer, "Stout"))
                {
                    stout.Add(newBeer);
                }
            }

            menu = RandyHandleThis(ale, pilsner, lager, belgian, porter, stout);

            // TODO: Verify validation
            if (menu != null)
            {


            }

            return menu;
        }

        //This overload for FillaMenu takes an extra int to seperate the methods, returns ALL beer from a brewery
        public List<Beer> FillaMenu(int x, int y)
        {
            List<Beer> menu = new List<Beer>();
            JObject beerJson = GetJson(x);

            Array beerArray = beerJson["data"].ToArray();
            for (int i = 0; i < beerArray.Length; i++)
            {
                //Evaluate here
                Beer newBeer = new Beer();
                newBeer = MakeABeer(beerJson, i);
                menu.Add(newBeer);
            }
            return menu;
        }

        //makes a menu of beer objects that conform to search parameters
        public List<Beer> FillaMenu(int x, double abv, double ibu, double srm, string flavor)
        {
            List<JObject> jObjects = GetJson(abv, ibu, srm, flavor);
            List<Beer> menu = new List<Beer>();

            //if the user doesn't put anything flavor is set to a space so that every result is true
            //this array is created for the sole purpose of finding the length of the data object
            JObject j = jObjects[x - 1];
            Array beerArray = j["data"].ToArray();

            for (int i = 0; i < beerArray.Length; i++)
            {
                //Evaluate here
                Beer newBeer = new Beer();
                newBeer = MakeABeer(j, i);
                if (flavor == null)
                {
                    if (LimitBeer(newBeer, abv, ibu, srm))
                    {
                        menu.Add(newBeer);
                    }

                }
                else if (abv == 0)
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
            // TOOO: Verify Validation
            if (menu != null)
            {


            }

            menu = RandoSort(menu);

            return menu;
        }

        //fills the menu with valid beers based on user parameters
        //parses through the json.
        public Beer MakeABeer(JObject beerJson, int x)
        {
            Beer craftBeer = new Beer();

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
            else if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["style"]["abvMax"] != null)
            {
                craftBeer.ABV = (double)beerJson["data"][x]["style"]["abvMax"];
            }
            else if (beerJson["data"][x]["style"] != null && beerJson["data"][x]["style"]["abvMin"] != null)
            {
                craftBeer.ABV = (double)beerJson["data"][x]["style"]["abvMin"];

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
                    if (beer.ABV >= 0 && beer.ABV <= 5.1)
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
                    if (beer.SRM >= 8 && beer.SRM <= 16)
                    {
                        counter++;
                    }
                    break;
                case 3:
                    if (beer.SRM >= 16 && beer.SRM <= 24)
                    {
                        counter++;
                    }
                    break;
                case 4:
                    if (beer.SRM >= 24 && beer.SRM <= 32)
                    {
                        counter++;
                    }
                    break;
                case 5:
                    if (beer.SRM >= 32 && beer.SRM <= 40)
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
                    if (beer.SRM >= 8 && beer.SRM <= 16)
                    {
                        counter++;
                    }
                    break;
                case 3:
                    if (beer.SRM >= 16 && beer.SRM <= 24)
                    {
                        counter++;
                    }
                    break;
                case 4:
                    if (beer.SRM >= 24 && beer.SRM <= 32)
                    {
                        counter++;
                    }
                    break;
                case 5:
                    if (beer.SRM >= 32 && beer.SRM <= 40)
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
        
        public void abvStyle(Beer beer1, Beer beer2)
        {
            
            int score = 0;
            int b1cat = 0;
            int b2cat = 9;
            if (beer1.ABV >= 1 && beer1.ABV <= 5.1)
            {
                b1cat = 1;
            }
            else if (beer1.ABV >= 5.1 && beer1.ABV <= 9.1)
            {
                b1cat = 2;
            }
            else if (beer1.ABV >= 9.1)
            {
                b1cat = 3;
            }

            if (beer2.ABV >= 1 && beer2.ABV <= 5.1)
            {
                b2cat = 1;
            }
            else if (beer2.ABV >= 5.1 && beer2.ABV <= 9.1)
            {
                b2cat = 2;
            }
            else if (beer2.ABV >= 9.1)
            {
                b2cat = 3;
            }
            if (b1cat == b2cat)
            {
                score++;
            }
            BeerComparisons.ABVScore = score;
            BeerComparisons.ABVCat = (b1cat + b2cat)/2;
            
        }

            //looks to see if IBU for two beers falls into the same ibu category (these categories match the LimitBeer method) and returns a score int
            public void IbuStyle(Beer beer1, Beer beer2)
        {
            
            int score = 0;
            int beer1cat = 0;
            int beer2cat = 9;

            if (beer1.IBU <= 30 && beer1.IBU > 0)
            {
                beer1cat = 1;
            }
            else if (beer1.IBU <= 60 && beer1.IBU >= 30)
            {
                beer1cat = 2;
            }
            else if (beer1.IBU >= 60)
            {
                beer1cat = 3;
            }

            if (beer2.IBU <= 30 && beer2.IBU > 0)
            {
                beer2cat = 1;
            }
            else if (beer2.IBU <= 60 && beer2.IBU >= 30)
            {
                beer2cat = 2;
            }
            else if (beer2.IBU >= 60)
            {
                beer2cat = 3;
            }

            if (beer1cat == beer2cat)
            {
                score++;
            }

            BeerComparisons.IBUScore = score;
            BeerComparisons.IBUCat = (beer1cat + beer2cat) / 2;
           
        }

        //looks to see if SRM for two beers falls into the same srm category (these categories match the LimitBeer method) and returns a score int
        public void SrmStyle(Beer beer1, Beer beer2)
        {
            
            int score = 0;
            int beer1cat = 0;
            int beer2cat = 9;

            if (beer1.SRM <= 8 && beer1.SRM > 1)
            {
                beer1cat = 1;
            }
            else if (beer1.SRM >= 8 && beer1.SRM <= 16)
            {
                beer1cat = 2;
            }
            else if (beer1.SRM >= 16 && beer1.SRM <= 24)
            {
                beer1cat = 3;
            }
            else if (beer1.SRM >= 24 && beer1.SRM <= 32)
            {
                beer1cat = 4;
            }
            else if (beer1.SRM >= 32 && beer1.SRM <= 40)
            {
                beer1cat = 5;
            }


            if (beer2.SRM <= 8 && beer2.SRM > 1)
            {
                beer2cat = 1;
            }
            else if (beer2.SRM >= 8 && beer2.SRM <= 16)
            {
                beer2cat = 2;
            }
            else if (beer2.SRM >= 16 && beer2.SRM <= 24)
            {
                beer2cat = 3;
            }
            else if (beer2.SRM >= 24 && beer2.SRM <= 32)
            {
                beer2cat = 4;
            }
            else if (beer2.SRM >= 32 && beer2.SRM <= 40)
            {
                beer2cat = 5;
            }

            if (beer1cat == beer2cat)
            {
                score++;
            }

            BeerComparisons.SRMScore = score;
            BeerComparisons.SRMCat = (beer1cat + beer2cat) / 2;
            
        }

        //allows the flight explorer to have an element of randomization
        public List<Beer> RandyHandleThis(List<Beer> ale, List<Beer> pilsner, List<Beer> lager, List<Beer> belgian, List<Beer> porter, List<Beer> stout)
        {
            List<Beer> final = new List<Beer>();

            ale = RandoSort(ale);
            pilsner = RandoSort(pilsner);
            lager = RandoSort(lager);
            belgian = RandoSort(belgian);
            porter = RandoSort(porter);
            stout = RandoSort(stout);

            //Any() searches for matching elements, in this case 'are there any elements in the list?'
            if (ale.Any())
            {
                final.Add(ale[0]);
            }

            if (pilsner.Any())
            {
                final.Add(pilsner[0]);
            }

            if (lager.Any())
            {
                final.Add(lager[0]);
            }

            if (belgian.Any())
            {
                final.Add(belgian[0]);
            }

            if (porter.Any())
            {
                final.Add(porter[0]);
            }

            if (stout.Any())
            {
                final.Add(stout[0]);
            }

            return final;
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

        //Concatonates a string for the guided tour
        public static string SelfGuidedTour(string Atwater, string Vivant, string Elk, string Founders, string Harmony, string Hideout, string Hopcat, string Jolly, string Holland, string Peoples, string Perrin, string Rockford, string Schmohz, string Mitten)
        {
            string breweries = "";
            if (Atwater == "on")
            {
                breweries += "Atwater Brewery/";
            }

            if (Vivant == "on")
            {
                breweries += "Brewery Vivant/";
            }

            if (Elk == "on")
            {
                breweries += "Elk Brewing/";
            }

            if (Founders == "on")
            {
                breweries += "Founders Brewery/";
            }

            if (Harmony == "on")
            {
                breweries += "Harmony Brewing/";
            }

            if (Hideout == "on")
            {
                breweries += "Hideout Brewing/";
            }

            if (Hopcat == "on")
            {
                breweries += "Hopcate Brewing/";
            }

            if (Jolly == "on")
            {
                breweries += "Jolly Pumpkin Brewing/";
            }

            if (Holland == "on")
            {
                breweries += "New Holland Knickerbocker/";
            }

            if (Peoples == "on")
            {
                breweries += "Peoples Cider/";
            }

            if (Perrin == "on")
            {
                breweries += "Perrin Brewery/";
            }

            if (Rockford == "on")
            {
                breweries += "Rockfrod Brewing/";
            }

            if (Schmohz == "on")
            {
                breweries += "Schmohz Brewing/";
            }

            if (Mitten == "on")
            {
                breweries += "The Mitten Brewing/";
            }
            return breweries;
        }

        //Use this to populate the DB the first time this program runs. Not after.
        public void PopulateDB()
        {
            Brewery GrandCircus = new Brewery();

            for (int i = 1; i < 15; i++)
            {

                switch (i)
                {
                    case 1:
                        GrandCircus.Name = "Schmozh";
                        GrandCircus.Url = "https://schmohz.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/AVEsqU/upload_uRmLOu-squareLarge.png";
                        GrandCircus.BreweryID = "AVEsqU";
                        break;
                    case 2:
                        GrandCircus.Name = "Jolly Pumpkin";
                        GrandCircus.Url = "http://brewery.jollypumpkin.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/pzWq1r/upload_2YHJS9-squareLarge.png";
                        GrandCircus.BreweryID = "pzWq1r";
                        break;
                    case 3:
                        GrandCircus.Name = "Atwater";
                        GrandCircus.Url = "https://www.atwaterbeer.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/boTIWO/upload_qHbhaE-squareLarge.png";
                        GrandCircus.BreweryID = "boTIWO";
                        break;
                    case 4:
                        GrandCircus.Name = "New Holland";
                        GrandCircus.Url = "http://newhollandbrew.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/AqEUBQ/upload_0xEGxj-squareLarge.png";
                        GrandCircus.BreweryID = "AqEUBQ";
                        break;
                    case 5:
                        GrandCircus.Name = "Brewery Vivant";
                        GrandCircus.Url = "http://www.breweryvivant.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/LFkVMc/upload_GhuNYz-squareLarge.png";
                        GrandCircus.BreweryID = "LFkVMc";
                        break;
                    case 6:
                        GrandCircus.Name = "Elk Brewing";
                        GrandCircus.Url = "http://elkbrewing.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/sjblac/upload_2QxSy3-squareLarge.png";
                        GrandCircus.BreweryID = "sjblac";
                        break;
                    case 7:
                        GrandCircus.Name = "Founders";
                        GrandCircus.Url = "http://www.foundersbrewing.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/Idm5Y5/upload_O8MoRg-squareLarge.png";
                        GrandCircus.BreweryID = "Idm5Y5";
                        break;
                    case 8:
                        GrandCircus.Name = "Harmony";
                        GrandCircus.Url = "https://harmonybeer.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/P0oEwB/upload_5Ngoxq-squareLarge.png";
                        GrandCircus.BreweryID = "P0oEwB";
                        break;
                    case 9:
                        GrandCircus.Name = "Hideout";
                        GrandCircus.Url = "http://hideoutbrewing.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/35YJeP/upload_eNle75-squareLarge.png";
                        GrandCircus.BreweryID = "35YJeP";
                        break;
                    case 10:
                        GrandCircus.Name = "The People's Cider";
                        GrandCircus.Url = "http://www.thepeoplescider.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/iebYze/upload_bv6mpy-squareLarge.png";
                        GrandCircus.BreweryID = "iebYze";
                        break;
                    case 11:
                        GrandCircus.Name = "Perrin Brewing Company";
                        GrandCircus.Url = "http://www.perrinbrewing.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/Boa6td/upload_6ADB1F-squareLarge.png";
                        GrandCircus.BreweryID = "Boa6td";
                        break;
                    case 12:
                        GrandCircus.Name = "Rockford Brewing Company";
                        GrandCircus.Url = "https://www.rockfordbrewing.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/U92Ctx/upload_6NaOBl-squareLarge.png";
                        GrandCircus.BreweryID = "U92Ctx";
                        break;
                    case 13:
                        GrandCircus.Name = "The Mitten";
                        GrandCircus.Url = "http://www.mittenbrewing.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/bdFoir/upload_NNuOnt-squareLarge.png";
                        GrandCircus.BreweryID = "bdFoir";
                        break;
                    case 14:
                        GrandCircus.Name = "HopCat";
                        GrandCircus.Url = "http://hopcat.com/";
                        GrandCircus.PictureUrl = "https://brewerydb-images.s3.amazonaws.com/brewery/HizvxH/upload_oqijUs-squareLarge.png";
                        GrandCircus.BreweryID = "HizvxH";
                        break;
                    default:
                        break;
                }

                db.Breweries.Add(GrandCircus);
                db.SaveChanges();
            }
        }
    }
}