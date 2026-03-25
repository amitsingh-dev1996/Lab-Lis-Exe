using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VSoftLIS_Interface.Common
{
    public class InterfaceUIHelper
    {
        public static void UpdateAppSettingValue(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection app = config.AppSettings;
            app.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
        }

        public static string OpenFileDialog(TextBox txt)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = ofd.CheckPathExists = true;
            ofd.DefaultExt = "";
            ofd.Multiselect = false;
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txt.Text = ofd.FileName;
                return ofd.FileName;
            }
            return "";
        }

        public static string OpenFolderDialog(TextBox txt)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dr = fbd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txt.Text = fbd.SelectedPath;
                return fbd.SelectedPath;
            }
            return "";
        }

        public static void DisableControlsCascaded(params Control[] controls)
        {
            foreach (Control control in controls)
            {
                foreach (Control childControl in control.Controls)
                {
                    DisableControlsCascaded(childControl);
                }
                control.Enabled = false;
            }
        }

        public static void DisableFormControls(Form form)
        {
            foreach (Control childControl in form.Controls)
            {
                DisableControlsCascaded(childControl);
            }
        }
    }
}
