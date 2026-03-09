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

        List<Student> students = StudentData.Load();

        foreach (Student s in students)
        {
            if (s.NumberClass == newClass && s.NumberStudent == newNumber)
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
        newStudent.NumberStudent = newNumber;
        newStudent.NumberClass = newClass;

        StudentData.Add(newStudent);

        DisplayAlert("OK", "Uczeñ zosta³ zapisany!", "OK");

        firstNameStudent.Text = "";
        lastNameStudent.Text = "";
        numberStudentList.Text = "";
        nameClass.Text = "";
    }
}
