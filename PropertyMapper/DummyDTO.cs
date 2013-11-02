using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PropertyMapper;
using Dummy1;

namespace Dummy2
{
    public class DummyDTO
    {
        [PropertyMapper("1")]
        public string first { get; set; }
        [PropertyMapper("2")]
        public string second { get; set; }
        [PropertyMapper("3")]
        public int dummy1 { get; set; }
        [PropertyMapper("Child")]
        public ChildDTO child { get; set; }
        public int test { get; set; }
    }
}
