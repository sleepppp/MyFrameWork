using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyFrameWork
{
    public partial class UpdateManager
    {
        public enum UpdateState : int
        {
            None = 0,
            Update = 1,
            WaitForSeconds = 2,
        }

        public interface IHandle
        {
            public int ID { get; }
            public UpdateState State { get; }
            public void ChangeState(UpdateState state);
            public bool IsValid();
        }

        struct Handle : IHandle,IEquatable<Handle>
        {
            public static Handle Null = new Handle(0);

            public int ID { get; private set; }
            public UpdateState State { get; private set; }
            public Handle(IEnumerator enumerator)
                : this(enumerator.GetHashCode()) { }

            public Handle(int id)
            {
                ID = id;
                State = UpdateState.Update;
            }

            public void ChangeState(UpdateState state)
            {
                State = state;
            }

            public bool IsValid()
            {
                return UpdateManager.IsValid(this);
            }

            public bool Equals(Handle other) => ID == other.ID;
            public override bool Equals(object o)
            {
                if (o is Handle == false) return false;
                return this.ID == ((Handle)o).ID;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public static bool operator ==(Handle d1, Handle d2)
            {
                return d1.ID == d2.ID;
            }
            public static bool operator !=(Handle d1, Handle d2)
            {
                return d1.ID != d2.ID;
            }
        }

        public interface IUpdateBase
        {
            public IEnumerator Enumerator { get; }
            public IHandle Handle { get; }
            public void Init(IHandle handle, IEnumerator enumerator);
        }

        public interface IUpdater : IUpdateBase
        {
            public bool MoveNext();
            public object Current { get; }
        }

        public struct Updater : IUpdater
        {
            public IEnumerator Enumerator { get; private set; }
            public IHandle Handle { get; private set; }
            public object Current { get { return Enumerator.Current; } }
            public Updater(IHandle handle, IEnumerator enumerator)
            {
                Handle = handle;
                Enumerator = enumerator;
            }
            public void Init(IHandle handle, IEnumerator enumerator)
            {
                Enumerator = enumerator;
                Handle = handle;
            }

            public bool MoveNext()
            {
                return Enumerator.MoveNext();
            }
        }

        public interface IWaitUptate : IUpdateBase { }
        public interface IWaitCheckInUpdate : IWaitUptate
        {
            public bool IsFinishedWait();
        }

        public struct WaitForSeconds : IWaitCheckInUpdate
        {
            public IEnumerator Enumerator { get; private set; }
            public IHandle Handle { get; private set; }

            public readonly float WaitTime;
            public readonly float StartTime;
            public WaitForSeconds(float waitTime)
            {
                WaitTime = waitTime;
                StartTime = Time.time;
                Enumerator = null;
                Handle = null;
            }

            public void Init(IHandle handle, IEnumerator enumerator)
            {
                Enumerator = enumerator;
                Handle = handle;
            }

            public bool IsFinishedWait()
            {
                return Time.time - StartTime >= WaitTime;
            }
        }
    }
}
