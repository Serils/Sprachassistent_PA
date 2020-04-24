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
        public MainWindow()
        {
            Thread wecker = new Thread(new ThreadStart(hintergrundWecker));
            verwaltung.getErrorNote().setTitel("Error");
            InitializeComponent();

            Loaded += MainWindow_Loaded;

        }


        //Wecker Attribute
        private Boolean weckerRecognized = false;
        private string wakeTimeComplete = "";


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


        //Notiz Methoden
        public void createNotizText() 
        {
            using (
            SpeechRecognitionEngine recognizer=
                new SpeechRecognitionEngine(
                new System.Globalization.CultureInfo("de-DE")))

            {   
                recognizer.LoadGrammar(dGrammar);

                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized_Notiz);

                recognizer.SetInputToDefaultAudioDevice();

                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                while (notizInput) { }

            }
        }
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
        public void recognizer_SpeechRecognized_Notiz(object sender, SpeechRecognizedEventArgs e)
        {
            addTempNoteText(e.Result.Text);      
        }
        public void recognizer_SpeechRecognized_Titel(object sender, SpeechRecognizedEventArgs e)
        {
            setTempTitel(e.Result.Text);
        }
        public void setNotizInput(Boolean pNotizInput)
        {
            notizInput = pNotizInput;
        }
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
        public void createNotiz()
        {
            notiz.setTitel(tempTitel);
            notiz.setNotiz(tempNotiztext);
            verwaltung.addNote(notiz);
            notizClear();
        }
        public void notizClear()
        {
            notiz.setTitel("");
            notiz.setNotiz("");
            tempNotiztext = "";
            tempTitel = "";
        }


        //Programm starten Methoden
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
        }
        public void programmStarten()
        {
            Process.Start(getTempProgrammName() + ".exe");
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
        }
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
                       
                        while (getWeckerRecognized())
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
                recognizer.LoadGrammar(new DictationGrammar());

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
        public Boolean getWeckerRecognized()
        {
            return getWeckerRecognized();
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
