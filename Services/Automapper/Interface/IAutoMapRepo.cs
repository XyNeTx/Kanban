namespace KANBAN.Services.Automapper.Interface
{
    public interface IAutoMapRepo<T1, T2> where T1 : class where T2 : class
    {
        T2 MapTo(T1 source);
    }
}
