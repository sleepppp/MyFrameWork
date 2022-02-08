using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MyFramework.UI
{
    using MyFramework.GameData;
    //���� �ؽ�Ʈ���� �ؽ�Ʈ�� �ش� ������Ʈ�� �޾Ƽ� ó��
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