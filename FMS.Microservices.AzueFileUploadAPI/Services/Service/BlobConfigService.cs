﻿using FMS.Services.AzueFileUploadAPI.Helpers;
using FMS.Services.AzueFileUploadAPI.Model.Dto;
using FMS.Services.AzueFileUploadAPI.Services.IService;

namespace FMS.Services.AzueFileUploadAPI.Services.Service
{
    public class BlobConfigService : IBlobConfigService
    {
        private readonly string _blobConnectionString;
        private readonly string _dbConnectionString;
        public BlobConfigService(IConfiguration configuration) 
        {
            _blobConnectionString = configuration.GetValue<string>("BlobConnectionString");
            _dbConnectionString = configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
        }
        public async Task<BlobConfigDto> GetBlobInfoAsync()
        {
            try
            {
                
                BlobConfigDto response = new BlobConfigDto();
                response.AccountName = ExtractNameFromString.GetNameAsync(_blobConnectionString,"AccountName");
                response.ServerName = ExtractNameFromString.GetNameAsync(_dbConnectionString, "Data Source");

                return response;

            }
            catch (Exception ex) 
            {
                throw ex;
            }

        }
    }
}
