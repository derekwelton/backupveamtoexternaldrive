using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleUI.Services
{
    public static class FileService
    {
        public static void SetDirectoriesAndFilesAttributesToNormal(this DirectoryInfo dir)
        {
            foreach (var subDir in dir.GetDirectories())
                subDir.SetDirectoriesAndFilesAttributesToNormal();
            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }
        }

        public static List<DriveInfo> GetAllExternalDrives()
        {
            List<DriveInfo> allDrives = DriveInfo.GetDrives().ToList();
            List<DriveInfo> backupDrives = new List<DriveInfo>();

            foreach (DriveInfo dirve in allDrives)
            {
                if (dirve.IsReady)
                {
                    if (dirve.DriveType == DriveType.Removable)
                    {
                        backupDrives.Add(dirve);
                    }
                }
            }

            return backupDrives;
        }

        public static void FormatExternalDrives(List<DriveInfo> drives)
        {
            foreach (var drive in drives)
            {
                var dirInfo = new DirectoryInfo(drive.Name);
                foreach (var dir in dirInfo.GetDirectories())
                {
                    if (dirInfo.Name.Contains(@"$RECYCLE")) continue;

                    try
                    {
                        dirInfo.SetDirectoriesAndFilesAttributesToNormal();
                        Program.log.Information("trying to delete directory {dirName} from drive: {volume}...", dir.Name, drive.VolumeLabel);
                        dir.Delete(true);
                        Program.log.Information("Successfully deleted {dirName} from drive: {volume}", dir.Name, drive.VolumeLabel);
                    }
                    catch
                    {
                        if (dirInfo.Name.Contains("WD")) Program.log.Fatal("COULD NOT DELETE {directory} from drive: {volume}", dir.Name, drive.VolumeLabel);
                        else Program.log.Warning("Could not delete {directory} from drive: {volume}", dir.Name, drive.VolumeLabel);
                    }
                }
            }
        }

        public static void CopyOverVeamFilesToDrives(List<DriveInfo> drives, string sourcePath)
        {
            var saturdayDate = DateService.GetLastSaturday();
            foreach (var drive in drives)
            {
                //setup directories on the drive to mirror the source
                var newRootPath = Directory.CreateDirectory($"{drive.Name}\\BACKUPS");
                var directoriesCollection = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
                foreach (var directory in directoriesCollection)
                {
                    Directory.CreateDirectory(directory.Replace(sourcePath, $"{newRootPath}\\"));
                }

                //begin copying over everything
                foreach (var filePath in Directory.GetFiles(sourcePath, "*.*",searchOption:SearchOption.AllDirectories))
                {
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.LastWriteTime >= saturdayDate)
                    {
                        File.Copy(filePath,filePath.Replace(sourcePath, $"{newRootPath}\\"),true);
                        Program.log.Information("Volume {volume}: File ({filename}) LastWriteDate ({date}) was copied.",drive.VolumeLabel,fileInfo.Name,fileInfo.LastWriteTime);
                    }
                }
            }
        }
    }
}