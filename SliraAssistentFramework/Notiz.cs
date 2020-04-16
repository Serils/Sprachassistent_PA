using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace SliraAssistentFramework
{
    class Notiz
    {
        SpeechSynthesizer speech = new SpeechSynthesizer();
        

        private string titel;
        private string notiz;
        private string path;
        public void addText()
        {


            //path = @"C:\Users\Nils Hauser\source\Desktop\Notizen\" + getTitel() + ".txt";
            //sollte im Notizen Ordner im SliraAssistentFramework gespeichert werden
            path = @"\Notizen" + getTitel() + ".txt";
            try
            {
                using (FileStream fs = new FileStream(path,FileMode.Create))
                {
                    //created das File oder überschreibt es wenn es schon existiert
                    byte[] info = new UTF8Encoding(true).GetBytes(getNotiz());
                    fs.Write(info, 0, info.Length);
                }


            }
            catch (Exception)
            {

                Console.WriteLine("Fehler beim erstellen einer Notiz");
            }
        }

        public void readNotiz()
        {
            speech.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);
            speech.SpeakAsync(getNotiz());
        }

        public void setTitel(string pTitel)
        {
            titel = pTitel;
        }
        public void setNotiz(string pNotiz)
        {
            notiz = pNotiz;
        }
        public string getTitel()
        {
            return titel;
        }
        public string getNotiz()
        {
            return notiz;
        }
    }
}
