using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace FTPSync
{
    class Program
    {
        private static readonly string _basePath = "/Users/jinyazhou/Documents/Site/";

        //新旧文件夹字典
        private static List<string> _oldFolderList = new List<string>();
        private static List<string> _newFolderList = new List<string>();
        //新旧文件字典
        private static Dictionary<string, string> _oldFileList = new Dictionary<string, string>();
        private static Dictionary<string, string> _newFileList = new Dictionary<string, string>();
        private static Timer _syncTimer = null;
        private static bool _workDown = true;

        static void Main()
        {
            try
            {
                _syncTimer = new Timer(SyncEvent, null, 0, 10000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
            _syncTimer.Dispose();
        }

        static void SyncEvent(object obj)
        {
            if (!_workDown)
            {
                return;
            }

            _workDown = false;

            _newFolderList = new List<string>();
            _newFileList = new Dictionary<string, string>();
            
            _newFolderList = GetSubFolders(_basePath);
            _newFileList = GetAllFiles(_basePath);

            if (_oldFileList.SequenceEqual(_newFileList))
            {
                _workDown = true;

                return;
            }

            /*-----「删除」差异文件夹-----*/
            var deletedFolderList = _oldFolderList.Except(_newFolderList).ToList();
            if (deletedFolderList.Count > 0)
            {
                foreach (var folder in deletedFolderList)
                {
                    string remoteFolderPath = folder.Replace(_basePath, "");
                    var result = FtpClient.DeleteDir(remoteFolderPath);

                    if (result.Contains("successfully"))
                    {
                        Console.WriteLine($"{DateTime.Now} - delete folder {remoteFolderPath} OK.");
                    }
                }
            }

            /*-----「新增」差异文件夹-----*/
            var addedFolderList = _newFolderList.Except(_oldFolderList).ToList();
            if (addedFolderList.Count > 0)
            {
                foreach (var folder in addedFolderList)
                {
                    string remoteFolderPath = folder.Replace(_basePath, "");
                    var result = FtpClient.CreateDir(remoteFolderPath);

                    if (result.Contains("successfully"))
                    {
                        Console.WriteLine($"{DateTime.Now} - create folder {remoteFolderPath} OK.");
                    }
                }
            }

            /*-----「删除」差异文件-----*/
            var deletedFileList = _oldFileList.Except(_newFileList).ToDictionary(x => x.Key, x => x.Value);
            if (deletedFileList.Count > 0)
            {
                foreach (var file in deletedFileList)
                {
                    string filePath = file.Key;
                    string remoteFilePath = filePath.Replace(_basePath, "");
                    FtpClient.DeleteFile(remoteFilePath);

                    Console.WriteLine($"{DateTime.Now} - delete file {remoteFilePath} OK.");
                }
            }

            /*-----「新增」差异文件-----*/
            var addedFileList = _newFileList.Except(_oldFileList).ToDictionary(x => x.Key, x => x.Value);
            if (addedFileList.Count > 0)
            {
                foreach (var file in addedFileList)
                {
                    string localFilePath = file.Key;
                    string remoteFilePath = localFilePath.Replace(_basePath, "");
                    FtpClient.UploadFile(remoteFilePath, localFilePath);

                    Console.WriteLine($"{DateTime.Now} - upload file {remoteFilePath} OK.");
                }
            }
            
            _oldFolderList = new List<string>(_newFolderList);
            _oldFileList = new Dictionary<string, string>(_newFileList);

            _workDown = true;

            Console.WriteLine("All Work down!!!");
        }

        /// <summary>
        /// 获取子文件夹路径
        /// </summary>
        /// <param name="folderPath"></param>
        static List<string> GetSubFolders(string folderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            if (!dir.Exists)
            {
                return null;
            }

            var itemFiles = dir.GetDirectories().Select(x => x.FullName);
            foreach (var item in itemFiles)
            {
                _newFolderList.Add(item);
            }

            //遍历子目录
            var currentDirSubDirs = dir.GetDirectories().ToList();
            currentDirSubDirs.ForEach(p => GetSubFolders(p.FullName));

            return _newFolderList;
        }

        /// <summary>
        /// 获取文件路径,md5
        /// </summary>
        /// <param name="folderPath"></param>
        static Dictionary<string, string> GetAllFiles(string folderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            if (!dir.Exists)
            {
                return null;
            }

            var itemFiles = dir.GetFiles().Where(x => x.Name != ".DS_Store").Select(x => x.FullName);
            foreach (var item in itemFiles)
            {
                string fileMd5 = GetMD5Hash(item);

                _newFileList.Add(item, fileMd5);
            }

            //遍历子目录
            var currentDirSubDirs = dir.GetDirectories().ToList();
            currentDirSubDirs.ForEach(p => GetAllFiles(p.FullName));

            return _newFileList;
        }

        /// <summary>
        /// 获取文件的MD5码
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        private static string GetMD5Hash(string filePath)
        {
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5Hash() fail, error:" + ex.Message);
            }
        }
    }
}