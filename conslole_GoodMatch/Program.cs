using System.Text.RegularExpressions;
//method for getting the frequency string 
static string CalcFreq(string input) 
{
    List<char> checkedChars = new List<char>();
    string frequencyStr = "";
    for(int p = 0; p<input.Length;p++)
    {
        if (!checkedChars.Contains(input[p]))// if not checked-> we check it.
        {
            checkedChars.Add(input[p]);//add to checked list
            int count = 0;
            for (int i = p; i < input.Length; i++)
            {
                if (input[i].Equals(input[p]))
                {
                    count += 1;
                }
            }
            
            frequencyStr += count.ToString();
            count = 0;
        }
    }
    return frequencyStr;
}
//method for calculating the match percentage
static string CalcMatchPerc(string input)//uses recursion
{
    string finalString = "";
    
    int length = input.Length;
    
    if (input.Length == 2)// terminating condition
    {
        return input;
    }
    
    if (length%2 == 0)// even length
    {
        for (int i = 0; i<length/2;i++)
        {
            int sum = int.Parse(input[i].ToString()) + int.Parse(input[length-1-i].ToString());
            finalString = String.Concat(finalString, sum);
        }
    }
    else// odd length
    {
        for (int i = 0; i<=length/2;i++)
        {
            if(i == length/2)
            {
                finalString = String.Concat(finalString, input[i]);
            }
            else
            {
                int sum = int.Parse(input[i].ToString()) + int.Parse(input[length-1-i].ToString());
                finalString = String.Concat(finalString, sum);
            }
        }
        
    }
    return CalcMatchPerc(finalString);
}
        
//good match 
static string GoodMatch(string input)
{
    string[] temp = input.Split(' ');
    string freq = CalcFreq(temp[0] + temp[1] + temp[2]);//get frequency string
    int percentage = int.Parse(CalcMatchPerc(freq));// get percentage

    return percentage > 80 ? $"{temp[0]} matches {temp[2]}, {percentage}%, good match" : $"{temp[0]} matches {temp[2]} {percentage}%";
}

static List<List<string>> ReadCsvFile()
{
    var malePlayers = new List<string>();
    var femalePlayers = new List<string>();
    Console.WriteLine("Please Enter CSV file FULL path: ");
    string input = Console.ReadLine();
    try
    {
        if (input != null)
        {
            StreamReader reader = new StreamReader(input);
            using (reader)
            {
                string line = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    if (!Regex.IsMatch(line, @"^\w+,+\w+csv$") || line.Contains(" "))//validation
                    {
                        Console.WriteLine("Input data badly formatted!");
                        Environment.Exit(0);
                    }
                    var values = line.Split(',');
                    
                    if (values[1].Equals("m") && !malePlayers.Contains(values[0]))
                    {
                        malePlayers.Add(values[0]);
                    }
                    else if (values[1].Equals("f") && !femalePlayers.Contains(values[0]))
                    {
                        femalePlayers.Add(values[0]);
                    }
                    line = reader.ReadLine();
                }
            }
        }
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine("That file does not exist!, restart the program to try again.");
    }
    catch (DirectoryNotFoundException)
    {
        Console.WriteLine("Directory does not exist!, restart the program to try again.");
    }
    catch (IOException)
    {
        Console.WriteLine("Oops! Something Wrong happened!");
    }
    var groups = new List<List<string>>();
    groups.Add(malePlayers);
    groups.Add(femalePlayers);
    
    return groups;
    
}

static void ValidateName(string? name)
{
    if (!Regex.IsMatch(name, @"^[a-zA-Z]+$") || name.Contains(" ") || name.Equals(""))
    {
        Console.WriteLine("Invalid Name, restart application to try again");
        Environment.Exit(0);
    }
}

static void RunOnFileData(List<List<string>> data)
{
    const string filename = "output.txt";
    var males = data[0];
    var females = data[1];
    using var StrWr = new StreamWriter(filename);
    foreach (var male in males)
    {
        Dictionary<string, int> dictUnsorted = new Dictionary<string, int>();
        foreach (var female in females)
        { 
            var feedback = GoodMatch($"{male} matches {female}");
            var percentage = int.Parse((feedback.Split(" "))[3].Remove(2));
           
           dictUnsorted.Add(feedback,percentage);
        }
        
        var myDict = dictUnsorted.OrderBy(key => key.Key); //sort alphabetically first
        
        StrWr.WriteLine($"---------------{male}---------------");//heading
        foreach (var item in myDict.OrderByDescending(key => key.Value))
        {
            StrWr.WriteLine(item.Key);
        }

        dictUnsorted.Clear();
       
    }
    StrWr.Close();
    Console.WriteLine("output.txt File Location: "+"bin->Debug->net6.0->output.txt");
}
/*----------------------------------------Main----------------------------------------*/
Console.WriteLine("Enter 1 -> Get a match between two players.\nEnter 2 -> Input a csv file.\n");
var input = Console.ReadLine();
if (input.Equals("1"))//get a match for two players
{
    Console.Write("Enter the first name: ");
    string? firstName = Console.ReadLine();
    ValidateName(firstName);
    Console.Write("Enter the second name: ");
    string? secondName = Console.ReadLine();
    ValidateName(secondName);
            
    string str = $"{firstName} matches {secondName}";
    
    Console.WriteLine(GoodMatch(str));
    Console.Write("Press Any Key To Exit!");
    Console.ReadKey();
}
else if (input.Equals("2"))//for file input
{
    var groups = ReadCsvFile();
    RunOnFileData(groups);
    Console.Write("Press Any Key To Exit!,");
    Console.ReadKey();
}
else
{
    Console.WriteLine("Invalid input, restart the program to try again");
    Console.Write("Press Any Key To Exit!");
    Console.ReadKey();
}

/*------------------------------------------------------------------------------------------*/