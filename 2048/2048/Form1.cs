using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace _2048
{
    public partial class Form1 : Form
    {
        public const int BOARD_WIDTH = 4;
        private const int TOP_MARGIN = 80; //multiple of 8 for consistency
        private const int BOTTOM_MARGIN = 80;
        private const int SIDE_MARGIN = 80;
        private const int TILE_WIDTH = 80;
        private const int TILE_MARGIN = 0;
        private readonly Button[,] buttons = new Button[BOARD_WIDTH, BOARD_WIDTH];
        private readonly int[,] values = new int[BOARD_WIDTH, BOARD_WIDTH];
        private int score = 0;
        private Label scoreLabel;
        public Form1()
        {
            

            InitializeComponent();
           this.BackColor = System.Drawing.ColorTranslator.FromHtml("#faf8ef"); //changes form background
            menuStrip2.BackColor = System.Drawing.ColorTranslator.FromHtml("#faf8ef");
            menuStrip1.BackColor = System.Drawing.ColorTranslator.FromHtml("#faf8ef");
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    buttons[x, y] = new Button();
                    buttons[x, y].SetBounds(SIDE_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * x, TOP_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * y, TILE_WIDTH, TILE_WIDTH);
                    buttons[x, y].Click += new EventHandler(this.ButtonEvent_Click);
                    buttons[x, y].Name = x.ToString() + " " + y.ToString();
                    buttons[x, y].Font = new Font("Courier New", 24, FontStyle.Bold); //font
                    buttons[x, y].FlatStyle = FlatStyle.Flat;
                    buttons[x, y].FlatAppearance.BorderColor = System.Drawing.ColorTranslator.FromHtml("#bbada0");
                    buttons[x, y].FlatAppearance.BorderSize = 4;
                    Controls.Add(buttons[x, y]);
                }
            }
            addScoreLabel();
            GenerateNumber(2);
            Redraw();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.mainForm.Size = new Size(SIDE_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * BOARD_WIDTH + SIDE_MARGIN, TOP_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * BOARD_WIDTH + BOTTOM_MARGIN);
        }

        private void ButtonEvent_Click(object sender, EventArgs e)
        {
            int clickedX = 0;
            int clickedY = 0;
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    if (((Button)sender).Name == x.ToString() + " " + y.ToString())
                    {
                        clickedX = x;
                        clickedY = y;
                    }

                }
            }

            bool moved = false;
            if (clickedY == 0 && (clickedX != 0 && clickedX != BOARD_WIDTH - 1))
            {
                moved = MoveUp();
            }
            else if (clickedX == BOARD_WIDTH - 1 && (clickedY != 0 && clickedY != BOARD_WIDTH - 1))
            {
                moved = MoveRight();
            }
            else if (clickedY == BOARD_WIDTH - 1 && (clickedX != 0 && clickedX != BOARD_WIDTH - 1))
            {
                moved = MoveDown();
            }
            else if (clickedX == 0 && (clickedY != 0 && clickedY != BOARD_WIDTH - 1))
            {
                moved = MoveLeft();
            }
            Redraw();
            if (moved)
            {
                GenerateNumber(1);
            }
            Redraw();
        }

        private void GenerateNumber(int amount)
        {
            Random random = new Random(); // Generates new number
            Coordinates coordinates = CheckOpenSpace(); // gets the coordinates of a space
            if (amount == 1) // If we need to generate only 1 new number
            {
                if(coordinates == null) // if coordinates are null that means there are no more open spaces left
                {
                    //GAME OVER (NO MORE SPACE LEFT)
                    Console.WriteLine("ENDDD");
                    return;
                }
                int number = random.Next(0, 1) == 0 ? 2 : 4; // generate a random number and decide if it's a 2 or a 4

                buttons[coordinates.x, coordinates.y].Text = number.ToString(); // write the new number
                values[coordinates.x, coordinates.y] = number;
            }
            else // if we need 2 numbers
            {
                buttons[coordinates.x, coordinates.y].Text = "4"; // write the new number
                values[coordinates.x, coordinates.y] = 4;
                coordinates = CheckOpenSpace();
                buttons[coordinates.x, coordinates.y].Text = "2"; // write the new number
                values[coordinates.x, coordinates.y] = 2;

            }
        }
        
        private Coordinates CheckOpenSpace()
        {
            Coordinates OpenSpace = null;
            int count = 0; // the count of free spaces
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                   if (values[x, y] == 0) // if it's a free space
                    {
                        if(count == 0) // if it's the 1st free space then get it's coordinates
                        {
                            OpenSpace = new Coordinates(x, y);
                        }
                        count++;
                    } 
                }
            }
            if (count > 1) // if there are more then 1 open space available
            {
                Random random = new Random();
                int x, y;
                do 
                {
                    x = random.Next(0, BOARD_WIDTH);
                    y = random.Next(0, BOARD_WIDTH);
                } while (values[x, y] != 0); //repeat the loop until an open space is found
                OpenSpace = new Coordinates(x, y);
                               
            }
           
            return OpenSpace;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RestartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // restart game grid
            score = 0;
            RestartGame();
        }

        private void RulesToolStripMenuItem_Click(object sender, EventArgs e)
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
        
        private bool MoveUp()
        {
            bool moved = false;
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                List<int> newColumn = new List<int>();
                int value = 0;
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    if (value != 0 && values[x, y] == value)
                    {
                        newColumn.Add(value * 2);
                        score += value * 2;
                        value = 0;
                    }
                    else
                    {
                        if (values[x, y] != 0 && value != 0)
                        {
                            newColumn.Add(value);
                            value = values[x, y];
                        }
                        if (value == 0)
                        {
                            value = values[x, y];
                        }
                    }
                }
                if (value != 0)
                {
                    newColumn.Add(value);
                }

                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    int oldValue = values[x, y];
                    values[x, y] = newColumn.Count > y ? newColumn[y] : 0;
                    if (oldValue != values[x, y])
                    {
                        moved = true;
                    }
                }
            }
            return moved;
        }

        private bool MoveRight()
        {
            bool moved = false;
            for (int y = 0; y < BOARD_WIDTH; y++)
            {
                List<int> newColumn = new List<int>();
                int value = 0;
                for (int x = BOARD_WIDTH - 1; x >= 0; x--)
                {
                    if (value != 0 && values[x, y] == value)
                    {
                        newColumn.Add(value * 2);
                        score += value * 2;
                        value = 0;
                    }
                    else
                    {
                        if (values[x, y] != 0 && value != 0)
                        {
                            newColumn.Add(value);
                            value = values[x, y];
                        }
                        if (value == 0)
                        {
                            value = values[x, y];
                        }
                    }
                }
                if (value != 0)
                {
                    newColumn.Add(value);
                }

                for (int x = BOARD_WIDTH - 1; x >= 0; x--)
                {
                    int oldValue = values[x, y];
                    values[x, y] = newColumn.Count > BOARD_WIDTH - 1 - x ? newColumn[BOARD_WIDTH - 1 - x] : 0;
                    if (oldValue != values[x, y])
                    {
                        moved = true;
                    }
                }
            }
            return moved;
        }

        private bool MoveDown()
        {

            bool moved = false;
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                List<int> newColumn = new List<int>();
                int value = 0;
                for (int y = BOARD_WIDTH - 1; y >= 0; y--)
                {
                    if (value != 0 && values[x, y] == value)
                    {
                        newColumn.Add(value * 2);
                        score += value * 2;
                        value = 0;
                    }
                    else
                    {
                        if (values[x, y] != 0 && value != 0)
                        {
                            newColumn.Add(value);
                            value = values[x, y];
                        }
                        if (value == 0)
                        {
                            value = values[x, y];
                        }
                    }
                }
                if (value != 0)
                {
                    newColumn.Add(value);
                }

                for (int y = BOARD_WIDTH - 1; y >= 0; y--)
                {
                    int oldValue = values[x, y];
                    values[x, y] = newColumn.Count > BOARD_WIDTH - 1 - y ? newColumn[BOARD_WIDTH - 1 - y] : 0;
                    if (oldValue != values[x, y])
                    {
                        moved = true;
                    }
                }
            }
            return moved;
        }

        private bool MoveLeft()
        {
            bool moved = false;
            for (int y = 0; y < BOARD_WIDTH; y++)
            {
                List<int> newColumn = new List<int>();
                int value = 0;
                for (int x = 0; x < BOARD_WIDTH; x++)
                {
                    if (value != 0 && values[x, y] == value)
                    {
                        newColumn.Add(value * 2);
                        score += value * 2;
                        value = 0;
                    }
                    else
                    {
                        if (values[x, y] != 0 && value != 0)
                        {
                            newColumn.Add(value);
                            value = values[x, y];
                        }
                        if (value == 0)
                        {
                            value = values[x, y];
                        }
                    }
                }
                if (value != 0)
                {
                    newColumn.Add(value);
                }

                for (int x = 0; x < BOARD_WIDTH; x++)
                {
                    int oldValue = values[x, y];
                    values[x, y] = newColumn.Count > x ? newColumn[x] : 0;
                    if (oldValue != values[x, y])
                    {
                        moved = true;
                    }
                }
            }
            return moved;
        }
        
        public void Redraw()
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
                    ChangeColor(x, y); //changes the button text and background colors
                }
            }
            scoreLabel.Text = "Score: " + Convert.ToString(score); //updates scoreLabel text

        }
        
        public void ChangeColor(int x, int y)
        {
            int number = values[x, y];
            switch (number)
            {
                case 0: //square is empty
                    buttons[x, y].BackColor = System.Drawing.ColorTranslator.FromHtml("#e6e2df");
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

        private void addScoreLabel()
        {
            scoreLabel = new Label();
            scoreLabel.AutoSize = true;
            int tile_margin = 16;
            int xScoreLabel = SIDE_MARGIN - tile_margin + (TILE_WIDTH - tile_margin) * (BOARD_WIDTH - 1);
            int yScoreLabel = TOP_MARGIN - tile_margin * 2;
            scoreLabel.Location = new Point(xScoreLabel, yScoreLabel);
            scoreLabel.Font = new Font("Courier New", 16, FontStyle.Bold); //font
            Controls.Add(scoreLabel);
        }
        
        public void RestartGame()
        {
            // restart game grid
            Array.Clear(values, 0, BOARD_WIDTH * BOARD_WIDTH);
            GenerateNumber(2);
            Redraw();
        }
    }
    
}
