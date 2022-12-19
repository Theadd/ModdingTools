using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace ModdingTools.Core.Common;

public class ZipLib
{
    public DirectoryInfo Destination { get; set; } = new DirectoryInfo(".");
    
    public void UnzipFromStream(Stream zipStream, Func<string, string> entryFileRenameFunc)
    {
        var outFolder = Destination.FullName;
        
        using(var zipInputStream = new ZipInputStream(zipStream))
        {
            while (zipInputStream.GetNextEntry() is ZipEntry zipEntry)
            {
                var entryFileName = entryFileRenameFunc(zipEntry.Name);
                // To remove the folder from the entry:
                //var entryFileName = Path.GetFileName(entryFileName);
                // Optionally match entrynames against a selection list here
                // to skip as desired.
                // The unpacked length is available in the zipEntry.Size property.

                // 4K is optimum
                var buffer = new byte[4096];

                // Manipulate the output filename here as desired.
                var fullZipToPath = Path.Combine(outFolder, entryFileName);
                var directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0)
                    Directory.CreateDirectory(directoryName);

                // Skip directory entry
                if (Path.GetFileName(fullZipToPath).Length == 0)
                {
                    continue;
                }

                // Unzip file in buffered chunks. This is just as fast as unpacking
                // to a buffer the full size of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                }
            }
        }
    }
}
