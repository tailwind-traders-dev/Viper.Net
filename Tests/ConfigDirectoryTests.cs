using Xunit;

public class ConfigDirectoryTests{
  Viper _viper;
  public ConfigDirectoryTests()
  {
    _viper = new Viper();
    //get the current directory
    _viper.LoadJson("development.json");
  }
  [Fact]
  public void Reads_First_Setting_in_ENV_File(){
    Assert.Equal("third", _viper.Get("three"));
  }
  [Fact]
  public void Reads_Second_Setting_With_Equal_File(){
    Assert.Equal("fourth", _viper.Get("four"));
  }
  [Fact]
  public void Reads_Third_Setting_With_Line_Skip(){
    Assert.Equal("fifth", _viper.Get("five"));
  }
}