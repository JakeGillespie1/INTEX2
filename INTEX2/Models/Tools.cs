﻿using System.Collections.Generic;
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
            { "keyHour", DateTime.Now.Hour }
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
            else if(hourOfDay >= 16 && hourOfDay <= 23) 
            {
                timeOfDay = "Evening";
            }

            return timeOfDay;
        }
    }

}