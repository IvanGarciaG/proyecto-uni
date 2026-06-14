using MassTransit;
using Shared.Contracts.Events;

namespace Users.Infrastructure.Messaging;

// Ejemplo: otro servicio consumiendo el evento de usuario registrado
public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var evt = context.Message;
        // Lógica de reacción: enviar email de bienvenida, crear perfil, etc.
        await Task.CompletedTask;
    }
}
