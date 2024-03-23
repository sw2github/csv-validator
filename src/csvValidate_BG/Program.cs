using System.ComponentModel.DataAnnotations;

namespace csvValidate_BG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            int iReturn;

            if (args.Length > 0)
            {
                Console.WriteLine("args[0]:" + args[0]);
            }

            if (args.Length == 2)
            {
                Console.WriteLine("args[0]-csv-Datei:" + args[0]);
                Console.WriteLine("args[1]-Prüfvorschrift:" + args[1]);

                iReturn = SplitCsvFile(args[0]);
                Console.WriteLine("splitCsvFile:" + iReturn.ToString());
                if (iReturn != 0)
                {
                    throw new Exception("Fehler: " + iReturn.ToString());
                }

                iReturn = ValidateFiles(args[0], args[1]);
                Console.WriteLine("splitCsvFile:" + iReturn.ToString());
                if (iReturn != 0)
                {
                    throw new Exception("Fehler: " + iReturn.ToString());
                }
            }
        }

        static int ValidateFiles(string sFileName, string sCheckFileName)
        {
            int iReturn = 0;

            Console.WriteLine(" >>> prüfe Kopfzeile >>> ");
            string sCvsFileK = Path.GetDirectoryName(sFileName) + "\\K.tmp";
            iReturn += ValidateFile(sCvsFileK, sCheckFileName + "K.json");

            Console.WriteLine(" >>> prüfe Infozeile >>> ");
            string sCvsFileI = Path.GetDirectoryName(sFileName) + "\\I.tmp";
            iReturn += ValidateFile(sCvsFileI, sCheckFileName + "I.json");

            Console.WriteLine(" >>> prüfe Artikel >>> ");
            string sCvsFileA = Path.GetDirectoryName(sFileName) + "\\A.tmp";
            iReturn += ValidateFile(sCvsFileA, sCheckFileName + "A.json");

            //Console.WriteLine("Ergebnis: " + sOutput);
            //Console.WriteLine("Fehler: " + sError);

            return iReturn;

        }

        static int ValidateFile(string sFilename, string sCheckFileName)
        {
            System.Diagnostics.Process pValidateProcess = System.Diagnostics.Process.Start("Validate.exe", "-f " + sFilename + " -w " + sCheckFileName);
            pValidateProcess.StartInfo.UseShellExecute = false;
            pValidateProcess.StartInfo.CreateNoWindow = true;
            pValidateProcess.StartInfo.RedirectStandardOutput = true;
            pValidateProcess.StartInfo.RedirectStandardError = true;

            pValidateProcess.Start();
            pValidateProcess.WaitForExit();

            string sOutput = pValidateProcess.StandardOutput.ReadToEnd();
            string sError = pValidateProcess.StandardError.ReadToEnd();
            return pValidateProcess.ExitCode;
        }

        static int SplitCsvFile(string sFileName)
            {
            String line;
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                //DEBUG Console.WriteLine("Zieldatei:" + Path.GetDirectoryName(sFileName) + "\\K.tmp");
                StreamWriter swKopf    = new StreamWriter(Path.GetDirectoryName(sFileName) + "\\K.tmp");
                StreamWriter swInfo    = new StreamWriter(Path.GetDirectoryName(sFileName) + "\\I.tmp");
                StreamWriter swArtikel = new StreamWriter(Path.GetDirectoryName(sFileName) + "\\A.tmp");
                swKopf.WriteLine("############  Kopf  ############");
                swInfo.WriteLine("############  Info  ############");
                swArtikel.WriteLine("############  Artikel  ############");

                int CountKopf = 0;
                int CountInfo = 0;

                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(sFileName);
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    //write the line to console window
                    Console.WriteLine(line);
                    if (line[0] == 'K')
                    {
                        //Kopf
                        // DEBUG Console.WriteLine("Kopfzeiel:" + line);
                        swKopf.WriteLine(line);
                        CountKopf++;
                        }
                    if (line[0] == 'I')
                    {
                        //Info
                        swInfo.WriteLine(line);
                        CountInfo++;
                    }
                    if (line[0] == 'A')
                    {
                        //Artikel
                        swArtikel.WriteLine(line);
                    }

                    //Read the next line
                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
                swKopf.Close();
                swInfo.Close();
                swArtikel.Close();

                if ((CountInfo > 1) || (CountKopf > 1))
                {
                    Console.WriteLine("FEHLER: zu viele Kopf oder Infozeilen");
                    return 100;
                }



            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                // Console.WriteLine("Executing finally block.");
            }
            return 0;
        }
    }
}
