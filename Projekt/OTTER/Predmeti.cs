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
    class Predmeti:LikBezUpravljanja
    {
        private bool dodir;
        public bool Dodir
        {
            get { return dodir; }
            set { dodir = value; }
        }

        public override int X
        {
            get
            {
                return x;
            }
            set
            {
                if (value <= 0)
                {
                    this.X = GameOptions.RightEdge - this.Width;
                }
                else
                    x = value;
            }
        }
        public Predmeti(string spriteImage, int posX, int posY) : base(spriteImage, posX, posY)
        {
            this.Dodir = false;
        }
    }
}
