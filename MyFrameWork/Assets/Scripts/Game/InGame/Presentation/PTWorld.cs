using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFramework.Presentation
{
    public class PTWorld :  IWorldMessageReceiver
    {
        public PTWorldMap PTWorldMap { get; private set; }

        public void ProcessWorldMessage(WorldMessageName name, IWorldMessage message)
        {
            switch(name)
            {
                case WorldMessageName.DataLoadingEnd:
                    PTWorldMap = new PTWorldMap(0);
                    break;
            }
        }
    }
}
