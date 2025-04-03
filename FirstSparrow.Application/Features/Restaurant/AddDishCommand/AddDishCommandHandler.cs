using FirstSparrow.Application.Shared;
using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.AddDishCommand;

public class AddDishCommandHandler(RequestMetadata requestMetadata) : IRequestHandler<AddDishCommand, AddDishResponse>
{
    public Task<AddDishResponse> Handle(AddDishCommand request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}