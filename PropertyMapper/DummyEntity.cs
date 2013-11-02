using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PropertyMapper;
using Dummy2;
using LinqKit;

namespace Dummy1
{
    public class DummyEntity : PropertyMap<DummyDTO, DummyEntity>
    {
        static DummyEntity()
        {
            Console.WriteLine("Initialised Dummy");

            //These NEED to be declared out of linq - else linq will see them as method and LinqKit wont be able to expand query.
            var childDTOExpression = ChildEntity.toDTOExpression();
            var childEntityExpression = ChildEntity.toEntityExpression();

            //Arguments are : Name, Expression for column from DTO, Expression for column from Entity
            Map("1", x => x.first != null ? x.first.ToString() : null, x => x.first != null ? x.first.ToString() : null);
            Map("2", x => x.second.ToString(), x => x.second.ToString());
            Map("3", x => x.dummy1/2, x => x.dummy1*2);
            Map("Child", x => childEntityExpression.Invoke(x.child), x => childDTOExpression.Invoke(x.child));
        }
        //This attribute will need to be declared on both entity and DTO
        [PropertyMapper("1")]
        public string first { get; set; }
        [PropertyMapper("2")]
        public string second { get; set; }
        [PropertyMapper("3")]
        public int dummy1 { get; set; }
        [PropertyMapper("Child")]
        public ChildEntity child { get; set; }
    }
}
