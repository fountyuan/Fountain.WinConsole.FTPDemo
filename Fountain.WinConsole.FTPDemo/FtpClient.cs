using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Fountain.WinConsole.FTPDemo
{
    public class FtpClient
    {
        /// <summary>
        /// 服务器
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        private string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        private string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private string Port { get; set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="hostIP"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public FtpClient(string host, string port, string userName, string password)
        {
            Host = host;
            Port = port;
            UserName = userName;
            Password = password;
        }
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="remoteFileName">服务器文件</param>
        /// <param name="localPath">本地文件目录</param>
        /// <param name="localFileName">本地文件</param>
        public void Download(string remoteFileName, string localPath, string localFileName)
        {
            try
            {
                if (Directory.Exists(localPath) == false)
                {
                    Directory.CreateDirectory(localPath);
                }
                int bufferSize = 2048;

                string localFullFilename = $"{localPath}{Path.DirectorySeparatorChar}{localFileName}";
                // 
                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(this.Host + "/" + remoteFileName);
                // 提供FTP 登录信息
                ftpWebRequest.Credentials = new NetworkCredential(this.UserName, this.Password);
                // 指定文件传输的数据类型
                ftpWebRequest.UseBinary = true;
                // 客设置户端应用程序的数据传输过程的行为
                ftpWebRequest.UsePassive = true;
                // 指定请求完成之后是否关闭连接
                ftpWebRequest.KeepAlive = true;
                // 请求下载文件方法 
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                // 与FTP服务器建立返回通信
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                // 获取FTP服务器的响应流

                using (Stream fileStream = ftpWebResponse.GetResponseStream())
                {
                    // 创建读写文件流
                    using (FileStream localFileStream = new FileStream(localFullFilename, FileMode.Create))
                    {
                        // 下载数据的缓冲区
                        byte[] fileBuffer = new byte[bufferSize];
                        // 对取数据流
                        int bytesRead = fileStream.Read(fileBuffer, 0, bufferSize);
                        // 通过写入缓冲数据下载文件，直到传输完成
                        while (bytesRead > 0)
                        {
                            localFileStream.Write(fileBuffer, 0, bytesRead);
                            bytesRead = fileStream.Read(fileBuffer, 0, bufferSize);
                        }
                    }
                }
                ftpWebResponse.Close();
                ftpWebRequest.Abort(); ;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="directory"></param>
        public void CreateDirectory(string directory)
        {
            try
            {
                string uri = $"{this.Host}:{this.Port}/{directory}";
                // 创建 FTP 请求
                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                // 提供FTP 登录信息
                ftpWebRequest.Credentials = new NetworkCredential(this.UserName, this.Password);
                // 指定文件传输的数据类型
                ftpWebRequest.UseBinary = true;
                // 客设置户端应用程序的数据传输过程的行为
                ftpWebRequest.UsePassive = true;
                // 指定请求完成之后是否关闭连接
                ftpWebRequest.KeepAlive = true;
                // 请求上传文件方法
                ftpWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                // 
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                ftpWebResponse.Close();
                ftpWebRequest.Abort();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetDirectorys(string relatePath)
        {
            List<string> result = new List<string>();
            try
            {
                FtpWebRequest ftpWebRequest = null;
                string uri = string.Format("ftp://{0}:{1}{2}", this.Host, this.Port, relatePath);
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.UsePassive = true;
                ftpWebRequest.Credentials = new NetworkCredential(this.UserName, this.Password);
                ftpWebRequest.KeepAlive = false;
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                WebResponse objWebResponse = ftpWebRequest.GetResponse();

                StreamReader objReader = new StreamReader(objWebResponse.GetResponseStream());
                string objLine = objReader.ReadLine();
                while (objLine != null)
                {

                    if (objLine.Trim().Contains("<DIR>"))
                    {
                        result.Add(objLine.Substring(39).Trim());
                    }
                    else
                    {
                        if (objLine.Trim().Substring(0, 1).ToUpper() == "D")
                        {
                            result.Add(objLine.Substring(55).Trim());
                        }
                    }
                    objLine = objReader.ReadLine();
                }
                objReader.Close();
                objWebResponse.Close();
                return result;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="remoteFileName">上传服务器后储存的文件</param>
        /// <param name="localFileName">上传的本地文件</param>
        public void Upload(string remoteFileName, string localFileName)
        {
            try
            {
                if (!File.Exists(localFileName))
                {
                    throw new Exception($"{localFileName}文件不存在！");
                }
                int bufferSize = 2048;
                // 
                string uri = $"{this.Host}:{this.Port}/{remoteFileName}";
                // 创建 FTP 请求
                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                // 提供FTP 登录信息
                ftpWebRequest.Credentials = new NetworkCredential(this.UserName, this.Password);
                // 指定文件传输的数据类型
                ftpWebRequest.UseBinary = true;
                // 客设置户端应用程序的数据传输过程的行为
                ftpWebRequest.UsePassive = true;
                // 指定请求完成之后是否关闭连接
                ftpWebRequest.KeepAlive = true;
                // 请求上传文件方法
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                // 上传数据至FTP服务器的数据流
                using (Stream fileStream = ftpWebRequest.GetRequestStream())
                {
                    // 读取本地上传文件的数据流
                    using (FileStream localFileStream = new FileStream(localFileName, FileMode.Open))
                    {
                        // 上传数据的缓冲区
                        byte[] byteBuffer = new byte[bufferSize];
                        // 读取数据流
                        int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                        // 通过写入缓冲数据上传文件，直到传输完成
                        while (bytesSent != 0)
                        {
                            fileStream.Write(byteBuffer, 0, bytesSent);
                            bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                        }
                    }
                }
                ftpWebRequest.Abort();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}