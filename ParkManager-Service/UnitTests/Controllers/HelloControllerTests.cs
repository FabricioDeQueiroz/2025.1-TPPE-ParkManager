using ParkManager_Service.Controllers;

namespace ParkManager_Service.UnitTests.Controllers;

public class HelloControllerTests
{
    private readonly HelloController _controller = new();

    [Fact(DisplayName = "Hello World")]
    public void Get_ReturnsExpectedMessage()
    {
        var result = _controller.Get();
        
        Assert.Equal("Olá Mundo!", result);
    }
    
    [Fact(DisplayName = "Ignorado 1", Skip = "Ignorado")]
    public void Get_Ignorado()
    {
        // Implementar depois
    }
}