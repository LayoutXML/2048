﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace _2048
{
    public partial class Form1 : Form
    {
        // constants
        public const int BOARD_WIDTH = 4;
        private const int TOP_MARGIN = 80; //multiple of 8 for consistency
        private const int BOTTOM_MARGIN = 80;
        private const int SIDE_MARGIN = 80;
        private const int TILE_WIDTH = 80;
        private const int TILE_MARGIN = 0;
        private readonly string[] COLORS = new[] { "#CDC1B5", "#EEE4DB", "#F1E2CE", "#F0B57D", "#F19F64", "#F77E73", "#FD5644", "#F1D275", "#F1D275", "#E6C847", "#EDBE4A", "#EFBE45" };
        private const string DEFAULT_COLOR = "#3D3A31";
        private const string DEFAULT_TEXT_COLOR = "#000000";
        private const string INVERSE_TEXT_COLOR = "#ffffff";
        private const string BACKGROUND_COLOR = "#faf8ef";
        private const string FONT = "Courier New";
        private const int FONT_SIZE_DEFAULT = 24;
        private const int FONT_SIZE_SMALL = 18;
        private const int FONT_SIZE_EXTRA_SMALL = 12;


        private readonly Button[,] buttons = new Button[BOARD_WIDTH, BOARD_WIDTH];
        private readonly int[,] values = new int[BOARD_WIDTH, BOARD_WIDTH];
        private int score = 0;
        private Label scoreLabel;
        public Form1()
        {
            InitializeComponent();
            this.BackColor = ColorTranslator.FromHtml(BACKGROUND_COLOR); //changes form background
            menuStrip2.BackColor = ColorTranslator.FromHtml(BACKGROUND_COLOR);
            menuStrip1.BackColor = ColorTranslator.FromHtml(BACKGROUND_COLOR);
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    buttons[x, y] = new Button();
                    buttons[x, y].SetBounds(SIDE_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * x, TOP_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * y, TILE_WIDTH, TILE_WIDTH);
                    buttons[x, y].Click += new EventHandler(this.ButtonEvent_Click);
                    buttons[x, y].Name = x.ToString() + " " + y.ToString();
                    buttons[x, y].FlatStyle = FlatStyle.Flat;
                    buttons[x, y].FlatAppearance.BorderColor = ColorTranslator.FromHtml("#bbada0");
                    buttons[x, y].FlatAppearance.BorderSize = 4;
                    Controls.Add(buttons[x, y]);
                }
            }
            AddScoreLabel();
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
                values[coordinates.x, coordinates.y] = number;
            }
            else // if we need 2 numbers
            {
                values[coordinates.x, coordinates.y] = 4;
                coordinates = CheckOpenSpace();
                values[coordinates.x, coordinates.y] = 2;
                coordinates = CheckOpenSpace();
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
                "Make a square with value 2048 to win the game.\n\n" +
                "Press top row's middle buttons to move up.\n" +
                "Press bottom row's middle buttons to move down.\n" +
                "Press left collumn's middle buttons to move left.\n" +
                "Press right collumn's middle buttons to move right.\n";
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
            int index = values[x, y] == 0 ? 0 : (int)Math.Log(values[x, y], 2);
            string backColor = index >= COLORS.Length ? DEFAULT_COLOR : COLORS[index];
            string textColor = values[x, y] > 4 ? INVERSE_TEXT_COLOR : DEFAULT_TEXT_COLOR;
            buttons[x, y].BackColor = ColorTranslator.FromHtml(backColor);
            buttons[x, y].ForeColor = ColorTranslator.FromHtml(textColor);
            int fontSize;
            if (values[x, y] > 999)
            {
                fontSize = FONT_SIZE_EXTRA_SMALL;
            }
            else if (values[x, y] > 99)
            {
                fontSize = FONT_SIZE_SMALL;
            }
            else
            {
                fontSize = FONT_SIZE_DEFAULT;
            }
            buttons[x, y].Font = new Font(FONT, fontSize, FontStyle.Bold); //font
        }

        private void AddScoreLabel()
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

        protected override bool ProcessCmdKey(ref Message message, Keys key)
        {
            bool moved = false;
            bool processed = false;
            switch (key)
            {
                case Keys.Up:
                    moved = MoveUp();
                    processed = true;
                    break;
                case Keys.Right:
                    moved = MoveRight();
                    processed = true;
                    break;
                case Keys.Down:
                    moved = MoveDown();
                    processed = true;
                    break;
                case Keys.Left:
                    moved = MoveLeft();
                    processed = true;
                    break;
                default:
                    processed = false;
                    break;
            }
            Redraw();
            if (moved)
            {
                GenerateNumber(1);
            }
            Redraw();

            if (processed)
            {
                return true;
            }
            return base.ProcessCmdKey(ref message, key);
        }
    }
    
}
