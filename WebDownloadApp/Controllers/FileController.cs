using Microsoft.AspNetCore.Mvc;
using WebDownload.App.TNT;

namespace WebDownload.App.Controllers;

[Route("file")]
public class FileController : ControllerBase
{
    [HttpGet("{fileName}")]
    public FileResult? DownloadFile(string fileName)
    {
        var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}wwwroot/download/{fileName}";
        if (!System.IO.File.Exists(filePath)) return null;

        var stream = System.IO.File.OpenRead(filePath);  //创建文件流
        var contentType = MIMEAssistant.GetMIMEType(fileName);  　　　　//获取文件类型

        return File(stream, contentType, fileName);

    }
}