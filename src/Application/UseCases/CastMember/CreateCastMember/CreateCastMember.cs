using Application.Interfaces;
using Application.UseCases.CastMember.Common;
using Domain.Repository;
using Entities = Domain.Entity;

namespace Application.UseCases.CastMember;

public class CreateCastMember : ICreateCastMember
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICastMemberRepository _repository;

    public CreateCastMember(ICastMemberRepository castMemberRepository, IUnitOfWork unitOfWork)
    {
        _repository = castMemberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CastMemberModelOutput> Handle(CreateCastMemberInput input, CancellationToken cancellationToken)
    {
        var castMember = new Entities.CastMember(input.Name, input.Type);
        await _repository.Insert(castMember, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return CastMemberModelOutput.FromCastMember(castMember);
    }
}