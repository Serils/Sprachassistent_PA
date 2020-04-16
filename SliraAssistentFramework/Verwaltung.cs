using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace SliraAssistentFramework
{
    class Verwaltung
    {
        private Notiz errorNotiz = new Notiz();
        private Notiz[] notizVerwaltung = new Notiz[30];
        private int notizIndex = 1;
        //List Class muss noch gemacht werden
        private List wecker = new List();


        public void addNotiz(Notiz pNotiz) 
        {
            if(notizIndex == 30)
            {

            }
            else
            {
                notizVerwaltung[notizIndex] = pNotiz;
                notizIndex++;
            }
        }

        public Notiz getNotizByTitel(string pTitel) 
        {
            for(int i = 1; i <= notizIndex; i++)
            {
                if(pTitel.Equals(notizVerwaltung[i].getTitel()))
                {
                    return notizVerwaltung[i];
                }
            }
            return errorNotiz;
        }

        public Notiz getErrorNotiz()
        {
            return errorNotiz;
        }

        public Boolean weckerExists()
        {
            if (wecker.Count != 0)
            {
                return true;
            }
            else { return false; }
        }

        public List getWeckerList()
        {
            return wecker;
        }

        //test Methode
        public Boolean weckerActive(Time pTimenow) { return true; }
        public WeckerTyp testWecker = new WeckerTyp();
        public WeckerTyp searchActiveWecker(Time pTimenow) { return testWecker; }
    }
}
