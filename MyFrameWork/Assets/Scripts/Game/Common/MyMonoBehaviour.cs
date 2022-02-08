using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFramework
{
    public class MyMonoBehaviour : MonoBehaviour
    {
        readonly List<UpdateManager.IHandle> _handleList = new List<UpdateManager.IHandle>();

        protected virtual void OnDestroy()
        {
            StopAllUpdate();
        }

        protected virtual void OnDisable()
        {
            StopAllUpdate();
        }

        protected UpdateManager.IHandle StartUpdate(IEnumerator enumerator)
        {
            UpdateManager.IHandle handle = UpdateManager.Register(enumerator);
            _handleList.Add(handle);
            return handle;
        }

        protected void StopUpdate(UpdateManager.IHandle handle)
        {
            if (_handleList.Contains(handle))
                _handleList.Remove(handle);

            UpdateManager.UnRegister(handle);
        }

        protected void StopAllUpdate()
        {
            foreach(var handle in _handleList)
            {
                UpdateManager.UnRegister(handle);
            }
            _handleList.Clear();
        }

        /// <summary>
        /// check handle state and remove
        /// </summary>
        protected void RefreshHandles()
        {
            for (int i = 0; i < _handleList.Count; ++i)
            {
                if(_handleList[i].State == UpdateManager.UpdateState.None ||
                    _handleList[i].IsValid() == false)
                {
                    _handleList.RemoveAt(i);
                    i--;
                }
            }
        }

    }
}
