
using AutoMapper;
using KANBAN.Services.Automapper.Interface;

namespace KANBAN.Services.Automapper.Repo
{
    public class AutoMapRepo<T1,T2> : IAutoMapRepo<T1,T2> where T1 : class where T2 : class
    {
        private readonly IMapper _mapper;

        public AutoMapRepo(IMapper mapper)
        {
            _mapper = mapper;
        }

        public T2 MapTo(T1 source)
        {
            return _mapper.Map<T2>(source);
        }
    }
}
