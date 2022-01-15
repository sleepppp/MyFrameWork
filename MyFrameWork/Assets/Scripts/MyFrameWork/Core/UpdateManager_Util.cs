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
            WaitUntil = 3,
        }

        public interface IHandle
        {
            /// <summary>
            /// Handle UUID
            /// </summary>
            public int ID { get; }
            /// <summary>
            /// You Can Check Enumerator State by this property
            /// </summary>
            public UpdateState State { get; }
            /// <summary>
            /// Plz don't use this func in your code. It work correctly only in updateManager;
            /// </summary>
            public void ChangeState(UpdateState state);
            /// <summary>
            /// You Can check Enemerator is working by this func
            /// </summary>
            public bool IsValid();
        }

        struct Handle : IHandle,IEquatable<Handle>
        {
            public static Handle Null = new Handle(0);
            public static IHandle CreateHandle(IEnumerator enumerator)
            {
                return new Handle(enumerator);
            }

            public int ID { get; private set; }
            public UpdateState State { get; private set; }
            Handle(IEnumerator enumerator)
                : this(enumerator.GetHashCode()) { }

            Handle(int id)
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

        interface IRequestInfo
        {
            public IEnumerator Enumerator { get; }
            public IHandle Handle { get; }
            public void Init(IHandle handle, IEnumerator enumerator);
        }

        interface IUpdater : IRequestInfo
        {
            public bool MoveNext();
            public object Current { get; }
        }

        struct Updater : IUpdater
        {
            public static IUpdater CreateUpdater(IHandle handle, IEnumerator enumerator)
            {
                return new Updater(handle, enumerator);
            }

            public IEnumerator Enumerator { get; private set; }
            public IHandle Handle { get; private set; }
            public object Current { get { return Enumerator.Current; } }
            Updater(IHandle handle, IEnumerator enumerator)
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

        interface IWaitUpdate : IRequestInfo { }
        interface IWaitCheckInUpdate : IWaitUpdate
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

        public struct WaitUntil : IWaitCheckInUpdate
        {
            public IEnumerator Enumerator { get; private set; }
            public IHandle Handle { get; private set; }
            readonly Func<bool> _waitFunc;
            public WaitUntil(Func<bool> waitFunc)
            {
#if UNITY_EDITOR
                if(waitFunc == null)
                {
                    throw new System.Exception("Func<bool> is Null");
                }
#endif
                _waitFunc = waitFunc;
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
                return _waitFunc();
            }
        }

        //todo
        //public struct WaitForFixedUpdate : IWaitUpdate
        //todo
        //public struct WaitEndOfFrame : IWaitUpdate
    }
}
