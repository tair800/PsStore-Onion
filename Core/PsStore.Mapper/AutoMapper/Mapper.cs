using IAutoMapper = AutoMapper.IMapper;
using ICustomMapper = PsStore.Application.Interfaces.AutoMapper.IMapper;

namespace PsStore.Mapper.AutoMapper
{
    public class Mapper : ICustomMapper
    {
        private readonly IAutoMapper _mapper;

        public Mapper(IAutoMapper mapper)
        {
            _mapper = mapper;
        }

        public TDestination Map<TDestination>(object source)
            => _mapper.Map<TDestination>(source);

        public IList<TDestination> Map<TDestination>(IList<object> source)
            => _mapper.Map<IList<TDestination>>(source);

        public TDestination Map<TDestination, TSource>(TSource source)
            => _mapper.Map<TSource, TDestination>(source);

        public IList<TDestination> Map<TDestination, TSource>(IList<TSource> source)
            => _mapper.Map<IList<TDestination>>(source);

        public void Map<TSource, TDestination>(TSource source, TDestination destination)
            =>
               _mapper.Map(source, destination);

    }
}
