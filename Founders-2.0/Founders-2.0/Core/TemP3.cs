using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime;
using TagLib;
using TagLib.Id3v2;


namespace CloudCoinCore
{
    class TempP3
    {
        public static bool mp3Start()
        {
            Encoding FileEncoding = Encoding.ASCII;//Define the encoding.
            TagLib.File Mp3File = null;// = TagLib.File.Create(Mp3Path); //Create TagLib file... ensures a Id3v2 header.;
            TagLib.Ape.Tag ApeTag = null;// = Methods.CheckApeTag(Mp3File); //return existing tag. create one if none.
            string[] collectCloudCoinStack = new string[3];
            //State 1:endState[0] = MP3 filename
            //State 2:endState[1] = Name of external CloudCoinStack to be inserted.
            //State 3:endState[2] = Name of the CloudCoinStack currently found in the MP3.
            //State 4:endState[3] =
            //State 5:endState[4] =
            //State 6:endState[5] =
            string[] endState = new string[6]; //Keeps the current state of each case.
            string Mp3Path = null;// = Methods.ReturnMp3FilePath(); //Save file path.
            bool menuStyle = true; // Discriptive / Standard
            bool makingChanges = true; //Keeps the session runnning.

            Methods.printWelcome();
            int userChoice = Methods.printOptions() + 1;
            Console.WriteLine("");

            while (makingChanges){
                switch(userChoice){
                    case 1: //Select .mp3 file.
                        Mp3Path = Methods.ReturnMp3FilePath(); //Save file path.
                        selectMp3();
                    break;
                    case 2://Select .stack file from Bank folder
                        selectStack();
                    break;
                    case 3://Insert the .stack file into the .mp3 file 
                        stackToMp3();
                    break;
                    case 4://Return .stack from .mp3
                        stackFromMp3();
                    break;
                    case 5://Delete .stack from .mp3 
                        deleteFromMp3();
                    break;
                    case 6://Save .mp3's current state
                        saveMp3();
                    break;
                    case 7://Quit
                        makingChanges = false;
                    break;
                    case 8://Descriptions
                        menuStyle = !menuStyle;
                    break;
                    default:
                        Console.Out.WriteLine("No matches for input!");
                    break;
                }
                Console.Clear();
                ///Switch bestween discriptive and standard menus options.
                switch(menuStyle){
                    case true:
                        Methods.consoleGap(1);
                        Methods.printStates(endState);
                        userChoice = Methods.printOptions() + 1;
                    break;
                    case false:
                        Methods.consoleGap(1);
                        Methods.printStates(endState);
                        userChoice = Methods.printHelp() + 1;  
                    break;
                }//end switch

            }//end while loop.
            Console.Out.WriteLine("Goodbye");

            void selectMp3()
            {
                if(Mp3Path != "null")
                {
                    Mp3File = TagLib.File.Create(Mp3Path); //Create TagLib file... ensures a Id3v2 header. 
                    ApeTag = Methods.CheckApeTag(Mp3File); //return existing tag. create one if none.
                    string fileName = Mp3File.Name;
                    resetEndStates(fileName, ApeTag);
                    endState[0] = "MP3 file: " + fileName + " has been selected. ";
                }// end if.
                else
                {
                    endState[0] = "No file chosen.";
                }// end else.
            }// end selectMp3

            void selectStack()//External souce to be inserted in the mp3 file.
            {
                    try
                    {
                    collectCloudCoinStack = Methods.collectBankStacks(); //Select stacks to insert. 
                        if(collectCloudCoinStack[0] != "null")
                        {
                            endState[1] = "External Stack: " + collectCloudCoinStack[0];
                        }//end if
                        else
                        {
                            endState[1] = "No stack file chose.";
                        }//end else.
                    }//end try
                    catch
                    {
                    endState[1] = ".stack error ";
                    }//end catch
                    Console.Out.WriteLine(endState[1]);
            }// end selectStack

            void stackToMp3()//the process of inserting the selected stack into the file.
            {
                    string cloudCoinStack = collectCloudCoinStack[1];
                    string stackName = collectCloudCoinStack[2];
                    Console.Out.WriteLine("Existing Stacks in the mp3 will be overwritten");
                    Console.Out.WriteLine("Enter/Return to continue, Any other key to go back.");
                    if(Console.ReadKey(true).Key == ConsoleKey.Enter)//prompt user to continue.
                    {
                        if(cloudCoinStack != null && ApeTag != null)
                        {
                            Console.Out.WriteLine("Existing Stacks in the mp3 will be overwritten");
                            ApeTag = Methods.CheckApeTag(Mp3File);
                            Methods.SetApeTagValue(ApeTag, cloudCoinStack, stackName);
                            endState[2] = ".stack was successfully inserted in " + Mp3File.Name;
                            endState[4] = "Stacks in " + Mp3File.Name + " have been added.";
                        }//end if
                        else
                        {
                            Methods.SetApeTagValue(ApeTag, cloudCoinStack, stackName);
                            endState[2] = "No saved cloud coin stack.";
                        }//end else
                        Console.Out.WriteLine(endState[2]);
                    }//end if
            }//end stackToMp3

            void stackFromMp3()//returning the stack from the mp3
            {
                    Mp3File = TagLib.File.Create(Mp3Path);
                    if(Mp3File != null)
                    {
                        string mp3CurrentCoinStack = Methods.ReturnCloudCoinStack(Mp3File);//The current stack from the mp3 gets saved.
                        if(mp3CurrentCoinStack != "null")
                        {
                            endState[3] = "A file was created:  " + mp3CurrentCoinStack;
                        }//end if.
                        else{
                            endState[3] = "Incorrect key press.";
                        }//end else.
                        Console.Out.WriteLine(endState[3]);
                    }//end if.
                    else
                    {
                        Console.Out.WriteLine("No mp3 file selected.");
                    }//end else.
            } //end stackFromMp3

            void deleteFromMp3()
            {
                    Console.Out.WriteLine("WARNING: you are about to permenantley delete any stack files found in " + Mp3File.Name);
                    Console.Out.WriteLine("Enter/Return to continue, Any other key to go back.");

                    if(Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        bool isDeleted = Methods.RemoveExistingStacks(ApeTag);
                        Console.Out.WriteLine(isDeleted);
                        if(isDeleted)
                        {
                            Mp3File.Save();
                            selectMp3();// rerun code to update states.
                            endState[4] = "Any existing stacks in " + Mp3File.Name + " have been deleted.";
                        }//end if (is Deleted)
                        else
                        {
                            endState[4] = "Stacks in " + Mp3File.Name + " have not been properly deleted.";
                        }
                    }//end if.
                    else
                    {
                    endState[4] = "Stacks in " + Mp3File.Name + " have NOT been deleted.";
                    }//end else.
                    Console.Out.WriteLine(endState[4]);
            } // end deleteFromMp3.

            void saveMp3()
            {
                    Mp3File.Save(); // Save changes.
                    endState[5] = Mp3File.Name + " has been saved with the changes made";
                    Console.Out.WriteLine(endState[5]);
            }//end saveMp3


            //Endstates are the 6 messages that are displayed to the user between actions.
            void resetEndStates(string filename, TagLib.Ape.Tag tag){
                int length = endState.Length;
                TagLib.Ape.Item StackN = tag.GetItem("StackName");

                //UPDATE endState[1]: remove current external cloudcoinstack.
                endState[1] = "No external CloudCoin stack selected.";
                for(int i = 0; i< 3; i++)
                {
                    collectCloudCoinStack[i] = "";
                }// end for

                //UPDATE endState[2]: Check for an existing CloudCoinStack in new file.
                if(StackN.Size <= 1)
                {
                 
                   endState[2] = filename + " does not include a stack.";
                }//end if
                else
                {
                    Console.Out.WriteLine(StackN.Size);
                    endState[2] = filename + " includes cloudcoin stack: " + StackN + ".";
                    
                }// end else

                endState[3] = "";
                endState[4] = "";
                endState[5] = "";

            }
            return true;
        }//end main.
    }//end TemP3
    public class Methods
    {
        public static KeyboardReader reader = new KeyboardReader();
        //Creates and saves a .txt ByteFile file, and outputs to the console.

