# MessageTableReader

A .Net class to allow access to the message table entries in a .dll or .exe file.

This class can be used within PowerShell using ``Add-Type`` or compiled into a Windows library (.dll).

This script uses C# code to add a class called MessageTableReader.Reader to the PowerShell session.

The class has 2 methods: GetMessageList and GetMessage.

GetMessageList returns a list of all of the messages and message IDs in a file. If no file name is provided, then ``C:\WINDOWS\system32\msobjs.dll`` is used.

GetMessage takes a message ID and file name and returns the text of the specific message in the file. If no file name is provided, then ``C:\WINDOWS\system32\msobjs.dll`` is used.

## What Use is MessageTableReader?

MessageTableReader was originally conceived to help with monitoring and audit of Windows Event Logs. Many event log messages have placeholder codes that need to be looked up from .dll files. MessageTableReader can do this. There's more detail on this in my ebook *PowerShell and Windows Event Logs*.

### Example
```PowerShell
PS C:\>.\MessageTableReader.ps1
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
PS C:\>.\MessageTableReader.ps1
PS C:\>$messageTable = New-Object MessageTableReader.Reader
PS C:\>$messageTable.GetMessage(14676,'C:\WINDOWS\system32\msobjs.dll')
Active Directory Domain Services
```