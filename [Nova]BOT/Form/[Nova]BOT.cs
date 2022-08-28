using NovaBOT.Commands;
using NovaBOT.DiscordRPC;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NovaBOT
{
    public partial class NovaBOT : Form
    {
        public NovaBOT()
        {
            InitializeComponent();
            try
            {
                handlers = default;
                DiscordRpc.Initialize("your rpc id", ref handlers, true, null);
                handlers = default;
                DiscordRpc.Initialize("your rpc id", ref handlers, true, null);
                presence.details = "your rpc details";
                presence.state = "your rpc details";
                presence.largeImageKey = "logo";
                presence.smallImageKey = "logo";
                presence.largeImageText = "logo";
                DiscordRpc.UpdatePresence(ref presence);
            }
            catch { }
        }
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
        IntPtr pdv, [In] ref uint pcFonts);

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);

        private DiscordRpc.EventHandlers handlers;

        private DiscordRpc.RichPresence presence;

        public bool enabled;

        public bool internet;

        public Stopwatch sw = Stopwatch.StartNew();

        private async void startBtn_Click(object sender, EventArgs e)
        {
            if (startBtn.Checked == false)
            {
                sw.Stop();
                startBtn.Text = "Start Bot";
                pingBtn.Enabled = false;
                sendMsgBtn.Enabled = false;
                setBtn.Enabled = false;
                guna2Button5.Enabled = false;
                guna2Button3.Enabled = false;
                guna2Button2.Enabled = false;
                guna2Button4.Enabled = false;
                guna2Button6.Enabled = false;
                guna2Button8.Enabled = false;
                try { await Client.DisconnectAsync().ConfigureAwait(false); } catch { }
            }
            else
            {
                sw.Start();
                startBtn.Text = "Stop Bot";
                pingBtn.Enabled = true;
                sendMsgBtn.Enabled = true;
                setBtn.Enabled = true;
                guna2Button5.Enabled = true;
                guna2Button3.Enabled = true;
                guna2Button2.Enabled = true;
                guna2Button4.Enabled = true;
                guna2Button6.Enabled = true;
                guna2Button8.Enabled = true;
                await Task.Delay(500);
                await RunRoundBkpAsync();
            }
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

        protected void Form1_Load(object sender, EventArgs e)
        {
            Process process = Process.GetCurrentProcess();
            Process[] dupl = Process.GetProcessesByName(process.ProcessName);
            if (dupl.Length > 1)
            {
                foreach (Process p in dupl)
                {
                    if (p.Id != process.Id)
                    {
                        p.Kill();
                    }
                }
            }
            while (internet)
            {
                bool flag3 = !NovaBOT.InternetGetConnectedState(out int _, 0);
                if (flag3)
                {
                    enabled = false;
                }
                else
                {
                    bool flag4 = NovaBOT.InternetGetConnectedState(out _, 0);
                    if (flag4)
                    {
                        _ = Task.Delay(1000);
                        enabled = true;
                    }
                }
            }
            _ = WinAPI.AnimateWindow(base.Handle, 700, 20);
            Stopwatch time = new Stopwatch();
            time.Start();
            TextWriter _writer = new TextBoxStreamWriter(outputTB);
            Console.SetOut(_writer);
            Console.WriteLine("Welcome to [Nova]BOT! Console output will be redirected here unless disabled.");
            _ = new NotifyIcon
            {
                Visible = true,
                Icon = Icon,
                ContextMenuStrip = contextMenuStrip1
            };
            time.Stop();
            Console.WriteLine("Time Taken To Start: " + time.ElapsedMilliseconds.ToString() + "ms");
        }

        protected WebClient SecureWebClient()
        {
            WebClient w = new WebClient
            {
                Proxy = null
            };
            return w;
        }

        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public async Task RunRoundBkpAsync()
        {
            string json = string.Empty;

            using (FileStream fs = File.OpenRead("config.json"))
            using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
            {
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            }

            ConfigJson configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            DiscordConfiguration config = new DiscordConfiguration()
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LargeThreshold = 1000,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(config);
            Client.Ready += OnClientReady;
            _ = Client.UseInteractivity(new InteractivityConfiguration());


            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = true,
                IgnoreExtraArguments = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<MainCommands>();
            Commands.RegisterCommands<ModerationCommands>();
            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<RbxCommands>();

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private async void Ready()
        {
            await Task.Delay(1000);
            enabled = true;
            while (enabled)
            {
                await Task.Delay(6000);
                await Client.UpdateStatusAsync(new DiscordActivity(Client.Guilds.Count + " Servers", ActivityType.Watching));
                await Task.Delay(6000);
                await Client.UpdateStatusAsync(new DiscordActivity("?help", ActivityType.Playing), null, null);
                await Task.Delay(100);
            }
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            Ready();
            return null;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            outputTB.AppendText("Pong! | " + "Latency: " + Client.Ping.ToString() + "ms");
            outputTB.AppendText(Environment.NewLine);
        }


        private void sendMsgBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ulong channnelID = ulong.Parse(channelTB.Text);
                Task<DiscordChannel> channel = Client.GetChannelAsync(channnelID);
                _ = Client.SendMessageAsync(channel.Result, msgTB.Text);
            }
            catch
            {
                return;
            }
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            if (guna2Button1.Checked == false)
            {
                guna2Button1.Text = "Disable Output (Speeds Things Up)";
                TextWriter _writer = new TextBoxStreamWriter(outputTB);
                Console.SetOut(_writer);
                Console.WriteLine("Console output is now set to textbox.");
            }
            else
            {
                guna2Button1.Text = "Enable Output (Useful for debugging)";
                StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput())
                {
                    AutoFlush = true
                };
                Console.SetOut(standardOutput);
            }
        }

        private void pingBtn_Click(object sender, EventArgs e)
        {
            outputTB.AppendText("Pong! | " + "Latency: " + Client.Ping.ToString() + "ms");
            outputTB.AppendText(Environment.NewLine);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _ = Client.DisconnectAsync().ConfigureAwait(false);
                DiscordRpc.Shutdown();
            }
            catch
            {
                return;
            }
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Dispose();
        }

        private void setBtn_Click(object sender, EventArgs e)
        {
            _ = Client.UpdateStatusAsync(new DiscordActivity(statusTB.Text, ActivityType.Playing));
        }
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            string cipherText = guna2TextBox4.Text.Trim();
            string decryptedText = CryptorEngine.Decrypt(cipherText, true);
            guna2TextBox4.Text = decryptedText;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ulong channnelID = ulong.Parse(guna2TextBox1.Text);
            Task<DiscordChannel> channel = Client.GetChannelAsync(channnelID);
            for (int i = 0; i < 5; i++)
            {
                _ = Client.SendMessageAsync(channel.Result, guna2TextBox2.Text);
                _ = Task.Delay(1000);
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(guna2TextBox4.Text);
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            ProcessStartInfo proc = new ProcessStartInfo("ipconfig", "/renew")
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            _ = Process.Start(proc);
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            string cipherText = guna2TextBox3.Text.Trim();
            string encrypt = CryptorEngine.Encrypt(cipherText, true);
            guna2TextBox3.Text = encrypt;
        }
        private void guna2Button8_Click(object sender, EventArgs e)
        {
            Form Config = Application.OpenForms["Config"];
            if (Config == null)
            {
                Config = new Config(this);
                Config.Show();
            }
            else
            {
                Config.Close();
                Config = new Config(this);
                Config.Show();
            }
            base.Visible = false;
        }

        private void restardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Dispose();
            try { _ = Client.DisconnectAsync().ConfigureAwait(false); } catch { }
            _ = Process.Start(Application.ExecutablePath);
            Process.GetCurrentProcess().Kill();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Dispose();
            try { _ = Client.DisconnectAsync().ConfigureAwait(false); } catch { }
            Process.GetCurrentProcess().Kill();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
        }

        private void bunifuGradientPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _ = ReleaseCapture();
                _ = SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void guna2ControlBox2_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (base.Opacity > 0.0)
            {
                base.Opacity -= 0.025;
                return;
            }
            timer1.Stop();
            base.Close();
        }
    }
}

