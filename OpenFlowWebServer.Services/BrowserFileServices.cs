using Microsoft.AspNetCore.Components.Forms;
using OpenFlowWebServer.Repository;
using File = OpenFlowWebServer.Domain.Entities.File;

namespace OpenFlowWebServer.Services
{
    public interface IBrowserFileService
    {
        Task<File> HandleFileChange(InputFileChangeEventArgs e, string container);
        Task<FileUpload> HandleFileChange2(IBrowserFile file, string container);
    }

    public class BrowserFileService: IBrowserFileService
    {
        private IBlobRepository<Stream> BlobRepository { get; set; }

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

        public async Task<FileUpload> HandleFileChange2(IBrowserFile file, string container)
        {
            var upload = new FileUpload();
            upload.File = new File()
            {
                //BlobGuid = Guid.NewGuid(),
                Container = container,
                Extension = file.Name.Split('.').Last(),
                Name = file.Name,
                Id = Guid.NewGuid()
            };

            upload.DataStream = file.OpenReadStream(file.Size); 
            return upload;
        }

    }





    public class FileUpload
    {
        public File File { get; set; }
        public Stream DataStream { get; set; }
    }



}
