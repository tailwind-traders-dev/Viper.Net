using Microsoft.Extensions.Configuration;
using Xunit;

public class AppSettingsTests {
  Viper _viper;
  public AppSettingsTests()
  {
    _viper = Viper.Config();
  }

  [Fact]
  public void Reads_Simple_AppSettings_Key(){
    var val = _viper.Get("Test");
    Assert.Equal("Success", val);
  }
  [Fact]
  public void Reads_Nested_AppSettings_Key(){
    var val = _viper.Get("Position:Title");
    Assert.Equal("Person", val);
  }
}