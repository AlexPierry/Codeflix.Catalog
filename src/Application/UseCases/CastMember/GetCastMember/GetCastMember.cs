using Application.UseCases.CastMember.Common;
using Domain.Repository;

namespace Application.UseCases.CastMember;

public class GetCastMember : IGetCastMember
{
    private readonly ICastMemberRepository _repository;

    public GetCastMember(ICastMemberRepository castMemberRepository)
    {
        _repository = castMemberRepository;
    }

    public async Task<CastMemberModelOutput> Handle(GetCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await _repository.Get(request.Id, cancellationToken);

        return CastMemberModelOutput.FromCastMember(castMember);
    }
}