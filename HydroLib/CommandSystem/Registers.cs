﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib.CommandSystem
{
    internal enum Registers : byte
    {
        /*
	     * R - 1 byte 
	     * (H80 0x37, Cooling node 0x38, Lighting node 0x39, H100 0x3A, 80i 0x3B, 100i 0x3c -- 
	     * this field as well as the version are common on all C-Link devices; but the rest aren't
	     * */
        [ReadOnly(true)]
        DeviceID = 0x00,


        /*
         * R - 2 bytes
         * Firmware Version in BCD (for example 1.0.5 is 0x1005, or 0x05, 0x10 in little endianess)
         * */
        [ReadOnly(true)]
        FirmwareID = 0x01,


        /*
         * R - 8 bytes
         * Product name, zero-terminated - only present on the H80i and H100i
         * */
        [ReadOnly(true)]
        ProductName = 0x02,


        /*
         * R - 1 byte
         * Status, 0 okay, 0xFF bad
         * */
        [ReadOnly(true)]
        Status = 0x03,


        /*
         * RW - 1 byte
         * Select current LED
         * */        
        LED_SelectCurrent = 0x04,


        /*
         * R - 1 byte	 
         * Number of LEDs
         * */
        [ReadOnly(true)]
        LED_Count = 0x05,


        /*
         * RW - 1 byte
         * LED mode - 0x00 for static color, 0x40 for 2-color cycle, 0x80 for 4-color, 0xC0 for temperature mode; 
         * low nibble defines cycle speed or the temperature channel to use (0 internal sensor, 7 manual)
         * */
        LED_Mode = 0x06,


        /*
         * R - 3 bytes
         * LED current color, RGB color of the selected LED
         * */
        [ReadOnly(true)]
        LED_CurrentColor = 0x07,


        /*
         * RW - 2 bytes
         * In temperature controlled mode (0xC0) this defines the colour to use to with the below gradients
         * */
        LED_TemperatureColor = 0x08,


        /*
         * RW - 6 bytes
         * LED temperature-mode temperatures: 3 temperatures; used when cycle mode is 0xc0
         * */
        LED_TemperatureMode = 0x09,


        /*
         * RW - 9 bytes
         * LED temperature-mode colors: RGBx3 colors, corresponding to temperatures in register above
         * */
        LED_TemperatureModeColors = 0x0A,


        /*
         * RW - 12 bytes
         * LED cycle colors: RGBx4 colors (only first color used if cycle mode set to 00, first two if 4b, ignored if c0)
         * */
        LED_CycleColors = 0x0B,


        /*
         * RW - 1 byte
         * Select active temperature sensor
         * */
        TEMP_SelectActiveSensor = 0x0C,


        /*
         * R - 1 byte
         * Number of temperature sensors
         * */
        [ReadOnly(true)]
        TEMP_CountSensors = 0x0D,


        /*
         * R - 2 bytes
         * Temperature as measured by selected sensor
         * */
        [ReadOnly(true)]
        TEMP_Read = 0x0E,


        /*
         * RW - 2 bytes
         * Temperature limit (when the temperature goes over this, status is set to 0xff)
         * */
        TEMP_Limit = 0x0F,


        /*
         * RW - 1 byte
         * Select current fan; for H100i, 0-3 are the fans, 4 is pump
         * */
        FAN_Select = 0x10,


        /*
         * R - 1 byte
         * Number of fans
         * */
        [ReadOnly(true)]
        FAN_Count = 0x11,


        /*
         * RW - 1 byte
         * Fan mode; 02=fixed PWM, 04=fixed RPM, 06=default, 08=quiet, 0a=balanced, 0c=performance, 0e=custom; high bit is one when fan is detected, 
         * low bit is one when the fan is 4-pin, bits 6~4 define the temperautre channel to use in "curve" modes, 0 internal and 7 manual
         * */
        FAN_Mode = 0x12,


        /*
         * RW - 1 byte
         * Fan fixed PWM, 0-255, only used if fan mode is 02
         * */
        FAN_FixedPWM = 0x13,


        /*
         * RW - 2 bytes
         * Fan fixed RPM; when fan mode is 04, controller will target this RPM
         * */
        FAN_FixedRPM = 0x14,


        /*
         * RW - 2 bytes
         * Report external temperature to fan controller - used for controlling fans via external sensors
         * */
        FAN_ReportExtTemp = 0x15,


        /*
         * R - 2 bytes
         * Current fan RPM
         * */
        [ReadOnly(true)]
        FAN_ReadRPM = 0x16,


        /*
         * R - 2 bytes
         * Maximum RPM recorded since power-on
         * */
        [ReadOnly(true)]
        FAN_MaxRecordedRPM = 0x17,


        /*
         * RW - 2 bytes
         * Fan under speed threshold
         * */
        FAN_UnderSpeedThreshold = 0x18,


        /*
         * RW - 10 bytes
         * Fan RPM table, for custom (0e) mode: array of 5 RPMs
         * */
        FAN_RPMTable = 0x19,


        /*
         * RW - 10 bytes
         * Fan temp table, for custom (0e) mode: array of 5 temperatures
         * */
        FAN_TempTable = 0x1A
    }
}
