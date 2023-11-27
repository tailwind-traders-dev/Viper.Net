## Viper.NET: A Simple Wrapper for IConfig

This project is inspired by [The Viper project from Go land](https://github.com/spf13/viper), but it's a bit lighter in terms of what it does and, hopefully, allows you to work with environment variables in a more straightforward way. Eventually we're hoping to wrap Azure Key Vault and GitHub secrets too.

## Using Viper

Viper is simply a wrapper for the `ConfigurationBuilder` in .NET. It has a convenience method, `Get()`, that will do its best to find the thing you're looking for, whether its in your app's settings, secrets, or the environment.

You can also access `Settings` directly if you need. But there's more!

### Adding some convention

If you're coming from places other than .NET land, you might be used to storing your configuration in a `/configuration` directory using something like `development.json`, `product.json`, or `test.json`. You might also expect certain runtime environment variables to be set for you by the runtime itself.

.NET doesn't work this way. If you want a runtime environment variable set, you need to do it yourself. For instance:  `ASPNETCORE_ENVIRONMENT` or `DOTNETCORE_ENVIRONMENT` are conventional, but not enforced by .NET. This is primarily because environment variables on Windows can be tricky business.

If you're using .NET Core on Linux, however, environment variables are the way to go. Viper lets you embrace these things with (hopefully) very little work on your part.

## Secrets vs. Settings

It's important to understand that secrets **should never be checked into source control!**. If you add connection strings or passwords to a file ending in `.json` you might be getting yourself into trouble.

.NET Core gives you access to `dotnet secrets`, which is a key/value store that stores secrets for you. You can use that directly if you like, and Viper will work just fine because `IConfig` is the backing store for everything.

If, however, you want to use environment variables, you can do that too! You can use a `.env` file in the root of your project (or in the `/config`) directory and off you go:

```sh
DATABASE_URL="postgres://me:password@localhost/my-db"
```

If that exists in a .env file in your project root, Viper will find it and load it for you and you can access it immediately using `Viper.Get("DATABASE_URL")` once you load things up:

```csharp
var _viper = Viper.Config(); //defaults to current environment from ASPNETCORE_ENVIRONMENT or DOTNETCORE_ENVIRONMENT
var setting = _viper.Get("DATABASE_URL");
```

When Viper loads up, it sets an internal settings dictionary to the keys and values it finds in the `.env` file as well as any JSON file you have on disk. To get those, you use `Get(key)` and off you go.

## Development, Production, Test, Staging

You can force the loading of a given environment by using its factory method:

```csharp
var _viper = Viper.Production();
_viper.Get("DATABASE_URL"); //careful now!
```

This is helpful for testing purposes. You can also use an overload:

```csharp
var _viper = Viper.Config("production");
_viper.Get("DATABASE_URL"); //careful now!
```

## Setting Defaults

Sometimes it's useful to set defaults for configuration, with a CLI, for instance. You can do this with the following override:

```csharp
_viper = Viper.Config(new Dictionary<string, string>(){
  {"one", "first"},
  {"two", "second"},
  {"six", "overwrite me"}
});
```

Here we're making sure the default values exist before loading the rest of the config from `.env` and `development.json`, which is the default configuration. You can do all of this manually, if you want, for a given environment:

```csharp
_viper = Viper.Config("test", new Dictionary<string, string>(){
  {"one", "first"},
  {"two", "second"},
  {"six", "overwrite me"}
});
```

## Key Vault Integration

I'm working on this right now, but it's slow and more than a bit buggy. Hopefully will have a way to read these values in sometime soon!


## Questions?

I know most of .NET does things using what's provided from the platform. The rest of the world, however, uses Environment variables and config files. Hopefully this can help you and your projects!

Always happy for a PR if you want.