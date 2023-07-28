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


        public UploadFile(IConfiguration configuration, FileUpload uploadFile)
        {
            _configuration = configuration;
            //db = new DBConnect(_configuration);
            _uploadFile = uploadFile;
        }
        public AzureBlobResponseDto UploadFileAsync([FromForm] FileManagementDTO fileManagementDTO)
        {
            var requestData = DataMapping.MapData(fileManagementDTO);
            AzureBlobResponseDto response = new();
            var fileNameObj = new NewFileNameDto();
            string fileNameNew = "";
            string connectionString = "Data Source=avd-devper1-107;Initial Catalog=IncedoFMSDb;Integrated Security=True";
            
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("InsertIntoFileDetailsMaster",connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FileName", requestData.FileName);
                        command.Parameters.AddWithValue("@SourcePath", requestData.SourcePath);
                        command.Parameters.AddWithValue("@DestinationPath", requestData.DestinationPath);
                        command.Parameters.AddWithValue("@FileTypeID", requestData.FileTypeID);
                        command.Parameters.AddWithValue("@Delimeter", requestData.Delimiter);
                        command.Parameters.AddWithValue("@FixedLength", requestData.FixedLength);
                        command.Parameters.AddWithValue("@TemplateName", requestData.TemplateName);
                        command.Parameters.AddWithValue("@EmailID", requestData.EmailID);
                        command.Parameters.AddWithValue("@ClientID", requestData.ClientID);
                        command.Parameters.AddWithValue("@FileDate", requestData.FileDate);
                        command.Parameters.AddWithValue("@InsertionMode", requestData.InsertionMode);
                        command.Parameters.AddWithValue("@IsActive", requestData.IsActive);
                        command.Parameters.AddWithValue("@DbNotebook", requestData.DbNoteBook);

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
                                }
                                
                                if (fileNameObj.GUID != null && fileNameObj.FileName != null)
                                {
                                    fileNameNew = $"{fileNameObj.GUID}{fileNameObj.FileName}";

                                }
                            }

                            // create new file name

                            IFormFile uploadFile = null;
                            AzureBlobResponseDto response1 = new ();
                            AzureBlobResponseDto response2 = new();
                            string newFileName = RenameFile.CreateNewFileName(fileManagementDTO.TemplateFile.FileName, fileNameNew);
                            if (newFileName != null)
                            {
                                uploadFile = new RenameFile(fileManagementDTO.TemplateFile, newFileName);
                                response1 = _uploadFile.FileUploadAsync(uploadFile).Result;
                            }

                            if (!response1.Error)
                            {
                                if (fileManagementDTO.SampleFile != null)
                                {
                                    response2 = _uploadFile.FileUploadAsync(fileManagementDTO.SampleFile).Result;
                                    if (!response2.Error)
                                    {
                                        transaction.Commit();
                                        response.Error = false;
                                        response.Status = response2.Status;
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
