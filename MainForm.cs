using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace VexCardDebugger
{
    public class MainForm : Form
    {
        // ====== \\ Fields ====== \\
        private TokenDecoder decoder = new TokenDecoder();
        private readonly string sdDrive;                 // SD card drive
        private readonly string exeFolder;               // folder where .exe is
        private readonly string settingsFile;           // path to settings.json

        private ListView fileList = new ListView();
        private RichTextBox logBox = new RichTextBox();
        private float logFontSize = 10f;

        // Colors
        private readonly Color colorToolbar = ColorTranslator.FromHtml("#2c2c2c");
        private readonly Color colorTxtArea = ColorTranslator.FromHtml("#171717");
        private readonly Color colorToken = ColorTranslator.FromHtml("#212121");
        private readonly Color colorText = ColorTranslator.FromHtml("#f5f5f5");
        private readonly Color colorSelected = ColorTranslator.FromHtml("#2a2a2a");

        private AppSettings settings;

        // ====== \\ AppSettings Class ====== \\
        private class AppSettings
        {
            public float LogFontSize { get; set; } = 10f;
        }

        // ====== \\ Constructor ====== \\
        public MainForm()
        {
            exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            settingsFile = Path.Combine(exeFolder, "settings.json");
            sdDrive = @"V:\";

            Text = "VEX SD Card Debugger";
            Size = new Size(900, 520);
            StartPosition = FormStartPosition.CenterScreen;

            LoadSettings();
            logFontSize = settings.LogFontSize;

            InitializeLayout();
            LoadFiles();
            ApplyTheme();
        }

        // ====== \\ Load Settings ====== \\
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(settingsFile))
                {
                    string json = File.ReadAllText(settingsFile);
                    settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                else
                {
                    settings = new AppSettings();
                }
            }
            catch
            {
                settings = new AppSettings();
            }
        }

        // ====== \\ Save Settings ====== \\
        private void SaveSettings()
        {
            try
            {
                settings.LogFontSize = logFontSize;
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsFile, json);
            }
            catch
            {
                // silently fail
            }
        }

        // ====== \\ Toolbar ====== \\
        private void InitializeToolbar(TableLayoutPanel mainLayout)
        {
            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(6),
                Height = 40,
                BackColor = colorToolbar,
                WrapContents = false
            };
            mainLayout.Controls.Add(toolbar, 0, 0);

            var settingsLabel = new Label
            {
                Text = "Settings",
                AutoSize = true,
                ForeColor = colorText,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            };
            settingsLabel.Location = new Point(settingsLabel.Location.X, (toolbar.Height - settingsLabel.Height) / 2);
            settingsLabel.Click += (s, e) => OpenSettings();
            toolbar.Controls.Add(settingsLabel);

            var filler = new Panel { Dock = DockStyle.Fill };
            toolbar.Controls.Add(filler);
        }

        // ====== \\ Content ====== \\
        private void InitializeContent(TableLayoutPanel mainLayout)
        {
            var contentTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            contentTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            contentTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainLayout.Controls.Add(contentTable, 0, 1);

            // File list (sidebar)
            fileList = new ListView
            {
                View = View.List,
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                HideSelection = false,
                OwnerDraw = true,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.None
            };
            fileList.DrawItem += FileList_DrawItem;
            fileList.SelectedIndexChanged += FileList_SelectedIndexChanged;
            fileList.MouseUp += FileList_MouseUp; // <-- Add right-click event
            contentTable.Controls.Add(fileList, 0, 0);

            // Log box (token area)
            logBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", logFontSize),
                BorderStyle = BorderStyle.None,
                BackColor = colorToken,
                ForeColor = colorText
            };
            contentTable.Controls.Add(logBox, 1, 0);
        }

        // Right-click event handler
        private void FileList_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var item = fileList.GetItemAt(e.X, e.Y);
            if (item == null) return;

            var contextMenu = new ContextMenuStrip();
            var deleteItem = new ToolStripMenuItem("Delete File");
            deleteItem.Click += (s, ev) =>
            {
                string filePath = Path.Combine(sdDrive, item.Text);
                try
                {
                    File.Delete(filePath);
                    fileList.Items.Remove(item);
                    logBox.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            contextMenu.Items.Add(deleteItem);
            contextMenu.Show(fileList, e.Location);
        }

        // ====== \\ Layout ====== \\
        private void InitializeLayout()
        {
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(mainLayout);

            InitializeToolbar(mainLayout);
            InitializeContent(mainLayout);
        }

        // ====== \\ Load Files ====== \\
        private void LoadFiles()
        {
            try
            {
                if (!Directory.Exists(sdDrive)) return;
                string[] files = Directory.GetFiles(sdDrive, "*.txt");
                fileList.BeginUpdate();
                fileList.Items.Clear();
                foreach (var f in files)
                    fileList.Items.Add(Path.GetFileName(f));
                fileList.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====== \\ Draw ListView Items ====== \\
        private void FileList_DrawItem(object? sender, DrawListViewItemEventArgs e)
        {
            Color backColor = e.Item.Selected ? colorSelected : colorTxtArea;
            Color foreColor = colorText;

            using var bg = new SolidBrush(backColor);
            using var fg = new SolidBrush(foreColor);

            e.Graphics.FillRectangle(bg, e.Bounds);
            e.Graphics.DrawString(e.Item.Text, fileList.Font, fg, e.Bounds.Location);
        }

        // ====== \\ Handle file selection and populate log box ====== \\
        private void FileList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (fileList.SelectedItems.Count == 0) return;

            var item = fileList.SelectedItems[0];
            if (item == null) return;

            string filePath = Path.Combine(sdDrive, item.Text);
            if (!File.Exists(filePath)) return;

            try
            {
                logBox.Clear();
                var decodedLines = decoder.DecodeFile(filePath);
                foreach (var decoded in decodedLines)
                {
                    logBox.AppendText(decoded + Environment.NewLine);
                }

                logBox.SelectionStart = logBox.Text.Length;
                logBox.ScrollToCaret();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====== \\ Decode ====== \\
        private string DecodeLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return string.Empty;
            if (line.StartsWith("P12PO:", StringComparison.Ordinal))
            {
                string time = line.Substring("P12PO:".Length);
                return $"(P12PO) {time} Pnuematic 12 Pump On";
            }
            return $"(UNKNOWN) {line}";
        }

        // ====== \\ Open Settings ====== \\
        private void OpenSettings()
        {
            using var settingsForm = new Form
            {
                Text = "Settings",
                Size = new Size(360, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            settingsForm.Controls.Add(mainPanel);

            var layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false
            };
            mainPanel.Controls.Add(layout);

            var fontPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            var fontLabel = new Label { Text = "Log Font Size:", AutoSize = true };
            var fontSizeUpDown = new NumericUpDown
            {
                Minimum = 6,
                Maximum = 28,
                Value = (decimal)logFontSize,
                Width = 70,
                Margin = new Padding(8, 0, 0, 0)
            };
            fontPanel.Controls.Add(fontLabel);
            fontPanel.Controls.Add(fontSizeUpDown);
            layout.Controls.Add(fontPanel);

            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 44 };
            settingsForm.Controls.Add(bottom);

            var applyBtn = new Button { Text = "Apply", Width = 90, Height = 28, Anchor = AnchorStyles.Right | AnchorStyles.Bottom };
            applyBtn.Location = new Point(settingsForm.ClientSize.Width - applyBtn.Width - 20, 8);
            applyBtn.Click += (s, e) =>
            {
                logFontSize = (float)fontSizeUpDown.Value;
                logBox.Font = new Font("Consolas", logFontSize);
                SaveSettings(); // <-- save immediately
                settingsForm.Close();
            };
            bottom.Controls.Add(applyBtn);

            var cancelBtn = new Button { Text = "Cancel", Width = 90, Height = 28, Anchor = AnchorStyles.Right | AnchorStyles.Bottom };
            cancelBtn.Location = new Point(settingsForm.ClientSize.Width - applyBtn.Width - cancelBtn.Width - 32, 8);
            cancelBtn.Click += (s, e) => settingsForm.Close();
            bottom.Controls.Add(cancelBtn);

            settingsForm.ShowDialog(this);
        }

        // ====== \\ Apply Theme ====== \\
        private void ApplyColorsRecursive(Control c, Color back, Color fore)
        {
            switch (c)
            {
                case Button btn:
                    btn.BackColor = colorToolbar;
                    btn.ForeColor = fore;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    break;
                default:
                    c.BackColor = back;
                    c.ForeColor = fore;
                    break;
            }
        }

        private void ApplyTheme()
        {
            if (fileList != null)
            {
                fileList.BackColor = colorTxtArea;
                fileList.ForeColor = colorText;
                if (fileList.Parent != null) fileList.Parent.BackColor = colorTxtArea;
                fileList.Invalidate();
            }

            if (logBox != null)
            {
                logBox.BackColor = colorToken;
                logBox.ForeColor = colorText;
                logBox.Invalidate();
            }

            foreach (Control c in Controls)
            {
                if (c is TableLayoutPanel tlp && tlp.RowCount > 0 && tlp.Controls[0] is FlowLayoutPanel toolbar)
                    toolbar.BackColor = colorToolbar;
            }

            foreach (Control c in Controls)
                ApplyColorsRecursive(c, colorTxtArea, colorText);
        }
    }
}
