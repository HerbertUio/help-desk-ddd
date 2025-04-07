using Domain.Enums.UserEnums;
using Domain.Models.Common;
using Domain.ValueObjects.UserValueObjects;

namespace Domain.Models;

public class UserModel: BaseModel
{
    public UserModel(int id, string name, string lastName, UserPhoneNumber userPhoneNumber, UserEmail email, string password, int departmentId, UserRole role, bool active) : base(id)
    {
        Name = name;
        LastName = lastName;
        UserPhoneNumber = userPhoneNumber;
        Email = email;
        Password = password;
        DepartmentId = departmentId;
        Role = role;
        Active = active;
    }

    public string Name {get; set;}
    public string LastName { get; set; }
    public UserPhoneNumber UserPhoneNumber { get; set; }
    public UserEmail Email { get; set; }
    public string Password { get; set; }
    public int DepartmentId { get; set; }
    public UserRole Role { get; set; }
    public bool Active { get; set; }
    
    public void UpdateUserModel(string name, string lastName, UserPhoneNumber userPhoneNumber, UserEmail email, string password, int departmentId, UserRole role, bool active)
    {
        Name = name;
        LastName = lastName;
        UserPhoneNumber = userPhoneNumber;
        Email = email;
        Password = password;
        DepartmentId = departmentId;
        Role = role;
        Active = active;
    }
    public void ActivateUser()=> Active = true;
    public void DeactivateUser()=> Active = false;

}