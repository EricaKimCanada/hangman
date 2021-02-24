/*
 * Coders: Yeonju Jeong, Sungok Kim, Osiris Hernandez
 * 
 * Date: Friday, April 10th, 2020
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;  // WCF types

namespace HangmanLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Game : IGame
    {
        // Private attributes
        private List<string> allWords = new List<string>();
        private string newWord = "";
        private WordInfo newWordInfo = new WordInfo();
        private Player currentPlayer = null;

        // For console and object check
        private static uint objCount = 0;
        private uint objNum;

        // Collection - proxy callback
        private HashSet<ICallback> callbacks = new HashSet<ICallback>();




        /************************
        *      PROPERTIES       *
        ************************/
        public int AttemptsLeft { get; set; } = 6;

        public List<Player> AllPlayers { get; set; }

        public int CurrentPlayerId { get; set; } = 1;

        public bool IsCorrectWord { get; set; }

        public bool IsGameStarted { get; set; } = false;




        /************************
        *      CONSTRUCTOR      *
        ************************/
        public Game()
        {
            AllPlayers = new List<Player>();
            IsGameStarted = false;

            // Add all words in document
            // matching length requirements
            allWords = File.ReadAllLines("words.txt").ToList().Where(w => w.Length >= 4 && w.Length <= 8).ToList();
            newWord = GetNewWord(allWords);

            // -----
            objNum = ++objCount;
            Console.WriteLine($"creating game obj #{objNum}");

        } // END Game()




        /************************
        *        METHODS        *
        ************************/

        /*
         * Purpose: Allows client to play a word and updates the properties
         *          that will be passed over to Callback method
         */
        public void PlayLetter(string letter)
        {
            int wordPoints = 0;

            // Add to proper list
            if (newWord.Contains(letter))
            {
                foreach (var character in newWord)
                {
                    if (character.ToString() == letter)
                    {
                        wordPoints++;
                        newWordInfo.RightLetters.Add(letter);
                    }
                }
            }
            else
            {
                newWordInfo.WrongLetters.Add(letter);
                AttemptsLeft--;
            }

            newWordInfo.PlayedLetters.Add(letter);

            // Set player index
            foreach (var player in AllPlayers)
            {
                if (player.PlayerIndex == CurrentPlayerId)
                {
                    currentPlayer = player;
                }
            }

            // Add up player's points
            currentPlayer.TotalPoints += wordPoints;

            ChangePlayTurn();

            //callback: update all client
            UpdateAllClient();

            Console.WriteLine(currentPlayer.TotalPoints + " " + currentPlayer.PlayerName);
            Console.WriteLine($"play letter game obj #{objNum}");

        } // END PlayLetter()



        /*
         * Purpose: Resets game properties
         */
        public WordInfo SetNewWord()
        {
            // Set word
            newWordInfo.CurrentWord = newWord;
            newWordInfo.WordLength = newWord.Length;

            return newWordInfo;

        } // SetNewWord()



        /*
         * Purpose: Logic to determine current player 
         */
        public void ChangePlayTurn()
        {
            // Logic for 2 players
            if (AllPlayers.Count == 2)
            {
                if (CurrentPlayerId == 1) CurrentPlayerId = 2;
                else CurrentPlayerId = 1;
            }

            // Logic for 3 players
            else if (AllPlayers.Count == 3)
            {
                if (CurrentPlayerId == 1)
                {
                    CurrentPlayerId = 2;
                }
                else if (CurrentPlayerId == 2)
                {
                    CurrentPlayerId = 3;
                }
                else if (CurrentPlayerId == 3)
                {
                    CurrentPlayerId = 1;
                }
            }

        } // END ChangePlayTurn()



        /*
         * Purpose: Method to trigger client status update
         */
        public void UpdateStatus()
        {
            UpdateAllClient();

        } // END UpdateStatus()



        /*
         * Purpose: Function that resets Game object properties
         */
        public void ResetGame()
        {
            newWordInfo.PlayedLetters.Clear();
            newWordInfo.RightLetters.Clear();
            newWordInfo.WrongLetters.Clear();

            AttemptsLeft = 6;

            // Get random word
            int index = RandomNumber(0, allWords.Count);
            newWord = allWords[index];

            SetNewWord();

        } // END ResetGame()




        /************************
        *   HELPER METHODS      *
        ************************/

        /*
         * Purpose: Retrieve a random word from List<string> 
         */
        private string GetNewWord(List<string> allWords)
        {
         int index = RandomNumber(0, allWords.Count); 
            
            return allWords[index];

        } // GetNewWord()



        /*
         * Purpose: Generate a random number between two numbers
         */
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();

            return random.Next(min, max);

        } // END RandomNumber()




        /************************
        *   CALLBACK METHODS    *
        ************************/

        /*
         * Purpose: Callback function to update client status
         */
        private void UpdateAllClient()
        {
            CallbackInfo cinfo = new CallbackInfo(AttemptsLeft, AllPlayers, newWordInfo, CurrentPlayerId, IsGameStarted);

            // Pass it to client
            foreach(ICallback cb in callbacks)
            {
                cb.UpdateGui(cinfo);
            }

        } // END UpdateAllClient()



        public void RegisterForCallbacks()
        {
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();

            if (!callbacks.Contains(cb))  // If it doesn't contain a reference 
                callbacks.Add(cb);

            foreach(var c in callbacks)
                Console.WriteLine(" ----- "+ c);

        } // END RegisterForCallbacks()



        public void UnregisterFromCallbacks()
        {
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();

            if (callbacks.Contains(cb))
                callbacks.Remove(cb);

        } // END UnregisterFromCallbacks()



    } // END Game class

} // END namespace