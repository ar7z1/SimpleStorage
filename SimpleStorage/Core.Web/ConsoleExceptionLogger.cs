using System;
using System.Web.Http.ExceptionHandling;

namespace Core.Web
{
    public class ConsoleExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            Console.Error.WriteLine(context.Exception);
        }
    }
}