namespace ASPWeb_Demo2.Controllers.Managers
{
    public interface IManager<T>
    {

        List<T>? GetAll();
        T? GetOne(object identifier);
        bool Add(T value);
        bool Remove(object identifier);
        bool Update(object identifier, T NewValue);

    }
}
