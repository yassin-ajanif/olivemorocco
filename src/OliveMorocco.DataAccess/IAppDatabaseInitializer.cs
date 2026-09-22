namespace OliveMorocco.DataAccess;

public interface IAppDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
