using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyTestApp
{
    public static class ThemeManager
    {
        public static bool IsDarkMode { get; private set; } = false;

        public static void ApplyTheme(Form form, bool darkMode)
        {
            IsDarkMode = darkMode;

            Color backColor = darkMode ? Color.FromArgb(30, 30, 30) : SystemColors.Control;
            Color foreColor = darkMode ? Color.White : SystemColors.ControlText;

            ApplyToControl(form, backColor, foreColor);
        }

        private static void ApplyToControl(Control control, Color backColor, Color foreColor)
        {
            control.BackColor = backColor;
            control.ForeColor = foreColor;

            foreach (Control child in control.Controls)
            {
                ApplyToControl(child, backColor, foreColor);
            }
        }
    }
}
