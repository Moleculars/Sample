using Bb.Workflows.Models;
using System;

namespace MyCustomBusiness
{

    public static class BusinessMethodToEvaluates
    {

        [System.ComponentModel.DisplayName("IsMajor")]
        [System.ComponentModel.Description("Check if the value of incomingEvent.Age is greated of specifid age")]
        public static bool IsMajor(RunContext ctx, int agemin)
        {
            var p = ctx.IncomingEvent.ExtendedDatas["Age"];
            if (p == null || p == DynObject.None)
                return false;
            var value = p.ValueAs<int>(ctx);
            return value >= agemin;
        }

        [System.ComponentModel.DisplayName("IsEmpty")]
        [System.ComponentModel.Description("test if specified text is null or empty")]
        public static bool IsEmpty(RunContext ctx, string text)
        {
            return string.IsNullOrEmpty((string)text);
        }

    }

}
