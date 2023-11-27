//a configuration helper that's modeled after 
//https://github.com/spf13/viper
using System;
using System.Reflection;
using System.Text.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

public class Viper {
  //Dictionary<string,string> _config = new Dictionary<string,string>();
  
  //IConfiguration _config;
  public IConfiguration Settings { get; set; }
  public Dictionary<string,string> Secrets { get; set; } = new Dictionary<string,string>();
  public string Env { get; set; } = "development";
  //force the use of factory methods
  private Viper(){}

  static void SentEnv(string env = "development"){
    //Console.WriteLine("Setting environment to " + env);
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", env);
    Environment.SetEnvironmentVariable("DOTNETCORE_ENVIRONMENT", env);
  }
  /// <summary>
  /// Returns the development configuration
  /// </summary>
  /// <returns>Viper instance</returns>
  public static Viper Development(){
    SentEnv("development");
    return Config("development");
  }
  /// <summary>
  /// Returns the production configuration
  /// </summary>
  /// <returns>Viper instance</returns>
  public static Viper Production(){
    SentEnv("production");
    return Config("production");
  }
  /// <summary>
  /// Returns the staging configuration
  /// </summary>
  /// <returns>Viper instance</returns>
  public static Viper Staging(){
    SentEnv("staging");
    return Config("staging");
  }
  
  /// <summary>
  /// Returns the test configuration
  /// </summary>
  /// <returns>Viper instance</returns>
  public static Viper Test(){
    SentEnv("test");
    return Config("test");
  }

  /// <summary>
  /// Returns the current environment using ASPNETCORE_ENVIRONMENT or DOTNETCORE_ENVIRONMENT. Defaults to Development.
  /// </summary>
  /// <returns></returns>
  public static string GetEnvironment(){
    var runtimeEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    if(String.IsNullOrWhiteSpace(runtimeEnv)) runtimeEnv = Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");
    if(String.IsNullOrWhiteSpace(runtimeEnv)) runtimeEnv = "development";
    return runtimeEnv;
  }

  /// <summary>
  /// Defaults to whatever is set in the environment. If nothing is set, it defaults to Development.
  /// </summary>
  /// <returns></returns>
  public static Viper Config(){
    var env = Viper.GetEnvironment();
    return Config(env);
  }

  /// <summary>
  /// Defaults to whatever is set in the environment or the defaults you pass in. If nothing is set, it defaults to Development.
  /// </summary>
  /// <param name="defaults"></param>
  /// <returns></returns>
  public static Viper Config(IEnumerable<KeyValuePair<string,string?>> defaults){
    var env = Viper.GetEnvironment();
    return Config(env, defaults);
  }

  /// <summary>
  /// Read the configuration from the environment or any JSON file with the name of the environment. You can put the JSON file in the root of the project, in a config directory, or in the current directory.
  /// </summary>
  /// <param name="env">Name of environment you want to load</param>
  /// <returns>Viper instance</returns>
  public static Viper Config(string env ){
    SentEnv(env);
    return Config(env, null);
  }


  /// <summary>
  /// Read the configuration from the environment or any JSON file with the name of the environment. You can put the JSON file in the root of the project, in a config directory, or in the current directory. You can also pass in a dictionary of default values.
  /// </summary>
  /// <param name="env">Name of environment you want to load</param>
  /// <param name="defaults">Dictionary of default values</param>
  /// <returns>Viper instance</returns>
  public static Viper Config(string env, IEnumerable<KeyValuePair<string,string?>> defaults){
    var viper = new Viper();
    viper.Env = env;
    var _builder = new ConfigurationBuilder();

    if(defaults != null) _builder.AddInMemoryCollection(defaults);

    var settingsFile = viper.FindConfigFile($"appsettings.json");
    var envSettings = viper.FindConfigFile($"{env}.json");
    var envAppSettings = viper.FindConfigFile($"appsettings.{env}.json");
    
    //add the appsettings.json file if it exists
    if(settingsFile != null) _builder.AddJsonFile(settingsFile);
    if(envSettings != null) _builder.AddJsonFile(envSettings);
    if(envAppSettings != null) _builder.AddJsonFile(envAppSettings);

    //add stuff to ENV
    viper.LoadEnvFile();

    //add any secrets on in ENV
    _builder.AddUserSecrets(Assembly.GetExecutingAssembly(),true);

    _builder.AddEnvironmentVariables();

    viper.Settings = _builder.Build();

    return viper;
  }


  /// <summary>
  /// Locates the specified configuration file in the current directory, the project root, or the config directory.
  /// </summary>
  /// <param name="name">The name of the config file, 'development.json', e.g.</param>
  /// <returns>File path, null if not found</returns>
  public string FindConfigFile(string name){
    var execDirectory = Directory.GetCurrentDirectory();
    //do we have an .ENV file here?
    var filePath = Path.Combine(execDirectory, name);
    if(File.Exists(filePath)) return filePath;

    //project root
    string projectDirectory = Directory.GetParent(execDirectory).Parent.Parent.FullName;
    filePath = Path.Combine(projectDirectory, name);
    if(File.Exists(filePath)) return filePath;

    //config directory
    filePath = Path.Combine(projectDirectory, "config", name);
    if(File.Exists(filePath)) return filePath;

    return null;
  }

  /// <summary>
  /// Reads in a .env file and loads the values into the configuration. Best to put the .env file in the root of the project.
  /// </summary>
  public void LoadEnvFile()
  {
    var filePath = FindConfigFile(".env");
    if(filePath == null){
        Console.WriteLine("No .env file found");
        return;
    }
    //Console.WriteLine("Loading .env file");
    foreach (var line in File.ReadAllLines(filePath))
    {

      if(line.IndexOf("#") == 0) continue; //it's a comment
      //get the first index of = and split on that; the values might have = in them so split won't work
      var idx = line.IndexOf('=');
      if(idx <= 0) continue; //empty line skip it
      //var parts = line.Split('=',StringSplitOptions.RemoveEmptyEntries);
      var key = line.Substring(0, idx);
      var val = line.Substring(idx + 1);
      this.Secrets.Add(key, val);
      Environment.SetEnvironmentVariable(key, val);
      //this.Set(key, val);
    }
  }

  /// <summary>
  /// Looks up a configuration value from whatever stores were loaded. If the value is not found, null is returned.
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  public string Get(string key){
    //look up values in the environment 
    //or appsettings.json
    //GitHub Secrets?
    //or Azure Key Vault
    // if(_config.ContainsKey(key)){
    //   return _config[key];
    // }
    if(this.Settings[key] != null){
      //strip off quotes which are added by the ENV reader for some reason
      return this.Settings[key].Replace("\"", "");
    }
    //look in the environment
    var envValue = Environment.GetEnvironmentVariable(key);
    if(!String.IsNullOrWhiteSpace(envValue)){
      return envValue;
    }
    return null;
  }

  public string? FromVault(string key){
    //we need to have a vault name in the environment
    //HACK: I don't like using a property here but there seems to be a weird race condition
    //with the set environment stuff
    if(this.Env != "production"){
      //TODO: do we want this guard here? I kind of think we do
      throw new InvalidOperationException("You can only use Azure Key Vault in production");
    }
    var vaultName= Get("KEY_VAULT_NAME");
    if(String.IsNullOrEmpty(vaultName)) throw new InvalidOperationException("You must set the KEY_VAULT_NAME in your config using .env or a settings file to use Azure Key Vault");

    var kvUri = $"https://{vaultName}.vault.azure.net";

    //HACK: how do we get the credentials in here?
    var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
    var value = client.GetSecret(key);
    if(value == null) return null;
    return value.ToString();
  }
}