using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Timers;
using System.Diagnostics;
using System.Threading;
using System.Collections;


namespace SliraAssistentFramework
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {

        SpeechRecognitionEngine r = new SpeechRecognitionEngine();
        SpeechSynthesizer s = new SpeechSynthesizer();
        DictationGrammar dGrammar = new DictationGrammar();



        string[] commands = new string[] {"Notiz hinzufügen", "Hallo", "Okay Danke", "Notiz vorlesen", "Programm", "suche", "Ich brauche einen Wecker auf", "Wecker gehört", "Wecker aus", "Notiz Speicher" };



        //general Attributes
        private Verwaltung verwaltung = new Verwaltung();
        private string tempProgrammName = "";


        //Notiz
        private Boolean notizInput = false;
        private String tempNotiztext = "";
        private string tempTitel = "";
        private System.Timers.Timer timer;
        private Notiz notiz = new Notiz();


        //Internet search
        private String suchText = "";

        
        //Wecker Attribute
        private Boolean weckerRecognized = false;
        private string wakeTimeComplete = "";

        public MainWindow()
        {
            Thread wecker = new Thread(new ThreadStart(hintergrundWecker));
            verwaltung.getErrorNote().setTitel("Error");
            InitializeComponent();
            Thread main = new Thread(new ThreadStart(mainWindow_loaded));
            //Loaded += MainWindow_Loaded;
        }

        public void mainWindow_loaded() 
        {
            Loaded += MainWindow_Loaded;
        }
        


        


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
           
            dGrammar.Enabled = true;
            
            Choices vocabulary = new Choices();
            vocabulary.Add(commands);

            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(vocabulary);

            Grammar grammar = new Grammar(gBuilder);

            r.LoadGrammar(grammar);
            r.LoadGrammar(dGrammar);
            r.SetInputToDefaultAudioDevice();
            r.SpeechRecognized += R_SpeechRecognized1;

            r.RecognizeAsync(RecognizeMode.Multiple);
            s.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);
            verwaltung.loadFiles();
        }
        private void R_SpeechRecognized1(object sender, SpeechRecognizedEventArgs e)
        {
            add_tBoxt_erkannteSprache(e.Result.Text);
            switch (e.Result.Text)
            {
                case "Notiz hinzufügen":
                    setNotizInput(true);
                    createNotizText();
                    setNotizInput(true);
                    createNotizTitel();
                    createNotiz();
                    break;

                case "Notiz vorlesen":
                    setNotizInput(true);
                    createNotizTitel();
                    if (tempTitel.Equals(verwaltung.getNoteByTitel(tempTitel)))
                    {
                        s.SpeakAsync(verwaltung.getNoteByTitel(tempTitel).getNotiz());
                    }
                    else
                    {
                        s.SpeakAsync("Titel nicht gefunden");
                        s.SpeakAsync("Weiderholen sie den Befehl");
                    }
                    notizClear();
                    break;

                case "Programm":
                    setNotizInput(true);
                    programmName();
                    programmStarten();
                    setProgrammName("");
                    break;

                case "suche":
                    setNotizInput(true);
                    suchAnfrage();
                    suchen();
                    break;

                case "Ich brauche einen Wecker auf":
                    setNotizInput(true);
                    setWakeUpTime();
                    break;

                case "Wecker gehört":
                    setWeckerRecognized(true);
                    break;

                case "Wecker aus":
                    setWeckerRecognized(true);
                    break;


                case "Hallo":
                    //Uhrzeit abfragen und demnach eine Ausgabe
                    s.SpeakAsync("Guten Morgen");
                    break;

                case "Slira schließen":
                    Close();
                    break;
            }
        }


        //GUI Befehle

            //tBoxt_erkannteSprache
        public void set_tBoxt_erkannteSprache(String pText) 
        {
            tBoxt_erkannteSprache.Text = pText;
        }
        public void add_tBoxt_erkannteSprache(String pText) 
        {
            tBoxt_erkannteSprache.Text = tBoxt_erkannteSprache + pText;
        }
        public void clear_tBoxt_erkannteSprache() 
        {
            tBoxt_erkannteSprache.Text = "";
        }
        public String get_tBoxt_erkannteSprache() 
        {
            return tBoxt_erkannteSprache.Text;
        }

            //tBoxt_Ausgabe
        public void set_tBoxt_Ausgabe(String pText) 
        {
            tBoxt_Ausgabe.Text = pText;
        }
        public void add_tBoxt_Ausgabe(String pText) 
        {
            tBoxt_Ausgabe.Text = tBoxt_Ausgabe.Text + pText;
        }
        public void clear_tBoxt_Ausgabe() 
        {
            tBoxt_Ausgabe.Text = "";
        }
        public String get_tBoxt_Ausgabe() 
        {
            return tBoxt_Ausgabe.Text;
        }



        //Notiz Methoden

        /*
         * deklarieren und Initialisieren eines neuen Spracherkennungobjektes
         * Dafür zuständig, dass alles in die Notiz geladen wird solange notizInput == true
         */
        public void createNotizText() 
        
        
        {
            DictationGrammar tempGrammar = new DictationGrammar();
            using (
            SpeechRecognitionEngine recognizer=
                new SpeechRecognitionEngine(
                new System.Globalization.CultureInfo("de-DE")))

            {
                
                recognizer.LoadGrammar(tempGrammar);

                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized_Notiz);

                recognizer.SetInputToDefaultAudioDevice();

                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                while (notizInput) { }

            }
        }
        
        /*
         * deklarieren und Initialisieren eines neuen Spracherkennungobjektes
         * setzt einen Timer mit 5 Sekunden Laufzeit
         * In der Zeit kann der Titel ausgesprochen werden
         */
        public void createNotizTitel()
        {
            using (
            SpeechRecognitionEngine recognizer =
                new SpeechRecognitionEngine(
                new System.Globalization.CultureInfo("de-DE")))

            {
                recognizer.LoadGrammar(dGrammar);

                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized_Titel);

                recognizer.SetInputToDefaultAudioDevice();

                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                setTimer(5000);
                while (notizInput)
                {
                    
                }
            }
        }

        /*
         * gibt den gehörten Text für die Notizen weiter zum speichern
         * fügt den gehörten Text in die TextBox add_tBoxt_erkannteSprache hinzu
         */
        public void recognizer_SpeechRecognized_Notiz(object sender, SpeechRecognizedEventArgs e)
        {
            addTempNoteText(e.Result.Text);
            add_tBoxt_erkannteSprache(e.Result.Text);
        }

        /*
         * gibt den gehörten Text für den Titel einer Notiz weiter zum speichern
         * fügt den gehörten Text in die TextBox add_tBoxt_erkannteSprache hinzu
         */
        public void recognizer_SpeechRecognized_Titel(object sender, SpeechRecognizedEventArgs e)
        {
            setTempTitel(e.Result.Text);
            add_tBoxt_erkannteSprache(e.Result.Text);
        }
        public void setNotizInput(Boolean pNotizInput)
        {
            notizInput = pNotizInput;
        }

        /*
         * fügt den erhaltenen Text in die Notiz Variable ein
         * prüft den Inhalt auf den Speichern Befehl und setzt notizInput auf False falls der Befehl zum Speicher erkannt wurde
         */
        public void addTempNoteText(String pText)
        {
            tempNotiztext = tempNotiztext + pText;
            
            if (tempNotiztext.Contains("Notiz Speichern"))
            {
                setNotizInput(false);
            }

        }
        

        public void setTempTitel(String pText)
        {
            tempTitel = tempTitel + pText;
        }
        
        /*
         * Deklariert und initialisiert ein Timerobjekt mit der erhaltenen Zeit pTime
         * das OnTimeEvent, wenn die erhaltene Zeit vorbei ist, setzt die Variable notizInput auf false
         */
        public void setTimer(int pTime)
        {
            timer = new System.Timers.Timer(pTime);
            timer.Elapsed += OnTimeEvent;
            timer.AutoReset = false;
            timer.Enabled = true;
        }
        public void OnTimeEvent(object source, ElapsedEventArgs e)
        {
            setNotizInput(false);
        }
        
        /*
         * speichert den vorher gehörten Titel und den Notiztext in einem Notiz Objekt
         * übergiebt dieses an die Verwaltung zum Speichern
         * ruft die Methode notizClear() auf, um die dummy Notiz "leer" zu machen
         */
        public void createNotiz()
        {
            notiz.setTitel(tempTitel);
            notiz.setNotiz(tempNotiztext);
            verwaltung.addNote(notiz);
            notizClear();
        }

        /*
         * "leeren" der dummy Notiz
         */
        public void notizClear()
        {
            notiz.setTitel("");
            notiz.setNotiz("");
            tempNotiztext = "";
            tempTitel = "";
        }


        //Programm starten Methoden

        /*
         * Fast wie CreateNotizTitel(), nur mit 2 Sekunden Zeit um den Programmnamen zu sagen
         */
        public void programmName() 
        {
            using (
            SpeechRecognitionEngine recognizer =
                new SpeechRecognitionEngine(
                new System.Globalization.CultureInfo("de-DE")))

            {
                recognizer.LoadGrammar(dGrammar);

                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized_programmMethode);

                recognizer.SetInputToDefaultAudioDevice();

                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                setTimer(2000);
                while (notizInput) { }

            }
        }


        public void recognizer_SpeechRecognized_programmMethode(object sender, SpeechRecognizedEventArgs e)
        {
            addTempProgrammName(e.Result.Text);
            add_tBoxt_erkannteSprache(e.Result.Text);
        }

        
        /*
         * gehörtem Programmname wird ein ".exe" angehangen um das jeweilige Programm zu starten
         * Die Eingabetextbox wird "leer" gemacht mit dem clear Befehl
         */
        public void programmStarten()
        {
            Process.Start(getTempProgrammName() + ".exe");
            clear_tBoxt_erkannteSprache();
        }
        

        //get set und add Methoden

        public void addTempProgrammName(string pName)
        {
            tempProgrammName = tempProgrammName + pName;
        }
        public void setProgrammName(string pName)
        {
            tempProgrammName = pName;
        }
        public string getTempProgrammName()
        {
            return tempProgrammName;
        }
       


        //Internet Suche

        /*
         * 10 Sekunden Zeit für die Eingabe der Suchanfrage
         * Eingabe wird in der Variable suchtext später gespeichert 
         */
        public void suchAnfrage()
        {
            using (
           SpeechRecognitionEngine recognizer =
               new SpeechRecognitionEngine(
               new System.Globalization.CultureInfo("de-DE")))

            {
                recognizer.LoadGrammar(dGrammar);

                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized_Suche);

                recognizer.SetInputToDefaultAudioDevice();

                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                setTimer(10000);
                while (notizInput) { }

            }
        }
        public void recognizer_SpeechRecognized_Suche(object sender, SpeechRecognizedEventArgs e)
        {
            addSuchAnfrage(e.Result.Text);
            add_tBoxt_erkannteSprache(e.Result.Text);
        }
       
        
        /*
         * Im Standartbrowser wird ein neuer Tab geöffnet
         * die Variable suchText wird in den Google Suclink eingefügt, so dass über Google der eingegeben Text gesucht wird
         * danach wird die Variable suchText "geleert"
         */
        public void suchen()
        {
            System.Diagnostics.Process.Start("https://www.google.de/search?hl=de&q=" + getSuchText() + "&btnG=Google-Suche&Meta=");
            setSuchText();
        }


        //get set und add Methoden
        public void addSuchAnfrage(string pSuche)
        {
            suchText = suchText + pSuche;
        }
        public string getSuchText()
        {
            return suchText;
        }
        public void setSuchText()
            {
                suchText = "";
            }


        //Wecker Methoden
        public void hintergrundWecker()
        {
            System.DateTime moment = new System.DateTime();
            Time timeNow = new Time();
            timeNow.setHour(moment.Hour);
            timeNow.setMinute(moment.Minute);
            while (true)
            {
                timeNow.setHour(moment.Hour);
                timeNow.setMinute(moment.Minute);
                if (verwaltung.weckerExists())
                {
                    if (verwaltung.weckerActive(timeNow))
                    {
                       
                        while (weckerRecognized == false)
                        {

                            //play Alert
                        }
                        setWeckerRecognized(false);
                        if (verwaltung.searchActiveWecker(timeNow).getNotizUsed())
                        {
                            string tempNotiz = verwaltung.searchActiveWecker(timeNow).getNotiz();
                            s.SpeakAsync(tempNotiz);
                        }
                        //List Class muss noch gemacht werden (the morpheus tutprials c# Listen)
                        //verwaltung.getWeckerList().remove(verwaltung.searchActiveWecker(timeNow));
                    }
                        
                }

            }
        }

        public void setWakeUpTime()
        {
            using (
            SpeechRecognitionEngine recognizer =
                new SpeechRecognitionEngine(
                new System.Globalization.CultureInfo("de-DE")))

            {
                recognizer.LoadGrammar(dGrammar);

                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized_wakeUpTime);

                recognizer.SetInputToDefaultAudioDevice();

                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                setTimer(4000);
                while (notizInput) { }

            }
        }
        public void recognizer_SpeechRecognized_wakeUpTime(object sender, SpeechRecognizedEventArgs e)
        {
            addWakeTimeComplete(e.Result.Text); 
            //Schauen wie der string (x Uhr x|x minuten) unterteilt wird für den wecker thread
        }

        public void setWeckerRecognized(Boolean pRecognized)
        {
            weckerRecognized = pRecognized;
        }
        public void getweckerRecognized()
        {
            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("de-DE"));
            recognizer.LoadGrammar(dGrammar);

            recognizer.SpeechDetected += Recognizer_SpeechDetected;

            recognizer.SetInputToDefaultAudioDevice();

            recognizer.RecognizeAsync(RecognizeMode.Multiple);

            
        }

        private void Recognizer_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            setWeckerRecognized(true);
        }

        public void addWakeTimeComplete(string pTime)
        {
            wakeTimeComplete = wakeTimeComplete + pTime;
        }
        public String getWakeTimeComplete()
        {
            return wakeTimeComplete;
        }

    }
}
