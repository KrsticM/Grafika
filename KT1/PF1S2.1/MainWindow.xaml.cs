using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL.SceneGraph;
using SharpGL;

namespace Transformations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///  Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(openGLControl.OpenGL);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta", "GRESKA", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //Iscrtaj svet
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_world.Anim == 0)
            {
                switch (e.Key)
                {

                    case System.Windows.Input.Key.F4: this.Close(); break;
                    case System.Windows.Input.Key.I: m_world.RotationX -= 5.0f; break;
                    case System.Windows.Input.Key.K: m_world.RotationX += 5.0f; break;
                    case System.Windows.Input.Key.J: m_world.RotationY -= 5.0f; break;
                    case System.Windows.Input.Key.L: m_world.RotationY += 5.0f; break;
                    case System.Windows.Input.Key.OemMinus: m_world.Translate -= 1f; break;
                    case System.Windows.Input.Key.OemPlus: m_world.Translate += 1f; break;
                    case System.Windows.Input.Key.V: m_world.animacija(); break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_world.Anim == 0)
            {
                double d;

                if (Double.TryParse(inputText.Text, out d))
                {
                    Console.WriteLine("'{0}' --> {1}", inputText.Text, d);
                    m_world.dbTranslate += (float)d;
                }
                else
                {
                    Console.WriteLine("Unable to parse '{0}'.", inputText.Text);
                }
            }
             
                
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_world.Anim == 0)
            {
                double d = e.NewValue;
                m_world.lbRotate = (float)90.0 + (float)d;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_world.Anim == 0)
            {
                ComboBox cb = (ComboBox)sender;
                ComboBoxItem typeItem = (ComboBoxItem)cb.SelectedItem;
                string value = typeItem.Content.ToString();


                if (value.Equals("Crvena"))
                {
                    //Console.WriteLine(value);
                    m_world.PrvaK = 0.4f;
                    m_world.DrugaK = 0.0f;
                    m_world.TrecaK = 0.0f;
                }
                else if (value.Equals("Plava"))
                {
                    m_world.PrvaK = 0.0f;
                    m_world.DrugaK = 0.0f;
                    m_world.TrecaK = 0.4f;

                }
                else if (value.Equals("Bela"))
                {
                    m_world.PrvaK = 0.4f;
                    m_world.DrugaK = 0.4f;
                    m_world.TrecaK = 0.4f;
                }
            }
        }
    }
}
