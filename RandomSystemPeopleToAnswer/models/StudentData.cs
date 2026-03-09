using System.Collections.Generic;

namespace RandomSystemPeopleToAnswer.Models;

public static class StudentData
{
    public static string FilePath
    {
        get
        {
            return Path.Combine(FileSystem.AppDataDirectory, "baza_szkoly.txt");
        }
    }

    public static List<Student> Load()
    {
        List<Student> list = new List<Student>();

        if (!File.Exists(FilePath))
            return list;

        string[] lines = File.ReadAllLines(FilePath);

        foreach (string line in lines)
        {
            string[] parts = line.Split(';');

            if (parts.Length == 4)
            {
                Student s = new Student();
                s.NumberClass = parts[0];
                s.FirstName = parts[1];
                s.LastName = parts[2];
                s.NumberStudent = parts[3];

                list.Add(s);
            }
        }

        return list;
    }

    public static void Save(List<Student> students)
    {
        List<string> lines = new List<string>();

        foreach (Student s in students)
        {
            string line = s.NumberClass + ";" + s.FirstName + ";" + s.LastName + ";" + s.NumberStudent;
            lines.Add(line);
        }

        File.WriteAllLines(FilePath, lines);
    }

    public static void Add(Student student)
    {
        string line = student.NumberClass + ";" + student.FirstName + ";" + student.LastName + ";" + student.NumberStudent;
        File.AppendAllText(FilePath, line + Environment.NewLine);
    }
}
