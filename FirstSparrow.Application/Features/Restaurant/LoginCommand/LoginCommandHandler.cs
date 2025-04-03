using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.LoginCommand;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    public Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}