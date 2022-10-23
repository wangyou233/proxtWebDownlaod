using Microsoft.AspNetCore.SignalR;
using WebDownload.App.Dtos.Models.Hub;

namespace WebDownload.App.Hubs;

public class DownloadNotifyHub : Hub<IDownloadNotifyClient>
{


    public async Task Subscribe(string id)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, id);
    }
    
    public async override Task OnConnectedAsync()
    {
        // 客户端链接时创建将当前链接与用户sessionId关联
        await base.OnConnectedAsync();
    }

    public async override Task OnDisconnectedAsync(Exception exception)
    {
        // 客户端断开时，删除关联
        await base.OnDisconnectedAsync(exception);
    }
}

public interface IDownloadNotifyClient
{
    /// <summary>
    /// 文件下载进度条
    /// </summary>
    /// <param name="vo"></param>
    /// <returns></returns>
    Task DownloadFileStatus(DownloadFileStatusVo vo);

    /// <summary>
    /// 文件完成提示并传输url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    Task DownloadFileCompletion(DownloadFileCompletionVo vo);
}