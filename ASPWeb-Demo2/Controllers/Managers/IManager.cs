namespace ASPWeb_Demo2.Controllers.Managers
{

    public interface IManager<T>
    {

        List<T>? GetAll();
        T? GetOne(object identifier);
        Task Add(T value);
        Task Remove(object identifier);
        Task Update(object identifier, T NewValue);

    }

}
