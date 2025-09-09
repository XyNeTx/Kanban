using AutoMapper;

namespace KANBAN.Services.Automapper
{
    public class AutoMapConfig<T1,T2> : Profile where T1 : class where T2 : class
    {
        public AutoMapConfig()
        {
            CreateMap<T1, T2>();
        }
    }
}
