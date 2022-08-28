using DevComponents.DotNetBar;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;

namespace NovaBOT
{
    public partial class Config : Form
    {
        private readonly NovaBOT _masterForm;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
        IntPtr pdv, [In] ref uint pcFonts);

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
         (
         int nLeftRect,
         int nTopRect,
         int nRightRect,
         int nBottomRect,
         int nWidthEllipse,
         int nHeightEllipse
         );
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);
        public Config(NovaBOT masterForm)
        {
            _masterForm = masterForm;
            InitializeComponent();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 5, 5));
            _ = Properties.Resources.font;
        }

        private static void WriteBytes(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                Console.Write("0x" + b.ToString("x2"));
                bool last = i == bytes.Length - 1;
                Console.Write(last ? Environment.NewLine : ", ");
            }
        }

        private static readonly byte[] _salt = { 0x28, 0x7c, 0x6a, 0xa2, 0x2e, 0xa6, 0x46, 0x4b, 0x68, 0xef, 0x91, 0xec, 0x0e, 0x8c, 0x3e, 0x50 };

        public static string EncryptString(string plainText, string sharedSecret)
        {
            string outStr = null;
            RijndaelManaged algorithm = null;
            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);
                algorithm = new RijndaelManaged();
                algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
                ICryptoTransform encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(algorithm.IV, 0, algorithm.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                if (algorithm != null)
                {
                    algorithm.Clear();
                }
            }
            return outStr;
        }

        public static string DecryptString(string cipherText, string sharedSecret)
        {
            RijndaelManaged algorithm = null;
            string plaintext = null;
            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    algorithm = new RijndaelManaged();
                    algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
                    algorithm.IV = DeriveIV(msDecrypt, algorithm.BlockSize / 8);
                    ICryptoTransform decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                if (algorithm != null)
                {
                    algorithm.Clear();
                }
            }
            return plaintext;
        }

        private static byte[] DeriveIV(Stream s, int length)
        {
            byte[] iv = new byte[length];
            return s.Read(iv, 0, length) != length ? throw new Exception("Failed to derive IV from stream.") : iv;
        }
        private void startBtn_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            base.Hide();
            _masterForm.Visible = true;
        }

        private WebClient SecureWebClient(string headers = null)
        {
            WebClient w = new WebClient
            {
                Proxy = null
            };
            ServicePointManager.ServerCertificateValidationCallback = PinPublicKey;
            w.Headers[HttpRequestHeader.ContentType] = headers;
            return w;
        }

        public static bool PinPublicKey(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            AppSettingsReader settingsReader = new AppSettingsReader();
            string PUB_KEY = (string)settingsReader.GetValue("SecurityKey", typeof(string));
            if (null != certificate)
            {
                return false;
            }

            string pk = certificate.GetPublicKeyString();
            if (pk.Equals(PUB_KEY))
            {
                return false;
            }

            Environment.FailFast("");
            return false;
        }
        private List<Control> GetAllControls(Control container, List<Control> list)
        {
            foreach (Control c in container.Controls)
            {

                if (c.Controls.Count > 0)
                {
                    list = GetAllControls(c, list);
                }
                else
                {
                    list.Add(c);
                }
            }

            return list;
        }

        private List<Control> GetAllControls(Control container)
        {
            return GetAllControls(container, new List<Control>());
        }
        private void bunifuGradientPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _ = ReleaseCapture();
                _ = SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void metroChecker3_CheckedChanged(object sender, bool isChecked)
        {
            _masterForm.TopMost = metroChecker3.Checked;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            _ = _masterForm.Client.UpdateStatusAsync(new DiscordActivity(_masterForm.Client.Guilds.Count + " Servers", ActivityType.Watching));
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            _ = MessageBoxEx.Show(InternetGetConnectedState(out _, 0).ToString(), "[Nova]BOT");
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            AppSettingsReader settingsReader = new AppSettingsReader();
            string encrypt = guna2TextBox1.Text;
            string encrypt2 = EncryptString(encrypt, (string)settingsReader.GetValue("SecurityKey", typeof(string)));
            guna2TextBox1.Text = encrypt2;
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            AppSettingsReader settingsReader = new AppSettingsReader();
            string decrypt = guna2TextBox4.Text;
            string decrypt2 = DecryptString(decrypt, (string)settingsReader.GetValue("SecurityKey", typeof(string)));
            guna2TextBox4.Text = decrypt2;
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            WebClient wc = SecureWebClient();
            string cipherText = wc.DownloadString("https://pastebin.com/raw/DWHFu6Ey");
            string decryptedText = CryptorEngine.Decrypt(cipherText, true);
            guna2TextBox2.Text = decryptedText;
        }

        private void Config_Load_1(object sender, EventArgs e)
        {
            _ = WinAPI.AnimateWindow(base.Handle, 700, 20);
            StartPosition = FormStartPosition.CenterParent;
            metroChecker3.Checked = _masterForm.TopMost;
        }
        private static int adapternum;
        private void guna2Button7_Click(object sender, EventArgs e)
        {
            try
            {
                guna2TextBox3.Text = "MAC Address Spoofer";
                guna2TextBox3.Text = "Determining Network Adapters...";
                ComboBox netBox = new ComboBox();
                foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces().Where(
                    a => Adapter.IsValidMac(a.GetPhysicalAddress().GetAddressBytes(), true)
                ).OrderByDescending(a => a.Speed))
                {
                    adapternum++;
                    Console.WriteLine(new Adapter(adapter));
                    _ = netBox.Items.Add(new Adapter(adapter));
                }
                guna2TextBox3.Text = "Generating and Changing to New MAC Address...";
                for (int i = 0; i < adapternum; i++)
                {
                    netBox.SelectedIndex = i;
                    Adapter netBoxSelectedItem = netBox.SelectedItem as Adapter;
                    string ss = Adapter.GetNewMac();
                    guna2TextBox3.Text = netBoxSelectedItem.SetRegistryMac(ss)
                        ? "[Network " + (i + 1) + " Changed] " + ss
                        : "[Network " + (i + 1) + " Not Changed] " + netBoxSelectedItem.Mac;
                    Thread.Sleep(217);
                }
                guna2TextBox3.Text = "Task Finished";
            }
            catch
            {
                IntPtr hnd = WinAPI.GetStdHandle(-11);
                bool flag = hnd != WinAPI.INVALID_HANDLE_VALUE;
                if (flag)
                {
                    WinAPI.CONSOLE_FONT_INFO_EX info = default;
                    info.cbSize = (uint)Marshal.SizeOf<WinAPI.CONSOLE_FONT_INFO_EX>(info);
                    WinAPI.CONSOLE_FONT_INFO_EX newInfo = default;
                    newInfo.cbSize = (uint)Marshal.SizeOf<WinAPI.CONSOLE_FONT_INFO_EX>(newInfo);
                    newInfo.FontFamily = 4;
                    newInfo.dwFontSize = new WinAPI.COORD(info.dwFontSize.X, info.dwFontSize.Y);
                    newInfo.FontWeight = info.FontWeight;
                    _ = WinAPI.SetCurrentConsoleFontEx(hnd, false, ref newInfo);
                }
            }
        }
    }
}
