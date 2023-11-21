using Xunit;

public class ConfigDirectoryTests{
  Viper _viper;
  public ConfigDirectoryTests()
  {
    _viper = Viper.Config("development");
    //get the current directory
  }
  [Fact]
  public void Reads_First_Setting_in_ENV_File(){
    Assert.Equal("sixth", _viper.Get("six"));
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