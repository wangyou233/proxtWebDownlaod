using System.ComponentModel.DataAnnotations;

namespace WebDownloadApp.Dtos.Models.Api;

public class DownloadFileQo
{
    /// <summary>
    /// 是否使用代理
    /// </summary>
    public bool IsProxy { get; set; } = false;
    
    /// <summary>
    /// 下载地址
    /// </summary>
    public string DownloadFileUrl { get; set; }
}