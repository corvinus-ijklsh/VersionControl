using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldsHardestGame;

namespace EvolutionExample
{
    public partial class Form1 : Form
    {
        GameController gc = new GameController();
        GameArea ga = null;
        Brain winnerBrain = null;

        int populationSize = 100;
        int nbrofSteps = 10;
        int nbrofStepsIncrement = 10; //iterációnként mennyi plusz lépést kap egy játékos
        int generation = 1;
        public Form1()
        {
            InitializeComponent();

            gc.GameOver += Gc_GameOver;

            ga = gc.ActivateDisplay();
            //egy korábban létrehozott vezérlőnek hívom meg a display fvnyét ami az areát adja vissza
            this.Controls.Add(ga);
            for (int i = 0; i < populationSize; i++)
            {
                gc.AddPlayer(nbrofSteps);
                
            }
            gc.Start();
            //ha véget ért a játék, ki akarom emelni a legjobb népességet
            //a gcnek van GameOver eseménye, ehhez ugyanúgy lehet eseménykezelőt rendelni


        }

        private void Gc_GameOver(object sender)
        {
            generation++;
            //ha kódból hoztam volna létre a labelt: lblGeneration.BringToFront();
            lblGeneration.Text = generation.ToString() + ". generáció";

            //***Evolúciós lépés***
            //Játákosok listája, fitness szerint rendezve
            var playerList = from p in gc.GetCurrentPlayers()
                             orderby p.GetFitness()
                             select p;
            var topPerformers = playerList.Take(playerList.Count() / 2).ToList();
            //azért kell tolisttel listává alakítani, mert így lemásoljuk a kiválasztott listaelemeket
            //ha ezt nem tesszük meg, akkor az eredeti lista egy szűrt változatával dolgozunk

            var winners = from p in topPerformers
                          where p.IsWinner
                          select p;
            if (winners.Count()>0)
            {
                winnerBrain = winners.FirstOrDefault().Brain.Clone();
                gc.GameOver -= Gc_GameOver; //megakadályozzuk, hogy még egyszer meg legyen hívva a GameOver
                //ha nem akadályozzuk meg, nem lehet új játékot kezdeni
                btnStart.Visible = true;
                return;
            }

            gc.ResetCurrentLevel();
            foreach (var p in topPerformers)
            {
                var brain = p.Brain.Clone();
                if (generation % 3 == 0)
                {
                    gc.AddPlayer(brain.ExpandBrain(nbrofStepsIncrement));
                }
                else
                {
                    gc.AddPlayer(brain);
                }

                if (generation % 3 == 0)
                {
                    gc.AddPlayer(brain.Mutate().ExpandBrain(nbrofStepsIncrement));
                }
                else
                {
                    gc.AddPlayer(brain.Mutate());
                }
            }
            gc.Start();
            


        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            gc.ResetCurrentLevel();
            gc.AddPlayer(winnerBrain.Clone());
            gc.AddPlayer();
            ga.Focus();

            gc.Start(true);
        }
    }
}
