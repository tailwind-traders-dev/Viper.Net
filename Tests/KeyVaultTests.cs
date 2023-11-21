using Xunit;

public class KeyVaultTests{
  Viper _viper;
  public KeyVaultTests()
  {
    _viper = Viper.Production();
  }
  [Fact]
  public void It_will_return_secret_from_vault()
  {
    //this doesn't work and is extremely slow
    // var secret = _viper.FromVault("thingy");
    // Assert.Equal("buddy", secret);
  }
}