using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace checkers
{
    public partial class Form1 : Form
    {

        const int mapSize = 8;
        const int cellSize = 70;

        int currentPlayer;


        int countBeatSteps = 0;
        Button previousButton;
        Button pressedButton;
        bool isContinue = false;

        bool isMoving;

        Button[,] buttons = new Button[mapSize, mapSize];

        int[,] map = new int[mapSize, mapSize];

        Image whiteFigure;
        Image blackFigure;

        public Form1()
        {
            InitializeComponent();

            whiteFigure = new Bitmap(new Bitmap(@"C:\Users\User\source\repos\checkers\images\w.png"), new Size(cellSize - 10, cellSize - 10));
            blackFigure = new Bitmap(new Bitmap(@"C:\Users\User\source\repos\checkers\images\b.png"), new Size(cellSize - 10, cellSize - 10));

            this.Text = "Checkers";

            Init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void Init()
        {
            currentPlayer = 1;
            isMoving = false;
            previousButton = null;

            map = new int[mapSize, mapSize]{
                { 0,1,0,1,0,1,0,1},
                { 1,0,1,0,1,0,1,0},
                { 0,1,0,1,0,1,0,1},
                { 0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0},
                { 2,0,2,0,2,0,2,0},
                { 0,2,0,2,0,2,0,2},
                { 2,0,2,0,2,0,2,0}
            };
            CreateMap();
        }

        public void CreateMap()
        {
            this.Width = (mapSize + 1) * cellSize;
            this.Height = (mapSize + 1) * cellSize;

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.Click += new EventHandler(PressOnFigure);
                    if (map[i, j] == 1)
                        button.Image = whiteFigure;
                    else if(map[i, j] == 2)
                        button.Image = blackFigure;
                    button.BackColor = GetPreviousFigureColor(button);
                    this.Controls.Add(button);
                }
            }
        }

        public void SwitchPlayer()
        {
            currentPlayer = currentPlayer == 1 ? 2 : 1;
        }

        public Color GetPreviousFigureColor(Button previousButton)
        {
            int previousButtonLocationX = previousButton.Location.X / cellSize;
            int previousButtonLocationY = previousButton.Location.Y / cellSize;
            if ((previousButtonLocationX % 2 != 0 && previousButtonLocationY % 2 == 0) || 
                (previousButtonLocationX % 2 == 0 && previousButtonLocationY % 2 != 0))
                {
                    return Color.Black;
                }
            return Color.White;
        }
        public void PressOnFigure(object sender, EventArgs e)
        {
            if (previousButton != null)
                previousButton.BackColor = GetPreviousFigureColor(previousButton);

            Button pressedButton = sender as Button;

            if (map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] != 0 &&
                map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] == currentPlayer)
                {
                    pressedButton.BackColor = Color.Red;
                    isMoving = true;
                }
            else
            {
                if(isMoving)
                {
                    int forSwap = map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize];
                    map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] = map[previousButton.Location.Y / cellSize, previousButton.Location.X / cellSize];
                    map[previousButton.Location.Y / cellSize, previousButton.Location.X / cellSize] = forSwap;
                    pressedButton.Image = previousButton.Image;
                    previousButton.Image = null;
                    isMoving = false;
                    SwitchPlayer();
                }
            }

            previousButton = pressedButton;
        }

        public void ShowWhatToBeat(int x, int y, bool isQueen = false)
        {
            int directionX = x - pressedButton.Location.Y / cellSize;
            int directionY = y - pressedButton.Location.X / cellSize;
            directionX = directionX < 0 ? -1 : 1;
            directionY = directionY < 0 ? -1 : 1;
            int i = x, j = y;
            bool nothingToBeat = true;
            while (IsInsideBoarders(i, j))
            {
                if (map[i, j] != 0 && map[i, j] != currentPlayer)
                {
                    nothingToBeat = false;
                    break;
                }

                i += directionX;
                j += directionY;
                
                if(!isQueen) break;
            }
            
            if(nothingToBeat) return;
            List<Button> stayAfterBeat = new List<Button>();
            bool afterBeat = false;
            i += directionX;
            j += directionY;
            while (IsInsideBoarders(i, j))
            {
                if (map[i, j] == 0)
                {
                    if (HaveFigureToBeat(i, j, isQueen, new int[2] { directionX, directionY }))
                    {
                        afterBeat = true;
                    }
                    else
                    {
                        stayAfterBeat.Add(buttons[i, j]);
                    }

                    buttons[i, j].BackColor = Color.LemonChiffon;
                    buttons[i, j].Enabled = true;
                }
                else break;
                if(!isQueen) break;
                i += directionX;
                j += directionY;
            }

            if (afterBeat && stayAfterBeat.Count > 0)
            {
                CloseAfterSteps(stayAfterBeat);
            }

        }

        public bool HaveFigureToBeatDirection(int positionOfX, int positionOfY, bool isQueen, int[] direction, int x, int y)
        {
            bool beat = false;
            for (int i = positionOfX + x, j = positionOfY + y ; IsInsideBoarders(i, j); i += x, j += y)
            {
                if (currentPlayer == 1 && !isQueen && !isContinue) break;
                if (direction[0] == -x && direction[1] == -y && isQueen) break;
                if (IsInsideBoarders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer)
                    {
                        beat = true;
                        if (!IsInsideBoarders(i + x, j + y)) beat = false;
                        else if (map[i, j] != 0) beat = false;
                        else return beat;
                    }
                }

                if (!isQueen) break;
            }
            return beat;
        }
        
        public bool HaveFigureToBeat(int positionOfX, int positionOfY, bool isQueen, int[] direction)
        {
            if (HaveFigureToBeatDirection(positionOfX, positionOfX, isQueen, direction, 1, 1))
                return true;
            else if (HaveFigureToBeatDirection(positionOfX, positionOfX, isQueen, direction, 1, -1))
                return true;
            else if (HaveFigureToBeatDirection(positionOfX, positionOfX, isQueen, direction, -1, 1))
                return true;
            else if (HaveFigureToBeatDirection(positionOfX, positionOfX, isQueen, direction, -1, -1))
                return true;
            return false;
        }


        public void CloseSteps()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    buttons[i, j].BackColor = GetPreviousFigureColor(buttons[i, j]);
                }
            }
        }
        public bool IsInsideBoarders(int i, int j)
        {
            return (i >= mapSize || j >= mapSize || i < 0 || j < 0) ? false : true;
        }
        public void ActivateAllButtons()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    buttons[i, j].Enabled = true;
                }
            }
        }
        public void DeactivateAllButtons()
        {
            for(int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                    buttons[i, j].Enabled = false;
            }
        }

}
}