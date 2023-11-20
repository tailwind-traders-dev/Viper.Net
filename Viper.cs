//a configuration helper that's modeled after 
//https://github.com/spf13/viper
using System;  
using System.Configuration; 
using System.Text.Json;
using System.Text.Json.Serialization;

public class Viper {
  Dictionary<string,string> _config = new Dictionary<string,string>();
  public Viper():this(null)
  {
    
  }
  public Viper(Dictionary<string,string> defaults)
  {
    if(defaults != null){
      foreach(var kvp in defaults){
        _config.Add(kvp.Key, kvp.Value);
      }
    }
    //defaults get trampled by ENV
    this.LoadEnvFile();
    var runtimeEnv = this.GetEnvironment();
    this.LoadJson($"{runtimeEnv}.json");
  }
  public string GetEnvironment(){
    var runtimeEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    if(runtimeEnv == null) runtimeEnv =Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");
    if(runtimeEnv == null) runtimeEnv = "Development";
    return runtimeEnv;
  }
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

  public void Set(string key, string value){
    Environment.SetEnvironmentVariable(key, value);
    if(this._config.ContainsKey(key)){
      this._config[key] = value.Replace("\"", "");
    }else{
      this._config.Add(key, value.Replace("\"", ""));
    }
  }
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