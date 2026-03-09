using RandomSystemPeopleToAnswer.Models;

namespace RandomSystemPeopleToAnswer;

public static class LuckyNumberService
{
    private const string Key = "LuckyNumber";
    private const string DateKey = "LuckyNumberDate";

    public static int GenerateLuckyNumber(int min, int max)
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        string savedDate = Preferences.Get(DateKey, "");

        if (savedDate == today)
        {
            return Preferences.Get(Key, -1);
        }

        Random random = new Random();
        int lucky = random.Next(min, max + 1);

        Preferences.Set(Key, lucky);
        Preferences.Set(DateKey, today);

        return lucky;
    }

    public static int GetSavedLuckyNumber()
    {
        return Preferences.Get(Key, -1);
    }
}


public partial class RandomStudent : ContentPage
{
    private int _lucky;
    Queue<string> lastThree = new Queue<string>();

    public RandomStudent()
    {
        InitializeComponent();
        LoadClassesToPicker();
        LoadQueue();

        _lucky = LuckyNumberService.GenerateLuckyNumber(1, 40);
        luckyNumberLabel.Text = "Szczêœliwy numerek: " + _lucky;
    }
    List<Student> LoadStudents()
    {
        string path = Path.Combine(FileSystem.AppDataDirectory, "baza_szkoly.txt");

        var list = new List<Student>();

        foreach (var line in File.ReadAllLines(path))
        {
            var parts = line.Split(';');
            list.Add(new Student
            {
                NumberClass = parts[0],
                FirstName = parts[1],
                LastName = parts[2],
                NumberStudent = parts[3]
            });
        }

        return list;
    }

    void LoadClassesToPicker()
    {
        List<Student> students = LoadStudents();
        List<string> classes = new List<string>();

        foreach (Student s in students)
        {
            if (!classes.Contains(s.NumberClass))
            {
                classes.Add(s.NumberClass);
            }
        }

        classPicker.ItemsSource = classes;
    }

    Dictionary<string, bool> LoadAttendance(string selectedClass)
    {
        Dictionary<string, bool> present = new Dictionary<string, bool>();

        string file = Path.Combine(FileSystem.AppDataDirectory, $"attendance_{selectedClass}.txt");

        if (File.Exists(file))
        {
            foreach (var line in File.ReadAllLines(file))
            {
                var p = line.Split(';');
                present[p[0]] = bool.Parse(p[1]);
            }
        }

        return present;
    }
    void LoadQueue()
    {
        string file = Path.Combine(FileSystem.AppDataDirectory, "cooldown.txt");

        if (!File.Exists(file))
            return;

        foreach (var line in File.ReadAllLines(file))
        {
            lastThree.Enqueue(line);
        }
    }
    void SaveQueue()
    {
        string file = Path.Combine(FileSystem.AppDataDirectory, "cooldown.txt");
        File.WriteAllLines(file, lastThree);
    }
    private void randomStudent(object sender, EventArgs e)
    {
        string selectedClass = "";

        if (classPicker.SelectedItem != null)
        {
            selectedClass = classPicker.SelectedItem.ToString();
        }
        else
        {
            randomNumberText.Text = "Wybierz klasê!";
            return;
        }

        if (string.IsNullOrWhiteSpace(selectedClass))
        {
            randomNumberText.Text = "Wybierz klasê!";
            return;
        }

        List<Student> allStudents = LoadStudents();
        List<Student> classStudents = new List<Student>();

        foreach (Student s in allStudents)
        {
            if (s.NumberClass == selectedClass)
            {
                classStudents.Add(s);
            }
        }

        if (classStudents.Count == 0)
        {
            randomNumberText.Text = "Brak uczniów w tej klasie!";
            return;
        }

        Dictionary<string, bool> present = LoadAttendance(selectedClass);

        List<Student> presentStudents = new List<Student>();

        foreach (var s in classStudents)
        {
            if (present.ContainsKey(s.NumberStudent))
            {
                if (present[s.NumberStudent] == true)
                {
                    if (!lastThree.Contains(s.NumberStudent))
                        presentStudents.Add(s);
                }
            }
            else
            {
                if (!lastThree.Contains(s.NumberStudent))
                    presentStudents.Add(s);
            }
        }

        if (presentStudents.Count == 0)
        {
            randomNumberText.Text = "Brak obecnych uczniów!";
            return;
        }

        Random random = new Random();
        Student chosen;

        do
        {
            int index = random.Next(presentStudents.Count);
            chosen = presentStudents[index];
        }
        while (int.Parse(chosen.NumberStudent) == _lucky);

        randomNumberText.Text = chosen.FirstName + " " + chosen.LastName + " (nr " + chosen.NumberStudent + ")";

        lastThree.Enqueue(chosen.NumberStudent);

        if (lastThree.Count > 3)
            lastThree.Dequeue();

        SaveQueue();
    }
}
