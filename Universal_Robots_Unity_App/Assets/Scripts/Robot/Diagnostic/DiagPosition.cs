using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Robot
{
    public class DiagPosition : MonoBehaviour
    {

        public TMP_Text positionText, rotationText;
        public List<TMP_Text> jointPositionsText;


        private void Update()
        {
            positionText.text = RobotPos.Current.position.ToString("F3");
            rotationText.text = RobotPos.Current.rotation.ToString("F3");

            for (int i = 0; i < 6; i++)
            {
                jointPositionsText[i].text = $"{(RobotPos.Current.jointRot[i] / 0.0174532925199):0.##}�";
            }
        }
    }
}

