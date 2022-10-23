using WebDownloadApp.Dtos.Models.Api;

namespace WebDownload.App.Services;

public interface IDownloadFileImpl
{
    Task DownloadFileAsync(DownloadFileQo qo,string sessionId);
}