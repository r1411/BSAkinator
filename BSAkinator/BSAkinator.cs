using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace BSAkinator
{
    internal static class BSAkinator
    {
        private static Process swiplProcess;

        public static StreamWriter GetPrologInput()
        {
            return swiplProcess.StandardInput;
        }

        public static StreamReader GetPrologOutput()
        {
            return swiplProcess.StandardOutput;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            string exePath = "C:/Program Files/swipl/bin/swipl.exe";
            if (!File.Exists(exePath))
            {
                MessageBox.Show("SWI-Prolog не найден. Укажите путь до swipl.exe", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Исполняемые файлы|*.exe";
                if(dialog.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }
                exePath = dialog.FileName;
            }
            swiplProcess = new Process();
            swiplProcess.StartInfo = new ProcessStartInfo(exePath);
            swiplProcess.StartInfo.Arguments = "-q -l bsa.pl";
            swiplProcess.StartInfo.UseShellExecute = false;
            swiplProcess.StartInfo.RedirectStandardInput = true;
            swiplProcess.StartInfo.RedirectStandardOutput = true;
            swiplProcess.StartInfo.CreateNoWindow = true;
            swiplProcess.Start();

            GetPrologInput().WriteLine("init_db.");
            GetPrologOutput().ReadLine();
            GetPrologOutput().ReadLine();
            SoundPlayer simpleSound = new SoundPlayer(Properties.Resources.supercell_jingle);
            simpleSound.Play();
            Thread.Sleep(2000);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
