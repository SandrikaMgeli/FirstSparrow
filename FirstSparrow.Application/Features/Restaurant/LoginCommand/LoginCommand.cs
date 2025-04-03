using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.LoginCommand;

public class LoginCommand : IRequest<LoginResponse>
{
    public string PhoneNumber { get; set; }

    public string Password { get; set; }
}