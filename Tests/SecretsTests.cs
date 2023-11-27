using Microsoft.Extensions.Configuration;
using Xunit;

public class SecretsTests {
  Viper _viper;
  public SecretsTests()
  {
    _viper = Viper.Config();
  }

  [Fact]
  public void Reads_a_secret(){
    //this requires dotnet user-secrets set "Test:Key" "Test" to be run first
    //sorry for the derpy test just not sure how to get around that
    var val = _viper.Get("Test:Key");
    Assert.Equal("Test", val);
  }
}