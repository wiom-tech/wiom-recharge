using Amazon.S3;
using Amazon.S3.Model;
using i2e1_basics.Utilities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace i2e1_core.Utilities
{
    public class S3BasicUtils
    {
        private static S3BasicUtils instance;
        private IAmazonS3 client;
        private S3BasicUtils(string s3AccessKey, string s3SecretKey)
        {
            this.client = new AmazonS3Client(s3AccessKey, s3SecretKey, I2e1ConfigurationManager.GetAWSRegion());
        }

        public static S3BasicUtils CreateInstance(string s3AccessKey, string s3SecretKey)
        {
            if (instance == null)
                instance = new S3BasicUtils(s3AccessKey, s3SecretKey);

            return instance;
        }

        public static S3BasicUtils GetInstance()
        {
            return instance;
        }

        public async Task<string> CreateFile(System.IO.Stream file, string relativeFilePath, string folder, string bucketName = "i2e1-client-data")
        {
            if (relativeFilePath.IndexOf('/') != 0) relativeFilePath = "/" + relativeFilePath;
            return await CreateFile(new PutObjectRequest
            {
                BucketName = bucketName,
                Key = folder + relativeFilePath,
                InputStream = file,
                CannedACL = S3CannedACL.PublicRead
            });
        }

        public async Task<string> CreateJsonFile(JObject content, string relativeFilePath, string folder, string bucketName)
        {
            if (relativeFilePath.IndexOf('/') != 0) relativeFilePath = "/" + relativeFilePath;
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(content));
            var seekableStream = new MemoryStream(byteArray);
            seekableStream.Position = 0;

            return await CreateFile(new PutObjectRequest
            {
                BucketName = bucketName,
                Key = $"{folder}{relativeFilePath}.json",
                InputStream = seekableStream
            });
        }

        public async Task<string> UploadFile(IFormFile file, string relativeFilePath, string folder, string bucketName = "i2e1-client-data")
        {
            if (relativeFilePath.IndexOf('/') != 0) relativeFilePath = "/" + relativeFilePath;
            return await CreateFile(new PutObjectRequest
            {
                BucketName = bucketName,
                Key = folder + relativeFilePath,
                InputStream = file.OpenReadStream(),
                CannedACL = S3CannedACL.PublicRead
            });
        }

        public async Task<string> UploadFile(Stream stream, string relativeFilePath, string folder, string bucketName = "i2e1-client-data")
        {
            if (relativeFilePath.IndexOf('/') != 0) relativeFilePath = "/" + relativeFilePath;
            return await CreateFile(new PutObjectRequest
            {
                BucketName = bucketName,
                Key = folder + relativeFilePath,
                InputStream = stream,
                CannedACL = S3CannedACL.PublicRead
            });
        }

        public async Task<string> CreateFile(PutObjectRequest putRequest)
        {
            try
            {
                if (putRequest.Key.IndexOf('/') == 0) putRequest.Key = putRequest.Key.Substring(1);

                PutObjectResponse response = await client.PutObjectAsync(putRequest);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    var url = $"https://{putRequest.BucketName}.s3.amazonaws.com/{putRequest.Key}";
                    return url;
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Logger.GetInstance().Error("Check the provided AWS Credentials.");
                }
                else
                {
                    Logger.GetInstance().Error(amazonS3Exception.Message);
                }
                throw new System.Exception("Error uploading file: " + amazonS3Exception.Message);
            }

            return null;
        }
    }
}
