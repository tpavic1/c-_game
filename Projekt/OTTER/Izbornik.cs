using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OTTER
{
    public partial class Izbornik : Form
    {
        public Izbornik()
        {        
           
            InitializeComponent();
            
        }
        
        private void button1_Click(object sender, EventArgs e)
        {

            label3.Text = "Prvi zadatak je sakupiti 7 maski i 5 dezinficijensa. " + "\n Krećeš se pomoću strelica gore i dolje."+
                        "\nAko uspješno obavite \nzadatak, slijedi drugi dio igrice - dućan.";
            
            

            button5.Show();

            button1.Hide();
        }

        private void Izbornik_Load(object sender, EventArgs e)
        {
            button2.Hide();
            button3.Hide();
            button4.Hide();
            button5.Hide();
            button6.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            GameOptions.broj_zivota = 3;
      
            BGL.START = true;
            BGL igra = new BGL();
            igra.Izbornik = this;
            igra.ShowDialog();          
        }

        private void button3_Click(object sender, EventArgs e)
        {                   
            GameOptions.broj_zivota = 2;
        
            BGL.START = true;
            BGL igra = new BGL();
            igra.Izbornik = this;
            igra.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {                         
            GameOptions.broj_zivota = 1;
           
            BGL.START = true;
            BGL igra = new BGL();
            igra.Izbornik = this;
            igra.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label3.Text = "Igrica ima tri razine: lako, srednje i teško. \nOvisno o razini, mijenja se broj života.";
            button2.Show();
            button3.Show();
            button4.Show();
            button6.Show();
            button5.Hide();    
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
