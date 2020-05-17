using elFinder.NetCore;
using elFinder.NetCore.Drivers;
using elFinder.NetCore.Drivers.FileSystem;
using SK.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace SK.Business.Services
{

    public class FileService : Service
    {
        public FileService(ServiceInjection inj) : base(inj)
        {
        }

        public FileSystemDriver GetFileSystemDriver(RootVolume rootVolume)
        {
            var driver = new FileSystemDriver();
            driver.AddRoot(rootVolume);
            return driver;
        }

        public Connector GetConnector(IDriver driver)
        {
            return new Connector(driver)
            {
                MimeDetect = MimeDetect.INTERNAL
            };
        }

        public async Task<string> GetFileUrlAsync(
            FileDestination fileDest,
            FileDestinationMetadata metadata)
        {
            if (fileDest.RelativePath != null)
                return metadata.SourceUrl + "/" + fileDest.RelativePath;
            else if (fileDest.EFHash != null)
            {
                var driver = GetFileSystemDriver(metadata.RootVolume);
                var fullPath = await driver.ParsePathAsync(fileDest.EFHash);
                var file = fullPath.File;
                var relPath = file.FullName.Replace(metadata.RootPath, "")
                    .Replace("\\", "/");
                return metadata.SourceUrl + relPath;
            }
            return fileDest.Url;
        }

        public async Task<IFile> GetFileAsync(FileDestination fileDest,
            FileDestinationMetadata metadata)
        {
            if (fileDest.RelativePath != null)
                return new FileSystemFile(metadata.RootPath + "/" + fileDest.RelativePath);
            else if (fileDest.EFHash != null)
            {
                var driver = GetFileSystemDriver(metadata.RootVolume);
                var fullPath = await driver.ParsePathAsync(fileDest.EFHash);
                var file = fullPath.File;
                return file;
            }
            return null;
        }

    }
}
