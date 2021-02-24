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
    public class Player
    {
        
        /************************
        *      PROPERTIES       *
        ************************/
        [DataMember]
        public string PlayerName { get; set; }
        [DataMember]
        public int PlayerIndex { get; set; }
        [DataMember]
        public int TotalPoints { get; set; } = 0;     



        /************************
        *      CONSTRUCTOR      *
        ************************/
        public Player(string name)
        {
            PlayerName = name;
        }



    } // END Player class

} // END namespace