using System;

namespace MyFrameWork.Game
{
    //public interface IClone<T> where T : class
    //{
    //    T Clone();
    //}

    [Serializable]
    public class Muid 
    {
        [NonSerialized] Guid _guid;

        public Muid()
        {
            _guid = Guid.NewGuid();
        }

        public Muid(Muid muid)
        {
            _guid = muid._guid;
        }

        public bool IsValid()
        {
            return _guid == Guid.Empty;
        }

        public static bool operator ==(Muid left, Muid right)
        {
            return object.ReferenceEquals(left, right);
        }

        public static bool operator !=(Muid left, Muid right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Muid);
        }
        public bool Equals(Muid obj)
        {
            return object.ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
    }
}