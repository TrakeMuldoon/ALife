using System.Configuration;

namespace RunningDots
{
    public partial class BioRunner : Form
    {

        Rectangle GridRectangle;
        Simulation theSim;

        public BioRunner()
        {
            InitializeComponent();

            Paint += BioRunner_Paint;

            theSim = new Simulation(32423423, 5);
            foreach(Color c in theSim.colours)
            {
                Brush b = new SolidBrush(c);
                brushes.Add(c, b);
            }


            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            GridRectangle = new Rectangle(new Point(0, 0)
                                            , new Size(theSim.worldGrid.gridWidthInCells * theSim.worldGrid.cellWidth
                                                        , theSim.worldGrid.gridHeightInCells * theSim.worldGrid.cellHeight));

            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Tick += T_Tick;
            t.Interval = 66;
            t.Start();

            //RunSim(theSim);


        }

        private void T_Tick(object? sender, EventArgs e)
        {
            this.Invalidate();
        }

        private float PenWidth = 3;
        int counter = 0;

        Pen GridPen = new Pen(Color.Bisque, (float)1.5);
        Brush BackgroundBrush = new SolidBrush(Color.AntiqueWhite);
        Dictionary<Color, Brush> brushes = new Dictionary<Color, Brush>();

        private void BioRunner_Paint(object? sender, PaintEventArgs e)
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

        protected override void OnPaintBackground(PaintEventArgs p)
        {

        }
    }
}
