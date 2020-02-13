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
    public partial class RulesForm : Form
    {
        private const int TOP_MARGIN = 96; //multiple of 8 for consistency
        private const int BOTTOM_MARGIN = 96;
        private const int SIDE_MARGIN = 96;
        private const int BUTTON_WIDTH = 120;
        private const int BUTTON_HEIGHT = 40;
        private readonly string[] COLORS = new[] { "#CDC1B5", "#EEE4DB", "#F1E2CE", "#F0B57D", "#F19F64", "#F77E73", "#FD5644", "#F1D275", "#F1D275", "#E6C847", "#EDBE4A", "#EFBE45" };
        private const string BACKGROUND_COLOR = "#faf8ef";
        private const string DEFAULT_TEXT_COLOR = "#000000";
        private const string FONT = "Courier New";
        private const int FONT_SIZE_EXTRA_SMALL = 12;

        private Button movementRuleButton;
        private Button winRuleButton;
        private Button loseRuleButton;
        private Button backButton;
        private Label ruleLabel;
        private string[] rulesText = new string[] {"Combine two squares with the same value to make one square with a two times larger value.\n" +
                "Press top row's middle buttons to move UP.\n" +
                "Press bottom row's middle buttons to move DOWN.\n" +
                "Press left collumn's middle buttons to move LEFT.\n" +
                "Press right collumn's middle buttons to move RIGHT.\n",

                "The player wins the game if he makes a square with value 2048.",

                "The player loses the game if there are no valid moves.\n" +
                "(Grid is full and none of the squares can be combined.)"
        };

        public RulesForm()
        {
            InitializeComponent();

            BackColor = ColorTranslator.FromHtml(BACKGROUND_COLOR);
            Size = new Size(SIDE_MARGIN * 2 + BUTTON_WIDTH * 3, TOP_MARGIN + BOTTOM_MARGIN + BUTTON_HEIGHT);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            AddMovementRuleButton();
            AddWinRuleButton();
            AddLoseRuleButton();
            AddBackButton();
            AddRuleLabel();

            StartPosition = FormStartPosition.CenterScreen;
            Visible = true;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void AddMovementRuleButton()
        {
            movementRuleButton = new Button();
            movementRuleButton.SetBounds(SIDE_MARGIN, TOP_MARGIN, BUTTON_WIDTH, BUTTON_HEIGHT);
            movementRuleButton.Click += new EventHandler(this.MovementButton_Click);
            movementRuleButton.Text = "MOVEMENT";
            movementRuleButton.FlatStyle = FlatStyle.Flat;
            movementRuleButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#bbada0");
            movementRuleButton.FlatAppearance.BorderSize = 4;
            movementRuleButton.BackColor = ColorTranslator.FromHtml(COLORS[0]);
            movementRuleButton.ForeColor = ColorTranslator.FromHtml(DEFAULT_TEXT_COLOR);
            movementRuleButton.Font = new Font(FONT, FONT_SIZE_EXTRA_SMALL, FontStyle.Bold);
            Controls.Add(movementRuleButton);
        }

        private void AddWinRuleButton()
        {
            winRuleButton = new Button();
            winRuleButton.SetBounds(SIDE_MARGIN + BUTTON_WIDTH, TOP_MARGIN, BUTTON_WIDTH, BUTTON_HEIGHT);
            winRuleButton.Click += new EventHandler(this.WinButton_Click);
            winRuleButton.Text = "GOAL";
            winRuleButton.FlatStyle = FlatStyle.Flat;
            winRuleButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#bbada0");
            winRuleButton.FlatAppearance.BorderSize = 4;
            winRuleButton.BackColor = ColorTranslator.FromHtml(COLORS[0]);
            winRuleButton.ForeColor = ColorTranslator.FromHtml(DEFAULT_TEXT_COLOR);
            winRuleButton.Font = new Font(FONT, FONT_SIZE_EXTRA_SMALL, FontStyle.Bold);
            Controls.Add(winRuleButton);
        }

        private void AddLoseRuleButton()
        {
            loseRuleButton = new Button();
            loseRuleButton.SetBounds(SIDE_MARGIN + BUTTON_WIDTH * 2, TOP_MARGIN, BUTTON_WIDTH, BUTTON_HEIGHT);
            loseRuleButton.Click += new EventHandler(this.LoseButton_Click);
            loseRuleButton.Text = "LOSS";
            loseRuleButton.FlatStyle = FlatStyle.Flat;
            loseRuleButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#bbada0");
            loseRuleButton.FlatAppearance.BorderSize = 4;
            loseRuleButton.BackColor = ColorTranslator.FromHtml(COLORS[0]);
            loseRuleButton.ForeColor = ColorTranslator.FromHtml(DEFAULT_TEXT_COLOR);
            loseRuleButton.Font = new Font(FONT, FONT_SIZE_EXTRA_SMALL, FontStyle.Bold);
            Controls.Add(loseRuleButton);
        }

        private void AddBackButton()
        {
            backButton = new Button();
            backButton.SetBounds(SIDE_MARGIN + BUTTON_WIDTH, TOP_MARGIN + BUTTON_HEIGHT, BUTTON_WIDTH, BUTTON_HEIGHT);
            backButton.Click += new EventHandler(this.BackButton_Click);
            backButton.Text = "BACK";
            backButton.FlatStyle = FlatStyle.Flat;
            backButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#bbada0");
            backButton.FlatAppearance.BorderSize = 4;
            backButton.BackColor = ColorTranslator.FromHtml(COLORS[0]);
            backButton.ForeColor = ColorTranslator.FromHtml(DEFAULT_TEXT_COLOR);
            backButton.Font = new Font(FONT, FONT_SIZE_EXTRA_SMALL, FontStyle.Bold);
            backButton.Visible = false;
            Controls.Add(backButton);
        }

        private void AddRuleLabel()
        {
            ruleLabel = new Label();
            ruleLabel.Size = new Size(SIDE_MARGIN * 2 + BUTTON_WIDTH * 3, TOP_MARGIN + BUTTON_HEIGHT);
            ruleLabel.TextAlign = ContentAlignment.MiddleCenter;
            ruleLabel.Font = new Font(FONT, FONT_SIZE_EXTRA_SMALL);
            Controls.Add(ruleLabel);
        }

        private void MovementButton_Click(object sender, EventArgs e)
        {
            displayRule(0);
        }

        private void WinButton_Click(object sender, EventArgs e)
        {
            displayRule(1);
        }

        private void LoseButton_Click(object sender, EventArgs e)
        {
            displayRule(2);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            ruleLabel.Visible = false;
            backButton.Visible = false;
            movementRuleButton.Visible = true;
            winRuleButton.Visible = true;
            loseRuleButton.Visible = true;
        }

        private void displayRule(int ruleNumber)
        {
            movementRuleButton.Visible = false;
            winRuleButton.Visible = false;
            loseRuleButton.Visible = false;

            if (ruleNumber < rulesText.Length)
            {
                ruleLabel.Text = rulesText[ruleNumber];
            }
            ruleLabel.Visible = true;
            backButton.Visible = true;
        }
    }
}