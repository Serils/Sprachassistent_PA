using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace SliraAssistentFramework
{
    class Verwaltung
    {

        //general methods
        public void loadFiles() {}

        //Notiz Attribute
        private Notiz errorNotiz = new Notiz();
        private Notiz[] notizVerwaltung = new Notiz[30];
        private int notizIndex = 1;
             //List Class muss noch gemacht werden
             //private List wecker = new List();
        private WeckerTyp[] wecker = new WeckerTyp[19];



       



        //Notiz Methoden
        public void saveNote(Notiz pNotiz)
        {


            //path = @"C:\Users\Nils Hauser\source\Desktop\Notizen\" + getTitel() + ".txt";
            //sollte im Notizen Ordner im SliraAssistentFramework gespeichert werden
            String path = @"\Notizen" + pNotiz.getTitel() + ".txt";
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    //created das File oder überschreibt es wenn es schon existiert
                    byte[] info = new UTF8Encoding(true).GetBytes(pNotiz.getNotiz());
                    fs.Write(info, 0, info.Length);
                }

            }
            catch (Exception)
            {

                Console.WriteLine("Fehler beim erstellen einer Notiz");
            }
        }
        public void addNote(Notiz pNotiz) 
        {
            if(notizIndex == 30)
            {

            }
            else
            {
                notizVerwaltung[notizIndex] = pNotiz;
                notizIndex++;
            }
            saveNote(pNotiz);
        }
        public Notiz getNoteByTitel(string pTitel) 
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
        public Notiz getErrorNote()
        {
            return errorNotiz;
        }


       //Wecker Methoden
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
