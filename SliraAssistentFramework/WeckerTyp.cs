using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SliraAssistentFramework
{
    class WeckerTyp
    {
        //Attribute
        private Time wakeUpTime = new Time();
        private Boolean notizUsed = false;
        private string notiz = "";

        public Boolean getNotizUsed() { return notizUsed; }
        public string getNotiz() { return notiz; }
        public void setNotizUsed(Boolean pUsed) 
        {
            notizUsed = pUsed;
        }
        public void setNotiz(string pNotiz)
        {
            notiz = pNotiz;
        }
    }
}
