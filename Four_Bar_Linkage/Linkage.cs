using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace Four_Bar_Linkage
{
    class Linkage
    {
        // size of the canvas we want on the screen
        int xoff, yoff;
        double scale;

        //length of linkages
        double Lg = 20.0; //frame 
        double Ld = 10.0; //driver 
        double Lc = 35.0; //coupler
        double Lf = 25.0; //follower 
        double Da = 17.5; //axial offset on coupler
        double Db = 5.0; //radial offset on coupler
        double alpha = 1.0; //driving angle
        
        //pins of linkages
        PointF P1 = new PointF(0, 0);
        PointF P2 = new PointF();
        PointF P3 = new PointF();
        PointF P4 = new PointF();
        PointF Pp = new PointF();

        // public properties
        [Category("Four-Bar Linkage")]
        [DisplayName("Frame  (Green)")]
        [Description("The length of the frame.")]
        public double GroundLength
        {
            get => Lg;
            set
            {
                double length = Lg;
                Lg = value;
                if (!isLinkageFeasible())  //if linkage is not feasible, return to default
                    Lg = length;
            }
        }

        [Category("Four-Bar Linkage")]
        [DisplayName("Driver (Red)")]
        [Description("The length of the driver.")]
        public double DriverLength
        {
            get => Ld;
            set
            {
                double length = Ld;
                Ld = value;
                if (!isLinkageFeasible()) //if linkage is not feasible, return to default
                    Ld = length; 
            }
        }

        [Category("Four-Bar Linkage")]
        [DisplayName("Coupler  (Yellow)")]
        [Description("The length of the coupler.")]
        public double ConnectorLength
        {
            get => Lc;
            set
            {
                double length = Lc;
                Lc = value;
                if (!isLinkageFeasible())  //if linkage is not feasible, return to default
                    Lc = length;
            }
        }

        [Category("Four-Bar Linkage")]
        [DisplayName("Follower  (Blue)")]
        [Description("The length of the follower.")]
        public double FollowerLength
        {
            get => Lf;
            set
            {
                double length = Lf;
                Lf = value;
                if (!isLinkageFeasible())  //if linkage is not feasible, return to default
                    Lf = length;
            }
        }

        [Category("Track Point")]
        [Description("axial offset on coupler")]
        public double AxialOffset
        {
            get => Da;
            set { Da = value; }
        }
        [Category("Track Point")]
        [Description("radial offset on coupler")]
        public double RadiallOffset
        {
            get => Db;
            set { Db = value; }
        }

        // Check the four-bar linkage is feasible
        bool isLinkageFeasible()
        {
            if (Lg > Lc + Ld + Lf || Lg == Lc + Ld + Lf || Lc > Lg + Ld + Lf || Lc == Lg + Ld + Lf ||
                Ld > Lc + Lg + Lf || Ld == Lc + Lg + Lf || Lf > Lc + Ld + Lg || Lf == Lc + Ld + Lg)
                return false;
            else
                return true;
        }
        //If the four-bar linkage for newAlpha angle is feasible, return true and update current alpha.
        public bool updateNewPosition(double newAlpha)
        {
            P2.X = (float)Lg;
            P2.Y = 0;
            P4.X = (float)Ld * (float)Math.Cos(newAlpha);
            P4.Y = (float)Ld * (float)Math.Sin(newAlpha);

            double L;
            L = Math.Pow(Math.Pow(Lg - P4.X, 2) + Math.Pow(P4.Y, 2), 0.5);
            if (((Lf + Lc) < L) || ((L + Lc) < Lf) || ((L + Lf) < Lc)) return false; // not feasible
            alpha = newAlpha; // feasible, unpdate new alpha

            double omega, theta, delta;

            omega = Math.Acos((Math.Pow(Lf, 2) + Math.Pow(L, 2) - Math.Pow(Lc, 2)) / 2 / Lf / L);
            delta = Math.Atan2(P4.Y, (P4.X - Lg));
            theta = delta - omega;

            P3.X = (float)Lf * (float)Math.Cos(theta) + (float)Lg;
            P3.Y = (float)Lf * (float)Math.Sin(theta);
            Pp.X = P4.X + (P3.X - P4.X) * (float)Da / (float)Lc + (P4.X - P1.X) * (float)Db / (float)Ld;
            Pp.Y = P4.Y + (P3.Y - P4.Y) * (float)Da / (float)Lc + (P4.Y - P1.Y) * (float)Db / (float)Ld;

            alpha = newAlpha;
            return true;
        }

        //print the result (each point's coordinate) on the screen
        public string printCoordinate()
        {
            return $"Angle = {alpha: 0.00}   P1=({P1.X},{P1.Y})   P2=({P2.X},{P2.Y})   P3=({P3.X:0.00},{P3.Y:0.00})   P4({P4.X:0.00},{P4.Y:0.00})   Pp({Pp.X:0.00},{Pp.Y:0.00})";
        }

        // Draw the four-bar linkage on the screen.
        public void drawLinkage(Graphics g, Rectangle rect)
        {
            scale = rect.Width / (Lc + Lf + Lg) * 1.1;
            xoff = (int)(Ld * scale * 2.0f);
            yoff = rect.Height / 2;
            //set screen point
            Point s1, s2, s3, s4, sp;
            s1 = transPoint(P1);
            s2 = transPoint(P2);
            s3 = transPoint(P3);
            s4 = transPoint(P4);
            sp = transPoint(Pp);
            //connect points
            float penW = rect.Height / 40;
            Pen myPen = new Pen(Color.Green, penW);
            myPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            myPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            g.DrawLine(myPen, s1, s2);
            myPen.Color = Color.Blue;
            g.DrawLine(myPen, s2, s3);
            myPen.Color = Color.Yellow;
            g.DrawLine(myPen, s3, s4);
            myPen.Color = Color.Red;
            g.DrawLine(myPen, s4, s1);
            myPen.Color = Color.Brown;
            g.DrawLine(myPen, s3, sp);
            g.DrawLine(myPen, sp, s4);
            //draw pins
            float pinW = rect.Width / 40;
            Pen myPoint = new Pen(Color.White, pinW);
            g.DrawRectangle(myPoint, s1.X - 1, s1.Y - 1, 0.4f, 0.4f);
            g.DrawRectangle(myPoint, s2.X - 1, s2.Y - 1, 0.4f, 0.4f);
            g.DrawRectangle(myPoint, s3.X - 1, s3.Y - 1, 0.4f, 0.4f);
            g.DrawRectangle(myPoint, s4.X - 1, s4.Y - 1, 0.4f, 0.4f);
            myPoint.Color = Color.Orange;
            g.DrawRectangle(myPoint, sp.X - 1, sp.Y - 1, 0.4f, 0.4f);
        }
        //set the screen coordinate of the linkage point
        Point transPoint(PointF pt)
        {
            Point p = Point.Empty;
            p.X = (int)(xoff + pt.X * scale);
            p.Y = (int)(yoff + pt.Y * scale);
            return p;
        }

        //trajectory points
        public Point trajectoryPt
        {
            get { return transPoint(Pp); }
        }
    }
}
