using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyFrameWork
{
    //GC 최적화를 위해 Unity Coroutine을 대체해서 사용합니다. 
    //UpdateManager.Instance를 직접 참조 하지말고 MyMonoBehaviour를 상속해서 StartUpdate,StopUpdate,StopAllUpdate를 사용하십시오.
    public partial class UpdateManager : MonoBehaviourSingleton<UpdateManager>
    {
        /// <summary>
        /// Rigister Func. But plz use MyMonoBehaviour.StartUpdate
        /// return IHandle is same with UnityEngine.Coroutine
        /// </summary>
        public static IHandle Register(IEnumerator enumerator)
        {
            return Instance.PrivateRegister(enumerator);
        }
        /// <summary>
        /// UnRegister Func. But plz use MyMonoBehaviour.StopUpdate
        /// </summary>
        /// <param name="handle"></param>
        public static void UnRegister(IHandle handle)
        {
            Instance.PrivateUnRegister(handle);
        }
        /// <summary>
        /// Check Handle was valid. Plz use Handle.IsValid()
        /// </summary>
        public static bool IsValid(IHandle handle)
        {
            return Instance.PrivateIsValid(handle);
        }
        /// <summary>
        /// Handle.ID is Contain here
        /// </summary>
        readonly HashSet<int> _handleIDList = new HashSet<int>();
        /// <summary>
        /// default request is contain here. update every frame
        /// </summary>
        readonly List<IUpdater> _updateList = new List<IUpdater>();
        /// <summary>
        /// WaitForSeconds && WaitUntil is Contain here. check end wait every frame
        /// </summary>
        readonly List<IWaitCheckInUpdate> _waitCheckInUpdateList = new List<IWaitCheckInUpdate>(); 

#if UNITY_EDITOR
        [SerializeField] int _updateCount;
#endif

        private void Update()
        {
            ExecuteWaitCheckInUpdate();
            ExecuteUpdater();

#if UNITY_EDITOR
            _updateCount = _handleIDList.Count;
#endif
        }

        void ExecuteWaitCheckInUpdate()
        {
            for (int i = 0; i < _waitCheckInUpdateList.Count; ++i)
            {
                if (_waitCheckInUpdateList[i].IsFinishedWait())
                {
                    if (_waitCheckInUpdateList[i].Enumerator.MoveNext() == false)
                    {
                        _waitCheckInUpdateList[i].Handle.ChangeState(UpdateState.None);
                        _handleIDList.Remove(_waitCheckInUpdateList[i].Handle.ID);
                    }
                    else
                    {
                        _waitCheckInUpdateList[i].Handle.ChangeState(UpdateState.Update);
                        _updateList.Add(Updater.CreateUpdater(_waitCheckInUpdateList[i].Handle, _waitCheckInUpdateList[i].Enumerator));
                    }

                    _waitCheckInUpdateList.RemoveAt(i--);
                }
            }
        }

        void ExecuteUpdater()
        {
            for (int i = 0; i < _updateList.Count; ++i)
            {
                if (_updateList[i].Current != null)
                {
                    if (_updateList[i].Current is WaitForSeconds)
                    {
                        IWaitCheckInUpdate checkInUpdate = _updateList[i].Current as IWaitCheckInUpdate;
                        checkInUpdate.Init(_updateList[i].Handle, _updateList[i].Enumerator);
                        _updateList[i].Handle.ChangeState(UpdateState.WaitForSeconds);

                        _waitCheckInUpdateList.Add(checkInUpdate);
                        _updateList.RemoveAt(i--);
                    }
                    else if (_updateList[i].Current is WaitUntil)
                    {
                        IWaitCheckInUpdate checkInUpdate = _updateList[i].Current as IWaitCheckInUpdate;
                        checkInUpdate.Init(_updateList[i].Handle, _updateList[i].Enumerator);
                        _updateList[i].Handle.ChangeState(UpdateState.WaitUntil);

                        _waitCheckInUpdateList.Add(checkInUpdate);
                        _updateList.RemoveAt(i--);
                    }

                }
                else if (_updateList[i].MoveNext() == false)
                {
                    _handleIDList.Remove(_updateList[i].Handle.ID);
                    _updateList.RemoveAt(i--);
                }
            }
        }

        IHandle PrivateRegister(IEnumerator enumerator)
        {
            IHandle handle = Handle.CreateHandle(enumerator);
            IUpdater updater = Updater.CreateUpdater(handle, enumerator);
            updater.Init(handle, enumerator);

            _handleIDList.Add(handle.ID);
            _updateList.Add(updater);

            return handle;
        }

        void PrivateUnRegister(IHandle handle)
        {
            if (_handleIDList.Contains(handle.ID) == false)
            {
                //is already end update
                return;
            }

            switch(handle.State)
            {
                case UpdateState.None:
                    break;
                case UpdateState.Update:
                    for(int i =0; i < _updateList.Count; ++i)
                    {
                        if(_updateList[i].Handle.ID == handle.ID)
                        {
                            _updateList.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                case UpdateState.WaitForSeconds:
                    for(int i =0; i < _waitCheckInUpdateList.Count; ++i)
                    {
                        if(_waitCheckInUpdateList[i].Handle.ID == handle.ID)
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
