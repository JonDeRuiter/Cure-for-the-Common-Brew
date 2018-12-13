using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Craft_Beer_Me.Models
{
    public class BeerComparison
    {
        public int ABVScore { get; set; }
        public int ABVCat { get; set; }
        public int IBUScore { get; set; }
        public int IBUCat { get; set; }
        public int SRMScore { get; set; }
        public int SRMCat { get; set; }

        public BeerComparison()
        {

        }

        public BeerComparison(int ABVScore, int ABVCat, int IBUScore, int IBUCat, int SRMScore, int SRMCat)
        {
            this.ABVScore = ABVScore;
            this.ABVCat = ABVCat;
            this.IBUScore = IBUScore;
            this.IBUCat = IBUCat;
            this.SRMScore = SRMScore;
            this.SRMCat = SRMCat;
        }

    }
}