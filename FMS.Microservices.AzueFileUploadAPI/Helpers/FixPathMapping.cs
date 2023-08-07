using FMS.Services.AzueFileUploadAPI.Utility;
using Microsoft.AspNetCore.Components.Forms;

namespace FMS.Services.AzueFileUploadAPI.Helpers
{
    public class FixPathMapping
    {
        private readonly string _tempContainerName;
        private readonly string _sampContainerName;
        public FixPathMapping(IConfiguration configuration)
        {
            _tempContainerName = configuration.GetValue<string>("BlobContainerNameTempFile");
            _sampContainerName = configuration.GetValue<string>("BlobContainerNameSampFile");
        }
        public string FixPathMapper(string path, string fileType)
        {
            string[] pathComponents = path.Split('/');
            string returnPath = "";
            if (fileType == StaticDetails.fileTypeTemplate)
            {
                if (pathComponents.Length >= 3)
                {
                    returnPath = StaticDetails.templateFilePath;
                }
            }
            else if(fileType == StaticDetails.fileTypeSample)
            {
                if (pathComponents.Length >= 2)
                {
                    returnPath = string.Join("/", pathComponents, 1, 4);
                }
            }
            

            return returnPath;
        }
    }
}
