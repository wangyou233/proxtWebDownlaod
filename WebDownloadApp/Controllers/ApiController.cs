using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebDownload.App.Hubs;
using WebDownload.App.Services;
using WebDownloadApp.Dtos.Models.Api;

namespace WebDownload.App.Controllers;

[Route("api")]
public class ApiController : BaseController
{
    private readonly IHubContext<DownloadNotifyHub, IDownloadNotifyClient> _hubContext;
    private readonly IDownloadFileImpl _downloadFileImpl;

    public ApiController(IHubContext<DownloadNotifyHub, IDownloadNotifyClient> hubContext,
        IDownloadFileImpl downloadFileImpl)
    {
        _hubContext = hubContext;
        _downloadFileImpl = downloadFileImpl;
    }

    [HttpPost("download")]
    public async Task<IActionResult> DownloadFileAsync([FromForm] DownloadFileQo qo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var id = Guid.NewGuid().ToString();

        await _downloadFileImpl.DownloadFileAsync(qo, id);
        return Ok(id);
    }


}