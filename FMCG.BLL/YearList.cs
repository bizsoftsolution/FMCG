using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCG.BLL
{
   public class YearList
    {
        private static List<YearList> _toList;
        public static List<YearList> toList
        {
            get
            {
                if ( _toList== null)
                {
                    _toList = FMCGHubClient.FMCGHub.Invoke<List<YearList>>("Year_List").Result;
                }

                return _toList;
            }
        }

        public string AccountYear
        { get; set; }
    }
}
