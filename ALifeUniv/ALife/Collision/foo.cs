using System.Collections.Generic;
using System.Linq;

namespace ALifeUni.ALife.Collision
{
    public class bar
    {

    }

    public class foo<T> where T : bar
    {
        public List<T> DoStuff(T something)
        {
            List<T> chacha = new List<T>();
            List<bar> chabchab = chacha.Cast<bar>().ToList();
            return MakeBarDoStuff(chabchab);
        }

        private List<T> MakeBarDoStuff(List<bar> bbbb)
        {
            return new List<T>();
        }
    }
}
