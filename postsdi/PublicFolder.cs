using System;
using System.Linq;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace postsdi
{
    public class PublicFolder
    {
        private readonly string MIME_FOLDER = "application/vnd.google-apps.folder";
        private readonly string Q_FOLDER = "(mimeType = 'application/vnd.google-apps.folder' and trashed = false)";
        private readonly string Q_FILE = "(mimeType != 'application/vnd.google-apps.folder' and trashed = false)";

        DriveService service;
            
        public PublicFolder(DriveService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            this.service = service;
        }

        static string _q(string text)
        {
            return "'" + text.Replace("'", "''") + "'";
        }

        async Task<ChildList> ListFolder(string folderId, string query = null)
        {
            var files = service.Children.List(folderId);
            files.Q = query;
            return await files.ExecuteAsync();
        }

        async Task<File> CreateFolder(string parentFolderId, string name)
        {
            var folder = await service.Files.Insert(new File()
            {
                Title = name,
                MimeType = MIME_FOLDER,
                Parents = new[] { new ParentReference() { Id = parentFolderId } }.ToList(),
            }).ExecuteAsync();

            return folder;
        }

        public void UploadFile(string parentFolderId, string name, System.IO.Stream stream, string mimeType = "text/html")
        {
            var fileUploadProgress = service.Files.Insert(new File()
            {
                Title = name,
                Parents = new[] { new ParentReference() { Id = parentFolderId } }.ToList(),
            }, stream, mimeType).Upload();
        }

        public async Task<string> Setup(string folderName)
        {
            var pathList = new List<string>();
            for (var p = folderName; p != "" && p != "/" && p != "\\" && p != null; p = System.IO.Path.GetDirectoryName(p))
            {
                pathList.Insert(0, System.IO.Path.GetFileName(p));
            }

            var folderId = (await service.About.Get().ExecuteAsync()).RootFolderId;
            foreach (var name in pathList)
            {
                var fileList = await ListFolder(folderId, string.Format("{0} and title = {1}", Q_FOLDER, _q(name)));

                if (fileList.Items.Any())
                {
                    folderId = fileList.Items.First().Id;
                }
                else
                {
                    var folder = await CreateFolder(folderId, name);
                    folderId = folder.Id;
                }
            }

            var permission = new Permission()
            {
                Value = "",
                Type = "anyone",
                Role = "reader"
            };

            await service.Permissions.Insert(permission, folderId).ExecuteAsync();

            return folderId;
        }

    }
}

