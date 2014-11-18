using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoJudge
{
    public class FileEnumerator
    {
        public string SearchDirectory { get; private set; }

        private string[] Files { get; set; }
        private int FileIndex { get; set; }

        public FileEnumerator(string directory)
        {
            SearchDirectory = directory;
        }

        private void EnsureFiles()
        {
            if (Files == null)
            {
                Files = Directory.GetFiles(SearchDirectory, "*", SearchOption.AllDirectories);
            }
        }

        public FileInfo GetNextFile()
        {
            EnsureFiles();

            if (FileIndex < Files.Length)
            {
                return new FileInfo(Files[FileIndex++]);
            }
            
            return null;
        }
    }
}
