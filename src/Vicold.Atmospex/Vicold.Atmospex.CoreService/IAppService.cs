namespace Vicold.Atmospex.CoreService;

public interface IAppService
{
    T? GetService<T>() where T : class;
}