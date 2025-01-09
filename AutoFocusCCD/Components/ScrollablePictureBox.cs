using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AutoFocusCCD.Components
{
    public partial class ScrollablePictureBox : PictureBox
    {

        Point startPoint;
        int preX, preY;
        public Rectangle rect;
        bool pressOut = false;
        bool moving;
        bool isDragging;
        public bool IsBounding = true;
        protected int frameWidth = 5;
        protected int minSize = 5;
        protected int startDragX, startDragY;
        protected bool resizeLeft, resizeTop, resizeRight, resizeBottom, move;
        int selX, selY, selW, selH;
        int offset;
        Point currentScrollPos;
        readonly System.Windows.Forms.Timer myTimer;
        public ScrollablePictureBox()
        {
            InitializeComponent();
            rect = Rectangle.Empty;

            myTimer = new System.Windows.Forms.Timer();
            myTimer.Tick += new EventHandler(TimerOnTick);
            myTimer.Interval = 500;
            myTimer.Start();

        }

        public ScrollablePictureBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            rect = Rectangle.Empty;

            myTimer = new System.Windows.Forms.Timer();
            myTimer.Tick += new EventHandler(TimerOnTick);
            myTimer.Interval = 500;
            myTimer.Start();

        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            if (rect != Rectangle.Empty)
            {
                offset += 3;
                if (offset > 9)
                {
                    offset = 0;
                }
                this.Invalidate(new Rectangle(rect.X, rect.Y, rect.Width + 1, rect.Height + 1));
            }
        }

        public Rectangle GetRect() => rect;

        public Rectangle GetRectOriginal(Rectangle rectangle)
        {
            if (rectangle == Rectangle.Empty) return Rectangle.Empty;
            if (this.Image == null) return Rectangle.Empty;

            // Original image size
            float originalWidth = this.Image.Width;
            float originalHeight = this.Image.Height;

            // Control size (picture box size)
            float controlWidth = this.Width;
            float controlHeight = this.Height;

            // Calculate aspect ratios
            float imageAspectRatio = originalWidth / originalHeight;
            float controlAspectRatio = controlWidth / controlHeight;

            // Calculate the scaling factors depending on how the image fits inside the control
            float scaleFactorX = controlWidth / originalWidth;
            float scaleFactorY = controlHeight / originalHeight;

            float offsetX = 0;
            float offsetY = 0;

            if (imageAspectRatio > controlAspectRatio)
            {
                // Image is wider than the control
                scaleFactorY = scaleFactorX;
                offsetY = (controlHeight - originalHeight * scaleFactorY) / 2;
            }
            else
            {
                // Image is taller than the control
                scaleFactorX = scaleFactorY;
                offsetX = (controlWidth - originalWidth * scaleFactorX) / 2;
            }

            int x = (int)((rectangle.X - offsetX) / scaleFactorX);
            int y = (int)((rectangle.Y - offsetY) / scaleFactorY);
            int w = (int)(rectangle.Width / scaleFactorX);
            int h = (int)(rectangle.Height / scaleFactorY);

            return new Rectangle(x, y, w, h);
        }

        public Rectangle GetRectOriginal()
        {
            if (rect == Rectangle.Empty) return Rectangle.Empty;
            if (this.Image == null) return Rectangle.Empty;

            // Original image size
            float originalWidth = this.Image.Width;
            float originalHeight = this.Image.Height;

            // Control size (picture box size)
            float controlWidth = this.Width;
            float controlHeight = this.Height;

            // Calculate aspect ratios
            float imageAspectRatio = originalWidth / originalHeight;
            float controlAspectRatio = controlWidth / controlHeight;

            // Calculate the scaling factors depending on how the image fits inside the control
            float scaleFactorX = controlWidth / originalWidth;
            float scaleFactorY = controlHeight / originalHeight;

            float offsetX = 0;
            float offsetY = 0;

            if (imageAspectRatio > controlAspectRatio)
            {
                // Image is wider than the control
                scaleFactorY = scaleFactorX;
                offsetY = (controlHeight - originalHeight * scaleFactorY) / 2;
            }
            else
            {
                // Image is taller than the control
                scaleFactorX = scaleFactorY;
                offsetX = (controlWidth - originalWidth * scaleFactorX) / 2;
            }

            int x = (int)((rect.X - offsetX) / scaleFactorX);
            int y = (int)((rect.Y - offsetY) / scaleFactorY);
            int w = (int)(rect.Width / scaleFactorX);
            int h = (int)(rect.Height / scaleFactorY);

            return new Rectangle(x, y, w, h);
        }

        public Rectangle GetRectangleWithPictureBox(Rectangle rectangle)
        {
            if (rectangle == Rectangle.Empty) return Rectangle.Empty;
            if (this.Image == null) return Rectangle.Empty;

            // Original image size
            float originalWidth = this.Image.Width;
            float originalHeight = this.Image.Height;

            // Control size (picture box size)
            float controlWidth = this.Width;
            float controlHeight = this.Height;

            // Calculate aspect ratios
            float imageAspectRatio = originalWidth / originalHeight;
            float controlAspectRatio = controlWidth / controlHeight;

            // Calculate the scaling factors depending on how the image fits inside the control
            float scaleFactorX = controlWidth / originalWidth;
            float scaleFactorY = controlHeight / originalHeight;

            float offsetX = 0;
            float offsetY = 0;

            if (imageAspectRatio > controlAspectRatio)
            {
                // Image is wider than the control
                scaleFactorY = scaleFactorX;
                offsetY = (controlHeight - originalHeight * scaleFactorY) / 2;
            }
            else
            {
                // Image is taller than the control
                scaleFactorX = scaleFactorY;
                offsetX = (controlWidth - originalWidth * scaleFactorX) / 2;
            }

            int x = (int)(rectangle.X * scaleFactorX + offsetX);
            int y = (int)(rectangle.Y * scaleFactorY + offsetY);
            int w = (int)(rectangle.Width * scaleFactorX);
            int h = (int)(rectangle.Height * scaleFactorY);

            return new Rectangle(x, y, w, h);
        }

        public void SetRect(Rectangle rectangle, bool isOrigin = false)
        {
            if (rectangle == Rectangle.Empty)
            {
                rect = Rectangle.Empty;
                this.Invalidate();
                return;
            }

            if (this.Image == null) return;

            if (isOrigin)
            {
                this.rect = GetRectangleWithPictureBox(rectangle);
            }
            else
            {
                this.rect = rectangle;
            }

            this.Invalidate();
        }

        public void Deselect()
        {
            startPoint = Point.Empty;
            rect = Rectangle.Empty;
        }

        /// <summary>
        /// Segmented regions.
        /// </summary>
        public Dictionary<Color, List<Rectangle>> SegmentedRegions
        { get; set; }

        protected override void OnPaint(PaintEventArgs pe)
        {

            if (this.Image == null) return;


            base.OnPaint(pe);


            if (!IsBounding) return;

            // draw segmented regions
            if (SegmentedRegions != null)
            {
                //int x = (this.getWidth() - this.getIcon().getIconWidth()) / 2;
                //int y = (this.getHeight() - this.getIcon().getIconHeight()) / 2;

                Graphics g = (Graphics)pe.Graphics;

                foreach (Color color in SegmentedRegions.Keys)
                {
                    // Create pen
                    Pen pen = new Pen(color);

                    foreach (Rectangle region in SegmentedRegions[color])
                    {
                        g.DrawRectangle(pen, region);
                    }

                    pen.Dispose();
                }
            }

            if (rect != Rectangle.Empty)
            {
                Graphics g = (Graphics)pe.Graphics;

                // Create pen
                Pen redPen = new Pen(Color.Red);
                //redPen.DashStyle = DashStyle.Solid;

                List<Rectangle> squares = createSquares(rect);
                foreach (Rectangle square in squares)
                {
                    g.DrawRectangle(redPen, square);
                }

                redPen.DashCap = DashCap.Round;
                redPen.LineJoin = LineJoin.Round;
                redPen.MiterLimit = 0;
                redPen.DashPattern = new float[] { 6, 6 };
                redPen.DashOffset = offset;

                try
                {
                    g.DrawRectangle(redPen, rect);
                }
                catch (OutOfMemoryException e)
                {
                    Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + rect.ToString());
                }

                redPen.Dispose();
            }
        }
        /**
      * Creates grip squares.
      *
      */
        List<Rectangle> createSquares(Rectangle rect)
        {
            List<Rectangle> ar = new List<Rectangle>();
            if (moving)
            {
                return ar;
            }

            int wh = 6;

            int x = rect.X - wh / 2;
            int y = rect.Y - wh / 2;
            int w = rect.Width;
            int h = rect.Height;

            ar.Add(new Rectangle(x, y, wh, wh));
            ar.Add(new Rectangle(x + w / 2, y, wh, wh));
            ar.Add(new Rectangle(x + w, y, wh, wh));
            ar.Add(new Rectangle(x + w, y + h / 2, wh, wh));
            ar.Add(new Rectangle(x + w, y + h, wh, wh));
            ar.Add(new Rectangle(x + w / 2, y + h, wh, wh));
            ar.Add(new Rectangle(x, y + h, wh, wh));
            ar.Add(new Rectangle(x, y + h / 2, wh, wh));

            return ar;
        }

        private void ScrollablePictureBox_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;

                if (rect != null)
                {
                    moving = false;
                    pressOut = false;
                    this.Invalidate();
                }
            }
        }

        private void ScrollablePictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                MouseDrag(sender, e);
            }

            if (rect != Rectangle.Empty)
            {
                int selX = rect.X;
                int selY = rect.Y;
                int selW = rect.Width;
                int selH = rect.Height;

                Rectangle leftFrame = new Rectangle(selX, selY, frameWidth, selH);
                Rectangle topFrame = new Rectangle(selX, selY, selW, frameWidth);
                Rectangle rightFrame = new Rectangle(selX + selW - frameWidth, selY, frameWidth, selH);
                Rectangle bottomFrame = new Rectangle(selX, selY + selH - frameWidth, selW, frameWidth);

                Point p = e.Location;

                bool isInside = rect.Contains(p);
                bool isLeft = leftFrame.Contains(p);
                bool isTop = topFrame.Contains(p);
                bool isRight = rightFrame.Contains(p);
                bool isBottom = bottomFrame.Contains(p);

                if (isLeft && isTop)
                {
                    this.Cursor = Cursors.SizeNWSE;
                }
                else if (isTop && isRight)
                {
                    this.Cursor = Cursors.SizeNESW;
                }
                else if (isRight && isBottom)
                {
                    this.Cursor = Cursors.SizeNWSE;
                }
                else if (isBottom && isLeft)
                {
                    this.Cursor = Cursors.SizeNESW;
                }
                else if (isLeft)
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else if (isTop)
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else if (isRight)
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else if (isBottom)
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else if (isInside)
                {
                    this.Cursor = Cursors.SizeAll;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        public void MouseDrag(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X;
                int y = e.Y;

                if (pressOut)
                {
                    rect.X = Math.Min(startPoint.X, x);
                    rect.Y = Math.Min(startPoint.Y, y);
                    rect.Width = Math.Abs(x - startPoint.X);
                    rect.Height = Math.Abs(y - startPoint.Y);
                    moving = true;
                    this.Invalidate();
                }
                else
                {
                    int diffX = startDragX - x;
                    int diffY = startDragY - y;

                    if (resizeLeft)
                    {
                        rect.X = selX - diffX;
                        rect.Width = selW + diffX;
                    }
                    if (resizeTop)
                    {
                        rect.Y = selY - diffY;
                        rect.Height = selH + diffY;
                    }
                    if (resizeRight)
                    {
                        rect.Width = selW - diffX;
                    }
                    if (resizeBottom)
                    {
                        rect.Height = selH - diffY;
                    }
                    if (move)
                    {
                        moving = true;
                        rect.Location = new Point(preX + x, preY + y);
                    }

                    if (rect.Width > minSize && rect.Height > minSize)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        private void ScrollablePictureBox_MouseEnter(object sender, EventArgs e)
        {
            if (!this.Focused && this.FindForm().ContainsFocus)
            {
                //currentScrollPos = ((Panel)this.Parent).AutoScrollPosition;
                this.Focus();
            }
        }

        private void ScrollablePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;

                if (rect == Rectangle.Empty)
                {
                    startPoint = e.Location;
                    pressOut = true;
                }
                else
                {
                    selX = rect.X;
                    selY = rect.Y;
                    selW = rect.Width;
                    selH = rect.Height;

                    Rectangle leftFrame = new Rectangle(selX, selY, frameWidth, selH);
                    Rectangle topFrame = new Rectangle(selX, selY, selW, frameWidth);
                    Rectangle rightFrame = new Rectangle(selX + selW - frameWidth, selY, frameWidth, selH);
                    Rectangle bottomFrame = new Rectangle(selX, selY + selH - frameWidth, selW, frameWidth);

                    Point p = e.Location;

                    bool isInside = rect.Contains(p);
                    bool isLeft = leftFrame.Contains(p);
                    bool isTop = topFrame.Contains(p);
                    bool isRight = rightFrame.Contains(p);
                    bool isBottom = bottomFrame.Contains(p);

                    if (isLeft && isTop)
                    {
                        resizeLeft = true;
                        resizeTop = true;
                        resizeRight = false;
                        resizeBottom = false;
                        move = false;
                    }
                    else if (isTop && isRight)
                    {
                        resizeLeft = false;
                        resizeTop = true;
                        resizeRight = true;
                        resizeBottom = false;
                        move = false;
                    }
                    else if (isRight && isBottom)
                    {
                        resizeLeft = false;
                        resizeTop = false;
                        resizeRight = true;
                        resizeBottom = true;
                        move = false;
                    }
                    else if (isBottom && isLeft)
                    {
                        resizeLeft = true;
                        resizeTop = false;
                        resizeRight = false;
                        resizeBottom = true;
                        move = false;
                    }
                    else if (isLeft)
                    {
                        resizeLeft = true;
                        resizeTop = false;
                        resizeRight = false;
                        resizeBottom = false;
                        move = false;
                    }
                    else if (isTop)
                    {
                        resizeLeft = false;
                        resizeTop = true;
                        resizeRight = false;
                        resizeBottom = false;
                        move = false;
                    }
                    else if (isRight)
                    {
                        resizeLeft = false;
                        resizeTop = false;
                        resizeRight = true;
                        resizeBottom = false;
                        move = false;
                    }
                    else if (isBottom)
                    {
                        resizeLeft = false;
                        resizeTop = false;
                        resizeRight = false;
                        resizeBottom = true;
                        move = false;
                    }
                    else if (isInside)
                    {
                        resizeLeft = false;
                        resizeTop = false;
                        resizeRight = false;
                        resizeBottom = false;
                        move = true;
                    }
                    else
                    {
                        resizeLeft = false;
                        resizeTop = false;
                        resizeRight = false;
                        resizeBottom = false;
                        move = false;
                    }

                    int x = e.X;
                    int y = e.Y;

                    startDragX = x;
                    startDragY = y;

                    preX = rect.X - startDragX;
                    preY = rect.Y - startDragY;

                    if (!rect.Contains(p))
                    {
                        startPoint = p;
                        pressOut = true;
                    }
                }
            }
        }

        private void ScrollablePictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (rect == null) return;
                if (rect != null)
                {
                    Rectangle testRect = new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Height + 2);
                    if (!testRect.Contains(e.Location))
                    {
                        Deselect();
                        this.Invalidate();
                    }
                }
            }
        }

        private void ScrollablePictureBox_GotFocus(object sender, EventArgs e)
        {
            if (this.Image == null) return;
            if (this.Parent is Panel panel)
            {
                //panel.AutoScrollPosition = new Point(Math.Abs(currentScrollPos.X), Math.Abs(currentScrollPos.Y));
                ((Panel)this.Parent).AutoScrollPosition = new Point(Math.Abs(currentScrollPos.X), Math.Abs(currentScrollPos.Y));

            }
            else
            {
                // Handle the case where Parent is not a Panel
                // For example, log an error or take alternative action
            }
        }

        private void ScrollablePictureBox_LostFocus(object sender, EventArgs e)
        {
            if (this.Image == null) return;
            if (this.Parent is Panel panel)
            {
                currentScrollPos = panel.AutoScrollPosition;
            }
            else
            {
                // Handle the case where Parent is not a Panel
                // For example, log an error or take alternative action
            }
        }
    }

}
