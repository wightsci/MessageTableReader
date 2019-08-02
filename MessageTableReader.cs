using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

 namespace MessageTableReader
 {
    public class Reader
    {

    //Based on https://stackoverflow.com/questions/33498244/marshaling-a-message-table-resource
                    
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr FindResource(IntPtr hModule, int lpID, int lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int FormatMessage(
            FormatMessageFlags dwFlags,
            IntPtr lpSource,
            uint dwMessageId,
            uint dwLangugeId,
            ref IntPtr lpBuffer,
            uint nSize,
            IntPtr Arguments
        );

        [StructLayout(LayoutKind.Sequential)]
        struct MESSAGE_RESOURCE_BLOCK
        {
            public int LowId;
            public int HighId;
            public int OffsetToEntries;
        }

        [Flags]
        public enum FormatMessageFlags : uint
        {
            FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,
            FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,
            FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,
            FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000,
            FORMAT_MESSAGE_FROM_HMODULE = 0x00000800,
            FORMAT_MESSAGE_FROM_STRING = 0x00000400,
        }

        public IList<string> GetMessageList(string filepath = @"C:\WINDOWS\system32\msobjs.dll")
        {
            IList<string> messageList = new List<string>();
            const int RT_MESSAGETABLE = 11;
            if (!File.Exists(filepath))
            {
                throw new ApplicationException(string.Format("File {0} not found.", filepath));
            }
            else
            {
                IntPtr hModule = LoadLibrary(filepath);
                IntPtr msgTableInfo = FindResource(hModule, 1, RT_MESSAGETABLE);
                IntPtr msgTable = LoadResource(hModule, msgTableInfo);
                IntPtr memTable = LockResource(msgTable);
                int numberOfBlocks = Marshal.ReadInt32(memTable);
                IntPtr blockPtr = IntPtr.Add(memTable, 4);
                int blockSize = Marshal.SizeOf<MESSAGE_RESOURCE_BLOCK>();
                for (int i = 0; i < numberOfBlocks; i++)
                {
                    var block = Marshal.PtrToStructure<MESSAGE_RESOURCE_BLOCK>(blockPtr);
                    IntPtr entryPtr = IntPtr.Add(memTable, block.OffsetToEntries);
                    for (int id = block.LowId; id <= block.HighId; id++)
                    {
                        var length = Marshal.ReadInt16(entryPtr);
                        var flags = Marshal.ReadInt16(entryPtr, 2);
                        var textPtr = IntPtr.Add(entryPtr, 4);
                        var text = "Bad flags??";
                        if (flags == 0)
                        {
                            text = Marshal.PtrToStringAnsi(textPtr);
                        }
                        else if (flags == 1)
                        {
                            text = Marshal.PtrToStringUni(textPtr);
                        }
                        text = text.Replace("\r\n", "");
                        messageList.Add(string.Format("{0}:{1}", id, text));
                        entryPtr = IntPtr.Add(entryPtr, length);
                    }
                    blockPtr = IntPtr.Add(blockPtr, blockSize);
                }
            }
            return messageList;
        }

        public string GetMessage(uint messageid, string filepath = @"C:\WINDOWS\system32\msobjs.dll")
        {
            string messagetext = "";
            if (!File.Exists(filepath))
            {
                throw new ApplicationException(string.Format("File {0} not found.", filepath));
            }
            else
            {
                IntPtr hModule = LoadLibrary(filepath);
                IntPtr textPtr = IntPtr.Zero;
                int returnValue = FormatMessage(FormatMessageFlags.FORMAT_MESSAGE_FROM_HMODULE | FormatMessageFlags.FORMAT_MESSAGE_ALLOCATE_BUFFER | FormatMessageFlags.FORMAT_MESSAGE_IGNORE_INSERTS,
                hModule,
                messageid,
                0,
                ref textPtr,
                0,
                IntPtr.Zero
                );
                if (returnValue == 0)
                {
                    throw new ApplicationException(string.Format("Message ID {0} not found in file {1}", messageid, filepath));
                }
                messagetext = Marshal.PtrToStringAnsi(textPtr);
                messagetext = messagetext.Replace("\r\n", "");
            }
            return messagetext;
        }
    }  
 }