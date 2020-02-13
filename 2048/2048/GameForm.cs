using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace _2048
{
    public partial class GameForm : Form
    {
        // constants
        public const int BOARD_WIDTH = 4; // board size, allowed to be changed before launching the game
        private const int TOP_MARGIN = 136; // multiple of 8 for consistency
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
        private const string SAVE_FILE = "scores.txt";

        private const string BACKGROUND_MUSIC = "background.wav";
        private System.Media.SoundPlayer Background_player = new System.Media.SoundPlayer(); // Background music

        private bool soundOn = true;
        private bool isHardMode = false;
        private readonly Button[,] buttons = new Button[BOARD_WIDTH, BOARD_WIDTH];
        private readonly int[,] values = new int[BOARD_WIDTH, BOARD_WIDTH];
        private readonly int[,] oldValues = new int[BOARD_WIDTH, BOARD_WIDTH]; // used for undo functionality
        private int[] scoreTable = { 0, 0, 0, 0, 0 };   // initialise array with 0 values
        private int score = 0;
        private Label scoreLabel;
        private Label gameOverLabel;
        private Button undoButton;
        private Button toggleSoundButton;

        public GameForm()
        {
            InitializeComponent();
            this.BackColor = ColorTranslator.FromHtml(BACKGROUND_COLOR); //changes form background
            menuStrip2.BackColor = ColorTranslator.FromHtml(BACKGROUND_COLOR);
            if (System.IO.File.Exists(BACKGROUND_MUSIC) && soundOn) // Check if file exists
            {
                Background_player.SoundLocation = BACKGROUND_MUSIC;
                Background_player.PlayLooping();
            }

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
            AddSoundButton();
            AddGameOverLabel();
            GenerateNumber(2, -1);
            CopyValues();
            Redraw();
            LoadFromFile();
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
            if (CheckIfMovesAvailable())
            {
                Undo();
                CopyValues();
                Redraw();
            }
        }
        private void toogleSoundButton_Click(object sender, EventArgs e)
        { 
            if(soundOn)
            {
                Background_player.Stop();
                toggleSoundButton.Text = "Sound OFF";
                soundOn = false;
            }
            else
            {
                if (System.IO.File.Exists(BACKGROUND_MUSIC))
                {
                    Background_player.SoundLocation = BACKGROUND_MUSIC;
                    Background_player.PlayLooping();
                    toggleSoundButton.Text = "Sound ON";
                    soundOn = true;
                }
            }
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

            if (!CheckIfMovesAvailable())
            {
                gameOverLabel.Text = "Game Over";
                gameOverLabel.Visible = true;
            }
        }

        private void GenerateNumber(int amount, int direction)
        {
            Random random = new Random(); // Generates new number
            if (amount == 1) // If we need to generate only 1 new number
            {
                Coordinates coordinates = CheckOpenSpace(direction); // gets the coordinates of a space
                if (coordinates.x == -1 || coordinates.y == -1) // if coordinates are null that means there are no more open spaces left
                {
                    //GAME OVER (NO MORE SPACE LEFT)
                    Console.WriteLine("ENDDD");
                    return;
                }
                int number = random.Next(0, 1) == 0 ? 2 : 4; // generate a random number and decide if it's a 2 or a 4
                values[coordinates.x, coordinates.y] = number;
                oldValues[coordinates.x, coordinates.y] = number;
            }
            else // if we need 2 numbers
            {
                Coordinates coordinates = CheckOpenSpace(direction); // gets the coordinates of a space
                values[coordinates.x, coordinates.y] = 4;
                oldValues[coordinates.x, coordinates.y] = 4;
                coordinates = CheckOpenSpace(direction);
                values[coordinates.x, coordinates.y] = 2;
                oldValues[coordinates.x, coordinates.y] = 2;
            }
        }

        private Coordinates CheckOpenSpace(int direction)
        {
            Coordinates OpenSpace = new Coordinates(-1, -1);
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
                if (isHardMode && direction != -1)
                {
                    /* The idea with hard mode is that it places the newly generated tiles in the worst possible position
                     * The worst position is along the edge opposite to the previous move, right next to the highest value on that row/column
                     * Placing it in the opposite direction next to the highest value makes it harder to group values together before merging
                     */
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
                if (!isHardMode || direction == -1)
                {
                    Random random = new Random();
                    int x, y;
                    do
                    {
                        x = random.Next(0, BOARD_WIDTH);
                        y = random.Next(0, BOARD_WIDTH);
                    } while (values[x, y] != 0); // repeat the loop until an open space is found
                    OpenSpace = new Coordinates(x, y);
                }

            }

            return OpenSpace;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveToFile();
            Close();
        }

        private void RulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
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
            */
            RulesForm ruleForm = new RulesForm();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
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
                        if (value * 2 == 2048)
                        {
                            gameOverLabel.Text = "You Won!";
                            gameOverLabel.Visible = true;
                        }
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
            for (int x = 0; x < BOARD_WIDTH; x++)   // goes through the board and redraws the text
            {
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    if (values[x, y] == 0)
                    {
                        buttons[x, y].Text = "";
                    }
                    else
                    {
                        buttons[x, y].Text = values[x, y].ToString(); // sets the button text to the correct value
                    }
                    ChangeColor(x, y); // changes the button text and background colors
                }
            }
            scoreLabel.Text = "Score: " + Convert.ToString(score); // updates scoreLabel text
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
            scoreLabel.Location = new Point(SIDE_MARGIN, TOP_MARGIN - TILE_WIDTH / 2);
            scoreLabel.Font = new Font(FONT, FONT_SIZE_SMALL, FontStyle.Bold); //font
            Controls.Add(scoreLabel);
        }

        private void AddUndoButton()
        {
            undoButton = new Button();
            undoButton.SetBounds(SIDE_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * (BOARD_WIDTH - 1), TOP_MARGIN - TILE_MARGIN - TILE_WIDTH / 2 - 8, TILE_WIDTH, TILE_WIDTH / 2);
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
        private void AddSoundButton()
        {
            toggleSoundButton = new Button();
            toggleSoundButton.SetBounds(SIDE_MARGIN - TILE_MARGIN + (TILE_WIDTH + TILE_MARGIN) * (BOARD_WIDTH - 1), TOP_MARGIN - TILE_MARGIN - TILE_WIDTH / 2 - 16 - TILE_WIDTH/2 , TILE_WIDTH, TILE_WIDTH / 2);
            toggleSoundButton.Click += new EventHandler(this.toogleSoundButton_Click);
            toggleSoundButton.FlatStyle = FlatStyle.Flat;
            toggleSoundButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#bbada0");
            toggleSoundButton.FlatAppearance.BorderSize = 4;
            toggleSoundButton.BackColor = ColorTranslator.FromHtml(COLORS[0]);
            toggleSoundButton.ForeColor = ColorTranslator.FromHtml(DEFAULT_TEXT_COLOR);
            toggleSoundButton.Font = new Font(FONT, 7, FontStyle.Bold);
            toggleSoundButton.Text = "Sound ON";
            Controls.Add(toggleSoundButton);
        }
        private void AddGameOverLabel()
        {
            gameOverLabel = new Label();
            gameOverLabel.AutoSize = true;
            gameOverLabel.Location = new Point(SIDE_MARGIN, TOP_MARGIN - TILE_WIDTH - TILE_MARGIN);
            gameOverLabel.Font = new Font(FONT, FONT_SIZE_DEFAULT, FontStyle.Bold); //font
            gameOverLabel.Visible = false;
            Controls.Add(gameOverLabel);
        }

        private void RestartNormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isHardMode = false;
            RestartGame();
        }

        private void RestartHardagainstAIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isHardMode = true;
            RestartGame();
        }

        public void RestartGame()
        {
            // restart game grid
            SaveToFile();
            score = 0;
            Array.Clear(values, 0, BOARD_WIDTH * BOARD_WIDTH);
            Array.Clear(oldValues, 0, BOARD_WIDTH * BOARD_WIDTH);
            GenerateNumber(2, -1);
            Redraw();
            gameOverLabel.Text = "";
            gameOverLabel.Visible = false;
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

            if (!CheckIfMovesAvailable())
            {
                gameOverLabel.Text = "Game Over";
                gameOverLabel.Visible = true;
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

        public void LoadFromFile()
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

        public void SaveToFile()
        {
            if (InsertScore()) // if the scoreboard changed
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(SAVE_FILE);
                for(int x = 0; x < scoreTable.Length; x++)
                {
                    file.WriteLine(scoreTable[x]);
                }
                file.Close();
            }
        }

        public bool InsertScore()
        {
            bool inserted = false;
            
            int num1;   // used for swaping values
            int num2;

            int tableLenght = scoreTable.Length;
           
            if (score <= scoreTable[tableLenght-1]) // score is lower then the top 5 so it will not be inserted to the table
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

        private void ScoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scoreTable[0] != 0) // if there is something recorded in the score table
            {
                ScoreTableForm table = new ScoreTableForm(scoreTable);
                table.ShowDialog();
            }
            else
            {
                MessageBox.Show("No scores recorded!", "Error");
            }
        }

        private bool CheckIfMovesAvailable()
        {
            bool isEmpty = false;
            bool isConcecutive = false;
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                for (int y = 0; y < BOARD_WIDTH; y++)
                {
                    if (values[x, y] == 0)
                    {
                        isEmpty = true;
                    }
                    if (y > 0)
                    {
                        if (values[x, y - 1] == values[x, y])
                        {
                            isConcecutive = true;
                        }
                        if (values[y - 1, x] == values[y, x])
                        {
                            isConcecutive = true;
                        }
                    }
                    if (y < BOARD_WIDTH - 1)
                    {
                        if (values[x, y] == values[x, y + 1])
                        {
                            isConcecutive = true;
                        }
                        if (values[y, x] == values[y + 1, x])
                        {
                            isConcecutive = true;
                        }
                    }
                }
            }
            return isConcecutive || isEmpty;
        }
    }
    
}
