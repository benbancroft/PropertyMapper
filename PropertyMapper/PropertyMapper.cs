using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using LinqKit;
using System.Runtime.CompilerServices;

namespace PropertyMapper
{
    public interface IPropertyMap
    {
    }

    public abstract class PropertyMap<DTO, ENTITY> : IPropertyMap where ENTITY : PropertyMap<DTO, ENTITY>, new()
    {
        private static readonly List<PropertyMapInfo<DTO, ENTITY>> properties = new List<PropertyMapInfo<DTO, ENTITY>>();

        public static void Map(string name, Expression<Func<DTO, object>> getDTO, Expression<Func<ENTITY, object>> getEntity)
        {
            PropertyInfo dtoProperty = null;
            foreach (var property in typeof(DTO).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = property.GetCustomAttributes(typeof(PropertyMapperAttribute), false);

                if (property.CanWrite && attr.Length > 0 && ((PropertyMapperAttribute)attr[0]).Name == name)
                {
                    dtoProperty = property;
                }
            }

            PropertyInfo entityProperty = null;
            foreach (var property in typeof(ENTITY).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = property.GetCustomAttributes(typeof(PropertyMapperAttribute), false);

                if (property.CanWrite && attr.Length > 0 && ((PropertyMapperAttribute)attr[0]).Name == name)
                {
                    entityProperty = property;
                }
            }

            if (dtoProperty != null && entityProperty != null)
            {
                properties.Add(new PropertyMapInfo<DTO, ENTITY>(dtoProperty, getDTO, entityProperty, getEntity));
            }
        }

        private static IEnumerable<MemberBinding> GetPropertyBindings(PropertyMapDirection direction, ParameterExpression entityParam, ParameterExpression dtoParam)
        {
            if (direction == PropertyMapDirection.TO_DTO)
            {
                return
                    from property in properties
                    select Expression.Bind(property.DTOProperty, Expression.Convert(new PredicateRewriterVisitor(entityParam).Visit(property.getEntity.Body), property.DTOProperty.PropertyType));
            }
            else
            {
                return
                    from property in properties
                    select Expression.Bind(property.EntityProperty, Expression.Convert(new PredicateRewriterVisitor(dtoParam).Visit(property.getDTO.Body), property.EntityProperty.PropertyType));
            }
        }

        private static MemberInitExpression GetMemberInit(PropertyMapDirection direction, ParameterExpression entityParam, ParameterExpression dtoParam)
        {
            if (direction == PropertyMapDirection.TO_DTO)
            {
                return Expression.MemberInit(Expression.New(typeof(DTO)), GetPropertyBindings(direction, entityParam, dtoParam));
            }
            else
            {
                return Expression.MemberInit(Expression.New(typeof(ENTITY)), GetPropertyBindings(direction, entityParam, dtoParam));
            }
        }

        public static Expression<Func<ENTITY, DTO>> toDTOExpression()
        {
            var entityParam = Expression.Parameter(typeof(ENTITY), "entity");
            var dtoParam = Expression.Parameter(typeof(DTO), "dto");
            return Expression.Lambda<Func<ENTITY, DTO>>(GetMemberInit(PropertyMapDirection.TO_DTO, entityParam, dtoParam), entityParam).Expand();
        }


        public static Expression<Func<DTO, ENTITY>> toEntityExpression()
        {
            var entityParam = Expression.Parameter(typeof(ENTITY), "entity");
            var dtoParam = Expression.Parameter(typeof(DTO), "dto");
            return Expression.Lambda<Func<DTO, ENTITY>>(GetMemberInit(PropertyMapDirection.TO_ENTITY, entityParam, dtoParam), dtoParam).Expand();
        }

        public DTO ToDTO()
        {
            return toDTOExpression().Invoke((ENTITY)this);
        }

        public ENTITY ToEntity(DTO dtoPOCO)
        {
            return toEntityExpression().Compile().Invoke(dtoPOCO);
        }
    }

    public static class PropertyMapperExtensions
    {
        public static ENTITY ToEntity<DTO, ENTITY>(this DTO dtoPOCO) where ENTITY : PropertyMap<DTO, ENTITY>, new()
        {
            return new ENTITY().ToEntity(dtoPOCO);
        }

        public static void LoadMappedEntities()
        {
            var mappedEntitys = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => typeof(IPropertyMap).IsAssignableFrom(t) && !t.IsAbstract).ToList();

            List<Type> orderedTypes = new List<Type>();

            foreach (var entity in mappedEntitys)
            {
                orderMappedEntities(entity, ref orderedTypes, ref mappedEntitys);
            }

            foreach (var type in orderedTypes)
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
        }

        private static void orderMappedEntities(Type startType, ref List<Type> orderedTypes, ref List<Type> mappedEntitys)
        {
            var properties = startType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (mappedEntitys.Contains(property.PropertyType))
                {
                    orderMappedEntities(property.PropertyType, ref orderedTypes, ref mappedEntitys);
                }
            }
            if (!orderedTypes.Contains(startType))
            {
                orderedTypes.Add(startType);
            }
        }
    }

    public class PredicateRewriterVisitor : System.Linq.Expressions.ExpressionVisitor
    {
        private readonly ParameterExpression parameterExpression;

        public PredicateRewriterVisitor(ParameterExpression parameterExpression)
        {
            this.parameterExpression = parameterExpression;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return this.parameterExpression;
        }
    }

    public enum PropertyMapDirection
    {
        TO_DTO,
        TO_ENTITY
    }

    public class PropertyMapInfo<DTO, ENTITY>
    {
        public PropertyMapInfo(PropertyInfo DTOProperty, Expression<Func<DTO, object>> getDTO, PropertyInfo EntityProperty, Expression<Func<ENTITY, object>> getEntity)
        {
            this.DTOProperty = DTOProperty;
            this.EntityProperty = EntityProperty;
            this.getDTO = getDTO;
            this.getEntity = getEntity;
        }

        public PropertyInfo DTOProperty { get; set; }
        public PropertyInfo EntityProperty { get; set; }
        public Expression<Func<DTO, object>> getDTO { get; set; }
        public Expression<Func<ENTITY, object>> getEntity { get; set; }

    }

    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyMapperAttribute : System.Attribute
    {

        public string Name { get; set; }

        public PropertyMapperAttribute(string Name)
        {
            this.Name = Name;
        }
    }

}
