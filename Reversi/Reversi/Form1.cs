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
        int muisX;
        int muisY;
        int formaatVakje = 50;
        bool blauwBeurt;

        int[,] gameState = new int[breed, hoog];

        public Form1()
        {
            int middenX = breed / 2;
            int middenY = hoog / 2;

            gameState[middenX, middenY] = 2;
            gameState[middenX-1, middenY-1] = 2;
            gameState[middenX, middenY-1] = 1;
            gameState[middenX - 1, middenY] = 1;

            this.blauwBeurt = true;
            
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.Size = new System.Drawing.Size(formaatVakje*breed+1, formaatVakje*hoog+1);

            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Brush roodBrush = new SolidBrush(Color.Red);
            Brush blauwBrush = new SolidBrush(Color.Blue);

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
                        gr.FillEllipse(roodBrush, formaatVakje * b, formaatVakje * h, formaatVakje, formaatVakje);
                    }
                    else if (gameState[b, h] == 2)
                    {
                        gr.FillEllipse(blauwBrush, formaatVakje * b, formaatVakje * h, formaatVakje, formaatVakje);
                    }
                }
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            this.blauwBeurt = !this.blauwBeurt;
            // if geen geldige beurt, doe weer this.blauwBeurt = !this.blauwBeurt;

            // x / breede per vakje en y / hoogte per vakje
            int vakjeX = this.muisX / this.formaatVakje;
            int vakjeY = this.muisY / this.formaatVakje;

            if (this.blauwBeurt)
            {
                this.gameState[vakjeX, vakjeY] = 2;
            }
            else 
            {
                this.gameState[vakjeX, vakjeY] = 1; 
            }

            this.Refresh();
           
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            this.muisX = e.X;
            this.muisY = e.Y;
        }

        private bool geldigeZet() 
        {
            bool geldigeZet = true;
            
            

            return geldigeZet;
        }
        //methode ingesloten
        // methode geldig
    }
}