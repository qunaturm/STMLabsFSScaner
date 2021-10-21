using System.Collections.Generic;

namespace FSScanerUI.Domain
{
    public class Folder
    {
        public string FullPath { get; set; }
        public long Size { get; set; }
        public List<File> Files { get; set; }
        public List<Folder> Folders { get; set; }
        public delegate void ChangeRootSize(long size);
        private ChangeRootSize Callback { get; set; }
        public Folder(string fullPath, ChangeRootSize callback)
        {
            FullPath = fullPath;
            Callback = callback;
        }
        public void ChangeSize(long size)
        {
            Size += size;
            Callback?.Invoke(size);
        }
    }
}
