using System;
using System.Windows.Forms;
namespace NovaBOT
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new NovaBOT());
        }
    }
}