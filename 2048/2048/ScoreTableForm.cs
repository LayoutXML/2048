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
    public partial class ScoreTableForm : Form
    {
        private const string FONT = "Courier New";
        private const int FONT_SIZE_DEFAULT = 18;
        private const int FONT_SIZE_SMALL = 12;
        private const string BACKGROUND_COLOR = "#faf8ef";
        private const int SIDE_MARGIN = 40;
        private const int BOTTOM_MARGIN = 104; // multiple of 8 for consistency
        private const int TOP_MARGIN = 24; // multiple of 8 for consistency
        private const int LABEL_HEIGHT = 40;
        private const int TILE_WIDTH = 80;


        int[] curScores;
        public ScoreTableForm()
        {
            InitializeComponent();
        }
        public ScoreTableForm(int[] scores)
        {
            InitializeComponent();
            curScores = scores;
            this.BackColor = ColorTranslator.FromHtml(BACKGROUND_COLOR); // changes form background
            this.Size = new Size(SIDE_MARGIN*2+200, TOP_MARGIN+LABEL_HEIGHT*6+ BOTTOM_MARGIN); // sets the size
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            CreateLabel("Top Scores", (SIDE_MARGIN * 2) / 2 + FONT_SIZE_DEFAULT*2, TOP_MARGIN, new Font(FONT, FONT_SIZE_DEFAULT, FontStyle.Bold));

            int y = 0;
            while(y < curScores.Length && curScores[y] != 0)
            {
                CreateLabel((y + 1).ToString() + ".", SIDE_MARGIN, TOP_MARGIN + LABEL_HEIGHT + y * LABEL_HEIGHT, new Font(FONT, FONT_SIZE_DEFAULT, FontStyle.Bold));
                CreateLabel(curScores[y].ToString(), SIDE_MARGIN + 2 * FONT_SIZE_DEFAULT, TOP_MARGIN + LABEL_HEIGHT + y * LABEL_HEIGHT, new Font(FONT, FONT_SIZE_DEFAULT, FontStyle.Bold));
                y++;
            }
            CreateResetButton();

        }
        public void CreateLabel(String text, int xlocation, int ylocation, Font font)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Text = text;
            label.Location = new Point(xlocation, ylocation);
            label.Font = font;
            Controls.Add(label);
        }
       public void CreateResetButton()
        {
            
            Button resetButton = new Button();
            resetButton.SetBounds(SIDE_MARGIN, TOP_MARGIN + LABEL_HEIGHT * 6 + 10 ,200,LABEL_HEIGHT);

            // undoButton.Click += new EventHandler(this.UndoButton_Click);
            resetButton.FlatStyle = FlatStyle.Flat;
            resetButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#bbada0");
            resetButton.FlatAppearance.BorderSize = 4;
            resetButton.BackColor = ColorTranslator.FromHtml("#CDC1B5");
            resetButton.ForeColor = ColorTranslator.FromHtml("#3D3A31");
            resetButton.Font = new Font(FONT, FONT_SIZE_SMALL, FontStyle.Bold);
            resetButton.Text = "Reset Score";
            Controls.Add(resetButton);
            
        }
    }
}
