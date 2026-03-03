using MediatR;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Auth;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, MeResult?>
{
    private readonly IUserRepository _userRepository;

    public GetMeQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<MeResult?> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return null;

        return new MeResult(user.Id, user.Email, user.IsActive);
    }
}
