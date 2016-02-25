using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;

// Project Name: Robot Car
// Description: Code to wirelessly control a robot car powered by a Raspberry Pi 2
//              running Windows 10 IoT with a wireless keyboard or through the GUI
// Created By: Blake Warrington
// Contact: bwarring24@gmail.com
// Posted on: http://hackster.io
// Last Update: February 25, 2016


namespace RobotCar
{
    

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int motor1P1 = 5;
        private const int motor1P2 = 6;
        private const int motor2P1 = 13;
        private const int motor2P2 = 26;
        private GpioPin motor1Pin1;
        private GpioPin motor1Pin2;
        private GpioPin motor2Pin1;
        private GpioPin motor2Pin2;
        private GpioPinValue pinValue;
        private bool forward = false;
        private bool reverse = false;
        private bool left = false;
        private bool right = false;

        public MainPage()
        {
            this.InitializeComponent();
            InitGPIO();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // If there is an error lets null out our pins to make sure we don't get any strange outpit
            if (gpio == null)
            {
                motor1Pin1 = null;
                motor1Pin2 = null;
                motor2Pin1 = null;
                motor2Pin2 = null;
                return;
            }

            motor1Pin1 = gpio.OpenPin(motor1P1);
            motor1Pin2 = gpio.OpenPin(motor1P2);
            motor2Pin1 = gpio.OpenPin(motor2P1);
            motor2Pin2 = gpio.OpenPin(motor2P2);
            pinValue = GpioPinValue.High;

            // Set all the pins to high so no motor spins
            motor1Pin1.Write(pinValue);
            motor1Pin2.Write(pinValue);
            motor2Pin1.Write(pinValue);
            motor2Pin2.Write(pinValue);

            // Set the drivemode to output
            motor1Pin1.SetDriveMode(GpioPinDriveMode.Output);
            motor1Pin2.SetDriveMode(GpioPinDriveMode.Output);
            motor2Pin1.SetDriveMode(GpioPinDriveMode.Output);
            motor2Pin2.SetDriveMode(GpioPinDriveMode.Output);

        }

        // Turn motor 1 counter clockwise
        private void btnMotor1CCW_Click(object sender, RoutedEventArgs e)
        {
            motor1Pin1.Write(GpioPinValue.Low);
            motor1Pin2.Write(GpioPinValue.High);
            blkMotor1Status.Text = "Motor spinning counter clockwise";
        }

        // Turn motor 1 clockwise
        private void btnMotor1CW_Click(object sender, RoutedEventArgs e)
        {
            motor1Pin1.Write(GpioPinValue.High);
            motor1Pin2.Write(GpioPinValue.Low);
            blkMotor1Status.Text = "Motor spinning clockwise";
        }

        // Stop motor 1
        private void btnMotor1Stop_Click(object sender, RoutedEventArgs e)
        {
            motor1Pin1.Write(GpioPinValue.High);
            motor1Pin2.Write(GpioPinValue.High);
            blkMotor1Status.Text = "Stopped";
        }

        // Turn motor 2 counter clockwise
        private void btnMotor2CCW_Click(object sender, RoutedEventArgs e)
        {
            motor2Pin1.Write(GpioPinValue.Low);
            motor2Pin2.Write(GpioPinValue.High);
            blkMotor2Status.Text = "Motor spinning counter clockwise";
        }

        // Turn motor 2 clockwise
        private void btnMotor2CW_Click(object sender, RoutedEventArgs e)
        {
            motor2Pin1.Write(GpioPinValue.High);
            motor2Pin2.Write(GpioPinValue.Low);
            blkMotor2Status.Text = "Motor spinning clockwise";
        }

        // Stop motor 2
        private void btnMotor2Stop_Click(object sender, RoutedEventArgs e)
        {
            motor2Pin1.Write(GpioPinValue.High);
            motor2Pin2.Write(GpioPinValue.High);
            blkMotor2Status.Text = "Stopped";
        }

        // Capture keyboard event key down
        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            blkMotor1Status.Text = "Key Pressed";

            // If the key is the up arrow go forward
            if(e.Key == Windows.System.VirtualKey.Up)
            {
                goForward();
                forward = true;
            }
            // If the key is the down arrow go reverse
            else if(e.Key == Windows.System.VirtualKey.Down)
            {
                goBack();
                reverse = true;
            }
            // If the key is the left arrow turn left
            else if(e.Key == Windows.System.VirtualKey.Left)
            {
                goLeft();
                left = true;
            }
            // If the key is the right arrow turn right
            else if(e.Key == Windows.System.VirtualKey.Right)
            {
                goRight();
                right = true;
            }
        }

        private void goForward()
        {
            motor1Pin1.Write(GpioPinValue.High);
            motor1Pin2.Write(GpioPinValue.Low);
            motor2Pin1.Write(GpioPinValue.Low);
            motor2Pin2.Write(GpioPinValue.High);
        }

        private void goBack()
        {
            motor1Pin1.Write(GpioPinValue.Low);
            motor1Pin2.Write(GpioPinValue.High);
            motor2Pin1.Write(GpioPinValue.High);
            motor2Pin2.Write(GpioPinValue.Low);
        }

        private void stop()
        {
            motor1Pin1.Write(GpioPinValue.High);
            motor1Pin2.Write(GpioPinValue.High);
            motor2Pin1.Write(GpioPinValue.High);
            motor2Pin2.Write(GpioPinValue.High);

            forward = false;
            reverse = false;
            left = false;
            right = false;
        }

        private void goRight()
        {
            motor1Pin1.Write(GpioPinValue.Low);
            motor1Pin2.Write(GpioPinValue.High);
            motor2Pin1.Write(GpioPinValue.Low);
            motor2Pin2.Write(GpioPinValue.High);
        }

        private void goLeft()
        {
            motor1Pin1.Write(GpioPinValue.High);
            motor1Pin2.Write(GpioPinValue.Low);
            motor2Pin1.Write(GpioPinValue.High);
            motor2Pin2.Write(GpioPinValue.Low);
        }

        // Capture the key up event to translate if we need to stop the car
        private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
        {

            // If the up key is let up then stop the car
            if (e.Key == Windows.System.VirtualKey.Up)
            {
                stop();
            }
            // If we are going forward and try to turn left at the same time
            // then turn until release and then continue forward
            else if (e.Key == Windows.System.VirtualKey.Left && forward)
            {
                goForward();
            }
            // If we are going forward and try to turn right at the same time
            // then turn until release and then continue forward
            else if (e.Key == Windows.System.VirtualKey.Right && forward)
            {
                goForward();
            }
            // If we are going reverse and try to turn left at the same time
            // then turn until release and then continue reverse
            else if (e.Key == Windows.System.VirtualKey.Left && reverse)
            {
                goBack();
            }
            // If we are going reverse and try to turn right at the same time
            // then turn until release and then continue reverse
            else if (e.Key == Windows.System.VirtualKey.Right && reverse)
            {
                goBack();
            }
            // Any other condition just stop
            else
            {
                stop();
            }

        }
    }
}
