using System;
using System.Collections.Generic;
using System.Text;

namespace Refinitiv.Aaa.GuissApi.Facade.Models
{
    public class CacheHelperOptions
    {
      public int DefaultExpirationInSeconds { get; set; }
      public bool Enabled { get; set; }
    }
}
