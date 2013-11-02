using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PropertyMapper;
using Dummy2;
using LinqKit;

namespace Dummy1
{
    public class ChildEntity : PropertyMap<ChildDTO, ChildEntity>
    {
        static ChildEntity()
        {
            Console.WriteLine("Initialised Child");

            var schildDTOExpression = SecondChildEntity.toDTOExpression();
            var schildEntityExpression = SecondChildEntity.toEntityExpression();

            Map("FirstName", x => x.FirstName, x => x.FirstName);
            Map("LastName", x => x.LastName, x => x.LastName);
            Map("SecondChild", x => schildEntityExpression.Invoke(x.SecondChild), x => schildDTOExpression.Invoke(x.SecondChild));
        }
        [PropertyMapper("FirstName")]
        public string FirstName { get; set; }
        [PropertyMapper("LastName")]
        public string LastName { get; set; }
        [PropertyMapper("SecondChild")]
        public SecondChildEntity SecondChild { get; set; }
    }
}
