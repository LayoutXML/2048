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
         
            redraw(); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.mainForm.Size = new System.Drawing.Size(SIDE_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * BOARD_WIDTH + SIDE_MARGIN, TOP_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * BOARD_WIDTH + BOTTOM_MARGIN);
        }

        private void ButtonEvent_Click(object sender, EventArgs e)
        {

        }

        public void redraw()
        {
            for (int x = 0; x < BOARD_WIDTH; x++)   //goes through the board and redraws the text
            {
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    if(values[x,y] == 0)
                    {
                        buttons[x, y].Text = ""; 
                    }
                    else
                    {
                        buttons[x, y].Text = values[x, y].ToString(); //sets the button text to the correct value
                    }
                    changeColor(x, y); //changes the button text and background colors
                }
            }


        }
        public void changeColor(int x, int y)
        {
            int number = values[x, y];

            buttons[x, y].ForeColor = System.Drawing.ColorTranslator.FromHtml("#F9F6F2");
            switch (number)
            {
                case 0: //square is empty
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#EEEBE8");
                    break;
                case 2:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#EEE4DA");
 
                    break;
                case 4:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#EDE0C8");
   
                    break;
                case 8:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#F2B179");
 
                    break;
                case 16:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#F59563");
   
                    break;
                case 32:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#F67C5F");
                  
                    break;
                case 64:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#F65E3B");
                 
                    break;
                case 128:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#EDCF72");
      
                    break;
                case 256:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#EDCC61");

                    break;
                case 512:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#EDC850");
                    
                    break;
                case 1024:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#EDC53F");
                    break;
                case 2048:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#EDC22E");
                    break;
                default:
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#A78510");
                    break;
            }

        }
    }
}
