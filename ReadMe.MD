# MessageTableReader

A .Net class to allow access to the message table entries in a .dll or .exe file.

This class can be used within PowerShell using ``Add-Type`` or compiled into a Windows library (.dll).

[![Build Status](https://dev.azure.com/wightsci/GitHubRepos/_apis/build/status/wightsci.MessageTableReader?branchName=master)](https://dev.azure.com/wightsci/GitHubRepos/_build/latest?definitionId=3&branchName=master)

## Installation
First, download the MessageTableReader.cs C# file from this repo.

### Method 1
Load the file into a variable in your script by using ``Get-Content`` or by pasting it into a here-string.

Import the class by running by running the following, where ``<variablename>`` is the name of the variable that you have loaded the C# into. 
```PowerShell
Add-Type -TypeDefinition <variablename>
```

### Method 2
Import the class directly by running by running the following, where ``<path>`` is the path ot the C# file (or dll). 
```PowerShell
Add-Type -Path <path>
```
## Compiling the .DLL
The C# file can be compiled using the C# complier ```csc.exe``` provided as part of the .Net framework. No Visual Studio required. The version of .Net must be 4.0 or greater.

``csc.exe /target:library /out:MessageTableReader.dll MessageTableReader.cs``

## Using the Class

## Methods

#### GetMessageList(string filename)
GetMessageList returns a list of all of the messages and message IDs in a file. If no file name is provided, then ``C:\WINDOWS\system32\msobjs.dll`` is used.

#### GetMessage(string id, string filename)
GetMessage takes a message ID and file name and returns the text of the specific message in the file. If no file name is provided, then ``C:\WINDOWS\system32\msobjs.dll`` is used.

## What Use is MessageTableReader?

MessageTableReader was originally conceived to help with monitoring and audit of Windows Event Logs. Many event log messages have placeholder codes that need to be looked up from .dll files. MessageTableReader can do this. There's more detail on this in my ebook *PowerShell and Windows Event Logs*.

### Example
```PowerShell
PS C:\>$messageTable = New-Object MessageTableReader.Reader
PS C:\>$messageTable.GetMessageList('C:\WINDOWS\system32\msobjs.dll')
279:Undefined Access (no effect) Bit 7
1536:Unused message ID
1537:DELETE
1538:READ_CONTROL
1539:WRITE_DAC
1540:WRITE_OWNER
1541:SYNCHRONIZE
1542:ACCESS_SYS_SEC
1543:MAX_ALLOWED
...
```

### Example
```PowerShell
PS C:\>$messageTable = New-Object MessageTableReader.Reader
PS C:\>$messageTable.GetMessage(14676,'C:\WINDOWS\system32\msobjs.dll')
Active Directory Domain Services
```
## Why not just provide a .dll ?
Some organisations are not able to introduce unapproved binary files into their environment (with good reason). The textual C# is an alternative, and the code can be easily scrutinised.