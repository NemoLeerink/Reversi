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
        const int breed = 10;
        const int hoog = 10;
        int maximaal = Math.Max(breed, hoog);
        int minimaalFormaat = 500;
        int muisX;
        int muisY;
        int formaatVakje;
        bool speler1Beurt;
        Color kleurSpeler1 = Color.Blue;
        Color kleurSpeler2 = Color.Red;

        int[,] gameState = new int[breed, hoog];

        public Form1()
        {
            
            formaatVakje = minimaalFormaat / maximaal;

            int middenX = breed / 2;
            int middenY = hoog / 2;

            gameState[middenX, middenY] = 2;
            gameState[middenX-1, middenY-1] = 2;
            gameState[middenX, middenY-1] = 1;
            gameState[middenX - 1, middenY] = 1;

            this.speler1Beurt = true;

            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(formaatVakje * breed + 1, formaatVakje * hoog + 51);
            this.panel1.Size = new System.Drawing.Size(formaatVakje * breed + 1, formaatVakje * hoog + 1);

            //this.panel1.BackColor = Color.FromArgb(174, 184, 254);
            this.label1.Text = "Speler 1 is aan de beurt.";
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Brush speler2Brush = new SolidBrush(kleurSpeler2);
            Brush speler1Brush = new SolidBrush(kleurSpeler1);

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
                    if (gameState[b, h] == 1) {
                        gr.FillEllipse(speler2Brush, formaatVakje * b, formaatVakje * h, formaatVakje, formaatVakje);
                    }
                    else if (gameState[b, h] == 2)
                    {
                        gr.FillEllipse(speler1Brush, formaatVakje * b, formaatVakje * h, formaatVakje, formaatVakje);
                    }
                }
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            string beurt;

            if (this.speler1Beurt) {
                beurt = "Speler 2 is aan de beurt.";
            } else {
                beurt = "Speler 1 is aan de beurt.";
            }
            
            this.label1.Text = beurt;

            // x / breede per vakje en y / hoogte per vakje
            int vakjeX = this.muisX / this.formaatVakje;
            int vakjeY = this.muisY / this.formaatVakje;

            if (this.speler1Beurt)
            {
                this.gameState[vakjeX, vakjeY] = 2;
            }
            else 
            {
                this.gameState[vakjeX, vakjeY] = 1; 
            }

            this.speler1Beurt = !this.speler1Beurt;
            // if geen geldige beurt, doe weer this.blauwBeurt = !this.blauwBeurt;
            this.Refresh();
           
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            this.muisX = e.X;
            this.muisY = e.Y;
        }

        private bool geldigeZet(int rowChange, int columnChange, int row, int column)
        {
            int other;
            if (this.speler1Beurt)
            {
                other = 1;
            }
            else
            {
                other = 2;
            }

            if ((row + rowChange < 0) || (row + rowChange > hoog))
            {
                return false;
            }
            if ((column + columnChange < 0) || (column + columnChange > breed))
            {
                return false;
            }
            if (gameState[row + rowChange, column + columnChange] != other)
            {
                return false;
            }
            if (gameState[row + rowChange, column + columnChange] == other)
            {

            }

            bool geldigeZet = true;



            return geldigeZet;
        }

        private void berekenGeldigeZet()
        {
            int[,] valid = new int[breed, hoog];
            for (int row = 0; row < breed; row++)
            {
                for (int column = 0; column < hoog; column++)
                {
                    if (gameState[row, column] == 0)
                    {
                        bool nw = geldigeZet(-1, -1, row, column);
                        bool nn = geldigeZet(-1, 0, row, column);
                        bool ne = geldigeZet(-1, 1, row, column);

                        bool ee = geldigeZet(0, 1, row, column);
                        bool ww = geldigeZet(0, -1, row, column);

                        bool sw = geldigeZet(1, -1, row, column);
                        bool ss = geldigeZet(1, 0, row, column);
                        bool se = geldigeZet(1, 1, row, column);

                        if (nw || nn || ne || ee || ww || sw || ss || se)
                        {
                            if (this.speler1Beurt)
                            {
                                valid[row, column] = 2;
                            }
                            else
                            {
                                valid[row, column] = 1;
                            }
                        }
                    }
                }
            }
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