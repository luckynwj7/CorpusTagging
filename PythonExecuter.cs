using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CorpusTagging
{
    public static class PythonExecuter
    {
        public static void KreanSentenceSplit(string inputText)
        {
            var engine = IronPython.Hosting.Python.CreateEngine();
            var scope = engine.CreateScope();
            var paths = engine.GetSearchPaths();

            try
            {
                // 파이썬 모듈 참고 경로 추가
                paths.Add(@"C:\\Python38");
                paths.Add(@"C:\\Python38\\DLLs");
                paths.Add(@"C:\\Python38\\Lib");
                paths.Add(@"C:\\Python38\\Lib\\site-packages");
                engine.SetSearchPaths(paths);
            }catch(Exception e)
            {
                MessageBox.Show("패스 추가 실패");
            }

            try
            {
                var source = engine.CreateScriptSourceFromFile(@"KoreanSentenceSpliter.py");
                source.Execute(scope);

                //var sum = scope.GetVariable<Func<string,dynamic>>("sentenceSplit");
                //Console.WriteLine("타입은"+sum(inputText).GetType());
            }
            catch(Exception ex)
            {
                Console.WriteLine("실패");
            }
        }
    }
}
