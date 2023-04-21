using System;
using wp08_personalInfoApp.Logics;

namespace wp08_personalInfoApp.Models
{
    internal class Person
    {
        // 외부에서 접근불가 private
       
        private string email;
        private DateTime date;

        public string FirstName { get; set ; }    // Auto property
        public string LastName { get; set; }
        public string Email
        { 
            get => email;
            set 
            {
                if (Commons.IsValidEmail(value) != true)    // 이메일은 형식에 일치하지않음
                {
                    throw new Exception("유효하지 않은 이메일형식");
                }
                else
                {
                    email = value;
                }
            }
        }
        public DateTime Date 
        { 
            get => date; 
            set
            {
                var result = Commons.GetAge(value);
                if (result > 120 || result <= 0)
                {
                    throw new Exception("유효하지 않은 생일");
                }
                else
                {
                    date = value;
                }
            }
        }

        public bool IsAdult
        {
            get
            {
                return Commons.GetAge(date) > 18;   // 19살 이상이면 true
            }
        }

        public bool IsBirthDay
        {
            get
            {
                return DateTime.Now.Month == date.Month &&
                    DateTime.Now.Day == date.Day;   // 오늘과 월 일이 같으면 생일
            }
        }

        public string Zodiac
        {
            get => Commons.GetZodiac(date);     // 12지로 띠를 받아옴 (람다식으로 표현)
        }

        public Person(string firstName, string lastName, string email, DateTime date)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Date = date;
        }
    }
}
