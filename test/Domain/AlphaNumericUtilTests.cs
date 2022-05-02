//
// AlphaNumericUtilsTests.cs
//
// Author:
//       jcurbelo <robin@axzes.com>
//
// Copyright (c) 2019 Axzes
//
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).


using System.Collections.Generic;
using MGCap.Domain.Utils;
using Xunit;

namespace MGCap.Domain.Tests
{
    public class AlphaNumericUtilTests
    {
        [Fact]
        public void ConvertNumberToAlpha()
        {
            var expectations = new Dictionary<int, string>
            {
                [1] = "A",
                [3] = "C",
                [26] = "Z",
                [27] = "AA",
                [28] = "AB",
                [32] = "AF",
                [-3] = "A",
                [0] = "A",
                [53] = "BA",
                [104] = "CZ",
                [int.MaxValue] = "XRHSXFW",
                [12356631] = "AAAAAA",
                [12356630] = "ZZZZZ",
            };

            foreach (var kvp in expectations)
            {
                Assert.Equal(kvp.Value, AlphaNumericUtils.ConvertNumberToAlpha(kvp.Key));
            }
        }

    }
}
