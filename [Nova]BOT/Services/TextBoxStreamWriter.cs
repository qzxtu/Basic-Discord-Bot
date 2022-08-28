using Guna.UI2.WinForms;
using System;
using System.IO;
using System.Text;

namespace NovaBOT
{
    public class TextBoxStreamWriter : TextWriter
    {
        private readonly Guna2TextBox _output = null;

        public TextBoxStreamWriter(Guna2TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            _ = _output.BeginInvoke(new Action(() =>
            {
                _output.AppendText(value.ToString());
            })
            );
        }

        public override Encoding Encoding => Encoding.UTF8;
    }
}
