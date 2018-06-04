using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LolSynergy
{
    class Champion
    {
        public string Name { get; set; }
        public int Victories { get; set; } = 0;
        public int Defeats { get; set; } = 0;
        public int TotalMatches => Victories + Defeats;
        public string WinRate
        {
            get
            {
                return ((float)Victories / TotalMatches).ToString("P2", new NumberFormatInfo{
                    PercentPositivePattern = 1, PercentNegativePattern = 1
                });
            }
        }
    }
}