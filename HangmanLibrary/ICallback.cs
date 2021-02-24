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
using System.ServiceModel;

namespace HangmanLibrary
{
    //Defines a callback contract for the clinet to implement
    [ServiceContract]
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateGui(CallbackInfo info);


    } // END ICallback interface

} // END namespace