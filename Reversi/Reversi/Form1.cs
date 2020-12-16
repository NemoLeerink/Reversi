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
        const int xFormaat = 500;

        int maximaal = Math.Max(breed, hoog);
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
            // Voegt de knoppen en eventhandlers toe
            InitializeComponent();
            
            // Geeft beginwaarden aan de variabelen 
            nieuwSpel();
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
                        gr.FillEllipse(speler2Brush, formaatVakje * b + 3, formaatVakje * h + 3, formaatVakje-6, formaatVakje-6);
                    }
                    else if (gameState[b, h] == 1)
                    {
                        gr.FillEllipse(speler1Brush, formaatVakje * b + 3, formaatVakje * h + 3, formaatVakje - 6, formaatVakje - 6);
                    }
                }
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
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

                // Telt het totaal aantal posities van de spelers
                int somSpeler1 = 0;
                int somSpeler2 = 0;
                string steen1 = "stenen";
                string steen2 = "stenen";
                for (int b = 0; b < breed; b++)
                {
                    for (int h = 0; h < hoog; h++)
                    {
                        if (gameState[b, h] == 1) somSpeler1++;
                        else if (gameState[b, h] == 2) somSpeler2++;
                    }
                }
               
                if (somSpeler1 == 1) steen1 = "steen";
                if (somSpeler2 == 1) steen2 = "steen";

                this.label2.Text = $"Speler 1 heeft {somSpeler1} {steen1}."; 
                this.label3.Text = $"Speler 2 heeft {somSpeler2} {steen2}."; 

                // Draag de beurt naar de andere speler over
                this.speler1Beurt = !this.speler1Beurt;
                if (this.speler1Beurt)
                {
                    this.label1.Text = "Speler 1 is aan de beurt.";
                    this.label1.ForeColor = kleurSpeler1;

                }
                else
                {
                    this.label1.Text = "Speler 2 is aan de beurt.";
                    this.label1.ForeColor = kleurSpeler2;
                }
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
            // Werkt maar kleurt soms teveel in omdat het uitgaat van de nieuwe situatie
            bool[] geldigeRichtingen = new bool[8];
            int[] hoeveelInkleuren = new int[8];
            int[,] richtingen = new int[8, 2] {{ -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 }, { 0, -1 }, { 1, -1 }, { 1, 0}, { 1, 1 }};
    
            // Het vakje moet leeg zijn om er een steen te kunnen plaatsen
            if (gameState[row, column] != 0) return false;

            // Berekend hoeveel stenen er in iedere richting moeten worden ingekleurd. Als dit gelijk is aan 0 is het dus geen geldige richting
            for (int i = 0; i < geldigeRichtingen.Length; i++)
            {
                hoeveelInkleuren[i] = ingesloten(row, column, richtingen[i, 0], richtingen[i, 1]);
                if (hoeveelInkleuren[i] != 0) geldigeRichtingen[i] = true;
                else geldigeRichtingen[i] = false;
            }

            // Staat in een apparte for-loop zodat stenen die deze beurt worden ingekleurd niet op hun beurt weer andere stenen gaan inkleuren
            for (int i = 0; i < geldigeRichtingen.Length; i++)
            {
                inkleuren(row, column, richtingen[i, 0], richtingen[i, 1], hoeveelInkleuren[i]);
            }
          
            // Als een van de richtingen geldig was, is het een geldige zet
            for (int i = 0; i < geldigeRichtingen.Length; i++)
            {
                if (geldigeRichtingen[i] == true) return true;
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
                if (leukKleurtje.ShowDialog() == DialogResult.OK && leukKleurtje.Color != Color.White && leukKleurtje.Color != kleurSpeler2) {
                    kleurSpeler1 = leukKleurtje.Color;
                    this.label1.ForeColor = kleurSpeler1;
                    this.label2.ForeColor = kleurSpeler1;
                }
                
            }
            else 
            {
                leukKleurtje.Color = kleurSpeler2;

                if (leukKleurtje.ShowDialog() == DialogResult.OK && leukKleurtje.Color != Color.White && leukKleurtje.Color != kleurSpeler1) 
                {
                    kleurSpeler2 = leukKleurtje.Color;
                    this.label1.ForeColor = kleurSpeler2;
                    this.label3.ForeColor = kleurSpeler2;
                }
                    
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

            formaatVakje = xFormaat / maximaal;
            hulpModus = false;

            gameState = new int[breed, hoog];
            valid = new int[breed, hoog]; // nog niet zeker of nodig
           
            gameState[middenX, middenY] = 1;
            gameState[middenX - 1, middenY - 1] = 1;
            gameState[middenX, middenY - 1] = 2;
            gameState[middenX - 1, middenY] = 2;

            this.speler1Beurt = true;
            
            this.label1.Text = "Speler 1 is aan de beurt.";
            this.label2.Text = "Speler 1 heeft 2 stenen.";
            this.label3.Text = "Speler 2 heeft 2 stenen.";

            this.label1.ForeColor = kleurSpeler1;
            this.label2.ForeColor = kleurSpeler1;
            this.label3.ForeColor = kleurSpeler2;

            this.ClientSize = new System.Drawing.Size(xFormaat, formaatVakje * hoog + 101);
            this.panel1.Size = new System.Drawing.Size(formaatVakje * breed + 1, formaatVakje * hoog + 1);

            // Zet het paneel in het midden van de client
            int xVanPaneel = (xFormaat - (formaatVakje * breed))/2;

            this.panel1.Location = new Point(xVanPaneel, 100);

        }

        // Deze methode checkt of er binnen de meegegeven richting een steen wordt ingesloten
        // De return waarde is het aantal in te kleuren vakjes in deze richting
        private int ingesloten(int row, int column, int x, int y) {

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
                        return 0;
                    }
                }

                // Als ik op deze rij mezelf nog een keer tegenkom, wordt er een steen ingesloten. We hebben namelijk eerder al vastgesteld dat 
                // de steen direct naast mij van de tegenstander is
                if (gameState[row + t * x, column + t * y] == me) {

                    return t;
                }

                t++;
            }

            return 0;
        }

        // Deze methode kleurt de "overgenomen" posities in
        private void inkleuren(int row, int column, int x, int y, int aantalVakjes) {
            for (int i = 1; i < aantalVakjes; i++)
            {
                if (this.speler1Beurt) gameState[row + i * x, column + i * y] = 1;
                else gameState[row + i * x, column + i * y] = 2;
            }
        }

        // Tekent de twee cirkeltjes op de form in de kleur van de speler 
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillEllipse(new SolidBrush(kleurSpeler1),25, 7, 30, 30);
            e.Graphics.FillEllipse(new SolidBrush(kleurSpeler2),25, 39, 30, 30);
        }
    }

}