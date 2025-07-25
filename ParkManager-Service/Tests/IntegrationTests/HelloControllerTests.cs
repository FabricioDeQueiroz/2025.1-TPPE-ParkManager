using ParkManager_Service.Controllers;

namespace ParkManager_Service.Tests.IntegrationTests;

public class HelloControllerTests
{
    private readonly HelloController _controller = new();

    [Fact(DisplayName = "Hello World")]
    public void GetReturnsExpectedMessage()
    {
        var result = _controller.Get();
        Assert.Equal("ParkManager API!", result);
    }
}
