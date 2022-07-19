using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Robot
{
    /// <summary>
    /// No Need to access this. Connection will handle it. Here are some docs on what its doing https://s3-eu-west-1.amazonaws.com/ur-support-site/16496/ClientInterfaces_Realtime.pdf
    /// </summary>
    internal static class ConnectionRecieve
    {
        public static TcpClient tcpRead;
        private static Thread thread;

        private static readonly byte[] packet = new byte[1116]; //1116
        private static byte[] data = new byte[1116];
        private static readonly byte firstPacketSize = 4;
        private static readonly byte offset = 8;
        private static readonly UInt32 totalMsgLenght = 3288596480;

        private static readonly int
            timeStep = 2; //  Communication speed: CB-Series 125 Hz (8 ms), E-Series 500 Hz (2 ms)


        public static async Task Start(string host, int port)
        {
            tcpRead = new TcpClient();
            await tcpRead.ConnectAsync(host, port);

            thread = new Thread(new ThreadStart(FetchValues));
            thread.Start();
        }

        public static void Stop()
        {
            Robot.mode = Robot.RoboModes.noController;
            Robot.safety = Robot.RoboSafety.noRobotDetected;

            tcpRead.Close();
            tcpRead.Dispose();
        }

        private static async void FetchValues()
        {
            NetworkStream stream = tcpRead.GetStream();

            try
            {
                while (Connection.unityState != Connection.UnityState.offline)
                {
                    if (stream.Read(packet, 0, data.Length) != 0)
                    {
                        data = packet;
                        double msgLength = BitConverter.ToUInt32(data, firstPacketSize - 4);

                        if (msgLength == totalMsgLenght || msgLength == 1543766016)
                        {
                            ReadRobotInfo();
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Connection.Disconnect();
                Debug.Log("Socket Exception:" + e);
            }
        }

        private static void ReadRobotInfo()
        {
            Array.Reverse(data);

            //Target Joint Positions 2 - 7


            //Joint Velocity 8 - 13
            Robot.isMoving = CheckIfMoving();


            //Actual joint posistions 32 - 37
            RobotPos.Current.jointRot[0] = BitConverter.ToDouble(data, data.Length - firstPacketSize - (32 * offset));
            RobotPos.Current.jointRot[1] = BitConverter.ToDouble(data, data.Length - firstPacketSize - (33 * offset));
            RobotPos.Current.jointRot[2] = BitConverter.ToDouble(data, data.Length - firstPacketSize - (34 * offset));
            RobotPos.Current.jointRot[3] = BitConverter.ToDouble(data, data.Length - firstPacketSize - (35 * offset));
            RobotPos.Current.jointRot[4] = BitConverter.ToDouble(data, data.Length - firstPacketSize - (36 * offset));
            RobotPos.Current.jointRot[5] = BitConverter.ToDouble(data, data.Length - firstPacketSize - (37 * offset));

            //Actual Cartesian Coord of tool 56 - 61
            RobotPos.Current.position = new Vector3(
                (float)BitConverter.ToDouble(data, data.Length - firstPacketSize - (56 * offset)),
                (float)BitConverter.ToDouble(data, data.Length - firstPacketSize - (57 * offset)),
                (float)BitConverter.ToDouble(data, data.Length - firstPacketSize - (58 * offset)));

            RobotPos.Current.rotation = new Vector3(
                (float)BitConverter.ToDouble(data, data.Length - firstPacketSize - (59 * offset)),
                (float)BitConverter.ToDouble(data, data.Length - firstPacketSize - (60 * offset)),
                (float)BitConverter.ToDouble(data, data.Length - firstPacketSize - (61 * offset)));

            //87 => Base Temp
            //88 => Shoulder Temp
            //89 => Elbow Temp
            //90 => Wrist 1 Temp
            //91 => Wrist 2 Temp
            //92 => Wrist 3 Temp

            //94 => 2 Might be control mode
            //95 => Robot Mode Off = 2, Boot = 5, On = 7
            //96 - 101 => Joint Mode

            //Modes mode 95
            Robot.mode = (Robot.RoboModes)BitConverter.ToDouble(data, data.Length - firstPacketSize - (95 * offset));

            //Saftey mode 102
            Robot.safety =
                (Robot.RoboSafety)BitConverter.ToDouble(data, data.Length - firstPacketSize - (102 * offset));

            //Digital Outputs 121 Not used on our Robot
            //Connection.digitalOutput = BitConverter.ToDouble(packet, packet.Length - firstPacketSize - (131 * offset));


            bool CheckIfMoving()
            {
                for (int i = 0; i < 6; i++)
                {
                    if (BitConverter.ToDouble(data, data.Length - firstPacketSize - ((8 + i) * offset)) == 0) continue;
                    else return true;
                }

                return false;
            }
        }
    }

    public static class Robot
    {
        public static RoboModes mode;
        public static RoboSafety safety;


        public static bool isMoving;

        //We gave it to Robot Position because it can be more than just current
        /*public static double[] jointRot = new double[6];
        public static Vector3 position, rotation;

        public static Pose ToPose() => new Pose(jointRot);*/

        public enum RoboSafety
        {
            noRobotDetected = 0,
            normal = 1,
            reduced = 2,
            protectiveStop = 3,
            recovery = 4,
            safeGuardStop = 5,
            emergencyStopEuromap67 = 6,
            emergencyStopScreen = 7,
            violation = 8,
            fault = 9,
            validateJointId = 10,
            undefinedSafetyMode = 11,
            safeguardStop = 12,
            positionEnablingStop = 13
        }

        public enum RoboModes
        {
            noController = -1,
            disconnected = 0,
            confirmSafety = 1,
            booting = 2,
            powerOff = 3,
            powerOn = 4,
            idle = 5,
            backdrive = 6,
            running = 7,
            updatingFirmware = 8
        }
    }
}