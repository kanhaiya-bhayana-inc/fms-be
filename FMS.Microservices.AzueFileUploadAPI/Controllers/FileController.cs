using FMS.Services.AzueFileUploadAPI.Model.Dto;
using FMS.Services.AzueFileUploadAPI.Services;
using FMS.Services.AzueFileUploadAPI.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FMS.Services.AzueFileUploadAPI.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IUploadFile _uploadFile;

        public FileController(IUploadFile uploadFile)
        {
            _uploadFile = uploadFile;
        }
        [HttpPost("UploadFile")]
        public async Task<AzureBlobResponseDto> UploadFile([FromForm] FileManagementDTO fileManagementDTO)
        {
            var response = _uploadFile.UploadFileAsync(fileManagementDTO);
            return response;
        }
    }
}
