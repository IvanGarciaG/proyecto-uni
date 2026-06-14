using Notifications.Application;
using Scriban;

namespace Notifications.Infrastructure.Email;

/// <summary>
/// Renderiza plantillas HTML usando Scriban.
/// Las plantillas se buscan en Templates/ relativo al assembly.
/// </summary>
public class ScribanTemplateRenderer : IEmailTemplateRenderer
{
    private static readonly string TemplatesDir =
        Path.Combine(AppContext.BaseDirectory, "Templates");

    public string Render(string templateName, object model)
    {
        var path = Path.Combine(TemplatesDir, $"{templateName}.html");
        var source = File.ReadAllText(path);
        var template = Template.Parse(source);
        return template.Render(new { model });
    }
}
