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
    //this is extremely slow and relies on azure.sh being run, which will set a secret in your vault
    //var secret = _viper.FromVault("thingy");
    //Assert.Equal("buddy", secret);
  }
}