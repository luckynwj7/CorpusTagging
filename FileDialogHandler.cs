using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorpusTagging
{
    public static class FileDialogHandler
    {
        public static string FindFileDialogStr(string fileFormat)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            if (fileFormat == "txt")
            {
                dlg.DefaultExt = ".txt";
                dlg.Filter = "텍스트 파일 (*.txt)|*.txt|csv 파일 (*.csv)|*.csv|모든 파일|*";
            }
            else if(fileFormat == "csv")
            {
                dlg.DefaultExt = ".csv";
                dlg.Filter = "csv 파일 (*.csv)|*.csv|모든 파일|*";
            }

            // Display OpenFileDialog by calling ShowDialog method 
            object result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result != null)
            {
                // Open document 
                return dlg.FileName;
            }
            else
            {
                return "";
            }

            // 출처 https://tanklove.tistory.com/147
        }

        public static string FindDirectoryDialogStr()
        {
            FolderBrowserDialog ofd = new System.Windows.Forms.FolderBrowserDialog();
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                return ofd.SelectedPath;
            }
            else if (dr == DialogResult.Cancel)
            {
                return "";
            }
            return "";
        }
    }
}
