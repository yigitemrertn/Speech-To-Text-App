// ===================================
// Program.cs
// ===================================
using SpeechRecognition;
using System;
using System.Windows.Forms;

namespace SpeechRecognition
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}