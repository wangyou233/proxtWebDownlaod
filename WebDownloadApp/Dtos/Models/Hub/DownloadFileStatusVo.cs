namespace WebDownload.App.Dtos.Models.Hub;

public class DownloadFileStatusVo
{
    
    public int Progress { get; set; }
    
    public float TotalDownloadSize { get; set; }
    
    public float TotalSize { get; set; }
    
    public float Speed { get; set; }
}