using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFrameWork.Demo
{
    using Handle = UpdateManager.IHandle;
    public class UpdateDemo : MyMonoBehaviour
    {
        Handle _handle = null;
        public bool _isHandleValid = true;
        public UpdateManager.UpdateState _handleState;
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
            _handleState = _handle.State;
        }

        IEnumerator UpdateTest()
        {
            //while(true)
            {
                yield return new UpdateManager.WaitForSeconds(3f);
                Debug.Log("UpdateTest");

                while (true)
                {
                    yield return null;
                    Debug.Log("UpdateTest");
                }
            }
        }
    }
}