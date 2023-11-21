//a configuration helper that's modeled after 
//https://github.com/spf13/viper
using System;  
using System.Text.Json;

public class Viper {
  Dictionary<string,string> _config = new Dictionary<string,string>();
  
  //force the use of factory methods
  private Viper(){}

  /// <summary>
  /// Returns the development configuration
  /// </summary>
  /// <returns>Viper instance</returns>
  public static Viper Development(){
    return Config("development");
  }
  /// <summary>
  /// Returns the production configuration
  /// </summary>
  /// <returns>Viper instance</returns>
  public static Viper Production(){
    return Config("production");
  }
  /// <summary>
  /// Returns the staging configuration
  /// </summary>
  /// <returns>Viper instance</returns>
  public static Viper Staging(){
    return Config("staging");
  }
  
  /// <summary>
  /// Returns the test configuration
  /// </summary>
  /// <returns>Viper instance</returns>
  public static Viper Test(){
    return Config("test");
  }

  /// <summary>
  /// Read the configuration from the environment or any JSON file with the name of the environment. You can put the JSON file in the root of the project, in a config directory, or in the current directory.
  /// </summary>
  /// <param name="env">Name of environment you want to load</param>
  /// <returns>Viper instance</returns>
  public static Viper Config(string env = "development"){
    return Config(env, null);
  }

  /// <summary>
  /// Loads only a JSON file. You can put the JSON file in the root of the project, in a config directory, or in the current directory.
  /// </summary>
  /// <param name="fileName"></param>
  /// <returns></returns>
  public static Viper Json(string fileName){
    var viper = new Viper();
    viper.LoadJson(fileName);
    return viper;
  }

  /// <summary>
  /// Read the configuration from the environment or any JSON file with the name of the environment. You can put the JSON file in the root of the project, in a config directory, or in the current directory. You can also pass in a dictionary of default values.
  /// </summary>
  /// <param name="env">Name of environment you want to load</param>
  /// <param name="defaults">Dictionary of default values</param>
  /// <returns>Viper instance</returns>
  public static Viper Config(string env, Dictionary<string,string> defaults){
    var viper = new Viper();

    if(defaults != null){
      foreach(var kvp in defaults){
        viper.Set(kvp.Key, kvp.Value);
      }
    }

    viper.LoadEnvFile();
    viper.LoadJson($"{env}.json");
    return viper;
  }

  /// <summary>
  /// Returns the current environment using ASPNETCORE_ENVIRONMENT or DOTNETCORE_ENVIRONMENT. Defaults to Development.
  /// </summary>
  /// <returns></returns>
  public string GetEnvironment(){
    var runtimeEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    if(runtimeEnv == null) runtimeEnv =Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");
    if(runtimeEnv == null) runtimeEnv = "development";
    return runtimeEnv;
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

  //load up a JSON file
  /// <summary>
  /// Reads in a JSON file and loads the values into the configuration.
  /// </summary>
  /// <param name="jsonFile">The name of the file to load</param>
  /// <exception cref="InvalidOperationException"></exception>
  public void LoadJson(string jsonFile)
  {
    string filePath = this.FindConfigFile(jsonFile);
    //find the appsettings file
    if(File.Exists(filePath)){
      //Console.WriteLine("Loading appsettings.json");
      var settings = File.ReadAllText(filePath);
      var deserialized = JsonSerializer.Deserialize<Dictionary<string,string>>(settings);
      foreach(var kvp in deserialized){
        
        this.Set(kvp.Key, kvp.Value);
      }
    }else{
      throw new InvalidOperationException($"No json file found {filePath}");
    }
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

      this.Set(key, val);
    }
  }

  /// <summary>
  /// Sets a configuration value in the environment as well as config settings internally. If the value already exists, it will be overwritten.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  public void Set(string key, string value){
    Environment.SetEnvironmentVariable(key, value);
    if(this._config.ContainsKey(key)){
      this._config[key] = value.Replace("\"", "");
    }else{
      this._config.Add(key, value.Replace("\"", ""));
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
    if(_config.ContainsKey(key)){
      return _config[key];
    }

    return null;
  }
}