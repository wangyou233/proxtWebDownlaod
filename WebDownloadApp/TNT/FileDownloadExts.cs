using System.Diagnostics;
using System.Net;
using WebDownload.App.Configs;

namespace WebDownload.App.TNT;

public class FileDownloadUtil
{
    private string url; //文件下载网络地址
    private string path; //文件下载位置，如d:/download
    private string fileId; //文件ID，文件唯一标识，一般为UUID

    private ProxyConfig proxyConfig;

    public delegate void ProgressChangedHandler(int progress, string fileId,float totalDownloadSize,float totalSize,float speed);
    public event ProgressChangedHandler ProgressChanged;
    protected virtual void OnProgressChanged(int progress, string fileId,float totalDownloadSize,float totalSize,float speed)
    {
        ProgressChanged?.Invoke(progress, fileId,totalDownloadSize,totalSize,speed);
    }
    public delegate void DownloadFinishHandler(bool isSuccess, string downloadPath, string fileId,TimeSpan dateTime, string msg = null);
    public event DownloadFinishHandler DownloadFinish;
    protected virtual void OnDownloadFinish(bool isSuccess, string downloadPath, string fileId,TimeSpan dateTime, string msg = null)
    {
        DownloadFinish?.Invoke(isSuccess, downloadPath, fileId, dateTime,msg);
    }

    //通过网络链接直接下载任意文件
    public FileDownloadUtil(string url, string path,string fileId,ProxyConfig proxyConfig)
    {
        this.url = url;
        this.path = path;
        this.fileId = fileId;
        this.proxyConfig = proxyConfig;
    }
    public void Download()
    {
        Download(url, path, fileId,proxyConfig);
    }
    private void Download(string url, string path, string fileId,ProxyConfig? proxyConfig = null)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        if (!Directory.Exists(path))   //判断文件夹是否存在
            Directory.CreateDirectory(path);
        Random random = new Random();
        var ext = Path.GetExtension(url);
        var fileName = $"{Path.GetFileName(url).Replace(ext,"")}.{random.Next(0000,9999)}{ext}";
        string tempFile = path + @"\" +fileName; //临时文件
        if (File.Exists(tempFile))
        {
            File.Delete(tempFile);    //存在则删除
        }
        if (File.Exists(path))
        {
            File.Delete(path);    //存在则删除
        } 
        FileStream fs = null;
        HttpWebRequest request = null;
        
        HttpWebResponse response = null;
        Stream responseStream = null;
       
        try
        {
            //创建临时文件
            fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            request = WebRequest.Create(url) as HttpWebRequest;
            if (proxyConfig != null)
            {
                var urlBuilder = new UriBuilder(proxyConfig.IP);
                urlBuilder.Port = proxyConfig.Port;

                ICredentials credentials = new NetworkCredential(proxyConfig.UserName, proxyConfig.PassWord);

                request.Proxy = new WebProxy(urlBuilder.Uri,true,null,credentials);
            }
            
            //发送请求并获取相应回应数据
            response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            responseStream = response.GetResponseStream();
            byte[] bArr = new byte[10240];
            long totalBytes = response.ContentLength; //通过响应头获取文件大小，前提是响应头有文件大小
            int size = responseStream.Read(bArr, 0, (int)bArr.Length); //读取响应流到bArr，读取大小
            float percent = 0;  //用来保存计算好的百分比
            long totalDownloadedByte = 0;  //总共下载字节数
            while (size > 0)  //while循环读取响应流
            {
                fs.Write(bArr, 0, size); //写到临时文件
                totalDownloadedByte += size; 
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
                percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                OnProgressChanged((int)percent, fileId,totalDownloadedByte,totalBytes,size);  //下载进度回调
            }
            if (File.Exists(path))
            {
                File.Delete(path);    //存在则删除
            }
            fs.Close();
            File.Move(tempFile, $"{path}/{fileName}" ); //重命名为正式文件
            sw.Stop();
            OnDownloadFinish(true, fileName, fileId, sw.Elapsed);  //下载完成，成功回调
        }
        catch (Exception ex)
        {
            sw.Stop();
            OnDownloadFinish(false, null, fileId,sw.Elapsed, ex.Message);  //下载完成，失败回调
        }
        finally
        {
            if (fs != null)
                fs.Close();
            if (request != null)
                request.Abort();
            if (response != null)
                response.Close();
            if (responseStream != null)
                responseStream.Close();
        }
    }
}