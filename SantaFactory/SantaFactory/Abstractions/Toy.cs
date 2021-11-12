﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SantaFactory.Abstractions
{

    public abstract class Toy : Label 
    {
        public Toy()
        {
            AutoSize = false;
            Width = 50;
            Height = Width;
            Paint += Toy_Paint;
        }

        private void Toy_Paint(object sender, PaintEventArgs e)
        {
            Drawimage(e.Graphics);
        }

        protected abstract void Drawimage(Graphics g);
        
        public void MoveToy()
        {
            Left++;
        }
    }
}
