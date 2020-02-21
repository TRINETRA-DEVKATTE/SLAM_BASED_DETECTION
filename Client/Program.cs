using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SynchronousSocketClient
{
    public static void StartClient()
    {
        // Data buffer for incoming data.  
        byte[] bytes = new byte[1024];

        // Connect to a remote device.  
        try
        {
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.   
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("192.168.0.102"), 11000);

            // Create a TCP/IP  socket.  
            Socket sender = new Socket(IPAddress.Parse("192.168.0.102").AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());
                while (true)
                {
                    System.Console.WriteLine("Enter your choice :");
                    Console.Write("1.Forward\n2.Backward\n3.Left\n4.Right\n5.Idle\n6.Exit\n=>");
                    //System.Console.WriteLine(typeof(Console.Read()));
                    int inp = Convert.ToInt32(Console.ReadLine());
                    if (inp != 6) 
                    {
                        byte data = Convert.ToByte(inp);
                        // Encode the data string into a byte array.  
                        byte[] msg = new byte[] { data };

                        // Send the data through the socket.  
                        int bytesSent = sender.Send(msg);

                        // Receive the response from the remote device.  
                        int bytesRec = sender.Receive(bytes);
                        Console.WriteLine(System.Text.Encoding.Default.GetString(bytes));
                    }
                    else
                    {
                        break;
                    }
                }
                // Release the socket.  
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }


    public static int Main(String[] args)
    {
        StartClient();
        return 0;
    }
}





