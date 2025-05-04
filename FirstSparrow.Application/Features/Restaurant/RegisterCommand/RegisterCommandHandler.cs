using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.RegisterCommand;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    public Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new RegisterResponse());
    }
}