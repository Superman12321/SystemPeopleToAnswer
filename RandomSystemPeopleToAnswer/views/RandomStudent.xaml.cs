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
    Queue<int> lastThree = new Queue<int>();

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

        if (!File.Exists(path))
            return list;

        foreach (var line in File.ReadAllLines(path))
        {
            var parts = line.Split(';');

            Student s = new Student();
            s.NumberClass = parts[0];
            s.FirstName = parts[1];
            s.LastName = parts[2];

            if (int.TryParse(parts[3], out int num))
                s.NumberStudent = num;
            else
                s.NumberStudent = 0;

            list.Add(s);
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

    Dictionary<int, bool> LoadAttendance(string selectedClass)
    {
        Dictionary<int, bool> present = new Dictionary<int, bool>();

        string file = Path.Combine(FileSystem.AppDataDirectory, $"attendance_{selectedClass}.txt");

        if (!File.Exists(file))
            return present;

        foreach (var line in File.ReadAllLines(file))
        {
            var p = line.Split(';');

            if (int.TryParse(p[0], out int num))
            {
                bool value = bool.Parse(p[1]);
                present[num] = value;
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
            if (int.TryParse(line, out int num))
                lastThree.Enqueue(num);
        }
    }

    void SaveQueue()
    {
        string file = Path.Combine(FileSystem.AppDataDirectory, "cooldown.txt");
        File.WriteAllLines(file, lastThree.Select(x => x.ToString()));
    }

    private void randomStudent(object sender, EventArgs e)
    {
        if (classPicker.SelectedItem == null)
        {
            randomNumberText.Text = "Wybierz klasê!";
            return;
        }

        string selectedClass = classPicker.SelectedItem.ToString();

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

        Dictionary<int, bool> present = LoadAttendance(selectedClass);

        List<Student> presentStudents = new List<Student>();

        foreach (var s in classStudents)
        {
            bool isPresent = true;

            if (present.ContainsKey(s.NumberStudent))
            {
                isPresent = present[s.NumberStudent];
            }

            if (isPresent)
            {
                if (!lastThree.Contains(s.NumberStudent))
                {
                    presentStudents.Add(s);
                }
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
        while (chosen.NumberStudent == _lucky);

        randomNumberText.Text =
            $"{chosen.FirstName} {chosen.LastName} (nr {chosen.NumberStudent})";

        lastThree.Enqueue(chosen.NumberStudent);

        if (lastThree.Count > 3)
            lastThree.Dequeue();

        SaveQueue();
    }
}
