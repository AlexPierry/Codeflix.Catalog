using Application.Interfaces;
using Application.UseCases.CastMember.Common;
using Domain.Repository;

namespace Application.UseCases.CastMember;

public class UpdateCastMember : IUpdateCastMember
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICastMemberRepository _repository;

    public UpdateCastMember(ICastMemberRepository castMemberRepository, IUnitOfWork unitOfWork)
    {
        _repository = castMemberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CastMemberModelOutput> Handle(UpdateCastMemberInput request, CancellationToken cancellationToken)
    {
        var currentCastMember = await _repository.Get(request.Id, cancellationToken);
        currentCastMember.Update(request.Name, request.Type ?? currentCastMember.Type);

        await _repository.Update(currentCastMember, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return CastMemberModelOutput.FromCastMember(currentCastMember);
    }
}