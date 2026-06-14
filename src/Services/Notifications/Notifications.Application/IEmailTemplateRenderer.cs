namespace Notifications.Application;

public interface IEmailTemplateRenderer
{
    /// <summary>Renderiza una plantilla Scriban/Razor por nombre con el modelo dado.</summary>
    string Render(string templateName, object model);
}
