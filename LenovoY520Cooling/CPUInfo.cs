﻿using LibreHardwareMonitor;
using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LenovoY520Cooling
{
    internal class CPUInfo
    {
        private static int CPUTempSum = 0;
        private static int sampleCount = 0;
        private static LibreHardwareMonitor.Hardware.Computer? computer;

        public static void OpenComputer()
        {
            computer = new Computer()
            {
                IsCpuEnabled = true
            };
            computer.Open();
        }

        public static void CloseComputer()
        {
            if (computer != null)
                computer.Close();
        }

        public static int? GetTemp()
        {
            if (computer == null)
                return null;

            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name == "CPU Package")
                        {
                            if (sensor.Value == null)
                                return null;
                            return (int)sensor.Value;
                        }
                    }
                }
            }

            return null;
        }
        public static int? GetAVGTemp(int sampleSize) // This only returns the average cpu temp once the sample size exceeds the given argument.
        {
            int? cpuTemp = GetTemp();
            if (cpuTemp == null)
                return null;

            CPUTempSum += cpuTemp.Value;
            sampleCount++;

            if (sampleCount >= sampleSize)
            {
                int AVGTemp = CPUTempSum / sampleCount;
                CPUTempSum = 0;
                sampleCount = 0;
                return AVGTemp;
            }
            return null;
        }

    }
}
