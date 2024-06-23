using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fountain.WinConsole.FTPDemo
{
    internal class Program
    {
        static  void Main(string[] args)
        {
            FtpClient fTPClient = new FtpClient("127.0.0.1","21", "test", "123456");
            List<string> result = fTPClient.GetDirectorys("");
            // 创建FTP目录
            if (!result.Contains("Video"))
            {
                fTPClient.CreateDirectory("Video");
            }
            string videoSubdir = $"{DateTime.Now:yyyyMMdd}";
            result = fTPClient.GetDirectorys("Video/");
            if (!result.Contains(videoSubdir))
            {
                //// 按日期创建子目录 
                fTPClient.CreateDirectory($"Video/{videoSubdir}");
            }
            string remotePath = $"Video/{DateTime.Now:yyyyMMdd}";
            // 文件名
            string remoteFilename = "20240501233402.mp4";
            // 上传文件
            fTPClient.Upload($"{remotePath}/{remoteFilename}", @"C:\Users\fountyuan\Desktop\20240501233402.mp4");
            // 下载文件
            fTPClient.Download($"{remotePath}/{remoteFilename}", @"C:\Users\fountyuan\Desktop\download", "20240501233402.mp4");

        }
    }
}
