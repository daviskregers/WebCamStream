using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebEye.Controls.WinForms.WebCameraControl;
using System.Windows.Forms;
using System.Drawing;

namespace WebcamStream
{

    public class Webcam
    {

        WebCameraControl webCameraControl1;
        ComboBox comboBox1;
        TextBox output;

        public Webcam(
            WebCameraControl WebCameraControl1,
            ComboBox comboBox1,
            TextBox output
        ) {

            this.comboBox1 = comboBox1;
            this.webCameraControl1 = WebCameraControl1;
            this.output = output;

        }

        private class ComboBoxItem
        {
            public ComboBoxItem(WebCameraId id)
            {
                _id = id;
            }

            private readonly WebCameraId _id;
            public WebCameraId Id
            {
                get { return _id; }
            }

            public override string ToString()
            {
                // Generates the text shown in the combo box.
                return _id.Name;
            }
        }

        public void changeSource()
        {

            try
            {
                ComboBoxItem i = (ComboBoxItem)comboBox1.SelectedItem;
                webCameraControl1.StartCapture(i.Id);
            }
            catch( Exception e )
            {
                output.AppendText("Error starting capture " + e.Message + "\n");
            }
        }

        public Bitmap GetCurrentImage()
        {
            return webCameraControl1.GetCurrentImage();
        }

        public void loadSources()
        {

            foreach (WebCameraId camera in webCameraControl1.GetVideoCaptureDevices())
            {
                comboBox1.Items.Add(new ComboBoxItem(camera));
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedItem = comboBox1.Items[0];
            }

        }

    }
}
