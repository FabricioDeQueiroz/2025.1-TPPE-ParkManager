using ParkManager_Service.Controllers;

namespace ParkManager_Service.Tests.UnitTests;

public class HelloControllerTests
{
    private readonly HelloController _controller = new();

    [Fact(DisplayName = "Hello World")]
    public void GetReturnsExpectedMessage()
    {
        var result = _controller.Get();
        Assert.Equal("ParkManager API!", result);
    }

    [Fact(DisplayName = "Ignorado 1", Skip = "Ignorado")]
    public void GetIgnorado()
    {
        // Implementar depois
    }
}
