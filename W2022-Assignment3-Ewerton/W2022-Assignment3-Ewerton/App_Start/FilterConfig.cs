﻿using System.Web;
using System.Web.Mvc;

namespace W2022_Assignment3_Ewerton
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}