using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AA.Forms.Logging
{
    public class RichTextBoxTraceListener : TraceListener
    {
        private RichTextBox _targetRichTextBox;
        private Control _target;
        private StringSendDelegate _invokeWrite;

        public bool ShowDate { get; set; } = false;

        public RichTextBoxTraceListener(Control target)
        {
            _target = target;
            _invokeWrite = new StringSendDelegate(SendString);
            if (target is RichTextBox)
                _targetRichTextBox = target as RichTextBox;
            else
                throw new Exception("Control is not RichTextBox controller");
        }



        public override void Write(string message)
        {
            if (_target.IsHandleCreated)
                _target.Invoke(_invokeWrite, new object[] { message });
        }

        public override void WriteLine(string message)
        {
            if (_target.IsHandleCreated)
                _target.Invoke(_invokeWrite, new object[] { message + Environment.NewLine });
        }

        private delegate void StringSendDelegate(string message);

        private void SendString(string message)
        {
            if (ShowDate)
                message = DateTime.Now.ToString("hh:mm:ss") + "  " + message;

            // No need to lock text box as this function will only 
            // ever be executed from the UI thread
            if (_target is RichTextBox)
            {
                Color c = Color.Black;
                if (message.StartsWith("Info"))
                    c = Color.Green;
                else if (message.StartsWith("Error"))
                    c = Color.Red;

                _targetRichTextBox.SelectedText = string.Empty;
                _targetRichTextBox.SelectionFont = new Font(_targetRichTextBox.SelectionFont, FontStyle.Regular);
                _targetRichTextBox.SelectionColor = c;
                _targetRichTextBox.AppendText(message);
                _targetRichTextBox.ScrollToCaret();
            }
            else
            {
                _target.Text += message;
            }

        }
    }
 }
