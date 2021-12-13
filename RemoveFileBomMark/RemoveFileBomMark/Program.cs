using System;
using System.IO;
using System.Text;

namespace RemoveFileBomMark {
    class Program {
            public static void Main() {
                //tips
                Console.WriteLine("Folder path format example : D:/File/Path");
                //源文件夹
                Console.WriteLine("Please enter the folder you need to find :");
                string searchDir = Console.ReadLine();
                Console.WriteLine("Replace the original path?(y/n)");
                string replace = Console.ReadLine();
                bool isReplace = replace == "y";
                string tarDir = searchDir;
                if (!isReplace) {
                    //复制的目标文件夹
                    Console.WriteLine("Please enter the destination folder :");
                    tarDir = Console.ReadLine();
                }

                //查找的文件类型
                Console.WriteLine("Please enter the file suffix :");
                string fileType = Console.ReadLine();
                try {
                    if (searchDir != null) {
                        ListFiles(new DirectoryInfo(searchDir), searchDir, tarDir, fileType);
                    }
                } catch (IOException e) {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine("Task finished");
                Console.ReadLine();
            }
            private static Encoding GetEncoding(string filePath) {
                using (var reader = new StreamReader(filePath, Encoding.Default, true)) {
                    if (reader.Peek() >= 0) // you need this!
                        reader.Read();

                    Encoding encoding = reader.CurrentEncoding;
                    reader.Close();
                    return encoding;
                }
            }
            public static void ListFiles(FileSystemInfo info, string searchDir, string tarDir, string fileType) {
                if (!info.Exists) {
                    Console.WriteLine(info.FullName + " is not exist");
                    return;
                }
                DirectoryInfo dir = info as DirectoryInfo;
                //不是目录
                if (dir == null) {
                    Console.WriteLine(dir + "is not a directory");
                    return;
                }
                FileSystemInfo[] files = dir.GetFileSystemInfos();
                for (int i = 0; i < files.Length; i++) {
                    FileInfo file = files[i] as FileInfo;
                    if (file != null) {
                        if (file.Extension == "." + fileType) {
                            string dirPath = file.Directory.ToString();
                            string searchDirConv = searchDir.Replace("/", "\\");
                            string tarDirConv = tarDir.Replace("/", "\\");
                            string newDirPath = dirPath.Replace(searchDirConv, tarDirConv);
                            string filePath = file.FullName.ToString();
                            //创建文件夹
                            if (new DirectoryInfo(newDirPath).Exists == false) {
                                Console.WriteLine("create floder : " + newDirPath);
                                Directory.CreateDirectory(newDirPath);
                            }
                            string newFilePath = filePath.Replace(searchDirConv, tarDirConv);
                            string tips = filePath == newFilePath ? "replaced" : "copied";
                            Console.WriteLine("file has been " + tips + " to path : " + newFilePath);
                            //Encoding oldEncoding = GetEncoding(file.FullName.ToString());
                            //读取文件内容
                            string str = string.Empty;
                            using (StreamReader sr = new StreamReader(filePath, new UTF8Encoding())) {
                                str = sr.ReadToEnd();
                                sr.Close();
                            }
                            //以UTF-8 NO-BOM格式重新写入文件
                            Encoding newEncoding = new UTF8Encoding(false);
                            using (StreamWriter sw = new StreamWriter(newFilePath, false, newEncoding)) {
                                sw.Write(str);
                                //Console.WriteLine(str);
                                sw.Close();
                            }
                        }
                    } else { //子目录递归查找
                        ListFiles(files[i], searchDir, tarDir, fileType);
                    }
                }
            }
        }
}
