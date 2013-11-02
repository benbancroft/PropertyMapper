using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PropertyMapper;
using Dummy1;

namespace Dummy2
{
    public class ChildDTO
    {
        [PropertyMapper("FirstName")]
        public string FirstName { get; set; }
        [PropertyMapper("LastName")]
        public string LastName { get; set; }
        [PropertyMapper("SecondChild")]
        public SecondChildDTO SecondChild { get; set; }
    }
}
