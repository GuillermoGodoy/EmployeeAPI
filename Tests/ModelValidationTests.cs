using EmployeeAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Tests;

public class ModelValidationTests
{
    private static IList<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Punch_WithRequiredFields_IsValid()
    {
        var punch = new Punch
        {
            Device_Id = "652d1f0b4b5c9a0001a1b2c3",
            PunchType = "IN",
            Punch_Dtm = DateTime.UtcNow,
            Pin = "1234"
        };

        Assert.Empty(Validate(punch));
    }

    [Fact]
    public void Punch_WithoutDevice_IsInvalid()
    {
        var punch = new Punch { PunchType = "IN", Punch_Dtm = DateTime.UtcNow };

        Assert.Contains(Validate(punch), r => r.MemberNames.Contains(nameof(Punch.Device_Id)));
    }

    [Theory]
    [InlineData("12")]
    [InlineData("abcd")]
    [InlineData("12345678901")]
    public void Enrollment_WithInvalidPin_IsInvalid(string pin)
    {
        var enrollment = new Enrollment { Employee_Id = "652d1f0b4b5c9a0001a1b2c3", Pin = pin };

        Assert.Contains(Validate(enrollment), r => r.MemberNames.Contains(nameof(Enrollment.Pin)));
    }

    [Fact]
    public void Device_WithoutTimezone_IsInvalid()
    {
        var device = new Device { Name = "Reloj Recepcion", Location = "Casa Matriz", Timezone = null! };

        Assert.Contains(Validate(device), r => r.MemberNames.Contains(nameof(Device.Timezone)));
    }
}
