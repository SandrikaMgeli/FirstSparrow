using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.ConfirmOwnerPhoneCommand;

public class ConfirmOwnerPhoneCommandHandler : IRequestHandler<ConfirmOwnerPhoneCommand, ConfirmOwnerPhoneResponse>
{
    public Task<ConfirmOwnerPhoneResponse> Handle(ConfirmOwnerPhoneCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}