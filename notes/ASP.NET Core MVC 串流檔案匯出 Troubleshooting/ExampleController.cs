using Microsoft.AspNetCore.Mvc;

public class ExampleController : ControllerBase
{
    public IActionResult DownloadWrong()
    {
        var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, leaveOpen: true);
        writer.WriteLine("content 1");
        writer.WriteLine("content 2");
        writer.Flush();
        return File(stream, "application/octet-stream", "example.txt");
    }

    public IActionResult DownloadCorrect()
    {
        var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, leaveOpen: true);
        writer.WriteLine("content 1");
        writer.WriteLine("content 2");
        writer.Flush();
        stream.Position = 0;
        return File(stream, "application/octet-stream", "example.txt");
    }
}