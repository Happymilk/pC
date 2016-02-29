using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Graphics graph;
        Random rnd = new Random();

        const int pointsCount = 10000;
        double pc1, pc2;

        public Form1()
        {
            InitializeComponent();
            graph = pictureBox1.CreateGraphics();
        }

        private void Do(Graphics graphics)
        {
            var points1 = new int[pointsCount];
            var points2 = new int[pointsCount];
            double mx1 = 0;
            double mx2 = 0;

            for (int i = 0; i < pointsCount; i++)
            {
                points1[i] = rnd.Next(100, 740);
                points2[i] = rnd.Next(-100, 540);
                mx1 += points1[i];
                mx2 += points2[i];
            }
            mx1 /= pointsCount;
            mx2 /= pointsCount;

            double sigma1 = 0;
            double sigma2 = 0;
            for (int i = 0; i < pointsCount; i++)
            {
                sigma1 += Math.Pow(points1[i] - mx1, 2);
                sigma2 += Math.Pow(points2[i] - mx2, 2);
            }
            sigma1 = Math.Sqrt(sigma1 / pointsCount);
            sigma2 = Math.Sqrt(sigma2 / pointsCount);

            var result1 = new double[pictureBox1.Width];
            var result2 = new double[pictureBox1.Width];
            result1[0] = (Math.Exp(-0.5 * Math.Pow((-100 - mx1) / sigma1, 2)) /
                    (sigma1 * Math.Sqrt(2 * Math.PI)) * pc1); ;
            result2[0] =
                    (Math.Exp(-0.5 * Math.Pow((-100 - mx2) / sigma2, 2)) /
                    (sigma2 * Math.Sqrt(2 * Math.PI)) * pc2); ;

            int D = 0;
            for (int x = 1; x < pictureBox1.Width; x++)
            {
                result1[x] =
                    (Math.Exp(-0.5 * Math.Pow((x - 100 - mx1) / sigma1, 2)) /
                    (sigma1 * Math.Sqrt(2 * Math.PI)) * pc1);

                result2[x] =
                    (Math.Exp(-0.5 * Math.Pow((x - 100 - mx2) / sigma2, 2)) /
                    (sigma2 * Math.Sqrt(2 * Math.PI)) * pc2);

                if (Math.Abs(result1[x] * 500 - result2[x] * 500) < 0.002)
                {
                    D = x;
                }

                graphics.DrawLine(Pens.Blue,
                     new Point(x - 1, (pictureBox1.Height - (int)(result1[x - 1] * pictureBox1.Height * 500))),
                    new Point(x, (pictureBox1.Height - (int)(result1[x] * pictureBox1.Height * 500))));
                graphics.DrawLine(Pens.Red,
                     new Point(x - 1, (pictureBox1.Height - (int)(result2[x - 1] * pictureBox1.Height * 500))),
                    new Point(x, (pictureBox1.Height - (int)(result2[x] * pictureBox1.Height * 500))));

            }
            double error1 = result2.Take((int)D).Sum();
            double error2;
            if (pc1 > pc2)
            {
                error2 = result2.Skip((int)D).Sum();
            }
            else
            {
                error2 = result1.Skip((int)D).Sum();
            }

            using (var textBrush = new SolidBrush(Color.Black))
            {
                graphics.DrawLine(Pens.Chartreuse, D, 0, D, pictureBox1.Height);
                graphics.DrawLine(Pens.Black, 0, pictureBox1.Height - 1,
                    pictureBox1.Width, pictureBox1.Height - 1);
                graphics.DrawLine(Pens.Black, pictureBox1.Width,
                    pictureBox1.Height - 1, pictureBox1.Width - 15,
                    pictureBox1.Height - 5);
                graphics.DrawLine(Pens.Black, 100, pictureBox1.Height - 1, 100, 0);
                graphics.DrawLine(Pens.Black, 100, 0, 95, 15);
                graphics.DrawLine(Pens.Black, 100, 0, 105, 15);
                graphics.DrawString("X", this.Font, Brushes.Black,
                    pictureBox1.Width - 10, pictureBox1.Height - 20);
                graphics.DrawLine(Pens.Blue, pictureBox1.Width - 150, 15,
                    pictureBox1.Width - 100, 15);
                graphics.DrawString("p(X / C1) P(C1)", this.Font, textBrush,
                    pictureBox1.Width - 90, 5);

                graphics.DrawLine(Pens.Red, pictureBox1.Width - 150, 30,
                    pictureBox1.Width - 100, 30);
                graphics.DrawString("p(X / C2) P(C2)", this.Font, textBrush,
                    pictureBox1.Width - 90, 25);
            }

            MessageBox.Show("Вер-ть ложной тревоги " + error1.ToString());
            MessageBox.Show("Вер-ть пропуска обнаружения " + error2.ToString());
            MessageBox.Show("Суммарная ошибка классификации" + (error1 + error2).ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pc1 = (double)numericUpDown1.Value;
            pc2 = (double)numericUpDown2.Value;

            var bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Do(graphics);
                pictureBox1.Image = bitmap;
            }
        }
    }
}