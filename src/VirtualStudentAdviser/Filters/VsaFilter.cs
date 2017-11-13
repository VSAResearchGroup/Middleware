using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualStudentAdviser.Filters
{
    public class VsaFilter: ActionFilterAttribute

    {
        //public override void OnResultExecuted(ResultExecutedContext actionExecutedContext)
        //{
        //    Console.WriteLine("I got here");
        //    actionExecutedContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        //    actionExecutedContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");
        //    actionExecutedContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "*");

        //    // Response.Headers.Add("customHeader", "custom value date time");
        //}

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            Console.WriteLine("I got here action");
            actionExecutedContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            actionExecutedContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            actionExecutedContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "*");
            // Response.Headers.Add("customHeader", "custom value date time");
        }
    }

    
}
