using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyFrameWork
{
    public partial class UpdateManager : MonoBehaviourSingleton<UpdateManager>
    {
        public static IHandle Register(IEnumerator enumerator)
        {
            return Instance.PrivateRegister(enumerator);
        }

        public static void UnRegister(IHandle handle)
        {
            Instance.PrivateUnRegister(handle);
        }

        public static bool IsValid(IHandle handle)
        {
            return Instance.PrivateIsValid(handle);
        }

        readonly HashSet<int> _handleIDList = new HashSet<int>();
        readonly List<IUpdater> _updateList = new List<IUpdater>();

        readonly List<IWaitCheckInUpdate> _waitCheckInUpdateList = new List<IWaitCheckInUpdate>();

#if UNITY_EDITOR
        [SerializeField] int _updateCount;
#endif

        private void Update()
        {
            for(int i =0; i < _waitCheckInUpdateList.Count; ++i)
            {
                if(_waitCheckInUpdateList[i].IsFinishedWait())
                {
                    if(_waitCheckInUpdateList[i].Enumerator.MoveNext() == false)
                    {
                        _waitCheckInUpdateList[i].Handle.ChangeState(UpdateState.None);
                        _handleIDList.Remove(_waitCheckInUpdateList[i].Handle.ID);
                    }
                    else
                    {
                        _waitCheckInUpdateList[i].Handle.ChangeState(UpdateState.Update);
                        _updateList.Add(new Updater(_waitCheckInUpdateList[i].Handle, _waitCheckInUpdateList[i].Enumerator));
                    }

                    _waitCheckInUpdateList.RemoveAt(i--);
                }
            }

            for(int i =0; i < _updateList.Count; ++i)
            {
                if(_updateList[i].Current != null)
                {
                    if(_updateList[i].Current is IWaitCheckInUpdate)
                    {
                        IWaitCheckInUpdate checkInUpdate = _updateList[i].Current as IWaitCheckInUpdate;
                        checkInUpdate.Init(_updateList[i].Handle, _updateList[i].Enumerator);
                        _updateList[i].Handle.ChangeState(UpdateState.WaitForSeconds);

                        _waitCheckInUpdateList.Add(checkInUpdate);
                        _updateList.RemoveAt(i--);
                    }
                }
                else if(_updateList[i].MoveNext() == false)
                {
                    _handleIDList.Remove(_updateList[i].Handle.ID);
                    _updateList.RemoveAt(i--);
                }
            }

#if UNITY_EDITOR
            _updateCount = _handleIDList.Count;
#endif
        }

        IHandle PrivateRegister(IEnumerator enumerator)
        {
            IHandle handle = new Handle(enumerator.GetHashCode());
            
            Updater updater = new Updater(handle, enumerator);
            updater.Init(handle, enumerator);

            _handleIDList.Add(handle.ID);
            _updateList.Add(updater);

            return handle;
        }

        void PrivateUnRegister(IHandle handle)
        {
            if (_handleIDList.Contains(handle.ID) == false)
                return;

            switch(handle.State)
            {
                case UpdateState.None:
                    break;
                case UpdateState.Update:
                    for(int i =0; i < _updateList.Count; ++i)
                    {
                        if(_updateList[i].Handle == handle)
                        {
                            _updateList.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                case UpdateState.WaitForSeconds:
                    for(int i =0; i < _waitCheckInUpdateList.Count; ++i)
                    {
                        if(_waitCheckInUpdateList[i].Handle == handle)
                        {
                            _waitCheckInUpdateList.RemoveAt(i);
                            break;
                        }
                    }
                    break;
            }

            _handleIDList.Remove(handle.ID);

            handle.ChangeState(UpdateState.None);
        }

        bool PrivateIsValid(IHandle handle)
        {
            return _handleIDList.Contains(handle.ID);
        }
    }
}
