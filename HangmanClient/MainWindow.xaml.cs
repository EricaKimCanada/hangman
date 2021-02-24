/*
 * Coders: Yeonju Jeong, Sungok Kim, Osiris Hernandez
 * 
 * Purpose: WPF client that allows 2-3 players to play a modified game of Hangman.
 *          Lives are shared by all players.  Points are cumulative.  Once a client
 *          selects who they want to be they are locked in to their choice for the
 *          duration of their session.  After how many matches players was to play
 *          the one with the most points would be the winner.
 * 
 * Date: Friday, April 10th, 2020
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using HangmanLibrary;

namespace HangmanClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public partial class MainWindow : Window, ICallback
    {

        /************************
        *      PROPERTIES       *
        ************************/

        DuplexChannelFactory<IGame> channel;
        private IGame newGame;

        private List<Player> allPlayers;
        private List<TextBox> charFields = new List<TextBox>();

        private WordInfo wordInfo;
        
        private int myPlayerIndex;




        /************************
        *      CONSTRUCTOR      *
        ************************/
        public MainWindow()
        {
            InitializeComponent();

            // Disable PlayLetter button at client start
            playLetterBtn.IsEnabled = false;

            try
            {
                NewGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        } // END MainWindow()




        /************************
        *     CLICK METHODS     *
        ************************/

        /*
         * Purpose: Allows client to select a player, initializes the clients
         *          labels and triggers and update for all clients
         */
        private void PlayerJoin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (newGame != null)
                {
                    Button btn = (Button)sender;

                    Player newPlayer = new Player(btn.Content.ToString());
                    newPlayer.PlayerIndex = newGame.AllPlayers.Count + 1;

                    // Add player to local List and copy over to Game object
                    allPlayers = newGame.AllPlayers;
                    allPlayers.Add(newPlayer);
                    newGame.AllPlayers = allPlayers;

                    // Determine player index and initialize labels for current player
                    myPlayerIndex = newGame.AllPlayers.Count;
                    playerNameLabel.Content = $"{newPlayer.PlayerName} ({myPlayerIndex})";
                    player1PointsBox.Text = newPlayer.TotalPoints.ToString();

                    // Disable player buttons for this client
                    earthBtn.IsEnabled = false;
                    windBtn.IsEnabled = false;
                    fireBtn.IsEnabled = false;

                    // Update all clients
                    newGame.UpdateStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        } // END Player1Join_Click()



        /*
         * Purpose: Checks that enough players have joined and starts a game
         */
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (newGame != null)
                {
                    // Let user know more players are needed to play
                    if (newGame.AllPlayers.Count() < 2)
                    {
                        MessageBox.Show("At least 2 players needed!");

                        return;
                    }

                    // Trigger bool
                    newGame.IsGameStarted = true;

                    // Update all clients
                    newGame.UpdateStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }     

        } // END StartGame_Click()



        /*
         * Purpose: Validates client's input and plays the letter
         */
        private void PlayLetter_Click(object sender, RoutedEventArgs e)
        {
            // Conver to lower case to ensure case insensitivity
            string letter = guessTextBox.Text.ToLower();

            // Check that player has entered something
            if (letter == "")
            {
                MessageBox.Show("Must enter a letter!");

                return;
            }

            // Check that character is a letter
            foreach (char c in letter)
            {
                if (!char.IsLetter(c))
                {
                    guessTextBox.Text = "";
                    MessageBox.Show("Must play a letter!");

                    return;
                }
            }

            // Check that it has not been played already
            if (wordInfo.PlayedLetters.Contains(letter))
            {
                guessTextBox.Text = "";
                MessageBox.Show("That letter has already been played!");

                return;
            }

            // Play letter
            try
            {     
                newGame.PlayLetter(letter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Callback method UpdateGui() runs

        } // END PlayLetter_Click()




        /************************
        *     HELPER METHODS    *
        ************************/

        /*
         * Purpose: Creates object that implements the callback contract.
         *          Initialization done in constructor.
         */
        public void NewGame()
        {
            //Create a Game object with WCF
            channel = new DuplexChannelFactory<IGame>(this, "GameEndPoint");
            newGame = channel.CreateChannel(); //this return IGame interface

            //Register to receive callbacks
            newGame.RegisterForCallbacks();

            // Set new word 
            wordInfo = newGame.SetNewWord();

            LoadCharFields();

        } // END NewGame()



        /*
         * Purpose: Sets the hangman image as per lives left
         */
        public void SetPicture(int lives)
        {
            switch (lives)
            {
                case 5:
                    hangmanImg.Source = new BitmapImage(new Uri(@"/Images/2-Head.png", UriKind.RelativeOrAbsolute));
                    break;

                case 4:
                    hangmanImg.Source = new BitmapImage(new Uri(@"/Images/3-Body.png", UriKind.RelativeOrAbsolute));
                    break;

                case 3:
                    hangmanImg.Source = new BitmapImage(new Uri(@"/Images/4-OneArm.png", UriKind.RelativeOrAbsolute));
                    break;

                case 2:
                    hangmanImg.Source = new BitmapImage(new Uri(@"/Images/5-TwoArms.png", UriKind.RelativeOrAbsolute));
                    break;

                case 1:
                    hangmanImg.Source = new BitmapImage(new Uri(@"/Images/6-OneLeg.png", UriKind.RelativeOrAbsolute));
                    break;

                case 0:
                    hangmanImg.Source = new BitmapImage(new Uri(@"/Images/7-TwoLegs.png", UriKind.RelativeOrAbsolute)); 
                    break;

                default:
                    break;
            }

        } // END SetPicture()



        /*
         * Purpose: Loads TextBox controls into a List
         *          and sets font color to White
         */
        public void LoadCharFields()
        {
            // Load textbox controls into a List
            charFields.Add(char0);
            charFields.Add(char1);
            charFields.Add(char2);
            charFields.Add(char3);
            charFields.Add(char4);
            charFields.Add(char5);
            charFields.Add(char6);
            charFields.Add(char7);

            // Set foreground to White
            foreach (var field in charFields)
            {
                field.Foreground = Brushes.White;
            }

        } // END LoadCharFields()



        /*
         * Purpose: Resets textbox properties, listbox and hangmang image
         */
        public void ResetFields()
        {
            foreach (var field in charFields)
            {
                field.Foreground = Brushes.White;
                field.Text = "";
            }

            wrongLettersListBox.ItemsSource = "";
            hangmanImg.Source = new BitmapImage(new Uri(@"/Images/1-StartingPost.png", UriKind.RelativeOrAbsolute));

        } // END ResetCharFields()




        /********************************************
        *     ICALLBACK CONTRACT IMPLEMENTATION     *
        ********************************************/

        private delegate void ClientUpdateDelegate(CallbackInfo info);

        /*
         * Purpose: Method that receives a Callback object which is used
         *          to update the status fields and properties in all clients
         */
        public void UpdateGui(CallbackInfo cinfo)
        {

            if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                // Unless game has started, disable the play letter button
                if (cinfo.isGameStarted == true)
                {
                    startBtn.IsEnabled = false;

                    // Activate the controls of client whose turn it is
                    if (myPlayerIndex == cinfo.currentPlayerId)
                    {
                        playLetterBtn.IsEnabled = true;
                    }
                    else
                    {
                        playLetterBtn.IsEnabled = false;
                    }
                }


                // Iterate through letters text fields
                for (int i = 0; i < charFields.Count; i++)
                {
                    // IF game has just started, add word
                    // letters to respective text field
                    if (newGame.AttemptsLeft == 6)
                    {
                        if (i < cinfo.wordInfo.WordLength)
                        {
                            charFields[i].Text = cinfo.wordInfo.CurrentWord[i].ToString();
                        }
                    }

                    // Disable any textbox field not being utilized
                    if (i >= cinfo.wordInfo.WordLength)
                    {
                        charFields[i].IsEnabled = false;
                    }
                    else
                    {
                        charFields[i].IsEnabled = true;
                    }

                }


                // If correct letter guessed, display it to user in Green color
                for (int i = 0; i < cinfo.wordInfo.CurrentWord.Length; i++)
                {
                    foreach (var letter in cinfo.wordInfo.RightLetters)
                    {
                        if (letter.ToString() == cinfo.wordInfo.CurrentWord[i].ToString())
                        {
                            charFields[i].Foreground = Brushes.Green;
                            charFields[i].Text = letter.ToString();
                        }
                    }
                }


                // Update client field labels for all users
                wordInfo = cinfo.wordInfo;
                wordLenLabel.Content = cinfo.wordInfo.WordLength.ToString();
                wrongLettersListBox.ItemsSource = cinfo.wordInfo.WrongLetters;
                livesTextBox.Content = cinfo.attemptsLeft.ToString();
                playerTurn.Content = "Your turn : " + cinfo.currentPlayerId;
                guessTextBox.Text = "";

                // Return focus to guess textbox
                guessTextBox.Focus();


                // Update points for player and disable btn
                // that has already been selected
                foreach (var player in cinfo.allPlayers)
                {
                    if (player.PlayerName == "Earth")
                    {
                        earthBtn.IsEnabled = false;
                        player1PointsBox.Text = player.TotalPoints.ToString();
                    }
                    else if (player.PlayerName == "Wind")
                    {
                        windBtn.IsEnabled = false;
                        player2PointsBox.Text = player.TotalPoints.ToString();
                    }
                    else if (player.PlayerName == "Fire")
                    {
                        fireBtn.IsEnabled = false;
                        player3PointsBox.Text = player.TotalPoints.ToString();
                    }
                }


                // Update hangman image according to lives left
                SetPicture(cinfo.attemptsLeft);

                // Find out how many letters have been discovered
                // to be used to detect if word solved
                int matchedLetters = 0;

                foreach (var field in charFields)
                {
                    if (field.Foreground == Brushes.Green)
                    {
                        matchedLetters++;
                    }
                }


                if (matchedLetters == cinfo.wordInfo.WordLength)
                {
                    // Reset bools
                    newGame.IsGameStarted = false;
                    startBtn.IsEnabled = true;
                    playLetterBtn.IsEnabled = false;

                    // Find winner(s) [player(s) with most points]
                    var biggestPoints = cinfo.allPlayers.Max(p => p.TotalPoints);
                    IEnumerable<Player> iterator = cinfo.allPlayers.Where(p => p.TotalPoints == biggestPoints);

                    // Create string statement
                    string winPlayers = "";
                    int count = 0;

                    foreach (Player item in iterator)
                    {
                        if (count > 0)
                        {
                            winPlayers += ", " + item.PlayerName;
                        }
                        else
                        {
                            winPlayers += item.PlayerName;
                        }

                        count++;
                    }

                    // Display winner(s) to clients
                    if (count == 1)
                    {
                        MessageBox.Show($"Word Found! {winPlayers} is winning");
                    }
                    else if (count > 1)
                    {
                        MessageBox.Show($"Word Found! {winPlayers} are tied");
                    }

                    // Reset client labels, guess box
                    livesTextBox.Content = 6;
                    wordLenLabel.Content = "0";
                    guessTextBox.Text = "";

                    ResetFields();
                    newGame.ResetGame();

                }
                // If out of lives
                else if (cinfo.attemptsLeft == 0)
                {
                    // Reset bools
                    newGame.IsGameStarted = false;
                    startBtn.IsEnabled = true;
                    playLetterBtn.IsEnabled = false;

                    // Display remaining unguessed letters in Red
                    foreach (var field in charFields)
                    {
                        if (field.Foreground != Brushes.Green)
                        {
                            field.Foreground = Brushes.Red;
                        }
                    }

                    // Find winner(s) [player(s) with most points]
                    var biggestPoints = cinfo.allPlayers.Max(p => p.TotalPoints);
                    IEnumerable<Player> iterator = cinfo.allPlayers.Where(p => p.TotalPoints == biggestPoints);

                    // Create string statement
                    string winPlayers = "";
                    int count = 0;

                    foreach (Player item in iterator)
                    {
                        if (count > 0)
                        {
                            winPlayers += ", " + item.PlayerName;
                        }
                        else
                        {
                            winPlayers += item.PlayerName;
                        }
                        
                        count++;
                    }

                    // IF no player scored any points
                    if (biggestPoints == 0)
                    {
                        MessageBox.Show($"Game over! No one scored a point!");
                    }
                    else
                    {
                        // Display winner(s) to clients
                        if (count == 1)
                        {
                            MessageBox.Show($"Game over! {winPlayers} is winning");
                        }
                        else if (count > 1)
                        {
                            MessageBox.Show($"Game over! {winPlayers} are currently tied");
                        }
                    }

                    // Reset client labels, guess box
                    livesTextBox.Content = 6;
                    wordLenLabel.Content = "0";
                    guessTextBox.Text = "";

                    ResetFields();
                    newGame.ResetGame();

                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(new ClientUpdateDelegate(UpdateGui), cinfo);
            }

        } // END UpdateGui()



        /*
         * Purpose: Unregisters client from callbacks
         */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (newGame != null)
                {
                    newGame?.UnregisterFromCallbacks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        } // END Window_Closing()



    } // END MainWindow class

} // END namespace