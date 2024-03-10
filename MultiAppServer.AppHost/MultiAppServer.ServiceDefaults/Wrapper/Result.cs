<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Threading.Tasks;

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
    public class ResultBase<T> : IResultBase<T>
    {
        public ResultBase()
        {
            StatusCode = 200;
            ErrorMessages = new List<string>();
        }

        public ResultBase(T result)
        {
            Success = true;
            Result = result;
            ErrorMessages = new List<string>();
            StatusCode = 200;
        }

        public ResultBase(int statusCode, params string[] errors)
        {
            Success = false;
            ErrorMessages = errors.ToList();
            StatusCode = statusCode;
        }

        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public T Result { get; set; }
        public List<string> ErrorMessages { get; set; }
<<<<<<< HEAD

    }
    public class ResultBase
    {
        public ResultBase()
        {
            StatusCode = 200;
            ErrorMessages = new List<string>();
        }

        public ResultBase(int statusCode, params string[] errors)
        {
            Success = false;
            ErrorMessages = errors.ToList();
            StatusCode = statusCode;
        }

        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
=======
    }
}
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b
