﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Exceptions
{
    public class AuthenticationErrorException:Exception
    {
        public AuthenticationErrorException() : base("Authentication Hatası") { }
       
    }
}
