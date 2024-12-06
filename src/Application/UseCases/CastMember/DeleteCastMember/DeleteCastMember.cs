using Application.Interfaces;
using Domain.Repository;

namespace Application.UseCases.CastMember;

public class DeleteCastMember : IDeleteCastMember
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICastMemberRepository _repository;

    public DeleteCastMember(ICastMemberRepository castMemberRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = castMemberRepository;
    }

    public async Task Handle(DeleteCastMemberInput request, CancellationToken cancellationToken)
    {
        var category = await _repository.Get(request.Id, cancellationToken);
        await _repository.Delete(category, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
    }
}