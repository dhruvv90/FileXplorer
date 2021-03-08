using System;
using System.IO;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Linq;
using FileXplorer.Internals;

namespace FileXplorer
{
    public class FileSystemEntity : IBaseClass
    {
        public FileSystemInfo FileSystemInfo
        {
            get { return base.GetValue<FileSystemInfo>("FileSystemInfo"); }
            private set { base.SetValue("FileSystemInfo", value); }
        }

        public bool IsExpanded
        {
            get { return base.GetValue<bool>("IsExpanded"); }
            set { base.SetValue("IsExpanded", value); }
        }

        public ObservableCollection<FileSystemEntity> Children
        {
            get { return base.GetValue<ObservableCollection<FileSystemEntity>>("Children"); }
            private set { base.SetValue("Children", value); }
        }

        public ImageSource ImageSource
        {
            get { return base.GetValue<ImageSource>("ImageSource"); }
            private set { base.SetValue("ImageSource", value); }
        }

        public FileSystemEntity(FileSystemInfo info)
        {
            if (this is DummyFileSystemEntity)
            {
                return;
            }

            this.Children = new ObservableCollection<FileSystemEntity>();
            this.FileSystemInfo = info;
            if (info is DirectoryInfo)
            {
                this.ImageSource = FolderManager.GetImageSource(info.FullName, ItemState.Close);
                this.AddDummy();
            }
            else if (info is FileInfo)
            {
                this.ImageSource = FileManager.GetImageSource(info.FullName);
            }

            this.PropertyChanged += FileSystemEntity_PropertyChanged;
        }

        private bool HasDummy()
        {
            return !ReferenceEquals(this.GetDummy(), null);
        }

        private void AddDummy()
        {
            this.Children.Add(new DummyFileSystemEntity());
        }

        private DummyFileSystemEntity GetDummy()
        {
            var list = this.Children.OfType<DummyFileSystemEntity>().ToList();
            if (list.Count > 0)
            {
                return list.First();
            }
            return null;
        }

        private void RemoveDummy()
        {
            this.Children.Remove(this.GetDummy());
        }


        private void FileSystemEntity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.FileSystemInfo is DirectoryInfo)
            {
                if (string.Equals(e.PropertyName, "IsExpanded", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (this.IsExpanded)
                    {
                        this.ImageSource = FolderManager.GetImageSource(this.FileSystemInfo.FullName, ItemState.Open);
                        if (this.HasDummy())
                        {
                            this.RemoveDummy();
                            this.ExploreDirectories();
                            this.ExploreFiles();
                        }
                    }
                    else
                    {
                        this.ImageSource = FolderManager.GetImageSource(this.FileSystemInfo.FullName, ItemState.Close);
                    }
                }
            }
        }

        private void ExploreDirectories()
        {
            if (this.FileSystemInfo is DirectoryInfo)
            {
                DirectoryInfo[] directories = (this.FileSystemInfo as DirectoryInfo).GetDirectories();

                foreach (DirectoryInfo dir in directories)
                {
                    bool isHidden = (dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                    bool isSystem = (dir.Attributes & FileAttributes.System) == FileAttributes.System;
                    
                    if(!isHidden && !isSystem)
                    {
                        this.Children.Add(new FileSystemEntity(dir));
                    }
                }
            }
        }

        private void ExploreFiles()
        {
            if (this.FileSystemInfo is DirectoryInfo)
            {
                DirectoryInfo[] directories = (this.FileSystemInfo as DirectoryInfo).GetDirectories();
                FileInfo[] files = (this.FileSystemInfo as DirectoryInfo).GetFiles();

                foreach (FileInfo file in files)
                {
                    bool isHidden = (file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                    bool isSystem = (file.Attributes & FileAttributes.System) == FileAttributes.System;

                    if (!isHidden && !isSystem)
                    {
                        this.Children.Add(new FileSystemEntity(file));
                    }
                }
            }
        }

        public FileSystemEntity(DriveInfo drive) : this(drive.RootDirectory)
        { }

    }

    public class DummyFileSystemEntity : FileSystemEntity
    {
        public DummyFileSystemEntity() : base(new DirectoryInfo("Empty"))
        { }

    }
}
