using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace QueueTrigeredFunc
{
    public class BlobReader
    {
        private CloudStorageAccount _storageAccount { get; }
        private CloudBlobClient _blobClient { get; }
        private string _containerName { get; }

        public BlobReader(string connectionString, string container)
        {            
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _containerName = container;
            _blobClient = _storageAccount.CreateCloudBlobClient();
        }

        public CloudBlobContainer BlobContainer
        {
            get
            {
                CloudBlobContainer blobContainer = _blobClient.GetContainerReference(_containerName);
                blobContainer.CreateIfNotExistsAsync();

                return blobContainer;
            }
        }

        public async Task<byte[]> GetBlobDocument(string blobFileName)
        {
            CloudBlockBlob blob = BlobContainer.GetBlockBlobReference(blobFileName);
            await blob.FetchAttributesAsync();
            long fileByteLength = blob.Properties.Length;
            byte[] fileContent = new byte[fileByteLength];
            for (int i = 0; i < fileByteLength; i++)
            {
                fileContent[i] = 0x20;
            }
            await blob.DownloadToByteArrayAsync(fileContent, 0);
            return fileContent;
        }

    }
}