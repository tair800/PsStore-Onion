namespace PsStore.Application.Interfaces.AutoMapper
{
    public interface IMapper
    {
        TDestination Map<TDestination>(object source);
        IList<TDestination> Map<TDestination>(IList<object> source);

        TDestination Map<TDestination, TSource>(TSource source);
        IList<TDestination> Map<TDestination, TSource>(IList<TSource> source);

        void Map<TSource, TDestination>(TSource source, TDestination destination);

    }
}
