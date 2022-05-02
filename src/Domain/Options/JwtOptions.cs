using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Options
{
    public class JwtOptions
    {
        public string Key { get; set; }

        public string Issuer { get; set; }

        public int ExpireMinutes { get; set; }
    }
}
