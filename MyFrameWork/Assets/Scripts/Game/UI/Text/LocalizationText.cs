using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MyFramework.UI
{
    using MyFramework.GameData;
    //고정 텍스트들은 텍스트에 해당 컴포넌트를 달아서 처리
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class LocalizationText : MonoBehaviour
    {
        [SerializeField] int _descID;
        [SerializeField] Text _text;

        public Text Text
        {
            get
            {
                if (_text == null)
                {
                    _text = GetComponent<Text>();
                }
                return _text;
            }
        }

        private void OnEnable()
        {
            Text.text = DataTableManager.TextTable.GetRecord(_descID).Value;
        }
    }
}