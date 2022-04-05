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
    abstract class LikBezUpravljanja: Sprite
    {
        private string naziv;

        public string Naziv
        {
            get { return naziv; }
            set { naziv = value; }
        }

        private int brzina;

        public int Brzina
        {
            get { return brzina; }
            set { brzina = value; }
        }
        public LikBezUpravljanja(string spriteImage, int posX, int posY) : base(spriteImage, posX, posY)
        {
            
        }
    }
}
