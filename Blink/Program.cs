using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Azuxiren.Raspberry;

namespace Blink {
    public class SynchronousSocketClient {

        /// <summary>The static device for this Application</summary>
        public static RaspberryPi3 Device;
        /// <summary>Left Motor Positive Terminal</summary>
        public static readonly byte LP = Pins.GetPin (11, PinType.Physical, PinType.BCM); //17
        /// <summary>Left Motor Negative Terminal</summary>
        public static readonly byte LN = Pins.GetPin (12, PinType.Physical, PinType.BCM); //18
        /// <summary>Right Motor Positive Terminal</summary>
        public static readonly byte RP = Pins.GetPin (13, PinType.Physical, PinType.BCM); //27
        /// <summary>Right Motor Negative Terminal</summary>
        public static readonly byte RN = Pins.GetPin (15, PinType.Physical, PinType.BCM); //22

        public static string ServerIp = "192.168.0.102";
        public static void StartListening () {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];
            Device = new RaspberryPi3 (
                null, new byte[] { LP, LN, RP, RN }
            );
            // Establish the local endpoint for the socket.  

            IPEndPoint localEndPoint = new IPEndPoint (IPAddress.Parse (ServerIp), 11000);
            byte data = 0;
            // Create a TCP/IP socket.  
            Socket listener = new Socket (IPAddress.Parse (ServerIp).AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try {
                listener.Bind (localEndPoint);
                listener.Listen (10);

                // Start listening for connections.  
                while (true) {
                    Console.WriteLine ("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept ();
                    data = 0;

                    // An incoming connection needs to be processed.  
                    while (true) {
                        int bytesRec = handler.Receive (bytes);
                        data = bytes[0];

                        Console.WriteLine ("\nText received : {0}", data);

                        if (Convert.ToInt32 (data) == 6) break;
                        else {
                            byte[] msg = new byte[] { data };
                            // Echo the data back to the client. 
                            handler.Send (msg);
                            MoveRover (data);
                        }
                    }

                    handler.Shutdown (SocketShutdown.Both);
                    handler.Close ();
                }

            } catch (Exception e) {
                Console.WriteLine (e.ToString ());
            }

            Console.WriteLine ("\nPress ENTER to continue...");
            Console.Read ();

        }

        public static void MoveRover (int data) {
            switch (data) {
                case 1:
                    Console.WriteLine ("Moving Forward");
                    Foreward ();
                    break;
                case 2:
                    Console.WriteLine ("Moving Backward");
                    Backward ();
                    break;
                case 3:
                    Console.WriteLine ("Moving left");
                    Left ();
                    break;
                case 4:
                    Console.WriteLine ("Moving Right");
                    Right ();
                    break;
                default:
                    Console.WriteLine ("In Idle State");
                    Idle ();
                    break;
            }
        }
        static void Foreward () => Set (true, false, true, false);
        static void Backward () => Set (false, true, false, true);
        static void Left () => Set (false, false, true, false);
        static void Right () => Set (true, false, false, false);
        static void Idle () => Set (false, false, false, false);
        static void Set (bool Ll, bool Lr, bool Rl, bool Rr) {
            Device[LP] = Ll;
            Device[LN] = Lr;
            Device[RP] = Rl;
            Device[RN] = Rr;
        }
        public static int Main (String[] args) {
            StartListening ();
            return 0;
        }
    }

}