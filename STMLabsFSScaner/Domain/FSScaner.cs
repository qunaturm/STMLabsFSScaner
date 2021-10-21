using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace STMLabsFSScaner.Domain
{
    public class FSScaner
    {
        private Dictionary<string, Folder> avaliableFolders { get; set; }
        public FSScaner()
        {
            avaliableFolders = new Dictionary<string, Folder>();
        }
        public Folder? GetFolder(string path)
        {
            avaliableFolders.TryGetValue(path, out var folder);
            return folder;
        }
        public void ScanFolder(string path)
        {
            var folders = TraverseFolder(path);
            foreach (var folder in folders)
            {
                avaliableFolders.Add(folder.FullPath, folder);
            }
        }
        private List<Folder> TraverseFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new ArgumentException();
            }
            Stack<Folder> foldersStack = new Stack<Folder>();
            List<Folder> result = new List<Folder>();
            Folder rootFolder = new Folder(path, null);
            foldersStack.Push(rootFolder);
            result.Add(rootFolder);
            while (foldersStack.Count > 0)
            {
                Folder currFolder = foldersStack.Pop();
                DirectoryInfo dirInfo = new DirectoryInfo(currFolder.FullPath);

                FileInfo[] nestedFiles = null;
                long filesSize = 0;
                try
                {
                    nestedFiles = dirInfo.GetFiles();
                    if (nestedFiles.Length > 0)
                    {
                        filesSize = nestedFiles.Select(u => u.Length).Aggregate((x, y) => x + y);
                    }
                }
                catch (UnauthorizedAccessException e)
                {

                    Console.WriteLine(e.Message);
                    continue;
                }

                catch (System.IO.DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                List<Folder> nestedFolders = new List<Folder>();
                try
                {
                    if (Directory.GetDirectories(currFolder.FullPath).Length > 0)
                    {
                        nestedFolders = dirInfo.GetDirectories().Select(x =>
                            new Folder(x.FullName, currFolder.ChangeSize)).ToList();
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

               
                currFolder.Folders = nestedFolders;
                currFolder.Files = nestedFiles.Select(x => new File()
                {
                    FullPath = x.FullName,
                    Size = x.Length
                }).ToList();
                currFolder.ChangeSize(filesSize);

                if (nestedFolders.Count > 0)
                {
                    foreach ( var folder in nestedFolders)
                    {
                        foldersStack.Push(folder);
                    }
                }
                result.AddRange(nestedFolders);
            }
            return result;
        }
    }
}
