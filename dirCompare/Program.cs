using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace dirCompare
{
    class Program
    {
        static void Main(string[] args)
        {
            CompareAllFiles();
            CompareDirectories();
            //CompareFiles();
        }

        public static void CompareAllFiles()
        {
            foreach (string[] item in DirPaths.ExagoRootDirectory)
            {
                string dirName = item[0];
                string dirPath1 = item[1];
                string dirPath2 = item[2];

                IEnumerable<string> dir1Files = Directory
                    .EnumerateFiles(dirPath1, "*", SearchOption.AllDirectories)
                    .Select(Path.GetFullPath);
                IEnumerable<string> dir2Files = Directory
                    .EnumerateFiles(dirPath2, "*", SearchOption.AllDirectories)
                    .Select(Path.GetFullPath);

                List<string> fileNameDir1 = new List<string>();
                List<string> fullPathNameDir1 = new List<string>();
                int ind = 0;
                foreach (var path in dir1Files)
                {
                    string lastpart = path.Split(Path.DirectorySeparatorChar).Last();
                    fileNameDir1.Insert(ind, lastpart);
                    fullPathNameDir1.Insert(ind, path);
                    ind++;
                }

                List<string> fileNameDir2 = new List<string>();
                List<string> fullPathNameDir2 = new List<string>();
                int ind2 = 0;
                foreach (var path in dir2Files)
                {
                    string lastpart = path.Split(Path.DirectorySeparatorChar).Last();
                    fileNameDir2.Insert(ind2, lastpart);
                    fullPathNameDir2.Insert(ind2, path);
                    ind2++;
                }

                string[] diff1s = fileNameDir1.Except(fileNameDir2).Distinct().ToArray();
                string[] diff2s = fileNameDir2.Except(fileNameDir1).Distinct().ToArray();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("====================================================================================");
                Console.WriteLine("================== Compare " + dirPath1 + " with " + dirPath2 + " ==================");
                Console.WriteLine("====================================================================================");
                Console.ResetColor();
                foreach (var word in diff1s)
                {
                    //igore these files, all git files have 38 characters
                    if (word.Contains(".DS_Store") || word.Length == 38)
                        continue;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("FAILURE ");
                    Console.ResetColor();
                    Console.WriteLine("The following file exists in " + dirPath1 + " but does NOT in " + dirPath2);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(word);
                    Console.ResetColor();

                    for (int i = 0; i < fileNameDir1.Count; i++)
                    {
                        if (fileNameDir1[i].Contains(word))
                        {
                            Console.WriteLine("possible path " + fullPathNameDir1[i] + "\n");
                        }
                    }
                }

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("====================================================================================");
                Console.WriteLine("================== Compare " + dirPath2 + " with " + dirPath1 + " ==================");
                Console.WriteLine("====================================================================================");
                Console.ResetColor();
                foreach (var word2 in diff2s)
                {
                    //igore these files, all git files have 38 characters
                    if (word2.Contains(".DS_Store") || word2.Length == 38)
                        continue;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("FAILURE ");
                    Console.ResetColor();
                    Console.WriteLine("The following file exists in " + dirPath2 + " but does NOT in " + dirPath1);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(word2);
                    Console.ResetColor();

                    for (int i = 0; i < fileNameDir1.Count; i++)
                    {
                        if (fileNameDir2[i].Contains(word2))
                        {
                            Console.WriteLine("possible path " + fullPathNameDir2[i] + "\n");
                        }
                    }
                }

                // Keep the console window open in debug mode.  
                //Console.WriteLine("Press any key to exit.");
                //Console.ReadKey();
            }
        }

        public static void CompareFiles()
        {
            foreach (string[] item in DirPaths.ExagoRootDirectory)
            {
                Console.WriteLine("\n---------CHECKING FILES---------");
                string dirName = item[0];
                string dirPath1 = item[1];
                string dirPath2 = item[2];
                DirectoryInfo dir1 = new DirectoryInfo(dirPath1);
                DirectoryInfo dir2 = new DirectoryInfo(dirPath2);

                // Take a snapshot of the file system.  
                IEnumerable<FileInfo> list1 = dir1.GetFiles("*.*", SearchOption.AllDirectories);
                IEnumerable<FileInfo> list2 = dir2.GetFiles("*.*", SearchOption.AllDirectories);

                //----------------------------------------------------------
                FileCompare myFileCompare = new FileCompare();
                var queryCommonFiles = list1.Intersect(list2, myFileCompare);
                if (!queryCommonFiles.Any())
                    Console.WriteLine("There are no common files in the two folders.");

                var queryList1Only = (from file in list1
                                      select file).Except(list2, myFileCompare);

                foreach (var v in queryList1Only)
                {
                    //igore these files
                    string lastpart = v.FullName.Split(Path.DirectorySeparatorChar).Last();
                    if (v.FullName.Contains(".DS_Store") || lastpart.Length == 38)
                        continue;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("FILE FAILURE ");
                    Console.ResetColor();
                    Console.WriteLine("The following file exists in" + dirPath1 + " but does not in " + dirPath2);
                    Console.WriteLine(v.FullName);
                }

                //----------------------------------------------------------
                FileCompare myFileCompare2 = new FileCompare();
                var queryCommonFiles2 = list2.Intersect(list1, myFileCompare2);
                var queryList2Only = (from file in list2
                                      select file).Except(list1, myFileCompare2);

                foreach (var v in queryList2Only)
                {
                    //igore these files
                    string lastpart = v.FullName.Split(Path.DirectorySeparatorChar).Last();
                    if (v.FullName.Contains(".DS_Store") || lastpart.Length == 38)
                        continue;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("FILE FAILURE ");
                    Console.ResetColor();
                    Console.WriteLine("The following file exists in" + dirPath2 + " but does not in " + dirPath1);
                    Console.WriteLine(v.FullName);
                }
            }
        }

        public static void CompareDirectories()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("=============================================================");
            Console.WriteLine("================== CHECKING DIRECTORIES ==================");
            Console.WriteLine("=============================================================");
            Console.ResetColor();
            foreach (string[] dir in DirPaths.ExagoSubRootDirectories)
            {
                string dirName = dir[0];
                string dirPath1 = dir[1];
                string dirPath2 = dir[2];

                if (!Directory.Exists(dirPath1))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("DIR FAILURE ");
                    Console.ResetColor();
                    Console.WriteLine("Directory " + dirPath1 + " does NOT exist");
                    continue;
                }

                if (!Directory.Exists(dirPath2))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("DIR FAILURE ");
                    Console.ResetColor();
                    Console.WriteLine("Directory " + dirPath2 + " does NOT exist");
                    continue;
                }
            }
        }
    }
}
