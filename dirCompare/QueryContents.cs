using System;
using System.Collections.Generic;
using System.Linq;

namespace dirCompare
{
    public class QueryContents
    {
        public QueryContents()
        {
            string startFolder = "/Users/tanner/Dev/exago";
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(startFolder);
            IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            string searchTerm = @"Visual Studio";

            var queryMatchingFiles =
                from file in fileList
                where file.Extension == ".htm"
                let fileText = GetFileText(file.FullName)
                where fileText.Contains(searchTerm)
                select file.FullName;

            // Read the contents of the file.  
            static string GetFileText(string name)
            {
                string fileContents = String.Empty;

                // If the file has been deleted since we took
                // the snapshot, ignore it and return the empty string.  
                if (System.IO.File.Exists(name))
                {
                    fileContents = System.IO.File.ReadAllText(name);
                }
                return fileContents;
            }
        }
    }
}
