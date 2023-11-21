## Viper.NET: Making Environment Variables Easier in .NET

This project is inspired by [The Viper project from Go land](https://github.com/spf13/viper), but it's a bit lighter in terms of what it does. 

## Isn't This a Solved Problem?

In some ways, yes. The [Microsoft Configuration docs](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration) walk you through a number of ways you can store configuration settings on disk or in memory. If you're a .NET person, this is probably easy for you to go over and use.

I'm coming back to .NET after a decade away in other realms (Node, Ruby, Go) and I find that managing secrets and settings in those platforms to be quite straightforward. Config "stuff" is typically handled by a dedicated class that works off some conventions.

If you prefer to use a little convention, store your settings on disk and your secrets in the Environment, this library could be helpful for you.

## Using Viper

The simplest thing to do is to create a `/config` directory and then `development.json`. Name it whatever you want - it should correspond to the environment that you're in, which in the .NET world is either `ASPNETCORE_ENVIRONMENT` or `DOTNETCORE_ENVIRONMENT`.

If you want to load secrets, use a `.env` file in the root of your project (or in the `/config`) directory and off you go.

```csharp
var _viper = Viper.Config(); //defaults to current environment from ASPNETCORE_ENVIRONMENT or DOTNETCORE_ENVIRONMENT
var setting = _viper.Get("SOME_KEY");
```

When Viper loads up, it sets an internal settings dictionary to the keys and values it finds in the `.env` file as well as any JSON file you have on disk. To get those, you use `Get(key)` and off you go.

## Setting Things

You can also override whatever Viper has set by using `Set(key, val)`:

```csharp
var _viper = Viper.Config(); //defaults to current environment from ASPNETCORE_ENVIRONMENT or DOTNETCORE_ENVIRONMENT
_viper.Set("SOME_KEY", "my value");
```

The `Set()` method will also set the environment variable on the system.

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