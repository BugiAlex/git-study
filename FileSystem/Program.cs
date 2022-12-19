using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FileSystem
{
    class Program
    {
       static int countFile = 0;
       
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

            bool a;
            string startDate = "";
            string finishDate = "";
            do
            {
                Console.Write("Введите начальную дату (в формате \"гггг/мм/дд\") диапазона для поиска а файлах: ");
                startDate = Console.ReadLine();

                string pattern = @"^([0-9]){1,4}/(([0]?[1-9])|([1][0-2]))/(([0]?[1-9])|([12][0-9])|([3][01]))$";
                Regex regex = new Regex(pattern);

                if (regex.IsMatch(startDate))
                {
                    a = false;
                }
                else
                {
                    a = true;
                    Console.WriteLine("Вы ввели некоректную дату. Попробуйте снова.");
                }
            } while (a);

            do
            {
                Console.Write("Введите конечную дату (в формате \"гггг/мм/дд\") диапазона для поиска а файлах: ");
                finishDate = Console.ReadLine();

                string pattern = @"^([0-9]){1,4}/(([0]?[1-9])|([1][0-2]))/(([0]?[1-9])|([12][0-9])|([3][01]))$";
                Regex regex = new Regex(pattern);

                if (regex.IsMatch(finishDate))
                {
                    a = false;
                }
                else
                {
                    a = true;
                    Console.WriteLine("Вы ввели некоректную дату. Попробуйте снова.");
                }
            } while (a);

            DateTime startD = Convert.ToDateTime(startDate);
            DateTime finishD = Convert.ToDateTime(finishDate);

            Regex regMask = new Regex(Mask, RegexOptions.IgnoreCase);

            StreamWriter swClean = new StreamWriter("ReportFile.txt", false);
            swClean.Close();

            try
            {             
                ulong Count = FindFilesData(di, regMask, startD, finishD);
                Console.WriteLine("Всего обработано файлов: {0}.", Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static ulong FindFilesData(DirectoryInfo di, Regex regMask, DateTime startD, DateTime finishD)
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
                if (regMask.IsMatch(f.Name) && f.LastWriteTime>=startD && f.LastWriteTime<=finishD)
                {
                    ++countFile;
                    StreamWriter sw = new StreamWriter("ReportFile.txt", true);
                    ++CountOfMatchFiles;
                    sw.WriteLine(countFile + "\t"+f.LastWriteTime+"\t"+ f.Name);
                    sw.Close();                 
                }
            }
            DirectoryInfo[] diSub = di.GetDirectories();
            foreach (DirectoryInfo diSubDir in diSub)
            CountOfMatchFiles += FindFilesData(diSubDir, regMask, startD, finishD);
            return CountOfMatchFiles;
        }
    }
}


