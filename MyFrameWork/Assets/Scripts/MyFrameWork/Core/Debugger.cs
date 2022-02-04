
using System.Diagnostics;

namespace MyFrameWork
{
    public static class Debugger
    {
        [Conditional("UNITY_EDITOR")]
        public static void Throw(bool isThrow,string message = null)
        {
            if(isThrow)
            {
                throw new System.Exception(message);
            }
        }
    }
}