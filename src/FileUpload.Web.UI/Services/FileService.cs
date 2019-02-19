using FileUpload.Models;
using Neptuo;
using Neptuo.Text.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Services
{
    public class FileService
    {
        public IReadOnlyList<FileModel> FindList(UploadSettings configuration)
        {
            Ensure.NotNull(configuration, "configuration");

            if (!configuration.IsBrowserEnabled)
                return null;

            if (!Directory.Exists(configuration.StoragePath))
                return Array.Empty<FileModel>();

            List<FileModel> files = Directory
                .EnumerateFiles(configuration.StoragePath)
                .Where(f => configuration.IsSupportedExtension(Path.GetExtension(f).ToLowerInvariant()))
                .Select(f => new FileModel(new FileInfo(f)))
                .OrderBy(f => f.Name)
                .ToList();

            return files;
        }

        public (FileStream Content, string ContentType)? FindContent(UploadSettings configuration, string fileName, string extension)
        {
            Ensure.NotNull(configuration, "configuration");

            if (!configuration.IsDownloadEnabled)
                return null;

            if (extension == null)
                return null;

            extension = "." + extension;
            fileName = fileName + extension;
            if (fileName.Contains(Path.DirectorySeparatorChar) || fileName.Contains(Path.AltDirectorySeparatorChar) || fileName.Contains("..") || Path.IsPathRooted(fileName))
                return null;

            extension = extension.ToLowerInvariant();
            if (!configuration.IsSupportedExtension(extension))
                return null;

            string filePath = Path.Combine(configuration.StoragePath, fileName);
            if (File.Exists(filePath))
            {
                string contentType = "application/octet-stream";
                if (extension == ".gif")
                    contentType = "image/gif";
                else if (extension == ".png")
                    contentType = "image/png";
                else if (extension == ".jpg" || extension == ".jpeg")
                    contentType = "image/jpg";

                return (new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), contentType);
            }

            return null;
        }

        public async Task<bool> SaveAsync(UploadSettings configuration, string name, long length, Stream content)
        {
            Ensure.NotNull(configuration, "configuration");

            if (length > configuration.MaxLength)
                return false;

            string extension = Path.GetExtension(name);
            if (extension == null)
                return false;

            extension = extension.ToLowerInvariant();
            if (!configuration.IsSupportedExtension(extension))
                return false;

            DirectoryInfo directory = new DirectoryInfo(configuration.StoragePath);
            if (configuration.MaxStorageLength != null && directory.GetLength() + length > configuration.MaxStorageLength.Value)
                return false;

            if (!directory.Exists)
                directory.Create();

            string filePath = Path.Combine(configuration.StoragePath, name);
            if (File.Exists(filePath))
            {
                if (configuration.IsOverrideEnabled)
                {
                    TryBackupFile(configuration, filePath);
                    File.Delete(filePath);
                }
                else
                {
                    return false;
                }
            }

            using (Stream fileContent = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                await content.CopyToAsync(fileContent);

            return true;
        }

        private static void TryBackupFile(UploadSettings configuration, string filePath)
        {
            if (!String.IsNullOrEmpty(configuration.BackupTemplate))
            {
                TokenWriter writer = new TokenWriter(configuration.BackupTemplate);

                int order = 0;
                string newFilePath = null;
                do
                {
                    string newFileName = writer.Format(token =>
                    {
                        if (token == "FileName")
                            return Path.GetFileNameWithoutExtension(filePath);

                        if (token == "Extension")
                            return Path.GetExtension(filePath).Substring(1);

                        if (token == "Order")
                            return (++order).ToString();

                        throw Ensure.Exception.NotSupported($"Not supported token '{token}' in backup template '{configuration.BackupTemplate}'.");
                    });

                    string currentNewFilePath = Path.Combine(Path.GetDirectoryName(filePath), newFileName);

                    if (currentNewFilePath == newFilePath || order > 100)
                        throw Ensure.Exception.InvalidOperation($"Maximum path probing reached on path '{newFilePath}'.");

                    newFilePath = currentNewFilePath;
                }
                while (File.Exists(newFilePath));

                File.Copy(filePath, newFilePath);
            }
        }

        public bool Delete(UploadSettings configuration, string fileName)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");

            if (!configuration.IsDeleteEnabled)
                return false;

            string extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (!configuration.IsSupportedExtension(extension))
                return false;

            if (!Directory.Exists(configuration.StoragePath))
                return false;

            string filePath = Path.Combine(configuration.StoragePath, fileName);
            File.Delete(filePath);

            return true;
        }
    }
}
