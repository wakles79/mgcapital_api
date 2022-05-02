using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public static class ComparisonUtils
    {
        public static Dictionary<ComparisonPredicate, string> ComparisonPredicates = new Dictionary<ComparisonPredicate, string>
        {
            [ComparisonPredicate.Equal] = "=",
            [ComparisonPredicate.Unequal] = "<>",
            [ComparisonPredicate.LessThan] = "<",
            [ComparisonPredicate.LessThanOrEqualTo] = "<=",
            [ComparisonPredicate.GreaterThan] = ">",
            [ComparisonPredicate.GreaterThanOrEqualTo] = ">="
        };

        public static string GetComparisonOperator(ComparisonPredicate obj)
        {
            if (ComparisonPredicates.ContainsKey(obj))
            {
                return ComparisonPredicates[obj];
            }
            return string.Empty;
        }
    }
}
