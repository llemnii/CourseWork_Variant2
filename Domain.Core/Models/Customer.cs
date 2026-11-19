using Domain.Core.Common;

namespace Domain.Core.Models;

public class Customer : EntityBase
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public Customer()
    {
    }

    public Customer(string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Ім’я клієнта не може бути порожнім.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Введено некоректну електронну пошту.");

        FullName = fullName.Trim();
        Email = email.Trim();
    }

    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(FullName))
            throw new ArgumentException("Ім’я клієнта не може бути порожнім.");
        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
            throw new ArgumentException("Введено некоректну електронну пошту.");
    }
}
