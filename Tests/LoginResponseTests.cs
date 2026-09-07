using EmployeeAPI.Controllers;
using EmployeeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EmployeeAPI.Tests;

/// <summary>Cubre la mejora implementada: el login no debe exponer la contraseña ni el documento completo del usuario.</summary>
public class LoginResponseTests
{
    [Fact]
    public void LoginEndpoint_ReturnsLoginResponse_NotUser()
    {
        var returnType = typeof(AuthController).GetMethod(nameof(AuthController.Post))!.ReturnType;
        var actionResultType = returnType.GetGenericArguments().Single();

        Assert.Equal(typeof(ActionResult<LoginResponse>), actionResultType);
    }

    [Fact]
    public void LoginResponse_DoesNotExposePassword()
    {
        var payload = JsonSerializer.Serialize(new LoginResponse
        {
            Username = "admin",
            Token = "token-de-prueba",
            ExpiresAtUtc = DateTime.UtcNow
        });

        Assert.DoesNotContain("password", payload, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("admin123", payload, StringComparison.OrdinalIgnoreCase);
    }
}
