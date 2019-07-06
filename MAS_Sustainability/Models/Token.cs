using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MAS_Sustainability
{
    public class Token
    {
     
        public String ProblemName { get; set; }
        public String ProblemCategory { get; set; }
        public String Location { get; set; }
        public String Description { get; set; }
        public String FirstImagePath { get; set; }
        public HttpPostedFileBase FirstImageFile { get; set; }
        public String SecondImagePath { get; set; }
        public HttpPostedFileBase SecondImageFile { get; set; }
        public int AttentionLevel { get; set; }

    }
}