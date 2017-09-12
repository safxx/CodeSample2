using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HighLoad.Framework.Filters
{
    public class RestrictInvalidValuesFilter: Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var bodyStr = string.Empty;

            context.HttpContext.Request.EnableRewind();

            using (StreamReader reader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                bodyStr = reader.ReadToEnd();

            if (bodyStr.Length == 0 || bodyStr.Contains(": null")) context.Result = new BadRequestResult();

            context.HttpContext.Request.Body.Position = 0;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            
        }
    }
}
