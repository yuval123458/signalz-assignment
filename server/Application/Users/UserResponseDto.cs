namespace Application.Users;

public record UserResponseDto(int Id, string Username, string Email, DateOnly BirthDate, string ImagePath);
