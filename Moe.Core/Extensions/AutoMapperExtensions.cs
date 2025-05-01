using AutoMapper;

namespace Moe.Core.Extensions;

public static class AutoMapperExtensions
{
    public static IMappingExpression<TSource, TDestination> IgnoreNull<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
    {
        mappingExpression.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        return mappingExpression;
    } 
    public static IMappingExpression<TSource, TDestination> IgnoreNullAndEmptyGuids<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
    {
        mappingExpression.ForAllMembers(opt =>
            opt.Condition((src, dest, srcMember, destMember, context) =>
                srcMember != null && 
                (!(srcMember is Guid) || !Guid.Empty.Equals(srcMember))));

        return mappingExpression;
    } 
}