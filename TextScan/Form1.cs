using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Tesseract;

namespace TextScan
{
    public partial class Form1 : Form
    {
        private Button captureButton;
        private TextBox resultTextBox;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        public Form1()
        {
            InitializeComponent();
            SetupUI();
            SetupTrayIcon();
        }

        private void SetupUI()
        {
            this.Text = "TextScan - OCR";
            this.Size = new Size(400, 300);
            captureButton = new Button
            {
                Text = "Capture Screenshot",
                Size = new Size(150, 40),
                Location = new Point(120, 20)
            };
            captureButton.Click += CaptureScreenshot;
            resultTextBox = new TextBox
            {
                Multiline = true,
                Size = new Size(350, 150),
                Location = new Point(20, 80),
                ScrollBars = ScrollBars.Vertical // Add scrollbar for long text
            };
            Controls.Add(captureButton);
            Controls.Add(resultTextBox);

            // Handle form closing to minimize to tray instead
            this.FormClosing += Form1_FormClosing;
        }

        private void SetupTrayIcon()
        {
            // Create tray menu
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Open", null, OnTrayOpenClick);
            trayMenu.Items.Add("Capture", null, CaptureScreenshot);
            trayMenu.Items.Add("-"); // Separator
            trayMenu.Items.Add("Exit", null, OnTrayExitClick);

            // Create tray icon
            trayIcon = new NotifyIcon
            {
                Icon = new Icon("icon.ico"), // You can replace with your custom icon
                Text = "TextScan OCR",
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            // Add double-click handler to open application
            trayIcon.DoubleClick += OnTrayOpenClick;
        }

        private void OnTrayOpenClick(object sender, EventArgs e)
        {
            // Show and bring to front
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void OnTrayExitClick(object sender, EventArgs e)
        {
            // Clean up tray icon before exiting
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the user clicked the X button, minimize to tray instead of closing
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; // Cancel the close
                this.Hide();      // Hide the form
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Start minimized to tray if needed
            if (Environment.GetCommandLineArgs().Length > 1 &&
                Environment.GetCommandLineArgs()[1].ToLower() == "/minimized")
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
        }

        private void CaptureScreenshot(object sender, EventArgs e)
        {
            this.Hide(); // Hide the main window
            using (var screenshotForm = new ScreenshotForm())
            {
                if (screenshotForm.ShowDialog() == DialogResult.OK)
                {
                    string extractedText = ExtractTextFromImage(screenshotForm.CapturedImage);
                    resultTextBox.Text = extractedText;
                }
            }
            this.Show(); // Show the main window again
        }

        private string ExtractTextFromImage(Bitmap image)
        {
            string result = "";
            try
            {
                string tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
                // Check if tessdata folder exists
                if (!Directory.Exists(tessDataPath))
                {
                    MessageBox.Show("Error: tessdata folder not found!\nExpected Path: " + tessDataPath);
                    return "";
                }
                using (var engine = new TesseractEngine(tessDataPath, "lit", EngineMode.Default))
                {
                    using (var img = PixConverter.ToPix(image))
                    {
                        using (var page = engine.Process(img))
                        {
                            result = page.GetText();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return result;
        }
    }
}