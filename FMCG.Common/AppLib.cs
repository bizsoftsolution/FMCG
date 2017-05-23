﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FMCG.Common
{
    public static class AppLib
    {

        public static decimal GSTPer = (decimal)0.06;
        public static string CurrencyName1 = "RINGGIT";
        public static string CurrencyName2 = "CENT";
        public static void toCopy<T>(this object objSource, T objDestination)
        {
            var l1 = objSource.GetType().GetProperties().Where(x => x.PropertyType.Namespace != "System.Collections.Generic").ToList();

            foreach (var pFrom in l1)
            {
                try
                {
                    var pTo = objDestination.GetType().GetProperties().Where(x => x.Name == pFrom.Name).FirstOrDefault();
                    pTo.SetValue(objDestination, pFrom.GetValue(objSource));
                }
                catch (Exception ex) { }
            }
        }

        public static void MutateVerbose<TField>(this INotifyPropertyChanged instance, ref TField field, TField newValue, Action<PropertyChangedEventArgs> raise, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TField>.Default.Equals(field, newValue)) return;
            field = newValue;
            raise?.Invoke(new PropertyChangedEventArgs(propertyName));
        }
      
        #region NumberToWords

        public static string ToCurrencyInWords(this decimal? Number)
        {
            if (Number == null) return "";

            string[] Nums = string.Format("{0:0.00}", Number).Split('.');

            int number1 = int.Parse(Nums[0]);
            int number2 = int.Parse(Nums[1]);

            String words = "";

            words = string.Format("{0} {1}{2} ", number1.ToWords(), CurrencyName1, number1 > 1 ? "S" : "");
            if (number2 > 0) words = string.Format("{0} AND {1} {2}{3}", words, number2.ToWords(),CurrencyName2, number2>1?"S":"" );
            words = string.Format("{0} ONLY", words);
            return words;

        }
        public static string ToWords(this int number1 )
        {
            if (number1 == 0)
                return "Zero";

            if (number1 < 0)
                return "minus " + ToWords(Math.Abs(number1));

            string words = "";

            if ((number1 / 1000000) > 0)
            {
                words += ToWords(number1 / 1000000) + " Million ";
                number1 %= 1000000;
            }

            if ((number1 / 1000) > 0)
            {
                words += ToWords(number1 / 1000) + " Thousand ";
                number1 %= 1000;
            }

            if ((number1 / 100) > 0)
            {
                words += ToWords(number1 / 100) + " Hundred ";
                number1 %= 100;
            }

            if (number1 > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number1 < 20)
                    words += unitsMap[number1];
                else
                {
                    words += tensMap[number1 / 10];
                    if ((number1 % 10) > 0)
                        words += "-" + unitsMap[number1 % 10];
                }
            }
            return words.ToUpper();
        }        
        
        #endregion
    }
}
