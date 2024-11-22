using Microsoft.AspNetCore.Mvc;

public class ExampleController : ControllerBase
{
    public IActionResult GetAudio()
    {
        return PhysicalFile(@"D:\examplePath\example.wav", "application/octet-stream", "example.wav");
    }
    
    public IActionResult GetAudioPartial()
    {
        return PhysicalFile(@"D:\examplePath\example.wav", "application/octet-stream", "example.wav", true);
    }
}