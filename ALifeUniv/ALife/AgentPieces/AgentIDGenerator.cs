using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces
{
    internal static class AgentIDGenerator
    {
        const String PrimaryIDChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        const String ChildIDChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        static int primLen = PrimaryIDChars.Length;
        static int primLenSq = primLen * primLen;
        static int primaryIDNum = 0;

        internal static string GetNextAgentId()
        {
            char one = PrimaryIDChars[primaryIDNum % primLen];
            char two = PrimaryIDChars[(primaryIDNum / primLen) % primLen];
            char three = PrimaryIDChars[(primaryIDNum / primLenSq) % primLen];

            primaryIDNum++;

            if(primaryIDNum > primLen * primLenSq)
            {
                throw new Exception("Maxed Out On New IDs");
            }
            string ret = String.Empty + three + two + one;
            return ret;
        }

        internal static string GetNextChildId(string id, int numChildren)
        {
            if(numChildren > ChildIDChars.Length)
            {
                throw new Exception("Too Many Children");
            }

            return id + ChildIDChars[numChildren];
        }
    }
}
