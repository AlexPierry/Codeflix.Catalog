using Domain.Enum;

namespace Api.Models.CastMember;

public class UpdateCastMemberApiInput
{
    public string Name { get; set; }
    public CastMemberType? Type { get; set; }

    public UpdateCastMemberApiInput(string name, CastMemberType? type = null)
    {
        Name = name;
        Type = type;
    }
}