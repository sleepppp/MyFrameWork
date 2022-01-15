using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFrameWork
{
    public class MyMonoBehaviour : MonoBehaviour
    {
        readonly HashSet<UpdateManager.IHandle> _handleList = new HashSet<UpdateManager.IHandle>();

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
    }
}
