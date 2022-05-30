using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Language_School.DB;
using static Language_School.ClassHelper;
namespace Language_School
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        int numberPage = 0;
        int countClient;

        public MainWindow()
        {
            InitializeComponent();
            LVClientList.ItemsSource = context.Client.ToList();

            List<Gender> genders = context.Gender.ToList();
            genders.Insert(0, new Gender() {GenderName= "Все" });
            cbGender.ItemsSource = genders;
            cbGender.DisplayMemberPath = "GenderName";
            cbGender.SelectedIndex = 0;
            
            cbOrder.ItemsSource = new List<string>
            
            {
                "по умолчанию",
                "фамилии(в алфавитном порядке)",
                "дате последнего посещения(от новых к старым)",
                "количество посещений(от большего к меньшему)"
            };
            cbOrder.SelectedIndex = 0;

            cmPage.ItemsSource = new List<string>
            {
                "Все",
                "10",
                "50",
                "200"
            };
            cmPage.SelectedIndex = 0;
        }

        /// <summary>
        /// Метод для фильтрации и сортировки списка клиентов
        /// </summary>


        ///фильтр по полю
        public void Filter()
        {
            var list = context.Client.ToList();

            list = list.
                Where(i => i.LastName.Contains(tbSearch.Text)
                || i.FirstName.Contains(tbSearch.Text)
                || i.Email.Contains(tbSearch.Text)
                || i.Patronymic.Contains(tbSearch.Text)).ToList();

            if (cbGender.SelectedIndex != 0)
            {
                var gender = cbGender.SelectedItem as Gender;
                list = list.Where(i => i.IDGender == gender.ID).ToList();
            }

            if (CheckBBirth.IsChecked == true)
            {
                list = list.Where(i => i.Birthday.Month == DateTime.Now.Month).ToList();
            }


            ///сортировка

            switch (cbOrder.SelectedIndex)
            {
                case 1:
                    list = list.OrderBy(i => i.LastName).ToList();
                    break;
                case 2:
                    list = list.OrderByDescending(i => i.LastVisit).ToList();
                    break;
                case 3:
                    list = list.OrderBy(i => i.CountVisit).ToList();
                    break;
                default:
                    list = list.OrderBy(i => i.ID).ToList();
                    break;
            }

           

            LVClientList.ItemsSource = list;
           
            //постраничный вывод
            switch (cmPage.SelectedIndex)
            {
                case 0:
                    LVClientList.ItemsSource = list.ToList();
                    break;
                case 1:
                    LVClientList.ItemsSource = list.Skip(numberPage * 10).Take(10).ToList();
                    break;
                case 2:
                    LVClientList.ItemsSource = list.Skip(numberPage * 50).Take(50).ToList();
                    break;
                case 3:
                    LVClientList.ItemsSource = list.Skip(numberPage * 200).Take(200).ToList();
                    break;
            }

        }

        private void cbGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void cbOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVClientList.SelectedItem is Client client)
            {
                var result = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (context.ClientService.Where(i => i.ClientID == client.ID).FirstOrDefault() != null)
                    {
                        MessageBox.Show("У клиента есть информация о посещениях, его удаление из базы данных запрещено","Удаление записи", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        context.Client.Remove(client);
                        context.SaveChanges();
                        MessageBox.Show("Запись успешно удалено", "Удаление записи", MessageBoxButton.OK, MessageBoxImage.Information);
                        Filter();
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            NewUser newUser = new NewUser();
            newUser.ShowDialog();
            this.Close();
        }

        private void cmPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
            
        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            CheckBBirth.IsChecked = false;
            cbGender.SelectedIndex = 0;
            cbOrder.SelectedIndex = 0;
            tbSearch.Clear();
        }

      

        private void CheckBBirth_Checked_1(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void CheckBBirth_Unchecked_1(object sender, RoutedEventArgs e)
        {
            Filter();
        }
    }
}
