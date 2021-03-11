using System;
using System.IO;
using System.Windows;


namespace FileXplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeFileExplorer();
        }

        private void InitializeFileExplorer()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    this.treeView.Items.Add(new FileSystemEntity(drive));
                }
            }
            SelectDefaultDirectory();
        }

        private void SelectDefaultDirectory()
        {
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            new TreeViewHelper(this.treeView).NavigateToDirectory(defaultPath);
        }
    }
}
