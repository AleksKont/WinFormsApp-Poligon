

using Point = Poligon.Point;

namespace WinFormsApp
{
    public partial class Form1 : Form
    {
        private MeshContext? _context;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            DisplayContext();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var points = new List<Point>()
            {
                new() { X = -6, Y = 4 },
                new() { X = -1, Y = 3 },
                new() { X = 1, Y = 4 },
                new() { X = 2, Y = 7 },
                new() { X = -1, Y = 9 },
                new() { X = -5, Y = 8 },
                new() { X = -8, Y = 6 },
            };

            _context = new()
            {
                points = points,
                triangles = new Poligon.Poligon().CreateMesh(points)
            };

            DisplayContext();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var points = new List<Point>()
            {
                new() { X = -6, Y = 4 },
                new() { X = -1, Y = 3 },
                new() { X = 1, Y = 4 },
                new() { X = 2, Y = 7 },
                new() { X = -6.95F, Y = 6 },
                new() { X = -6, Y = 8 },
                new() { X = -7, Y = 9 },
                new() { X = -8, Y = 6 },
            };

            _context = new()
            {
                points = points,
                triangles = new Poligon.Poligon().CreateMesh(points)
            };

            DisplayContext();
        }

        private void DisplayContext()
        {
            if (_context is null)
                return;

            var points = _context.points;
            var triangles = _context.triangles;
            var g = panel1.CreateGraphics();
            g.Clear(Color.LightGray);
            DrawGrid(g);

            //------------------------------------------------------

            var pen = new Pen(Color.Red, 4);
            var brush2 = new SolidBrush(Color.GreenYellow);
            for (int i = 0; i < points.Count; i++)
            {
                System.Drawing.Point A = toGraphicsCoord(points[i], panel1.Width, panel1.Height);
                System.Drawing.Point B = toGraphicsCoord(points[(i + 1) % points.Count], panel1.Width, panel1.Height);
                g.DrawLine(pen, A, B);
                g.FillEllipse(brush2, A.X - 5, A.Y - 5, 10, 10);
            }

            //------------------------------------------------------

            pen = new Pen(Color.Blue, 2);
            Font drawFont = new Font("Arial", 10);
            var brush = new SolidBrush(Color.Red);
            int j = 1;
            foreach (var (a, b, c) in triangles)
            {
                System.Drawing.Point A = toGraphicsCoord(a, panel1.Width, panel1.Height);
                System.Drawing.Point B = toGraphicsCoord(b, panel1.Width, panel1.Height);
                System.Drawing.Point C = toGraphicsCoord(c, panel1.Width, panel1.Height);
                g.DrawLine(pen, A, B);
                g.DrawLine(pen, B, C);
                g.DrawLine(pen, C, A);
                g.DrawString($"{j++}", drawFont, brush, new System.Drawing.Point((A.X + B.X + C.X) / 3, (A.Y + B.Y + C.Y) / 3));
            }
        }

        private void DrawGrid(Graphics g)
        {
            var pen = new Pen(Color.DarkOliveGreen);
            var brush = new SolidBrush(Color.Red);
            Font drawFont = new Font("Arial", 10);
            for (int i = -_context.Scale; i < _context.Scale; i++)
            {
                System.Drawing.Point A = toGraphicsCoord(new Point() { X = i, Y = +_context.Scale }, panel1.Width, panel1.Height);
                System.Drawing.Point B = toGraphicsCoord(new Point() { X = i, Y = -_context.Scale }, panel1.Width, panel1.Height);
                g.DrawLine(pen, A, B);
                g.DrawString($"{i}", drawFont, brush, A);
                System.Drawing.Point C = toGraphicsCoord(new Point() { Y = i, X = +_context.Scale }, panel1.Width, panel1.Height);
                System.Drawing.Point D = toGraphicsCoord(new Point() { Y = i, X = -_context.Scale }, panel1.Width, panel1.Height);
                g.DrawLine(pen, C, D);
                g.DrawString($"{i}", drawFont, brush, D);
            }

            pen = new Pen(Color.DarkGreen, 2);
            System.Drawing.Point A1 = toGraphicsCoord(new Point() { X = 0, Y = +_context.Scale }, panel1.Width, panel1.Height);
            System.Drawing.Point B1 = toGraphicsCoord(new Point() { X = 0, Y = -_context.Scale }, panel1.Width, panel1.Height);
            System.Drawing.Point C1 = toGraphicsCoord(new Point() { Y = 0, X = +_context.Scale }, panel1.Width, panel1.Height);
            System.Drawing.Point D1 = toGraphicsCoord(new Point() { Y = 0, X = -_context.Scale }, panel1.Width, panel1.Height);
            g.DrawLine(pen, A1, B1);
            g.DrawLine(pen, C1, D1);
        }

        private System.Drawing.Point toGraphicsCoord(Point a, int widht, int height) =>
            new((int)(a.X * widht * 0.5 / _context.Scale + widht * 0.5), (int)(-a.Y * height * 0.5 / _context.Scale + height * 0.5));

    }

    internal class MeshContext
    {
        public List<Point> points { get; set; }
        public List<(Point, Point, Point)> triangles { get; set; }

        public int Scale = 10;
    }
}