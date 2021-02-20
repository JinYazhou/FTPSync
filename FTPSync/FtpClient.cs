using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace FTPSync
{
    public class FtpClient
    {
        public static string _hostAddress = "ftp://127.0.0.1";
        public static string _username = "jyz";
        public static string _password = "jyz";
        private static FtpWebRequest _ftpRequest = null;
        private static FtpWebResponse _ftpResponse = null;

        //// Constructor
        //public FtpClient(string hostAddress, string username, string password)
        //{
        //    _hostAddress = hostAddress;
        //    _username = username;
        //    _password = password;
        //}

        /// <summary>
        /// Download File
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <param name="localFile"></param>
        public static void DownloadFile(string remoteFile, string localFile)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_username, _password);
                    client.DownloadFile((_hostAddress + "/" + remoteFile).Trim(), localFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Upload File
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <param name="localFile"></param>
        public static void UploadFile(string remoteFile, string localFile)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_username, _password);
                    client.UploadFile((_hostAddress + "/" + remoteFile).Trim(), localFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Delete File
        /// </summary>
        /// <param name="deleteFile"></param>
        public static string DeleteFile(string deleteFile)
        {
            try
            {
                // Create an FTP Request
                _ftpRequest = (FtpWebRequest)WebRequest.Create(_hostAddress + "/" + deleteFile);
                // Log in to the FTP Server with the User Name and Password Provided
                _ftpRequest.Credentials = new NetworkCredential(_username, _password);
                // Specify the Type of FTP Request
                _ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                // Establish Return Communication with the FTP Server
                using (_ftpResponse = (FtpWebResponse)_ftpRequest.GetResponse())
                {
                    return _ftpResponse.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Delete Dir
        /// </summary>
        /// <param name="directoryPath"></param>
        public static string DeleteDir(string directoryPath)
        {
            try
            {
                // Create an FTP Request
                _ftpRequest = (FtpWebRequest)WebRequest.Create(_hostAddress + "/" + directoryPath);
                // Log in to the FTP Server with the User Name and Password Provided
                _ftpRequest.Credentials = new NetworkCredential(_username, _password);
                // Specify the Type of FTP Request
                _ftpRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                // Establish Return Communication with the FTP Server
                using (_ftpResponse = (FtpWebResponse)_ftpRequest.GetResponse())
                {
                    return _ftpResponse.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Rename File
        /// </summary>
        /// <param name="currentFileNameAndPath"></param>
        /// <param name="newFileName"></param>
        public static string RenameFile(string currentFileNameAndPath, string newFileName)
        {
            try
            {
                // Create an FTP Request
                _ftpRequest = (FtpWebRequest)WebRequest.Create(_hostAddress + "/" + currentFileNameAndPath);
                // Log in to the FTP Server with the User Name and Password Provided
                _ftpRequest.Credentials = new NetworkCredential(_username, _password);
                // Specify the Type of FTP Request
                _ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                // Rename the File
                _ftpRequest.RenameTo = newFileName;
                // Establish Return Communication with the FTP Server
                using (_ftpResponse = (FtpWebResponse)_ftpRequest.GetResponse())
                {
                    return _ftpResponse.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Create a New Directory on the FTP Server
        /// </summary>
        /// <param name="newDirectory"></param>
        public static string CreateDir(string newDirectory)
        {
            try
            {
                // Create an FTP Request
                _ftpRequest = (FtpWebRequest)WebRequest.Create(_hostAddress + "/" + newDirectory);
                // Log in to the FTP Server with the User Name and Password Provided
                _ftpRequest.Credentials = new NetworkCredential(_username, _password);
                // Specify the Type of FTP Request
                _ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                // Establish Return Communication with the FTP Server
                using (_ftpResponse = (FtpWebResponse)_ftpRequest.GetResponse())
                {
                    return _ftpResponse.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Get the Date/Time a File was Created
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DateTime GetFileCreatedDateTime(string fileName)
        {
            try
            {
                // Create an FTP Request
                _ftpRequest = (FtpWebRequest)WebRequest.Create(_hostAddress + "/" + fileName);

                // Log in to the FTP Server with the User Name and Password Provided
                _ftpRequest.Credentials = new NetworkCredential(_username, _password);

                // Specify the Type of FTP Request
                _ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;

                // Establish Return Communication with the FTP Server
                using (_ftpResponse = (FtpWebResponse)_ftpRequest.GetResponse())
                {
                    return _ftpResponse.LastModified;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Get the Size of a File
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static long GetFileSize(string fileName)
        {
            try
            {
                // Create an FTP Request
                _ftpRequest = (FtpWebRequest)WebRequest.Create(_hostAddress + "/" + fileName);

                // Log in to the FTP Server with the User Name and Password Provided
                _ftpRequest.Credentials = new NetworkCredential(_username, _password);

                // Specify the Type of FTP Request
                _ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;

                // Establish Return Communication with the FTP Server
                using (_ftpResponse = (FtpWebResponse)_ftpRequest.GetResponse())
                {
                    return _ftpResponse.ContentLength;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// List Directory Contents File/Folder Name Only
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static IList<string> DirectoryListSimple(string directory)
        {
            try
            {
                // Create an FTP Request
                _ftpRequest = (FtpWebRequest)WebRequest.Create(_hostAddress + "/" + directory);

                // Log in to the FTP Server with the User Name and Password Provided
                _ftpRequest.Credentials = new NetworkCredential(_username, _password);

                // Specify the Type of FTP Request
                _ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

                // Establish Return Communication with the FTP Server
                using (_ftpResponse = (FtpWebResponse)_ftpRequest.GetResponse())
                {
                    StreamReader streamReader = new StreamReader(_ftpResponse.GetResponseStream());

                    IList<string> directories = new List<string>();

                    string line = streamReader.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        directories.Add(line);
                        line = streamReader.ReadLine();
                    }

                    return directories;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// List Directory Contents in Detail (Name, Size, Created, etc.)
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static IList<string> DirectoryListDetailed(string directory)
        {
            try
            {
                // Create an FTP Request
                _ftpRequest = (FtpWebRequest)WebRequest.Create(_hostAddress + "/" + directory);

                // Log in to the FTP Server with the User Name and Password Provided
                _ftpRequest.Credentials = new NetworkCredential(_username, _password);

                // Specify the Type of FTP Request
                _ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                // Establish Return Communication with the FTP Server
                using (_ftpResponse = (FtpWebResponse)_ftpRequest.GetResponse())
                {
                    StreamReader streamReader = new StreamReader(_ftpResponse.GetResponseStream());

                    IList<string> directories = new List<string>();

                    string line = streamReader.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        directories.Add(line);
                        line = streamReader.ReadLine();
                    }
                    return directories;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}