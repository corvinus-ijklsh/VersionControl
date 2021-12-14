using MicroSimExample.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroSimExample
{
    public partial class Form1 : Form
    {
        List<Person> Population = null;
        List<BirthProbability> BirthProbabilities = null;
        List<DeathProbability> DeathProbabilities = null;


        public Form1()
        {
            InitializeComponent();

            Population = GetPopulation(@"C:\temp\nép-teszt.csv");
            BirthProbabilities = GetBirthProbabilities(@"C:\temp\születés.csv");
            DeathProbabilities = GetDeathProbabilities(@"C:\temp\halál.csv");
        }

        public List<Person> GetPopulation(string csvPath)
        {
            List<Person> population = new List<Person>();
            using (var sr = new StreamReader(csvPath,Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    population.Add(new Person()
                    {
                        BirthYear = int.Parse(line[0]),
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[1]),
                        NbrOfChildren = int.Parse(line[2])
                    });
                      
                    
                }
            }

            return population;
        }

        public List<BirthProbability> GetBirthProbabilities(string csvPath)
        {
            List<BirthProbability> birthProbabilities = new List<BirthProbability>();
            using (var sr = new StreamReader(csvPath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    birthProbabilities.Add(new BirthProbability()
                    {
                        Age = int.Parse(line[0]),
                        NbrOfChildren = int.Parse(line[1]),
                        P = double.Parse(line[2].Replace(",","."))
         
                    });


                }
            }

            return birthProbabilities;
        }

        public List<DeathProbability> GetDeathProbabilities(string csvPath)
        {
            List<DeathProbability> deathProbabilities = new List<DeathProbability>();
            using (var sr = new StreamReader(csvPath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    deathProbabilities.Add(new DeathProbability()
                    {
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[0]),
                        Age = int.Parse(line[1]),
                        P = double.Parse(line[2])
                    
                    });


                }
            }

            return deathProbabilities;
        }
    }
}
