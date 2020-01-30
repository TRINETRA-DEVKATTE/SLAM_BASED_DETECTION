using System.Threading;
using Azuxiren.Raspberry;
namespace Blink
{
	/// <summary>The main program class for this application </summary>
	public static class Program
	{
		/// <summary>The static device for this Application</summary>
		public static RaspberryPi3 Device;
		/// <summary>Left Motor Positive Terminal</summary>
		public static readonly byte LP=Pins.GetPin(11,PinType.Physical,PinType.BCM);//17
		/// <summary>Left Motor Negative Terminal</summary>
		public static readonly byte LN=Pins.GetPin(12,PinType.Physical,PinType.BCM);//18
		/// <summary>Right Motor Positive Terminal</summary>
		public static readonly byte RP=Pins.GetPin(13,PinType.Physical,PinType.BCM);//27
		/// <summary>Right Motor Negative Terminal</summary>
		public static readonly byte RN=Pins.GetPin(15,PinType.Physical,PinType.BCM);//22
		/// <summary>This is the Main method that begins the execution of this program</summary>
		public static void Main()
		{
			Device=new RaspberryPi3
			(
				null,new byte[]{LP,LN,RP,RN}
			);
			while(true)
			{
				Foreward();
				Thread.Sleep(2000);
				Backward();
				Thread.Sleep(2000);
				Left();
				Thread.Sleep(2000);
				Right();
				Thread.Sleep(2000);
				Idle();
				Thread.Sleep(2000);
			}
			static void Foreward()=>Set(true,false,true,false);
			static void Backward()=>Set(false,true,false,true);
			static void Left()=>Set(false,false,true,false);
			static void Right()=>Set(true,false,false,false);
			static void Idle()=>Set(false,false,false,false);
			static void Set(bool Ll, bool Lr, bool Rl, bool Rr)
			{
				Device[LP]=Ll;
				Device[LN]=Lr;
				Device[RP]=Rl;
				Device[RN]=Rr;
			}
		}
	}
}