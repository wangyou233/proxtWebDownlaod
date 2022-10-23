using Microsoft.AspNetCore.SignalR;
using WebDownload.App.Configs;
using WebDownload.App.Dtos.Models.Hub;
using WebDownload.App.Hubs;
using WebDownload.App.TNT;
using WebDownloadApp.Dtos.Models.Api;

namespace WebDownload.App.Services.ImpI;

public class DownloadFileImpl : IDownloadFileImpl
{
    private readonly IHubContext<DownloadNotifyHub, IDownloadNotifyClient> _hubContext;
    private readonly ProxyConfig _proxyConfig;
    private readonly IConfiguration _configuration;

    public DownloadFileImpl(IHubContext<DownloadNotifyHub, IDownloadNotifyClient> hubContext, ProxyConfig proxyConfig,IConfiguration configuration)
    {
        _hubContext = hubContext;
        _proxyConfig = proxyConfig;
        _configuration = configuration;
    }

    public async Task DownloadFileAsync(DownloadFileQo qo, string sessionId)
    {
        var savePath = $"{AppDomain.CurrentDomain.BaseDirectory}wwwroot/download";

        var file = new FileDownloadUtil(qo.DownloadFileUrl, savePath, sessionId, qo.IsProxy ? _proxyConfig : null);
        file.ProgressChanged += File_ProgressChanged;
        file.DownloadFinish += File_DownloadFinish;
        Thread childThread = new Thread(file.Download);
        childThread.Start();
    }

    private void File_DownloadFinish(bool isSuccess, string downloadPath, string fileId, TimeSpan dateTime,
        string msg = null)
    {
        if (isSuccess)
        {
            Console.WriteLine("下载完成");

            
            
            _hubContext.Clients.Group(fileId).DownloadFileCompletion(new DownloadFileCompletionVo()
            {
                IsSuccess = isSuccess,
                DownloadPath = $"{_configuration.GetSection("Host").Get<string>()}file/{downloadPath}",
                DateTime = $"{dateTime.Hours}:{dateTime.Minutes}:{dateTime.Seconds}",
                Msg = ""
            });
        }
        else
        {
            _hubContext.Clients.Group(fileId).DownloadFileCompletion(new DownloadFileCompletionVo()
            {
                IsSuccess = isSuccess,
                Msg = msg
            });
            Console.WriteLine("下载失败");
        }
    }

    private void File_ProgressChanged(int progress, string fileId, float totalDownloadSize, float totalSize,
        float speed)
    {
        _hubContext.Clients.Group(fileId).DownloadFileStatus(new DownloadFileStatusVo()
        {
            Progress = progress,
            TotalSize = totalSize,
            Speed = speed,
            TotalDownloadSize = totalDownloadSize
        });
    }
}