using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib
{
    internal enum OpCodes : byte
    {   
        /*
	     * 06 AA BB - Write BB into one-byte register AA
	     * */
        WriteOneByte = 0x06,
        
        /*
         * 07 AA - Read from one-byte register AA
         * */
        ReadOneByte = 0x07,
        
        /*
         * 08 AA BB CC - Write BB CC into two-byte register AA
         * */
        WriteTwoBytes = 0x08,
        
        /*
         * 09 AA - Read from two-byte register AA
         * */
        ReadTwoBytes = 0x09,
        
        /*
         * 0A AA 03 00 11 22 - Write 3-byte sequence (00 11 22) into 3-byte register AA
         * */
        WriteThreeBytes = 0x0A,
        
        /*
         * 0B AA 03 - Read from 3-byte register AA
         * */
        ReadThreeBytes = 0x0B,
    }
}
