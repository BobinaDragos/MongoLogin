using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoLogin
{ 
    public partial class Admin : Form
    {

        public Admin()
        {
            InitializeComponent();
            Populate();
        }
        public class AccountModel
        {
            [BsonId]
            public Guid Id { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }
        public class MongoCrud
        {
            internal IMongoDatabase db;
            public MongoCrud(string database) //initializare client
            {
                var client = new MongoClient();
                db = client.GetDatabase(database);
            }
            
            public List<T> LoadRecords<T>(string table) //Read
            {
                var collection = db.GetCollection<T>(table);
                return collection.Find(new BsonDocument()).ToList();
            }
            public T LoadRecordByUsername<T>(string table, string username) //Query
            {
                var collection = db.GetCollection<T>(table);
                var filter = Builders<T>.Filter.Eq("username", username);

                return collection.Find(filter).First();
            }

            public void DeleteRecord<T>(string table, string username) //Delete
            {
                var collection = db.GetCollection<T>(table);
                var filter = Builders<T>.Filter.Eq("username", username);
                collection.DeleteOne(filter);
            }
            public void UpsertRecord<T>(string table, string username, T record) //Update/Insert in cazul in care nu exista
            {
                var collection = db.GetCollection<T>(table);
                var result = collection.ReplaceOne(
                    new BsonDocument("username", username),
                    record,
                    new UpdateOptions {IsUpsert = true});
            }
  
        }
        public void Populate()
        {
            if (listBox1.Items.Count == 0)
            {
                MongoCrud db = new MongoCrud("Login");
                var recs = db.LoadRecords<Form1.AccountModel>("Users");
                foreach (var rec in recs)
                {
                    var dbname = $"{rec.username}";
                    listBox1.Items.Add(dbname);
                }
            }
            else
            {
                listBox1.Items.Clear();
                MongoCrud db = new MongoCrud("Login");
                var recs = db.LoadRecords<Form1.AccountModel>("Users");
                foreach (var rec in recs)
                {
                    var dbname = $"{rec.username}";
                    listBox1.Items.Add(dbname);
                }
            }
        }
        public static string GetPw(string username)
        {
            MongoCrud db = new MongoCrud("Login");
            var recs = db.LoadRecords<Form1.AccountModel>("Users");
            var pasu="";
            foreach (var rec in recs)
            {
                var usn=$"{rec.username}";
                var pas=$"{rec.password}";
                if (username==usn)
                {
                    pasu = pas;
                }
            }

            return pasu;
        }

        public void UpdatePas(string username,string password)
        {
            MongoCrud db = new MongoCrud("Login");
            var oneRec = db.LoadRecordByUsername<AccountModel>("Users", username);
            oneRec.password = password;
            db.UpsertRecord("Users",username,oneRec);
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtUser.Text = listBox1.SelectedItem.ToString();
            txtParola.Text=GetPw(listBox1.SelectedItem.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var username = txtUser.Text;
            var parola = GetPw(listBox1.SelectedItem.ToString());
            if (txtParola.Text != parola)
            {
                UpdatePas(username, txtParola.Text);
                MessageBox.Show(@"Modificarea a fost salvata cu succes!");
            }
            else
            {
                MessageBox.Show(@"Nu este nicio modificare de facut.");
                
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            MongoCrud db = new MongoCrud("Login");
            db.DeleteRecord<AccountModel>("Users",txtUser.Text);
            Populate();
        }


        private void Admin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}