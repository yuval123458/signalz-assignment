namespace Application.Users;

public interface IImageStorage
{
    Task<string> SaveAsync(Stream content, string fileName);
}
