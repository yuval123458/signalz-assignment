using Application.Users;

namespace Infrastructure;

public class DiskImageStorage : IImageStorage
{
    private readonly string _folder;

    public DiskImageStorage(string folder)
    {
        _folder = folder;
        Directory.CreateDirectory(_folder);
    }

    public async Task<string> SaveAsync(Stream content, string fileName)
    {
        var ext = Path.GetExtension(fileName);
        var name = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine(_folder, name);

        await using var file = File.Create(path);
        await content.CopyToAsync(file);

        return $"/uploads/{name}";
    }
}
