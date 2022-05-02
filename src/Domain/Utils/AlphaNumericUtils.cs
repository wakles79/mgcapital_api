//
// AlphaNumericUtils.cs
//
// Author:
//       jcurbelo <robin@axzes.com>
//
// Copyright (c) 2019 Axzes
//
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).

using System.Text;

namespace MGCap.Domain.Utils
{
    public static class AlphaNumericUtils
    {
        public static readonly char[] Alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        /// <summary>
        /// Returns the alphabet character (or characters) corresponding
        /// to a given number. The result will be a base-26 like alpha value
        /// </summary>
        /// <param name="number">The number to be converted from</param>
        /// <returns>The alphabet character (or the concatenation of several alphabet characters)</returns>
        public static string ConvertNumberToAlpha(int number)
        {
            // Safety check
            if (number <= 0)
            {
                return "A";
            }

            var result = new StringBuilder();
            int index = (--number) % 26;

            while (number / 26 > 0)
            {
                number /= 26;
                var i = (--number) % 26;
                result.Append(Alpha[i]);
            }

            result.Append(Alpha[index]);

            return result.ToString();
        }
    }
}
