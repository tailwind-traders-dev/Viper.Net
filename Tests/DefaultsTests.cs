using Xunit;

public class DefaultsTests{
  Viper _viper;
  public DefaultsTests()
  {
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "development");
    _viper = Viper.Config(new Dictionary<string, string>(){
      {"xx", "first"},
      {"yy", "second"},
      {"six", "overwrite"} //overwritten by development.json
    });
    //get the current directory
  }
  [Fact]
  public void Sets_the_first_default(){
    Assert.Equal("first", _viper.Get("xx"));
  }
  [Fact]
  public void Sets_the_second_default(){
    Assert.Equal("second", _viper.Get("yy"));
  }
  [Fact]
  public void Third_default_is_overriden_for_six(){
    Assert.Equal("sixth", _viper.Get("six"));
  }
}