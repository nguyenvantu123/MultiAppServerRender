
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAppServer.ServiceDefaults.Wrapper
{
    public interface IResultBase
    {
        int StatusCode { get; set; }
        bool Success { get; set; }
        List<string> ErrorMessages { get; set; }
        // T Result { get; set; }
    }
}

