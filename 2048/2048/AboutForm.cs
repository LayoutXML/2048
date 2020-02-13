using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2048
{
    public partial class AboutForm : Form
    {
        private const int HEIGHT = 160;
        private const int WIDTH = 400;
        private const int TOP_MARGIN = 24;
        private const int SIDE_MARGIN = 24;
        private const string BACKGROUND_COLOR = "#faf8ef";
        private const string FONT = "Courier New";
        private const int FONT_SIZE_EXTRA_SMALL = 12;

        private Label aboutLabel;
        
        public AboutForm()
        {
            InitializeComponent();

            BackColor = ColorTranslator.FromHtml(BACKGROUND_COLOR);
            Size = new Size(WIDTH, HEIGHT);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            AddAboutLabel();

            StartPosition = FormStartPosition.CenterScreen;
        }

        private void AddAboutLabel()
        {
            aboutLabel = new Label();
            aboutLabel.Location = new Point(SIDE_MARGIN, TOP_MARGIN);
            aboutLabel.Size = new Size(WIDTH - SIDE_MARGIN * 2, HEIGHT - TOP_MARGIN * 2);
            aboutLabel.Text = "\"2048\" game in C# for AC22005 Coursework #1 by Jokubas Butkus, Rokas Jankunas, Justas Labeikis.";
            aboutLabel.TextAlign = ContentAlignment.TopCenter;
            aboutLabel.Font = new Font(FONT, FONT_SIZE_EXTRA_SMALL);
            Controls.Add(aboutLabel);
        }
    }
}
