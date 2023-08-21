using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using IMetodLayer;
using DataLayer;

namespace DomainLayer
{
    public class DomainMetods : IMetods
    {
        DbConnection dbConnection = new DbConnection();
        string query = "";
        public void Menu()
        {
            Console.WriteLine("----GÜNLÜK----");
            Console.WriteLine("1-Kayıt Ol");
            Console.WriteLine("2-Giriş Yap");
            int cevap = Convert.ToInt32(Console.ReadLine());
            switch (cevap)
            {
                case 1: Record(); break;
                case 2: Login(); break;
                default: Console.WriteLine("HATA TUŞLAMA YAPTIN"); Menu(); break;
            }
        }
        public void Record()
        {
            Console.WriteLine("Kullanıcı adınızı giriniz:");
            string userName = Console.ReadLine();
            Console.WriteLine("Adınızı giriniz:");
            string firstName = Console.ReadLine();
            Console.WriteLine("Soyadınızı giriniz:");
            string lastName = Console.ReadLine();
            Console.WriteLine("Parolanızı giriniz:");
            string password = Console.ReadLine();
            byte[] hashad = Encoding.UTF8.GetBytes(password);
            string hashPassword = Convert.ToBase64String(hashad);
            Console.WriteLine("Güvenlik sorunuzu giriniz:");
            string securityQuestion = Console.ReadLine();
            Console.WriteLine("Güvenlik Cevabınızı giriniz");
            string securityResponse = Console.ReadLine();
            byte[] response = Encoding.UTF8.GetBytes(securityResponse);
            string hasResponse = Convert.ToBase64String(response);
            DbConnection dbConnection = new DbConnection();
            query = "SELECT * FROM Users WHERE UserName=@Username AND Password=@Password";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@UserName", userName);
            sqlCommand.Parameters.AddWithValue("@Password", hashPassword);
            SqlDataReader reader = sqlCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    while (reader.Read())
                    {
                        Console.WriteLine("Bu bilgiler ile kayıtlı kullanıcı mevcut şifreni güncelleme sayfasına yönlendiriliyorsun");
                        ChechSecirty();
                    }
            NewRecord(userName , firstName , lastName , hashPassword , securityQuestion , hasResponse);
        }
        public void NewRecord(string userName , string firstName , string lastName , string hashPassword , string securityQuestion , string hasResponse) 
        {
            DbConnection dbConnection = new DbConnection();
            query = "INSERT INTO Users (UserName,FirstName,LastName,Password,SecurtyQuestion,SecurityResponse) VALUES (@UserName,@FirstName,@LastName,@Password,@SecurtyQuestion,@SecurityResponse)";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@UserName", userName);
            sqlCommand.Parameters.AddWithValue("@FirstName", firstName);
            sqlCommand.Parameters.AddWithValue("@LastName", lastName);
            sqlCommand.Parameters.AddWithValue("@Password", hashPassword);
            sqlCommand.Parameters.AddWithValue("@SecurtyQuestion", securityQuestion);
            sqlCommand.Parameters.AddWithValue("@SecurityResponse", hasResponse);
            sqlCommand.ExecuteNonQuery();
                Console.WriteLine("Kaydınız alındı");
                Login();
            }
        public void Login()
        {
            DbConnection dbConnection = new DbConnection();
            Console.WriteLine("Kullanıcı adınız:?");
            string userName = Console.ReadLine();
            Console.WriteLine("Parolanız:?");
            string password = Console.ReadLine();
            byte[] hashad = Encoding.UTF8.GetBytes(password);
            string hashPassword = Convert.ToBase64String(hashad);
            int Id = 0;
            query = "SELECT Id FROM Users WHERE UserName=@UserName AND Password=@Password";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@UserName", userName);
            sqlCommand.Parameters.AddWithValue("Password", hashPassword);
            SqlDataReader reader = sqlCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            while (reader.Read())
                    {
                        Id = Convert.ToInt32(reader["Id"].ToString());
                    }
                
                if (Id == 0) { Console.Clear(); Console.WriteLine("Kullanıcı adı yada şifre hatalıdır\n Şifre yenileme sayfasına yönelndiriliyorsunuz"); ChechSecirty(); } else Entrance(Id);
            
        }
        public void ChechSecirty()
        {
            DbConnection dbConnection = new DbConnection();
            int Id = 0;
            Console.WriteLine("Güvenlik sorunuz nedir?");
            string securityQuestion = Console.ReadLine();
            Console.WriteLine("Güvenlik Cevabın nedir?");
            string securtyResponse = Console.ReadLine();
            byte[] response = Encoding.UTF8.GetBytes(securtyResponse);
            string hashResponse = Convert.ToBase64String(response);
            query = "SELECT * FROM Users WHERE SecurtyQuestion=@SecurtyQuestion AND SecurityResponse=@SecurityResponse";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@SecurtyQuestion", securityQuestion);
            sqlCommand.Parameters.AddWithValue("@SecurityResponse", hashResponse);
            SqlDataReader reader = sqlCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            while (reader.Read())
                    {
                        Id = Convert.ToInt32(reader["Id"].ToString());
                    }
                
            
            if (Id == 0) { Console.WriteLine("Sisteme Kayıtlı böyle bir kullanıcı yok kayıt olmak için yönelendiriliyorsun"); Record(); }
            else UpdatePassword(Id);
        }
        public void UpdatePassword(int Id)
        {
            DbConnection dbConnection = new DbConnection();
            Console.WriteLine("Yeni parolanı gir");
            string newPassword = Console.ReadLine();
            byte[] password = Encoding.UTF8.GetBytes(newPassword);
            string hashPassword = Convert.ToBase64String(password);
            query = "UPDATE Users SET Password=@Password WHERE Id=@Id";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@Id", Id);
            sqlCommand.Parameters.AddWithValue("@Password", hashPassword);
            sqlCommand.ExecuteNonQuery();
                Console.WriteLine("PAROLA GÜNCELLENDİ");
                Login();
            
        }
        public void Entrance(int Id)
        {
            Console.WriteLine("1-Yeni kayıt ekle");
            Console.WriteLine("2-Kayıtlarını listele");
            Console.WriteLine("3-Tüm kayıtlarını sil");
            Console.WriteLine("4-Çıkış Yap");
            int cevap = Convert.ToInt32(Console.ReadLine());
            switch (cevap)
            {
                case 1: NewRegister(Id); break;
                case 2: AllRegister(Id); break;
                case 3:
                    Console.WriteLine("Emin misiniz?");
                    Console.WriteLine("1-EVET");
                    Console.WriteLine("2-HAYIR");
                    int sorgu = Convert.ToInt32(Console.ReadLine());
                    if (sorgu == 1) DeleteAllData(Id); else Entrance(Id);
                    break;
                case 4: ExitProgram(); break;
            }
        }
        public void NewRegister(int Id)
        {
            DbConnection dbConnection = new DbConnection();
            int a = 0;
            List<DateTime> dateTime1 = new List<DateTime>();
            query = "SELECT Time FROM Diarys WHERE UserId=@UserId";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@UserId", Id);
            SqlDataReader reader = sqlCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            while (reader.Read())
                    {
                        a++;
                        dateTime1.Add(Convert.ToDateTime(reader["Time"].ToString()));
                    }
                
                if (a != 0)
                {

                    for (int i = 0; i < dateTime1.Count; i++)
                    {
                        string dateTime2 = dateTime1[i].ToString();
                        string dateTime3 = DateTime.Today.ToString();
                        bool sorgu = dateTime2.StartsWith(dateTime3.Substring(0, 10));
                        if (sorgu)
                        {
                            Console.Clear();
                            Console.WriteLine("Bugün günlük yazdın yine de eklemek ister misin");
                            Console.WriteLine("1-EVET");
                            Console.WriteLine("2-HAYIR");
                            int cevap = Convert.ToInt32(Console.ReadLine());
                            switch (cevap)
                            {
                                case 1: ReadNew(Id); break;
                                case 2: Entrance(Id); break;
                                default: Console.WriteLine("Yanlış Tuşladın Yeniden dene "); NewRegister(Id); break;
                            }
                        }
                    }
                }
                else ReadNew(Id);
            
        }
        public void ReadNew(int Id)
        {
            DbConnection dbConnection = new DbConnection();
            Console.WriteLine("Notunu Yaz");
            string note = Console.ReadLine();
            byte[] not = Encoding.UTF8.GetBytes(note);
            string hashData = Convert.ToBase64String(not);
            DateTime dateTime = DateTime.Now;
            query = "INSERT INTO Diarys (Time , Note , UserId) VALUES (@Time , @Note ,@UserId)";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@Note", hashData);
            sqlCommand.Parameters.AddWithValue("@Time", dateTime);
            sqlCommand.Parameters.AddWithValue("@UserId", Id);
            sqlCommand.ExecuteNonQuery();          
            //KT
            Console.WriteLine("Yeni kayıt eklendi..");
            Entrance(Id);
        }
        public void AllRegister(int userId)
        {

            DbConnection dbConnection = new DbConnection();
            List<string> note = new List<string>();
            List<DateTime> dateTime = new List<DateTime>();
            List<int> Id = new List<int>();
            int a = 0;
            query = "SELECT * FROM Diarys WHERE UserId=@UserId";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader reader = sqlCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            while (reader.Read())
                    {
                        a++;
                        dateTime.Add(Convert.ToDateTime(reader["Time"].ToString()));
                        note.Add(reader["Note"].ToString());
                        Id.Add(Convert.ToInt32(reader["Id"].ToString()));
                    }
                
                Console.Clear();
                string veri = "";
                Console.WriteLine("1-Tüm kayıtları listele");
                Console.WriteLine("2-Kayıt ara");
                int liste = Convert.ToInt32(Console.ReadLine());
                switch (liste)
                {
                    case 1:
                        for (int i = 0; i < a;)
                        {
                            if (a != 0)
                            {
                                byte[] not = Convert.FromBase64String(note[i]);
                            veri = Encoding.UTF8.GetString(not);
                                Console.WriteLine("--------------------------");
                                Console.WriteLine("Kayıt zamanı: " + dateTime[i]);
                                Console.WriteLine("Günlük yazın: " + veri + "\n--------------------------");
                                Console.WriteLine("1-Sonraki Kayıda git  \n2-Önceki kayda git \n3-Kaydı güncelle\n4-Kaydı sil\n5-Çıkış yap");
                                int cevap = Convert.ToInt32(Console.ReadLine());
                                switch (cevap)
                                {
                                    case 1:
                                        if (i < a - 1) i++;
                                        else Console.Clear(); Console.WriteLine("Son kayıttasın\n");
                                        break;
                                    case 2: if (i > 0) i--; else Console.Clear(); Console.WriteLine("İlk kayıttasın\n"); break;
                                    case 3: Update(Id[i], userId); break;
                                    case 4: Delete(Id[i]); break;
                                    case 5: ExitProgram(); break;
                                    default: Console.Clear(); Entrance(userId); break;
                                }
                            }
                        }
                        if (a == 0)
                            Console.WriteLine("Hiç kaydın yok yeni kayıt ekle."); NewRegister(userId);
                        break;
                    case 2:
                        Console.WriteLine("Hangi güne ait kayıt arıyorsunuz?");
                        int day = Convert.ToInt32(Console.ReadLine());
                    List(day, userId);
                        break;
                }

            
        }
        public void List(int day, int userId)
        {
            DbConnection dbConnection = new DbConnection();
            int a = 0;
            List<DateTime> time = new List<DateTime>();
            List<string> note = new List<string>();
            List<int> noteId = new List<int>();
            query = "SELECT * FROM Diarys WHERE UserId=@UserId";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader reader = sqlCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            while (reader.Read())
                    {
                        a++;
                        time.Add(Convert.ToDateTime(reader["Time"].ToString()));
                        note.Add(reader["Note"].ToString());
                        noteId.Add(Convert.ToInt32(reader["Id"].ToString()));
                    }                        
            if (a != 0)
            {
                for (int i = 0; i < a; i++)
                {
                    string dateTime2 = time[i].ToString();
                    string dateTime3 = day.ToString();
                    bool sorgu = dateTime2.StartsWith(dateTime3.Substring(0, 2));
                    if (sorgu)
                    {
                        Console.WriteLine("--------------------------");
                        Console.WriteLine("Kayıt zamanı: " + time[i]);
                        Console.WriteLine("Günlük yazın: " + note[i] + "\n--------------------------");
                    }
                }
            }
            else Console.WriteLine("Eşleşen kayıt yok");
        }
        public void ExitProgram()
        {
            Console.WriteLine("Exiting the program.");
            Environment.Exit(0);
        }
        public void Delete(int Id)
        {
            DbConnection dbConnection = new DbConnection();
            query = "DELETE FROM Diarys WHERE Id=@Id";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@Id", Id);
            sqlCommand.ExecuteNonQuery();
                Console.Clear();
                Console.WriteLine("Kaydın silindi.");
            Entrance(Id);
            

        }
        public void Update(int Id, int userId)
        {
            DbConnection dbConnection = new DbConnection();
            Console.WriteLine("Yeni notunu yaz:");
            string note = Console.ReadLine();
            byte[] not = Encoding.UTF8.GetBytes(note);
            string hashData = Convert.ToBase64String(not);
            DateTime time = DateTime.Now;
            query = "UPDATE Diarys SET Note=@Note,Time=@Time WHERE Id=@Id";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@Note", hashData);
            sqlCommand.Parameters.AddWithValue("@Time", time);
            sqlCommand.Parameters.AddWithValue("@Id", Id);
            sqlCommand.ExecuteNonQuery();
            AllRegister(userId);
            
        }
        public void DeleteAllData(int Id)
        {
            DbConnection dbConnection = new DbConnection();
            query = "DELETE FROM Diarys WHERE UserId=@UserId";
            SqlCommand sqlCommand = dbConnection.sqlCommand(query);
            sqlCommand.Parameters.AddWithValue("@UserId", Id);
            sqlCommand.ExecuteNonQuery();
            Console.Clear();
            Console.WriteLine("Tüm kayıtlar silindi.");
            Entrance(Id);

        }
    }
}
