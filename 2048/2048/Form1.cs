using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace _2048
{
    public partial class Form1 : Form
    {
        // constants
        public const int BOARD_WIDTH = 4;
        private const int TOP_MARGIN = 136; //multiple of 8 for consistency
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
        private const bool IS_HARD_MODE = false;
        private const string SAVE_FILE = "scores.txt";


        private readonly Button[,] buttons = new Button[BOARD_WIDTH, BOARD_WIDTH];
        private readonly int[,] values = new int[BOARD_WIDTH, BOARD_WIDTH];
        private readonly int[,] oldValues = new int[BOARD_WIDTH, BOARD_WIDTH];
        private int[] scoreTable = { 0, 0, 0, 0, 0 };   // initialise array with 0 values
        private int score = 0;
        private Label scoreLabel;
        private Button undoButton;
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
            AddUndoButton();
            GenerateNumber(2, -1);
            CopyValues();
            Redraw();
            loadFromFile();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.mainForm.Size = new Size(SIDE_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * BOARD_WIDTH + SIDE_MARGIN, TOP_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * BOARD_WIDTH + BOTTOM_MARGIN);
            Program.mainForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            Program.mainForm.MaximizeBox = false;
            Program.mainForm.MinimizeBox = false;
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            Undo();
            CopyValues();
            Redraw();
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

            CopyValues();
            bool moved = false;
            int direction = -1;
            if (clickedY == 0 && (clickedX != 0 && clickedX != BOARD_WIDTH - 1))
            {
                moved = MoveUp();
                direction = 0;
            }
            else if (clickedX == BOARD_WIDTH - 1 && (clickedY != 0 && clickedY != BOARD_WIDTH - 1))
            {
                moved = MoveRight();
                direction = 1;
            }
            else if (clickedY == BOARD_WIDTH - 1 && (clickedX != 0 && clickedX != BOARD_WIDTH - 1))
            {
                moved = MoveDown();
                direction = 2;
            }
            else if (clickedX == 0 && (clickedY != 0 && clickedY != BOARD_WIDTH - 1))
            {
                moved = MoveLeft();
                direction = 3;
            }
            Redraw();
            if (moved)
            {
                GenerateNumber(1, direction);
                Redraw();
            }
        }

        private void GenerateNumber(int amount, int direction)
        {
            Random random = new Random(); // Generates new number
            if (amount == 1) // If we need to generate only 1 new number
            {
                Coordinates coordinates = CheckOpenSpace(direction); // gets the coordinates of a space
                if (coordinates == null) // if coordinates are null that means there are no more open spaces left
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
                Coordinates coordinates = CheckOpenSpace(direction); // gets the coordinates of a space
                values[coordinates.x, coordinates.y] = 4;
                coordinates = CheckOpenSpace(direction);
                values[coordinates.x, coordinates.y] = 2;
            }
        }

        private Coordinates CheckOpenSpace(int direction)
        {
            Coordinates OpenSpace = null;
            int count = 0; // the count of free spaces
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    if (values[x, y] == 0) // if it's a free space
                    {
                        if (count == 0) // if it's the 1st free space then get it's coordinates
                        {
                            OpenSpace = new Coordinates(x, y);
                        }
                        count++;
                    }
                }
            }
            if (count > 1) // if there are more then 1 open space available
            {
                if (IS_HARD_MODE && direction != -1)
                {
                    int[] maxValues = new int[BOARD_WIDTH];
                    int[] xCoordinates = new int[BOARD_WIDTH];
                    int[] yCoordinates = new int[BOARD_WIDTH];
                    int finalX = -1;
                    int finalY = -1;
                    if (direction == 0)
                    {
                        for (int x = 0; x < BOARD_WIDTH; x++)
                        {
                            int firstValue = 0;
                            int index = BOARD_WIDTH - 1;
                            while (firstValue == 0 && index >= 0)
                            {
                                firstValue = values[x, index];
                                index--;
                            }
                            maxValues[x] = index + 2 < BOARD_WIDTH ? firstValue : 0;
                            yCoordinates[x] = index + 2;
                        }
                        int maxValue = 0;
                        for (int x = 0; x < BOARD_WIDTH; x++)
                        {
                            if (maxValues[x] > maxValue)
                            {
                                maxValue = maxValues[x];
                                finalX = x;
                                finalY = yCoordinates[x];
                            }
                        }
                    }
                    else if (direction == 1)
                    {
                        for (int y = 0; y < BOARD_WIDTH; y++)
                        {
                            int firstValue = 0;
                            int index = 0;
                            while (firstValue == 0 && index < BOARD_WIDTH)
                            {
                                firstValue = values[index, y];
                                index++;
                            }
                            maxValues[y] = index - 2 >= 0 ? firstValue : 0;
                            xCoordinates[y] = index - 2;
                        }
                        int maxValue = 0;
                        for (int y = 0; y < BOARD_WIDTH; y++)
                        {
                            if (maxValues[y] > maxValue)
                            {
                                maxValue = maxValues[y];
                                finalY = y;
                                finalX = xCoordinates[y];
                            }
                        }
                    }
                    else if (direction == 2)
                    {
                        for (int x = 0; x < BOARD_WIDTH; x++)
                        {
                            int firstValue = 0;
                            int index = 0;
                            while (firstValue == 0 && index < BOARD_WIDTH)
                            {
                                firstValue = values[x, index];
                                index++;
                            }
                            maxValues[x] = index - 2 >= 0 ? firstValue : 0;
                            yCoordinates[x] = index - 2;
                        }
                        int maxValue = 0;
                        for (int x = 0; x < BOARD_WIDTH; x++)
                        {
                            if (maxValues[x] > maxValue)
                            {
                                maxValue = maxValues[x];
                                finalX = x;
                                finalY = yCoordinates[x];
                            }
                        }
                    }
                    else if (direction == 3)
                    {
                        for (int y = 0; y < BOARD_WIDTH; y++)
                        {
                            int firstValue = 0;
                            int index = BOARD_WIDTH - 1;
                            while (firstValue == 0 && index >= 0)
                            {
                                firstValue = values[index, y];
                                index--;
                            }
                            maxValues[y] = index + 2 < BOARD_WIDTH ? firstValue : 0;
                            xCoordinates[y] = index + 2;
                        }
                        int maxValue = 0;
                        for (int y = 0; y < BOARD_WIDTH; y++)
                        {
                            if (maxValues[y] > maxValue)
                            {
                                maxValue = maxValues[y];
                                finalY = y;
                                finalX = xCoordinates[y];
                            }
                        }
                    }


                    if (finalX == -1 || finalY == -1 || finalX >= BOARD_WIDTH || finalY >= BOARD_WIDTH)
                    {
                        direction = -1;
                    }
                    else
                    {
                        OpenSpace = new Coordinates(finalX, finalY);
                    }
                }
                if (!IS_HARD_MODE || direction == -1)
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
            saveToFile();
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
                    if (values[x, y] == 0)
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
            scoreLabel.Font = new Font(FONT, FONT_SIZE_SMALL, FontStyle.Bold); //font
            Controls.Add(scoreLabel);
        }

        private void AddUndoButton()
        {
            undoButton = new Button();
            undoButton.SetBounds(SIDE_MARGIN - TILE_MARGIN, TOP_MARGIN - TILE_MARGIN - TILE_WIDTH, TILE_WIDTH, TILE_WIDTH / 2);
            undoButton.Click += new EventHandler(this.UndoButton_Click);
            undoButton.FlatStyle = FlatStyle.Flat;
            undoButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#bbada0");
            undoButton.FlatAppearance.BorderSize = 4;
            undoButton.BackColor = ColorTranslator.FromHtml(COLORS[0]);
            undoButton.ForeColor = ColorTranslator.FromHtml(DEFAULT_TEXT_COLOR);
            undoButton.Font = new Font(FONT, FONT_SIZE_EXTRA_SMALL, FontStyle.Bold);
            undoButton.Text = "UNDO";
            Controls.Add(undoButton);
        }

        public void RestartGame()
        {
            // restart game grid
           
            Array.Clear(values, 0, BOARD_WIDTH * BOARD_WIDTH);
            GenerateNumber(2, -1);
            Redraw();
            
        }

        protected override bool ProcessCmdKey(ref Message message, Keys key)
        {
            bool moved = false;
            bool processed = false;
            int direction = -1;
            switch (key)
            {
                case Keys.Up:
                    CopyValues();
                    moved = MoveUp();
                    processed = true;
                    direction = 0;
                    break;
                case Keys.Right:
                    CopyValues();
                    moved = MoveRight();
                    processed = true;
                    direction = 1;
                    break;
                case Keys.Down:
                    CopyValues();
                    moved = MoveDown();
                    processed = true;
                    direction = 2;
                    break;
                case Keys.Left:
                    CopyValues();
                    moved = MoveLeft();
                    processed = true;
                    direction = 3;
                    break;
                default:
                    processed = false;
                    break;
            }
            Redraw();
            if (moved)
            {
                GenerateNumber(1, direction);
                Redraw();
            }

            if (processed)
            {
                return true;
            }
            return base.ProcessCmdKey(ref message, key);
        }

        public void CopyValues()
        {
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    oldValues[x, y] = values[x, y];
                }
            }
        }

        public void Undo()
        {
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    values[x, y] = oldValues[x, y];
                }
            }
        }


        public void loadFromFile()
        {
            if (System.IO.File.Exists(SAVE_FILE)) // Check if file exists
            {
                string line;
                int a = 0;

                System.IO.StreamReader file = new System.IO.StreamReader(SAVE_FILE); // Open file
                while ((line = file.ReadLine()) != null)
                {
                    scoreTable[a] = int.Parse(line); // read the contents of the file to the array
                    a++;
                }
                file.Close();
            }
        }

        public void saveToFile()
        {
            if (inserScore()) //if the scoreboard changed
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(SAVE_FILE))
                {
                    foreach (int number in scoreTable)
                    {

                        file.WriteLine(number.ToString());
                    }
                }
            }


        }
   
        
        public bool inserScore()
        {
            bool inserted = false;
            
            int num1;   // used for swaping values
            int num2;

            int tableLenght = scoreTable.Length;
           
            if (score <= scoreTable[tableLenght-1]) //score is lower then the top 5 so it will not be inserted to the table
            {
                   inserted = false;
            }
            else
                {
                    int x = 0;
                    while(!inserted && x < tableLenght)
                    { 
                        
                        if(score > scoreTable[x])
                        {
                            num1 = scoreTable[x];
                            scoreTable[x] = score;
                            for(int i = x+1; i < tableLenght;i++)
                            {
                                num2 = scoreTable[i];
                                scoreTable[i] = num1;
                                num1 = num2;

                            }
                        inserted = true;
                        }
                    x++;
                    }

                    
                }
          

            return inserted;

        }

        private void scoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scoreTable[0] != 0) //if there is something recorded in the score table
            {
                ScoreTable table = new ScoreTable(scoreTable);
                table.ShowDialog();
            }
            else
            {
                MessageBox.Show("No scores recorded!", "Error");
            }
        }
    }
    
}
