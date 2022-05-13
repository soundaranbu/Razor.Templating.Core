namespace EmailService.Templates.WelcomeEmail;

public class WelcomeEmailModel
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string SenderName { get; set; } = null!;
}
