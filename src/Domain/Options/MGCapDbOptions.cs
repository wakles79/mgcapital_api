using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Options
{
    /// <summary>
    /// Receives a string indicating the connection string for creating DbConnections
    /// </summary>
    public class MGCapDbOptions
    {
        public string ConnectionString { get; set; }
    }
}
