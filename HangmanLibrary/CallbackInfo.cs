/*
 * Coders: Yeonju Jeong, Sungok Kim, Osiris Hernandez
 * 
 * Date: Friday, April 10th, 2020
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HangmanLibrary
{
    [DataContract]
    public class CallbackInfo
    {

        /************************
        *      PROPERTIES       *
        ************************/
        [DataMember]
        public int attemptsLeft { get; private set; }

        [DataMember]
        public List<Player> allPlayers { get; private set; }

        [DataMember]
        public WordInfo wordInfo { get; private set; }

        [DataMember]
        public int currentPlayerId { get; private set; }

        [DataMember]
        public bool isGameStarted { get; private set; }




        /************************
        *      CONSTRUCTOR      *
        ************************/
        public CallbackInfo(int _attemptsLeft, List<Player> _allPlayers, WordInfo _wordInfo, int _currentPlayerId, bool _isGameStarted)
        {
            attemptsLeft = _attemptsLeft;
            allPlayers = _allPlayers;
            wordInfo = _wordInfo;
            currentPlayerId = _currentPlayerId;
            isGameStarted = _isGameStarted;
        }


    } // END CallbackInfo class

} // END namespace