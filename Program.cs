using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace projectOSConsole
{
    
    class Program
    {
        private static DateTime lastTime;
        private static TimeSpan lastTotalProcessorTime;
        private static DateTime CurentTime;
        private static TimeSpan CurentTotalProcessorTime;
        static void Main(string[] args)
        {

            StartProgram();

        }
        static void StartProgram()
        {
        Started:


            Console.WriteLine("-------------Welcome------------- ");
            Console.WriteLine("1 -  List  Processes (press any key to back here again)");
            Console.WriteLine("2 -  Kill Process ");
            Console.WriteLine("3 -  Change Priority ");
            Console.WriteLine("4 - Exit");
            Console.Write("Enter Youer Choice : ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    ListProcesses();
                    break;


                case 2:
                //label to back enter id 
                KillChoice:
                    Console.Write("Enter Process ID :  ");
                    int ID = int.Parse(Console.ReadLine());
                    //check if Kill is done or not 
                    if (KillProcess(ID))
                    {
                        Console.WriteLine("Done");
                        StartProgram();
                    }
                    // if kill not done
                    else
                    {
                        Console.WriteLine("this Process Not Found ");
                        Console.WriteLine("1- Enter diffrent ID");
                        Console.WriteLine("2- back");
                        choice = int.Parse(Console.ReadLine());
                        switch (choice)
                        {
                            case 1:
                                // back to enter id again
                                goto KillChoice;

                            case 2: goto Started;
                            // back to start again 
                            default:
                                Console.WriteLine("please Enter Correct Choice 1 or 2 ");
                                goto Started;
                        }
                    }
                    break;

                case 3:

                ProcessID:
                    Console.Write("Enter Process ID : ");
                    ID = int.Parse(Console.ReadLine());
                PriorityChoice:

                    Console.WriteLine("1- Hight");
                    Console.WriteLine("2- Normal");
                    Console.WriteLine("3- BelowNormal");
                    Console.WriteLine("4- RealTime");
                    Console.WriteLine("5- AboveNormal");
                    Console.WriteLine("6- Idle");
                    Console.WriteLine("7 - Not Change");
                    Console.Write("Enter Your choice : ");
                    int PriorityChoice = int.Parse(Console.ReadLine());

                    ProcessPriorityClass processPriorityClass = new ProcessPriorityClass();
                    // assigne Prioity user selected
                    switch (PriorityChoice)
                    {
                        case 1:
                            processPriorityClass = ProcessPriorityClass.High;
                            break;
                        case 2:
                            processPriorityClass = ProcessPriorityClass.Normal;
                            break;
                        case 3:
                            processPriorityClass = ProcessPriorityClass.BelowNormal;
                            break;

                        case 4:
                            processPriorityClass = ProcessPriorityClass.RealTime;
                            break;

                        case 5:
                            processPriorityClass = ProcessPriorityClass.AboveNormal;
                            break;
                        case 6:
                            processPriorityClass = ProcessPriorityClass.Idle;
                            break;
                        case 7:
                            // don't change priority and back to started
                            goto Started;

                        default:
                            Console.WriteLine("Sorry this choice not found ");
                            // back to select correct choice
                            goto PriorityChoice;

                    }

                    if (ChangePriority(ID, processPriorityClass))
                    {
                        Console.WriteLine("Done");
                        //come bake to start
                        goto Started;
                    }
                    else
                    {
                        Console.WriteLine("this Process Not Found ");
                        Console.WriteLine("1- Enter diffrent ID");
                        Console.WriteLine("2- back");

                        choice = int.Parse(Console.ReadLine());

                        switch (choice)
                        {
                            case 1:
                                // backe to enter id againe
                                goto ProcessID;
                            case 2:
                                StartProgram();
                                break;
                            default:
                                //come backe to start
                                Console.WriteLine("please Enter Correct Choice 1 or 2 ");
                                goto Started;
                        }
                    }
                    break;


                default:
                    return;

            }

        }
        static void ListProcesses()
        {
            try
            {
                //Get All Processes run in this Computer
               
                Console.WriteLine(" Process Name\t\tID\t\tCPU Usage");
                Process[] processeses = Process.GetProcesses();
                // Dictionary it's key is Process ID and Value is Tuple containe the lastTime and  lastTotalProcessorTime for the past use
                Dictionary<int, Tuple<DateTime, TimeSpan>> ProcessLastTime = new Dictionary<int, Tuple<DateTime, TimeSpan>>();
                // to assign all id for processes
                foreach (Process Process in processeses)
                {
                    ProcessLastTime.Add(Process.Id, new Tuple<DateTime, TimeSpan>(new DateTime(), new TimeSpan()));
                }
                // Recalculate untile User press any Key
                while (!Console.KeyAvailable)
                {

                    double CpuUsageWithoutProcessNumberZero = 0;
                    // Calculate Cpu Usage For each Process 
                    foreach (Process CurrentProcess in processeses)
                    {

                        
                        //can't access Process Number 0 For this Claculate by 100 - Total Usage  
                        if (CurrentProcess.Id == 0 )
                        {
                            Console.WriteLine(" {0} \t\t|| {1} \t || CPU usge is {2:0.0} % ", CurrentProcess.ProcessName, CurrentProcess.Id,100 -CpuUsageWithoutProcessNumberZero );
                        }
                        
                        else
                        {


                            
                            //update CurrentTime
                            CurentTime = DateTime.Now;
                            //update CurentTotalProcessorTime
                            CurentTotalProcessorTime = CurrentProcess.TotalProcessorTime;
                            // Last Time From Dicionary by ID
                            lastTime = ProcessLastTime[CurrentProcess.Id].Item1;
                            // Last lastTotalProcessorTime From Dicionary by ID
                            lastTotalProcessorTime = ProcessLastTime[CurrentProcess.Id].Item2; 
                            
                            // Claculate Cpu Usage Expression
                            double CpuUsage = (CurentTotalProcessorTime.TotalSeconds - lastTotalProcessorTime.TotalSeconds) / CurentTime.Subtract(lastTime).TotalSeconds / Environment.ProcessorCount;
                            CpuUsageWithoutProcessNumberZero += CpuUsage;
                            Console.WriteLine(" {0} \t\t\t\t|| {1} \t\t\t ||  {2:0.0} % ", CurrentProcess.ProcessName, CurrentProcess.Id, CpuUsage * 100);
                           
                            #region Update Dictionary
                            // Update Dictionary
                            lastTime = CurentTime;
                            lastTotalProcessorTime = CurentTotalProcessorTime;
                            ProcessLastTime.Remove(CurrentProcess.Id);
                            ProcessLastTime.Add(CurrentProcess.Id, new Tuple<DateTime, TimeSpan>(lastTime, lastTotalProcessorTime)); 
                            #endregion

                        }
                        
                    }
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------");
                   
                    }

                StartProgram();


                }
            
            catch (Exception)
            {
                

                ListProcesses();
            }


        }
       
        static bool KillProcess(int id) {
            try
            {
                Process ProcessKilled = Process.GetProcessById(id);
                
                ProcessKilled.Kill();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
           
        }
        static bool ChangePriority(int id  , ProcessPriorityClass processPriorityClass)
        {
            try
            { 
            Process ProcessChangePriority = Process.GetProcessById(id);
            ProcessChangePriority.PriorityClass = processPriorityClass;
            return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        
       
    }
}
