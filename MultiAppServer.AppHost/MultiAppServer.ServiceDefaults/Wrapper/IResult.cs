<<<<<<< HEAD
﻿using System.Collections.Generic;

namespace Wrapper
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAppServer.ServiceDefaults.Wrapper
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b
{
    public interface IResultBase<T>
    {
        int StatusCode { get; set; }
        bool Success { get; set; }
        List<string> ErrorMessages { get; set; }
        T Result { get; set; }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b
