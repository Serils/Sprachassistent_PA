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
