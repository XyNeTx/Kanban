using KANBAN.Models;

namespace KANBAN.Libs
{
    public interface IResponseLib<T> where T : class
    {
        T Response { get; }

        ResponseModel Ok(string message, T T);
    }
}
