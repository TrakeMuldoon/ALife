using System;
using System.Drawing;
using System.Windows.Forms;

namespace RunningDots
{
    public partial class DotsForm : Form
    {
        Rectangle GridRectangle;
        Simulation theSim;
        Dictionary<Color, Brush> brushes = new Dictionary<Color, Brush>();

        public DotsForm()
        {
            InitializeComponent();

            this.SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.DoubleBuffer, true);

            theSim = new Simulation(StaticRandom.NextInt(int.MaxValue), 6);
            foreach(Color c in theSim.colours)
            {
                Brush b = new SolidBrush(c);
                brushes.Add(c, b);
            }

            GridRectangle = new Rectangle(new Point(0, 0)
                                            , new Size(theSim.worldGrid.gridWidthInCells * theSim.worldGrid.cellWidth
                                                        , theSim.worldGrid.gridHeightInCells * theSim.worldGrid.cellHeight));


            System.Windows.Forms.Timer GameTimer = new System.Windows.Forms.Timer();
            GameTimer.Interval = 60;
            GameTimer.Tick += new EventHandler(GameTimer_Tick);
            GameTimer.Start();

            this.Paint += new PaintEventHandler(DotsForm_Paint);

            this.KeyDown += new KeyEventHandler(DotsForm_KeyDown);
        }

        void DotsForm_KeyDown(object? sender, KeyEventArgs e)
        {
        }

        private float PenWidth = 3;
        int counter = 0;

        Pen GridPen = new Pen(Color.Bisque, (float)1.5);
        Brush BackgroundBrush = new SolidBrush(Color.AntiqueWhite);

        private void BioRunner_Paint(object? sender, PaintEventArgs e)
        {
        }

        void DotsForm_Paint(object? sender, PaintEventArgs e)
        {
            //Draw background
            e.Graphics.FillRectangle(BackgroundBrush, GridRectangle);

            //Draw Grid
            for(int x = 0; x <= theSim.worldGrid.gridWidthInCells; x++)
            {
                e.Graphics.DrawLine(GridPen
                                    , new Point(x * theSim.worldGrid.cellWidth, 0)
                                    , new Point(x * theSim.worldGrid.cellWidth, GridRectangle.Bottom));
            }
            for(int y = 0; y <= theSim.worldGrid.gridHeightInCells; y++)
            {
                e.Graphics.DrawLine(GridPen
                                    , new Point(0, y * theSim.worldGrid.cellHeight)
                                    , new Point(GridRectangle.Right, y * theSim.worldGrid.cellHeight));
            }
            e.Graphics.DrawLine(new Pen(Color.Blue, PenWidth), new Point(counter * 10, 0), new Point(counter * 10, 200));
            counter += 1;

            foreach(BioCell bc in theSim.agents)
            {
                e.Graphics.FillEllipse(brushes[bc.myColour]
                    , (float)bc.Location.X - bc.Radius
                    , (float)bc.Location.Y - bc.Radius
                    , bc.Radius * 2
                    , bc.Radius * 2);
            }
        }

        void Draw()
        {

        }

        void GameTimer_Tick(object? sender, EventArgs e)
        {
            theSim.RunForegroundStep();
            Invalidate();
            Draw();
        }
    }
}