namespace Application.Dtos.UserDtos;

public class UpdateUserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int DepartmentId { get; set; }
    public string Role { get; set; }
    public bool Active { get; set; }
}