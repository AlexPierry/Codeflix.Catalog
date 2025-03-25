using Application.UseCases.Video.Common;
using MediatR;

namespace Application.UseCases.Video.UploadMedias;

public record UploadMediasInput(Guid videoId, FileInput? videoFile, FileInput? trailerFile) : IRequest;
