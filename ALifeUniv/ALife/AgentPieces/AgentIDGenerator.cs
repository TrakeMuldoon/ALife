using System;

namespace ALifeUni.ALife.AgentPieces
{
    internal static class AgentIDGenerator
    {
        const String PrimaryIDChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        const String ChildIDCharsSetOne = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const String ChildIDCharsSetTwo = "abcdefghijklmnopqrstuvwxyz-0123456789";

        static int primLen = PrimaryIDChars.Length;
        static int primLenSq = primLen * primLen;
        static int primaryIDNum = 0;

        internal static void Reset()
        {
            primaryIDNum = 0;
        }

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


        internal static int tierOne = ChildIDCharsSetOne.Length * ChildIDCharsSetTwo.Length;
        internal static string GetNextChildId(string id, int numChildren)
        {
            string newId = String.Empty;
            if(numChildren < ChildIDCharsSetOne.Length)
            {
                newId = ChildIDCharsSetOne[numChildren - 1].ToString();
            }
            else if(numChildren < tierOne)
            {
                int childrenCounter = numChildren - ChildIDCharsSetOne.Length;
                int valueOne = childrenCounter / ChildIDCharsSetTwo.Length;
                int valueTwo = childrenCounter % ChildIDCharsSetTwo.Length;

                newId = String.Empty + ChildIDCharsSetOne[valueOne] + ChildIDCharsSetTwo[valueTwo];
            }
            else if(numChildren < tierOne * ChildIDCharsSetTwo.Length)
            {
                int childrenCounter = numChildren - tierOne;
                int valueOne = childrenCounter / (ChildIDCharsSetTwo.Length * ChildIDCharsSetTwo.Length);
                int valueTwo = (childrenCounter / ChildIDCharsSetTwo.Length) % ChildIDCharsSetTwo.Length;
                int valueThree = childrenCounter % ChildIDCharsSetTwo.Length;

                newId = String.Empty + ChildIDCharsSetOne[valueOne] + ChildIDCharsSetTwo[valueTwo] + ChildIDCharsSetTwo[valueThree];
            }

            return id + newId;
        }
    }
}
