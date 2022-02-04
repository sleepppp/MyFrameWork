
namespace MyFrameWork.Game
{
    public static class ItemExeption
    {
        //todo failed 타입 추가
        public enum ExeptionType : int
        {
            Succeeded = 0,

            Failed = 1,
        }

        static public bool CheckSucceeded(ExeptionType exeptionType, bool isLog = true)
        {
            if (exeptionType == ExeptionType.Succeeded) 
                return true;

            if(isLog)
                UnityEngine.Debug.LogFormat("Failed : {0}",exeptionType.ToString());

            return false;
        }

        public static bool IsSucceeded(this ExeptionType exeptionType, bool isLog = true)
        {
            return CheckSucceeded(exeptionType, isLog);
        }
    }
}