        //Used for debugging.
                // public static void ReadBytes(string Mp3Path, Encoding FileEncoding){
                //     Console.OutputEncoding = FileEncoding; //set the console output.
                //     byte[] MyMp3 = System.IO.File.ReadAllBytes(Mp3Path);
                //     string ByteFile = Hex.Dump(MyMp3); //Store Hex the Hex data from the Mp3 file.
                //     System.IO.File.WriteAllText("./Printouts/Mp3HexPrintout.txt", ByteFile); //Create a document containing Mp3 ByteFile (debugging).
                // }

        ///Method ensures the mp3 file is encapsulated with the appropriate data.
        ///Does not alter existing meta data. 
        ///Ensures the existence of an ApeTag item with the key: CloudCoinStack.
        ///Correct Apetag has a CloudCoinStack and StackName container.
        public static TagLib.Ape.Tag CheckApeTag(TagLib.File Mp3File){
            TagLib.Ape.Tag ApeTag;
            bool hasCCS = false; 
            bool hasStackName = false;

            // Pass a true parameter to the GetTag function in order to add one if the Mp3File doesn't already have a Mp3Tag.
            // By passing a the parameter 'TagTypes.Ape' we ensure the type is of Ape.

            try{ 
                ApeTag = (TagLib.Ape.Tag)Mp3File.GetTag(TagLib.TagTypes.Ape, true); 
                hasCCS = ApeTag.HasItem("CloudCoinStack");
                hasStackName = ApeTag.HasItem("StackName");
            }
            catch(Exception e)
            {
                Console.Out.WriteLine("The process failed: {0}", e.ToString());
                ApeTag = (TagLib.Ape.Tag)Mp3File.GetTag(TagLib.TagTypes.Ape, false);
            }
            if(!hasCCS){
                TagLib.Ape.Item item = new TagLib.Ape.Item("CloudCoinStack","");
                ApeTag.SetItem(item); 
            }
            if(!hasStackName){
                TagLib.Ape.Item itemName = new TagLib.Ape.Item("StackName","");
                ApeTag.SetItem(itemName);
            }
            return ApeTag;
        }
        
