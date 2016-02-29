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
        Graphics g;
        Random rnd = new Random();

        const int pointsCount = 10000;
        double pc1, pc2;

        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var brush = new SolidBrush(Color.Black);
            var textBrush = new SolidBrush(Color.White);

            g.FillRectangle(brush, 0, 0, this.pictureBox1.Width, this.pictureBox1.Height);

            g.DrawLine(Pens.White, 0, pictureBox1.Height - 100,
                pictureBox1.Width, pictureBox1.Height - 100);
            g.DrawLine(Pens.White, 100, pictureBox1.Height - 1, 100, 0);

            g.DrawLine(Pens.Yellow, 0, 15, 10, 15);
            g.DrawString("p(X/C1)", this.Font, textBrush, 20, 5);
            g.DrawLine(Pens.Lime, 0, 30, 10, 30);
            g.DrawString("p(X/C2)", this.Font, textBrush, 20, 25);

            pc1 = (double)numericUpDown1.Value;
            pc2 = (double)numericUpDown2.Value;

            var points1 = new int[pointsCount];
            var points2 = new int[pointsCount];
            double mx1 = 0, mx2 = 0;

            for (int i = 0; i < pointsCount; i++)
            {
                points1[i] = rnd.Next(100, 740);
                points2[i] = rnd.Next(-100, 540);
                mx1 += points1[i];
                mx2 += points2[i];
            }
            mx1 /= pointsCount;
            mx2 /= pointsCount;

            double sig1 = 0;
            double sig2 = 0;

            for (int i = 0; i < pointsCount; i++)
            {
                sig1 += Math.Pow(points1[i] - mx1, 2);
                sig2 += Math.Pow(points2[i] - mx2, 2);
            }
            sig1 = Math.Sqrt(sig1 / pointsCount);
            sig2 = Math.Sqrt(sig2 / pointsCount);

            var res1 = new double[pictureBox1.Width];
            var res2 = new double[pictureBox1.Width];

            res1[0] = (Math.Exp(-0.5 * Math.Pow((-100 - mx1) / sig1, 2)) /
                    (sig1 * Math.Sqrt(2 * Math.PI)) * pc1); ;
            res2[0] = (Math.Exp(-0.5 * Math.Pow((-100 - mx2) / sig2, 2)) /
                    (sig2 * Math.Sqrt(2 * Math.PI)) * pc2); ;

            int D = 0;
            
            for (int x = 1; x < pictureBox1.Width; x++)
            {
                res1[x] = (Math.Exp(-0.5 * Math.Pow((x - 100 - mx1) / sig1, 2)) /
                    (sig1 * Math.Sqrt(2 * Math.PI)) * pc1);
                res2[x] = (Math.Exp(-0.5 * Math.Pow((x - 100 - mx2) / sig2, 2)) /
                    (sig2 * Math.Sqrt(2 * Math.PI)) * pc2);

                if (Math.Abs(res1[x] * 500 - res2[x] * 500) < 0.002)
                    D = x;

                g.DrawLine(Pens.Yellow, new Point(x - 1, (pictureBox1.Height - 100 - (int)(res1[x - 1] * pictureBox1.Height * 500))),
                    new Point(x, (pictureBox1.Height - 100 - (int)(res1[x] * pictureBox1.Height * 500))));
                g.DrawLine(Pens.Lime, new Point(x - 1, (pictureBox1.Height - 100 - (int)(res2[x - 1] * pictureBox1.Height * 500))),
                    new Point(x, (pictureBox1.Height - 100 - (int)(res2[x] * pictureBox1.Height * 500))));
            }

            double error1 = res2.Take((int)D).Sum();
            double error2;

            if (pc1 > pc2)
                error2 = res2.Skip((int)D).Sum();
            else
                error2 = res1.Skip((int)D).Sum();
            
            g.DrawLine(Pens.Tomato, D, 0, D, pictureBox1.Height);
            
            MessageBox.Show("Вер-ть ложной тревоги " + error1.ToString());
            MessageBox.Show("Вер-ть пропуска обнаружения " + error2.ToString());
            MessageBox.Show("Суммарная ошибка классификации " + (error1 + error2).ToString());
        }
    }
}