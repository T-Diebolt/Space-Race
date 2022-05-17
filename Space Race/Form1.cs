using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace Space_Race
{
    public partial class Form1 : Form
    {
        Random randGen = new Random();
        
        Rectangle player1 = new Rectangle(190, 550, 15, 20);
        Rectangle player2 = new Rectangle(395, 550, 15, 20);
        Rectangle timer = new Rectangle(295, 0, 10, 600);

        List<Rectangle> obstacles = new List<Rectangle>();
        List<int> obstacleSpeeds = new List<int>();

        int playerSpeed = 5;
        int p1Score, p2Score;
        int gameTime = 0;
        int barCount = 0;
        int randNum = 0;
        
        
        
        bool wDown, sDown, upDown, downDown;

        SolidBrush greenBrush = new SolidBrush(Color.Lime);
        SolidBrush whiteBrush = new SolidBrush(Color.White);

        string gameState = "start";

        SoundPlayer deathPlayer = new SoundPlayer(Properties.Resources._8BitDeath);
        SoundPlayer winPlayer = new SoundPlayer(Properties.Resources.Win);
        SoundPlayer pointPlayer = new SoundPlayer(Properties.Resources.Beep);

        public Form1()
        {
            InitializeComponent();
        }

        public void GameStart()
        {
            obstacles.Clear();
            player1 = new Rectangle(190, 550, 15, 20);
            player2 = new Rectangle(395, 550, 15, 20);
            timer = new Rectangle(295, 0, 10, 600);
            barCount = 0;
            gameTime = 1500;
            gameState = "running";
            gameEngine.Enabled = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.Up:
                    upDown = true;
                    break;
                case Keys.Down:
                    downDown = true;
                    break;
                case Keys.Space:
                    if(gameState == "start" || gameState == "p1End" || gameState == "p2End" || gameState == "tieEnd")
                    {
                        GameStart();
                    }
                    break;
                case Keys.Escape:
                    if(gameState == "start" || gameState == "p1End" || gameState == "p2End" || gameState == "tieEnd")
                    {
                        Application.Exit();
                    }
                    break;
            }
            
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
                case Keys.Up:
                    upDown = false;
                    break;
                case Keys.Down:
                    downDown = false;
                    break;
            }
        }

        private void gameEngine_Tick(object sender, EventArgs e)
        {
            //move player
            if (wDown == true && player1.Y > 0)
            {
                player1.Y -= playerSpeed;
            }

            if (sDown == true && player1.Y < this.Height - player1.Height)
            {
                player1.Y += playerSpeed;
            }

            if (upDown == true && player2.Y > 0)
            {
                player2.Y -= playerSpeed;
            }

            if (downDown == true && player2.Y < this.Height - player2.Height)
            {
                player2.Y += playerSpeed;
            }

            //create and move obstacles
            randNum = randGen.Next(1, 6);
            
            if(randNum == 1)
            {
                randNum = randGen.Next(10, 525);
                obstacles.Add(new Rectangle(600, randNum, 20, 4));
                obstacleSpeeds.Add(randGen.Next(3,6) * -1);
            }
            
            if(randNum == 2)
            {
                randNum = randGen.Next(10, 525);
                obstacles.Add(new Rectangle(-20, randNum, 20, 4));
                obstacleSpeeds.Add(randGen.Next(3, 6));
            }

            for (int i = 0; i < obstacles.Count(); i++)
            {
                int x = obstacles[i].X + obstacleSpeeds[i];
                obstacles[i] = new Rectangle(x, obstacles[i].Y, 20, 4);
            }

            //destroy obstacles off of screen

            for(int i = 0; i < obstacles.Count(); i++)
            {
                if(obstacles[i].X == -25 || obstacles[i].X == 605)
                {
                    obstacles.RemoveAt(i);
                    obstacleSpeeds.RemoveAt(i);
                }
            }

            //player death
            for(int i = 0; i < obstacles.Count(); i++)
            {
                if (player1.IntersectsWith(obstacles[i]))
                {
                    obstacles.RemoveAt(i);
                    obstacleSpeeds.RemoveAt(i);
                    player1 = new Rectangle(190, 550, 15, 20);
                    deathPlayer.Play();
                }
            }

            for (int i = 0; i < obstacles.Count(); i++)
            {
                if (player2.IntersectsWith(obstacles[i]))
                {
                    obstacles.RemoveAt(i);
                    obstacleSpeeds.RemoveAt(i);
                    player2 = new Rectangle(395, 550, 15, 20);
                    deathPlayer.Play();
                }
            }

            //player point
            if(player1.Y == 0)
            {
                p1Score++;
                player1 = new Rectangle(190, 550, 15, 20);
                pointPlayer.Play();
            }
            
            if (player2.Y == 0)
            {
                p2Score++;
                player2 = new Rectangle(395, 550, 15, 20);
                pointPlayer.Play();
            }

            //player win by points
            if(p1Score == 3 && p2Score == 3)
            {
                gameState = "tieEnd";
                gameEngine.Enabled = false;
                
            }
            if(p1Score == 3)
            {
                gameState = "p1End";
                gameEngine.Enabled = false;
                winPlayer.Play();
            }
            if (p2Score == 3)
            {
                gameState = "p1End";
                gameEngine.Enabled = false;
                winPlayer.Play();
            }

            //timer
            gameTime--;
            barCount++;
            if(barCount == 5)
            {
                timer.Y = timer.Y + 2;
                barCount = 0;
            }
            if(gameTime == 0)
            {
                if (p1Score == p2Score)
                {
                    gameState = "tieEnd";
                    gameEngine.Enabled = false;

                    deathPlayer.Play();
                }
                else if (p1Score > p2Score)
                {
                    gameState = "p1End";
                    gameEngine.Enabled = false;
                    winPlayer.Play();
                }
                else if (p1Score < p2Score)
                {
                    gameState = "p1End";
                    gameEngine.Enabled = false;
                    winPlayer.Play();
                }
            }
            

            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            switch (gameState)
            {
                case "start":
                    titleLabel.Text = "SPACE RACE";
                    subtitleLabel.Text = "PRESS SPACE TO PLAY\nOR ESC TO EXIT";
                    player1Label.Text = "";
                    player2Label.Text = "";

                    break;
                case "running":
                    e.Graphics.FillRectangle(greenBrush, player1);
                    e.Graphics.FillRectangle(greenBrush, player2);
                    e.Graphics.FillRectangle(greenBrush, timer);
                    
                    for(int i = 0; i < obstacles.Count; i++)
                    {
                        e.Graphics.FillRectangle(whiteBrush, obstacles[i]);
                    }

                    titleLabel.Text = "";
                    subtitleLabel.Text = "";
                    player1Label.Text = $"{p1Score}";
                    player2Label.Text = $"{p2Score}";

                    break;
                case "p1End":
                    titleLabel.Text = "PLAYER 1 WINS";
                    subtitleLabel.Text = "PRESS SPACE TO REPLAY\nOR ESC TO EXIT";
                    player1Label.Text = $"";
                    player2Label.Text = $"";
                    break;
                case "p2End":
                    titleLabel.Text = "PLAYER 2 WINS";
                    subtitleLabel.Text = "PRESS SPACE TO REPLAY\nOR ESC TO EXIT";
                    player1Label.Text = $"";
                    player2Label.Text = $"";
                    break;
                case "tieEnd":
                    titleLabel.Text = "TIE";
                    subtitleLabel.Text = "PRESS SPACE TO REPLAY\nOR ESC TO EXIT";
                    player1Label.Text = $"";
                    player2Label.Text = $"";
                    break;
            }
        }
    }
}
