using RMS.Contants;
using RMS.Dtos.Tags.Create;

namespace RMS.IService.ITags.ICreate;

public interface ITagCreateSingleService
{
    Task<ServiceResult> ExecuteAsync(int userId, TagCreateSingleDto request);
}