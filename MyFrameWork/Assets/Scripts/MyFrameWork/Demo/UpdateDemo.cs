using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFrameWork
{
    using Handle = UpdateManager.IHandle;
    public class UpdateDemo : MyMonoBehaviour
    {
        Handle _handle = null;
        public bool _isHandleValid = true;
        void Start()
        {
            _handle = StartUpdate(UpdateTest());
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                StopUpdate(_handle);
            }
            _isHandleValid = _handle.IsValid();
        }

        IEnumerator UpdateTest()
        {
            //while(true)
            {
                yield return new UpdateManager.WaitForSeconds(1f);
                Debug.Log("UpdateTest");
                yield return new UpdateManager.WaitForSeconds(1f);
                Debug.Log("UpdateTest");
            }
        }
    }
}