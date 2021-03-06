﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Founders;
using ZXing;
using ZXing.Common;
using Pdf417;
using Newtonsoft.Json;

namespace CloudCoinCore
{
    public class Exporter
    {
        /* INSTANCE VARIABLES */
        IFileSystem fileSystem;
        

        /* CONSTRUCTOR */
        public Exporter(IFileSystem fileUtils)
        {
            
            this.fileSystem = fileUtils;
        }

        public delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);
        public event StatusUpdateHandler OnUpdateStatus;

        private void UpdateStatus(string status, int percentage = 0)
        {
            // Make sure someone is listening to event
            if (OnUpdateStatus == null) return;

            ProgressEventArgs args = new ProgressEventArgs(status, percentage);
            OnUpdateStatus(this, args);
        }

        /* PUBLIC METHODS */

        public void writeQRCodeFiles(int m1, int m5, int m25, int m100, int m250, String tag)
        {
            int totalSaved = m1 + (m5 * 5) + (m25 * 25) + (m100 * 100) + (m250 * 250);// Total value of all coins
            int coinCount = m1 + m5 + m25 + m100 + m250; // Total number of coins 
            String[] coinsToDelete = new String[coinCount];
            String[] bankedFileNames = new DirectoryInfo(this.fileSystem.BankFolder).GetFiles().Select(o => o.Name).ToArray(); // list all file names with bank extension
            String[] frackedFileNames = new DirectoryInfo(this.fileSystem.FrackedFolder).GetFiles().Select(o => o.Name).ToArray(); // list all file names with bank extension
            String[] partialFileNames = new DirectoryInfo(this.fileSystem.PartialFolder).GetFiles().Select(o => o.Name).ToArray();

            var list = new List<string>();
            list.AddRange(bankedFileNames);
            list.AddRange(frackedFileNames);
            list.AddRange(partialFileNames);

            bankedFileNames = list.ToArray(); // Add the two arrays together

            String path = this.fileSystem.ExportFolder;//the word path is shorter than other stuff

            // Look at all the money files and choose the ones that are needed.
            for (int i = 0; i < bankedFileNames.Length; i++)
            {
                String bankFileName = (this.fileSystem.BankFolder + bankedFileNames[i]);
                String frackedFileName = (this.fileSystem.FrackedFolder + bankedFileNames[i]);
                String partialFileName = (this.fileSystem.PartialFolder + bankedFileNames[i]);

                // Get denominiation
                String denomination = bankedFileNames[i].Split('.')[0];
                try
                {
                    switch (denomination)
                    {
                        case "1":
                            if (m1 > 0)
                            {
                                this.qrCodeWriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m1--;
                            }
                            break;
                        case "5":
                            if (m5 > 0)
                            {

                                this.qrCodeWriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m5--;
                            }
                            break;
                        case "25":
                            if (m25 > 0)
                            {

                                this.qrCodeWriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m25--;
                            }
                            break;

                        case "100":
                            if (m100 > 0)
                            {
                                this.qrCodeWriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m100--;
                            }
                            break;

                        case "250":
                            if (m250 > 0)
                            { this.qrCodeWriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m250--; }
                            break;
                    }//end switch

                    if (m1 == 0 && m5 == 0 && m25 == 0 && m100 == 0 && m250 == 0)// end if file is needed to write jpeg
                    {
                        break;// Break if all the coins have been called for.
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Console.Out.WriteLine(ex);
                    //CoreLogger.Log(ex.ToString());
                }
                catch (IOException ioex)
                {
                    Console.Out.WriteLine(ioex);
                    //CoreLogger.Log(ioex.ToString());
                }//end catch 
            }// for each 1 note  
        }//end write all jpegs

        public void writeBarCode417CodeFiles(int m1, int m5, int m25, int m100, int m250, String tag)
        {
            int totalSaved = m1 + (m5 * 5) + (m25 * 25) + (m100 * 100) + (m250 * 250);// Total value of all coins
            int coinCount = m1 + m5 + m25 + m100 + m250; // Total number of coins 
            String[] coinsToDelete = new String[coinCount];
            String[] bankedFileNames = new DirectoryInfo(this.fileSystem.BankFolder).GetFiles().Select(o => o.Name).ToArray(); // list all file names with bank extension
            String[] frackedFileNames = new DirectoryInfo(this.fileSystem.FrackedFolder).GetFiles().Select(o => o.Name).ToArray(); // list all file names with bank extension
            String[] partialFileNames = new DirectoryInfo(this.fileSystem.PartialFolder).GetFiles().Select(o => o.Name).ToArray();

            var list = new List<string>();
            list.AddRange(bankedFileNames);
            list.AddRange(frackedFileNames);
            list.AddRange(partialFileNames);

            bankedFileNames = list.ToArray(); // Add the two arrays together

            String path = this.fileSystem.ExportFolder;//the word path is shorter than other stuff

            // Look at all the money files and choose the ones that are needed.
            for (int i = 0; i < bankedFileNames.Length; i++)
            {
                String bankFileName = (this.fileSystem.BankFolder + bankedFileNames[i]);
                String frackedFileName = (this.fileSystem.FrackedFolder + bankedFileNames[i]);
                String partialFileName = (this.fileSystem.PartialFolder + bankedFileNames[i]);

                // Get denominiation
                String denomination = bankedFileNames[i].Split('.')[0];
                try
                {
                    switch (denomination)
                    {
                        case "1":
                            if (m1 > 0)
                            {
                                this.barCode417WriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m1--;
                            }
                            break;
                        case "5":
                            if (m5 > 0)
                            {

                                this.barCode417WriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m5--;
                            }
                            break;
                        case "25":
                            if (m25 > 0)
                            {

                                this.barCode417WriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m25--;
                            }
                            break;

                        case "100":
                            if (m100 > 0)
                            {
                                this.barCode417WriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m100--;
                            }
                            break;

                        case "250":
                            if (m250 > 0)
                            { this.barCode417WriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m250--; }
                            break;
                    }//end switch

                    if (m1 == 0 && m5 == 0 && m25 == 0 && m100 == 0 && m250 == 0)// end if file is needed to write jpeg
                    {
                        break;// Break if all the coins have been called for.
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Console.Out.WriteLine(ex);
                    //CoreLogger.Log(ex.ToString());
                }
                catch (IOException ioex)
                {
                    Console.Out.WriteLine(ioex);
                    //CoreLogger.Log(ioex.ToString());
                }//end catch 
            }// for each 1 note  
        }//end write all jpegs

        public void writeJPEGFiles(int m1, int m5, int m25, int m100, int m250, String tag)
        {
            int totalSaved = m1 + (m5 * 5) + (m25 * 25) + (m100 * 100) + (m250 * 250);// Total value of all coins
            int coinCount = m1 + m5 + m25 + m100 + m250; // Total number of coins 
            String[] coinsToDelete = new String[coinCount];
            String[] bankedFileNames = new DirectoryInfo(this.fileSystem.BankFolder).GetFiles().Select(o => o.Name).ToArray(); // list all file names with bank extension
            String[] frackedFileNames = new DirectoryInfo(this.fileSystem.FrackedFolder).GetFiles().Select(o => o.Name).ToArray(); // list all file names with bank extension
            String[] partialFileNames = new DirectoryInfo(this.fileSystem.PartialFolder).GetFiles().Select(o => o.Name).ToArray();

            var list = new List<string>();
            list.AddRange(bankedFileNames);
            list.AddRange(frackedFileNames);
            list.AddRange(partialFileNames);

            bankedFileNames = list.ToArray(); // Add the two arrays together

            String path = this.fileSystem.ExportFolder;//the word path is shorter than other stuff

            // Look at all the money files and choose the ones that are needed.
            for (int i = 0; i < bankedFileNames.Length; i++)
            {
                String bankFileName = (this.fileSystem.BankFolder + bankedFileNames[i]);
                String frackedFileName = (this.fileSystem.FrackedFolder + bankedFileNames[i]);
                String partialFileName = (this.fileSystem.PartialFolder + bankedFileNames[i]);

                // Get denominiation
                String denomination = bankedFileNames[i].Split('.')[0];
                try
                {
                    switch (denomination)
                    {
                        case "1":
                            if (m1 > 0)
                            {
                                this.jpegWriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m1--;
                            }
                            break;
                        case "5":
                            if (m5 > 0)
                            {

                                this.jpegWriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m5--;
                            }
                            break;
                        case "25":
                            if (m25 > 0)
                            {

                                this.jpegWriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m25--;
                            }
                            break;

                        case "100":
                            if (m100 > 0)
                            {
                                this.jpegWriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m100--;
                            }
                            break;

                        case "250":
                            if (m250 > 0)
                            { this.jpegWriteOne(path, tag, bankFileName, frackedFileName, partialFileName); m250--; }
                            break;
                    }//end switch

                    if (m1 == 0 && m5 == 0 && m25 == 0 && m100 == 0 && m250 == 0)// end if file is needed to write jpeg
                    {
                        break;// Break if all the coins have been called for.
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Console.Out.WriteLine(ex);
                    //CoreLogger.Log(ex.ToString());
                }
                catch (IOException ioex)
                {
                    Console.Out.WriteLine(ioex);
                    //CoreLogger.Log(ioex.ToString());
                }//end catch 
            }// for each 1 note  
        }//end write all jpegs

        /* Write JSON to .stack File  */
        public bool writeJSONFile(int m1, int m5, int m25, int m100, int m250, String tag)
        {
            bool jsonExported = true;
            int totalSaved = m1 + (m5 * 5) + (m25 * 25) + (m100 * 100) + (m250 * 250);
            // Track the total coins
            int coinCount = m1 + m5 + m25 + m100 + m250;
            String[] coinsToDelete = new String[coinCount];
            String[] bankedFileNames = new DirectoryInfo(this.fileSystem.BankFolder).GetFiles().Select(o => o.Name).ToArray();//Get all names in bank folder
            String[] frackedFileNames = new DirectoryInfo(this.fileSystem.FrackedFolder).GetFiles().Select(o => o.Name).ToArray(); ;
            String[] partialFileNames = new DirectoryInfo(this.fileSystem.PartialFolder).GetFiles().Select(o => o.Name).ToArray();
            // Add the two arrays together
            var list = new List<String>();
            list.AddRange(bankedFileNames);
            list.AddRange(frackedFileNames);
            list.AddRange(partialFileNames);

            // Program will spend fracked files like perfect files
            bankedFileNames = list.ToArray();


            // Check to see the denomination by looking at the file start
            int c = 0;
            // c= counter
            String json = "{" + Environment.NewLine;
            json = json + "\t\"cloudcoin\": " + Environment.NewLine;
            json = json + "\t[" + Environment.NewLine;
            String bankFileName;
            String frackedFileName;
            String partialFileName;
            string denomination;
            Stack stack = new Stack();

            // Put all the JSON together and add header and footer
            for (int i = 0; (i < bankedFileNames.Length); i++)
            {
                denomination = bankedFileNames[i].Split('.')[0];
                bankFileName = this.fileSystem.BankFolder + bankedFileNames[i];//File name in bank folder
                frackedFileName = this.fileSystem.FrackedFolder + bankedFileNames[i];//File name in fracked folder
                partialFileName = this.fileSystem.PartialFolder + bankedFileNames[i];
                if (denomination == "1" && m1 > 0)
                {
                    if (c != 0)//This is the json seperator between each coin. It is not needed on the first coin
                    {
                        json += ",\n";
                    }

                    if (File.Exists(bankFileName)) // Is it a bank file 
                    {
                        
                        CloudCoin coinNote = fileSystem.LoadCoin(bankFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = bankFileName;
                        c++;
                    }
                    else if (File.Exists(partialFileName)) // Is it a partial file 
                    {
                        CloudCoin coinNote = fileSystem.LoadCoin(partialFileName);
                        //coinNote = fileSystem.loa
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = partialFileName;
                        c++;
                    }
                    else
                    {
                        CloudCoin coinNote = this.fileSystem.LoadCoin(frackedFileName);
                        coinNote.aoid = null;
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = frackedFileName;
                        c++;
                    }

                    m1--;
                    // Get the clean JSON of the coin
                }// end if coin is a 1

                if (denomination == "5" && m5 > 0)
                {
                    if ((c != 0))
                    {
                        json += ",\n";
                    }

                    if (File.Exists(bankFileName))
                    {
                        CloudCoin coinNote = this.fileSystem.LoadCoin(bankFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = bankFileName;
                        c++;
                    }
                    else if (File.Exists(partialFileName)) // Is it a partial file 
                    {
                        CloudCoin coinNote = fileSystem.LoadCoin(partialFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = partialFileName;
                        c++;
                    }
                    else
                    {
                        CloudCoin coinNote = this.fileSystem.LoadCoin(frackedFileName);
                        coinNote.aoid = null;
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = frackedFileName;
                        c++;
                    }

                    m5--;
                } // end if coin is a 5

                if (denomination == "25" && m25 > 0)
                {
                    if ((c != 0))
                    {
                        json += ",\n";
                    }

                    if (File.Exists(bankFileName))
                    {
                        CloudCoin coinNote = this.fileSystem.LoadCoin(bankFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = bankFileName;
                        c++;
                    }
                    else if (File.Exists(partialFileName)) // Is it a partial file 
                    {
                        CloudCoin coinNote = fileSystem.LoadCoin(partialFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = partialFileName;
                        c++;
                    }
                    else
                    {
                        CloudCoin coinNote = this.fileSystem.LoadCoin(frackedFileName);
                        coinNote.aoid = null;
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = frackedFileName;
                        c++;
                    }

                    m25--;
                }// end if coin is a 25

                if (denomination == "100" && m100 > 0)
                {
                    if ((c != 0))
                    {
                        json += ",\n";
                    }

                    if (File.Exists(bankFileName))
                    {
                        CloudCoin coinNote = this.fileSystem.LoadCoin(bankFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = bankFileName;
                        c++;
                    }
                    else if (File.Exists(partialFileName)) // Is it a partial file 
                    {
                        CloudCoin coinNote = fileSystem.LoadCoin(partialFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = partialFileName;
                        c++;
                    }
                    else
                    {
                        CloudCoin coinNote = this.fileSystem.LoadCoin(frackedFileName);
                        coinNote.aoid = null;
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = frackedFileName;
                        c++;
                    }

                    m100--;
                } // end if coin is a 100

                if (denomination == "250" && m250 > 0)
                {
                    if ((c != 0))
                    {
                        json += ",\n";
                    }

                    if (File.Exists(bankFileName))
                    {
                        CloudCoin coinNote = this.fileSystem.LoadCoin(bankFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = bankFileName;
                        c++;
                    }
                    else if (File.Exists(partialFileName)) // Is it a partial file 
                    {
                        CloudCoin coinNote = fileSystem.LoadCoin(partialFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = partialFileName;
                        c++;
                    }
                    else
                    {
                        CloudCoin coinNote = this.fileSystem.LoadCoin(frackedFileName);
                        coinNote.aoid = null;
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = frackedFileName;
                        c++;
                    }

                    m250--;
                }// end if coin is a 250

                if (m1 == 0 && m5 == 0 && m25 == 0 && m100 == 0 && m250 == 0)
                {
                    break;
                } // Break if all the coins have been called for.     
            }// end for each coin needed

            /*WRITE JSON TO FILE*/
            json = json + "\t] " + Environment.NewLine;
            json += "}";
            String filename = (this.fileSystem.ExportFolder + Path.DirectorySeparatorChar + totalSaved + ".CloudCoins." + tag + ".stack");
            if (File.Exists(filename))
            {
                // tack on a random number if a file already exists with the same tag
                Random rnd = new Random();
                int tagrand = rnd.Next(999);
                filename = (this.fileSystem.ExportFolder + Path.DirectorySeparatorChar + totalSaved + ".CloudCoins." + tag + tagrand + ".stack");
            }//end if file exists

            File.WriteAllText(filename, json);
            Console.Out.WriteLine("Writing to : ");
            //CoreLogger.Log("Writing to : " + filename);
            Console.Out.WriteLine(filename);
            /*DELETE FILES THAT HAVE BEEN EXPORTED*/
            for (int cc = 0; cc < coinsToDelete.Length; cc++)
            {
                // Console.Out.WriteLine("Deleting " + coinsToDelete[cc]);
                if (coinsToDelete[cc] != null) { File.Delete(coinsToDelete[cc]); }
            }//end for all coins to delete

            // end if write was good
            return jsonExported;
        }//end write json to file

        public bool writeJSONFile(int m1, int m5, int m25, int m100, int m250, String tag, int mode = 0, string backupDir = "")
        {
            bool jsonExported = true;
            int totalSaved = m1 + (m5 * 5) + (m25 * 25) + (m100 * 100) + (m250 * 250);
            // Track the total coins
            int coinCount = m1 + m5 + m25 + m100 + m250;
            String[] coinsToDelete = new String[coinCount];
            String[] bankedFileNames = new DirectoryInfo(this.fileSystem.BankFolder).GetFiles().Select(o => o.Name).ToArray();//Get all names in bank folder
            String[] frackedFileNames = new DirectoryInfo(this.fileSystem.FrackedFolder).GetFiles().Select(o => o.Name).ToArray(); ;
            String[] partialFileNames = new DirectoryInfo(this.fileSystem.PartialFolder).GetFiles().Select(o => o.Name).ToArray();
            // Add the two arrays together
            var list = new List<String>();
            list.AddRange(bankedFileNames);
            list.AddRange(frackedFileNames);
            list.AddRange(partialFileNames);

            // Program will spend fracked files like perfect files
            bankedFileNames = list.ToArray();


            // Check to see the denomination by looking at the file start
            int c = 0;
            // c= counter
            String json = "{" + Environment.NewLine;
            json = json + "\t\"cloudcoin\": " + Environment.NewLine;
            json = json + "\t[" + Environment.NewLine;
            String bankFileName;
            String frackedFileName;
            String partialFileName;
            string denomination;

            // Put all the JSON together and add header and footer
            for (int i = 0; (i < bankedFileNames.Length); i++)
            {
                denomination = bankedFileNames[i].Split('.')[0];
                bankFileName = this.fileSystem.BankFolder + bankedFileNames[i];//File name in bank folder
                frackedFileName = this.fileSystem.FrackedFolder + bankedFileNames[i];//File name in fracked folder
                partialFileName = this.fileSystem.PartialFolder + bankedFileNames[i];
                if (denomination == "1" && m1 > 0)
                {
                    if (c != 0)//This is the json seperator between each coin. It is not needed on the first coin
                    {
                        json += ",\n";
                    }

                    if (File.Exists(bankFileName)) // Is it a bank file 
                    {
                        CloudCoin coinNote = fileSystem.loadOneCloudCoinFromJsonFile(bankFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = bankFileName;
                        c++;
                    }
                    else if (File.Exists(partialFileName)) // Is it a partial file 
                    {
                        CloudCoin coinNote = fileSystem.loadOneCloudCoinFromJsonFile(partialFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = partialFileName;
                        c++;
                    }
                    else
                    {
                        CloudCoin coinNote = this.fileSystem.loadOneCloudCoinFromJsonFile(frackedFileName);
                        coinNote.aoid = null;
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = frackedFileName;
                        c++;
                    }

                    m1--;
                    // Get the clean JSON of the coin
                }// end if coin is a 1

                if (denomination == "5" && m5 > 0)
                {
                    if ((c != 0))
                    {
                        json += ",\n";
                    }

                    if (File.Exists(bankFileName))
                    {
                        CloudCoin coinNote = this.fileSystem.loadOneCloudCoinFromJsonFile(bankFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = bankFileName;
                        c++;
                    }
                    else if (File.Exists(partialFileName)) // Is it a partial file 
                    {
                        CloudCoin coinNote = fileSystem.loadOneCloudCoinFromJsonFile(partialFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = partialFileName;
                        c++;
                    }
                    else
                    {
                        CloudCoin coinNote = this.fileSystem.loadOneCloudCoinFromJsonFile(frackedFileName);
                        coinNote.aoid = null;
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = frackedFileName;
                        c++;
                    }

                    m5--;
                } // end if coin is a 5

                if (denomination == "25" && m25 > 0)
                {
                    if ((c != 0))
                    {
                        json += ",\n";
                    }

                    if (File.Exists(bankFileName))
                    {
                        CloudCoin coinNote = this.fileSystem.loadOneCloudCoinFromJsonFile(bankFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = bankFileName;
                        c++;
                    }
                    else if (File.Exists(partialFileName)) // Is it a partial file 
                    {
                        CloudCoin coinNote = fileSystem.loadOneCloudCoinFromJsonFile(partialFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = partialFileName;
                        c++;
                    }
                    else
                    {
                        CloudCoin coinNote = this.fileSystem.loadOneCloudCoinFromJsonFile(frackedFileName);
                        coinNote.aoid = null;
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = frackedFileName;
                        c++;
                    }

                    m25--;
                }// end if coin is a 25

                if (denomination == "100" && m100 > 0)
                {
                    if ((c != 0))
                    {
                        json += ",\n";
                    }

                    if (File.Exists(bankFileName))
                    {
                        CloudCoin coinNote = this.fileSystem.loadOneCloudCoinFromJsonFile(bankFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = bankFileName;
                        c++;
                    }
                    else if (File.Exists(partialFileName)) // Is it a partial file 
                    {
                        CloudCoin coinNote = fileSystem.loadOneCloudCoinFromJsonFile(partialFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = partialFileName;
                        c++;
                    }
                    else
                    {
                        CloudCoin coinNote = this.fileSystem.loadOneCloudCoinFromJsonFile(frackedFileName);
                        coinNote.aoid = null;
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = frackedFileName;
                        c++;
                    }

                    m100--;
                } // end if coin is a 100

                if (denomination == "250" && m250 > 0)
                {
                    if ((c != 0))
                    {
                        json += ",\n";
                    }

                    if (File.Exists(bankFileName))
                    {
                        CloudCoin coinNote = this.fileSystem.loadOneCloudCoinFromJsonFile(bankFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = bankFileName;
                        c++;
                    }
                    else if (File.Exists(partialFileName)) // Is it a partial file 
                    {
                        CloudCoin coinNote = fileSystem.loadOneCloudCoinFromJsonFile(partialFileName);
                        coinNote.aoid = null;//Clear all owner data
                        json = json + fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = partialFileName;
                        c++;
                    }
                    else
                    {
                        CloudCoin coinNote = this.fileSystem.loadOneCloudCoinFromJsonFile(frackedFileName);
                        coinNote.aoid = null;
                        json = json + this.fileSystem.setJSON(coinNote);
                        coinsToDelete[c] = frackedFileName;
                        c++;
                    }

                    m250--;
                }// end if coin is a 250

                if (m1 == 0 && m5 == 0 && m25 == 0 && m100 == 0 && m250 == 0)
                {
                    break;
                } // Break if all the coins have been called for. 
                string status = String.Format("exported %d of %d coin.", i, bankedFileNames.Length);
                int percentCompleted = (i + 1) * 100 / bankedFileNames.Length;
            }// end for each coin needed

            /*WRITE JSON TO FILE*/
            json = json + "\t] " + Environment.NewLine;
            json += "}";
            String filename = (this.fileSystem.ExportFolder + Path.DirectorySeparatorChar + totalSaved + ".CloudCoins." + tag + ".stack");

            if (mode == 1)
            {
                filename = (backupDir + Path.DirectorySeparatorChar + totalSaved + ".CloudCoins." + tag + ".stack");
            }
            if (File.Exists(filename))
            {
                // tack on a random number if a file already exists with the same tag
                Random rnd = new Random();
                int tagrand = rnd.Next(999);
                filename = (this.fileSystem.ExportFolder + Path.DirectorySeparatorChar + totalSaved + ".CloudCoins." + tag + tagrand + ".stack");
            }//end if file exists

            File.WriteAllText(filename, json);
            Console.Out.WriteLine("Writing to : ");
            //CoreLogger.Log("Writing to : " + filename);
            Console.Out.WriteLine(filename);
            /*DELETE FILES THAT HAVE BEEN EXPORTED*/
            if (mode == 0)
                for (int cc = 0; cc < coinsToDelete.Length; cc++)
                {
                    // Console.Out.WriteLine("Deleting " + coinsToDelete[cc]);
                    if (coinsToDelete[cc] != null) { File.Delete(coinsToDelete[cc]); }
                }//end for all coins to delete

            // end if write was good
            return jsonExported;
        }//end write json to file

        public bool writeMp3Files(int m1, int m5, int m25, int m100, int m250, String tagMod){
            bool Mp3Exported = true;
            int coinCount = m1 + m5 + m25 + m100 + m250;   
            int totalSaved = m1 + (m5 * 5) + (m25 * 25) + (m100 * 100) + (m250 * 250);
            int c = 0; // c= counter
            string bankFileName = "";
            string frackedFileName = "";
            string partialFileName = "";
            string denomination = "";
           
            String[] stacksToDelete = new String[coinCount];
            String ccFilename = ( totalSaved + ".CloudCoins." + tagMod + ".stack");
            TagLib.File mp3File = TagLib.File.Create(Mp3Methods.ReturnMp3FilePath());
            TagLib.Ape.Tag apeTag = Mp3Methods.CheckApeTag(mp3File); 

            try{
                //Get all names in bank folders
                String[] bankedFileNames_ = new DirectoryInfo(fileSystem.BankFolder).GetFiles().Select(o => o.Name).ToArray();
                String[] frackedFileNames_ = new DirectoryInfo(fileSystem.FrackedFolder).GetFiles().Select(o => o.Name).ToArray(); 
                String[] partialFileNames_ = new DirectoryInfo(fileSystem.PartialFolder).GetFiles().Select(o => o.Name).ToArray();

                // Add the arrays together
                var list = new List<String>();
                list.AddRange(bankedFileNames_);
                list.AddRange(frackedFileNames_);
                list.AddRange(partialFileNames_);
                String[] arrayOfBankNames = list.ToArray();
                // Program will spend fracked files like perfect files

                foreach(string name in arrayOfBankNames){
                    Console.WriteLine("Working 1! " + name); 
                }//end foreach
                // Check to see the denomination by looking at the file start

                String json = "{" + Environment.NewLine;
                json = json + "\t\"cloudcoin\": " + Environment.NewLine;
                json = json + "\t[" + Environment.NewLine;

                Stack stack = new Stack();
                // Put all the JSON together and add header and footer
                for (int i = 0; (i < arrayOfBankNames.Length); i++)
                {
                    try{
                        string Denomination = arrayOfBankNames[i].Split('.')[0];
                        // Console.WriteLine("denomination! " + Denomination);
                        denomination = Denomination;

                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Catch: " + e);
                    }
                    try{
                        string BankFileName = fileSystem.BankFolder + arrayOfBankNames[i];//File name in bank folder
                        // Console.WriteLine("bankFileName! " + BankFileName);
                        bankFileName = BankFileName;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Catch: " + e);
                    }
                    try{
                        string FrackedFileName = fileSystem.FrackedFolder + arrayOfBankNames[i];//File name in fracked folder
                        // Console.WriteLine("frackedFileName! " + FrackedFileName);
                        frackedFileName = FrackedFileName;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Catch: " + e);
                    }
                    try{
                        string PartialFileName = fileSystem.PartialFolder + arrayOfBankNames[i];
                        // Console.WriteLine("partialFileName! " + PartialFileName);
                        partialFileName = PartialFileName;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Catch: " + e);
                    }

                    if (denomination == "1" && m1 > 0)
                    {
                        if (c != 0)//This is the json seperator between each coin. It is not needed on the first coin
                        {
                            json += ",\n";
                        }

                        if (System.IO.File.Exists(bankFileName)) // Is it a bank file 
                        {

                            CloudCoin coinNote = fileSystem.LoadCoin(bankFileName);
                            coinNote.aoid = null;//Clear all owner data
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = bankFileName;
                            c++;
                        }
                        else if (System.IO.File.Exists(partialFileName)) // Is it a partial file 
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(partialFileName);
                            //coinNote = fileSystem.loa
                            coinNote.aoid = null;//Clear all owner data
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = partialFileName;
                            c++;
                        }
                        else
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(frackedFileName);
                            coinNote.aoid = null;
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = frackedFileName;
                            c++;
                        }

                        m1--;
                        // Get the clean JSON of the coin
                    }// end if coin is a 1

                    if (denomination == "5" && m5 > 0)
                    {
                        if ((c != 0))
                        {
                            json += ",\n";
                        }

                        if (System.IO.File.Exists(bankFileName))
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(bankFileName);
                            coinNote.aoid = null;//Clear all owner data
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = bankFileName;
                            c++;
                        }
                        else if (System.IO.File.Exists(partialFileName)) // Is it a partial file 
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(partialFileName);
                            coinNote.aoid = null;//Clear all owner data
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = partialFileName;
                            c++;
                        }
                        else
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(frackedFileName);
                            coinNote.aoid = null;
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = frackedFileName;
                            c++;
                        }

                        m5--;
                    } // end if coin is a 5

                    if (denomination == "25" && m25 > 0)
                    {
                        if ((c != 0))
                        {
                            json += ",\n";
                        }

                        if (System.IO.File.Exists(bankFileName))
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(bankFileName);
                            coinNote.aoid = null;//Clear all owner data
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = bankFileName;
                            c++;
                        }
                        else if (System.IO.File.Exists(partialFileName)) // Is it a partial file 
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(partialFileName);
                            coinNote.aoid = null;//Clear all owner data
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = partialFileName;
                            c++;
                        }
                        else
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(frackedFileName);
                            coinNote.aoid = null;
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = frackedFileName;
                            c++;
                        }

                        m25--;
                    }// end if coin is a 25

                    if (denomination == "100" && m100 > 0)
                    {
                        if ((c != 0))
                        {
                            json += ",\n";
                        }

                        if (System.IO.File.Exists(bankFileName))
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(bankFileName);
                            coinNote.aoid = null;//Clear all owner data
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = bankFileName;
                            c++;
                        }
                        else if (System.IO.File.Exists(partialFileName)) // Is it a partial file 
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(partialFileName);
                            coinNote.aoid = null;//Clear all owner data
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = partialFileName;
                            c++;
                        }
                        else
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(frackedFileName);
                            coinNote.aoid = null;
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = frackedFileName;
                            c++;
                        }

                        m100--;
                    } // end if coin is a 100

                    if (denomination == "250" && m250 > 0)
                    {
                        if ((c != 0))
                        {
                            json += ",\n";
                        }
                        if (System.IO.File.Exists(bankFileName))
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(bankFileName);
                            coinNote.aoid = null;//Clear all owner data
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = bankFileName;
                            c++;
                        }
                        else if (System.IO.File.Exists(partialFileName)) // Is it a partial file 
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(partialFileName);
                            coinNote.aoid = null;//Clear all owner data
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = partialFileName;
                            c++;
                        }
                        else
                        {
                            CloudCoin coinNote = fileSystem.LoadCoin(frackedFileName);
                            coinNote.aoid = null;
                            json = json + fileSystem.setJSON(coinNote);
                            stacksToDelete[c] = frackedFileName;
                            c++;
                        }
                        m250--;
                    }// end if coin is a 250
                    if (m1 == 0 && m5 == 0 && m25 == 0 && m100 == 0 && m250 == 0)
                    {
                        break;
                    } // Break if all the coins have been called for. 
                    json = json + "\t] " + Environment.NewLine;
                    json += "}";  
                }// end for each coin needed
                if (System.IO.File.Exists(ccFilename))
                {
                    // tack on a random number if a file already exists with the same tag
                    Random rnd = new Random();
                    int tagrand = rnd.Next(999);
                    ccFilename = (totalSaved + ".CloudCoins." + tagMod + tagrand + ".stack");
                }//end if file exists
                
                Mp3Methods.consolePrintList(stacksToDelete, true, "message", true);
                apeTag = Mp3Methods.SetApeTagValue(apeTag, json, ccFilename);
                Console.WriteLine("Mp3 saving...");
                mp3File.Save();
                Console.WriteLine("Success");

            }
            catch(Exception e)
            {
                Console.WriteLine("Error: ", e);
                return false;
            }


            /*DELETE FILES THAT HAVE BEEN EXPORTED*/
            for (int cc = 0; cc < stacksToDelete.Length-1; cc++)
            {
                // Console.Out.WriteLine("Deleting " + coinsToDelete[cc]);
                if (stacksToDelete[cc] != null) { System.IO.File.Delete(stacksToDelete[cc]); }
            }//end for all coins to delete


            return Mp3Exported;
        }

        /* PRIVATE METHODS */
        private void qrCodeWriteOne(String path, String tag, String bankFileName, String frackedFileName, String partialFileName)
        {
            if (File.Exists(bankFileName))//If the file is a bank file, export a good bank coin
            {
                CloudCoin jpgCoin = this.fileSystem.LoadCoin(bankFileName);
                if (this.fileSystem.writeQrCode(jpgCoin, tag))//If the jpeg writes successfully 
                {
                    //string json = JsonConvert.SerializeObject(jpgCoin);
                    //var barcode = new Barcode(json, Settings.Default);
                    //barcode.Canvas.SaveBmp(jpgCoin.FileName+".jpg");
                    File.Delete(bankFileName);//Delete the files if they have been written to
                }//end if write was good. 
            }
            else if (File.Exists(partialFileName))//If the file is a bank file, export a good bank coin
            {
                CloudCoin jpgCoin = this.fileSystem.LoadCoin(partialFileName);
                if (this.fileSystem.writeQrCode(jpgCoin, tag))//If the jpeg writes successfully 
                {
                    File.Delete(partialFileName);//Delete the files if they have been written to
                }//end if write was good. 
            }
            else//Export a fracked coin. 
            {
                CloudCoin jpgCoin = fileSystem.LoadCoin(frackedFileName);
                if (this.fileSystem.writeQrCode(jpgCoin, tag))
                {
                    File.Delete(frackedFileName);//Delete the files if they have been written to
                }//end if
            }//end else
        }//End write one jpeg 

        private void barCode417WriteOne(String path, String tag, String bankFileName, String frackedFileName, String partialFileName)
        {
            if (File.Exists(bankFileName))//If the file is a bank file, export a good bank coin
            {
                CloudCoin jpgCoin = this.fileSystem.LoadCoin(bankFileName);
                if (this.fileSystem.writeBarCode(jpgCoin, tag))//If the jpeg writes successfully 
                {
                    //string json = JsonConvert.SerializeObject(jpgCoin);
                    //var barcode = new Barcode(json, Settings.Default);
                    //barcode.Canvas.SaveBmp(jpgCoin.FileName+".jpg");
                    File.Delete(bankFileName);//Delete the files if they have been written to
                }//end if write was good. 
            }
            else if (File.Exists(partialFileName))//If the file is a bank file, export a good bank coin
            {
                CloudCoin jpgCoin = this.fileSystem.LoadCoin(partialFileName);
                if (this.fileSystem.writeBarCode(jpgCoin, tag))//If the jpeg writes successfully 
                {
                    File.Delete(partialFileName);//Delete the files if they have been written to
                }//end if write was good. 
            }
            else//Export a fracked coin. 
            {
                CloudCoin jpgCoin = fileSystem.LoadCoin(frackedFileName);
                if (this.fileSystem.writeBarCode(jpgCoin, tag))
                {
                    File.Delete(frackedFileName);//Delete the files if they have been written to
                }//end if
            }//end else
        }//End write one jpeg 


        /* PRIVATE METHODS */
        private void jpegWriteOne(String path, String tag, String bankFileName, String frackedFileName, String partialFileName)
        {
            if (File.Exists(bankFileName))//If the file is a bank file, export a good bank coin
            {
                CloudCoin jpgCoin = this.fileSystem.LoadCoin(bankFileName);
                if (this.fileSystem.writeJpeg(jpgCoin, tag))//If the jpeg writes successfully 
                {
                    File.Delete(bankFileName);//Delete the files if they have been written to
                }//end if write was good. 
            }
            else if (File.Exists(partialFileName))//If the file is a bank file, export a good bank coin
            {
                CloudCoin jpgCoin = this.fileSystem.LoadCoin(partialFileName);
                if (this.fileSystem.writeJpeg(jpgCoin, tag))//If the jpeg writes successfully 
                {
                    File.Delete(partialFileName);//Delete the files if they have been written to
                }//end if write was good. 
            }
            else//Export a fracked coin. 
            {
                CloudCoin jpgCoin = fileSystem.LoadCoin(frackedFileName);
                if (this.fileSystem.writeJpeg(jpgCoin, tag))
                {
                    File.Delete(frackedFileName);//Delete the files if they have been written to
                }//end if
            }//end else
        }//End write one jpeg 

        private void writeMp3File()
        {
           
        }// end writeMp3Files()
        
    }// end exporter class
}//end namespace
