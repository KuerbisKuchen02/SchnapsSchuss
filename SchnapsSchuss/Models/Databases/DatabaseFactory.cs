using SchnapsSchuss.Models.Entities;

namespace SchnapsSchuss.Models.Databases;

public abstract class DatabaseFactory<T>
{
    public static IDatabase<T> GetDatabase()
    {
        return typeof(T).Name switch
        {
            nameof(Article) => (IDatabase<T>)new ArticleDatabase(),
            nameof(Invoice) => (IDatabase<T>)new InvoiceDatabase(),
            nameof(InvoiceItem) => (IDatabase<T>)new InvoiceItemDatabase(),
            nameof(Member) => (IDatabase<T>)new MemberDatabase(),
            nameof(Person) => (IDatabase<T>)new PersonDatabase(),
            _ => throw new ArgumentException($"Unknown database type: {typeof(T).Name}")
        };
    }
}