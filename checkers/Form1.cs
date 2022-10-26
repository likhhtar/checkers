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
        private int white = 1, black = 2;

        int countBeatSteps = 0;
        Button previousButton;
        Button pressedButton;
        bool isContinue = false;
        public bool isMusicPlaying = false;

        bool isMoving;

        Button[,] buttons = new Button[mapSize, mapSize];
        List<Button> nextSteps = new List<Button>();
        int[,] map = new int[mapSize, mapSize];

        Image whiteFigure;
        Image blackFigure;
        Image whiteQueenFigure;
        Image blackQueenFigure;
        
        public Button musicButton;
        //public Button restartButton;
        System.Media.SoundPlayer music = new System.Media.SoundPlayer(@"C:\Users\User\source\repos\checkers\audio\main.wav");

        public Button restartButton = new Button()
        {
            BackgroundImage = Image.FromFile("C:/Users/User/source/repos/checkers/images/restart2.png"),  
            Size = new Size(40, 35),
            Location = new Point(cellSize/2 - 20, (mapSize + 1) * cellSize + 15)
        };
        

        
        public Label winFirst = new Label()
        {
            Size = new Size(500, 50),
            Font = new Font("Arial", 40),
            Location = new Point(mapSize * cellSize / 2 - 190, mapSize * cellSize / 2 + 50),
            Text = "YOU WON!!!!",
            ForeColor = Color.Green
        };
    
        public Label winSecond = new Label()
        {
            Size = new Size(500, 50),
            Font = new Font("Arial", 40),
            Location = new Point(mapSize * cellSize / 2 - 190, mapSize * cellSize / 2 + 50),
            Text = "YOU LOSE",
            ForeColor = Color.Red
        };
        
        public Form1()
        {
            InitializeComponent();

            whiteQueenFigure = new Bitmap(new Bitmap(@"C:\Users\User\source\repos\checkers\images\w1_q.png"), new Size(cellSize - 10, cellSize - 10));
            blackQueenFigure = new Bitmap(new Bitmap(@"C:\Users\User\source\repos\checkers\images\b1_q.png"), new Size(cellSize - 10, cellSize - 10));
            whiteFigure = new Bitmap(new Bitmap(@"C:\Users\User\source\repos\checkers\images\w1.png"), new Size(cellSize - 10, cellSize - 10));
            blackFigure = new Bitmap(new Bitmap(@"C:\Users\User\source\repos\checkers\images\b1.png"), new Size(cellSize - 10, cellSize - 10));

            this.Text = "Checkers";

            Init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void Init()
        {
            currentPlayer = 2;
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
            
            musicButton = new Button();
            musicButton.BackColor = Color.Beige;
            musicButton.BackgroundImage = Image.FromFile("C:/Users/User/source/repos/Sea battle/images/soundoff2.png");  
            musicButton.Size = new Size(40, 35);
            musicButton.Location = new Point((mapSize + 1) * cellSize + 15, cellSize/2 - 20);
            musicButton.Click += new EventHandler(Music);
            this.Controls.Add(musicButton);
            
            
            restartButton.Click += new EventHandler(Restart);

            
            
            if (isMusicPlaying) music.PlayLooping();
            CreateMap();
        }
        
        public void Music(object sender, EventArgs e)
        {
            if (isMusicPlaying)
            {
                isMusicPlaying = false;
                musicButton.BackgroundImage = Image.FromFile("C:/Users/User/source/repos/checkers/images/soundoff2.png");
                music.Stop();
            }
            else
            {
                isMusicPlaying = true;
                musicButton.BackgroundImage = Image.FromFile("C:/Users/User/source/repos/checkers/images/sound2.png");
                music.PlayLooping();
            }
        }

        public void ResetGame()
        {
            bool player1 = false;
            bool player2 = false;
            bool notStep = false;

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == 2) player1 = true;
                    if (map[i, j] == 1) player2 = true;
                }
            }
            
        
            if (!player1 || !player2) //кто-то выйграл
            {
                if (player1)
                {
                    if(isMusicPlaying) music.Stop();
                    WinFirst();
                }
                else
                {
                    if(isMusicPlaying) music.Stop();
                    WinSecond();
                }

                restartButton.BackgroundImage =
                    Image.FromFile("C:/Users/User/source/repos/checkers/images/restart2.png");
            }
        }
        
        public void WinFirst()
        {
            this.Controls.Add(winFirst);
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\User\source\repos\checkers\audio\win.wav");
            if(isMusicPlaying) player.Play();
        }

        public void WinSecond()
        {
            this.Controls.Add(winSecond);
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\User\source\repos\checkers\audio\lost.wav");
            if(isMusicPlaying) player.Play();
        }

        public void CreateMap()
        {
            if (buttons[0, 0] != null) return;
            
            this.Width = (mapSize + 2) * cellSize + 20;
            this.Height = (mapSize + 2) * cellSize + 40;
            this.MinimumSize = new Size((mapSize + 2) * cellSize + 20, (mapSize + 2) * cellSize + 40);
            this.MaximumSize = new Size((mapSize + 2) * cellSize + 20, (mapSize + 2) * cellSize + 40);
            
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point((j + 1) * cellSize, (i + 1) * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.Click += new EventHandler(PressOnFigure);
                    if (map[i, j] == 2)
                        button.Image = whiteFigure;
                    else if(map[i, j] == 1)
                        button.Image = blackFigure;
                    button.BackColor = GetPreviousButtonColor(button);
                    buttons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            for (int i = 0; i <= mapSize + 1; i++)
            {
                Button button = new Button();
                button.Location = new Point(0, i * cellSize);
                if(i != 0 && i != mapSize + 1) button.Text = (mapSize - i + 1).ToString();
                button.Font = new Font("Arial", 10);
                button.Size = new Size(cellSize, cellSize);
                button.BackColor = Color.DarkRed;
                this.Controls.Add(button);
                
                Button button2 = new Button();
                button2.Location = new Point((mapSize + 1) * cellSize, i * cellSize);
                if(i != 0 && i != mapSize + 1) button2.Text = (mapSize - i + 1).ToString();
                button2.Font = new Font("Arial", 10);
                button2.Size = new Size(cellSize, cellSize);
                button2.BackColor = Color.DarkRed;
                this.Controls.Add(button2);
            }
            
            for (int i = 0; i <= mapSize + 1; i++)
            {
                Button button = new Button();
                button.Location = new Point(i * cellSize, 0);
                if(i != 0 && i != mapSize + 1) button.Text = ((char)(i - 1 + 'A')).ToString();
                button.Font = new Font("Arial", 10);
                button.Size = new Size(cellSize, cellSize);
                button.BackColor = Color.DarkRed;
                this.Controls.Add(button);
                
                Button button2 = new Button();
                button2.Location = new Point(i * cellSize, (mapSize + 1) * cellSize);
                if(i != 0 && i != mapSize + 1) button2.Text = ((char)(i - 1 + 'A')).ToString();
                button2.Font = new Font("Arial", 10);
                button2.Size = new Size(cellSize, cellSize);
                button2.BackColor = Color.DarkRed;
                this.Controls.Add(button2);
            }
            this.Controls.Add(restartButton);

        }
        
        public void Restart(object sender, EventArgs e)
        {
            this.Controls.Clear();
            Init();
        }

        public void SwitchPlayer()
        {
            currentPlayer = (currentPlayer == 1 ? 2 : 1);
            ResetGame();
        }

        public Color GetPreviousButtonColor(Button previousButton)
        {
            if (((previousButton.Location.X - cellSize) / cellSize % 2 != 0 && (previousButton.Location.Y - cellSize) / cellSize % 2 == 0) || 
                ((previousButton.Location.X - cellSize) / cellSize % 2 == 0 && (previousButton.Location.Y - cellSize) / cellSize % 2 != 0))
                {
                    return Color.SaddleBrown;
                }
            return Color.Bisque;
        }
        public void PressOnFigure(object sender, EventArgs e)
        {
            if (previousButton != null)
                previousButton.BackColor = GetPreviousButtonColor(previousButton);

            pressedButton = sender as Button;

            if (map[(pressedButton.Location.Y - cellSize) / cellSize, (pressedButton.Location.X - cellSize) / cellSize] != 0 &&
                map[(pressedButton.Location.Y - cellSize) / cellSize, (pressedButton.Location.X - cellSize) / cellSize] == currentPlayer)
                {
                    CloseSteps();
                    pressedButton.BackColor = Color.Red;
                    DeactivateAllButtons();
                    pressedButton.Enabled = true;
                    countBeatSteps = 0;
                    if (pressedButton.Text == "D")
                        ShowSteps((pressedButton.Location.Y - cellSize) / cellSize, (pressedButton.Location.X - cellSize) / cellSize, true);
                    else ShowSteps((pressedButton.Location.Y - cellSize) / cellSize, (pressedButton.Location.X - cellSize) / cellSize);
                    
                    if (isMoving)
                    {
                        CloseSteps();
                        pressedButton.BackColor = GetPreviousButtonColor(pressedButton);
                        ShowPossibleSteps();
                        isMoving = false;
                    }
                    else isMoving = true;
                }
            else
            {
                if(isMoving) // сделали ход
                {
                    isContinue = false;
                    if (Math.Abs((pressedButton.Location.X - cellSize) / cellSize - (previousButton.Location.X - cellSize) / cellSize) > 1)
                    {
                        isContinue = true;
                        DeleteBeaten(pressedButton, previousButton);
                    }
                    int forSwap = map[(pressedButton.Location.Y - cellSize) / cellSize, (pressedButton.Location.X - cellSize) / cellSize];
                    map[(pressedButton.Location.Y - cellSize) / cellSize, (pressedButton.Location.X - cellSize) / cellSize] = map[(previousButton.Location.Y - cellSize) / cellSize, (previousButton.Location.X - cellSize) / cellSize];
                    map[(previousButton.Location.Y - cellSize)/ cellSize, (previousButton.Location.X - cellSize) / cellSize] = forSwap;
                    pressedButton.Image = previousButton.Image;
                    previousButton.Image = null;
                    pressedButton.Text = previousButton.Text;
                    previousButton.Text = "";
                    SwitchButtonToQueen(pressedButton);
                    countBeatSteps = 0;
                    isMoving = false;
                    CloseSteps();
                    DeactivateAllButtons();
                    if (pressedButton.Text == "D")
                        ShowSteps((pressedButton.Location.Y - cellSize) / cellSize, (pressedButton.Location.X - cellSize) / cellSize, true);
                    else ShowSteps((pressedButton.Location.Y - cellSize) / cellSize, (pressedButton.Location.X - cellSize) / cellSize);
                    if (countBeatSteps == 0 || !isContinue) //нет съедобных ходов
                    {
                        CloseSteps();
                        SwitchPlayer();
                        ShowPossibleSteps();
                        isContinue = false;
                    }
                    else if (isContinue)
                    {
                        pressedButton.BackColor = Color.Red;
                        pressedButton.Enabled = true;
                        isMoving = true;
                    }
                }
            }

            previousButton = pressedButton;
        }

        public void ShowSteps(int x, int y, bool isQueen = false)
        {
            nextSteps.Clear();
            ShowDiagonal(x, y, isQueen);
            if(countBeatSteps > 0)
                CloseAfterSteps(nextSteps);
        }
        
        
        public bool ShowPossibleSteps()
        {
            bool isQueen;
            bool isBeatStep = false;
            DeactivateAllButtons();
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == currentPlayer)
                    {
                        if (buttons[i, j].Text == "D") isQueen = true;
                        else isQueen = false;
                        if (HaveFigureToBeat(i, j, isQueen, new int[2] { 0, 0 }))
                        {
                            isBeatStep = true;
                            buttons[i, j].Enabled = true;
                        }
                    }
                }
            }
            if(!isBeatStep) ActivateAllButtons();
            return isBeatStep;
        }
        public void SwitchButtonToQueen(Button button)
        {
            if (map[(button.Location.Y - cellSize) / cellSize, (button.Location.X - cellSize) / cellSize] == 1 &&
                (button.Location.Y - cellSize) / cellSize == mapSize - 1)
            {
                button.Text = "D";
                button.Image = blackQueenFigure;
            }

            if (map[(button.Location.Y - cellSize) / cellSize, (button.Location.X - cellSize) / cellSize] == 2 &&
                (button.Location.Y - cellSize) / cellSize == 0)
            {
                button.Text = "D";
                button.Image = whiteQueenFigure;
            }
                
        }
        public void DeleteBeaten(Button endButton, Button startButton)
        {
            int count = Math.Abs((endButton.Location.Y - cellSize) / cellSize - (startButton.Location.Y - cellSize) / cellSize);
            int startIndexX = (endButton.Location.Y - cellSize) / cellSize - (startButton.Location.Y - cellSize) / cellSize;
            int startIndexY = (endButton.Location.X - cellSize) / cellSize - (startButton.Location.X - cellSize) / cellSize;
            startIndexX = startIndexX < 0 ? -1 : 1;
            startIndexY = startIndexY < 0 ? -1 : 1;
            int currentCount = 0;
            int i = (startButton.Location.Y - cellSize) / cellSize + startIndexX;
            int j = (startButton.Location.X - cellSize) / cellSize + startIndexY;
            while (currentCount < count - 1)
            {
                map[i, j] = 0;
                buttons[i, j].Image = null;
                buttons[i, j].Text = "";
                i += startIndexX;
                j += startIndexY;
                currentCount++;
            }
        }

        public void ShowDiagonal(int X, int Y, bool isQueen = true)
        {
            int j = Y + 1;
            for (int i = X - 1; i >= 0 && j < 8; i--, j++)
            {
                if (currentPlayer == 1 && !isQueen && !isContinue) break;
                if (IsInsideBoarders(i, j))
                {
                    if (!DefinePath(i, j))
                        break;
                }
                if (!isQueen)
                    break;
            }

            j = Y - 1;
            for (int i = X - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (currentPlayer == 1 && !isQueen && !isContinue) break;
                if (IsInsideBoarders(i, j))
                {
                    if (!DefinePath(i, j))
                        break;
                }

                if (!isQueen)
                    break;
            }

            j = Y - 1;
            for (int i = X + 1; i < 8 && j >= 0; i++, j--)
            {
                if (currentPlayer == 2 && !isQueen && !isContinue) break;
                if (IsInsideBoarders(i, j))
                {
                    if (!DefinePath(i, j))
                        break;
                }

                if (!isQueen)
                    break;
            }

            j = Y + 1;
            for (int i = X + 1; i < 8 && j < 8; i++, j++)
            {
                if (currentPlayer == 2 && !isQueen && !isContinue) break;
                if (IsInsideBoarders(i, j))
                {
                    if (!DefinePath(i, j))
                        break;
                }

                if (!isQueen)
                    break;
            }
        }
        
        public bool DefinePath(int i, int j)
        {
            if (map[i, j] == 0 && !isContinue)
            {
                buttons[i, j].BackColor = Color.Yellow;
                buttons[i, j].Enabled = true;
                nextSteps.Add(buttons[i, j]);
            }
            else
            {
                if(map[i, j] != currentPlayer)
                {
                    if (pressedButton.Text == "D")
                        ShowWhatToBeat(i, j, true);
                    else ShowWhatToBeat(i, j);
                }
                return false;
            }
            return true;
        }
        
        public void CloseAfterSteps(List<Button> afterSteps)
        {
            for(int i = 0; i < afterSteps.Count; i++)
            {
                afterSteps[i].BackColor = GetPreviousButtonColor(afterSteps[i]);
                afterSteps[i].Enabled = false;
            }
        }
        
        public void ShowWhatToBeat(int x, int y, bool isQueen = false) // строит следующий ход, который бьет 
        {
            int directionX = x - (pressedButton.Location.Y - cellSize) / cellSize;
            int directionY = y - (pressedButton.Location.X - cellSize) / cellSize;
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

                    buttons[i, j].BackColor = Color.Yellow;
                    buttons[i, j].Enabled = true;
                    countBeatSteps++;
                }
                else break;
                if(!isQueen) break;
                i += directionX;
                j += directionY;
            }

            if (afterBeat && stayAfterBeat.Count > 0) // есть съедобный ход, нужно закрыть все простые ходы
            {
                CloseAfterSteps(stayAfterBeat);
            }

        }
        
        
        public bool HaveFigureToBeat(int positionOfX, int positionOfY, bool isQueen, int[] direction)
        {
            bool beat = false;
            int j = positionOfY + 1;
            for (int i = positionOfX - 1; i >= 0 && j < 8; i--, j++)
            {
                if (currentPlayer == 1 && !isQueen && !isContinue) break;
                if (direction[0] == 1 && direction[1] == -1 && isQueen)break;
                if (IsInsideBoarders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer)
                    {
                        beat = true;
                        if (!IsInsideBoarders(i - 1, j + 1))
                            beat = false;
                        else if (map[i - 1, j + 1] != 0)
                            beat = false;
                        else return beat;
                    }
                }
                
                if (!isQueen)
                    break;
            }

            j = positionOfY - 1;
            for (int i = positionOfX - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (currentPlayer == 1 && !isQueen && !isContinue) break;
                if (direction[0] == 1 && direction[1] == 1 && isQueen) break;
                if (IsInsideBoarders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer)
                    {
                        beat = true;
                        if (!IsInsideBoarders(i - 1, j - 1))
                            beat = false;
                        else if (map[i - 1, j - 1] != 0)
                            beat = false;
                        else return beat;
                    }
                }
                
                if (!isQueen)
                    break;
            }

            j = positionOfY - 1;
            for (int i = positionOfX + 1; i < 8 && j >= 0; i++, j--)
            {
                if (currentPlayer == 2 && !isQueen && !isContinue) break;
                if (direction[0] == -1 && direction[1] == 1 && isQueen) break;
                if (IsInsideBoarders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer)
                    {
                        beat = true;
                        if (!IsInsideBoarders(i + 1, j - 1))
                            beat = false;
                        else if (map[i + 1, j - 1] != 0)
                            beat = false;
                        else return beat;
                    }
                }

                if (!isQueen)
                    break;
            }

            j = positionOfY + 1;
            for (int i = positionOfX + 1; i < 8 && j < 8; i++, j++)
            {
                if (currentPlayer == 2 && !isQueen && !isContinue) break;
                if (direction[0] == -1 && direction[1] == -1 && isQueen) break;
                if (IsInsideBoarders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer)
                    {
                        beat = true;
                        if (!IsInsideBoarders(i + 1, j + 1))
                            beat = false;
                        else if (map[i + 1, j + 1] != 0)
                            beat = false;
                        else return beat;
                    }
                }

                if (!isQueen)
                    break;
            }
            return beat;
        }


        public void CloseSteps() //закрываем шаги, которые были открыты для предыдущей шашки
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    buttons[i, j].BackColor = GetPreviousButtonColor(buttons[i, j]);
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
