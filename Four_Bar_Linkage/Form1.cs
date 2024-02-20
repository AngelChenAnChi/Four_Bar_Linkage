using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Four_Bar_Linkage
{
    public partial class MainForm : Form
    {
        Linkage myLinkage;
        public MainForm()
        {
            InitializeComponent();
            myLinkage = new Linkage();
            property.SelectedObject = myLinkage;
            //make the screen don't flash (draw in memory)
            // form's setstyle
            //SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            //call panal2's setstyle           
            MethodInfo panalSty = typeof(Panel).GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object[] Paras = { ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true };
            panalSty.Invoke(splitContainer1.Panel2, Paras);
        }


        private void btn_result_Click(object sender, EventArgs e)
        {

            double delta = (double)nud.Value;
            double limit = Math.PI * 2;
            rtb_angleresult.Clear();
            for (double angle = 0.0; angle <= limit; angle += delta)
            {
                myLinkage.updateNewPosition(angle);
                rtb_angleresult.AppendText(myLinkage.printCoordinate());
                rtb_angleresult.AppendText("\n");
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            //Draw the graphic
            if (myLinkage != null)
                myLinkage.drawLinkage(e.Graphics, e.ClipRectangle);
            Rectangle r = new Rectangle(0, 0, 4, 4);
            foreach (Point pt in traPt)
            {
                r.X = pt.X - 2;
                r.Y = pt.Y - 2;
                e.Graphics.DrawRectangle(Pens.Orange, r);
            }
        }
        List<Point> traPt = new List<Point>();

        double currentAngle = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            myLinkage.updateNewPosition(currentAngle);
            traPt.Add(myLinkage.trajectoryPt);
            currentAngle += Convert.ToDouble(nud.Value);
            splitContainer1.Panel2.Refresh();
        }
        private void btn_Run_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            if (timer1.Enabled)
                btn_Run.Text = "Stop!";
            else
                btn_Run.Text = "Run";

        }
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            traPt.Clear();
            splitContainer1.Panel2.Refresh();
        
        }


        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.PageSettings.Landscape = true;
            myLinkage.drawLinkage(e.Graphics, e.PageBounds);
        }
        private void btn_Print_Click(object sender, EventArgs e)
        {
            
            if (printPreviewDialog1.ShowDialog() == DialogResult.OK) ;
            printDocument1.Print();
        }

   
    }
}
