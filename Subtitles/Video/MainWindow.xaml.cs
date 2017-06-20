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
using System;
using System.Windows;
using System.Windows.Threading;
using System.Timers;
using Video;

namespace WpfTutorialSamples.Audio_and_Video
{
    //Class de base trouvé sur le net pour lancer et gérer une vidéo
    public partial class MediaPlayerVideoControlSample : Window
    {
        Boolean pause;
        Timer temps;
        List<Soustitre> liste;
        static DateTime now;
        static TimeSpan passe;
        Boolean play;

        public MediaPlayerVideoControlSample()
        {
            InitializeComponent();

            //Declaration de variables (certaines ne sont plus utiliser mais je ne sais plus lesquelles)
            temps = new Timer();
            now = new DateTime();
            liste = new List<Soustitre>();
            Titres soustitres = new Titres();
            liste = soustitres.Main();
            passe = new TimeSpan(0,0,0,0);
            temps.Elapsed += OnTimedEvent; 
            DispatcherTimer timer = new DispatcherTimer();

            //Timer de la video
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();


            pause = false;
            play = false;
        }

        //(Je pense) affiche la barre de temps
        void timer_Tick(object sender, EventArgs e)
        {
            if (mePlayer.Source != null)
            {
                if (mePlayer.NaturalDuration.HasTimeSpan)
                lblStatus.Content = String.Format("{0} / {1}", mePlayer.Position.ToString(@"hh\:mm\:ss"), mePlayer.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss"));
            }
            else
                lblStatus.Content = "No file selected...";
        }

        //Lance la video lorsqu'on appuie sur le boutton play (empeche de relancer le demarrage des soustitre si ils sont deja lancer)
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!play)
            {
                mePlayer.Play();
                if (pause)
                {
                    temps.Stop();
                }
                Titres soustitres = new Titres();
                soustitres.play(liste, passe, Helsoustitre);
                play = true;
            }
        }

        //Met la video en pause quand on appuie sur le boutton pause
        // (Tentative de lancer une exception. Le probleme est que l'exception arrete tous les autres processus de l'appli)
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Pause();
            pause = true;
            Pause ElPause = new Pause();
            //throw ElPause;
            temps.Start();
            now = DateTime.Now;
            play = false;
        }

        //Arrete la video quand on appuie sur le boutton stop
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
            play = false;
        }

        //Lorsque la video est en pause, lance un timer qui s'arrete quand on appuie sur play et range son temps dans une variable TimeSpan
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            passe = now - e.SignalTime;
        }
    }
}