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
    public partial class Form1 : Form
    {
        public const int BOARD_WIDTH = 4;
        private const int TOP_MARGIN = 240; //multiple of 8 for consistency
        private const int BOTTOM_MARGIN = 128;
        private const int SIDE_MARGIN = 240;
        private const int TILE_WIDTH = 80;
        private const int TILE_MARGIN = 16;
        private readonly Button[,] buttons = new Button[BOARD_WIDTH, BOARD_WIDTH];
        private readonly int[,] values = new int[BOARD_WIDTH, BOARD_WIDTH];
        public Form1()
        {
            InitializeComponent();
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    buttons[x, y] = new Button();
                    buttons[x, y].SetBounds(SIDE_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * x, TOP_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * y, TILE_WIDTH, TILE_WIDTH);
                    buttons[x, y].Click += new EventHandler(this.ButtonEvent_Click);
                    Controls.Add(buttons[x, y]);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.mainForm.Size = new System.Drawing.Size(SIDE_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * BOARD_WIDTH + SIDE_MARGIN, TOP_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * BOARD_WIDTH + BOTTOM_MARGIN);
        }

        private void ButtonEvent_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // restart game grid
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Initialise variables of the MessageBox
            string message = "Combine two squares with the same value to make one square with a two times larger value.\n" +
                "Make a square with value 2048 to win the game.";
            string caption = "Rules";
            MessageBoxButtons button = MessageBoxButtons.OK;
            DialogResult result;
            // Display the MessageBox
            result = MessageBox.Show(message, caption, button);
        }
    }
}
