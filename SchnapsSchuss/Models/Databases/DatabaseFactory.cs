using SchnapsSchuss.Models.Entities;

namespace SchnapsSchuss.Models.Databases;

public abstract class DatabaseFactory<T>
{
    public static Database<T> GetDatabase()
    {
        return typeof(T).Name switch
        {
            nameof(Article) => (Database<T>)new ArticleDatabase(),
            nameof(Invoice) => (Database<T>)new InvoiceDatabase(),
            nameof(InvoiceItem) => (Database<T>)new InvoiceItemDatabase(),
            nameof(Member) => (Database<T>)new MemberDatabase(),
            nameof(Person) => (Database<T>)new PersonDatabase(),
            _ => throw new ArgumentException($"Unknown database type: {typeof(T).Name}")
        };
    }
}