using RandomSystemPeopleToAnswer.Models;

namespace RandomSystemPeopleToAnswer;

public partial class AttendanceList : ContentPage
{
    public AttendanceList()
    {
        InitializeComponent();
        LoadClasses();
    }

    void LoadClasses()
    {
        var students = LoadStudents();
        List<string> classes = new List<string>();

        foreach (var s in students)
        {
            if (!classes.Contains(s.NumberClass))
            {
                classes.Add(s.NumberClass);
            }
        }

        classPicker.ItemsSource = classes;
    }

    List<Student> LoadStudents()
    {
        string path = Path.Combine(FileSystem.AppDataDirectory, "baza_szkoly.txt");
        var list = new List<Student>();

        if (!File.Exists(path))
            return list;

        foreach (var line in File.ReadAllLines(path))
        {
            var p = line.Split(';');

            list.Add(new Student
            {
                NumberClass = p[0],
                FirstName = p[1],
                LastName = p[2],
                NumberStudent = int.TryParse(p[3], out int num) ? num : 0
            });
        }

        return list;
    }

    private void LoadAttendance(object sender, EventArgs e)
    {
        if (classPicker.SelectedItem == null)
            return;

        string selectedClass = classPicker.SelectedItem.ToString();

        List<Student> all = LoadStudents();
        List<Student> students = new List<Student>();

        foreach (var s in all)
        {
            if (s.NumberClass == selectedClass)
            {
                students.Add(s);
            }
        }

        string file = Path.Combine(FileSystem.AppDataDirectory, $"attendance_{selectedClass}.txt");

        Dictionary<int, bool> present = new Dictionary<int, bool>();

        if (File.Exists(file))
        {
            foreach (var line in File.ReadAllLines(file))
            {
                var p = line.Split(';');

                if (int.TryParse(p[0], out int num))
                    present[num] = bool.Parse(p[1]);
            }
        }

        List<AttendanceItem> list = new List<AttendanceItem>();

        foreach (var s in students)
        {
            AttendanceItem item = new AttendanceItem();
            item.FullName = s.FirstName + " " + s.LastName;
            item.NumberStudent = s.NumberStudent;

            if (present.ContainsKey(s.NumberStudent))
                item.IsPresent = present[s.NumberStudent];
            else
                item.IsPresent = true;

            list.Add(item);
        }

        attendanceList.ItemsSource = list;
    }

    private async void SaveAttendance(object sender, EventArgs e)
    {
        if (classPicker.SelectedItem == null)
            return;

        string selectedClass = classPicker.SelectedItem.ToString();
        string file = Path.Combine(FileSystem.AppDataDirectory, $"attendance_{selectedClass}.txt");

        List<AttendanceItem> items = new List<AttendanceItem>();

        foreach (var obj in attendanceList.ItemsSource)
        {
            items.Add((AttendanceItem)obj);
        }

        using (var sw = new StreamWriter(file))
        {
            foreach (var item in items)
            {
                sw.WriteLine(item.NumberStudent + ";" + item.IsPresent);
            }
        }

        await DisplayAlert("Sukces", "Obecnoœæ zosta³a zapisana.", "OK");
    }
}
