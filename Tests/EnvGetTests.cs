using Xunit;

public class EnvGetTests{
  Viper _viper;
  public EnvGetTests()
  {
    _viper = new Viper();
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