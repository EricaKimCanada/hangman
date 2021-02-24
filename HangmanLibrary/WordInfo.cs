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
    public class WordInfo
    {

        /*********************************
        *      PROPERTIES & METHODS      *
        **********************************/
        [DataMember]
        public string CurrentWord { get; internal set; } = "";

        [DataMember]
        public int WordLength { get; internal set; } = 0;

        [DataMember]
        public List<string> PlayedLetters { get; internal set; } = new List<string>();

        [DataMember]
        public List<string> WrongLetters { get; internal set; } = new List<string>();

        [DataMember]
        public List<string> RightLetters { get; internal set; } = new List<string>();


    } // END WordInfo class

} // END namespace