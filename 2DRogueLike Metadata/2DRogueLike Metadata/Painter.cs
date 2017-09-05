using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace _2DRogueLike_Metadata
{
    public partial class Painter : Form
    {
        private static readonly Random rng = new Random();
        public static int level = 0;
        public static int health = 100;
        public static int enemy = (int)Math.Log(level, 2f);
        public static int food = rng.Next(0, 6);
        public static int wall = rng.Next(1, 9);
        public static int[,] grid = new int[8, 8];
        private static double[] tempParams = new double[11];

        private GeneticSharp.Domain.Chromosomes.Gene[] gene;
        private GeneticSharp.Domain.Chromosomes.IChromosome chromosome;

        public double answer { get; set; }

        public void initValues(int level, int health, int food)
        {

            setLevel(level);
            setHealth(health);
            setFood(food);
            enemy = (int)Math.Log(level, 2f);
            wall = rng.Next(1, 9);
            playMusic();
        }
        
        public Painter(GeneticSharp.Domain.Chromosomes.Gene[] genes)
        {
            InitializeComponent();
            this.gene = genes;
            //tempParams = new int[] { binArrayToint(gene, 6, 5), binArrayToint(gene, 9, 14), binArrayToint(gene, 3, 17), binArrayToint(gene, 11, 28), binArrayToint(gene, 34, 62), binArrayToint(gene, 27, 89), binArrayToint(gene, 9, 98), binArrayToint(gene, 7, 105), binArrayToint(gene, 10, 115), binArrayToint(gene, 10, 125), binArrayToint(gene, 10, 135)};
            GeneticSharp.Domain.Chromosomes.FloatingPointChromosome tempChromosome = new GeneticSharp.Domain.Chromosomes.FloatingPointChromosome(
                new double[] { 0, 0, 0, 7, 30, 32, 23, 23, 0, 0, 0 }, //min values
                new double[] { 50, 500, 5, 1800, 9999999999, 99999999, 288, 72, 999, 999, 999 }, //max values
                new int[] { 6, 9, 3, 11, 34, 27, 9, 7, 10, 10, 10 }, //bits required for values
                new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } //bits for fractals
                );

            tempChromosome.ReplaceGenes(0, genes);
            tempParams = tempChromosome.ToFloatingPoints();

            Debug.WriteLine(tempParams[0] + " " + tempParams[1] + " " + tempParams[2] + " " + tempParams[3] + " " + tempParams[4] + " " + tempParams[5] + " " + tempParams[6] + " " + tempParams[7] + " " + tempParams[8] + " " + tempParams[9] + " " + tempParams[10]);
            initValues((int)tempParams[0], (int)tempParams[1], (int)tempParams[2]);
            setupNext();
            
        }

        private void Paint_Load(object sender, EventArgs e)
        {

            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(Painter_Paint);
            
                }

        private void Painter_Paint ( object sender, System.Windows.Forms.PaintEventArgs e)
        {

            e.Graphics.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);

            //drawboard
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    switch(grid[i, j])
                    {

                        case 0:
                            e.Graphics.DrawImage(Properties.Resources._0, i * 34, j * 30);
                            break;
                        case 1:
                            e.Graphics.DrawImage(Properties.Resources._1, i * 34, j * 30);
                            break;
                        case 2:
                            e.Graphics.DrawImage(Properties.Resources._2, i * 34, j * 30);
                            break;
                        case 3:
                            e.Graphics.DrawImage(Properties.Resources._3, i * 34, j * 30);
                            break;
                    }
                }
            }
            e.Graphics.DrawImage(Properties.Resources._4, 0, 0);
            e.Graphics.DrawString("HP" + health.ToString(), new Font("Arial", 20), Brushes.Red, 100, 200);
        }

        public void playMusic()
        {
            string html = string.Empty;
            int step = (int)(tempParams[6] * 30 / 60);
            string url = @"https://www.wolframcloud.com/objects/user-a13d29f3-43bf-4b00-8e9b-e55639ecde19/NKMMusicDownload" +
                "?id=NKM-G-10-" + 
                (int)tempParams[3] + "-" +
                (int)tempParams[4] + "-1-" +
                (int)tempParams[5] + "-" + 
                step + "-" +
                (int)tempParams[6] + "-4-2773-" +
                (int)tempParams[7] + "-0-1-" + 
                getRole((int)tempParams[8], roles) + "-1-" +
                getRole((int)tempParams[9], roles) + "-1-" + 
                getRole((int)tempParams[10], roles) + "-0-0-0-0-0&form=WAV";

            Debug.WriteLine(url);

            System.Media.SoundPlayer player;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                player = new System.Media.SoundPlayer(stream);
                player.Play();
            }
        }

        private int[] roles = { 0, 1, 10, 16, 20, 21, 51, 52, 901, 907, 912, 913, 914, 915, 916, 922, 923, 924, 932, 101, 201, 202, 102, 203, 206, 103, 104, 105, 107, 108, 209, 110, 109, 111, 121, 204, 205, 122, 207, 208, 210, 123, 124, 127, 128, 129, 130, 132, 131, 135, 136, 137, 144, 140, 149, 141, 142, 143, 145, 151, 153, 154, 161, 162, 163, 169, 170, 180, 183, 181, 182, 302, 316, 301, 303, 304, 305, 313, 306, 307, 314, 315, 340, 32, 341, 32, 312, 308, 309, 310, 311, 317, 350, 351, 501, 515, 516, 502, 520, 521, 503, 511, 545, 504, 505, 544, 512, 541, 542, 543, 513, 510, 514, 570, 536, 540, 552, 551, 537, 538, 561, 562 };

        private int getRole(int target, int[] array)
        {
            int currentDifference = 10000;
            int currentNearest = 0;
            for (int i = 1; i < array.Length; i++)
            {
                int diff = array[i] - target;
                if (diff < 0)
                {
                    diff = -diff;
                }
                if (diff < currentDifference)
                {
                    currentDifference = diff;
                    currentNearest = array[i];
                }
            }

            //Debug.Log(currentNearest);
            return currentNearest;
        }

        static void setupNext()
        {

            reset();

            //level++;
            //enemy = (int)Math.Log(level, 2f);
            //food = rng.Next(0, 6);
            //wall = rng.Next(1, 9);

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

            for (int i = 0; i < 8; i++)
            {

                for (int j = 0; j < 8; j++)
                {

                    grid[i, j] = 0;
                }
            }
        }

        private void Refresher_Tick(object sender, EventArgs e)
        {

            this.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            int x = 0;
            string input = textBox1.Text;
            double result = 0;
            if (Int32.TryParse(input, out x))
            {

                result = Convert.ToDouble(input);
                if (result <= 10 && result >= 1)
                {
                    this.answer = result;
                    this.DialogResult = DialogResult.OK;
                    //textBox1.Text = "";
                    //setupNext();
                    Close();
                }
                else
                {
                    textBox1.Text = "Choose a number between 1 and 10";
                }
            }
            else
            {
                textBox1.Text = "Invalid input";
            }
        }

        private static int binArrayToint(GeneticSharp.Domain.Chromosomes.Gene[] gene, int length, int pointer)
        {
            int result = 0;

            for (int i = 0; i < length; i++)
            {
                if ((int)gene[pointer - i].Value == 1)
                {
                    result += (int)(Math.Pow(2, Convert.ToDouble(gene[pointer - i].Value)));
                }
            }

            return result;
        }

        public static void setLevel(int level)
        {

            Painter.level = level;
        }
        public static void setHealth(int health)
        {

            Painter.health = health;
        }
        public static void setFood(int food)
        {

            Painter.food = food;
        }
    }
}
