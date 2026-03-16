namespace RandomSystemPeopleToAnswer;

using RandomSystemPeopleToAnswer.Models;

public partial class EditStudentList : ContentPage
{
    List<Student> allStudents = new List<Student>();

    public EditStudentList()
    {
        InitializeComponent();
        LoadStudents();
        LoadClasses();
    }

    void LoadStudents()
    {
        string path = Path.Combine(FileSystem.AppDataDirectory, "baza_szkoly.txt");

        allStudents.Clear();

        if (!File.Exists(path))
            return;

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            string[] parts = line.Split(';');

            if (parts.Length == 4)
            {
                Student s = new Student();
                s.NumberClass = parts[0];
                s.FirstName = parts[1];
                s.LastName = parts[2];
                s.NumberStudent = int.TryParse(parts[3], out int num) ? num : 0;

                allStudents.Add(s);
            }
        }
    }

    void LoadClasses()
    {
        List<string> classes = new List<string>();

        foreach (Student s in allStudents)
        {
            if (!classes.Contains(s.NumberClass))
            {
                classes.Add(s.NumberClass);
            }
        }

        classPicker.ItemsSource = classes;
    }

    void OnClassSelected(object sender, EventArgs e)
    {
        studentPicker.ItemsSource = null;

        if (classPicker.SelectedItem == null)
            return;

        string selectedClass = classPicker.SelectedItem.ToString();

        List<string> studentsInClass = new List<string>();

        foreach (Student s in allStudents)
        {
            if (s.NumberClass == selectedClass)
            {
                string fullName = s.FirstName + " " + s.LastName;
                studentsInClass.Add(fullName);
            }
        }

        studentPicker.ItemsSource = studentsInClass;
    }

    void OnStudentSelected(object sender, EventArgs e)
    {
        if (studentPicker.SelectedItem == null)
            return;

        string selectedClass = classPicker.SelectedItem.ToString();
        string selectedName = studentPicker.SelectedItem.ToString();

        foreach (Student s in allStudents)
        {
            string fullName = s.FirstName + " " + s.LastName;

            if (s.NumberClass == selectedClass && fullName == selectedName)
            {
                firstNameEntry.Text = s.FirstName;
                lastNameEntry.Text = s.LastName;
                numberEntry.Text = s.NumberStudent.ToString();
                break;
            }
        }
    }

    void SaveChanges(object sender, EventArgs e)
    {
        if (classPicker.SelectedItem == null || studentPicker.SelectedItem == null)
            return;

        string selectedClass = classPicker.SelectedItem.ToString();
        string selectedName = studentPicker.SelectedItem.ToString();

        if (!int.TryParse(numberEntry.Text, out int newNumber))
        {
            DisplayAlert("B³¹d", "Numer ucznia musi byæ liczb¹!", "OK");
            return;
        }

        foreach (Student s in allStudents)
        {
            string fullName = s.FirstName + " " + s.LastName;

            if (s.NumberClass == selectedClass &&
                fullName != selectedName &&
                s.NumberStudent == newNumber)
            {
                DisplayAlert("B³¹d",
                    $"Numer {newNumber} jest ju¿ zajêty przez: {fullName}",
                    "OK");
                return;
            }
        }

        foreach (Student s in allStudents)
        {
            string fullName = s.FirstName + " " + s.LastName;

            if (s.NumberClass == selectedClass && fullName == selectedName)
            {
                s.FirstName = firstNameEntry.Text;
                s.LastName = lastNameEntry.Text;
                s.NumberStudent = newNumber;
                break;
            }
        }

        SaveAllStudents();
        DisplayAlert("OK", "Dane ucznia zosta³y zapisane!", "OK");

        OnClassSelected(null, null);
    }

    void SaveAllStudents()
    {
        string path = Path.Combine(FileSystem.AppDataDirectory, "baza_szkoly.txt");

        List<string> lines = new List<string>();

        foreach (Student s in allStudents)
        {
            string line = s.NumberClass + ";" + s.FirstName + ";" + s.LastName + ";" + s.NumberStudent;
            lines.Add(line);
        }

        File.WriteAllLines(path, lines);
    }
}
