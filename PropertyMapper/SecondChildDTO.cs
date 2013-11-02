using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PropertyMapper;
using Dummy1;

namespace Dummy2
{
    public class SecondChildDTO
    {
        [PropertyMapper("FirstName")]
        public string FirstName2 { get; set; }
        [PropertyMapper("LastName")]
        public string LastName2 { get; set; }
    }
}
