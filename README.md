# roundabound
(Incomplete) Port of [perliedman/roundabound](https://github.com/perliedman/roundabound) to dotnet core


It appears there is no simple and easy to understand solution for rotating
logs in Windows. **roundabound** attempts to fix this simple problem.

It will also work on UNIX, if you would want it for some reason.

Configuration is done in JSON:

```json
{
    "sets": {
        "logs": {
            "pattern": "C:\\logs\\*.log.????????",
            "archive_age": 7,
            "archive_path": "c:\\archive"
        },
        "logs2": {
            "pattern": "C:\\logs\\*.log????????",
            "archive_age": 14,
            "archive_path": "c:\\archive"
        }
    }
}
```

To run:

```
dotnet run roundabound.csproj 
```

or, on Windows 
```
roundabound.exe
```
on Unix:
```
roundabound
```

To build (use [RID Catalog](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) to find your runtime): 

Linux
```
dotnet build roundabound.csproj -c Release -r linux-x64
```

Windows 
```
dotnet build roundabound.csproj -c Release -r win-x64
```

macOS 
```
dotnet build roundabound.csproj -c Release -r osx-x64
```

```CONFIG``` default is set to ```roundabound.cfg``` 
