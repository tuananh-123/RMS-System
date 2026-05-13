using RMS.Contants;
using RMS.Dtos.Tags.Create;

namespace RMS.IService.ITags.ICreate;

public interface ITagCreateService
{
    Task<ServiceResult> ExecuteAsync(int userId, TagCreateDto request);
}