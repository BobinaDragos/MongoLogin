using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoLogin
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }
        public class Mongo
        {
            private IMongoDatabase db;
            public Mongo(string database)
            {
                var client = new MongoClient();
                db = client.GetDatabase(database);
            }
            public List<T> LoadRecords<T>(string table)
            {
                var collection = db.GetCollection<T>(table);
                return collection.Find(new BsonDocument()).ToList();
            }
            public void InserRecord<T>(string table, T record)
            {
                var collection = db.GetCollection<T>(table);
                collection.InsertOne(record);
            }
        }
        public class AccountModel
        {
            [BsonId]
            public Guid Id { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            Mongo db = new Mongo("Login");
            var recs = db.LoadRecords<AccountModel>("Users");
            bool dejaCreat = false; //initializare variabila bool pentru verificare
            foreach (var rec in recs) //iterare prin elementele din baza de date
            {
                string numeCont = $"{rec.username}";
                if (textBox1.Text == numeCont) //verificare daca username-ul exista deja
                {
                    dejaCreat = true;
                }
            }
            if (textBox1.Text=="")
            {
                MessageBox.Show("Nu ai introdus un nume de utilizator.");
            }
            else if(textBox2.Text=="")
            {
                MessageBox.Show("Nu ai introdus o parola.");
            }
            else
            {
                if (dejaCreat == false) //daca nu exista deja in baza de date
                {
                    AccountModel account = new AccountModel() //se foloseste accountmodel-ul pentru a modela entry-ul pentru baza de date
                    {
                        username = textBox1.Text, //username
                        password = textBox2.Text, //parola
                    };
                    db.InserRecord("Users", account); //se insereaza in tabela Users modelul de mai sus
                    MessageBox.Show("Contul a fost creat cu succes!"); //afisare mesaj de creare
                    this.Hide();
                    this.Dispose(); //se ascunde si dupa se descarca formularul
                    Form1 loginForm = new Form1(); //initializare forma de login
                    loginForm.Show(); //afisare formular login
                }
                else
                {
                    MessageBox.Show("Username-ul exista deja. Te rog alege un alt nume de utilizator.");
                }
            }
        }

        private void Register_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            this.Dispose();
            Form1 loginForm = new Form1();
            loginForm.Show();
        }
    }
}