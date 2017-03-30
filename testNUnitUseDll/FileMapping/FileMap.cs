namespace NAT
{
    namespace INFRA
    {
        using System;
        using System.IO;
        using System.Runtime.InteropServices;




        //
        // Win32MapApis
        //
        /// <remarks>
        ///   Defines the PInvoke functions we use
        ///   to access the FileMapping Win32 APIs
        /// </remarks>
        //
        [StructLayout(LayoutKind.Sequential)]
        public class OFSTRUCT
        {
            public const int OFS_MAXPATHNAME = 128;
            public byte cBytes;
            public byte fFixedDisc;
            public UInt16 nErrCode;
            public UInt16 Reserved1;
            public UInt16 Reserved2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = OFS_MAXPATHNAME)]
            public string szPathName;
        }
        internal class Win32MapApis
        {
            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern int GetFileSize(IntPtr hFile, int lpFileSizeHigh);


            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr CreateFile(
                String lpFileName, int dwDesiredAccess, int dwShareMode,
                IntPtr lpSecurityAttributes, int dwCreationDisposition,
                int dwFlagsAndAttributes, IntPtr hTemplateFile);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr OpenFile(String lpFileName,
                [Out, MarshalAs(UnmanagedType.LPStruct)]
				OFSTRUCT lpReOpenBuff,
                int uStyle);


            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr CreateFileMapping(
                IntPtr hFile, IntPtr lpAttributes, int flProtect,
                int dwMaximumSizeLow, int dwMaximumSizeHigh,
                String lpName);

            [DllImport("kernel32", SetLastError = true)]
            public static extern bool FlushViewOfFile(
                IntPtr lpBaseAddress, int dwNumBytesToFlush);

            [DllImport("kernel32", SetLastError = true)]
            public static extern IntPtr MapViewOfFile(
                IntPtr hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh,
                int dwFileOffsetLow, int dwNumBytesToMap);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr OpenFileMapping(
                int dwDesiredAccess, bool bInheritHandle, String lpName);

            [DllImport("kernel32", SetLastError = true)]
            public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

            [DllImport("kernel32", SetLastError = true)]
            public static extern bool CloseHandle(IntPtr handle);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr GlobalAddAtom(String lpAtomName);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr GlobalDeleteAtom(IntPtr lpAtomName);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr GlobalFindAtom(
                String lpName);


        } // class Win32MapApis

        //
        // FileMapIOException
        //
        ///
        /// <exception cref="System.Exception">
        ///   Represents an exception occured as a result of an
        ///   invalid IO operation on any of the File mapping classes
        ///   It wraps the error message and the underlying Win32 error
        ///   code that caused the error.
        /// </exception>
        //
        public class FileMapIOException : IOException
        {
            private int m_win32Error = 0;
            public int Win32ErrorCode
            {
                get { return m_win32Error; }
            }
            public override string Message
            {
                get
                {
                    if (Win32ErrorCode != 0)
                        return base.Message + " (" + Win32ErrorCode + ")";
                    else
                        return base.Message;
                }
            }
            public FileMapIOException(int error)
                : base()
            {
                m_win32Error = error;
            }
            public FileMapIOException(string message)
                : base(message)
            {
            }
            public FileMapIOException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }



        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum MapProtection
        {
            PageNone = 0x00000000,
            // protection
            PageReadOnly = 0x00000002,
            PageReadWrite = 0x00000004,
            PageWriteCopy = 0x00000008,
            // attributes
            SecImage = 0x01000000,
            SecReserve = 0x04000000,
            SecCommit = 0x08000000,
            SecNoCache = 0x10000000,
        }

        //
        // MapViewStream
        //
        /// <remarks>
        ///   Allows you to read/write from/to
        ///   a view of a memory mapped file.
        /// </remarks>
        //
        public class MapViewStream : Stream//, IDisposable
        {
            MapProtection m_protection = MapProtection.PageNone;
            //base address of our buffer
            IntPtr m_base = IntPtr.Zero;
            //our current buffer length
            long m_length = 0;
            //our current position in the stream buffer
            long m_position = 0;
            //open status
            bool m_isOpen = false;

            internal MapViewStream(IntPtr baseAddress, long length,
                MapProtection protection)
            {
                m_base = baseAddress;
                m_length = length;
                m_protection = protection;
                m_position = 0;
                m_isOpen = (baseAddress.ToInt64() > 0);
            }

            ~MapViewStream()
            {
                this.Close();
            }

            public void Dispose()
            {
                this.Close();
                GC.SuppressFinalize(this);
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return true; }
            }
            public override bool CanWrite
            {
                get { return (((int)m_protection) & 0x000000C) != 0; }
            }
            public override long Length
            {
                get
                {

                    if (m_length == 0)
                    {

                        m_length = 4;
                        long temp = 0;
                        temp = this.ReadByte();
                        temp <<= 8;
                        temp += this.ReadByte();
                        temp <<= 8;
                        temp += this.ReadByte();
                        temp <<= 8;
                        temp += this.ReadByte();
                        m_length = temp;
                    }
                    return m_length;
                }
            }
            public override long Position
            {
                get { return m_position; }
                set
                {
                    if (value < this.Length)
                        m_position = value;
                    else
                        throw new FileMapIOException("Invalid Position");
                }
            }
            private bool IsOpen
            {
                get { return m_isOpen; }
                set { m_isOpen = value; }
            }

            public override void Flush()
            {
                if (!IsOpen)
                    throw new FileMapIOException("Stream is closed!");
                Win32MapApis.FlushViewOfFile(m_base, (int)m_length);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int i;
                long InternalLength = Length;
                try
                {
                    if (!IsOpen)
                        throw new FileMapIOException("Stream is closed!");

                    if (buffer.Length - offset < count)
                        throw new ArgumentException("Invalid Offset!");

                    Marshal.Copy((IntPtr)(m_base.ToInt64() + m_position), buffer, 0, count);
                    /*
                    for ( i=0; (i < (InternalLength-m_position)) && (i < count); i++ )
                    {
                        buffer[offset+i] = Marshal.ReadByte ( (IntPtr)(m_base.ToInt64()+m_position+i));
                    }
                    m_position += i;
                    return i;
                    */
                    m_position += count;
                    return count;
                }
                catch (Exception Err)
                {
                    throw Err;
                }
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                int i;
                try
                {
                    if (!this.IsOpen || !this.CanWrite)
                        throw new FileMapIOException("Stream cannot be written!");

                    if (buffer.Length - offset < count)
                        throw new ArgumentException("Invalid Offset!");

                    for (i = 0; (i < (Length - m_position)) && (i < count); i++)
                    {
                        Marshal.WriteByte((IntPtr)(m_base.ToInt64() + m_position + i), buffer[offset + i]);
                    }
                    m_position += i;
                }
                catch (Exception Err)
                {
                    throw Err;
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                try
                {
                    if (!IsOpen)
                        throw new FileMapIOException("Stream is closed!");

                    long newpos = 0;
                    switch (origin)
                    {
                        case SeekOrigin.Begin: newpos = offset; break;
                        case SeekOrigin.Current: newpos = Position + offset; break;
                        case SeekOrigin.End: newpos = Length - offset; break;
                    }
                    if (newpos < 0 || newpos > Length)
                        throw new FileMapIOException("Invalid Seek Offset!");
                    m_position = newpos;
                    return newpos;
                }
                catch (Exception Err)
                {
                    throw Err;
                }
            }

            public override void SetLength(long value)
            {
                // not supported!
                throw new NotSupportedException("Can't change View Length");
            }

            public override void Close()
            {
                if (IsOpen)
                {
                    Flush();
                    Win32MapApis.UnmapViewOfFile(m_base);
                    IsOpen = false;
                }
            }



        }

        /// <summary>
        /// 
        /// </summary>
        public enum MapAccess
        {
            FileMapCopy = 0x0001,
            FileMapWrite = 0x0002,
            FileMapRead = 0x0004,
            FileMapAllAccess = 0x001f,
        }



        /// <summary>
        /// 
        /// </summary>
        public class MemoryMappedFile : IDisposable
        {
            //
            // variables
            //
            //handle to MemoryMappedFile object
            private IntPtr m_hMap = IntPtr.Zero;
            //hold the file max size
            private long m_maxSize;

            private const int GENERIC_READ = unchecked((int)0x80000000);
            private const int GENERIC_WRITE = unchecked((int)0x40000000);
            private const int OPEN_ALWAYS = 4;
            private readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            private readonly IntPtr NULL_HANDLE = IntPtr.Zero;


            public MemoryMappedFile()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fileName"></param>
            /// <param name="protection"></param>
            /// <param name="access"></param>
            /// <param name="maxSize"></param>
            /// <param name="name"></param>
            public MemoryMappedFile(String fileName, MapProtection protection, MapAccess access,
                long maxSize, String name)
            {
                //try to open the mmf by name. if failed create the file and mmf.
                IntPtr hFile = IntPtr.Zero;
                try
                {
                    m_hMap = Win32MapApis.OpenFileMapping((int)access, false, name);
                    if (m_hMap == NULL_HANDLE)
                    {
                        int desiredAccess = GENERIC_READ;
                        if ((protection == MapProtection.PageReadWrite) ||
                            (protection == MapProtection.PageWriteCopy))
                        {
                            desiredAccess |= GENERIC_WRITE;
                        }
                        hFile = Win32MapApis.CreateFile(
                            GetMMFDir() + fileName, desiredAccess, 0,
                            IntPtr.Zero, OPEN_ALWAYS, 0, IntPtr.Zero
                            );
                        if (hFile != NULL_HANDLE)
                        {
                            m_hMap = Win32MapApis.CreateFileMapping(
                                hFile, IntPtr.Zero, (int)protection,
                                0,
                                (int)(maxSize & 0xFFFFFFFF), name
                                );
                            if (m_hMap != NULL_HANDLE)
                                m_maxSize = maxSize;
                            else
                                throw new FileMapIOException(Marshal.GetHRForLastWin32Error());
                        }
                        else
                            throw new FileMapIOException(Marshal.GetHRForLastWin32Error());
                    }
                }
                catch (Exception Err)
                {
                    throw Err;
                }
                finally
                {
                    if (!(hFile == NULL_HANDLE)) Win32MapApis.CloseHandle(hFile);
                }

            }
            /// <summary>
            /// 
            /// </summary>
            ~MemoryMappedFile()
            {
                this.Close();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fileName"></param>
            /// <param name="protection"></param>
            /// <param name="maxSize"></param>
            /// <param name="name"></param>
            public void Create(String fileName, MapProtection protection,
                long maxSize, String name)
            {
                // open file first
                IntPtr hFile = INVALID_HANDLE_VALUE;

                try
                {
                    if (fileName != null)
                    {

                        // determine file access needed
                        // we'll always need generic read access
                        int desiredAccess = GENERIC_READ;
                        if ((protection == MapProtection.PageReadWrite) ||
                            (protection == MapProtection.PageWriteCopy))
                        {
                            desiredAccess |= GENERIC_WRITE;
                        }

                        // open or create the file
                        // if it doesn't exist, it gets created
                        hFile = Win32MapApis.CreateFile(
                            GetMMFDir() + fileName, desiredAccess, 0,
                            IntPtr.Zero, OPEN_ALWAYS, 0, IntPtr.Zero
                            );
                        if (hFile != NULL_HANDLE)
                        {
                            m_hMap = Win32MapApis.CreateFileMapping(
                                hFile, IntPtr.Zero, (int)protection,
                                (int)((maxSize >> 32) & 0xFFFFFFFF),
                                (int)(maxSize & 0xFFFFFFFF), name
                                );
                        }
                        else
                        {
                            throw new FileMapIOException(Marshal.GetHRForLastWin32Error());
                        }
                        if (m_hMap == NULL_HANDLE)
                            throw new FileMapIOException(Marshal.GetHRForLastWin32Error());
                    }
                }
                catch (Exception Err)
                {
                    throw Err;
                }
                finally
                {
                    if (!(hFile == NULL_HANDLE)) Win32MapApis.CloseHandle(hFile);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="access"></param>
            /// <param name="name"></param>
            public bool Open(MapAccess access, String name)
            {
                bool RV = true;
                try
                {
                    m_hMap = Win32MapApis.OpenFileMapping((int)access, false, name);
                    if (m_hMap == NULL_HANDLE)
                        RV = false;
                    return RV;
                }
                catch
                {
                    return RV;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="FileName"></param>
            /// <param name="protection"></param>
            /// <param name="name"></param>
            /// <param name="access"></param>
            /// <returns></returns>
            public bool OpenEx(string FileName, MapProtection protection, string name, MapAccess access)
            {
                bool RV = false;

                m_hMap = Win32MapApis.OpenFileMapping((int)access, false, name);
                if (m_hMap == NULL_HANDLE)
                {
                    if (System.IO.File.Exists(GetMMFDir() + FileName))
                    {
                        long maxSize;
                        System.IO.FileStream stream = System.IO.File.Open(GetMMFDir() + FileName, System.IO.FileMode.Open);
                        maxSize = stream.Length;
                        stream.Close();


                        IntPtr hFile = INVALID_HANDLE_VALUE;
                        OFSTRUCT ipStruct = new OFSTRUCT();
                        string MMFName = GetMMFDir() + FileName;
                        hFile = Win32MapApis.OpenFile(MMFName,
                            ipStruct
                            , 2);

                        // determine file access needed
                        // we'll always need generic read access
                        int desiredAccess = GENERIC_READ;
                        if ((protection == MapProtection.PageReadWrite) ||
                            (protection == MapProtection.PageWriteCopy))
                        {
                            desiredAccess |= GENERIC_WRITE;
                        }

                        // open or create the file
                        // if it doesn't exist, it gets created
                        m_hMap = Win32MapApis.CreateFileMapping(
                            hFile, IntPtr.Zero, (int)protection,
                            (int)((maxSize >> 32) & 0xFFFFFFFF),
                            (int)(maxSize & 0xFFFFFFFF), name
                            );

                        // close file handle, we don't need it
                        if (!(hFile == NULL_HANDLE)) Win32MapApis.CloseHandle(hFile);
                        RV = true;
                    }
                }
                else
                    RV = true;
                return RV;
            }
            /// <summary>
            /// 
            /// </summary>
            public void Close()
            {
                if (m_hMap != NULL_HANDLE)
                    Win32MapApis.CloseHandle(m_hMap);
                m_hMap = NULL_HANDLE;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="access"></param>
            /// <param name="offset"></param>
            /// <param name="size"></param>
            /// <param name="path"></param>
            /// <returns></returns>
            public MapViewStream MapView(MapAccess access, long offset, int size, string path)
            {
                IntPtr baseAddress = IntPtr.Zero;

                try
                {
                    baseAddress = Win32MapApis.MapViewOfFile(
                        m_hMap, (int)access,
                        (int)((offset >> 32) & 0xFFFFFFFF),
                        (int)(offset & 0xFFFFFFFF), size
                        );

                    if (baseAddress != IntPtr.Zero)
                    {
                        MapProtection protection;
                        if (access == MapAccess.FileMapRead)
                            protection = MapProtection.PageReadOnly;
                        else
                            protection = MapProtection.PageReadWrite;

                        if (path != "")
                        {
                            System.IO.FileInfo oFi = new System.IO.FileInfo(GetMMFDir() + path);
                            m_maxSize = (int)oFi.Length;
                        }
                        return new MapViewStream(baseAddress, m_maxSize, protection);
                    }
                    return null;
                }
                catch
                {
                    throw new FileMapIOException(Marshal.GetHRForLastWin32Error());
                }
            }

            public void Dispose()
            {
                this.Close();
                GC.SuppressFinalize(this);
            }
            /// <summary>
            /// this function create and return the directpry of the MMF files.
            ///		we check if E:\ drive exist. if so we create the dir and 
            ///		return the path. if not we create it in the C:\ drive.
            /// </summary>
            /// <returns>string contain the path of MMF files</returns>
            public string GetMMFDir()
            {
                string[] strDrives = System.IO.Directory.GetLogicalDrives();
                string strDrv = "c:\\MmfFiles\\";
                for (int i = 0; i < strDrives.Length - 1; i++)
                {
                    if (strDrives[i].ToUpper() == "E:\\")
                        strDrv = "e:\\MmfFiles\\";
                }
                //if directory not exist create one
                if (!System.IO.Directory.Exists(strDrv))
                    System.IO.Directory.CreateDirectory(strDrv);
                return strDrv;
            }


        }
    }
}