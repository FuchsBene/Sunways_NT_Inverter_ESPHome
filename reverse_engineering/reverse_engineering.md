# Reverse Engineering

This directory contains relevant protocol-related files extracted from **Sunways NT Monitor Software V2.0.2a**.

The files originate from the original installation directory (`Program Files`).


## NTMonitor.mdb

`NTMonitor.mdb` is a Microsoft Access database used by NT Monitor to store historical measurement data and application configuration.
The database contains valuable information for protocol analysis, parameter scaling, and insights into how the software interprets inverter data.


### Exporting database tables to CSV (macOS)

To export all database tables into individual CSV files on macOS, first install `mdbtools` using Homebrew:

```
brew install mdbtools
```

Then run the following command in the directory containing `NTMonitor.mdb`:

```
mdb-tables -1 NTMonitor.mdb | xargs -I{} bash -c 'mdb-export NTMonitor.mdb "$1" > "$1".csv' -- {}
```


## DLLs & NTMonitor.exe

The most important component for understanding the communication protocol is `SWCommLib.dll`. It contains substantial parts of the protocol implementation, including the communication logic used by NT Monitor.
The remaining DLLs may also provide useful context.

`NTMonitor.exe` is the application executable.


## Decompiled Code

Selected parts of the original binaries have already been decompiled to C# and are available in the `./decompiled_snippets` directory.