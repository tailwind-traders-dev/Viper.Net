using Xunit;

public class EnvGetTests{
  Viper _viper;
  public EnvGetTests()
  {
    _viper = Viper.Config();
    Environment.SetEnvironmentVariable("zzzz", "TEST");
    
  }

  [Fact]
  public void Env_vals_can_be_set_and_read(){
    var val = Environment.GetEnvironmentVariable("zzzz");
    Assert.Equal("TEST", val);
  }
  [Fact]
  public void Env_vals_can_be_read_using_Get(){
    var val = _viper.Get("zzzz");
    Assert.Equal("TEST", val);
  }
  [Fact]
  public void Reads_First_Setting_in_ENV_File(){
    Assert.Equal("setting-one", _viper.Get("ONE"));
  }
  [Fact]
  public void Reads_Second_Setting_With_Equal_File(){
    Assert.Equal("setting=with=equals", _viper.Get("TWO"));
  }
  [Fact]
  public void Reads_Third_Setting_With_Line_Skip(){
    Assert.Equal("setting\\with/weird/chars", _viper.Get("THREE"));
  }
}