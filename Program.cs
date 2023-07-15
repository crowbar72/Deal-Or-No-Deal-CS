using System;
using System.IO;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
// Tristan Leid 2023
/*
 * This is an adaptation of an original deal or no deal application written by myself in C++.
 * This adaptation was made with minimal reference to the original.
 * Due to small scope of project class variables were used over creating a game model or passing individual variables
 */

class Program
{
    static int numCases = 26;                   //number of cases in the game. Left as a variable to account for other possible game sizes
    static int[] caseValues = new int[26];      //amount of money in each case in order as read from file
    static int[] caseRandom = new int[26];      //amount of money in each case after being randomized
    static int numRounds;                       //number of rounds in the game. Read from file
    static int[] casesPerRound = new int[10];   //cases to be open per round. Read from file
    static int userCase;                        //case user picks for the game
    static int unopened;                        //number of unopened case for banker offer calculations

    static void Main(string[] args)
    {
        userCase = 0;
        unopened = numCases;
        int intchoice;
        string strChoice;

        //Reading data from file
        ReadFile("GameData", numCases);

        //Randomize case order
        ShuffleCases();

        //Loops until a valid case is selected, which will start the game and allow exiting the loop
        while (userCase == 0)
        {
            Console.Clear();
            Console.WriteLine("Welcome to Deal or No Deal");
            Console.WriteLine("With your host Howie Mandel");

            Console.WriteLine("Please pick a case");
            DisplayCases();
            strChoice = Console.ReadLine();
            if (int.TryParse(strChoice, out intchoice) && intchoice >= 1 && intchoice <= numCases)
            {
                userCase = intchoice;
                GameLoop();
            }
            else
            {
                Console.WriteLine("Invalid selection");
                Console.Write("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }

    //Reads data from file
    static void ReadFile(string fileName, int numCases)
    {
        String line;
        int i = 0;
        try
        {
            StreamReader sr = new StreamReader("C:\\Users\\trist\\source\\repos\\DealOrNoDeal\\DealOrNoDeal\\" + fileName + ".txt");
            while (i < numCases)
            {
                line = sr.ReadLine();
                int.TryParse(line, out caseValues[i]);
                i++;
            }

            line = sr.ReadLine();
            line = sr.ReadLine();
            int.TryParse(line, out numRounds);
            

            i = 0;
            while (i < numRounds)
            {
                line = sr.ReadLine();
                int.TryParse(line, out casesPerRound[i]);
                i++;
            }

            sr.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }

    //Displays the cases on the game board
    static void DisplayCases()
    {
        int i = 0;
        while (i < numCases)
        {
            if (i + 1 != userCase && caseRandom[i] != 0)
            {
                Console.Write(i + 1);
            }
            Console.Write("\t");

            if (i == 4 && userCase != 0)
            {
                Console.Write("\tYour case: " + userCase);
            }

            if ((i + 1) % 5 == 0)
            {
                Console.WriteLine();
            }

            i++;
        }
        Console.WriteLine("");
    }

    //Shuffles the values in the cases
    static void ShuffleCases()
    {
        Random rnd = new Random();
        int i, j = 0;

        while(j < numCases)
        {
            i = rnd.Next() % numCases;
            if (caseRandom.Contains(caseValues[i]) == false)
            {
                caseRandom[j] = caseValues[i];
                j++;
            }
        }
    }

    //Shows values of the remaining cases in ascending order
    static void DisplayScore()
    {
        int i = 0;
        Console.WriteLine("Money Board:");
        while (i < numCases / 2)
        {
            if (caseValues[i] != 0)
            {
                Console.Write(caseValues[i]);
            }

            Console.Write("\t");

            if (caseValues[i + numCases / 2] != 0)
            {
                Console.Write(caseValues[i + numCases / 2]);
            }

            Console.WriteLine();
            i++;
        }
    }

    //Main game logic
    static void GameLoop()
    {
        int round = 1;
        double offer = 0;
        string deal = "";
        //Player opens cases for first round
        OpenCases(round);
        while (round < numRounds)
        {
            //Calculating banker offer
            for (int i = 0; i < numCases; i++)
            {
                offer = offer + caseRandom[i];
            }
            offer = offer / unopened * round / numRounds;
            offer = Math.Round(offer, 2);

            //Banker offer
            deal = "";
            while(deal != "0" && deal != "1")
            {
                Console.Clear();
                Console.WriteLine("The round had ended.");
                Console.WriteLine();
                DisplayScore();

                Console.WriteLine("Banker's offer: $" + offer);
                Console.WriteLine("Deal(0), or No Deal(1): ");
                deal = Console.ReadLine();

                if(deal == "0")
                {
                    Console.WriteLine("Congratulations! You have finished Deal Or No Deal with a score of: $" + offer);
                    Console.WriteLine("Your case contained: $" + caseRandom[userCase-1]);
                    Console.Write("Press any key to continue...");
                    Console.ReadKey();
                    break;
                }
                else if(deal == "1")
                {
                    Console.WriteLine("You have chosen No Deal. Going to the next round.");
                }
                else
                {
                    Console.WriteLine("Invalid selection");
                }
                Console.Write("Press any key to continue...");
                Console.ReadKey();
            }
            
            //If deal was selected
            if(deal == "0")
            {
                break;
            }

            //Proceded to next round
            round++;

            //Player opens cases for the round
            //Positioning at the end of the loop prevents banker phase from activating in the last round
            OpenCases(round);
        }

        if(deal != "0")
        {
            Console.WriteLine("In the case you selected was $" + caseRandom[userCase-1]);
            Console.WriteLine("Congratulations! You have finished Deal Or No Deal with a score of: $" + caseRandom[userCase - 1]);
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
        
    }

    //loop where players select a number of cases corresponding to the round. Opened cases get removed.
    static void OpenCases(int round)
    {
        int toOpen = casesPerRound[round - 1];
        int intchoice;
        string strChoice;
        while (toOpen  > 0)
        {
            Console.Clear();
            Console.WriteLine("DEAL OR NO DEAL");
            Console.WriteLine("Round " + round + " / " + numRounds);
            Console.WriteLine("Cases Left:");
            DisplayCases();
            Console.WriteLine();
            DisplayScore();
            Console.WriteLine("Cases to open this round: " + toOpen);
            Console.Write("Pick a case to open, or choose zero to insult Howie: ");

            strChoice = Console.ReadLine();
            if (int.TryParse(strChoice, out intchoice) && intchoice == 0)
            {
                Console.WriteLine("Not implemented");
            }
            else if ( intchoice >= 1 
                && intchoice <= numCases 
                && caseRandom[intchoice-1] != 0 
                && intchoice != userCase)
            {

                Console.WriteLine("The contents of case " + intchoice + ": $" + caseRandom[intchoice-1]);
                for(int i = 0; i < numCases; i++)
                {
                    if (caseValues[i] == caseRandom[intchoice - 1])
                    {
                        caseValues[i] = 0;
                    }
                }
                caseRandom[intchoice - 1] = 0;
                toOpen--;
                unopened--;
            }
            else
            {
                Console.WriteLine("Not a valid choice");
            }

            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}