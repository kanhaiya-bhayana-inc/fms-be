using FMS.Services.AzueFileUploadAPI.Model.DropdownOptions;
using FMS.Services.AzueFileUploadAPI.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FMS.Services.AzueFileUploadAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IDrowpdownOptionsService _dropdownOptions;
        private readonly IFilesDataService _filesData;

        public DataController(IDrowpdownOptionsService drowpdownOptionsService,IFilesDataService filesData)
        {
            _dropdownOptions = drowpdownOptionsService;
            _filesData = filesData;

        }

        [HttpGet("GetDelimiters")]
        public async Task<IActionResult> GetDelimiters()
        {
            try
            {
                var dropdownOptions = await _dropdownOptions.GetDelimiterOptionsAsync();
                return new JsonResult(dropdownOptions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetVendorDetails")]
        public async Task<IActionResult> GetVendorDetails()
        {
            try
            {
                var dropdownOptions = await _dropdownOptions.GetVendorOptionsAsync();
                return new JsonResult(dropdownOptions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetFiledateDetails")]
        public async Task<IActionResult> GetFiledateDetails()
        {
            try
            {
                var dropdownOptions = await _dropdownOptions.GetFiledateOptionsAsync();
                return new JsonResult(dropdownOptions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetFiletypeDetails")]
        public async Task<IActionResult> GetFiletypeDetails()
        {
            try
            {
                var dropdownOptions = await _dropdownOptions.GetFiledtypeOptionsAsync();
                return new JsonResult(dropdownOptions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetAllFilesDetails")]
        public async Task<IActionResult> GetAllFilesDetails()
        {
            try
            {
                var response = await _filesData.GetAllFilesAsync();
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
