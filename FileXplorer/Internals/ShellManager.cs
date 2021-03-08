using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

namespace FileXplorer.Internals
{
    public static class Interop
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out ShellFileInfo psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        public const uint SHGFI_ICON = 0x000000100;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        public const uint SHGFI_OPENICON = 0x000000002;
        public const uint SHGFI_SMALLICON = 0x000000001;
        public const uint SHGFI_LARGEICON = 0x000000000;
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        public const uint FILE_ATTRIBUTE_FILE = 0x00000100;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ShellFileInfo
    {
        public IntPtr hIcon;

        public int iIcon;

        public uint dwAttributes;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    public class ShellManager
    {
        public static Icon GetIcon(string path, ItemType type, IconSize iconSize, ItemState state)
        {
            var attributes = (uint)(type == ItemType.Folder ? FileAttribute.Directory : FileAttribute.File);
            var flags = (uint)(ShellAttribute.Icon | ShellAttribute.UseFileAttributes);

            if (type == ItemType.Folder && state == ItemState.Open)
            {
                flags = flags | (uint)ShellAttribute.OpenIcon;
            }
            if (iconSize == IconSize.Small)
            {
                flags = flags | (uint)ShellAttribute.SmallIcon;
            }
            else
            {
                flags = flags | (uint)ShellAttribute.LargeIcon;
            }

            var fileInfo = new ShellFileInfo();
            var size = (uint)Marshal.SizeOf(fileInfo);
            var result = Interop.SHGetFileInfo(path, attributes, out fileInfo, size, flags);

            if (result == IntPtr.Zero)
            {
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            try
            {
                return (Icon)Icon.FromHandle(fileInfo.hIcon).Clone();
            }
            catch
            {
                throw;
            }
            finally
            {
                Interop.DestroyIcon(fileInfo.hIcon);
            }
        }
    }

    public static class FolderManager
    {
        public static ImageSource GetImageSource(string directory, ItemState folderType)
        {
            return GetImageSource(directory, new Size(16, 16), folderType);
        }

        public static ImageSource GetImageSource(string directory, Size size, ItemState folderType)
        {
            using (var icon = ShellManager.GetIcon(directory, ItemType.Folder, IconSize.Large, folderType))
            {
                return Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
            }
        }
    }

    public static class FileManager
    {
        public static ImageSource GetImageSource(string filename)
        {
            return GetImageSource(filename, new Size(16, 16));
        }

        public static ImageSource GetImageSource(string filename, Size size)
        {
            using (var icon = ShellManager.GetIcon(Path.GetExtension(filename), ItemType.File, IconSize.Small, ItemState.Undefined))
            {
                return Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
            }
        }
    }


}
