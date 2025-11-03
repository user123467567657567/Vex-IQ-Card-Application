using System;
using System.Collections.Generic;
using System.IO;

public class TokenDecoder
{
    // Device mapping
    private readonly Dictionary<char, string> devices = new()
    {
        { 'C', "Controller" },
        { 'D', "Drivetrain" },
        { 'G', "Motor Group" },
        { 'M', "Motor" },
        { 'B', "Bumper" },
        { 'X', "Distance" },
        { 'T', "TouchLED" },
        { 'Y', "Color" },
        { 'V', "Vision" },
        { 'A', "AI Vision" },
        { 'O', "Optical" },
        { 'Z', "Gyro" },
        { 'P', "Pneumatic" }
    };

    // Action mapping
    private readonly Dictionary<string, string> actions = new()
    {
        { "PON", "Pump On" },
        { "POF", "Pump Off" },
        { "EX1", "Cylinder 1 Extend" },
        { "RT1", "Cylinder 1 Retract" },
        { "EX2", "Cylinder 2 Extend" },
        { "RT2", "Cylinder 2 Retract" },
        { "EUP", "EUp Pressed" },
        { "EDW", "EDown Pressed" },
        { "FUP", "FUp Pressed" },
        { "FDW", "FDown Pressed" },
        { "LUP", "Left Up Pressed" },
        { "LDW", "Left Down Pressed" },
        { "RUP", "Right Up Pressed" },
        { "RDW", "Right Down Pressed" },
        { "FRD", "Forward" },
        { "RRS", "Reverse" },
        { "STP", "Stop" },
        { "PRS", "Pressed" },
        { "TCH", "TouchLED Pressed" }
    };

    /// <summary>
    /// Decode all lines from a file
    /// </summary>
    public List<string> DecodeFile(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var decodedLines = new List<string>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            decodedLines.Add(DecodeLine(line));
        }

        return decodedLines;
    }

    /// <summary>
    /// Decode a single token line
    /// Format: [13-bit binary time][Device Letter][Port digits][Action code]
    /// Example: 0001010101010P12EX1
    /// </summary>
    public string DecodeLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line) || line.Length < 15) return line;

        string token = line;

        // Time: first 13 bits
        string timeBinary = line.Substring(0, 13);
        int timeValue = Convert.ToInt32(timeBinary, 2);
        double timeSeconds = timeValue / 100.0;

        // Device letter
        char deviceChar = line[13];
        string device = devices.ContainsKey(deviceChar) ? devices[deviceChar] : "UNKNOWN";

        // Port: grab next 1 or 2 digits
        int portStart = 14;
        int portEnd = portStart;

        // Port can be 1 or 2 digits
        while (portEnd < line.Length && char.IsDigit(line[portEnd]))
            portEnd++;

        string port = line.Substring(portStart, portEnd - portStart);

        // Action: everything after port
        string actionCode = line.Substring(portEnd);

        string action = actions.ContainsKey(actionCode) ? actions[actionCode] : actionCode;

        return $"({token}) {timeSeconds:F2}s {device} {port} {action}";
    }
}
