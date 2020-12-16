using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reversi
{
    public partial class Form1 : Form
    {
        const int breed = 8;
        const int hoog = 8;
        int maximaal = Math.Max(breed, hoog);
        int minimaalFormaat = 500;
        int muisX;
        int muisY;
        int formaatVakje;
        bool speler1Beurt;
        Color kleurSpeler1 = Color.Red;
        Color kleurSpeler2 = Color.Blue;

        int[,] gameState = new int[breed, hoog];

        public Form1()
        {

            formaatVakje = minimaalFormaat / maximaal;

            int middenX = breed / 2;
            int middenY = hoog / 2;

            gameState[middenX, middenY] = 1;
            gameState[middenX - 1, middenY - 1] = 1;
            gameState[middenX, middenY - 1] = 2;
            gameState[middenX - 1, middenY] = 2;

            gameState[5, 5] = 1;
            gameState[6, 6] = 1;

            this.speler1Beurt = true;

            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(formaatVakje * breed + 1, formaatVakje * hoog + 51);
            this.panel1.Size = new System.Drawing.Size(formaatVakje * breed + 1, formaatVakje * hoog + 1);

            //this.panel1.BackColor = Color.FromArgb(174, 184, 254);
            this.label1.Text = "Speler 1 is aan de beurt.";
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Brush speler1Brush = new SolidBrush(kleurSpeler1);
            Brush speler2Brush = new SolidBrush(kleurSpeler2);

            Graphics gr = e.Graphics;

            Pen penZwart = new Pen(Color.Black, 1);


            for (int h = 0; h <= breed; h++)
            {
                gr.DrawLine(penZwart, formaatVakje * h, 0, formaatVakje * h, panel1.Height);
            }

            for (int b = 0; b <= hoog; b++)
            {
                gr.DrawLine(penZwart, 0, formaatVakje * b, panel1.Width, formaatVakje * b);
            }

            for (int b = 0; b < breed; b++)
            {
                for (int h = 0; h < hoog; h++)
                {
                    if (gameState[b, h] == 1)
                    {
                        gr.FillEllipse(speler1Brush, formaatVakje * b, formaatVakje * h, formaatVakje, formaatVakje);
                    }
                    else if (gameState[b, h] == 2)
                    {
                        gr.FillEllipse(speler2Brush, formaatVakje * b, formaatVakje * h, formaatVakje, formaatVakje);
                    }
                }
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            string beurt;

            // x / breede per vakje en y / hoogte per vakje
            int vakjeX = this.muisX / this.formaatVakje;
            int vakjeY = this.muisY / this.formaatVakje;

            bool redo = false;

            if (this.speler1Beurt)
            {
                int[,] check = berekenGeldigeZet(vakjeX, vakjeY);
                if (check[vakjeX, vakjeY] == 1)
                {
                    this.gameState[vakjeX, vakjeY] = 1;
                }
                else
                {
                    redo = true;
                }
            }
            else
            {
                int[,] check = berekenGeldigeZet(vakjeX, vakjeY);
                if (check[vakjeX, vakjeY] == 2)
                {
                    this.gameState[vakjeX, vakjeY] = 2;
                }
                else
                {
                    redo = true;
                }
            }

            if (redo == false)
            {
                this.speler1Beurt = !this.speler1Beurt;
            }
            if (this.speler1Beurt)
            {
                beurt = "Speler 1 is aan de beurt.";
            }
            else
            {
                beurt = "Speler 2 is aan de beurt.";
            }
            this.label1.Text = beurt;
            this.Refresh();

        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            this.muisX = e.X;
            this.muisY = e.Y;
        }

        private int geldigeZet(int rowChange, int columnChange, int row, int column, int count)
        {
            int self;
            int other;
            if (this.speler1Beurt)
            {
                self = 1;
                other = 2;
            }
            else
            {
                self = 2;
                other = 1;
            }

            if ((row + rowChange < 0) || (row + rowChange > hoog - 1))
            {
                return count;
            }
            else if ((column + columnChange < 0) || (column + columnChange > breed - 1))
            {
                return count;
            }
            else if (gameState[column + columnChange, row + rowChange] != other)
            {
                return count;
            }
            else if (gameState[column + columnChange, row + rowChange] == other)
            {
                if (geldigeZet(rowChange, columnChange, row+rowChange, column+columnChange, count) == 0)
                {
                    return count + 1;
                }
                else
                {
                    return geldigeZet(rowChange, columnChange, row + rowChange, column + columnChange, count) + 1;
                }
            }
            else
            {
                return count;
            }
        }

        private int[,] berekenGeldigeZet(int c, int r)
        {
            int row = r;
            int column = c;
            int[,] valid = new int[breed, hoog];

            if (this.speler1Beurt)
            {
                int beurt = 1;
            }
            else
            {
                int beurt = 2;
            }

            if (gameState[column, row] == 0)
            {
                int nw = geldigeZet(-1, -1, row, column, 0);
                int nn = geldigeZet(-1, 0, row, column, 0);
                int ne = geldigeZet(-1, 1, row, column, 0);

                int ee = geldigeZet(0, 1, row, column, 0);
                int ww = geldigeZet(0, -1, row, column, 0);

                int sw = geldigeZet(1, -1, row, column, 0);
                int ss = geldigeZet(1, 0, row, column, 0);
                int se = geldigeZet(1, 1, row, column, 0);
                Console.WriteLine(se);

                int[] all8 = new int[8] {nw, nn, ne, ee, ww, sw, ss, se };

                for (int i = 0; i < 8; i++)
                {
                    if (all8[i] > 0)
                    {
                        if (this.speler1Beurt)
                        {
                            valid[column, row] = 1;
                        }
                        else
                        {
                            valid[column, row] = 2;
                        }
                    }

                }
            }
            return valid;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog leukKleurtje = new ColorDialog();

            if (speler1Beurt)
            {
                leukKleurtje.Color = kleurSpeler1;

                // Update the text box color if the user clicks OK 
                if (leukKleurtje.ShowDialog() == DialogResult.OK && leukKleurtje.Color != Color.White && leukKleurtje.Color != kleurSpeler2)
                    kleurSpeler1 = leukKleurtje.Color;
            }
            else
            {
                leukKleurtje.Color = kleurSpeler2;

                // Update the text box color if the user clicks OK 
                if (leukKleurtje.ShowDialog() == DialogResult.OK && leukKleurtje.Color != Color.White && leukKleurtje.Color != kleurSpeler1)
                    kleurSpeler2 = leukKleurtje.Color;
            }

            this.Refresh();
            // kleur mag niet zelfde als andere speler en niet wit
        }
        //methode is de steen ingesloten
        // methode geldige zet
    }
}