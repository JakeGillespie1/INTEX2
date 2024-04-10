using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Identity;

namespace INTEX2.Models
{
    public class Tools
    {
        public Tools()
        { }

        //DateTime Tools
        private readonly Dictionary<string, int> _dateDictionary = new Dictionary<string, int>()
        {
            { "keyYear", DateTime.Now.Year },
            { "keyMonth", DateTime.Now.Month },
            { "keyDay", DateTime.Now.Day },
            //Hard-coded MST here for speed and convenience. This would be good to change on the client-side later.
            { "keyHour", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time")).Hour }
        };

        public string GetFormattedStringDate()
        {
            return "";
        }

        public string GetTimeOfDay()
        {
            int hourOfDay = _dateDictionary["keyHour"];
            string timeOfDay = "";
            if (hourOfDay >= 0 &&  hourOfDay < 12) 
            {
                timeOfDay = "Morning";
            }
            else if (hourOfDay >= 12 && hourOfDay < 16)
            {
                timeOfDay = "Afternoon";
            }
            else if(hourOfDay >= 16 && hourOfDay <= 20) 
            {
                timeOfDay = "Evening";
            }
            else if (hourOfDay >= 20 && hourOfDay <= 23)
            {
                timeOfDay = "Night";
            }

            return timeOfDay;
        }
    }

}
