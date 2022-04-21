using System.IO;

namespace NetProvider.Network
{
    /// <summary>
    /// 文件
    /// </summary>
    public class FileTransferData
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="contentName"></param>
        /// <param name="contentType"></param>
        public FileTransferData(string filePath,string contentName=null,string contentType= "application/octet-stream")
        {
            UploadFile = File.OpenRead(filePath);
            this.ContentType = contentType;
            if (contentName == null)
            {
                ContentName = Path.GetFileName(filePath);
            }
            this.FileName= Path.GetFileName(filePath);
        }
        /// <summary>
        /// 自定义文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 内容名
        /// </summary>
        public string ContentName { get;private set; }
        /// <summary>
        /// 上传的文件
        /// </summary>
        public Stream UploadFile { get;private set; }
        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get;private set; }
    }
}
