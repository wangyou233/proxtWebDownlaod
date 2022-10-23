namespace WebDownload.App.Dtos.Models.Hub;

public class DownloadFileCompletionVo
{
    public bool IsSuccess { get; set; }
    
    public string DownloadPath { get; set; }
    
    public string DateTime { get; set; }
    public string Msg { get; set; }
}