using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoLogin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class AccountModel //modelare date din baza de date
        {
            [BsonId]
            public Guid Id { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }

        public class Mongo
        {
            private IMongoDatabase db;

            public Mongo(string database)
            {
                var client = new MongoClient(); //initializare client mongoDB
                db = client.GetDatabase(database); //preluare baza de date
            }
            public List<T> LoadRecords<T>(string table) //incarcare records din tabelul din baza de date
            {
                var collection = db.GetCollection<T>(table); //preluare colectie cu intrari
                return collection.Find(new BsonDocument()).ToList(); //returnare document Bson sub forma de lista
            }
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            Mongo db = new Mongo("Login"); //daca nu exista, baza de date e creata
            var recs = db.LoadRecords<AccountModel>("Users"); //incarcare records din table-ul Users
            bool corect = false; //initializare variabila
            foreach (var rec in recs) //iterare prin records din baza de date
            {
                var dbname =$"{rec.username}"; //stocare username in variabila
                var dbpw = $"{rec.password}"; //stocare parola in variabila
                if (txtUser.Text== dbname && txtParola.Text == dbpw) //daca username si parola sunt in baza de date
                {
                    corect = true; //operatiunea s-a realizat cu succes
                    Admin adminForm = new Admin(); //instantiere Admin form
                    this.Hide(); //ascundere form login
                    adminForm.Show(); //deschidere Admin form
                    break; //incheiere iterare foreach
                }

            }

            if (txtUser.Text=="") //verificare campuri nule
            {
                MessageBox.Show("Nu ai introdus un nume de utilizator.");
            }
            else if(txtParola.Text=="")
            {
                MessageBox.Show("Nu ai introdus o parola.");
            }
            else
            {
                if (corect == false)
                {
                    MessageBox.Show("Contul nu exista. Te rog sa incerci din nou.");
                }
            }
        }

        
        private void btnRegister_Click(object sender, EventArgs e) //afisare form de inregistrare
        {
            Register regForm = new Register(); 
            this.Hide();
            regForm.Show();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide(); //ascundere form Login
            this.Dispose(); //eliberarea resurselor folosite de form
        }
    }
}