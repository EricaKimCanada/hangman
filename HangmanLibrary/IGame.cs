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
using System.ServiceModel;  // WCF types

namespace HangmanLibrary
{
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IGame
    {

        /************************
        *      PROPERTIES       *
        ************************/
        int AttemptsLeft { [OperationContract] get; [OperationContract] set; }

        List<Player> AllPlayers { [OperationContract] get; [OperationContract] set; }

        int CurrentPlayerId { [OperationContract] get; [OperationContract] set; }

        bool IsCorrectWord { [OperationContract] get; [OperationContract] set; }

        bool IsGameStarted { [OperationContract] get; [OperationContract] set; }



        /************************
        *        METHODS        *
        ************************/
        [OperationContract(IsOneWay = true)]
        void PlayLetter(string letter);

        [OperationContract]
        WordInfo SetNewWord();

        [OperationContract]
        void ChangePlayTurn();

        [OperationContract]
        void UpdateStatus();

        [OperationContract]
        void ResetGame();



        /************************
        *        CALLBACK       *
        ************************/
        [OperationContract(IsOneWay = true)]
        void RegisterForCallbacks();

        [OperationContract(IsOneWay = true)]
        void UnregisterFromCallbacks();



    } // END IGame interface

} // END namespace