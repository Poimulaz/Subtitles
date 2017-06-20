using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using Video;

namespace WpfTutorialSamples.Audio_and_Video
{
    class Titres
    {
        //récupere les soustitres les range dans une liste qui est renvoyer a la classe qui s'occupe de la video
        internal List<Soustitre> Main()
        {
            
            String path =
                Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.Length - 16) + @"\Ressources\Lotr.txt";
            // !!! Le fichier texte est defini ici et il se trouve dans le dossier Ressources
            //     La vidéo est defini sur MainWindow.xaml comme si elle se trouvait sur le bureau (changer le chemin en fonction de votre bureau)

            Regex r = new Regex(@"-->");
            Regex v = new Regex(@":");
            Regex z = new Regex(@",");
            List<Soustitre> liste = new List<Soustitre>();
            Timer time = new Timer();
            time.Enabled = true;
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string l;
                    string[] grille;
                    bool txt = false;
                    Soustitre re = new Soustitre();
                    while ((l = sr.ReadLine()) != null)
                    {
                        //On verifie si la ligne lue est une ligne de temps ou de texte
                        if (txt)
                        {
                            //On arrete de lire les lignes en temps que texte si on arrive a la fin du soustitre
                            if (l == "")
                            {
                                //Arrivé a la fin du soustitre, on enregistre notre chaine (créée après) dans notre soustitre
                                liste.Add(re);
                                txt = false;
                            }
                            else
                            {
                                //Recupere chaque caractere de la ligne pour les ajouter dans une chaine contenant tout notre soustitre
                                int compteur = 0;
                                while (compteur != l.Length)
                                {
                                    re.texte += l[compteur];
                                    compteur++;
                                }
                                re.texte += " ";
                            }
                        }
                        if (l.Length == 29)
                        {
                            //On vérifie que les 29 caracteres de la ligne correspond a une ligne de temps et on prepare le temps pour traitement
                            grille = r.Split(l);
                            if (grille.Count() == 2)
                            {
                                re = new Soustitre();

                                //Traitement du temps de debut
                                TimeSpan debut = new TimeSpan(0, 0, 0, 0, 0);
                                //La premiere case de notre grille contient 3 temps (heures/minutes/secondes) qu'on separe
                                string[] segment = v.Split(grille[0]);
                                debut = debut.Add(TimeSpan.FromHours(Convert.ToDouble(segment[0])));
                                debut = debut.Add(TimeSpan.FromMinutes(Convert.ToDouble(segment[1])));
                                //La troisieme case de notre nouveau tableau contient 2 temps (secondes/millisecondes) qu'on separe
                                string[] milli = z.Split(segment[2]);
                                debut = debut.Add(TimeSpan.FromSeconds(Convert.ToDouble(milli[0])));
                                debut = debut.Add(TimeSpan.FromMilliseconds(Convert.ToDouble(milli[1])));

                                //Meme traitement mais utiliser cette fois sur le temps de fin
                                TimeSpan fin = new TimeSpan(0, 0, 0, 0, 0);
                                segment = v.Split(grille[1]);
                                fin = fin.Add(TimeSpan.FromHours(Convert.ToDouble(segment[0])));
                                fin = fin.Add(TimeSpan.FromMinutes(Convert.ToDouble(segment[1])));
                                milli = z.Split(segment[2]);
                                fin = fin.Add(TimeSpan.FromSeconds(Convert.ToDouble(milli[0])));
                                fin = fin.Add(TimeSpan.FromMilliseconds(Convert.ToDouble(milli[1])));

                                //On attribue les temps de debut et fin calculer precedemment a notre soutitre puis on definie
                                //que les lignes suivantes sont des lignes de texte
                                txt = true;
                                re.debut = debut;
                                re.fin = fin;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return liste;
        }

        //Au lancement de la vidéo lance tous les sous-titres en traitement asynchrone
        public void play(List<Soustitre> liste, TimeSpan pause, TextBlock helsoustitre)
        {
            foreach (Soustitre soustitre in liste)
            {
                Task defile = afficher(soustitre, helsoustitre,pause);
            }
        }

        //Chaque sous-titre attends le temps où il doit apparaitre, apparait, puis attends le temps pour disparaitre, et disparait
        private async Task afficher(Soustitre soustitre,TextBlock helsoustitre,TimeSpan pause)
        {
            TimeSpan compare = new TimeSpan(0, 0, 0, 0);
            //Tentative de lancer une erreur lorsque qu'on apppuie sur pause afin de détruire tous les sous-titre
            // (Ils seraient reconstruit avec un nouveau temps qu'en on appuierait sur play)
            try {
                if (soustitre.debut - pause >= compare)
                {
                    await Task.Delay(soustitre.debut - pause);
                    helsoustitre.Text = soustitre.texte;//Console.WriteLine(soustitre.texte);
                }
                if ((soustitre.fin - soustitre.debut) - pause >= compare)
                {
                    await Task.Delay((soustitre.fin - soustitre.debut) - pause);
                    helsoustitre.Text = "";//Console.Clear();
                }
            }
            catch (Pause e)
            {

            }
        }
    }
}