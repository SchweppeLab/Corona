using MSim;
using MSim.lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nova.Data;


class Blazar
{
  static int startScan = -1;
  static int endScan = -1;
  static int totalScan = -1;
  static int speed = 1;
  static string activeFile;
  static bool paused = false;
  static List<string> rawPath = new List<string>();
  static SimRunner runner = new SimRunner();

  static void Main(string[] args)
  {

    //TODO: Rather than put this in a function, create a small commandline parser class for processing
    // a command line and the various options. Then integrate this functionality into that class.
    if (args.Length == 0)
    {
      Usage();
      return;
    } else {
      for(int i=0;i<args.Length;i++)
      {
        if (args[i] == "-s")
        {
          startScan = Convert.ToInt32(args[++i]);
          if (startScan < 1)
          {
            Console.WriteLine("Invalid start scan number: must be greater than 0.");
            Usage();
            return;
          }
        }
        else if (args[i] == "-e")
        {
          endScan = Convert.ToInt32(args[++i]);
          if (endScan < 1)
          {
            Console.WriteLine("Invalid end scan number: must be greater than 0.");
            Usage();
            return;
          }
        }
        else if (args[i] == "-t")
        {
          totalScan = Convert.ToInt32(args[++i]);
          if (totalScan < 1)
          {
            Console.WriteLine("Invalid scan count: must be greater than 0.");
            Usage();
            return;
          }
        }
        else {
          if (args[i][0] == '-')
          {
            Console.WriteLine("Unknown parameter.");
            Usage();
            return;
          }
          else if (File.Exists(args[i]) && (Path.GetExtension(args[i]) == ".raw" || Path.GetExtension(args[i]) == ".mzML"))
          {
            rawPath.Add(args[i]);
          }
          else
          {
            Console.WriteLine("Unknown MS data file, or unsupported MS data file type.");
            Usage();
            return;
          }
        }
      }
    }

    if (startScan > endScan)
    {
      Console.WriteLine("Start scan number must be lower than end scan number.");
      Usage();
      return;
    }

    if(rawPath.Count == 0)
    {
      //error message for the user
      return;
    }
        
    Console.WriteLine("Starting.");
    Console.WriteLine("Bind Listeners.");
    runner.MsScanArrived += DCont_MsScanArrived;
    runner.AcquisitionStart += AcquisitionStart;
    runner.AcquisitionEnd += AcquisitionEnd;
    //runner.Acquisition.AcquisitionStreamOpening += Acquisition_AcquisitionStreamOpening;
    //runner.Acquisition.AcquisitionStreamClosing += Acquisition_AcquisitionStreamClosing;

    paused = false;
    activeFile = rawPath.First();
    rawPath.RemoveAt(0);
    runner.Run(activeFile,totalScan,startScan,endScan);
    while (KeepRunning()) ;
  }


  private static void AcquisitionEnd(RunInfo e)
  {
    Console.WriteLine("AcquisitionEnd");
    if (rawPath.Count > 0)
    {
      activeFile = rawPath.First();
      rawPath.RemoveAt(0);
      runner.Run(activeFile, totalScan, startScan, endScan);
    } 
    else
    {
      Console.WriteLine("All runs have been simulated. Press 'Q' to quit.");
    }
  }

  private static void AcquisitionStart(RunInfo e)
  {
    Console.WriteLine("AcquisitionStart");
  }


  private static void Acquisition_AcquisitionStreamClosing(object sender, EventArgs e)
    {
        Console.WriteLine("Stream closing");
    }

    //private static void Acquisition_AcquisitionStreamOpening(object sender, Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.AcquisitionOpeningEventArgs e)
    //{
    //    Console.WriteLine("Stream opening");
    //    Console.WriteLine(e.StartingInformation.First().Key);
    //}

  private static void DCont_MsScanArrived(object sender, MSimEventArgs e)
  {
    if (!e.IsNative) return; //skip any scans not in their original source format.
    RefreshScreen();
    Console.WriteLine("Scan Arrived.");
    SpectrumEx testScan = e.GetScan();
    Console.WriteLine(testScan.ScanNumber + " : RT=" + testScan.RetentionTime.ToString() + "  Data points=" + testScan.DataPoints.Length);
    Console.WriteLine("######################<<<<");
    Console.WriteLine("MS" + testScan.MsLevel.ToString() + "  IIT: " + testScan.IonInjectionTime.ToString());
    int j=testScan.Count;
    if (j > 10) j = 10;
    for(int i = 0; i < j; i++)
    {
      Console.WriteLine(testScan.DataPoints[i].Mz + "\t" + testScan.DataPoints[i].Intensity);
    }
  }

  private static void RefreshScreen()
  {
    Console.Clear();
    Console.WriteLine("Currently Reading: " + activeFile);
    Console.WriteLine("Speed: " + speed.ToString() + "x");
    Console.WriteLine("Press 'P' to pause/resume, '+' or '-' to change speed, 'Q' to quit");
  }

  private static bool KeepRunning()
  {
    var key = Console.ReadKey();
    if (key.Key == ConsoleKey.Q)
    {
      rawPath.Clear();
      runner.Stop();
      return false;
    }
    else if (key.Key == ConsoleKey.P)
    {
      if (paused)
      {
        paused = false;
        runner.Resume();
      }
      else
      {
        paused = true;
        runner.Pause();
      }
    } 
    else if(key.Key == ConsoleKey.OemPlus)
    {
      if (speed == 1) speed = 5;
      else if (speed == 5) speed = 10;
      else if (speed == 10) speed = 50;
      else if (speed == 50) speed = 100;
      runner.SetSpeed(speed); 
    }
    else if (key.Key == ConsoleKey.OemMinus)
    {
      if (speed == 100) speed = 50;
      else if (speed == 50) speed = 10;
      else if (speed == 10) speed = 5;
      else if (speed == 5) speed = 1;
      runner.SetSpeed(speed);
    }
    return true;    
  }

  private static void Usage()
  {
    Console.WriteLine("Blazar: A console-based MS simulator.");
    Console.WriteLine("Copyright 2025 Schweppe Lab, University of Washington");
    Console.WriteLine(Environment.NewLine+"Usage: Blazar [options] <MS file #1> ... <MS file #n>");
    Console.WriteLine("Options: -s <number> : Start each file from scan <number>.");
    Console.WriteLine("         -e <number> : Stop simulating each file when this scan <number> is reached.");
    Console.WriteLine("         -t <number> : Only simulate this <number> of scans in each file.");
    Console.WriteLine(Environment.NewLine + "Supported MS files are .mzML and Thermo .raw");
  }

}

