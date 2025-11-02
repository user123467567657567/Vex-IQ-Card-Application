using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace VexCardDebugger
{
    public class SplashForm : Form
    {
        private ListBox logBox;

        public string? SelectedDrive { get; private set; } = null;

        public SplashForm()
        {
            // Simple window with close/minimize
            this.Text = "VEX SD Debugger";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Size = new Size(500, 300);
            this.MaximizeBox = false;
            this.MinimizeBox = true;

            // Only ListBox for logs
            logBox = new ListBox()
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                HorizontalScrollbar = true
            };
            this.Controls.Add(logBox);

            // Run splash sequence when shown
            this.Shown += async (s, e) =>
            {
                await RunSplashSequenceAsync();
                this.Close(); // auto-close when done
            };
        }

        public async Task RunSplashSequenceAsync()
        {
            var sw = Stopwatch.StartNew();

            await AddLogAsync("Run Debugger");
            await Task.Delay(500);

            await AddLogAsync("Finding Drive...");
            var drive = DetectDrive();
            if (drive == null)
            {
                await AddLogAsync("No SD card detected. Exiting.");
                await Task.Delay(1500);
                return;
            }
            SelectedDrive = drive;
            await AddLogAsync($"Found Drive: {drive}");

            await AddLogAsync("Loading files...");
            string[] files = Directory.GetFiles(drive, "*.txt");
            await Task.Delay(500);

            await AddLogAsync("Decoding files...");
            int count = 1;
            foreach (var file in files)
            {
                await AddLogAsync($"Decoded file {Path.GetFileName(file)} ({count}/{files.Length})");
                count++;
                await Task.Delay(200); // simulate decoding
            }

            sw.Stop();
            await AddLogAsync($"Loaded {files.Length} files in {sw.Elapsed.TotalSeconds:F2} seconds");
            await Task.Delay(1000);
        }

        private async Task AddLogAsync(string message)
        {
            if (logBox.InvokeRequired)
            {
                logBox.Invoke(new Action(() => logBox.Items.Add(message)));
            }
            else
            {
                logBox.Items.Add(message);
            }
            // Auto-scroll to bottom
            logBox.TopIndex = logBox.Items.Count - 1;
            await Task.Yield();
        }

        private string? DetectDrive()
        {
            foreach (var d in DriveInfo.GetDrives())
            {
                if (d.DriveType == DriveType.Removable && d.IsReady)
                    return d.Name;
            }
            return null;
        }
    }
}
