namespace KANBAN.Services.Automapper.Interface
{
    public interface IAutoMapService
    {
        IAutoMapRepo<T1,T2> GetAutoMapRepo<T1, T2>() where T1 : class where T2 : class;
    }
}
