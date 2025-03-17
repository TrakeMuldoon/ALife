﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace RunningDots
{
    public partial class BallForm : Form
    {
        Bitmap Backbuffer;

        const int BallAxisSpeed = 2;

        Point BallPos = new Point(30, 30);
        Point BallSpeed = new Point(BallAxisSpeed, BallAxisSpeed);
        const int BallSize = 50;

        public BallForm()
        {
            InitializeComponent();

            this.SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.DoubleBuffer, true);

            System.Windows.Forms.Timer GameTimer = new System.Windows.Forms.Timer();
            GameTimer.Interval = 10;
            GameTimer.Tick += new EventHandler(GameTimer_Tick);
            GameTimer.Start();

            this.ResizeEnd += new EventHandler(BallForm_CreateBackBuffer);
            this.Load += new EventHandler(BallForm_CreateBackBuffer);
            this.Paint += new PaintEventHandler(BallForm_Paint);

            this.KeyDown += new KeyEventHandler(BallForm_KeyDown);
        }

        void BallForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Left)
                BallSpeed.X = -BallAxisSpeed;
            else if(e.KeyCode == Keys.Right)
                BallSpeed.X = BallAxisSpeed;
            else if(e.KeyCode == Keys.Up)
                BallSpeed.Y = -BallAxisSpeed; // Y axis is downwards so -ve is up.
            else if(e.KeyCode == Keys.Down)
                BallSpeed.Y = BallAxisSpeed;
        }

        void BallForm_Paint(object sender, PaintEventArgs e)
        {
            if(Backbuffer != null)
            {
                e.Graphics.DrawImageUnscaled(Backbuffer, Point.Empty);
            }
        }

        void BallForm_CreateBackBuffer(object sender, EventArgs e)
        {
            if(Backbuffer != null)
                Backbuffer.Dispose();

            Backbuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
        }

        void Draw()
        {
            if(Backbuffer != null)
            {
                using(var g = Graphics.FromImage(Backbuffer))
                {
                    g.Clear(Color.White);
                    g.FillEllipse(Brushes.Black, BallPos.X - BallSize / 2, BallPos.Y - BallSize / 2, BallSize, BallSize);
                }

                Invalidate();
            }
        }

        void GameTimer_Tick(object sender, EventArgs e)
        {
            BallPos.X += BallSpeed.X;
            BallPos.Y += BallSpeed.Y;


            Draw();

            // TODO: Add the notion of dying (disable the timer and show a message box or something)
        }
    }
}