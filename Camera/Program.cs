using System;
using OpenCvSharp;
namespace Camera
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program Started");
			VideoCapture capture=new VideoCapture(0);
			if(capture.Grab())
			{
				var mat=capture.RetrieveMat();
				mat.SaveImage($"Test{new Random().Next(10000)}.jpg");
			}
			else
			{
				System.Console.WriteLine("Capture failed");
			}
        }
    }
}
