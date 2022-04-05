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
    class Ljudi:LikBezUpravljanja
    {
        public override int Y
        {
            get
            {
                return y;
            }
            set
            {
                if (value >= GameOptions.DownEdge)
                {
                    this.Y = 0;
                }
                else
                    y = value;
            }
        }
        public Ljudi(string spriteImage, int posX, int posY) : base(spriteImage, posX, posY)
        {
        }
    }
}