        ///Stores the CloudCoin.stack file as the value tied to the CloudCoinStack key.
        public static bool SetApeTagValue(TagLib.Ape.Tag ApeTag, string MyCloudCoin, string stackName){
            // Get the APEv2 tag if it exists.
            try{
                TagLib.Ape.Item currentStacks = ApeTag.GetItem("CloudCoinStack");
                ApeTag.SetValue("CloudCoinStack", MyCloudCoin);
                ApeTag.SetValue("StackName", stackName);
                return true;
            }
            catch(Exception e)
            {
                Console.Out.WriteLine("The process failed: {0}", e.ToString());
                return false;
            }
        }

        //Searches for and removes the specified Key and Value.
        public static bool RemoveExistingStacks(TagLib.Ape.Tag ApeTag){
            try{
                ApeTag.RemoveItem("CloudCoinStack");
                ApeTag.RemoveItem("StackName");
                Console.Out.WriteLine( " stacks deleted.");
            }
            catch(Exception e)
            {
                Console.Out.WriteLine( "Error: ", e);
                return false;
            }
            return true;
        }

        //Collects the stacks saved in the mp3 file. saves them in the printouts folder.
        public static string ReturnCloudCoinStack(TagLib.File Mp3File){
            TagLib.Ape.Tag ApeTag = Methods.CheckApeTag(Mp3File);
            TagLib.Ape.Item CCS = ApeTag.GetItem("CloudCoinStack");
            TagLib.Ape.Item StackN = ApeTag.GetItem("StackName");

            if (CCS != null) {
                    string filename = StackN.ToString();
                    string message = "Press enter to extract the Cloudcoin stack from "+ filename + ".";

                    if(getEnter(message)){
                        Console.Out.WriteLine("Stack: " + filename + " has been found");
                        string CloudCoinAreaValues = CCS.ToString();
                        string path ="./Printouts/"+ filename;
                        try
                        {
                            System.IO.File.WriteAllText(path, CloudCoinAreaValues); //Create a document containing Mp3 ByteFile (debugging).
                        }
                        catch(Exception e)
                        {
                            Console.Out.WriteLine("Failed to save CloudCoin data {0}", e);
                        }
                        Console.Out.WriteLine("CCS: " + CloudCoinAreaValues);
                        return path;
                    }
                    return "null";
            }else{
                Console.Out.WriteLine("no stack in file" + CCS);
                return "no .stack in file";
            }
        }
        


