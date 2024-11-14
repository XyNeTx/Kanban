using AutoMapper;
using KANBAN.Services.Automapper;
using KANBAN.Services.Automapper.Interface;

namespace KANBAN.Services.Automapper.Repo
{
    public class AutoMapService : IAutoMapService
    {
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public AutoMapService(IMapper mapper, IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public IAutoMapRepo<T1,T2> GetAutoMapRepo<T1, T2>() where T1 : class where T2 : class
        {
            return _serviceProvider.GetRequiredService<IAutoMapRepo<T1, T2>>();
        }

    }
}
