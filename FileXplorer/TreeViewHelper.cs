using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace FileXplorer
{
    public class TreeViewHelper
    {
        private readonly TreeView _tree;

        public TreeViewHelper(TreeView t)
        {
            this._tree = t;            
        }

        public void NavigateToDirectory(string dirPath)
        {
            string rootString = Directory.GetDirectoryRoot(dirPath);
            FileSystemEntity rootEntity = new DummyFileSystemEntity();

            foreach (var item in this._tree.Items.OfType<FileSystemEntity>())
            {
                if (item.FileSystemInfo.FullName == rootString)
                {
                    item.IsExpanded = true;
                    rootEntity = item;
                }
            }

            if (rootEntity is DummyFileSystemEntity)
            {
                return;
            }


            FileSystemEntity parentEntity = rootEntity;
            bool found = true;

            while (found)
            {
                foreach (var child in parentEntity.Children)
                {
                    found = false;
                    if (dirPath.StartsWith(child.FileSystemInfo.FullName))
                    {
                        child.IsExpanded = true;
                        child.IsSelected = true;
                        parentEntity = child;
                        found = true;
                        break;
                    }
                }
            }
        }
    }
}
