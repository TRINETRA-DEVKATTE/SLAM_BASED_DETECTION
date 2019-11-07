using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Device.Gpio;
using System.Diagnostics;
namespace Blink
{
	class Program
	{
		static void Main(string[] args)
		{
			if(args.Length==0)
				ServerMain();
			else if(args[0]=="s")
				ServerMain(args[1]);
			else 
				ClientMain(args[0]);
		}
		static void ClientMain(string address)
		{
			var result = IPAddress.TryParse(address,out var ipAddress);
			if(result==false)ipAddress=IPAddress.Parse("127.0.0.1");  
			IPEndPoint remoteEP = new IPEndPoint(ipAddress,21348);
			byte idle=1,forward=2,backward=3,left=4,right=5;
			char x;
			try
			{
				while(true)
				{
					using Socket sender = new Socket(ipAddress.AddressFamily,SocketType.Stream,ProtocolType.Tcp);
					sender.Connect(remoteEP);
					Console.WriteLine($"Enter choice : Idle({idle}) Forward({forward}), Backward({backward}), left({left}), right({right})");
					Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());
					x=Console.ReadKey().KeyChar;
					Console.WriteLine();
					switch(x)
					{
						case '1':sender.Send(new byte[]{idle});break;
						case '2':sender.Send(new byte[]{forward});break;
						case '3':sender.Send(new byte[]{backward});break;
						case '4':sender.Send(new byte[]{left});break;
						case '5':sender.Send(new byte[]{right});break;
						case 'x':goto endprog;
						default:
							Console.WriteLine("Invalid key!");
							break;
					}
					sender.Shutdown(SocketShutdown.Both);
					sender.Close();
				}
				endprog:;
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
				Console.WriteLine(e.StackTrace);
			}
		}
		static void ServerMain(string address="127.0.0.1")
		{
			var result = IPAddress.TryParse(address,out var ipAddress);
			if(result==false)ipAddress=IPAddress.Parse("127.0.0.1");  
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 21348);
			Console.WriteLine($"Connection active at {ipAddress}");
			using Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
			try
			{
				byte[] data=new byte[128];
				listener.Bind(localEndPoint);
				while(true)
				{
					listener.Listen(3);
					Console.WriteLine("Awaiting response");
					Socket response=listener.Accept();
					int size=response.Receive(data);
					Console.WriteLine($"Response obtained of {size} bytes. Data :{data[0]}");
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
				Console.WriteLine(e.StackTrace);
			}
		}
		static void InOut()
		{
			Stopwatch watch = new Stopwatch();
			using var controller = new GpioController();
			int Trig = 6, Echo = 5, p0=17,p1=18,p2=27,p3=22;;//Physical : Trig:11, Echo:13
			controller.OpenPin(Trig, PinMode.Output);
			controller.OpenPin(p0, PinMode.Output);
			controller.OpenPin(p1, PinMode.Output);
			controller.OpenPin(p2, PinMode.Output);
			controller.OpenPin(p3, PinMode.Output);
			controller.OpenPin(Echo, PinMode.Input);

			Console.WriteLine("Program Started");

			while(true)
			{
				int length=(int)dist();
				length/=50;

				Console.WriteLine($"length = {length}cm");


				controller.Write(p0,PinValue.High);

				if(length>1)controller.Write(p1,PinValue.High);
				else controller.Write(p1,PinValue.Low);
				
				if(length>2)controller.Write(p2,PinValue.High);
				else controller.Write(p2,PinValue.Low);
				
				if(length>3)controller.Write(p3,PinValue.High);
				else controller.Write(p3,PinValue.Low);

			}



			double dist()
			{
				ManualResetEvent mre = new ManualResetEvent(false);
				mre.WaitOne(500);
				Stopwatch pulseLength = new Stopwatch();
				//Send pulse
				controller.Write(Trig,PinValue.High);
				mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
				controller.Write(Trig,PinValue.Low);
				//Recieve pusle
				while (controller.Read(Echo) == PinValue.Low);
				pulseLength.Start();
				while (controller.Read(Echo) == PinValue.High);
				pulseLength.Stop();
				//Calculating distance
				TimeSpan timeBetween = pulseLength.Elapsed;
				Debug.WriteLine(timeBetween.ToString());
				double distance = timeBetween.TotalSeconds * 17000;
				return distance;
			}
		}
		static void BlinkMain()
		{
			int pin = 17;
			using var controller = new GpioController();
			controller.OpenPin(pin, PinMode.Output);
			int lightTimeInMilliseconds = 100;

			int dimTimeInMilliseconds = 200;
			while (true)
			{
				Console.WriteLine($"Light for {lightTimeInMilliseconds}ms");
				controller.Write(pin, PinValue.High);
				Thread.Sleep(lightTimeInMilliseconds);
				Console.WriteLine($"Dim for {dimTimeInMilliseconds}ms");
				controller.Write(pin, PinValue.Low);
				Thread.Sleep(dimTimeInMilliseconds);
			}
		}
		static void UVMain()
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			Delay();
			watch.Stop();
			int Trig = 6, Echo = 5;//Physical : Trig:11, Echo:13
			using var controller = new GpioController();
			controller.OpenPin(Trig, PinMode.Output);
			controller.OpenPin(Echo, PinMode.Input);
			System.Console.WriteLine($"Program Started : Delay timing :{watch.Elapsed.Milliseconds}");

			while (true)
			{
				var diff = TimeDiff();
				Console.WriteLine($"Distance = {diff}cm");
			}

			double TimeDiff()
			{
				ManualResetEvent mre = new ManualResetEvent(false);
				mre.WaitOne(500);
				Stopwatch pulseLength = new Stopwatch();

				//Send pulse
				controller.Write(Trig,PinValue.High);
				mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
				controller.Write(Trig,PinValue.Low);
				//Recieve pusle
				while (controller.Read(Echo) == PinValue.Low);
				pulseLength.Start();
				while (controller.Read(Echo) == PinValue.High);
				pulseLength.Stop();
				//Calculating distance
				TimeSpan timeBetween = pulseLength.Elapsed;
				Debug.WriteLine(timeBetween.ToString());
				double distance = timeBetween.TotalSeconds * 17000;

				return distance;
			}
			static void Delay()
			{
				for (int i = 0; i < 1000; i++) ;
			}
		}
	}
}
