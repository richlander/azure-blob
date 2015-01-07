using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadFilesToBlobContainer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Blob Storage Upload Tool");
            Console.WriteLine("Arguments: uploadDir azureStorageKey blobAccount blobContainer");

            if (args == null || args.Length !=4)
            {
                Console.WriteLine("Incorrect aguments passed to tool.");
                return;
            }

            var uploadDir = args[0];
            var key = args[1];
            var account = args[2];
            var container = args[3];
            var accountUri = new Uri(String.Format("http://{0}.blob.core.windows.net", account));
            var creds = new StorageCredentials(account, key);
            UploadCatalog(creds, accountUri, container, new DirectoryInfo(uploadDir));
        }

        static void UploadCatalog(StorageCredentials creds, Uri accountUri, string containerName, DirectoryInfo uploadDir)
        {
            var client = new CloudBlobClient(accountUri, creds);
            var container = client.GetContainerReference(containerName);
            container.CreateIfNotExists();
            var baseDirLength = uploadDir.FullName.Length + 1; ;
            var files = uploadDir.EnumerateFiles("*",SearchOption.AllDirectories);
                        
            foreach(var file in files)
            {
                string partialName = file.FullName.Substring(baseDirLength);
                Console.WriteLine("Writing {0}", partialName);
                var blockBlob = container.GetBlockBlobReference(partialName);
                using (var fileStream = file.OpenRead())
                {
                    blockBlob.UploadFromStream(fileStream);
                } 
            }
        }
    }
}