        //Get the filepaths to any mp3 files in the specified folder.
        //call consolePrintList(paths to files[], argument for printing with index, note to user) 
        //call getUserInput(int max, string note) 
        //return the users choice.
        public static string ReturnMp3FilePath(){
            string message = "Mp3 files found: ";
            string note = "Select the file you wish to use.";
            try 
            {
                string[] mp3FilePaths = Directory.GetFiles("./Media", "*.mp3");
                consolePrintList(mp3FilePaths, true, message, true);
                int selection = getUserInput(mp3FilePaths.Length, note, true);
                if(selection > -1){
                    string choice = mp3FilePaths[selection];
                    return choice;
                }
                else
                {
                    return "null";
                }
            } 
            catch (Exception e) 
            {
                Console.Out.WriteLine("The process failed: {0}", e.ToString());
                return e.ToString();
            }
            
        }

        ///Get stacks from the bank, print them to the console. 
        ///Allow user to choose the stack to insert.
        ///Returns the stack to the function call.
        public static string[] collectBankStacks()
        {
            string message = "Stack files found: ";
            string[] myStack = new String[3];
            try 
            {
                string[] ccStackFilePaths = Directory.GetFiles("./Bank", "*.stack");
                string note = "Select the file you wish to use.";
                consolePrintList(ccStackFilePaths, true, message, true);
                int selection = getUserInput(ccStackFilePaths.Length, note, true);
                if(selection > -1)
                {
                    myStack[0] = ccStackFilePaths[selection]; //Choose the cloudcoin to be added to the mp3.
                    myStack[1] = System.IO.File.ReadAllText(myStack[0]);//save the cloudcoin stack data.
                    myStack[2] = System.IO.Path.GetFileName(myStack[0]);//save the stacks name.
                    return myStack;
                }//end if
                else
                {
                     myStack[0] = "null";
                     myStack[1] = "";
                     myStack[2] = "";
                    return myStack;
                }
            } 
            catch (Exception e) 
            {
                Console.Out.WriteLine("The process failed: {0}", e.ToString());
                myStack[0] = e.ToString();
                return myStack;
            }
        }

        //Method to prompt a user for input. 
        public static int getUserInput(int maxNum, string message, bool goBack)
        {     
            Console.Out.WriteLine("");
            Console.Out.WriteLine(message);
            int choice = reader.readInt(0, maxNum);
            return choice - 1;
        }

        
          //Method to prompt a user for input. 
        public static bool getEnter(string message)
        {     
            Console.Out.WriteLine("");
            Console.Out.WriteLine(message);
            if(Console.ReadKey().Key == ConsoleKey.Enter){
                return true;
            }else{
                return false;
            }
        }      

