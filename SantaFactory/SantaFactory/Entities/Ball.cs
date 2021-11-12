using SantaFactory.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SantaFactory.Entities
{

    public class Ball : Toy 
    {
        Random _rng = new Random();
        public SolidBrush BallColor { get; private set; }
        public Ball(Color color)
        {
            BallColor = new SolidBrush(color);
            Click += Ball_Click;
        }

        private void Ball_Click(object sender, EventArgs e)
        {
            var color = Color.FromArgb(_rng.Next(256), _rng.Next(256),_rng.Next(256));
            BallColor = new SolidBrush(color);
            Invalidate();
        }

        protected override void Drawimage(Graphics g)
        {
            
            g.FillEllipse(
                BallColor,
                0,
                0,
                Width,
                Height);
        }
 
    }
}
