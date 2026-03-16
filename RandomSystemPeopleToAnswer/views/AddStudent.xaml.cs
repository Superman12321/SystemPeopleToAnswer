using RandomSystemPeopleToAnswer.Models;

namespace RandomSystemPeopleToAnswer;

public partial class AddStudent : ContentPage
{
    public AddStudent()
    {
        InitializeComponent();
    }

    private void addStudent(object sender, EventArgs e)
    {
        string newClass = nameClass.Text;
        string newNumber = numberStudentList.Text;

        if (string.IsNullOrWhiteSpace(newClass) ||
            string.IsNullOrWhiteSpace(firstNameStudent.Text) ||
            string.IsNullOrWhiteSpace(lastNameStudent.Text) ||
            string.IsNullOrWhiteSpace(newNumber))
        {
            DisplayAlert("B³¹d", "Wype³nij wszystkie pola!", "OK");
            return;
        }

        if (!int.TryParse(newNumber, out int numberValue))
        {
            DisplayAlert("B³¹d", "Numer ucznia musi byæ liczb¹!", "OK");
            return;
        }

        List<Student> students = LoadStudents();

        foreach (Student s in students)
        {
            if (s.NumberClass == newClass && s.NumberStudent == numberValue)
            {
                DisplayAlert("B³¹d",
                    $"Numer {newNumber} jest ju¿ zajêty przez: {s.FirstName} {s.LastName}",
                    "OK");
                return;
            }
        }

        Student newStudent = new Student();
        newStudent.FirstName = firstNameStudent.Text;
        newStudent.LastName = lastNameStudent.Text;
        newStudent.NumberStudent = numberValue;
        newStudent.NumberClass = newClass;

        AddStudentToFile(newStudent);

        DisplayAlert("OK", "Uczeñ zosta³ zapisany!", "OK");

        firstNameStudent.Text = "";
        lastNameStudent.Text = "";
        numberStudentList.Text = "";
        nameClass.Text = "";
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
    void AddStudentToFile(Student student)
    {
        string path = Path.Combine(FileSystem.AppDataDirectory, "baza_szkoly.txt");
        string line = student.NumberClass + ";" + student.FirstName + ";" + student.LastName + ";" + student.NumberStudent;
        File.AppendAllText(path, line + Environment.NewLine);
    }
}
