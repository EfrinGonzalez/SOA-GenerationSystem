﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Customer
    {
       
        public Guid Id { get; set;  }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
