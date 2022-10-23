namespace WebDownload.App.Jobs;

public class DeleteExpiredFileJob : IHostedService, IDisposable
{
    private int executionCount = 0;
    private Timer? _timer = null;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        
        //小时执行  删除错误文件
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromHours(1));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var path = $"{AppDomain.CurrentDomain.BaseDirectory}/wwwroot/download";

        var files = Directory.GetFiles(path);
        foreach (var filePath in files)
        {
            var file = new FileInfo(filePath);
            if (file.CreationTime > file.CreationTime.AddHours(4))
            {
                File.Delete(filePath);
            }
        }

        var count = Interlocked.Increment(ref executionCount);
        Console.WriteLine(count);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}