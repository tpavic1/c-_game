using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

namespace OTTER
{
    class GlavniLik : Sprite
    {
        private string naziv;
        public string Naziv
        {
            get { return naziv; }
            set
            {
                naziv = value;
                if(naziv=="")
                {
                    MessageBox.Show("Niste unijeli ime!");
                    return;
                }
            }
        }
        private int brzina;
        public int Brzina
        {
            get { return brzina; }
            set { brzina = value; }
        }

        private int brojNamirnica;
        public int BrojNamirnica
        {
            get { return brojNamirnica; }
            set { brojNamirnica = value; }
        }

        public delegate void EventHandler();
        public event EventHandler KrajIgre;

        private int brojZivota;
        public int BrojZivota
        {
            get{ return brojZivota; }
            set
            {
                brojZivota = value;
                if (brojZivota == 0)
                    KrajIgre.Invoke();
            }
        }

        private int skupljeniDezinficijensi;
        public int SkupljeniDezinficijensi
        {
            get { return skupljeniDezinficijensi; }
            set { skupljeniDezinficijensi = value; }
        }

        private int skupljeneMaske;
        public int SkupljeneMaske
        {
            get { return skupljeneMaske; }
            set { skupljeneMaske = value; }
        }


        public override int Y
        {
            get
            {
                return y;
            }
            set
            {
                if (value > GameOptions.DownEdge - 200)
                    y = GameOptions.DownEdge - 200;
                else if (value < 0)
                    y = 0;
                else
                    y = value;
                 
            }
        }

        public GlavniLik(string spriteImage, int posX, int posY):base(spriteImage, posX, posY)
        {
            this.Brzina = 10;
            this.BrojZivota = GameOptions.broj_zivota;
        }

        public bool TouchingSprite(Predmeti p)
        {
            Sprite s = p;
            if (this.TouchingSprite(s))
            {
                p.Dodir = true;
                return true;
            }
            else
                return false;
        }
    }
}
