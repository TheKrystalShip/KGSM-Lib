# KGSM.Lib

KGSM-Lib is a C# library designed to interact with [KGSM][1], a lightweight 
game server manager for Linux. This library simplifies integration with KGSM 
by providing an intuitive API to manage game servers, blueprints, and 
instances, as well as listening to real-time events through a Unix Domain 
Socket.

## Features

- Event Handling:
    Subscribe to real-time events emitted by KGSM for actions such as server 
    installations, startups, shutdowns, and more using KgsmEvents.

- Blueprint Management:
    Create, list, and manage server blueprints easily through simple method 
    calls.

- Instance Management:
    Install, start, stop, restart, update, and manage game server instances 
    using a unified API.

- Ad-Hoc Commands:
    Execute custom commands that aren't explicitly mapped within the library.


## Installation

You can install KGSM-Lib via NuGet:

```sh
dotnet add package TheKrystalShip.KGSM.Lib
```

Or using the NuGet package manager

```sh
Install-Package TheKrystalShip.KGSM.Lib
```

## Usage example

- Listening for events
```c#
using TheKrystalShip.KGSM.Lib;

KgsmInterop interop = new("path/to/kgsm.sh", "/path/to/kgsm.sock");

interop.Events.RegisterHandler<InstanceInstalledData>(async (data) => {
    Console.WriteLine($"New instance installed: {data.InstanceId}");
});

```

## Blueprints
```c#
// List all blueprints
using TheKrystalShip.KGSM.Lib;

var interop = new KgsmInterop("/path/to/kgsm.sh", "/path/to/kgsm.sock");

Dictionary<string, Blueprint> blueprints = interop.GetBlueprints();
foreach (var blueprint in blueprints)
{
    Console.WriteLine($"{blueprint.Key}: {blueprint.Value}");
}

// Create a new blueprint
Blueprint blueprint = new Blueprint
{
    Name = "MyGameServer",
    Port = 27015,
    LaunchBin = "server_bin",
    SteamAppId = 123456,
    SteamAccountRequired = true
};

interop.CreateBlueprint(blueprint);
```

## Manage instances

```c#
// Install a new instance
interop.Install("MyBlueprint", "/install/path");

// Start an instance
interop.Start("MyInstance");

// Get logs
KgsmResult logs = interop.GetLogs("MyInstance");
Console.WriteLine(logs.Stdout);

// Create a backup
interop.CreateBackup("MyInstance");
```

## Ad-Hoc commands

```c#
// Execute a custom command
interop.AdHoc("--custom-command", "argument");
```

## License
This project is licensed under the MIT license. See the [LICENSE][2] file for 
details.

[1]: https://github.com/TheKrystalShip/KGSM
[2]: LICENSE
