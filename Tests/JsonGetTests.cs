// using Xunit;

// public class JsonGetTests{
//   Viper _viper;
//   public JsonGetTests()
//   {
//     _viper = Viper.Json("settings.json");
//   }
//   [Fact]
//   public void Reads_First_Setting_in_ENV_File(){
//     Assert.Equal("first", _viper.Get("one"));
//   }
//   [Fact]
//   public void Reads_Second_Setting_With_Equal_File(){
//     Assert.Equal("second", _viper.Get("two"));
//   }
//   [Fact]
//   public void Reads_Third_Setting_With_Line_Skip(){
//     Assert.Equal("third", _viper.Get("three"));
//   }
// }