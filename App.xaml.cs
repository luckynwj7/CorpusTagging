using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CorpusTagging
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private static FileSelectWindow fileSelectWin;
        public static FileSelectWindow FileSelectWin
        {
            get { return fileSelectWin; }
        }
        private static TaggingJobWindow taggingJobWin;
        public static TaggingJobWindow TaggingJobWin
        {
            get { return taggingJobWin; }
            set { taggingJobWin = value; }
        }
        private static InputTextWindow inputTextWin;
        public static InputTextWindow InputTextWin
        {
            get { return inputTextWin; }
            set { inputTextWin = value; }
        }
        private static TextChangeWindow textChangeWin;
        public static TextChangeWindow TextChangeWin
        {
            get { return textChangeWin; }
            set { textChangeWin = value; }
        }
        private static TextInsertWindow textInsertWin;
        public static TextInsertWindow TextInsertWin
        {
            get { return textInsertWin; }
            set { textInsertWin = value; }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            fileSelectWin = FileSelectWindow.FileSelectWin;
            fileSelectWin.Show();
        }
    }
}
