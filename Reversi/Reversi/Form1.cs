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
        const int breed = 4;
        const int hoog = 4;
        int maximaal = Math.Max(breed, hoog);
        int minimaalFormaat = 500;
        int muisX;
        int muisY;
        int formaatVakje;
        bool speler1Beurt;
        bool hulpModus = false;
        Color kleurSpeler1 = Color.Blue;
        Color kleurSpeler2 = Color.Red;

        int[,] valid = new int[breed, hoog];
        int[,] gameState = new int[breed, hoog];

        public Form1()
        {
            formaatVakje = minimaalFormaat / maximaal;

            InitializeComponent();
            nieuwSpel();
            this.ClientSize = new System.Drawing.Size(formaatVakje * breed + 1, formaatVakje * hoog + 101);
            this.panel1.Size = new System.Drawing.Size(formaatVakje * breed + 1, formaatVakje * hoog + 1);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Brush speler2Brush = new SolidBrush(kleurSpeler2);
            Brush speler1Brush = new SolidBrush(kleurSpeler1);

            Graphics gr = e.Graphics;

            Pen penZwart = new Pen(Color.Black, 1);

            if (hulpModus) {
                if (speler1Beurt)
                {
                    for (int b = 0; b < breed; b++)
                    {
                        for (int h = 0; h < hoog; h++)
                        {
                            if (valid[b, h] == 1)
                            {
                                gr.DrawEllipse(penZwart, (formaatVakje * b) + (formaatVakje / 4), (formaatVakje * h) + (formaatVakje / 4), formaatVakje / 2, formaatVakje / 2);
                            }
                        }
                    }
                }
                else {
                    for (int b = 0; b < breed; b++)
                    {
                        for (int h = 0; h < hoog; h++)
                        {
                            if (valid[b, h] == 2)
                            {
                                gr.DrawEllipse(penZwart, (formaatVakje * b) + (formaatVakje / 4), (formaatVakje * h) + (formaatVakje / 4), formaatVakje / 2, formaatVakje / 2);
                            }
                        }
                    }
                }
            }


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
                    if (gameState[b, h] == 2) {
                        gr.FillEllipse(speler2Brush, formaatVakje * b, formaatVakje * h, formaatVakje, formaatVakje);
                    }
                    else if (gameState[b, h] == 1)
                    {
                        gr.FillEllipse(speler1Brush, formaatVakje * b, formaatVakje * h, formaatVakje, formaatVakje);
                    }
                }
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            string beurt;
            bool redo = false;
            //int[,] check = new int[breed, hoog];

            // x / breede per vakje en y / hoogte per vakje
            int vakjeX = this.muisX / this.formaatVakje;
            int vakjeY = this.muisY / this.formaatVakje;

            if (this.speler1Beurt)
            {
                berekenGeldigeZet(vakjeX, vakjeY);
                if (valid[vakjeX, vakjeY] == 2)
                {
                    this.gameState[vakjeX, vakjeY] = 2;
                }
                else
                {
                    redo = true;
                }
            }
            else
            {
                berekenGeldigeZet(vakjeX, vakjeY);
                if (valid[vakjeX, vakjeY] == 1)
                {
                    this.gameState[vakjeX, vakjeY] = 1;
                }
                else
                {
                    redo = true;
                }
            }

            if (!redo) {
                
                this.speler1Beurt = !this.speler1Beurt;
                if (this.speler1Beurt)
                {
                    beurt = "Speler 1 is aan de beurt.";
                }
                else
                {
                    beurt = "Speler 2 is aan de beurt.";
                }

                this.label1.Text = beurt;

            }
           
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
                other = 2;
            }
            else
            {
                other = 1;
            }

            if ((row + rowChange < 0) || (row + rowChange > hoog-1))
            {
                return false;
            }
            else if ((column + columnChange < 0) || (column + columnChange > breed-1))
            {
                return false;
            }
            else if (gameState[row + rowChange, column + columnChange] != other)
            {
                return false;
            }
            else if (gameState[row + rowChange, column + columnChange] == other)
            {
                return true; // tijdelijk
            }

            return true;
        }

        private void berekenGeldigeZet(int b, int h)
        {
            int row = b;
            int column = h;

            bool[] geldigePositie = new bool[8];

            if (gameState[row, column] == 0)
            {
                bool nw = geldigeZet(-1, -1, row, column);
                geldigePositie[0] = nw;
                bool nn = geldigeZet(-1, 0, row, column);
                geldigePositie[1] = nn;
                bool ne = geldigeZet(-1, 1, row, column);
                geldigePositie[2] = ne;
                bool ee = geldigeZet(0, 1, row, column);
                geldigePositie[3] = ee;
                bool ww = geldigeZet(0, -1, row, column);
                geldigePositie[4] = ww;
                bool sw = geldigeZet(1, -1, row, column);
                geldigePositie[5] = sw;
                bool ss = geldigeZet(1, 0, row, column);
                geldigePositie[6] = ss;
                bool se = geldigeZet(1, 1, row, column);
                geldigePositie[7] = se;

                for (int i = 0; i < geldigePositie.Length; i++)
                {
                    Console.WriteLine(geldigePositie[i]);
                    if (geldigePositie[i])
                    {
                        if (this.speler1Beurt)
                        {
                            valid[row, column] = 1;
                        }
                        else
                        {
                            valid[row, column] = 2;
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

                // Kleur mag niet zelfde als andere speler en niet wit
                if (leukKleurtje.ShowDialog() == DialogResult.OK && leukKleurtje.Color != Color.White && leukKleurtje.Color != kleurSpeler2)
                    kleurSpeler1 = leukKleurtje.Color;
            }
            else 
            {
                leukKleurtje.Color = kleurSpeler2;
 
                if (leukKleurtje.ShowDialog() == DialogResult.OK && leukKleurtje.Color != Color.White && leukKleurtje.Color != kleurSpeler1)
                    kleurSpeler2 = leukKleurtje.Color;
            }

            this.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            nieuwSpel();
            this.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.hulpModus = !this.hulpModus;
            this.Refresh();
        }
        //methode is de steen ingesloten
        // methode geldige zet

        private void nieuwSpel() {
            int middenX = breed / 2;
            int middenY = hoog / 2;

            for (int b = 0; b < breed; b++)
            {
                for (int h = 0; h < hoog; h++)
                {
                    gameState[b, h] = 0;
                }
            }

            gameState[middenX, middenY] = 1;
            gameState[middenX - 1, middenY - 1] = 1;
            gameState[middenX, middenY - 1] = 2;
            gameState[middenX - 1, middenY] = 2;

            this.speler1Beurt = true;
            this.label1.Text = "Speler 1 is aan de beurt.";
        }
    }

}