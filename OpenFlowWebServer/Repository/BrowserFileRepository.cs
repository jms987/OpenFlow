using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using OpenFlowWebServer.Data.Repositories;
using File = OpenFlowWebServer.Data.Domain.File;

namespace OpenFlowWebServer.Repository
{
    public interface IBrowserFileService
    {
        Task<File> HandleFileChange(InputFileChangeEventArgs e, string container);
    }

    public class BrowserFileService: IBrowserFileService
    {
        private IBlobRepository<Stream> BlobRepository { get; set; }
        private IFileRepository FileRepository { get; set; }
        private IBrowserFile? InputFile;

        public BrowserFileService(IBlobRepository<Stream> blobRepository)
        {
            BlobRepository = blobRepository;
        }

        public async Task<File> HandleFileChange(InputFileChangeEventArgs e, string container)
        {
            var databaseFile = new File();
            var browserFile = e.File;

            databaseFile.Id = Guid.NewGuid();
            databaseFile.Container = container;
            databaseFile.Name = e.File.Name;
            var stream = browserFile.OpenReadStream();
            var results = await BlobRepository.AddBlobAsync(stream, container);

            databaseFile.BlobGuid = results.BlobId;
            databaseFile.Extension = e.File.Name.Split('.').Last();
            databaseFile.Url = results.BlobUrl;
            return databaseFile;
            /*await FileRepository.AddAsync(databaseFile);
            await FileRepository.SaveChangesAsync();*/
            /*databaseFile.Name = e.File.Name;*/
            /*return Task.CompletedTask;*/


            /*InputFile.Id = Guid.NewGuid()*/
            /*
            ;
            /*InputFile.Container = "Config";#1#
            InputFile.FileName = e.File.Name;
            using var stream = ConfigFile.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            InputFile.Base64data = Convert.ToBase64String(memoryStream.ToArray());
            InputFile.FileType = e.File.Name.Split('.').Last();
            /*InputFile.Container = "Config";#1#
            /*ConfigInputFile = InputFile;#1#
            */



        }

        /*private async void HandleFileChange(InputFileChangeEventArgs e, string container)
        {
            var InputFile = new File();
            var ConfigFile = e.File;
            /*InputFile.Id = Guid.NewGuid()#1#;
            /*InputFile.Container = "Config";#1#
            InputFile.FileName = e.File.Name;
            using var stream = ConfigFile.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            InputFile.Base64data = Convert.ToBase64String(memoryStream.ToArray());
            InputFile.FileType = e.File.Name.Split('.').Last();
            /*InputFile.Container = "Config";#1#
            /*ConfigInputFile = InputFile;#1#

            blo

        }*/


    }

    public class FileUpload
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Base64data { get; set; }
    }
}
