using FMS.Services.AzueFileUploadAPI.Model;
using FMS.Services.AzueFileUploadAPI.Model.Dto;

namespace FMS.Services.AzueFileUploadAPI.Helpers
{
    public class DataMapping
    {
        public static FileManagement MapData(FileManagementDTO request)
        {
            FileManagement response = new FileManagement();
            if (request != null)
            {
                response.FileDate = request.FileDate;
                response.FileName = request.FileName;
                response.SourcePath = $"00landing{request.SourcePath}";
                response.DestinationPath = request.DestinationPath;
                response.EmailID = request.EmailID;
                response.Delimiter = request.Delimiter;
                response.IsActive = request.IsActive == "true" ? "Y" : "N";
                response.ClientID = request.VendorName;
                response.FileTypeID = request.FileType;
                response.InsertionMode = request.InsertionMode;
                response.TemplateName = request.TemplateFile.FileName;
                response.FixedLength = request.FixedLength == "true" ? "Y" : "N";
                response.DbNoteBook = request.DbNotebook;
            }

            return response;
        }
    }
}


