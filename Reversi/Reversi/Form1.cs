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
        int formaatVakje;

        int muisX;
        int muisY;
              
        bool speler1Beurt;
        bool hulpModus;

        Color kleurSpeler1;
        Color kleurSpeler2;

        // Lijst met welke posities geldig zijn voor de huidige speler
        int[,] valid;
        int[,] gameState;

        public Form1()
        {
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

            // Later vervangen met kortere code
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


            // Tekent het speelraster
            for (int h = 0; h <= breed; h++)
            {
                gr.DrawLine(penZwart, formaatVakje * h, 0, formaatVakje * h, panel1.Height);
            }

            for (int b = 0; b <= hoog; b++)
            {
                gr.DrawLine(penZwart, 0, formaatVakje * b, panel1.Width, formaatVakje * b);
            }

            // Tekent de schijfjes van de spelers op het speelraster
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
            
            // Bereken het vakje waar op geclickt is
            int vakjeX = this.muisX / this.formaatVakje;
            int vakjeY = this.muisY / this.formaatVakje;

            // berekenGeldigeZet(vakjeX, vakjeY);

            if (this.geldigeZet(vakjeX, vakjeY))
            {
                if (this.speler1Beurt)
                {
                    this.gameState[vakjeX, vakjeY] = 1;
                }
                else
                {
                    this.gameState[vakjeX, vakjeY] = 2;
                }
            }
            else {
                redo = true;
            }

            // Mits de gemaakte zet geldig was wordt de beurt overgedragen
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

        // Checkt voor alle richtingen of het een geldige zet is 
        private bool geldigeZet(int row, int column)
        {
            if (gameState[row, column] != 0) 
            {
                return false;
            }
            if (ingesloten(row, column, -1, -1)) 
            {
                return true;
            }
            if (ingesloten(row, column, -1, 0))
            {
                return true;
            }
            if (ingesloten(row, column, -1, 1))
            {
                return true;
            }
            if (ingesloten(row, column, 0, 1))
            {
                return true;
            }
            if (ingesloten(row, column, 0, -1))
            {
                return true;
            }
            if (ingesloten(row, column, 1, -1))
            {
                return true;
            }
            if (ingesloten(row, column, 1, 0))
            {
                return true;
            }
            if (ingesloten(row, column, 1, 1))
            {
                return true;
            }

            return false;
        }

        // Deze knop/methode zorgt ervoor dat spelers zelf hun kleur kunnen kiezen
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

        // Deze methode laat spelers met behulp van de knop een nieuw spel starten
        private void button2_Click(object sender, EventArgs e)
        {
            nieuwSpel();
            this.Refresh();
        }

        // Deze methode zet de hulp functie aan/uit
        private void button3_Click(object sender, EventArgs e)
        {
            this.hulpModus = !this.hulpModus;
            this.Refresh();
        }

        // Deze methode wordt aangeroepen wanneer er een nieuw spel wordt gestart
        private void nieuwSpel() {
            int middenX = breed / 2;
            int middenY = hoog / 2;

            kleurSpeler1 = Color.Blue;
            kleurSpeler2 = Color.Red;

            formaatVakje = minimaalFormaat / maximaal;
            hulpModus = false;

            gameState = new int[breed, hoog];
            valid = new int[breed, hoog]; // nog niet zeker of nodig
           
            gameState[middenX, middenY] = 1;
            gameState[middenX - 1, middenY - 1] = 1;
            gameState[middenX, middenY - 1] = 2;
            gameState[middenX - 1, middenY] = 2;

            this.speler1Beurt = true;
            this.label1.Text = "Speler 1 is aan de beurt.";
        }

        // Deze methode checkt of er binnen de meegegeven richting een steen wordt ingesloten
        private bool ingesloten(int row, int column, int x, int y) {
            int other;
            int me;
            
            if (this.speler1Beurt)
            {
                other = 2;
                me = 1;
            }
            else {
                other = 1;
                me = 2;
            }

            int t = 1;
            // Blijf de richting checken behalve als je buiten de marges gaat.
            while ((row + x * t >= 0) && (row + x * t < breed) && (column + y * t >= 0) && (column + y * t < hoog)) {
                
                // Zit ik naast een steen van een andere kleur met deze richting?
                // Ja, ga verder
                // Nee, return false
                if (t == 1) {
                    if (gameState[row + x, column + y] != other)
                    {
                        return false;
                    }
                }

                // Als ik op deze rij mezelf nog een keer tegenkom, wordt er een steen ingesloten. We hebben namelijk eerder al vastgesteld dat 
                // de steen direct naast mij van de tegenstander is
                if (gameState[row + t * x, column + t * y] == me) {
                    return true;
                }

                t++;
            }

            // Helaas wordt er geen steen ingesloten
            return false;
        }
    }

}