using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain;

namespace _2DRogueLike_Metadata
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        private static readonly Random rng = new Random();
        public static int level = 0;
        public static int enemy = (int)Math.Log(level, 2f);
        public static int food = rng.Next(0, 6);
        public static int wall = rng.Next(1, 9);
        public static int[,] grid = new int[8, 8];

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Painter());

            //setupNext();
            var chromosome = new FloatingPointChromosome(
                //lvl, hp, food, ruletype, rule, seed, tempo, pitch, r1, r2, r3
                new double[] { 0, 0, 0, 7, 30, 32, 23, 23, 0, 0, 0 }, //min values
                new double[] { 50, 500, 5, 1800, 9999999999, 99999999, 288, 72, 999, 999, 999}, //max values
                new int[] { 6, 9, 3, 11, 34, 27, 9, 7, 10, 10, 10}, //bits required for values
                new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } //bits for fractals
                );
            var population = new Population(5, 25, chromosome); //min-max population
            var fitness = new myFitness();
            var selection = new EliteSelection();
            var crossover = new TwoPointCrossover();
            var mutation = new FlipBitMutation();
            var termination = new FitnessStagnationTermination(10);

            var ga = new GeneticAlgorithm(
                population,
                fitness,
                selection,
                crossover,
                mutation);

            ga.Termination = termination;

            ga.Start();

        }

        static void setupNext()
        {

            reset();

            level++;
            enemy = (int)Math.Log(level, 2f);
            food = rng.Next(0, 6);
            wall = rng.Next(1, 9);

            while (food > 0)
            {
                int x = rng.Next(2, 8);
                int y = rng.Next(2, 8);
                if (grid[x, y] == 0)
                {

                    grid[x, y] = 1;
                    food--;
                }
            }
            while (enemy > 0)
            {
                int x = rng.Next(2, 8);
                int y = rng.Next(2, 8);
                if (grid[x, y] == 0)
                {

                    grid[x, y] = 2;
                    enemy--;
                }
            }
            while (wall > 0)
            {
                int x = rng.Next(2, 8);
                int y = rng.Next(2, 8);
                if (grid[x, y] == 0)
                {

                    grid[x, y] = 3;
                    wall--;
                }
            }
        }

        private static void reset()
        {

            for(int i = 0; i < 8; i++)
            {

                for (int j = 0; j < 8; j++)
                {

                    grid[i, j] = 0;
                }
            }
        }
    }


}
