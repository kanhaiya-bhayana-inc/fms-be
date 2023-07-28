using FMS.Services.AzueFileUploadAPI.Model;
using FMS.Services.AzueFileUploadAPI.Model.DropdownOptions;

namespace FMS.Services.AzueFileUploadAPI.Services.IService
{
    public interface IFilesDataService
    {
        Task<IEnumerable<FileManagement>> GetAllFilesAsync();
    }
}
