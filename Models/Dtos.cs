namespace estudiantes_icinf.Models;

// Los campos son anulables para que FluentValidation pueda informar
// "campo obligatorio" en vez de recibir valores por defecto (0, "").

public class CreateStudentDto
{
    public string? Name { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public int? Age { get; set; }
    public List<CreatePetDto>? Pets { get; set; }
}

public class UpdateStudentDto
{
    public string? Name { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public int? Age { get; set; }
}

public class CreatePetDto
{
    public string? Name { get; set; }
    public string? Species { get; set; }
    public int? Age { get; set; }
}

public class UpdatePetDto
{
    public string? Name { get; set; }
    public string? Species { get; set; }
    public int? Age { get; set; }
}

public class RegisterDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class LoginDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