        //Methods accepts an array of strings. 
        //If indexed? indecese will be numbered 1 through selection.Length. 
        public static void consolePrintList(string[] selection, bool indexed, string message, bool goBack){
            int index = 0;
            Console.Out.WriteLine("");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Out.WriteLine(message);
            Console.Out.WriteLine("");
            foreach (string file in selection) 
            {   
                
                int fileLength = Console.WindowWidth - file.Length-1;
                
                if(indexed)
                {
                 string newFile = String.Format("{0, -25}", file);
                 string indexString = "     "+(index+1).ToString()+": ";
                 Console.Out.WriteLine("{0,-5} {1,3:N1}", indexString, newFile);
                 index++;
                }
                else
                {
                    Console.Out.WriteLine("{0, -4} {1,"+fileLength+":N1}", file, " ");
                }
                
            }//end foreach
            if(goBack)
            {
                Console.WriteLine();
                string backMsg = "Type '0' (Zero), then enter to go back.";
                Console.Out.WriteLine("{0, -4} {1,"+(Console.WindowWidth - backMsg.Length-1)+":N1}", backMsg, " ");
            }// end if

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        ///
        ///Methods to help standardise the UI.
        ///
        public static void printWelcome()
        {
            string[] welcomeMsg = new string[6];
            welcomeMsg[0] = "                     CloudCoin Founders Edition                       ";
            welcomeMsg[1] = "                        Version: June.14.2018                         ";
            welcomeMsg[2] = "            Used to store CloudCoin stacks in mp3 files.              ";
            welcomeMsg[3] = "        This Software is provided as is with all faults, defects      ";
            welcomeMsg[4] = "            and errors, and without warranty of any kind.             ";
            welcomeMsg[5] = "                  Free from the CloudCoin Consortium.                 ";
            consolePrintList(welcomeMsg, false, "", false); //1st bool true? message is indexed. 2nd bool, no goBack.
            Console.WriteLine();
        } // End print options

        public static int printOptions()//One of two possible dialogue screens the user is presented with.
        {
             string note = "Enter your selection: ";
            string[] userChoices = new string[8];
            userChoices[0] = "Select .mp3 file.                                                     "; //Option 1
            userChoices[1] = "Select .stack file from Bank folder.                                  "; //Option 2
            userChoices[2] = "Insert the .stack file into the .mp3 file.                            "; //Option 3
            userChoices[3] = "Return .stack from .mp3                                               "; //Option 4
            userChoices[4] = "Delete .stack from .mp3                                               "; //Option 5
            userChoices[5] = "Save .mp3's current state                                             "; //Option 6
            userChoices[6] = "Quit (remember to save!)                                              "; //Option 7
            userChoices[7] = "Show discriptions                                                     "; //Option 8
            consolePrintList(userChoices, true, note, false); //1st bool true? message is indexed. 2nd bool, no goBack.
            return getUserInput(8,note, false);//7? Range of inputs.
        } // End print welcome.

         public static int printHelp()//One of two possible dialogue screens the user is presented with.
        {
            // string note = "message + " {0}.", selection.Length"
            string note = "Enter your selection: ";
            string[] userChoices = new string[8];
            userChoices[0] = "Select an .mp3 file from a list of files in the 'mp3' folder.         "; //Option 1
            userChoices[1] = "Choose a  .stack file from the 'Bank' folder for storage in an mp3.   "; //Option 2
            userChoices[2] = "Insert the .stack file into the .mp3 file.                            "; //Option 3
            userChoices[3] = "Search the SAVED mp3's data for cloudcoins, then write them to a file."; //Option 4
            userChoices[4] = "DELETE YOUR CLOUDCOINS (from the .mp3)                                "; //Option 5
            userChoices[5] = "Changes made to the .mp3 file will be saved                           "; //Option 6
            userChoices[6] = "End this session, this option does not save changes to the mp3 file.  "; //Option 7
            userChoices[7] = "Standard menu                                                         "; //Option 8
            consolePrintList(userChoices, true, note, false); //1st bool true? message is indexed. 2nd bool, no goBack.
            return getUserInput(8,note, false);//7? Range of inputs.
        } // End print welcome

        public static void printStates(string[] states)
        {
            int index = 0;
            foreach(string state in states){
                index++;
                if(state != null)
                { 
                    Console.WriteLine( index + ": " + state);
                }
                
            }
        }
        public static void consoleGap(int multi){
            
            for(int i = 0; i < 5*multi; i++){
                Console.Out.WriteLine("<--------------------------------------------------------------->");
            }
            Console.Out.WriteLine("<-------------------------BREAK------------------------------->");
            Console.Out.WriteLine();
        }

    }//end Methods
}//end addToMp3

//Removed code
            // TagLib.Id3v2.Tag Mp3Tag = (TagLib.Id3v2.Tag)Mp3File.GetTag(TagTypes.Id3v2);
            // Methods.CreateAnId3Frame(Mp3File, Mp3Tag, MyCloudCoin, FileEncoding); // Create private frame.
            // Methods.ReadAFrame(Mp3File, Mp3Tag, FileEncoding); // Read contents of private frame.
