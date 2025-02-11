using System;
using UnityEngine;

namespace Robot
{
    internal class Link3 : Links
    {
        private void FixedUpdate()
        {
            if (Connection.unityState == Connection.UnityState.offline) return;
            transform.localEulerAngles = new Vector3(0, 0, (float)(RobotPos.Current.jointRot[2] * (180.0 / Math.PI)));
        }

        protected override void Rotate(float ammount)
        {
            RobotPos newPose = RobotPos.Current;
            newPose.jointRot[2] += ammount * 0.0174532925199;
            newPose.jointRot[2] = Step.ClosestStep((float)(newPose.jointRot[2] / 0.0174532925199), 5) * 0.0174532925199; //Forces Steps to be 5
            CMD.MoveJ(newPose.ToPose());
        }
    }
}

