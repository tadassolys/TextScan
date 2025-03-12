using System;
using System.Drawing;
using System.Windows.Forms;

namespace TextScan
{
    public class ScreenshotForm : Form
    {
        private Rectangle selection;
        private bool dragging;
        private Point startPoint;
        private Bitmap capturedImage;

        public Bitmap CapturedImage => capturedImage;

        public ScreenshotForm()
        {
            // Form initialization
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.ShowInTaskbar = false;  // Hide from taskbar
            this.BackColor = Color.Black;
            this.Opacity = 0.3;
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Cross;  // Use crosshair cursor for better precision

            // Event handlers
            this.KeyDown += ScreenshotForm_KeyDown;  // Add keyboard support
            this.MouseDown += ScreenshotForm_MouseDown;
            this.MouseMove += ScreenshotForm_MouseMove;
            this.MouseUp += ScreenshotForm_MouseUp;
            this.Paint += ScreenshotForm_Paint;

            // Capture all screens, not just primary
            this.Bounds = GetAllScreensBounds();
        }

        // Get bounds encompassing all screens
        private Rectangle GetAllScreensBounds()
        {
            Rectangle allBounds = Rectangle.Empty;
            foreach (Screen screen in Screen.AllScreens)
            {
                allBounds = Rectangle.Union(allBounds, screen.Bounds);
            }
            return allBounds;
        }

        private void ScreenshotForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Allow canceling with Escape key
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void ScreenshotForm_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            startPoint = e.Location;
            selection = new Rectangle(startPoint.X, startPoint.Y, 0, 0);
            this.Invalidate();
        }

        private void ScreenshotForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                // Update selection rectangle
                selection = new Rectangle(
                    Math.Min(startPoint.X, e.X),
                    Math.Min(startPoint.Y, e.Y),
                    Math.Abs(e.X - startPoint.X),
                    Math.Abs(e.Y - startPoint.Y)
                );

                this.Invalidate();
            }
        }

        private void ScreenshotForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (!dragging) return;

            dragging = false;

            // Only capture if we have a reasonable selection
            if (selection.Width > 5 && selection.Height > 5)
            {
                // Hide the form before capturing to avoid capturing the selection overlay
                this.Visible = false;
                Application.DoEvents(); // Process all pending UI events

                CaptureSelectedArea();
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }

            this.Close();
        }

        private void ScreenshotForm_Paint(object sender, PaintEventArgs e)
        {
            if (selection.Width <= 0 || selection.Height <= 0) return;

            // Dim the entire screen
            using (SolidBrush dimBrush = new SolidBrush(Color.FromArgb(120, 0, 0, 0)))
            {
                e.Graphics.FillRectangle(dimBrush, this.ClientRectangle);
            }

            // Clear the selected area to show it without dimming
            e.Graphics.SetClip(selection);
            e.Graphics.Clear(Color.Transparent);
            e.Graphics.ResetClip();

            // Draw border around selection with enhanced visibility
            using (Pen selectionPen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(selectionPen, selection);
            }

            // Show selection dimensions
            string dimensions = $"{selection.Width} x {selection.Height}";
            using (Font font = new Font("Arial", 10))
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
            {
                SizeF textSize = e.Graphics.MeasureString(dimensions, font);
                PointF textLocation = new PointF(
                    selection.X + (selection.Width - textSize.Width) / 2,
                    selection.Y + selection.Height + 5
                );

                // Draw background for text
                e.Graphics.FillRectangle(bgBrush,
                    textLocation.X - 2, textLocation.Y - 2,
                    textSize.Width + 4, textSize.Height + 4);

                e.Graphics.DrawString(dimensions, font, textBrush, textLocation);
            }
        }

        private void CaptureSelectedArea()
        {
            try
            {
                // Determine which screen(s) the selection intersects with
                capturedImage = new Bitmap(selection.Width, selection.Height);

                using (Graphics g = Graphics.FromImage(capturedImage))
                {
                    g.CopyFromScreen(selection.Location, Point.Empty, selection.Size);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing screenshot: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                capturedImage = null;
            }
        }
    }
}