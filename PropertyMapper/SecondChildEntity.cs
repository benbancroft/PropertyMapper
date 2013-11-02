using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PropertyMapper;
using Dummy2;

namespace Dummy1
{
    public class SecondChildEntity : PropertyMap<SecondChildDTO, SecondChildEntity>
    {
        static SecondChildEntity()
        {
            Console.WriteLine("Initialised Second Child");

            Map("FirstName", x => x.FirstName2, x => x.FirstName2);
            Map("LastName", x => x.LastName2, x => x.LastName2);
        }
        [PropertyMapper("FirstName")]
        public string FirstName2 { get; set; }
        [PropertyMapper("LastName")]
        public string LastName2 { get; set; }
    }
}
