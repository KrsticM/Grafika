using System;
using System.Collections;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Core;
using System.Diagnostics;
using AssimpSample;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace Transformations
{
    ///<summary> Klasa koja enkapsulira OpenGL programski kod </summary>
    class World
    {
        #region Atributi

        private AssimpScene formula1;
        private AssimpScene formula2;


        private int m_width = 0;
        private int m_height = 0;

        float m_xRotation = 0.0f;
        float m_yRotation = 0.0f;

        float m_Translate = 0.0f;

        float db_translate = 0.0f;
        float lb_rotate = 0.0f;


        float prva_komponenta = 0.2f;
        float druga_komponenta = 0.2f;
        float treca_komponenta = 0.2f;

        private enum TextureObjects { Asfalt = 0, Metal, Sljunak };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        private uint[] m_textures = null;

        private string[] m_textureFiles = { "..//..//images//asfalt.jpg", "..//..//images//metal-seamless-texture.jpg", "..//..//images//sljunak.jpg" };

  
        private DispatcherTimer timer1;

        #endregion


        #region Properties

        public float PrvaK
        {
            get { return prva_komponenta; }
            set { prva_komponenta = value; }
        }

        public float DrugaK
        {
            get { return druga_komponenta; }
            set { druga_komponenta = value; }
        }

        public float TrecaK
        {
            get { return treca_komponenta; }
            set { treca_komponenta = value; }
        }

        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        private int anim = 0;
        public int Anim
        {
            get { return anim; }
            set { anim = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { 
                if(value < 0 || value > 90)
                { return; }

                m_xRotation = value; }
        }

        public float dbTranslate
        {
            get { return db_translate; }
            set { db_translate = value;  }
        }

        public float lbRotate
        {
            get { return lb_rotate; }
            set { lb_rotate = value; }
        }

        public float Translate
        {
            get { return m_Translate; }
            set { m_Translate = value; }
        }

        private float pomerajLeve = 0.0f;
        public float PomerajLeve
        {
            get { return pomerajLeve; }
            set { pomerajLeve = value; }
        }

        private float pomerajDesne = 0.0f;

        public float PomerajDesne
        {
            get { return pomerajDesne; }
            set { pomerajDesne = value; }
        }

        private float pomerajSceneY = 0.0f;

        
        public float PomerajSceneY
        {
            get { return pomerajSceneY; }
            set { pomerajSceneY = value; }
        }

        private float pomerajSceneZ = 0.0f;


        public float PomerajSceneZ
        {
            get { return pomerajSceneZ; }
            set { pomerajSceneZ = value; }
        }

        #endregion


        #region Konstruktori

        /// <summary>
        ///		Konstruktor opengl sveta.
        /// </summary>
        public World(OpenGL gl)
        {
            m_textures = new uint[m_textureCount];
            this.formula1 = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Formula\\Audi"), "Audi_S3.3ds", gl);
            this.formula2 = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Formula\\Mini"), "Car mini N300114.3DS", gl);
            Console.WriteLine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Formula"));
        }

        #endregion

        #region Metode

        public void Initialize(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f); // podesi boju za brisanje ekrana na crnu
            gl.FrontFace(OpenGL.GL_CCW);

            // Osvetljenje
            SetupLighting(gl);

            gl.Enable(OpenGL.GL_NORMALIZE);

            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_SMOOTH);

            // Ukljucivanje ColorTracking mehanizma
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            // Pozivom glColor se definise ambijenta i difuzijalna komponenta
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            //float[] whiteLight = { 1.0f, 1.0f, 1.0f, 1.0f };
            //gl.LightModel(LightModelParameter.Ambient, whiteLight);

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_NORMALIZE);

            formula1.LoadScene();
            formula1.Initialize();
            formula2.LoadScene();
            formula2.Initialize();

            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(1);
            timer1.Tick += new EventHandler(UpdateAnimation1);
            

            // Teksture se primenjuju sa parametrom decal
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL); // GL_DECAL

            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);      // Linear Filtering

                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT); // wrapping
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT); // wrapping


                image.UnlockBits(imageData);
                image.Dispose();
            }
        }

        /// <summary>
        /// Definiše offset kocki
        /// </summary>
        private void UpdateAnimation1(object sender, EventArgs e)
        {
            if (PomerajSceneZ < 1)
            {
                PomerajSceneY += 0.03125f;
                PomerajSceneZ += 0.03125f;
                //Console.WriteLine("Pomeraj po Y: " + PomerajSceneY);
               
            }
            else if (PomerajSceneY > -7f)
            {
                PomerajSceneZ += 0.05f;
                PomerajSceneY -= 0.07f;
            }

            if (PomerajLeve > -8.0f)
                PomerajLeve -= 0.1f;

            if (PomerajDesne > -8.0f)
            {
                PomerajDesne -= 0.05f;
            }
            else
            {
                Anim = 0;

                PomerajDesne = 0f;
                PomerajLeve = 0f;
                PomerajSceneY = 0f;
                PomerajSceneZ = 0f;
                timer1.Stop();
            }

        }

        public void animacija()
        {
            Console.WriteLine("Animacija");
            Anim = 1;
            timer1.Start();



        }

        private void SetupLighting(OpenGL gl)
        {
            float[] global_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            float[] light0pos = new float[] { 0f, 2f, 5f, 1.0f }; // gore levo u odnosu na centar scene
            float[] light0ambient = new float[] { prva_komponenta, druga_komponenta, treca_komponenta, 1.0f };
            float[] light0diffuse = new float[] { 0.4f, 0.4f, 0.0f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.0f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);


            float[] light1pos = new float[] { -0.5f, 3f, 4f, 1.0f }; // iznad levog bolida
            float[] light1ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light1diffuse = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light1specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };
            float[] smer = { 0.0f, -1.0f, 0.0f };

            // Podesi parametre reflektorkskog izvora 
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1specular);

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 45.0f);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);

            float[] light2pos = new float[] { 0.5f, 3f, 4f, 1.0f }; // iznad desnog bolida
            float[] light2ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light2diffuse = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light2specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_POSITION, light2pos);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_AMBIENT, light2ambient);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_DIFFUSE, light2diffuse);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_SPECULAR, light2specular);

            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_SPOT_CUTOFF, 45.0f);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT2);


        }

        public void Resize(OpenGL gl, int width, int height)
        {
            m_height = height;
            m_width = width;
           // Console.WriteLine("Visina: " + height + "  Sirina: " + width);
            gl.Viewport(0, 0, width, height); // kreiraj viewport po celom prozoru
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(50, (float)width / height, 1, 20000);
            gl.LookAt(0f, 2f, 0f, 0f, 0f, -5f, 0f, 1f, 0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        public void Draw(OpenGL gl)
        {
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL); // GL_DECAL
            float[] light0ambient = new float[] { prva_komponenta, druga_komponenta, treca_komponenta, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_AMBIENT, light0ambient);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT);
            gl.Clear(OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Viewport(0, 0, m_width, m_height); // kreiraj viewport po celom prozoru

            //DrawGrid(gl);



                //    Console.WriteLine(m_xRotation);
            gl.PushMatrix();

            if (Anim == 1)
            {
                if(PomerajSceneZ < 1)
                {
                    gl.Translate(0f, PomerajSceneY, PomerajSceneZ);
                }
                else
                {
                    gl.Translate(0f, PomerajSceneY, PomerajSceneZ);
                    gl.Rotate(PomerajSceneZ*7, 1f, 0f, 0f);
                }
                
            }


            gl.Translate(0f, 0f, -8f); // -13
            gl.Translate(0f, 0f, m_Translate);
            gl.Rotate(m_xRotation, 1f, 0f, 0f);
            gl.Rotate(m_yRotation, 0f, 1f, 0f);

            DrawPodloga(gl);

            DrawStaza(gl);

            DrawOgrada(gl);
            DrawOgrada2(gl);


            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD); 
            DrawFormula1(gl);
            DrawFormula2(gl);
            
            DrawTekst(gl);

            gl.PopMatrix();
            gl.Flush();
        }

        private void DrawGrid(OpenGL gl)
        {
            gl.PushMatrix();
            Grid grid = new Grid();
            gl.Translate(0f, -1f, -10f);
            gl.Rotate(90f, 0f, 0f);
            grid.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
            gl.PopMatrix();
        }

        private void DrawPodloga(OpenGL gl)
        {
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();                // resetuj ModelView Matrix
           // gl.PushMatrix();
            gl.Scale(5f, 5f, 5f);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
           // gl.LoadIdentity();

            gl.PushMatrix();
            gl.Translate(0f, -1.001f, 0f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Sljunak]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0f, -1f, 0f);

            gl.Color(1f, 1f, 1f);
            gl.TexCoord(0f, 0f);
            gl.Vertex(-5f, 0f, -5f);
            gl.TexCoord(0f, 1f);
            gl.Vertex(-5f, 0f, 5f);
            gl.TexCoord(1f, 1f);
            gl.Vertex(5f, 0f, 5f);
            gl.TexCoord(1f, 0f);
            gl.Vertex(5f, 0f, -5f);
          
            gl.End();

            gl.PopMatrix();
        }

        private void DrawStaza(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(0f, -1f, 0f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Asfalt]);
            gl.Begin(OpenGL.GL_QUADS);

           
            gl.Normal(0f, 5f, 0f);

            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(1f, 0f, 5f);

            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(1f, 0f, -5f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(-1f, 0f, -5f);
            gl.TexCoord(1.0f, 0f);
            gl.Vertex(-1f, 0f, 5f);
            gl.End();


            gl.PopMatrix();
        }

        private void DrawOgrada(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(-1.1f, -0.9f, 0f);
            gl.Scale(0.1f, 0.1f, 5f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Metal]);
            //gl.Color(1f, 0f, 0f);
            Cube c1 = new Cube();
            c1.Render(gl, RenderMode.Render);
            gl.PopMatrix(); 
        }

        private void DrawOgrada2(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(1.1f, -0.9f, 0f);
            gl.Scale(0.1f, 0.1f, 5f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Metal]);
            // gl.Color(1f, 0f, 0f);
            Cube c1 = new Cube();
            c1.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }

        private void DrawFormula1(OpenGL gl) // Levi bolid
        {
            gl.PushMatrix();

            if(Anim == 1)
            {
                gl.Translate(0f, 0f, PomerajLeve);
            }
            gl.Translate(-0.5f, -1f, 4f ); // -4

            float[] light1pos = new float[] { 0f, 0.9f, 0f, 1.0f }; // iznad levog bolida
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            float[] smer = { 0.0f, -1.0f, 0.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);

            gl.Scale(0.4f, 0.4f, 0.4f);
            gl.Rotate(0.0f, lb_rotate, 0.0f);

           // gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL); // GL_DECAL
            formula1.Draw();
            gl.PopMatrix();
        }

        private void DrawFormula2(OpenGL gl)
        {
            if (Anim == 1)
            {
                gl.Translate(0f, 0f, PomerajDesne);
            }

            gl.PushMatrix();
            gl.Translate(0.5f, -1f, 4f);
            gl.Translate(db_translate, 0f, 0f); // 

            float[] light2pos = new float[] { 0f, 0.9f, 0f, 1.0f }; // iznad desnog bolida
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_POSITION, light2pos);
            float[] smer = { 0.0f, -1.0f, 0.0f };
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_SPOT_DIRECTION, smer);

            gl.Scale(0.003f, 0.003f, 0.003f);
            gl.Rotate(0.0f, -90f, 0f);

           // gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL); // GL_DECAL
            formula2.Draw();
            gl.PopMatrix();
        }

        private void DrawTekst(OpenGL gl)
        {
            gl.Viewport(m_width /2, 0, m_width / 2, m_height);
            
            gl.DrawText(100, 100, 0.0f, 1.0f, 1.0f, "Arial", 14, "Predmet: Racunarska grafika");
            gl.DrawText(100, 100, 0.0f, 1.0f, 1.0f, "Arial", 14, "_______________________");

            gl.DrawText(100, 85, 0.0f, 1.0f, 1.0f, "Arial", 14, "Sk.god: 2018/2019");
            gl.DrawText(100, 85, 0.0f, 1.0f, 1.0f, "Arial", 14, "_______________");

            gl.DrawText(100, 70, 0.0f, 1.0f, 1.0f, "Arial", 14, "Ime: Milos");
            gl.DrawText(100, 70, 0.0f, 1.0f, 1.0f, "Arial", 14, "________");

            gl.DrawText(100, 55, 0.0f, 1.0f, 1.0f, "Arial", 14, "Prezime: Krstic");
            gl.DrawText(100, 55, 0.0f, 1.0f, 1.0f, "Arial", 14, "____________");

            gl.DrawText(100, 40, 0.0f, 1.0f, 1.0f, "Arial", 14, "Sifra zad: 2.1");
            gl.DrawText(100, 40, 0.0f, 1.0f, 1.0f, "Arial", 14, "__________");
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~World()
        {
            this.Dispose(false);
        }

        #endregion

        #region IDisposable Metode

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    //Oslobodi managed resurse
            //}
        }

        #endregion
    }
}
