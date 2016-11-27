using EyeTribe.ClientSdk;
using EyeTribe.ClientSdk.Data;
using System.Runtime.InteropServices;

namespace WorkingWithXAML.Classes
{
    public class GazePoint : IGazeListener
    {
        [DllImport("User32.dll")]private static extern bool SetCursorPos(int X, int Y);


        private double gX, gY;
        private double sumgX = 0;
        private double sumgY = 0;
        int count = 0;

        public GazePoint()
        {
            // Connect client
            GazeManager.Instance.Activate(GazeManager.ApiVersion.VERSION_1_0);

            // Register this class for events
            GazeManager.Instance.AddGazeListener(this);
            gX = System.Windows.SystemParameters.VirtualScreenWidth/2 ;
            gY = System.Windows.SystemParameters.VirtualScreenHeight/2;
            
        }

        public void OnGazeUpdate(GazeData gazeData)
        {
            //movement filtering (mean of 5 gazes)
            count++;
            if (count <= 5)
            {
                sumgX += gazeData.RawCoordinates.X;
                sumgY += gazeData.RawCoordinates.Y;
            }
            else
            {
                if (count > 5)
                {
                    count = 0;
                    gX = sumgX / 5;
                    gY = sumgY / 5;
                    sumgX = 0;
                    sumgY = 0;
                }
            }
            gX = gazeData.SmoothedCoordinates.X;
            gY = gazeData.SmoothedCoordinates.Y;
            System.Console.WriteLine(gX.ToString() + " - " + gY.ToString());
            //When connection is lost, the tracker sets its position to 0,0
            if (gX != 0 || gY != 0) 
                //Prevents unexpected behaviour when blinking or closing 
                //the eyes (Very effective!)
                SetCursorPos((int)gX, (int)gY);

            // Move point, do hit-testing, log coordinates etc.
        }

        public void OnExit()
        {
            //Thread.Sleep(5000); // simulate app lifespan (e.g. OnClose/Exit event)

            // Disconnect client
            GazeManager.Instance.Deactivate();
        }
    }
}
