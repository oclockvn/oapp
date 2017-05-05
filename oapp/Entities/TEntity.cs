namespace oapp.Entities
{
    public class TEntity<T>
    {
        T Id { get; set; }
    }

    public class BaseEntity : TEntity<int>
    {

    }
}