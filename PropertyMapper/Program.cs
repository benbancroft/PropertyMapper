using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Dummy1;
using Dummy2;
using LinqKit;

namespace PropertyMapper
{
    class Program
    {

        static void Main(string[] args)
        {
            //This method needs to be called first - should be done at start of program else static constructors will be called in wrong order.
            PropertyMapperExtensions.LoadMappedEntities();

            var test = new DummyEntity();
            test.second = "test";
            test.dummy1 = 8;

            test.child = new ChildEntity();
            test.child.FirstName = "First";
            test.child.LastName = "Child";
            test.child.SecondChild = new SecondChildEntity();
            test.child.SecondChild.FirstName2 = "Second";
            test.child.SecondChild.LastName2 = "Child";

            //Should only be called on single entitys - do NOT use in select e.g. .Select(x => x.ToDTO())
            var test2 = test.ToDTO();

            //And back to entity
            var test3 = test2.ToEntity<DummyDTO, DummyEntity>();

            //Print whole expression to console
            //It will have expanded into a wholesum linq expression if done correctly, and should have no custom functions in it.
            Console.Write("Expanded Expression: ");
            Console.WriteLine(DummyEntity.toDTOExpression().ToString());

            //Now to show this on lists.

            List<DummyEntity> list = new List<DummyEntity>();

            for (int i = 0; i < 10; i++)
            {
                var entity = new DummyEntity();
                entity.second = "test" + i.ToString();
                entity.dummy1 = i;

                entity.child = new ChildEntity();
                entity.child.FirstName = "First";
                entity.child.LastName = "Child";
                entity.child.SecondChild = new SecondChildEntity();
                entity.child.SecondChild.FirstName2 = "Second";
                entity.child.SecondChild.LastName2 = "Child";

                list.Add(entity);
            }

            //e.g. Queryable
            IQueryable<DummyEntity> queryable = list.AsQueryable();

            Console.WriteLine("Converting Entity queryable to DTO queryable...");

            //Create new expression - this is how lists should be done
            var queryableExpression = DummyEntity.toDTOExpression();
            IQueryable<DummyDTO> newQueryable = queryable.Select(queryableExpression);

            var newList = newQueryable.ToList();

            Console.WriteLine("NewListSize: " + newList.Count);

            Console.ReadLine();
        }
    }
}
