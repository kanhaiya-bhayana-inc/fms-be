using FMS.Services.AzueFileUploadAPI.Model.Dto;
using FMS.Services.AzueFileUploadAPI.Services;
using Microsoft.AspNetCore.Mvc;
using FMS.Services.AzueFileUploadAPI.DBContext;
using System.Data.SqlClient;
using FMS.Services.AzueFileUploadAPI.Helpers;

namespace FMS.Services.AzueFileUploadAPI.Repository
{
    public class UploadFile : IUploadFile
    {
        private readonly IConfiguration _configuration;
        //private DBConnect db;

        private readonly FileUpload _uploadFile;
        private readonly string _tempFileContainerName;
        private readonly string _sampFileContainerName;


        public UploadFile(IConfiguration configuration, FileUpload uploadFile)
        {
            _configuration = configuration;
            //db = new DBConnect(_configuration);
            _uploadFile = uploadFile;
            _tempFileContainerName = configuration.GetValue<string>("BlobContainerNameTempFile");
            _sampFileContainerName = configuration.GetValue<string>("BlobContainerNameSampFile");
        }
        public AzureBlobResponseDto UploadFileAsync([FromForm] FileManagementDTO fileManagementDTO)
        {
            var requestData = DataMapping.MapData(fileManagementDTO);
            var t = new FileManagementDTO();
            AzureBlobResponseDto response = new();
            var fileNameObj = new NewFileNameDto();
            string fileNameNew = "";
            string connectionString = "Data Source=avd-devper1-107;Initial Catalog=IncedoFMSDb;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("UpsertIntoFileDetailsMaster2", connection))
                {
                    Guid id = new Guid();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FileMasterId", (requestData.FileMasterId)==null?id:requestData.FileMasterId);
                    command.Parameters.AddWithValue("@FileName", requestData.FileName);
                    command.Parameters.AddWithValue("@SourcePath", requestData.SourcePath);
                    command.Parameters.AddWithValue("@DestinationPath", requestData.DestinationPath);
                    command.Parameters.AddWithValue("@FileTypeID", requestData.FileTypeID);
                    command.Parameters.AddWithValue("@Delimiter", requestData.Delimiter);
                    command.Parameters.AddWithValue("@FixedLength", requestData.FixedLength);
                    command.Parameters.AddWithValue("@TemplateName", requestData.TemplateName);
                    command.Parameters.AddWithValue("@EmailID", requestData.EmailID);
                    command.Parameters.AddWithValue("@ClientID", requestData.ClientID);
                    command.Parameters.AddWithValue("@FileDate", requestData.FileDate);
                    command.Parameters.AddWithValue("@InsertionMode", requestData.InsertionMode);
                    command.Parameters.AddWithValue("@IsActive", requestData.IsActive);
                    command.Parameters.AddWithValue("@DbNotebook", requestData.DbNotebook);

                    SqlTransaction transaction = connection.BeginTransaction();
                    try
                    {
                        command.Transaction = transaction;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                fileNameObj.GUID = reader.GetGuid(0);
                                fileNameObj.FileName = reader.GetString(1);
                                t = DataMapping.MapReturnData(reader);
                            }

                            if (fileNameObj.GUID != null && fileNameObj.FileName != null)
                            {
                                fileNameNew = $"{fileNameObj.GUID}{fileNameObj.FileName}";

                            }
                        }

                        // create new file name

                        IFormFile uploadFile = null;
                        AzureBlobResponseDto response1 = new();
                        AzureBlobResponseDto response2 = new();
                        string newFileName = RenameFile.CreateNewFileName(fileManagementDTO.TemplateFile.FileName, fileNameNew);
                        if (newFileName != null)
                        {
                            uploadFile = new RenameFile(fileManagementDTO.TemplateFile, newFileName);
                            t.TemplateFile = uploadFile;
                            t.SampleFile = fileManagementDTO.SampleFile;
                            response1 = _uploadFile.FileUploadAsync(uploadFile,_tempFileContainerName).Result;
                        }

                        if (!response1.Error)
                        {
                            if (fileManagementDTO.SampleFile != null)
                            {
                                response2 = _uploadFile.FileUploadAsync(fileManagementDTO.SampleFile,_sampFileContainerName).Result;
                                if (!response2.Error)
                                {
                                    transaction.Commit();
                                    response.Error = false;
                                    response.Status = response2.Status;
                                    response.data = t;
                                    Console.WriteLine("Transaction Committed");
                                }
                                else
                                {
                                    response.Error = true;
                                    response.Status = response2.Status;
                                    throw new Exception(response.Status);
                                }
                            }
                            else
                            {
                                transaction.Commit();
                                response.Error = false;
                                response.Status = response1.Status;
                                response.data = t;
                                Console.WriteLine("Transaction Committed");
                            }
                        }
                        else
                        {
                            response.Error = true;
                            response.Status = response1.Status;
                            throw new Exception(response.Status);
                        }

                    }
                    catch (Exception EX)
                    {
                        transaction.Rollback();
                        response.Error = true;
                        response.Status = EX.Message;
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
            }

            return response;
        }
    }
}
