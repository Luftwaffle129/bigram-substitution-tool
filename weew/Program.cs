using System.ComponentModel.Design;
using System.IO;
using System.Security.Cryptography;
using weew;
using System.Text.RegularExpressions;
if (!File.Exists("retbgsges4rg.txt"))
{
    File.Create("retbgsges4rg.txt");
    Console.WriteLine("File created: add text to it");
    Console.ReadLine();
    Environment.Exit(0);
}
string file = File.ReadAllText("retbgsges4rg.txt");

if (file.Length == 0)
{
    Console.WriteLine("Add text to file");
    Console.ReadLine();
    Environment.Exit(0);
}
List<Rule> rules = new List<Rule>();

int KEYLEN;
Console.WriteLine("input key length");
KEYLEN = Convert.ToInt32(Console.ReadLine());

Console.WriteLine("" +
    "Add rule: A {key} {value} \n" +
    "Update rule: U {key} {value} \n" +
    "Delete rule: R {key} {value} \n" +
    "Delete all rules: DA\n" +
    "View frequencies: F \n" +
    "Highlight stuff: H {key} {key} {key} ...\n" +
    "View rules: V {key} {value} \n" +
    "View text: T \n" +
    "View view original text: Z \n\n" +
    "key and value are length = 2"
    );
while (true)
{
    string input = Console.ReadLine();
    //
    if (input != null && input != "")
    {
        if (input.Substring(0, 1).ToUpper() == "A")
        {
            if (Regex.IsMatch(input, @"^(\w \w\w \w\w)$"))
                AddRule(input.Substring(2, KEYLEN), input.Substring(5, KEYLEN));
                    else
            Console.WriteLine("syntax syntax");
        }
        else if (input.Substring(0, 1).ToUpper() == "U")
        {
            if (Regex.IsMatch(input, @"^(\w \w\w \w\w)$"))
                UpdateRule(input.Substring(2, KEYLEN), input.Substring(5, KEYLEN));
            else
                Console.WriteLine("syntax syntax");
        }
        else if (input.Substring(0, 1).ToUpper() == "R" || input.Substring(0, 1).ToUpper() == "D")
        {
            if (Regex.IsMatch(input, @"^(\w \w\w)$"))
                DelRule(input.Substring(2, KEYLEN));
            else
                Console.WriteLine("syntax syntax");
        }
        else if (input.Substring(0, 1).ToUpper() == "F")
        {
            GetFreq();
        }
        else if (input.Substring(0, 1).ToUpper() == "H")
        {
            if (input.Length > 2)
                Highlight(input.Substring(2));
            else
                Console.WriteLine("too short");
        }
        else if (input.Substring(0, 1).ToUpper() == "V")
        {
            ViewRule();
        }
        else if (input.Substring(0, 1).ToUpper() == "T")
        {
            Console.WriteLine(ViewText());
        }
        else if (input.Substring(0, 1).ToUpper() == "Z")
        {
            Console.WriteLine(file);
        }
        else if (input.Length >= 2)
            if (input.Substring(0, 2).ToUpper() == "DA")
            {
                DelAllRule();
            }
        else
            Console.WriteLine("no action");
    }
    else
        Console.WriteLine("no action");
}

void AddRule(string key, string value)
{
    key = key.ToUpper();
    value = value.ToLower();
    bool allowedAdd = true;
    if (rules.Count > 0)
    {
        foreach (Rule rule in rules)
        {
            if (rule.Key == key || rule.Value == value)
                allowedAdd = false;
        }
    }
    if (allowedAdd)
    {
        rules.Add(new Rule(key, value));
        Console.WriteLine("Added rule");
    }
    else
    {
        Console.WriteLine("Rule not unique");
    }

}
void Highlight(string str)
{
    if (Regex.IsMatch(str, @"\w{2}( \w{2})+"))
    {
        string[] highlights = str.Split(' ');

        string textRaw = ViewText();
        string textF = "";
        for (int i = 0; i < textRaw.Length; i += KEYLEN)
        {
            if (highlights.Contains(textRaw.Substring(i, KEYLEN)))
                textF += $" {textRaw.Substring(i, KEYLEN)} ";
            else
                textF += textRaw.Substring(i, KEYLEN);
        }
        Console.WriteLine(textF);
    }
    else
        Console.WriteLine("Incorrect format");
}
void UpdateRule(string key, string value)
{
    DelRule(key);
    AddRule(key, value);
}
void DelRule(string key)
{
    key = key.ToUpper();
    if (rules.Count > 0)
    {
        int index = 0;
        bool removed = false;
        do
        {
            if (rules[index].Key == key)
            {
                rules.RemoveAt(index);
                removed = true;
            }
        } while (rules.Count > ++index && !removed);
        if (removed)
            Console.WriteLine("Removed");
        else
            Console.WriteLine("Not removed");
    }
    else
        Console.WriteLine("Empty");
}
void DelAllRule()
{
    rules = new List<Rule>();
    Console.WriteLine("Round 2!");
}
void ViewRule()
{
    if (rules.Count > 0)
        foreach (var rule in rules)
            Console.WriteLine($"Key: {rule.Key}  Value: {rule.Value}");
    else
        Console.WriteLine("empty");
}
string ViewText()
{
    string output = "";
    for (int i = 0; i < file.Length; i += KEYLEN)
    {
        string val = file.Substring(i, KEYLEN);
        if (rules.Count > 0)
        {
            bool swap = false;
            int index = 0;

            do {
                if (rules[index].Key == val)
                {
                    output += rules[index].Value;
                    swap = true;
                }
            } while (++index < rules.Count && !swap);
            if (!swap)
            {
                output += val;
            }
        }
        else
            output += file.Substring(i, KEYLEN);
    }
    return output;
}
void GetFreq()
{
    List<Freq> keys = new List<Freq>();
    string output = ViewText();
    for (int i = 0; i < output.Length; i += KEYLEN)
    {
        bool incremented = false; 
        if (keys.Count > 0)
        {
            int j = 0;
            do
            {
                if (keys[j].Key == output.Substring(i, KEYLEN))
                {
                    incremented = true;
                    keys[j].Count++;
                }
            } while (++j < keys.Count && !incremented);
        }
        if (!incremented)
            keys.Add(new Freq(output.Substring(i, KEYLEN)));
    }
    if (keys.Count > 0)
    {
        Sort();
        keys.Reverse();
        foreach (Freq key in keys)
        {
            if (key.Count > 1)
                Console.WriteLine(key.Key + " " + key.Count);
        }
    }

    void Sort()
    {
        List<Freq> newKeys = new List<Freq>();
        for (int i = 0;i < keys.Count; i ++)
        {
            int insertIndex = -1;
            if (newKeys.Count > 0)
            {
                int j = 0;
                bool found = false;
                do
                {
                    if (keys[i].Count > newKeys[j].Count)
                    {
                        insertIndex = j;
                        found = true;
                    }
                } while (++j < newKeys.Count && !found);
                if (!found)
                    insertIndex = j;
            }
            else
                insertIndex = 0;
            newKeys.Insert(insertIndex, keys[i]);
        }
        keys = newKeys;
    }
}