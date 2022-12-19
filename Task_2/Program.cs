using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace FileSystem
{
    class Program
    {       
        static List<string> namesFile = new List<string>();
        static List<string> pathFile = new List<string>();
        static void Main()
        {

            Console.Write("Введите путь к каталогу: ");
            string Path = Console.ReadLine();

            if (Path[Path.Length - 1] != '\\')
                Path += '\\';

            DirectoryInfo di = new DirectoryInfo(Path);

            if (!di.Exists)
            {
                Console.WriteLine("Некорректный путь!!!");
                return;
            }
            Console.Write("Введите маску для файлов: ");
            string Mask = Console.ReadLine();

            Mask = Mask.Replace(".", @"\.");
            Mask = Mask.Replace("?", ".");
            Mask = Mask.Replace("*", ".*");
            Mask = "^" + Mask + "$";

            Regex regMask = new Regex(Mask, RegexOptions.IgnoreCase);

            StreamWriter swClean = new StreamWriter("ReportFile.txt", false);
            swClean.Close();

            try
            {
                ulong Count = FindFilesData(di, regMask);
                Console.WriteLine("Всего обработано файлов: {0}.", Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            PrintList();
            DeleteFile();
        }

        static void PrintList()
        {
            int count = 0;
            foreach (string str in namesFile)
            {
                Console.WriteLine(count+". " + str);
                ++count;
            }
        }

        static void DeleteFile()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("1. Удалить указаный файл \n2. Удалить все найденые файлы \n3. Удалить диапазон из найденых файлов");
            Console.Write("Выберите метод удаления файлов: ");
            int choice = Convert.ToInt32( Console.ReadLine());

            switch (choice)
            {
                case (1):
                    { 
                    Console.Write("Введите индекс файла который Вы хотите удалить: ");
                    int indDelete = Convert.ToInt32(Console.ReadLine());
                    FileInfo tmp = new FileInfo(pathFile[indDelete]);
                    tmp.Delete();
                    }
                    break;
                case (2):
                    for (int i = 0; i < namesFile.Count; i++)
                    {
                    FileInfo tmp = new FileInfo(pathFile[i]);
                    tmp.Delete(); 
                    }
                    break;
                case (3):
                    {   
                        Console.Write("Введите начальный индекс диапазона для удаления: ");
                        int indDeleteStart = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Введите конечный индекс диапазона для удаления: ");
                        int indDeleteFinish = Convert.ToInt32(Console.ReadLine());
                        for (int i = indDeleteStart; i <= indDeleteFinish; i++)
                        {
                            FileInfo tmp = new FileInfo(pathFile[i]);
                            tmp.Delete();
                        }
                    }
                    break;
                default: Console.WriteLine("Вы ввели не коректный индекс!");
                    break;

            }

        }
        static ulong FindFilesData(DirectoryInfo di, Regex regMask)
        {
            ulong CountOfMatchFiles = 0;
                     
            FileInfo[] fi = null;
            try
            {
                fi = di.GetFiles();
            }
            catch
            {
                return CountOfMatchFiles;
            }

            foreach (FileInfo f in fi)
            {
                if (regMask.IsMatch(f.Name))
                {
                    ++CountOfMatchFiles;
                    namesFile.Add(f.Name);
                    pathFile.Add(f.FullName);
                }
            }
            DirectoryInfo[] diSub = di.GetDirectories();
           
            foreach (DirectoryInfo diSubDir in diSub)
                CountOfMatchFiles += FindFilesData(diSubDir, regMask);
            return CountOfMatchFiles;
        }
    }
}


